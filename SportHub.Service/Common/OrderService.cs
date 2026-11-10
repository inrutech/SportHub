using SportHub.Core;
using SportHub.Core.Code;
using SportHub.Domain.Model;
using SportHub.Repository.Model;

using Microsoft.EntityFrameworkCore;

using Zero.Core.Extensions;

namespace SportHub.Service.Common
{
    public class OrderService(IServiceProvider provider) : BaseService(provider)
    {
        public Task<List<Repository.Model.Order>> CacheGetOrderListAsync()
        {
            var key = WebCache.RenderKey(Params.APP_KEY, "OrderList");
            return WebCache.Get(key, async () => await DB.Order.AsNoTracking().ToListAsync());
        }

        public void CacheRemoveOrderList()
        {
            var key = WebCache.RenderKey(Params.APP_KEY, "OrderList");
            WebCache.Remove(key);
        }

        public OrderInfo? RenderOrderInfo(Repository.Model.Order? info)
        {
            if (info == null) return null;

            return new OrderInfo
            {
                Id = info.Id,
                SourceNo = info.SourceNo,
                OrderCode = (info.OrderCode ?? 0).ToEnum(OrderCode.None),
                LegCount = info.LegCount,
                Stake = info.Stake,
                Odds = info.Odds,
                PotentialPayout = info.PotentialPayout,
                ActualPayout = info.ActualPayout,
                OrderSettleCode = (info.OrderSettleCode ?? 0).ToEnum(OrderSettleCode.None),
                CreatedTime = info.CreatedTime,
                UpdatedTime = info.UpdatedTime,
                Disabled = (info.Flag & (long)CommonCode.Disabled) > 0,
            };
        }
    }
}
