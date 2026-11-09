public enum MarketCode : long
{
    None = 0,

    // 胜平负（独赢），bet_type: 1x2
    OneXTwo = 1,

    // 让球（亚盘），bet_type: ah
    AsianHandicap = 2,

    // 大小球，bet_type: ou
    OverUnder = 3,

    // 单双，bet_type: oe
    OddEven = 4,

    // 总进球，bet_type: goals
    TotalGoals = 5,

    // 波胆（正确比分），bet_type: bodan
    CorrectScore = 6,

    // 半全场，bet_type: half_full
    HalfFullTime = 7,

    // 进球序，bet_type: goal_order
    GoalOrder = 8,

    // 双重机会，bet_type: dc
    DoubleChance = 9,

    // 角球大小，bet_type: corner_ou
    CornerOverUnder = 10,

    // 角球数/先开球，bet_type: corner_other
    CornerOther = 11,

    // 角球单双，bet_type: corner_oe
    CornerOddEven = 12,

    // 上半场单双，bet_type: oe_half
    HalfOddEven = 13,

    // 上半场总进球，bet_type: tg_half
    HalfTotalGoals = 14,

    // 加时/点球，bet_type: overtime
    Overtime = 15,

    // 优胜冠军，bet_type: champion
    Champion = 16,

    // 混合过关（整单标识）
    Parlay = 17,
}