using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportHub.Core.Code
{
    /// <summary>
    /// 配置类型
    /// </summary>
    public enum ConfigCode
    {
        /// <summary>
        /// 无配置。
        /// </summary>
        None = 0,

        /// <summary>
        /// 公共配置
        /// </summary>
        Base = 1,

        /// <summary>
        /// 三方配置
        /// </summary>
        Other = 99,
    }
}
