using Microsoft.AspNetCore.Mvc;

using SportHub.Domain.Other.Request;
using SportHub.Service.Other;

using Zero.Core.Result;

namespace SportHub.Controller.Other;

public class InitController(InitService service) : BaseController
{
    [HttpPost("sync")]
    public Task<SysResult<bool>> SyncAsync([FromBody] SyncLeagueRequest request)
    {
        return service.SyncLeagueAsync(request);
    }

    [HttpPost("sync-league")]
    public Task<SysResult<bool>> SyncLeagueAsync([FromBody] SyncLeagueRequest request)
    {
        return service.SyncLeagueAsync(request);
    }
}