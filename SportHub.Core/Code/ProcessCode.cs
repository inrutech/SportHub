public enum ProcessCode : long
{
    None = 0,

    /// <summary>
    /// 排队中
    /// </summary>
    Queued = 10,

    /// <summary>
    /// 处理中
    /// </summary>
    Processing = 20,

    /// <summary>
    /// 已完成
    /// </summary>
    Completed = 30,

    /// <summary>
    /// 失败
    /// </summary>
    Failed = 40,

}