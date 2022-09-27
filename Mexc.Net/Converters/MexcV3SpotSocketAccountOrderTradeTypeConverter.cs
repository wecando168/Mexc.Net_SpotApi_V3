using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Mexc.Net.Enums;

namespace Mexc.Net.Converters
{
    internal class MexcV3SpotSocketAccountOrderTradeTypeConverter : JsonConverter
    {
        private readonly bool quotes;

        public MexcV3SpotSocketAccountOrderTradeTypeConverter()
        {
            quotes = true;
        }

        public MexcV3SpotSocketAccountOrderTradeTypeConverter(bool useQuotes)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<MexcV3SpotSocketAccountOrderTradeType, int> values = new Dictionary<MexcV3SpotSocketAccountOrderTradeType, int>
        {
            { MexcV3SpotSocketAccountOrderTradeType.buy, 1 },
            { MexcV3SpotSocketAccountOrderTradeType.sell, 2 }

        };

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(MexcV3SpotSocketAccountOrderTradeType);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {

            int Value = Convert.ToInt32(reader.Value.ToString());
            switch (Value)
            {
                case 1:
                    return MexcV3SpotSocketAccountOrderTradeType.buy;
                case 2:
                    return MexcV3SpotSocketAccountOrderTradeType.sell;
                default:
                    return null;

            }

            //V3SpotSocketAccountOrderTradeType tradeType =  values.Single(v => v.Value == (int?)reader.Value).Key;
            //return tradeType;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(MexcV3SpotSocketAccountOrderTradeType)value!]);
            else
                writer.WriteRawValue(values[(MexcV3SpotSocketAccountOrderTradeType)value!].ToString());
        }
    }
}
