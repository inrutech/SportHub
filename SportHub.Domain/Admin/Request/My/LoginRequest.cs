using System.ComponentModel.DataAnnotations;

namespace SportHub.Domain.Admin.Request.My;

/// <summary>
/// 登录请求。
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// 登录名。
    /// </summary>
    [Required]
    public string? LoginName { get; set; }

    /// <summary>
    /// 登录密码的 MD5。
    /// </summary>
    [Required]
    public string? Password { get; set; }

    /// <summary>
    /// 谷歌验证码。
    /// </summary>
    [Required]
    public string? GoogleAuthKey { get; set; }
}
