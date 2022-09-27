using CryptoExchange.Net.Converters;
using Mexc.Net.Converters;
using Mexc.Net.Enums;
using Mexc.Net.Objects.Models.Spot;
using Newtonsoft.Json;
using System;

namespace Mexc.Net.Interfaces
{
    /// <summary>
    /// Stream kline data
    /// </summary>
    public class MexcV3StreamKlineDetail: IMexcV3StreamKlineDetail
    {
        /// <summary>
        /// The close time of this candlestick
        /// 这根K线的结束时间
        /// </summary>
        [JsonProperty("T"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime CloseTime { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// 这根K线期间成交额
        /// </summary>
        [JsonProperty("a")]
        public decimal QuoteVolume { get; set; }

        /// <summary>
        /// The close price of this candlestick
        /// 这根K线期间末一笔成交价
        /// </summary>
        [JsonProperty("c")]
        public decimal ClosePrice { get; set; }

        /// <summary>
        /// The highest price of this candlestick
        /// 这根K线期间最高成交价
        /// </summary>
        [JsonProperty("h")]
        public decimal HighPrice { get; set; }

        /// <summary>
        /// The interval of this candlestick
        /// K线间隔
        /// </summary>
        [JsonProperty("i"), JsonConverter(typeof(MexcV3StreamsKlineIntervalConverter))]
        public MexcV3StreamsKlineInterval Interval { get; set; }

        /// <summary>
        /// The lowest price of this candlestick
        ///  这根K线期间最低成交价
        /// </summary>
        [JsonProperty("l")]
        public decimal LowPrice { get; set; }

        /// <summary>
        /// The open price of this candlestick
        /// 这根K线期间第一笔成交价
        /// </summary>
        [JsonProperty("o")]
        public decimal OpenPrice { get; set; }

        /// <summary>
        /// The symbol of this candlestick
        /// 交易代码
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The open time of this candlestick
        /// 这根K线的起始时间
        /// </summary>
        [JsonProperty("t"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime OpenTime { get; set; }

        /// <inheritdoc />
        /// <summary>
        /// 这根K线期间成交量
        /// </summary>
        [JsonProperty("v")]
        public decimal Volume { get; set; }

        /// <summary>
        /// Casts this object to a <see cref="MexcV3SpotKline"/> object
        /// </summary>
        /// <returns></returns>
        public MexcV3SpotKline ToKline()
        {
            return new MexcV3SpotKline
            {
                OpenPrice = OpenPrice,
                ClosePrice = ClosePrice,
                Volume = Volume,
                CloseTime = CloseTime,
                HighPrice = HighPrice,
                LowPrice = LowPrice,
                OpenTime = OpenTime,
                QuoteVolume = QuoteVolume,
            };
        }
    }
}
