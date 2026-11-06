namespace SportHub.Domain.Context;

/// <summary>
/// 登录令牌信息
/// </summary>
public class LoginTokenInfo
{
    /// <summary>
    /// 用户 ID
    /// </summary>
    public long Id { get; set; }

    /// <summary>
    /// 时间戳
    /// </summary>
    public long Ticks { get; set; } = DateTimeOffset.UtcNow.Ticks;
}
