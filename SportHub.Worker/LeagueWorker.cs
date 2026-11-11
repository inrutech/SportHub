
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

using Task = System.Threading.Tasks.Task;
using Zero.Core.Extensions;

namespace SportHub.Worker
{
    public class LeagueWorker(ILogger<LeagueWorker> logger, IServiceProvider services) : BaseWorker(logger, services, TimeSpan.FromSeconds(1))
    {

        /// <summary>
        /// League同步
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