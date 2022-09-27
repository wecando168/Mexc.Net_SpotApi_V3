using System;
using Mexc.Net.Interfaces;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models
{
    /// <summary>
    /// Candlestick information for symbol
    /// 数据结构参考：https://mxcdevelop.github.io/apidocs/spot_v3_cn/#k
    /// （实现数组结构到IMexcV3Kline结构的转换）
    /// </summary>
    public abstract class MexcV3KlineBase : IMexcV3Kline 
    {
        /// <summary>
        /// The time this candlestick opened
        /// 开盘时间
        /// </summary>
        [ArrayProperty(0), JsonConverter(typeof(DateTimeConverter))]
        public DateTime OpenTime { get; set; }

        /// <summary>
        /// The price at which this candlestick opened
        /// 开盘价
        /// </summary>
        [ArrayProperty(1)]
        public decimal OpenPrice { get; set; }

        /// <summary>
        /// The highest price in this candlestick
        /// 最高价
        /// </summary>
        [ArrayProperty(2)]
        public decimal HighPrice { get; set; }

        /// <summary>
        /// The lowest price in this candlestick
        /// 最低价
        /// </summary>
        [ArrayProperty(3)]
        public decimal LowPrice { get; set; }

        /// <summary>
        /// The price at which this candlestick closed
        /// 收盘价
        /// </summary>
        [ArrayProperty(4)]
        public decimal ClosePrice { get; set; }

        /// <summary>
        /// The volume traded during this candlestick
        /// 成交量
        /// </summary>
        [ArrayProperty(5)]
        public abstract decimal Volume { get; set; }

        /// <summary>
        /// The close time of this candlestick
        /// 收盘时间
        /// </summary>
        [ArrayProperty(6), JsonConverter(typeof(DateTimeConverter))]
        public DateTime CloseTime { get; set; }

        /// <summary>
        /// The volume traded during this candlestick in the asset form
        /// 成交额
        /// </summary>
        [ArrayProperty(7)]
        public abstract decimal QuoteVolume { get; set; }
    }
}
