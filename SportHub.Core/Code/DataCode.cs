using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportHub.Core.Code
{
    /// <summary>
    /// 数据类型
    /// </summary>
    public enum DataCode
    {
        /// <summary>
        /// 无数据类型。
        /// </summary>
        None = 0,

        /// <summary>
        /// 布尔
        /// </summary>
        Boolean = 1,

        /// <summary>
        /// 整型
        /// </summary>
        Integer = 2,

        /// <summary>
        /// 浮点
        /// </summary>
        Decimal = 3,

        /// <summary>
        /// 字符串
        /// </summary>
        String = 4,
    }
}
