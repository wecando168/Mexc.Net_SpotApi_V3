using System;

namespace Mexc.Net.Interfaces
{
    /// <summary>
    /// Kline data
    /// 数据结构参考：https://mxcdevelop.github.io/apidocs/spot_v3_cn/#k
    /// </summary>
    public interface IMexcV3Kline
    {
        /// <summary>
        /// The time this candlestick opened
        /// 开盘时间
        /// </summary>
        DateTime OpenTime { get; set; }

        /// <summary>
        /// The price at which this candlestick opened
        /// 开盘价
        /// </summary>
        decimal OpenPrice { get; set; }

        /// <summary>
        /// The highest price in this candlestick
        /// 最高价
        /// </summary>
        decimal HighPrice { get; set; }

        /// <summary>
        /// The lowest price in this candlestick
        /// 最低价
        /// </summary>
        decimal LowPrice { get; set; }

        /// <summary>
        /// The price at which this candlestick closed
        /// 收盘价
        /// </summary>
        decimal ClosePrice { get; set; }

        /// <summary>
        /// The volume traded during this candlestick
        /// 成交量
        /// </summary>
        decimal Volume { get; set; }

        /// <summary>
        /// The close time of this candlestick
        /// 收盘时间
        /// </summary>
        DateTime CloseTime { get; set; }

        /// <summary>
        /// The volume traded during this candlestick in the asset form
        /// 成交额 计价货币成交量
        /// </summary>
        decimal QuoteVolume { get; set; }
    }
}