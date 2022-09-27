using System.Collections.Generic;
using Mexc.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Mexc.Net.Converters
{
    internal class TimeInForceConverter : BaseConverter<TimeInForce>
    {
        public TimeInForceConverter(): this(true) { }
        public TimeInForceConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<TimeInForce, string>> Mapping => new List<KeyValuePair<TimeInForce, string>>
        {
            new KeyValuePair<TimeInForce, string>(TimeInForce.GoodTillCancel, "GTC"),
            new KeyValuePair<TimeInForce, string>(TimeInForce.ImmediateOrCancel, "IOC"),
            new KeyValuePair<TimeInForce, string>(TimeInForce.FillOrKill, "FOK"),
            new KeyValuePair<TimeInForce, string>(TimeInForce.GoodTillCrossing, "GTX"),
            new KeyValuePair<TimeInForce, string>(TimeInForce.GoodTillExpiredOrCanceled, "GTE_GTC")
        };
    }
}
