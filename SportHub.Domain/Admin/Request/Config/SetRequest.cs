using System.ComponentModel.DataAnnotations;

namespace SportHub.Domain.Admin.Request.Config;

public class SetRequest
{
    /// <summary>
    /// 配置项的键值对
    /// </summary>
    [Required]
    public Dictionary<long, string>? ConfigArray { get; set; }

}
