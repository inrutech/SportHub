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
        Common = 1,

        /// <summary>
        /// 闪兑汇率
        /// </summary>
        ExchangeRate = 2,

        /// <summary>
        /// 区块链配置
        /// </summary>
        Chain = 3,

        /// <summary>
        /// 代收配置
        /// </summary>
        Credit = 4,

        /// <summary>
        /// 代付配置
        /// </summary>
        Debit = 5,

        /// <summary>
        /// 三方配置
        /// </summary>
        Other = 99,
    }
}
