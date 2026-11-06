using SportHub.Domain.Admin.Response.Config;
using Microsoft.EntityFrameworkCore;
using Zero.Core.Result;
using Microsoft.AspNetCore.Http;
using SportHub.Domain.Admin.Request.Config;

namespace SportHub.Service.Admin;

public class ConfigService(IServiceProvider provider) : BaseService(provider)
{
    public async Task<SysResult<List<ListResponse>>> ListAsync(ListRequest request)
    {
        var query = DB.Config.AsNoTracking();

        var list = await query.ToListAsync();

        var r = list.Select(x => new ListResponse
        {
            ConfigInfo = ConfigService.RenderConfigInfo(x),
        }).ToList();

        return Result(r);
    }

    public async Task<SysResult<bool>> SetAsync(SetRequest request)
    {
        var list = await DB.Config.ToListAsync();
        foreach (var item in request.ConfigArray)
        {
            var config = list.FirstOrDefault(x => x.GlobalHash == item.Key);
            if (config != null)
            {
                config.Value = item.Value;
                config.UpdatedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            }
        }
        await DB.SaveChangesAsync();
        ConfigService.CacheRemoveConfigList();
        return Result(true);
    }
}
