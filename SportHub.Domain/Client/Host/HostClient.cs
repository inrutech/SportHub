using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Net;
using System.Text;

using Zero.Core.Attribute;
using Zero.Core.Extensions;
using Zero.Core.Result;

namespace SportHub.Domain.Client.Host;

[Inject]
public partial class HostClient(IServiceProvider service)
{
    /// <summary>
    /// 日志·数
    /// </summary>
    public string _logPath = $"Client/{nameof(HostClient)}";

    /// <summary>
    /// 日志事件
    /// </summary>
    public ILogger<HostClient>? _logger => service.GetService<ILogger<HostClient>>();

    /// <summary>
    /// HTTP 客户端数˹设置
    /// </summary>
    public IHttpClientFactory? _httpClientFactory => service.GetService<IHttpClientFactory>();

    private HttpClient CreateHttpClient()
    {
        // var proxy = new WebProxy("http://65.20.68.6:33333")
        // {
        //     Credentials = new NetworkCredential("Zero", "472aa3d22c4b48a2bdf6f017b81e5f88")
        // };

        // var handler = new HttpClientHandler
        // {
        //     Proxy = proxy,
        //     UseProxy = true,
        //     PreAuthenticate = true,
        //     UseDefaultCredentials = false
        // };

        HttpClient client = _httpClientFactory!.CreateClient();

        // var client = new HttpClient(handler);
        return client;
    }

    private SysResult<T> ErrorResult<T>()
    {
        return new SysResult<T> { Code = ErrorCode.SYS_FAIL, Result = default };
    }

    /// <summary>
    /// 设备名ͽ请求的첽请求
    /// </summary>
    /// <typeparam name="T">请求设置</typeparam>
    /// <param name="message">HTTP 响应消息</param>
    /// <returns>设置ͽ数</returns>
    protected virtual async Task<SysResult<T>> ResultAsync<T>(HttpResponseMessage message) where T : class
    {
        string r = await message.Content.ReadAsStringAsync();
        _logger?.LogCustom($"result:{r}", _logPath);
        if (message.StatusCode != HttpStatusCode.OK)
        {
            return new SysResult<T>
            {
                Code = ErrorCode.SYS_FAIL,
                ErrorDesc = message.StatusCode.ToString(),
                Result = default
            };
        }

        return new SysResult<T> { Code = ErrorCode.SYS_SUCCESS, Result = r.DeserializeJson<T>() };
    }

    /// <summary>
    /// GET 请求
    /// </summary>
    /// <typeparam name="T">请求请求</typeparam>
    /// <param name="url">请求 URL</param>
    /// <returns>设备名</returns>
    public async Task<SysResult<T>> GetAsync<T>(string url) where T : class
    {
        try
        {
            if (url.IsNullOrEmpty())
            {
                return ErrorResult<T>();
            }

            HttpClient client = CreateHttpClient();
            HttpResponseMessage result = await client.GetAsync(url);
            _logger?.LogCustom($"url:{url}", _logPath);
            return await ResultAsync<T>(result);
        }
        catch (Exception ex)
        {
            _logger?.LogFail(ex);
            return ErrorResult<T>();
        }
    }

    /// <summary>
    /// POST 设置󣨷数ط数ͽ请求
    /// </summary>
    /// <typeparam name="T">请求请求</typeparam>
    /// <param name="url">请求 URL</param>
    /// <param name="args">请求设置</param>
    /// <returns>设备名</returns>
    public async Task<SysResult<T>> PostAsync<T>(string url, Dictionary<string, object?>? args = null) where T : class
    {
        try
        {
            if (url.IsNullOrEmpty())
            {
                return ErrorResult<T>();
            }

            HttpClient client = CreateHttpClient();
            StringContent content = new(args?.ToJson() ?? string.Empty, Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.PostAsync(url, content);
            _logger?.LogCustom($"url:{url}\r\ncontent:{content.ToJson()}", _logPath);
            return await ResultAsync<T>(result);
        }
        catch (Exception ex)
        {
            _logger?.LogFail(ex);
            return ErrorResult<T>();
        }
    }

    protected string RenderUrl(string ip)
    {
        var url = $"http://ip-api.com/json/{ip}";

        return url;
    }
}
