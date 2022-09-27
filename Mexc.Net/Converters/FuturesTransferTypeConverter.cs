using System.Collections.Generic;
using Mexc.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Mexc.Net.Converters
{
    internal class FuturesTransferTypeConverter: BaseConverter<FuturesTransferType>
    {
        public FuturesTransferTypeConverter() : this(true) { }
        public FuturesTransferTypeConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<FuturesTransferType, string>> Mapping => new List<KeyValuePair<FuturesTransferType, string>>
        {
            new KeyValuePair<FuturesTransferType, string>(FuturesTransferType.FromSpotToFutures, "1"),
            new KeyValuePair<FuturesTransferType, string>(FuturesTransferType.FromFuturesToSpot, "2"),            
        };
    }
}
