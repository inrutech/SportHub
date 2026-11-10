using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SportHub.Domain.Client.IM;
using SportHub.Domain.Context;
using SportHub.Repository.Model;
using SportHub.Service.Common;

using Zero.Core.Attribute;
using Zero.Core.Extensions;
using Zero.Core.Inject;
using Zero.Core.Result;

namespace SportHub.Service;


[Inject]
public partial class BaseService(IServiceProvider provider)
{
    protected readonly IServiceProvider _provider = provider;

    public IHttpClientFactory HttpClientFactory => _provider.GetRequiredService<IHttpClientFactory>();

    public IHttpContextAccessor HttpContextAccessor => _provider.GetRequiredService<IHttpContextAccessor>();

    public DBContext DB => _provider.GetRequiredService<DBContext>();

    public WebCache WebCache => _provider.GetRequiredService<WebCache>();

    public Snowflake Snowflake => _provider.GetRequiredService<Snowflake>();

    public WebClient WebClient => _provider.GetRequiredService<WebClient>();

    protected IHostEnvironment HostEnvironment => _provider.GetRequiredService<IHostEnvironment>()!;

    public ILogger<BaseService> Logger => _provider.GetRequiredService<ILogger<BaseService>>()!;

    public LoginContext<Repository.Model.Admin> AdminContext => _provider.GetRequiredService<LoginContext<Repository.Model.Admin>>();

    public IMClient IMClient => _provider.GetService<IMClient>()!;

    public ConfigService ConfigService => _provider.GetRequiredService<ConfigService>();

    public LeagueService LeagueService => _provider.GetRequiredService<LeagueService>();

    public TeamService TeamService => _provider.GetRequiredService<TeamService>();

    public MatchService MatchService => _provider.GetRequiredService<MatchService>();

    public OrderService OrderService => _provider.GetRequiredService<OrderService>();

    public OrderLegService OrderLegService => _provider.GetRequiredService<OrderLegService>();

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
