﻿namespace Mexc.Net.Enums
{
    /// <summary>
    /// Status of a transfer between spot and futures account
    /// </summary>
    public enum FuturesTransferStatus
    {
        /// <summary>
        /// Pending to execute
        /// </summary>
        Pending,
        /// <summary>
        /// Successfully transferred
        /// </summary>
        Confirmed,
        /// <summary>
        /// Execution failed
        /// </summary>
        Failed
    }
}
