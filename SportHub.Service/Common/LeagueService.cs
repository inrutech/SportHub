using SportHub.Core;
using SportHub.Core.Code;
using SportHub.Domain.Model;
using SportHub.Repository.Model;

using Microsoft.EntityFrameworkCore;

using Zero.Core.Extensions;

namespace SportHub.Service.Common
{
    public class LeagueService(IServiceProvider provider) : BaseService(provider)
    {
        public Task<List<Repository.Model.League>> CacheGetLeagueListAsync()
        {
            var key = WebCache.RenderKey(Params.APP_KEY, "LeagueList");
            return WebCache.Get(key, async () => await DB.League.AsNoTracking().ToListAsync());
        }

        public void CacheRemoveLeagueList()
        {
            var key = WebCache.RenderKey(Params.APP_KEY, "LeagueList");
            WebCache.Remove(key);
        }

        public LeagueInfo? RenderLeagueInfo(Repository.Model.League? info)
        {
            if (info == null) return null;

            return new LeagueInfo
            {
                Id = info.Id,
                LeagueCode = (info.LeagueCode ?? 0).ToEnum(LeagueCode.None),
                CountryId = info.CountryId,
                CountryEn = info.CountryEn,
                CountryZhCn = info.CountryZhCn,
                CountryZhTw = info.CountryZhTw,
                NameEn = info.NameEn,
                NameZhCn = info.NameZhCn,
                NameZhTw = info.NameZhTw,
                ShortNameEn = info.ShortNameEn,
                ShortNameZhCn = info.ShortNameZhCn,
                ShortNameZhTw = info.ShortNameZhTw,
                SourceArray = info.SourceArray,
                CreatedTime = info.CreatedTime,
                UpdatedTime = info.UpdatedTime,
                Disabled = (info.Flag & (long)CommonCode.Disabled) > 0,
            };
        }
    }
}
