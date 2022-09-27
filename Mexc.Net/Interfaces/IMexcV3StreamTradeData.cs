using CryptoExchange.Net.Converters;
using Mexc.Net.Converters;
using Mexc.Net.Enums;
using Mexc.Net.Objects.Models.Spot.Socket;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Mexc.Net.Interfaces
{
    /// <summary>
    /// Stream Trade data
    /// 逐比交易数据流返回的实际数据接口
    /// </summary>
    public interface IMexcV3StreamTradeData<T>
    {
        /// <summary>
        /// The event timestamp of this trade 
        /// 事件时间eventTime
        /// </summary>
        DateTime EventTime { get; set; }

        /// <summary>
        /// The deals of this trade data
        /// 交易明细
        /// </summary>
        IEnumerable<T> Deals { get; set; }

        /// <summary>
        /// The event type of this trade data
        /// 事件类型EventType
        /// </summary>
        string EventType { get; set; }

        /// <summary>
        /// The symbol of this trade data
        /// 数据交易代码Symbol
        /// </summary>
        string Symbol { get; set; }
    }
}
