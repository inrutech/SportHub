public enum OrderCode : long
{
    None = 0,

    // bet_type: 1x2
    OneXTwo = 1,

    // bet_type: ah
    AsianHandicap = 2,

    // bet_type: ou
    OverUnder = 3,

    // is_parlay=1
    Parlay = 4,

    // bet_type: oe
    OddEven = 5,

    // bet_type: goals
    TotalGoals = 6,

    // bet_type: bodan
    CorrectScore = 7,

    // bet_type: half_full
    HalfFullTime = 8,

    // bet_type: goal_order
    GoalOrder = 9,

    // bet_type: dc
    DoubleChance = 10,

    // bet_type: corner_ou
    CornerOverUnder = 11,

    // bet_type: corner_other
    CornerOther = 12,

    // bet_type: corner_oe
    CornerOddEven = 13,

    // bet_type: oe_half
    HalfOddEven = 14,

    // bet_type: tg_half
    HalfTotalGoals = 15,

    // bet_type: overtime
    Overtime = 16,

    // bet_type: champion
    Champion = 17,
}