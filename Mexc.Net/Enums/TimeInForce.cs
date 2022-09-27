using CryptoExchange.Net.Attributes;

namespace Mexc.Net.Enums
{
    /// <summary>
    /// The time the order will be active for
    /// </summary>
    public enum TimeInForce
    {
        /// <summary>
        /// GoodTillCanceled orders will stay active until they are filled or canceled
        /// </summary>
        [Map("GTC")]
        GoodTillCancel,
        /// <summary>
        /// ImmediateOrCancel orders have to be at least partially filled upon placing or will be automatically canceled
        /// </summary>
        [Map("IOC")]
        ImmediateOrCancel,
        /// <summary>
        /// FillOrKill orders have to be entirely filled upon placing or will be automatically canceled
        /// </summary>
        [Map("FOK")]
        FillOrKill,
        /// <summary>
        /// Should enter the book upon placing
        /// </summary>
        [Map("BOC")]
        BookOrCancel,
        /// <summary>
        /// Good Till Crossing
        /// </summary>
        [Map("GTX")]
        GoodTillCrossing,
        /// <summary>
        /// Good Till Expired Or Canceled
        /// </summary>
        [Map("GTE_GTC")]
        GoodTillExpiredOrCanceled,
    }
}
