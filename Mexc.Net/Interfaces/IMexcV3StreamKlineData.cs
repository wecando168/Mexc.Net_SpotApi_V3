using System;
using System.Collections.Generic;

namespace Mexc.Net.Interfaces
{
    /// <summary>
    /// Stream kline data
    /// </summary>
    public interface IMexcV3StreamKlineData<T>
    {
        /// <summary>
        /// The event timestamp of this candlestick data
        /// 事件时间eventTime
        /// </summary>
        DateTime EventTime { get; set; }

        /// <summary>
        /// The candlestick Detail of this candlestick data
        /// K线明细
        /// </summary>
        T Details { get; set; }

        /// <summary>
        /// The event type of this candlestick data
        /// 事件类型EventType
        /// </summary>
        string EventType { get; set; }

        /// <summary>
        /// The symbol of this candlestick data
        /// K线交易代码Symbol
        /// </summary>
        string Symbol { get; set; }
    }
}
