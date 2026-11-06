using SportHub.Domain.Admin.Request;

using Zero.Core.Result;

namespace SportHub.Service.Admin;

public class MyService(IServiceProvider provider) : BaseService(provider)
{
    public Task<SysResult<bool>> SetTokenAsync(SetTokenRequest request)
    {
        if (request is null || string.IsNullOrWhiteSpace(request.Token))
        {
            return Task.FromResult(Result(false, ErrorCode.SYS_FAIL, "设置失败：请求缺少 Token。"));
        }

        return Task.FromResult(Result(true));
    }
}
