﻿using CryptoExchange.Net.Attributes;

namespace Mexc.Net.Enums
{
    /// <summary>
    /// Staking type
    /// </summary>
    public enum StakingProductType
    {
        /// <summary>
        /// Locked staking
        /// </summary>
        [Map("STAKING")]
        Staking,
        /// <summary>
        /// Flexible DeFi staking
        /// </summary>
        [Map("F_DEFI")]
        FlexibleDeFi,
        /// <summary>
        /// Locked DeFi staking
        /// </summary>
        [Map("L_DEFI")]
        LockedDeFi
    }
}
