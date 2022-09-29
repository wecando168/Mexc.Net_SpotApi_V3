namespace Mexc.Net.Enums
{
    /// <summary>
    /// V3 Spot Socket Account Order Type
    /// </summary>
    public enum MexcV3SpotSocketAccountOrderType
    {
        /// <summary>
        /// 限价单 LIMIT_ORDER(1)
        /// </summary>
        LIMIT = 1,

        /// <summary>
        /// 仅发送 POST_ONLY(2) LIMIT_MAKER?
        /// </summary>
        LIMIT_MAKER = 2,

        /// <summary>
        /// Immediate or cancel
        /// IOC无法立即成交部分直接撤销的订单 (无法立即成交的部分就撤销,订单在失效前会尽量多的成交。)
        /// IMMEDIATE_OR_CANCEL(3)
        /// </summary>
        IMMEDIATE_OR_CANCEL = 3,

        /// <summary>
        /// FOK无法完全成交则整个订单撤销的订单 FILL_OR_KILL(4)
        /// </summary>
        FILL_OR_KILL = 4,

        /// <summary>
        /// 市价订单 MARKET_ORDER(5)
        /// </summary>
        MARKET = 5,

        /// <summary>
        /// 止盈止损订单 STOP_LIMIT(100)
        /// </summary>
        STOP_LIMIT = 100
    }
}
