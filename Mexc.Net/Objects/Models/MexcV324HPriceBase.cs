﻿using System;
using Mexc.Net.Interfaces;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models
{
    /// <summary>
    /// 24 hour rolling window price data
    /// </summary>
    public abstract class MexcV324HPriceBase : IMexcV324HPrice
    {
        /// <summary>
        /// The symbol the price is for
        /// 交易对
        /// </summary>
        [JsonProperty("symbol")] 
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The actual price change in the last 24 hours
        /// 价格变化
        /// </summary>
        [JsonProperty("priceChange")] 
        public decimal PriceChange { get; set; }

        /// <summary>
        /// The price change in percentage in the last 24 hours
        /// 价格变化比
        /// </summary>
        [JsonProperty("priceChangePercent")] 
        public decimal PriceChangePercent { get; set; }

        /// <summary>
        /// The weighted average price in the last 24 hours
        /// MEXC 没有这个参数
        /// </summary>
        [JsonProperty("weightedAvgPrice")]
        public decimal WeightedAveragePrice { get; set; }

        /// <summary>
        /// The most recent trade price
        /// 最新价
        /// </summary>
        [JsonProperty("lastPrice")] 
        public decimal LastPrice { get; set; }

        /// <summary>
        /// The most recent trade quantity
        /// 最新量
        /// </summary>
        [JsonProperty("lastQty")]
        public decimal LastQuantity { get; set; }

        /// <summary>
        /// The open price 24 hours ago
        /// 开始价
        /// </summary>
        [JsonProperty("openPrice")] 
        public decimal OpenPrice { get; set; }

        /// <summary>
        /// The highest price in the last 24 hours
        /// 最高价
        /// </summary>
        [JsonProperty("highPrice")] 
        public decimal HighPrice { get; set; }

        /// <summary>
        /// The lowest price in the last 24 hours
        /// 最低价
        /// </summary>
        [JsonProperty("lowPrice")] 
        public decimal LowPrice { get; set; }

        /// <summary>
        /// The base volume traded in the last 24 hours
        /// 成交量
        /// </summary>
        public abstract decimal Volume { get; set; }

        /// <summary>
        /// The quote asset volume traded in the last 24 hours
        /// 成交额
        /// </summary>
        public abstract decimal QuoteVolume { get; set; }

        /// <summary>
        /// Time at which this 24 hours opened
        /// 开始时间
        /// </summary>
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime OpenTime { get; set; }

        /// <summary>
        /// Time at which this 24 hours closed
        /// 结束时间
        /// </summary>
        [JsonConverter(typeof(DateTimeConverter))]
        public DateTime CloseTime { get; set; }

        /// <summary>
        /// The first trade ID in the last 24 hours
        /// </summary>
        [JsonProperty("firstId")]
        public long FirstTradeId { get; set; }

        /// <summary>
        /// The last trade ID in the last 24 hours
        /// </summary>
        [JsonProperty("lastId")]
        public long LastTradeId { get; set; }

        /// <summary>
        /// The amount of trades made in the last 24 hours
        /// </summary>
        [JsonProperty("count")]
        public long? TotalTrades { get; set; }
    }
}
