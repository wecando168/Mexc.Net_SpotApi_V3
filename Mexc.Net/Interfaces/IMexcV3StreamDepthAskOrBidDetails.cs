using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Mexc.Net.Interfaces
{
    /// <summary>
    /// asks:卖单 bids:买单 明细
    /// </summary>
    public interface IMexcV3StreamDepthAskOrBidDetails
    {
        /// <summary>
        /// The price of the change
        /// 变动的价格档位
        /// </summary>
        decimal Price { get; set; }

        /// <summary>
        /// The quantity of the change
        /// 数量
        /// </summary>
        decimal Quantity { get; set; }
    }
}
