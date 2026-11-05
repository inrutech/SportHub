using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Net;
using System.Net.Http;

using Zero.Core.Attribute;
using Zero.Core.Result;

namespace SportHub.Domain.Client.VIC;

[Inject]
public partial class VICClient(IServiceProvider service)
{
    private const string DefaultLeagueReferer = "https://site.vic88888.com/site/customize/fa_football_league/view.do";
    private const string DefaultLeagueUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/156.0.0.0 Safari/537.36 Edg/156.0.0.0";
    private const string DefaultLeagueCookie = "Authorization=eca4f808-f86e-4838-9fd5-a368eed400e3";
    private const string DefaultProxyUrl = "http://127.0.0.1:7890";

    public IHttpClientFactory HttpClientFactory => service.GetService<IHttpClientFactory>()!;

    public ILogger<VICClient>? Logger => service.GetService<ILogger<VICClient>>();

    private HttpClient CreateHttpClient(bool ignoreSslErrors, bool useProxy)
    {
        if (!ignoreSslErrors && !useProxy)
        {
            return HttpClientFactory.CreateClient();
        }

        var handler = new HttpClientHandler();

        if (ignoreSslErrors)
        {
            handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
        }

        if (useProxy)
        {
            var proxyUrl = GetEnvOrDefault("SPORTHUB_SYNC_PROXY", DefaultProxyUrl);
            var proxy = new WebProxy(proxyUrl)
            {
                BypassProxyOnLocal = false
            };

            handler.Proxy = proxy;
            handler.UseProxy = true;
        }

        return new HttpClient(handler, disposeHandler: true);
    }

    private static string GetEnvOrDefault(string key, string fallback)
    {
        var envValue = Environment.GetEnvironmentVariable(key);
        if (!string.IsNullOrWhiteSpace(envValue))
        {
            return envValue;
        }

        return fallback;
    }

    private static string FlattenExceptionMessages(Exception ex)
    {
        var messages = new List<string>();
        Exception? current = ex;

        while (current is not null)
        {
            if (!string.IsNullOrWhiteSpace(current.Message))
            {
                messages.Add(current.Message.Trim());
            }

            current = current.InnerException;
        }

        return string.Join(" | ", messages.Distinct());
    }

    private static SysResult<T> Fail<T>(string errorDesc)
    {
        return new SysResult<T>
        {
            Code = ErrorCode.SYS_FAIL,
            ErrorDesc = errorDesc,
            Result = default
        };
    }
}
