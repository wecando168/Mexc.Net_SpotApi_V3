using Mexc.Net.Interfaces;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models.Spot
{
    /// <summary>
    /// Price statistics of the last 24 hours
    /// </summary>
    public class MexcV324HPrice : MexcV324HPriceBase, IMexcV3Tick
    {
        /// <summary>
        /// The close price 24 hours ago
        /// 前一收盘价
        /// </summary>
        [JsonProperty("prevClosePrice")]
        public decimal PrevDayClosePrice { get; set; }

        /// <summary>
        /// The best bid price in the order book
        /// 买盘价格
        /// </summary>
        [JsonProperty("bidPrice")]
        public decimal BestBidPrice { get; set; }

        /// <summary>
        /// The quantity of the best bid price in the order book
        /// 买盘数量
        /// </summary>
        [JsonProperty("bidQty")]
        public decimal BestBidQuantity { get; set; }

        /// <summary>
        /// The best ask price in the order book
        /// 卖盘价格
        /// </summary>
        [JsonProperty("askPrice")]
        public decimal BestAskPrice { get; set; }

        /// <summary>
        /// The quantity of the best ask price in the order book
        /// 卖盘数量
        /// </summary>
        [JsonProperty("askQty")]
        public decimal BestAskQuantity { get; set; }
    }
}

//request response reference
/*
{
    "symbol": "BTCUSDT",                    //交易代码      IMexcV3MiniTick
    "priceChange": "-132.4",                //价格变化      IMexcV324HPrice
    "priceChangePercent": "-0.00618586",    //价格变化比    IMexcV324HPrice
    "prevClosePrice": "21403.64",           //前一收盘价    MexcV324HPrice
    "lastPrice": "21271.24",                //最新价        IMexcV3MiniTick
    "bidPrice": "21272.98",                 //买盘价格      MexcV324HPrice
    "bidQty": "0.04862",                    //买盘数量      MexcV324HPrice
    "askPrice": "21272.99",                 //卖盘价格      MexcV324HPrice
    "askQty": "0.006996",                   //卖盘数量      MexcV324HPrice
    "openPrice": "21403.64",                //开始价        IMexcV3MiniTick
    "highPrice": "21781.25",                //最高价        IMexcV3MiniTick
    "lowPrice": "20900",                    //最低价        IMexcV3MiniTick
    "volume": "19879.90012",                //成交量        IMexcV3MiniTick
    "quoteVolume": "424979572.72",          //成交额        IMexcV3MiniTick
    "openTime": 1661182500000,              //开始时间      IMexcV324HPrice
    "closeTime": 1661182761346,             //结束时间      IMexcV324HPrice
    "count": null
}
 */