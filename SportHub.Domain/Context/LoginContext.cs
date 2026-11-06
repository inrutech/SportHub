using Zero.Core.Attribute;

namespace SportHub.Domain.Context
{
    /// <summary>
    /// 登录上下文
    /// </summary>
    /// <typeparam name="T">登录信息类型</typeparam>
    [Inject]
    public class LoginContext<T> where T : class
    {
        /// <summary>
        /// 登录信息
        /// </summary>
        public T LoginInfo { get; set; } = default!;
    }
}
