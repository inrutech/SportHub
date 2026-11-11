
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Playwright;

using Task = System.Threading.Tasks.Task;
using SportHub.Domain.Context;

using Zero.Core.Extensions;

namespace SportHub.Worker.Client
{
    public class InitWorker(ILogger<InitWorker> logger, IServiceProvider services) : BaseWorker(logger, services, TimeSpan.FromSeconds(1))
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
            if (ClientContext.Playwright == null)
            {
                await InitPlaywrightAsync();
                _logger.LogCustom($"Playwright初始化完成.", LogPath);
            }
            return true;
        }

        private async Task InitPlaywrightAsync()
        {
            ClientContext.Playwright = await Playwright.CreateAsync();

            ClientContext.EdgeBrowser = await ClientContext.Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            {
                Headless = false,
                Channel = "msedge",
                SlowMo = 50,// 设置每个操作的延迟为 50 毫秒
                //Proxy = new Proxy { Server = "per-context" },
                Args = [
                        "--disable-blink-features=AutomationControlled", //禁用 Blink 引擎的 AutomationControlled 特性，防止网站检测到自动化工具（如 Playwright/Selenium）的控制。
                        "--disable-dev-shm-usage", //禁用 /dev/shm（共享内存）的使用，改用 /tmp 作为替代
                        "--no-sandbox", //禁用 Chrome 的沙盒（Sandbox）安全机制。
                        "--disable-gpu", //禁用 GPU 硬件加速。
                        "--disable-infobars", //禁用 Chrome 顶部的信息栏（如 "Chrome 正受到自动测试软件控制"）。
                        "--deny-permission-prompts", //自动拒绝所有权限请求（如地理位置、摄像头、通知等）。
                        "--disable-blink-features=AutomationControlled" // 避免被识别为自动化工具
                        //"--start-maximized" //# 启动时最大化
                ]
            });

            //NodeContext.EdgeShowBrowser = await NodeContext.Playwright.Chromium.LaunchAsync(new BrowserTypeLaunchOptions
            //{
            //    Headless = false,
            //    Channel = "msedge",
            //    //Proxy = new Proxy { Server = "per-context" },
            //    Args = [
            //           "--disable-blink-features=AutomationControlled",
            //            "--disable-dev-shm-usage",
            //            "--no-sandbox",
            //            "--disable-gpu",
            //            "--disable-infobars"
            //   ]
            //});
        }


    }

}