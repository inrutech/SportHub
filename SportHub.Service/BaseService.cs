using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SportHub.Repository.Model;

using Zero.Core.Attribute;
using Zero.Core.Extensions;
using Zero.Core.Inject;
using Zero.Core.Result;

namespace SportHub.Service;


[Inject]
public partial class BaseService(IServiceProvider provider)
{
    protected readonly IServiceProvider _provider = provider;

    public IHttpClientFactory HttpClientFactory => _provider.GetService<IHttpClientFactory>()!;

    public IHttpContextAccessor HttpContextAccessor => _provider.GetService<IHttpContextAccessor>()!;

    public DBContext DB => _provider.GetService<DBContext>()!;

    public WebCache WebCache => _provider.GetService<WebCache>()!;

    public Snowflake Snowflake => _provider.GetService<Snowflake>()!;

    public WebClient WebClient => _provider.GetService<WebClient>()!;

    protected IHostEnvironment HostEnvironment => _provider.GetService<IHostEnvironment>()!;

    public ILogger<BaseService> Logger => _provider.GetService<ILogger<BaseService>>()!;

    protected SysResult<T> Result<T>(T? model)
    {
        return Result(model, ErrorCode.SYS_SUCCESS, null);
    }
    // protected SysResult<T1, T2> Result<T1, T2>(T1? model, T2 extend)
    // {
    //     return Result(model, ErrorCode.SYS_SUCCESS, null, extend);
    // }

    protected SysResult<T> Result<T>(T? model, ErrorCode code)
    {
        return Result(model, code, code.GetDescription());
    }

    protected SysResult<T> Result<T>(T? model, ErrorCode code, string? errorDesc)
    {
        return new SysResult<T>
        {
            Code = code,
            Result = model,
            ErrorDesc = errorDesc
        };
    }

}
