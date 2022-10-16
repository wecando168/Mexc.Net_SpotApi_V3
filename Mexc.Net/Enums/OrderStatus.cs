namespace Mexc.Net.Enums
{
    /// <summary>
    /// The status of an orderн
    /// </summary>
    public enum OrderStatus
    {
        /// <summary>
        /// Is new Uncompleted Order:NEW
        /// 未成交的新订单
        /// </summary>
        New,

        /// <summary>
        /// Order is partly filled, still has quantity left to fill:PARTIALLY_FILLED
        /// 部分成交
        /// </summary>
        /// 
        PartiallyFilled,

        /// <summary>
        /// The order has been filled and completed:FILLED
        /// 已成交
        /// </summary>
        Filled,

        /// <summary>
        /// The order has been canceled:CANCELED
        /// 已撤销
        /// </summary>
        Canceled,

        /// <summary>
        /// The order is in the process of being canceled:PARTIALLY_CANCELED
        /// 部分撤销
        /// </summary>
        PartiallyCanceled,

        /// <summary>
        /// The order has been rejected:EXECUTED
        /// 已执行
        /// </summary>
        Eexcuted
    }
}
