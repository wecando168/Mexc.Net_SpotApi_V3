using Mexc.Net.Enums;

namespace Mexc.Net.Interfaces
{
    /// <summary>
    /// Stream kline data
    /// </summary>
    public interface IMexcV3StreamKlineDetail : IMexcV3Kline 
    {
        /// <summary>
        /// Interval
        /// K线间隔
        /// </summary>
        MexcV3StreamsKlineInterval Interval { get; set; }

        /// <summary>
        /// The Symbol this candlestick closed
        /// 这根K线的交易代码
        /// </summary>
        string Symbol { get; set; }
    }
}
