
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

using SportHub.Domain.Context;

using Zero.Core.Extensions;

namespace SportHub.Worker.Client
{
    public class TaskWorker(ILogger<TaskWorker> logger, IServiceProvider services) : BaseWorker(logger, services, TimeSpan.FromSeconds(1))
    {

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="stoppingToken"></param>
        /// <returns></returns>
        protected override async Task ProcessAsync(CancellationToken stoppingToken)
        {
            var succ = false;
            try
            {
                succ = await HandleAsync(stoppingToken);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogCustom("Concurrency Error.", LogPath);
                _logger.LogFail(ex, "DBError");
            }
            catch (Exception ex)
            {
                _logger.LogCustom("Error.", LogPath);
                _logger.LogFail(ex);
            }
            finally
            {
                if (!succ)
                {
                    await Task.Delay(TimeSpan.FromSeconds(1), stoppingToken);
                }
            }
        }

        private async Task<bool> HandleAsync(CancellationToken stoppingToken)
        {

            return true;
        }

    }
}