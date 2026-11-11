namespace SportHub.Domain.Admin.Response.Admin;

public class GetGoogleAuthSecretResponse
{
    /// <summary>
    /// 新的谷歌验证密钥。
    /// </summary>
    public string? NewGoogleAuthSecret { get; set; }

    /// <summary>
    /// Gets or sets the URL for initiating a new Google authentication process.
    /// </summary>
    public string? NewGoogleAuthLink { get; set; }
}
