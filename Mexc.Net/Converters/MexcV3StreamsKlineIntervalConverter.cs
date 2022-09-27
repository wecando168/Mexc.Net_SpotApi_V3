using System.Collections.Generic;
using Mexc.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Mexc.Net.Converters
{
    internal class MexcV3StreamsKlineIntervalConverter : BaseConverter<MexcV3StreamsKlineInterval>
    {
        public MexcV3StreamsKlineIntervalConverter(): this(true) { }
        public MexcV3StreamsKlineIntervalConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<MexcV3StreamsKlineInterval, string>> Mapping => new List<KeyValuePair<MexcV3StreamsKlineInterval, string>>
        {
            new KeyValuePair<MexcV3StreamsKlineInterval, string>(MexcV3StreamsKlineInterval.OneMinute, "Min1"),
            new KeyValuePair<MexcV3StreamsKlineInterval, string>(MexcV3StreamsKlineInterval.FiveMinutes, "Min5"),
            new KeyValuePair<MexcV3StreamsKlineInterval, string>(MexcV3StreamsKlineInterval.FifteenMinutes, "Min15"),
            new KeyValuePair<MexcV3StreamsKlineInterval, string>(MexcV3StreamsKlineInterval.ThirtyMinutes, "Min30"),
            new KeyValuePair<MexcV3StreamsKlineInterval, string>(MexcV3StreamsKlineInterval.OneHour, "Min60"),
            new KeyValuePair<MexcV3StreamsKlineInterval, string>(MexcV3StreamsKlineInterval.FourHour, "Hour4"),
            new KeyValuePair<MexcV3StreamsKlineInterval, string>(MexcV3StreamsKlineInterval.EightHour, "Hour8"),
            new KeyValuePair<MexcV3StreamsKlineInterval, string>(MexcV3StreamsKlineInterval.OneDay, "Day1"),
            new KeyValuePair<MexcV3StreamsKlineInterval, string>(MexcV3StreamsKlineInterval.OneWeek, "Week1"),
            new KeyValuePair<MexcV3StreamsKlineInterval, string>(MexcV3StreamsKlineInterval.OneMonth, "Month1")
        };
    }
}
