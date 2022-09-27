namespace Mexc.Net.Enums
{
    /// <summary>
    /// Universal transfer type
    /// </summary>
    public enum UniversalTransferType
    {
        /// <summary>
        /// Main (spot) to Funding
        /// </summary>
        MainToFunding,
        /// <summary>
        /// Main (spot) to Usd Futures
        /// </summary>
        MainToFutures,
        /// <summary>
        /// Main (spot) to Margin
        /// </summary>
        MainToMargin,
        /// <summary>
        /// Main (spot) to Mining
        /// </summary>
        MainToMining,

        /// <summary>
        /// Funding to Main (spot)
        /// </summary>
        FundingToMain,
        /// <summary>
        /// Funding to futures
        /// </summary>
        FundingToFutures,
        /// <summary>
        /// Funding to margin
        /// </summary>
        FundingToMargin,

        /// <summary>
        /// Futures to Main (spot)
        /// </summary>
        FuturesToMain,
        /// <summary>
        /// Futures to Funding
        /// </summary>
        FuturesToFunding,
        /// <summary>
        /// Futures to Margin
        /// </summary>
        FuturesToMargin,

        /// <summary>
        /// Margin to Main (spot)
        /// </summary>
        MarginToMain,
        /// <summary>
        /// Margin to futures
        /// </summary>
        MarginToFutures,
        /// <summary>
        /// Margin to Mining
        /// </summary>
        MarginToMining,
        /// <summary>
        /// Margin to Funding
        /// </summary>
        MarginToFunding,

        /// <summary>
        /// Isolated margin to margin
        /// </summary>
        IsolatedMarginToMargin,
        /// <summary>
        /// Margin to isolated margin
        /// </summary>
        MarginToIsolatedMargin,
        /// <summary>
        /// Isolated margin to Isolated margin
        /// </summary>
        IsolatedMarginToIsolatedMargin,

        /// <summary>
        /// Mining to Main (spot)
        /// </summary>
        MiningToMain,
        /// <summary>
        /// Mining to futures
        /// </summary>
        MiningToFutures,
        /// <summary>
        /// Mining to Margin
        /// </summary>
        MiningToMargin,
    }
}