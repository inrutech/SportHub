public enum LegSettleCode
{
    None = 0,

    //结算状态（PENDING:待定, WON:全赢, HALF_WON:赢一半, PUSH:走水, HALF_LOST:输一半, LOST:全输）

    // 待定
    Pending = 1,

    // 全赢
    Won = 2,

    // 赢一半
    HalfWon = 3,

    // 走水
    Push = 4,

    // 输一半
    HalfLost = 5,

    // 全输
    Lost = 6,

}