using System.ComponentModel.DataAnnotations;

namespace SportHub.Domain.Admin.Request.Admin;

public class SetRequest
{
    /// <summary>
    /// 主键 ID
    /// </summary>
    public long? AdminId { get; set; }

    /// <summary>
    /// 角色类型
    /// </summary>
    [Required]
    public long? RoleId { get; set; }

    /// <summary>
    /// 别名
    /// </summary>
    [Required]
    public string? Alias { get; set; }

    /// <summary>
    /// 登录名
    /// </summary>
    [Required]
    public string? LoginName { get; set; }

}
