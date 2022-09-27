using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Mexc.Net.Enums;

namespace Mexc.Net.Converters
{
    internal class MexcV3SpotSocketAccountOrderMakerOrTakerConverter : JsonConverter
    {
        private readonly bool quotes;

        public MexcV3SpotSocketAccountOrderMakerOrTakerConverter()
        {
            quotes = true;
        }

        public MexcV3SpotSocketAccountOrderMakerOrTakerConverter(bool useQuotes)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<MexcV3SpotSocketAccountOrderMakerOrTaker, int> values = new Dictionary<MexcV3SpotSocketAccountOrderMakerOrTaker, int>
        {
            { MexcV3SpotSocketAccountOrderMakerOrTaker.isTaker, 0 },
            { MexcV3SpotSocketAccountOrderMakerOrTaker.isMaker, 1 }
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
                case 0:
                    return MexcV3SpotSocketAccountOrderMakerOrTaker.isTaker;
                case 1:
                    return MexcV3SpotSocketAccountOrderMakerOrTaker.isMaker;
                default:
                    return null;
            }

            //return values.Single(v => v.Value == (int?)reader.Value).Key;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(MexcV3SpotSocketAccountOrderMakerOrTaker)value!]);
            else
                writer.WriteRawValue(values[(MexcV3SpotSocketAccountOrderMakerOrTaker)value!].ToString());
        }
    }
}
