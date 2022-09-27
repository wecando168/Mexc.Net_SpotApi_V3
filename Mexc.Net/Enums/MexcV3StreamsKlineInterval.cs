namespace Mexc.Net.Enums
{
    /// <summary>
    /// The interval for the kline, the int value represents the time in seconds
    /// 抹茶Socket连接V3版使用的K线周期枚举
    /// </summary>
    public enum MexcV3StreamsKlineInterval
    {
        /// <summary>
        /// Min1
        /// </summary>
        OneMinute = 60,

        /// <summary>
        /// Min5
        /// </summary>
        FiveMinutes = 60 * 5,

        /// <summary>
        /// Min15
        /// </summary>
        FifteenMinutes = 60 * 15,

        /// <summary>
        /// Min30
        /// </summary>
        ThirtyMinutes = 60 * 30,

        /// <summary>
        /// Min60
        /// </summary>
        OneHour = 60 * 60,

        /// <summary>
        /// Hour4
        /// </summary>
        FourHour = 60 * 60 * 4,

        /// <summary>
        /// Hour8
        /// </summary>
        EightHour = 60 * 60 * 8,

        /// <summary>
        /// Day1
        /// </summary>
        OneDay = 60 * 60 * 24,

        /// <summary>
        /// Week1
        /// </summary>
        OneWeek = 60 * 60 * 24 * 7,

        /// <summary>
        /// Month1
        /// </summary>
        OneMonth = 60 * 60 * 24 * 30
    }
}
