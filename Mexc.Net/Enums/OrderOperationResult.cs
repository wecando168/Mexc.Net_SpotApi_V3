﻿using CryptoExchange.Net.Attributes;

namespace Mexc.Net.Enums
{
    /// <summary>
    /// Operation result
    /// </summary>
    public enum OrderOperationResult
    {
        /// <summary>
        /// Successful
        /// </summary>
        [Map("SUCCESS")]
        Success,
        /// <summary>
        /// Failed
        /// </summary>
        [Map("FAILURE")]
        Failure,
        /// <summary>
        /// Not attempted
        /// </summary>
        [Map("NOT_ATTEMPTED")]
        NotAttempted
    }
}
