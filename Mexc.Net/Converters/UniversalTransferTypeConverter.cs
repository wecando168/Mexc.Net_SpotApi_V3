using Mexc.Net.Enums;
using CryptoExchange.Net.Converters;
using System.Collections.Generic;

namespace Mexc.Net.Converters
{
    internal class UniversalTransferTypeConverter : BaseConverter<UniversalTransferType>
    {
        public UniversalTransferTypeConverter() : this(true) { }
        public UniversalTransferTypeConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<UniversalTransferType, string>> Mapping => new List<KeyValuePair<UniversalTransferType, string>>
        {
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MainToFunding, "MAIN_FUNDING"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MainToFutures, "MAIN_FUTURE"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MainToMargin, "MAIN_MARGIN"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MainToMining, "MAIN_MINING"),

            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.FundingToMain, "FUNDING_MAIN"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.FundingToFutures, "FUNDING_FUTURE"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.FundingToMargin, "FUNDING_MARGIN"),

            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.FuturesToMain, "FUTURE_MAIN"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.FuturesToFunding, "FUTURE_FUNDING"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.FuturesToMargin, "FUTURE_MARGIN"),

            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MarginToIsolatedMargin, "MARGIN_ISOLATEDMARGIN"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.IsolatedMarginToMargin, "ISOLATEDMARGIN_MARGIN"),

            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MarginToMain, "MARGIN_MAIN"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MarginToFutures, "MARGIN_FUTURE"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MarginToMining, "MARGIN_MINING"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MarginToFunding, "MARGIN_FUNDING"),

            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MiningToMain, "MINING_MAIN"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MiningToFutures, "MINING_FUTURE"),
            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.MiningToMargin, "MINING_MARGIN"),

            new KeyValuePair<UniversalTransferType, string>(UniversalTransferType.IsolatedMarginToIsolatedMargin, "ISOLATEDMARGIN_ISOLATEDMARGIN"),
        };
    }
}