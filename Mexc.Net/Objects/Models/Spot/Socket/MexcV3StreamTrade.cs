using Mexc.Net.Interfaces;
using Newtonsoft.Json;
using Mexc.Net.Converters;
using CryptoExchange.Net.Converters;
using System;
using System.Collections.Generic;
using Mexc.Net.Enums;

namespace Mexc.Net.Objects.Models.Spot.Socket
{

    /// <summary>
    /// 订阅逐笔交易的返回数据(测试）
    /// </summary>
    public class MexcV3StreamTrade : IMexcV3Stream<MexcV3StreamTradeData>
    {
        /// <summary>
        /// The Subscription request
        /// 订阅频道（可作为当前请求客户端标识）
        /// </summary>
        [JsonProperty("c")]
        public string Stream { get; set; } = string.Empty;

        /// <summary>
        /// 数据
        /// </summary>
        [JsonProperty("d")]
        public MexcV3StreamTradeData Data { get; set; } = default!;

        /// <summary>
        /// 数据流交易代码Symbol
        /// </summary>
        [JsonProperty("s")]
        public string Symbol { get; set; } = string.Empty;

        /// <summary>
        /// The time the event happened
        /// 事件时间eventTime
        /// </summary>
        [JsonProperty("t")]
        public string? EventTimeStamp { get; set; }
    }
}

/*
{
    "c":"spot@public.deals.v3.api@BTCUSDT",     //当前订阅数据流
    "d":                                        //数据
    {
        "E":1663556442436,                      //事件时间EventTime
        "deals":                                //交易明细列表
        [
            {
                "S":2,                          //交易类型TradeType
                "p":"18767.98",                 //成交价格Price
                "t":1663556442409,              //成交时间DealTime
                "v":"0.096760"                  //成交数量Quantity
            }
        ],
        "e":"spot@public.deals.v3.api",         //事件类型EventType 
        "s":"BTCUSDT"                           //数据交易代码Symbol
    },
    "s":"BTCUSDT",                              //数据流交易代码Symbol
    "t":1663556442436                           //事件时间 EventTimeStep
}
*/