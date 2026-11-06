using Microsoft.EntityFrameworkCore;

using SportHub.Core;
using SportHub.Domain.Admin.Request.My;

using Zero.Core.Result;

namespace SportHub.Service.Admin;

public class MyService(IServiceProvider provider) : BaseService(provider)
{
    public async Task<SysResult<bool>> SetTokenAsync(SetTokenRequest request)
    {
        var token = await DB.Config.FirstOrDefaultAsync(x => x.GlobalHash == GlobalHash.LoginToken);
        if (token == null)
        {
            return Result(false, ErrorCode.USER_CUSTOM, "Token配置不存在");
        }
        token.Value = request.Token;
        token.UpdatedTime = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
        await DB.SaveChangesAsync();
        ConfigService.CacheRemoveConfigList();
        return Result(true);
    }
}
