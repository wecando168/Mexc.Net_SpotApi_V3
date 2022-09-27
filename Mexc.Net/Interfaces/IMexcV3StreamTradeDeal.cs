using CryptoExchange.Net.Converters;
using Mexc.Net.Converters;
using Mexc.Net.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Mexc.Net.Interfaces
{
    /// <summary>
    /// Stream Trade data deals interface
    /// 逐比交易数据流返回的实际数据交易明细接口
    /// </summary>
    public interface IMexcV3StreamTradeDeal
    {
        /// <summary>
        /// The trade type of the deal
        /// 交易类型TradeType
        /// </summary>
        MexcV3SpotSocketAccountOrderTradeType TradeType { get; set; }

        /// <summary>
        /// The price of the deal
        /// 成交价格Price
        /// </summary>
        decimal Price { get; set; }

        /// <summary>
        /// The time of the deal
        /// 成交时间DealTime
        /// </summary>
        DateTime DealTime { get; set; }

        /// <summary>
        /// The quantity of the deal
        /// 成交数量Quantity
        /// </summary>
        decimal Quantity { get; set; }
    }
}
