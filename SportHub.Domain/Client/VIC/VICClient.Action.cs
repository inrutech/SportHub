using Microsoft.Extensions.Logging;

using SportHub.Domain.Client.VIC.Response;

using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;

using Zero.Core.Result;

namespace SportHub.Domain.Client.VIC;

public partial class VICClient
{
    public async Task<SysResult<GetLeagueListResponse>> GetLeagueListAsync(string token)
    {

        var args = new Dictionary<string, object?>
        {
            ["tabParam"] = "",
            ["refFormId"] = "fa_football_league",
            ["_t"] = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString()
        };

        var payload = new Dictionary<string, object?>
        {
            ["tabParam"] = "",
            ["filter"] = "",
            ["_pageParam_"] = "eyJfb2Zmc2V0XyI6MSwiX2xpbWl0XyI6NTAwMCwiX29yZGVyQnlfIjoiY3JlYXRldGltZV9uYW1lIGRlc2MiLCJfY2FsY0ZpZWxkXyI6ImNvdW50KGlkKSJ9"
        };

        var headers = new Dictionary<string, string>
        {
            ["__userkeyid__"] = token,
            ["Cookie"] = "Authorization=f1f3c047-78e0-40d5-8d48-0a5aa644ecd9",
            // ["Accept-Encoding"] = "gzip, deflate, br",
            // ["Accept-Language"] = "zh-CN,zh;q=0.9,en;q=0.8",
            // ["Connection"] = "keep-alive",
            // ["Content-Type"] = "application/json; charset=UTF-8",
            // ["Origin"] = "https://site.vic88888.com",
            // ["Referer"] = "https://site.vic88888.com/site/customize/fa_football_league/view.do",
            // ["User-Agent"] = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/156.0.0.0 Safari/537.36 Edg/156.0.0.0"
        };

        var r = await PostAsync<GetLeagueListResponse>("site.vic88888.com", "/site/customize/fa_football_league/executeService/listData", args, payload, headers);
        return r;
    }

}
