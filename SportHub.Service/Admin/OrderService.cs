using System.Globalization;

using Microsoft.EntityFrameworkCore;

using SportHub.Core;
using SportHub.Domain.Admin.Request.Order;
using SportHub.Domain.Client.IM.Response;
using SportHub.Repository.Model;

using Zero.Core.Result;

namespace SportHub.Service.Admin;

public class OrderService(IServiceProvider provider) : BaseService(provider)
{
    public async Task<SysResult<bool>> SyncOrderAsync(SyncOrderRequest request)
    {
        if (request == null)
        {
            return Result(false, ErrorCode.USER_CUSTOM, "请求参数为空");
        }

        var orderId = request.OrderId?.Trim();
        if (string.IsNullOrWhiteSpace(orderId))
        {
            return Result(false, ErrorCode.USER_CUSTOM, "orderid 不能为空");
        }

        var apiKey = await ConfigService.GetValueAsync(GlobalHash.IMApiKey);
        if (string.IsNullOrWhiteSpace(apiKey))
        {
            return Result(false, ErrorCode.USER_CUSTOM, "IMApiKey 配置不存在");
        }

        var startTime = DateTime.UtcNow.AddDays(-30);
        var endTime = DateTime.UtcNow;
        var page = request.Page <= 0 ? 1 : request.Page;
        var pageSize = request.PageSize <= 0 ? 100 : Math.Min(request.PageSize, 500);

        var queryResult = await IMClient.GetOrderQueryAsync(apiKey, startTime, endTime, request.Settled, page, pageSize, orderId, request.UserId, request.BetType);
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

            order.LegCount = firstRow.LegCount ?? group.Count();
            order.Stake = ParseDecimal(firstRow.Stake);
            order.Odds = ParseDecimal(firstRow.OrderOdds);
            order.PotentialPayout = ParseDecimal(firstRow.PotentialReturn);
            order.ActualPayout = ParseDecimal(firstRow.Payout);
            order.OrderSettleCode = firstRow.SettleStatus;
            order.OrderCode = ParseMarketCode(firstRow.BetType);
            order.UpdatedTime = now;

            var existingLegs = await DB.OrderLeg.Where(x => x.OrderId == order.Id).ToListAsync();
            if (existingLegs.Count > 0)
            {
                DB.OrderLeg.RemoveRange(existingLegs);
            }

            var legIndex = 1;
            foreach (var row in group.OrderBy(x => x.LegNo ?? 0))
            {
                var orderLeg = new OrderLeg
                {
                    Id = Snowflake.NewID(),
                    OrderId = order.Id,
                    LegIndex = row.LegNo ?? legIndex,
                    MatchId = row.MatchId,
                    MatchStartTime = ParseTimeSeconds(row.MatchTime),
                    MarketCode = ParseMarketCode(row.BetType),
                    SelectionCode = ParseSelectionCode(row.Side),
                    BetOdds = ParseDecimal(row.LegOdds) ?? ParseDecimal(row.OrderOdds),
                    SettledOdds = ParseDecimal(row.OrderOdds),
                    LegSettleCode = row.SettleStatus,
                    SourceNo = ParseLong(row.Orderid),
                    HandicapValue = ParseDecimal(row.LineDisplay),
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

    private static long? ParseTimeSeconds(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        if (DateTime.TryParseExact(value, "yyyy-MM-dd HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var dt))
        {
            return new DateTimeOffset(dt, TimeSpan.FromHours(9)).ToUnixTimeSeconds();
        }

        if (DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dt))
        {
            return new DateTimeOffset(dt, TimeSpan.FromHours(9)).ToUnixTimeSeconds();
        }

        return null;
    }

    private static long? ParseMarketCode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim().ToLowerInvariant() switch
        {
            "1x2" => 1,
            "ah" => 2,
            "ou" => 3,
            "parlay" => 4,
            _ => null
        };
    }

    private static long? ParseSelectionCode(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return null;
        }

        return value.Trim().ToLowerInvariant() switch
        {
            "home" => 1,
            "away" => 2,
            "draw" => 3,
            "over" => 4,
            "under" => 5,
            _ => null
        };
    }
}
