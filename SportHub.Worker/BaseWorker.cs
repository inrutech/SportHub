using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using SportHub.Repository.Model;

using Zero.Core.Extensions;
using Zero.Core.Inject;

namespace SportHub.Worker;

public class BaseWorker(ILogger<BaseWorker> logger, IServiceProvider services, TimeSpan bootTime = default) : BackgroundService
{
    /// <summary>
    /// </summary>
    protected readonly TimeSpan _bootTime = bootTime;

    /// <summary>
    /// </summary>
    protected readonly ILogger<BaseWorker> _logger = logger;

    protected virtual string LogPath => $"/Worker/{this.GetType().Name}";

    /// <summary>
    /// </summary>
    protected readonly IServiceProvider _services = services;

    /// <summary>
    /// </summary>
    protected IServiceScope? _scope;

    protected Snowflake Snowflake => _services.GetService<Snowflake>()!;

    protected WebCache WebCache => _services.GetService<WebCache>()!;

    protected IHttpClientFactory HttpClientFactory => _services.GetService<IHttpClientFactory>()!;

    protected DBContext DB => _scope?.ServiceProvider.GetService<DBContext>()!;


    /// <summary>
    /// 任务入口    
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (_bootTime != default)
        {
            await Task.Delay(_bootTime, stoppingToken);
        }
        _logger.LogCustom($"start", LogPath);
        while (!stoppingToken.IsCancellationRequested)
        {
            await using var scope = _services.CreateAsyncScope();

            _scope = scope;

            await ProcessAsync(stoppingToken);

        }
        _logger.LogCustom($"end", LogPath);
    }

    /// <summary>
    /// 执行事件
    /// </summary>
    /// <param name="stoppingToken"></param>
    /// <returns></returns>
    protected virtual async Task ProcessAsync(CancellationToken stoppingToken) { await Task.CompletedTask; }

}
