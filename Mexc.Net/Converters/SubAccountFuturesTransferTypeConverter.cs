using System.Collections.Generic;
using Mexc.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Mexc.Net.Converters
{
    internal class SubAccountFuturesTransferTypeConverter : BaseConverter<SubAccountFuturesTransferType>
    {
        public SubAccountFuturesTransferTypeConverter() : this(true)
        {
        }

        public SubAccountFuturesTransferTypeConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<SubAccountFuturesTransferType, string>> Mapping =>
            new List<KeyValuePair<SubAccountFuturesTransferType, string>>
            {
                new KeyValuePair<SubAccountFuturesTransferType, string>(
                    SubAccountFuturesTransferType.FromSpotToFutures, "1"),
                new KeyValuePair<SubAccountFuturesTransferType, string>(
                    SubAccountFuturesTransferType.FromFuturesToSpot, "2"),
            };
    }
}