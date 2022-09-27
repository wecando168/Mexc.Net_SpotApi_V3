using CryptoExchange.Net.Attributes;

namespace Mexc.Net.Enums
{
    /// <summary>
    /// Order type for a spot order
    /// </summary>
    public enum SpotOrderType
    {
        /// <summary>
        /// Limit orders will be placed at a specific price. If the price isn't available in the order book for that asset the order will be added in the order book for someone to fill.
        /// 限价单
        /// </summary>
        [Map("LIMIT")]
        Limit,

        /// <summary>
        /// Market order will be placed without a price. The order will be executed at the best price available at that time in the order book.
        /// 市价单
        /// </summary>
        [Map("MARKET")]
        Market,

        /// <summary>
        /// Same as a limit order, however it will fail if the order would immediately match, therefor preventing taker orders
        /// 限价只挂单
        /// </summary>
        [Map("LIMIT_MAKER")]
        LimitMaker,

        /// <summary>
        /// Immediate or cancel
        /// IOC单 (无法立即成交的部分就撤销,订单在失效前会尽量多的成交。)
        /// </summary>
        [Map("IMMEDIATE_OR_CANCEL")]
        IOC,

        /// <summary>
        /// Fill or kill
        /// FOK单 (无法全部立即成交就撤销,如果无法全部成交,订单会失效。)
        /// </summary>
        [Map("FILL_OR_KILL")]
        FillOrKill
    }

    /// <summary>
    /// Order type for a futures order
    /// </summary>
    public enum FuturesOrderType
    {
        /// <summary>
        /// Limit orders will be placed at a specific price. If the price isn't available in the order book for that asset the order will be added in the order book for someone to fill.
        /// </summary>
        [Map("LIMIT")]
        Limit,
        /// <summary>
        /// Market order will be placed without a price. The order will be executed at the best price available at that time in the order book.
        /// </summary>
        [Map("MARKET")]
        Market,
        /// <summary>
        /// Stop order. Execute a limit order when price reaches a specific Stop price
        /// </summary>
        Stop,
        /// <summary>
        /// Stop market order. Execute a market order when price reaches a specific Stop price
        /// </summary>
        StopMarket,
        /// <summary>
        /// Take profit order. Will execute a limit order when the price rises above a price to sell and therefor take a profit
        /// </summary>
        TakeProfit,
        /// <summary>
        /// Take profit market order. Will execute a market order when the price rises above a price to sell and therefor take a profit
        /// </summary>
        TakeProfitMarket,
        /// <summary>
        /// A trailing stop order will execute an order when the price drops below a certain percentage from its all time high since the order was activated
        /// </summary>
        TrailingStopMarket,
        /// <summary>
        /// A liquidation order
        /// </summary>
        Liquidation
    }
}
