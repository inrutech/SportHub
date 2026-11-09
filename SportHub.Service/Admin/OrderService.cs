using System.Globalization;

using Microsoft.EntityFrameworkCore;

using SportHub.Core;
using SportHub.Domain.Admin.Request.Order;
using SportHub.Domain.Client.IM.Response;
using SportHub.Repository.Model;

using Zero.Core.Extensions;
using Zero.Core.Result;

namespace SportHub.Service.Admin;

public class OrderService(IServiceProvider provider) : BaseService(provider)
{
    public async Task<SysResult<bool>> SyncOrderAsync(SyncOrderRequest request)
    {
        var apiKey = await ConfigService.GetValueAsync(GlobalHash.IMApiKey);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return Result(false, ErrorCode.USER_CUSTOM, "IMApiKey 配置不存在");
        }

        var startTime = new DateTime(2026, 7, 20);
        var endTime = new DateTime(2026, 7, 26);

        var queryResult = await IMClient.GetOrderQueryAsync(apiKey, startTime, endTime, request.OrderId, 1);
        if (queryResult.Code != ErrorCode.SYS_SUCCESS || queryResult.Result?.Code != 200)
        {
            return Result(false, ErrorCode.SYS_FAIL, $"同步失败：订单查询接口返回异常。{queryResult.ErrorDesc}");
        }

        var rows = queryResult.Result.Data?.Rows ?? [];
        if (rows.Count == 0)
        {
            return Result(true);
        }

        var groupedRows = rows
            .Where(x => !string.IsNullOrWhiteSpace(x.Orderid))
            .GroupBy(x => x.Orderid)
            .ToList();

        foreach (var group in groupedRows)
        {
            var firstRow = group.First();
            var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            var order = await DB.Order.FirstOrDefaultAsync(x => x.SourceNo == firstRow.Orderid);
            if (order == null)
            {
                order = new Order
                {
                    Id = Snowflake.NewID(),
                    SourceNo = firstRow.Orderid,
                    Flag = 0,
                    CreatedTime = now
                };
                await DB.Order.AddAsync(order);
            }

            order.LegCount = firstRow.LegCount;
            order.Stake = ParseDecimal(firstRow.Stake);
            order.Odds = ParseDecimal(firstRow.OrderOdds);
            order.PotentialPayout = ParseDecimal(firstRow.PotentialReturn);
            order.ActualPayout = ParseDecimal(firstRow.Payout);
            order.OrderSettleCode = ParseOrderSettleCode(firstRow.MoneyStatus, firstRow.SettleStatus);
            order.OrderCode = (long)ParseOrderCode(firstRow.IsParlay, firstRow.IsRolling);
            order.UpdatedTime = now;

            var existingLegs = await DB.OrderLeg.Where(x => x.OrderId == order.Id).ToListAsync();
            if (existingLegs.Count > 0)
            {
                DB.OrderLeg.RemoveRange(existingLegs);
            }

            var legIndex = 1;
            foreach (var row in group.OrderBy(x => x.LegNo))
            {
                var leagueId = await ResolveLeagueIdAsync(row.LeagueChs);
                var homeTeamId = await EnsureTeamAsync(row.HomeChs, leagueId);
                var awayTeamId = await EnsureTeamAsync(row.AwayChs, leagueId);

                if (row.MatchId > 0)
                {
                    await EnsureMatchAsync(row.MatchId, leagueId, homeTeamId, awayTeamId);
                }

                var orderLeg = new OrderLeg
                {
                    Id = Snowflake.NewID(),
                    OrderId = order.Id,
                    LeagueId = leagueId,
                    HomeTeamId = homeTeamId,
                    AwayTeamId = awayTeamId,
                    LegIndex = row.LegNo,
                    MatchId = row.MatchId,
                    MatchTime = ParseTimeSeconds(row.MatchTime) ?? ParseTimeSeconds(row.BetTime) ?? ParseUnixSeconds(row.BetTs),
                    PeriodCode = ParsePeriodCode(row.Period),
                    MarketCode = ParseMarketCode(row.BetType),
                    SelectionCode = ParseSelectionCode(row.Side),
                    BetOdds = ParseDecimal(row.LegOdds) ?? ParseDecimal(row.OrderOdds),
                    SettledOdds = ParseDecimal(row.OrderOdds),
                    LegSettleCode = ParseLegSettleCode(row.SettleStatus),
                    SourceNo = ParseLong(row.Orderid),
                    HandicapValue = ParseHandicapValue(row.SelectionDisplay),
                    Describe = row.ToJson(),
                    Flag = 0,
                    CreatedTime = now,
                    UpdatedTime = now
                };

                await DB.OrderLeg.AddAsync(orderLeg);
                legIndex++;
            }
        }

        await DB.SaveChangesAsync();
        return Result(true);
    }

    private async Task<long?> ResolveLeagueIdAsync(string? leagueName)
    {
        if (string.IsNullOrWhiteSpace(leagueName))
        {
            return null;
        }

        var name = leagueName.Trim();
        var league = await DB.League.FirstOrDefaultAsync(x =>
            (!string.IsNullOrWhiteSpace(x.NameZhCn) && x.NameZhCn == name) ||
            (!string.IsNullOrWhiteSpace(x.NameZhTw) && x.NameZhTw == name) ||
            (!string.IsNullOrWhiteSpace(x.NameEn) && x.NameEn == name) ||
            (!string.IsNullOrWhiteSpace(x.ShortNameZhCn) && x.ShortNameZhCn == name) ||
            (!string.IsNullOrWhiteSpace(x.ShortNameZhTw) && x.ShortNameZhTw == name) ||
            (!string.IsNullOrWhiteSpace(x.ShortNameEn) && x.ShortNameEn == name));

        return league?.Id;
    }

    private async Task<long?> EnsureTeamAsync(string? teamName, long? leagueId)
    {
        if (string.IsNullOrWhiteSpace(teamName))
        {
            return null;
        }

        var name = teamName.Trim();
        var team = await DB.Team.FirstOrDefaultAsync(x =>
            x.LeagueId == leagueId &&
            ((x.NameZhCn != null && x.NameZhCn == name) ||
             (x.NameZhTw != null && x.NameZhTw == name) ||
             (x.NameEn != null && x.NameEn == name)));

        if (team != null)
        {
            return team.Id;
        }

        var createdAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        var newTeam = new Team
        {
            Id = Snowflake.NewID(),
            LeagueId = leagueId,
            NameEn = name,
            NameZhCn = name,
            NameZhTw = name,
            Flag = 0,
            CreatedTime = createdAt,
            UpdatedTime = createdAt
        };

        await DB.Team.AddAsync(newTeam);
        return newTeam.Id;
    }

    private async Task EnsureMatchAsync(long matchId, long? leagueId, long? homeTeamId, long? awayTeamId)
    {
        var match = await DB.Match.FirstOrDefaultAsync(x => x.Id == matchId);
        if (match != null)
        {
            match.LeagueId ??= leagueId;
            match.HomeTeamId ??= homeTeamId;
            match.AwayTeamId ??= awayTeamId;
            match.UpdatedTime = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            return;
        }

        var createdAt = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        await DB.Match.AddAsync(new Match
        {
            Id = matchId,
            LeagueId = leagueId,
            HomeTeamId = homeTeamId,
            AwayTeamId = awayTeamId,
            Flag = 0,
            CreatedTime = createdAt,
            UpdatedTime = createdAt
        });
    }

    private static long? ParseLong(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return long.TryParse(value, out var result) ? result : null;
    }

    private static decimal? ParseDecimal(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return decimal.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out var result) ? result : null;
    }

    private static string? ParseHandicapValue(string? selectionDisplay)
    {
        return ParseHandicapFromSelectionDisplay(selectionDisplay);
    }

    private static string? ParseHandicapFromSelectionDisplay(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var text = value.Trim();
        // Example: "Over 3.5 @ 2.04" -> 3.5
        var body = System.Text.RegularExpressions.Regex.Replace(text, @"\s@\s*[-+]?\d+(?:\.\d+)?\s*(EU)?\s*$", string.Empty, System.Text.RegularExpressions.RegexOptions.IgnoreCase).Trim();

        var splitMatch = System.Text.RegularExpressions.Regex.Match(body, @"([+-]?\d+(?:\.\d+)?)/([+-]?\d+(?:\.\d+)?)");
        if (splitMatch.Success)
        {
            return $"{decimal.Parse(splitMatch.Groups[1].Value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture)}/{decimal.Parse(splitMatch.Groups[2].Value, CultureInfo.InvariantCulture).ToString(CultureInfo.InvariantCulture)}";
        }

        var numberMatch = System.Text.RegularExpressions.Regex.Match(body, @"[+-]?\d+(?:\.\d+)?");
        if (numberMatch.Success && decimal.TryParse(numberMatch.Value, NumberStyles.Any, CultureInfo.InvariantCulture, out var number))
        {
            return number.ToString(CultureInfo.InvariantCulture);
        }

        return null;
    }

    private static long? ParseUnixSeconds(long value)
    {
        return value > 0 ? value : null;
    }

    private static long? ParseTimeSeconds(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var text = value.Trim();
        var formats = new[]
        {
            "yyyy-MM-dd HH:mm",
            "yyyy-MM-dd HH:mm:ss",
            "yyyy/MM/dd HH:mm",
            "yyyy/MM/dd HH:mm:ss",
            "yyyy-MM-ddTHH:mm:ss",
            "yyyy-MM-ddTHH:mm:ssZ",
            "yyyy-MM-ddTHH:mm:ss.fffZ",
        };

        if (DateTime.TryParseExact(text, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
        {
            var unspecified = DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
            return new DateTimeOffset(unspecified, TimeSpan.FromHours(9)).ToUnixTimeSeconds();
        }

        if (DateTimeOffset.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out var dto))
        {
            return dto.ToUnixTimeSeconds();
        }

        if (DateTime.TryParse(text, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces, out dt))
        {
            var unspecified = DateTime.SpecifyKind(dt, DateTimeKind.Unspecified);
            return new DateTimeOffset(unspecified, TimeSpan.FromHours(9)).ToUnixTimeSeconds();
        }

        return null;
    }

    private static long? ParseMarketCode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var marketCode = value.Trim().ToLowerInvariant() switch
        {
            "1x2" => MarketCode.OneXTwo,
            "ah" => MarketCode.AsianHandicap,
            "ou" => MarketCode.OverUnder,
            "oe" => MarketCode.OddEven,
            "goals" => MarketCode.TotalGoals,
            "bodan" => MarketCode.CorrectScore,
            "half_full" => MarketCode.HalfFullTime,
            "goal_order" => MarketCode.GoalOrder,
            "dc" => MarketCode.DoubleChance,
            "corner_ou" => MarketCode.CornerOverUnder,
            "corner_other" => MarketCode.CornerOther,
            "corner_oe" => MarketCode.CornerOddEven,
            "oe_half" => MarketCode.HalfOddEven,
            "tg_half" => MarketCode.HalfTotalGoals,
            "overtime" => MarketCode.Overtime,
            "champion" => MarketCode.Champion,
            "parlay" => MarketCode.Parlay,
            _ => MarketCode.None
        };

        return marketCode == MarketCode.None ? null : (long)marketCode;
    }

    private static OrderCode ParseOrderCode(long? isParlay, long? isRolling)
    {
        if (isParlay == 1)
        {
            return OrderCode.Parlay;
        }

        if (isRolling == 1)
        {
            return OrderCode.Rolling;
        }

        return OrderCode.PreMatch;
    }

    private static long? ParseOrderSettleCode(long? moneyStatus, long? settleStatus)
    {
        if (moneyStatus == 1)
        {
            return (long)OrderSettleCode.Settled;
        }

        if (settleStatus == 0)
        {
            return (long)OrderSettleCode.Unsettled;
        }

        if (settleStatus is >= 1 and <= 6)
        {
            return (long)OrderSettleCode.Settled;
        }

        return null;
    }

    private static long? ParseLegSettleCode(long? settleStatus)
    {
        return settleStatus switch
        {
            0 => (long)LegSettleCode.Pending,
            1 => (long)LegSettleCode.Won,
            2 => (long)LegSettleCode.Lost,
            3 => (long)LegSettleCode.Push,
            4 => (long)LegSettleCode.HalfWon,
            5 => (long)LegSettleCode.HalfLost,
            _ => null
        };
    }

    private static long? ParsePeriodCode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var periodCode = value.Trim().ToLowerInvariant() switch
        {
            "half" or "half_time" or "half-time" => PeriodCode.HalfTime,
            "full" or "full_time" or "full-time" => PeriodCode.FullTime,
            _ => PeriodCode.None
        };

        return periodCode == PeriodCode.None ? null : (long)periodCode;
    }

    private static long? ParseSelectionCode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        var selectionCode = value.Trim().ToLowerInvariant() switch
        {
            "home" => SelectionCode.Home,
            "away" => SelectionCode.Away,
            "draw" => SelectionCode.Draw,
            "over" => SelectionCode.Over,
            "under" => SelectionCode.Under,
            _ => SelectionCode.None
        };

        return selectionCode == SelectionCode.None ? null : (long)selectionCode;
    }
}
