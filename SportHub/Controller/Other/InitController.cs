using Microsoft.AspNetCore.Mvc;

using SportHub.Service.Other;

using Zero.Core.Result;

namespace SportHub.Controller.Other;

public class InitController(InitService service) : BaseController
{
    [HttpPost("sync")]
    public Task<SysResult<bool>> SyncAsync()
    {
        return service.SyncLeagueAsync();
    }

    [HttpPost("sync-league")]
    public Task<SysResult<bool>> SyncLeagueAsync()
    {
        return service.SyncLeagueAsync();
    }
}