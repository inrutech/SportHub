using Microsoft.AspNetCore.Mvc;

using SportHub.Domain.Admin.Request.Match;
using SportHub.Domain.Admin.Response.Match;
using SportHub.Filter;
using SportHub.Service.Admin;

using Zero.Core.Result;
using Zero.Core.Web;
using Zero.Core.Web.Request;

namespace SportHub.Controller.Admin;

public class MatchController(MatchService service) : BaseController
{
    [AdminValid]
    [HttpGet("Page")]
    public Task<SysResult<PageList<MatchPageResponse>>> PageAsync([FromQuery] MatchPageRequest request)
    {
        return service.PageAsync(request);
    }
}
