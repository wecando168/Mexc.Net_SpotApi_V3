using System.Collections.Generic;
using Mexc.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Mexc.Net.Converters
{
    internal class MexcV3RestKlineIntervalConverter: BaseConverter<MexcV3RestKlineInterval>
    {
        public MexcV3RestKlineIntervalConverter(): this(true) { }
        public MexcV3RestKlineIntervalConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<MexcV3RestKlineInterval, string>> Mapping => new List<KeyValuePair<MexcV3RestKlineInterval, string>>
        {
            new KeyValuePair<MexcV3RestKlineInterval, string>(MexcV3RestKlineInterval.OneMinute, "1m"),
            new KeyValuePair<MexcV3RestKlineInterval, string>(MexcV3RestKlineInterval.ThreeMinutes, "3m"),
            new KeyValuePair<MexcV3RestKlineInterval, string>(MexcV3RestKlineInterval.FiveMinutes, "5m"),
            new KeyValuePair<MexcV3RestKlineInterval, string>(MexcV3RestKlineInterval.FifteenMinutes, "15m"),
            new KeyValuePair<MexcV3RestKlineInterval, string>(MexcV3RestKlineInterval.ThirtyMinutes, "30m"),
            new KeyValuePair<MexcV3RestKlineInterval, string>(MexcV3RestKlineInterval.OneHour, "1h"),
            new KeyValuePair<MexcV3RestKlineInterval, string>(MexcV3RestKlineInterval.TwoHour, "2h"),
            new KeyValuePair<MexcV3RestKlineInterval, string>(MexcV3RestKlineInterval.FourHour, "4h"),
            new KeyValuePair<MexcV3RestKlineInterval, string>(MexcV3RestKlineInterval.SixHour, "6h"),
            new KeyValuePair<MexcV3RestKlineInterval, string>(MexcV3RestKlineInterval.EightHour, "8h"),
            new KeyValuePair<MexcV3RestKlineInterval, string>(MexcV3RestKlineInterval.TwelveHour, "12h"),
            new KeyValuePair<MexcV3RestKlineInterval, string>(MexcV3RestKlineInterval.OneDay, "1d"),
            new KeyValuePair<MexcV3RestKlineInterval, string>(MexcV3RestKlineInterval.ThreeDay, "3d"),
            new KeyValuePair<MexcV3RestKlineInterval, string>(MexcV3RestKlineInterval.OneWeek, "1w"),
            new KeyValuePair<MexcV3RestKlineInterval, string>(MexcV3RestKlineInterval.OneMonth, "1M")
        };
    }
}
