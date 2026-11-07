using Microsoft.EntityFrameworkCore;

using SportHub.Core;
using SportHub.Domain.Admin.Request.Order;
using SportHub.Repository.Model;

using Zero.Core.Result;

namespace SportHub.Service.Admin;

public class OrderService(IServiceProvider provider) : BaseService(provider)
{
    public async Task<SysResult<bool>> SyncOrderAsync(SyncOrderRequest request)
    {
        var token = await ConfigService.GetValueAsync(GlobalHash.LoginToken);
        var detailResult = await IMClient.GetOrderDetailAsync(token, request.OrderId, request.MatchId);
        if (detailResult.OccurError || detailResult.Result?.Code != 200)
        {
            return Result(false, ErrorCode.SYS_FAIL, $"同步失败：订单详情请求失败。{detailResult.ErrorDesc}");
        }

        var info = detailResult.Result.Info[0];
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

        var order = await DB.Order.FirstOrDefaultAsync(x => x.SourceNo == info.Orderid);
        if (order == null)
        {
            order = new Order
            {
                Id = Snowflake.NewID(),
                SourceNo = info.Orderid,
                Flag = 0,
                CreatedTime = now
            };
            await DB.Order.AddAsync(order);
        }

        order.OrderCode = ParseLong(info.BetType);
        order.LegCount = detailResult.Result.Schedule?.Count ?? 0;
        order.Stake = ParseDecimal(info.Stake);
        order.Odds = ParseDecimal(info.Odds);
        order.PotentialPayout = ParseDecimal(info.PotentialReturn);
        order.ActualPayout = ParseDecimal(info.Payout);
        order.OrderSettleCode = ParseLong(info.SettleStatus);
        order.UpdatedTime = now;

        var existingLegs = await DB.OrderLeg
            .Where(x => x.OrderId == order.Id)
            .ToListAsync();

        foreach (var leg in existingLegs)
        {
            DB.OrderLeg.Remove(leg);
        }

        if (detailResult.Result.Schedule != null)
        {
            var legIndex = 1;
            foreach (var schedule in detailResult.Result.Schedule)
            {
                var orderLeg = new OrderLeg
                {
                    Id = Snowflake.NewID(),
                    OrderId = order.Id,
                    LegIndex = legIndex++,
                    LeagueId = ParseLong(schedule.LeagueChsShort),
                    MatchId = ParseLong(schedule.MatchId),
                    MatchStartTime = ParseLong(schedule.MatchTime),
                    HomeTeamId = ParseLong(schedule.HomeChs),
                    AwayTeamId = ParseLong(schedule.AwayChs),
                    FullTimeScore = ParseLong(schedule.HomeScore) is null ? null : ParseLong(schedule.HomeScore),
                    // FullTimeScore 历史值使用成对分值，仅作为可用字段落库
                    // 其余结算/玩法字段在当前响应中无法精确对应时按默认值保留。
                    Flag = 0,
                    CreatedTime = now,
                    UpdatedTime = now
                };

                var scoreHome = ParseLong(schedule.HomeScore);
                var scoreAway = ParseLong(schedule.AwayScore);
                orderLeg.FullTimeScore = scoreHome;
                orderLeg.BetScore = scoreHome + scoreAway;
                orderLeg.LegSettleCode = ParseLong(info.SettleStatus);
                orderLeg.SourceNo = ParseLong(info.Id);

                await DB.OrderLeg.AddAsync(orderLeg);
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

        return decimal.TryParse(value, out var result) ? result : null;
    }
}
