using CryptoExchange.Net.Attributes;

namespace Mexc.Net.Enums
{
    /// <summary>
    /// Order urgency
    /// </summary>
    public enum OrderUrgency
    {
        /// <summary>
        /// Low urgency
        /// </summary>
        [Map("LOW")]
        Low,
        /// <summary>
        /// Medium urgency
        /// </summary>
        [Map("MEDIUM")]
        Medium,
        /// <summary>
        /// High urgency
        /// </summary>
        [Map("HIGH")]
        High
    }
}
