using Microsoft.AspNetCore.Mvc;

using SportHub.Service.Admin;

using Zero.Core.Result;

namespace SportHub.Controller.Admin;

public class LeagueController(LeagueService service) : BaseController
{

    [HttpPost("SyncLeague")]
    public Task<SysResult<bool>> SyncLeagueAsync()
    {
        return service.SyncLeagueAsync();
    }

}