using System.Collections.Generic;
using Mexc.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Mexc.Net.Converters
{
    internal class FuturesAccountTypeConverter : BaseConverter<FuturesAccountType>
    {
        public FuturesAccountTypeConverter() : this(false) { }
        public FuturesAccountTypeConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<FuturesAccountType, string>> Mapping => new List<KeyValuePair<FuturesAccountType, string>>
        {
            new KeyValuePair<FuturesAccountType, string>(FuturesAccountType.UsdtMarginedFutures, "1"),
            new KeyValuePair<FuturesAccountType, string>(FuturesAccountType.CoinMarginedFutures, "2"),
        };
    }
}
