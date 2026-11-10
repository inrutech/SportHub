using SportHub.Core;
using SportHub.Core.Code;
using SportHub.Domain.Model;
using SportHub.Repository.Model;

using Microsoft.EntityFrameworkCore;

namespace SportHub.Service.Common
{
    public class MatchService(IServiceProvider provider) : BaseService(provider)
    {
        public Task<List<Repository.Model.Match>> CacheGetMatchListAsync()
        {
            var key = WebCache.RenderKey(Params.APP_KEY, "MatchList");
            return WebCache.Get(key, async () => await DB.Match.AsNoTracking().ToListAsync());
        }

        public void CacheRemoveMatchList()
        {
            var key = WebCache.RenderKey(Params.APP_KEY, "MatchList");
            WebCache.Remove(key);
        }

        public MatchInfo? RenderMatchInfo(Repository.Model.Match? info)
        {
            if (info == null) return null;

            return new MatchInfo
            {
                Id = info.Id,
                LeagueId = info.LeagueId,
                HomeTeamId = info.HomeTeamId,
                AwayTeamId = info.AwayTeamId,
                SourceArray = info.SourceArray,
                CreatedTime = info.CreatedTime,
                UpdatedTime = info.UpdatedTime,
                Disabled = (info.Flag & (long)CommonCode.Disabled) > 0,
            };
        }
    }
}
