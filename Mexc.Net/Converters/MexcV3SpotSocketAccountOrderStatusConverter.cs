using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using Mexc.Net.Enums;

namespace Mexc.Net.Converters
{
    internal class MexcV3SpotSocketAccountOrderStatusConverter : JsonConverter
    {
        private readonly bool quotes;

        public MexcV3SpotSocketAccountOrderStatusConverter()
        {
            quotes = true;
        }

        public MexcV3SpotSocketAccountOrderStatusConverter(bool useQuotes)
        {
            quotes = useQuotes;
        }

        private readonly Dictionary<MexcV3SpotSocketAccountOrderStatus, int> values = new Dictionary<MexcV3SpotSocketAccountOrderStatus, int>
        {
            { MexcV3SpotSocketAccountOrderStatus.NewOrder, 1 },
            { MexcV3SpotSocketAccountOrderStatus.Filled, 2 },
            { MexcV3SpotSocketAccountOrderStatus.PartiallyFilled, 3 },
            { MexcV3SpotSocketAccountOrderStatus.OrderCanceled, 4 },
            { MexcV3SpotSocketAccountOrderStatus.OrderFilledPartially, 5 }

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
                    return MexcV3SpotSocketAccountOrderStatus.NewOrder;
                case 2:
                    return MexcV3SpotSocketAccountOrderStatus.Filled;
                case 3:
                    return MexcV3SpotSocketAccountOrderStatus.PartiallyFilled;
                case 4:
                    return MexcV3SpotSocketAccountOrderStatus.OrderCanceled;
                case 5:
                    return MexcV3SpotSocketAccountOrderStatus.OrderFilledPartially;
                default:
                    return null;
            }


            //return values.Single(v => v.Value == (int?)reader.Value).Key;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            if (quotes)
                writer.WriteValue(values[(MexcV3SpotSocketAccountOrderStatus)value!]);
            else
                writer.WriteRawValue(values[(MexcV3SpotSocketAccountOrderStatus)value!].ToString());
        }
    }
}
