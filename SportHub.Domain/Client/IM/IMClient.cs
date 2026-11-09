using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System.Net;
using System.Net.Http;
using System.Text;

using Zero.Core.Attribute;
using Zero.Core.Extensions;
using Zero.Core.Result;
using Zero.Core.Util;

namespace SportHub.Domain.Client.IM;

[Inject]
public partial class IMClient(IServiceProvider service)
{
    /// <summary>
    /// 日志·数
    /// </summary>
    public string _logPath = $"Client/{nameof(IMClient)}";

    /// <summary>
    /// 日志事件
    /// </summary>
    public ILogger<IMClient>? _logger => service.GetService<ILogger<IMClient>>();

    /// <summary>
    /// HTTP 客户端数˹设置
    /// </summary>
    public IHttpClientFactory? _httpClientFactory => service.GetService<IHttpClientFactory>();

    public string AppId { get; private set; } = string.Empty;
    public string AppKey { get; private set; } = string.Empty;
    public string AppSecret { get; private set; } = string.Empty;

    public void Init(string appId, string appKey, string appSecret)
    {
        AppId = appId;
        AppKey = appKey;
        AppSecret = appSecret;
    }

    private HttpClient CreateHttpClient(Dictionary<string, string>? headers = null)
    {
        var handler = new HttpClientHandler
        {
            AutomaticDecompression = DecompressionMethods.All,
            UseProxy = true,
            Proxy = new WebProxy("http://127.0.0.1:7890"),
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };

        var client = new HttpClient(handler);

        if (headers != null)
        {
            foreach (var header in headers)
            {
                client.DefaultRequestHeaders.TryAddWithoutValidation(header.Key, header.Value);
            }
        }

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
    public async Task<SysResult<T>> PostAsync<T>(string host, string action, Dictionary<string, object?>? args = null, Dictionary<string, object?>? payload = null, Dictionary<string, string>? headers = null) where T : class
    {
        try
        {
            if (action.IsNullOrEmpty())
            {
                return ErrorResult<T>();
            }

            var url = RenderUrl(host, action, args);

            HttpClient client = CreateHttpClient(headers);

            var content = new StringContent(payload?.ToJson() ?? string.Empty, Encoding.UTF8, "application/json");
            HttpResponseMessage result = await client.PostAsync(url, content);
            _logger?.LogCustom($"url:{url}\r\ncontent:{payload.ToJson()}", _logPath);
            return await ResultAsync<T>(result);
        }
        catch (Exception ex)
        {
            _logger?.LogFail(ex);
            return ErrorResult<T>();
        }
    }

    protected string RenderUrl(string host, string action, Dictionary<string, object?>? args = null)
    {
        var url = host.IsNullOrEmpty() ? action : $"{host}{action}";
        if (args?.Count > 0)
        {
            var query = string.Join("&", args.Where(a => a.Value != null).Select(a => $"{a.Key}={Uri.EscapeDataString(a.Value?.ToString() ?? string.Empty)}"));
            if (action.Contains('?'))
            {
                url += "&" + query;
            }
            else
            {
                url += "?" + query;
            }
        }
        return url;
    }
}
