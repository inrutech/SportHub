using SportHub.Core;
using SportHub.Core.Code;
using SportHub.Domain.Model;
using SportHub.Repository.Model;

using Microsoft.EntityFrameworkCore;

using Zero.Core.Extensions;

namespace SportHub.Service.Common
{
    public class OrderLegService(IServiceProvider provider) : BaseService(provider)
    {
        public Task<List<Repository.Model.OrderLeg>> CacheGetOrderLegListAsync()
        {
            var key = WebCache.RenderKey(Params.APP_KEY, "OrderLegList");
            return WebCache.Get(key, async () => await DB.OrderLeg.AsNoTracking().ToListAsync());
        }

        public void CacheRemoveOrderLegList()
        {
            var key = WebCache.RenderKey(Params.APP_KEY, "OrderLegList");
            WebCache.Remove(key);
        }

        public OrderLegInfo? RenderOrderLegInfo(Repository.Model.OrderLeg? info)
        {
            if (info == null) return null;

            return new OrderLegInfo
            {
                Id = info.Id,
                OrderId = info.OrderId,
                LegIndex = info.LegIndex,
                LeagueId = info.LeagueId,
                HomeTeamId = info.HomeTeamId,
                AwayTeamId = info.AwayTeamId,
                PeriodCode = (info.PeriodCode ?? 0).ToEnum(PeriodCode.None),
                MarketCode = (info.MarketCode ?? 0).ToEnum(MarketCode.None),
                HandicapValue = info.HandicapValue,
                SelectionCode = (info.SelectionCode ?? 0).ToEnum(SelectionCode.None),
                BetOdds = info.BetOdds,
                SettledOdds = info.SettledOdds,
                LegSettleCode = (info.LegSettleCode ?? 0).ToEnum(LegSettleCode.None),
                SourceNo = info.SourceNo,
                MatchId = info.MatchId,
                MatchTime = info.MatchTime,
                Describe = info.Describe,
                CreatedTime = info.CreatedTime,
                UpdatedTime = info.UpdatedTime,
                Disabled = (info.Flag & (long)CommonCode.Disabled) > 0,
            };
        }
    }
}
