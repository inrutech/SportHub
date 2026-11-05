using Microsoft.AspNetCore.Mvc;

using SportHub.Service.Other;

using Zero.Core.Result;

namespace SportHub.Controller.Other;

public class InitController(InitService service) : BaseController
{

    [HttpPost("sync-league")]
    public Task<SysResult<bool>> SyncLeagueAsync(SyncLeagueRequest request)
    {
        return service.SyncLeagueAsync(request);
    }
}