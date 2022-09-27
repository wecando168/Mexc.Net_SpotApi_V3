using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Mexc.Net.Interfaces
{
    /// <summary>
    /// Stream Depth data
    /// </summary>
    public interface IMexcV3StreamDepthData<T>
    {
        /// <summary>
        /// The event timestamp of this Depth 
        /// 事件时间eventTime
        /// </summary>
        DateTime EventTime { get; set; }

        /// <summary>
        /// The event type of this Depth data
        /// 事件类型EventType
        /// </summary>
        string EventType { get; set; }

        /// <summary>
        /// The symbol of this Depth data
        /// 数据交易代码Symbol
        /// </summary>
        string Symbol { get; set; }

        /// <summary>
        /// The version of this Depth data
        /// 当前API版本信息
        /// </summary>
        string Version { get; set; }

        /// <summary>
        /// aks:卖单
        /// </summary>
        IEnumerable<T>? Asks { get; set; }

        /// <summary>
        /// bids:买单
        /// </summary>
        IEnumerable<T>? Bids { get; set; }
    }
}
