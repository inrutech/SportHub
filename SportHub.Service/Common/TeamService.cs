using SportHub.Core;
using SportHub.Core.Code;
using SportHub.Domain.Model;
using SportHub.Repository.Model;

using Microsoft.EntityFrameworkCore;

namespace SportHub.Service.Common
{
    public class TeamService(IServiceProvider provider) : BaseService(provider)
    {
        public Task<List<Repository.Model.Team>> CacheGetTeamListAsync()
        {
            var key = WebCache.RenderKey(Params.APP_KEY, "TeamList");
            return WebCache.Get(key, async () => await DB.Team.AsNoTracking().ToListAsync());
        }

        public void CacheRemoveTeamList()
        {
            var key = WebCache.RenderKey(Params.APP_KEY, "TeamList");
            WebCache.Remove(key);
        }

        public TeamInfo? RenderTeamInfo(Repository.Model.Team? info)
        {
            if (info == null) return null;

            return new TeamInfo
            {
                Id = info.Id,
                LeagueId = info.LeagueId,
                NameEn = info.NameEn,
                NameZhCn = info.NameZhCn,
                NameZhTw = info.NameZhTw,
                SourceArray = info.SourceArray,
                CreatedTime = info.CreatedTime,
                UpdatedTime = info.UpdatedTime,
                Disabled = (info.Flag & (long)CommonCode.Disabled) > 0,
            };
        }
    }
}
