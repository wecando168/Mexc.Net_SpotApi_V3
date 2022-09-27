using System.Collections.Generic;
using CryptoExchange.Net.Converters;
using Mexc.Net.Enums;

namespace Mexc.Net.Converters
{
    internal class MarginStatusConverter : BaseConverter<MarginStatus>
    {
        public MarginStatusConverter(): this(false) { }
        public MarginStatusConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<MarginStatus, string>> Mapping => new List<KeyValuePair<MarginStatus, string>>
        {
            new KeyValuePair<MarginStatus, string>(MarginStatus.Pending, "PENDING"),
            new KeyValuePair<MarginStatus, string>(MarginStatus.Completed, "COMPLETED"),
            new KeyValuePair<MarginStatus, string>(MarginStatus.Confirmed , "CONFIRMED"),
            new KeyValuePair<MarginStatus, string>(MarginStatus.Failed, "FAILED"),
        };
    }
}
