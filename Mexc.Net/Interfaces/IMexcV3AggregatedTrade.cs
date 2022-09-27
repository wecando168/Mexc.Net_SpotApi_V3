using System;

namespace Mexc.Net.Interfaces
{
    /// <summary>
    /// Compressed aggregated trade information. Trades that fill at the time, from the same order, with the same price will have the quantity aggregated.
    /// </summary>
    public interface IMexcV3AggregatedTrade
    {
        /// <summary>
        /// The id of this aggregation
        /// 归集成交ID:a
        /// </summary>
        string OrderId { get; set; }

        /// <summary>
        /// The first trade id in this aggregation
        /// 被归集的首个成交ID:f
        /// </summary>
        string FirstTradeId { get; set; }

        /// <summary>
        /// Last tradeId
        /// 被归集的末个成交ID
        /// </summary>
        string LastTradeId { get; set; }

        /// <summary>
        /// The price of trades in this aggregation
        /// 成交价:p
        /// </summary>
        decimal Price { get; set; }

        /// <summary>
        /// The total quantity of trades in the aggregation
        /// 成交量:q
        /// </summary>
        decimal Quantity { get; set; }

        /// <summary>
        /// The timestamp of the trades
        /// 成交时间:T
        /// </summary>
        DateTime TradeTime { get; set; }

        /// <summary>
        /// Whether the buyer was the maker
        /// 是否为主动卖出单:m
        /// </summary>
        bool BuyerIsMaker { get; set; }

        /// <summary>
        /// Was the trade the best price match?
        /// 是否为最优撮合单(可忽略，目前总为最优撮合):M
        /// </summary>
        bool WasBestPriceMatch { get; set; }
    }
}
