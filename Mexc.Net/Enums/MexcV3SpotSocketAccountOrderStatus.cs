namespace Mexc.Net.Enums
{
    /// <summary>
    /// V3 Spot Socket Account Order Type
    /// </summary>
    public enum MexcV3SpotSocketAccountOrderStatus
    {
        /// <summary>
        /// 未成交(1)
        /// </summary>
        NewOrder = 1,

        /// <summary>
        /// 已成交(2)
        /// </summary>
        Filled = 2,

        /// <summary>
        /// 部分成交(3)
        /// </summary>
        PartiallyFilled = 3,

        /// <summary>
        /// 已撤单(4)
        /// </summary>
        OrderCanceled = 4,

        /// <summary>
        /// 部分撤单(5)
        /// </summary>
        OrderFilledPartially = 5
    }
}
