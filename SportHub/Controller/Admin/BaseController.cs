using System;

using Microsoft.AspNetCore.Mvc;

namespace SportHub.Controller.Admin;

/// <summary>
/// 蜜蜂应用基础控制器
/// </summary>
[ApiController]
[Route("api/admin/[controller]")]
[ApiExplorerSettings(GroupName = "admin")]
public class BaseController
{

}
