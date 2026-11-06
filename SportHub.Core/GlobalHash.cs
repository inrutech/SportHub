namespace SportHub.Core;

/// <summary>
/// 全局哈希常量类。
/// </summary>
public class GlobalHash
{

    #region 公共配置 (10000 ~ 10900)

    /// <summary>
    /// 版本号
    /// </summary>
    public const long VersionNumber = 10000;

    /// <summary>
    /// 钱包链接
    /// </summary>
    public const long WalletLink = 10010;

    /// <summary>
    /// 邀请注册链接
    /// </summary>
    public const long RegisterLink = 10020;

    /// <summary>
    /// 收银台链接
    /// </summary>
    public const long CashierLink = 10030;

    /// <summary>
    /// Telegram客服链接
    /// </summary>
    public const long Service_Telegram = 10100;

    /// <summary>
    /// WhatsApp客服链接
    /// </summary>
    public const long Service_Whatsapp = 10110;
    #endregion

    #region 闪兑汇率 (11000 ~ 11900)

    /// <summary>
    /// 商户代币兑换USDT汇率(代币/USDT)
    /// </summary>
    public const long MerchantToUsdtExchangeRate = 11000;

    /// <summary>
    /// 商户USDT兑换代币汇率(代币/USDT)
    /// </summary>
    public const long MerchantFromUsdtExchangeRate = 11010;

    /// <summary>
    /// 代理代币兑换USDT汇率(代币/USDT)
    /// </summary>
    public const long AgentToUsdtExchangeRate = 11020;

    /// <summary>
    /// 代理USDT兑换代币汇率(代币/USDT)
    /// </summary>
    public const long AgentFromUsdtExchangeRate = 11030;

    /// <summary>
    /// 用户代币提现USDT汇率(代币/USDT)
    /// </summary>
    public const long UserToUsdtExchangeRate = 11040;

    /// <summary>
    /// 用户代币充值USDT汇率(代币/USDT)
    /// </summary>
    public const long UserFromUsdtExchangeRate = 11050;

    /// <summary>
    /// 商户最小提现金额
    /// </summary>
    public const long MinMerchantWithdrawUsdtAmount = 11060;

    /// <summary>
    /// 代理最小提现金额
    /// </summary>
    public const long MinAgentWithdrawUsdtAmount = 11070;

    /// <summary>
    /// 用户最小提现金额
    /// </summary>
    public const long MinUserWithdrawUsdtAmount = 11080;

    /// <summary>
    /// 市场USDT兑换汇率(币/USDT)
    /// </summary>
    public const long MarketUsdtExchangeRate = 11990;

    #endregion

    #region 链上配置 (12000 ~ 12900)
    /// <summary>
    /// Bsc链USDT地址
    /// </summary>
    public const long BscUsdtAddress = 12000;

    /// <summary>
    /// Bsc链RPC链接
    /// </summary>
    public const long BscChainRpcLink = 12010;

    /// <summary>
    /// Tron链USDT地址
    /// </summary>
    public const long TronUsdtAddress = 12020;

    /// <summary>
    /// Tron链RPC链接
    /// </summary>
    public const long TronChainRpcLink = 12030;

    #endregion

    #region 代收配置 (13000 ~ 13900)
    /// <summary>
    /// 0级分佣比例
    /// </summary>
    public const long Level0AwardRate = 13000;

    /// <summary>
    /// 1级分佣比例
    /// </summary>
    public const long Level1AwardRate = 13010;

    /// <summary>
    /// 2级分佣比例
    /// </summary>
    public const long Level2AwardRate = 13020;

    /// <summary>
    /// 3级分佣比例
    /// </summary>
    public const long Level3AwardRate = 13030;

    /// <summary>
    /// 4级分佣比例
    /// </summary>
    public const long Level4AwardRate = 13040;

    /// <summary>
    /// UPI接单冷却时间（分钟）
    /// </summary>
    public const long UPICoolingMunite = 13100;

    /// <summary>
    /// 订单扣费时间（分钟）
    /// </summary>
    public const long OrderDeductionMunite = 13110;

    /// <summary>
    /// 订单冻结时间（分钟）
    /// </summary>
    public const long OrderFrozenMunite = 13120;

    /// <summary>
    /// 最大失败次数
    /// </summary>
    public const long MaxFailCount = 13130;

    /// <summary>
    /// 浮点下限
    /// </summary>
    public const long FloatingLowerLimit = 13140;

    // /// <summary>
    // /// 浮点锁定时间（分钟）
    // /// </summary>
    // public const long FloatingLockMinute = 13150;
    #endregion

    #region 代付配置 (14000 ~ 14900)

    /// <summary>
    /// 用户奖励比例
    /// </summary>
    public const long AwardRate = 14010;

    /// <summary>
    /// 用户奖励固定金额
    /// </summary>
    public const long AwardFixedAmount = 14020;

    /// <summary>
    /// 锁定过期时间（分钟）
    /// </summary>
    public const long LockExpireMunite = 14030;

    /// <summary>
    /// 确认过期时间（分钟）
    /// </summary>
    public const long CheckExpireMinute = 14040;

    /// <summary>
    /// 订单过期时间（分钟）
    /// </summary>
    public const long OrderExpireMinute = 14050;

    #endregion

    #region 三方配置 (19000 ~ 19900)
    /// <summary>
    /// 亚马逊AWS S3区域
    /// </summary>
    public const long AmazonRegion = 19000;

    /// <summary>
    /// 亚马逊AWS S3桶名称
    /// </summary>
    public const long AmazonBucketName = 19010;

    /// <summary>
    /// 发件人邮箱地址
    /// </summary>
    public const long FromEmailAddress = 19020;

    /// <summary>
    /// 发件人邮箱别名
    /// </summary>
    public const long FromEmailAlias = 19030;

    /// <summary>
    /// 邮件API密钥
    /// </summary>
    public const long EmailApiKey = 19040;

    /// <summary>
    /// 代理链接
    /// </summary>
    public const long ProxyHost = 19100;

    /// <summary>
    /// 代理端口
    /// </summary>
    public const long ProxyPort = 19110;

    /// <summary>
    /// 代理用户名
    /// </summary>
    public const long ProxyUserName = 19120;

    /// <summary>
    /// 代理密码
    /// </summary>
    public const long ProxyPassword = 19130;

    /// <summary>
    /// 短信应用ID
    /// </summary>
    public const long SMSAppId = 19200;

    /// <summary>
    /// 短信应用Key
    /// </summary>
    public const long SMSAppKey = 19210;

    /// <summary>
    /// 短信应用密钥
    /// </summary>
    public const long SMSAppSecret = 19220;

    #endregion



}
