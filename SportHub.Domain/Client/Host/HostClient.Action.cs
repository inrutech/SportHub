
using SportHub.Domain.Client.Host.Response;

using Zero.Core.Result;

namespace SportHub.Domain.Client.Host;

public partial class HostClient
{
    public Task<SysResult<HostInfo>> GetHostAsync(string? ip = null)
    {
        string url = RenderUrl(ip);

        return GetAsync<HostInfo>(url);
    }
}
