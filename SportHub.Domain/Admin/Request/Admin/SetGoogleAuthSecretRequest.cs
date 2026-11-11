using System.ComponentModel.DataAnnotations;

namespace SportHub.Domain.Admin.Request.Admin;

/// <summary>
/// 设置谷歌验证码密钥请求
/// </summary>
public class SetGoogleAuthSecretRequest
{
    /// <summary>
    /// adminId
    /// </summary>
    [Required]
    public long? AdminId { get; set; }

    /// <summary>
    /// 新的谷歌验证码。
    /// </summary>
    [Required]
    public string? NewGoogleAuthKey { get; set; }

    /// <summary>
    /// 新的谷歌验证码密钥。
    /// </summary>
    [Required]
    public string? NewGoogleAuthSecret { get; set; }
}
