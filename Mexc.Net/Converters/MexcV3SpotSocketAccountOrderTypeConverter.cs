using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Mexc.Net.Enums;

namespace Mexc.Net.Converters
{
    internal class MexcV3SpotSocketAccountOrderTypeConverter : JsonConverter
    {
        private readonly bool quotes;

        public MexcV3SpotSocketAccountOrderTypeConverter()
        {
            quotes = true;
        }

        public MexcV3SpotSocketAccountOrderTypeConverter(bool useQuotes)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<MexcV3SpotSocketAccountOrderType, int> values = new Dictionary<MexcV3SpotSocketAccountOrderType, int>
        {
            { MexcV3SpotSocketAccountOrderType.LIMIT, 1 },
            { MexcV3SpotSocketAccountOrderType.LIMIT_MAKER, 2 },
            { MexcV3SpotSocketAccountOrderType.IMMEDIATE_OR_CANCEL, 3 },
            { MexcV3SpotSocketAccountOrderType.FILL_OR_KILL, 4 },
            { MexcV3SpotSocketAccountOrderType.MARKET, 5 },
            { MexcV3SpotSocketAccountOrderType.STOP_LIMIT, 100 }

        };

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MexcV3SpotSocketAccountOrderType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
            int Value = Convert.ToInt32(reader.Value.ToString());
            switch (Value)
            {
                case 1:
                    return MexcV3SpotSocketAccountOrderType.LIMIT;
                case 2:
                    return MexcV3SpotSocketAccountOrderType.LIMIT_MAKER;
                case 3:
                    return MexcV3SpotSocketAccountOrderType.IMMEDIATE_OR_CANCEL;
                case 4:
                    return MexcV3SpotSocketAccountOrderType.FILL_OR_KILL;
                case 5:
                    return MexcV3SpotSocketAccountOrderType.MARKET;
                case 100:
                    return MexcV3SpotSocketAccountOrderType.STOP_LIMIT;
                default:
                    return null;
            }

            //return values.Single(v => v.Value == (int?)reader.Value).Key;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(MexcV3SpotSocketAccountOrderType)value!]);
            else
                writer.WriteRawValue(values[(MexcV3SpotSocketAccountOrderType)value!].ToString());
        }
    }
}
