using Mexc.Net.Enums;
using Mexc.Net.Objects.Models.Spot;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Diagnostics;

namespace Mexc.Net.Converters
{
    internal class SymbolFilterConverter : JsonConverter
    {
        public override bool CanConvert(Type objectType)
        {
            return false;
        }

        public override object ReadJson(JsonReader reader, Type objectType, object? existingValue, JsonSerializer serializer)
        {
#pragma warning disable 8604, 8602
            var obj = JObject.Load(reader);
            var type = new SymbolFilterTypeConverter(false).ReadString(obj["filterType"].ToString());
            MexcSymbolFilter result;
            switch (type)
            {
                case SymbolFilterType.LotSize:
                    result = new MexcSymbolLotSizeFilter
                    {
                        MaxQuantity = (decimal)obj["maxQty"],
                        MinQuantity = (decimal)obj["minQty"],
                        StepSize = (decimal)obj["stepSize"]
                    };
                    break;
                case SymbolFilterType.MarketLotSize:
                    result = new MexcSymbolMarketLotSizeFilter
                    {
                        MaxQuantity = (decimal)obj["maxQty"],
                        MinQuantity = (decimal)obj["minQty"],
                        StepSize = (decimal)obj["stepSize"]
                    };
                    break;
                case SymbolFilterType.MinNotional:
                    result = new MexcSymbolMinNotionalFilter
                    {
                        MinNotional = (decimal)obj["minNotional"],
                        ApplyToMarketOrders = (bool)obj["applyToMarket"],
                        AveragePriceMinutes = (int)obj["avgPriceMins"]
                    };
                    break;
                case SymbolFilterType.Price:
                    result = new MexcSymbolPriceFilter
                    {
                        MaxPrice = (decimal)obj["maxPrice"],
                        MinPrice = (decimal)obj["minPrice"],
                        TickSize = (decimal)obj["tickSize"]
                    };
                    break;
                case SymbolFilterType.MaxNumberAlgorithmicOrders:
                    result = new MexcSymbolMaxAlgorithmicOrdersFilter
                    {
                        MaxNumberAlgorithmicOrders = (int)obj["maxNumAlgoOrders"]
                    };
                    break;
                case SymbolFilterType.MaxNumberOrders:
                    result = new MexcSymbolMaxOrdersFilter
                    {
                        MaxNumberOrders = (int)obj["maxNumOrders"]
                    };
                    break;

                case SymbolFilterType.IcebergParts:
                    result = new MexcSymbolIcebergPartsFilter
                    {
                        Limit = (int)obj["limit"]
                    };
                    break;
                case SymbolFilterType.PricePercent:
                    result = new MexcSymbolPercentPriceFilter
                    {
                        MultiplierUp = (decimal)obj["multiplierUp"],
                        MultiplierDown = (decimal)obj["multiplierDown"],
                        AveragePriceMinutes = (int)obj["avgPriceMins"]
                    };
                    break;
                case SymbolFilterType.MaxPosition:
                    result = new MexcSymbolMaxPositionFilter
                    {
                        MaxPosition = obj.ContainsKey("maxPosition") ? (decimal)obj["maxPosition"] : 0
                    };
                    break;
                case SymbolFilterType.PercentagePriceBySide:
                    result = new MexcSymbolPercentPriceBySideFilter
                    {
                        AskMultiplierUp = (decimal)obj["askMultiplierUp"],
                        AskMultiplierDown = (decimal)obj["askMultiplierDown"],
                        BidMultiplierUp = (decimal)obj["bidMultiplierUp"],
                        BidMultiplierDown = (decimal)obj["bidMultiplierDown"],
                        AveragePriceMinutes = (int)obj["avgPriceMins"]
                    };
                    break;
                case SymbolFilterType.TrailingDelta:
                    result = new MexcSymbolTrailingDeltaFilter
                    {
                        MaxTrailingAboveDelta = (int)obj["maxTrailingAboveDelta"],
                        MaxTrailingBelowDelta = (int)obj["maxTrailingBelowDelta"],
                        MinTrailingAboveDelta = (int)obj["minTrailingAboveDelta"],
                        MinTrailingBelowDelta = (int)obj["minTrailingBelowDelta"],
                    };
                    break;
                default:
                    Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff} | Warning | Can't parse symbol filter of type: " + obj["filterType"]);
                    result = new MexcSymbolFilter();
                    break;
            }
#pragma warning restore 8604
            result.FilterType = type;
            return result;
        }

        public override void WriteJson(JsonWriter writer, object? value, JsonSerializer serializer)
        {
            var filter = (MexcSymbolFilter)value!;
            writer.WriteStartObject();

            writer.WritePropertyName("filterType");
            writer.WriteValue(JsonConvert.SerializeObject(filter.FilterType, new SymbolFilterTypeConverter(false)));

            switch (filter.FilterType)
            {
                case SymbolFilterType.LotSize:
                    var lotSizeFilter = (MexcSymbolLotSizeFilter)filter;
                    writer.WritePropertyName("maxQty");
                    writer.WriteValue(lotSizeFilter.MaxQuantity);
                    writer.WritePropertyName("minQty");
                    writer.WriteValue(lotSizeFilter.MinQuantity);
                    writer.WritePropertyName("stepSize");
                    writer.WriteValue(lotSizeFilter.StepSize);
                    break;
                case SymbolFilterType.MarketLotSize:
                    var marketLotSizeFilter = (MexcSymbolMarketLotSizeFilter)filter;
                    writer.WritePropertyName("maxQty");
                    writer.WriteValue(marketLotSizeFilter.MaxQuantity);
                    writer.WritePropertyName("minQty");
                    writer.WriteValue(marketLotSizeFilter.MinQuantity);
                    writer.WritePropertyName("stepSize");
                    writer.WriteValue(marketLotSizeFilter.StepSize);
                    break;
                case SymbolFilterType.MinNotional:
                    var minNotionalFilter = (MexcSymbolMinNotionalFilter)filter;
                    writer.WritePropertyName("minNotional");
                    writer.WriteValue(minNotionalFilter.MinNotional);
                    writer.WritePropertyName("applyToMarket");
                    writer.WriteValue(minNotionalFilter.ApplyToMarketOrders);
                    writer.WritePropertyName("avgPriceMins");
                    writer.WriteValue(minNotionalFilter.AveragePriceMinutes);
                    break;
                case SymbolFilterType.Price:
                    var priceFilter = (MexcSymbolPriceFilter)filter;
                    writer.WritePropertyName("maxPrice");
                    writer.WriteValue(priceFilter.MaxPrice);
                    writer.WritePropertyName("minPrice");
                    writer.WriteValue(priceFilter.MinPrice);
                    writer.WritePropertyName("tickSize");
                    writer.WriteValue(priceFilter.TickSize);
                    break;
                case SymbolFilterType.MaxNumberAlgorithmicOrders:
                    var algoFilter = (MexcSymbolMaxAlgorithmicOrdersFilter)filter;
                    writer.WritePropertyName("maxNumAlgoOrders");
                    writer.WriteValue(algoFilter.MaxNumberAlgorithmicOrders);
                    break;
                case SymbolFilterType.MaxPosition:
                    var maxPositionFilter = (MexcSymbolMaxPositionFilter)filter;
                    writer.WritePropertyName("maxPosition");
                    writer.WriteValue(maxPositionFilter.MaxPosition);
                    break;
                case SymbolFilterType.MaxNumberOrders:
                    var orderFilter = (MexcSymbolMaxOrdersFilter)filter;
                    writer.WritePropertyName("maxNumOrders");
                    writer.WriteValue(orderFilter.MaxNumberOrders);
                    break;
                case SymbolFilterType.IcebergParts:
                    var icebergPartsFilter = (MexcSymbolIcebergPartsFilter)filter;
                    writer.WritePropertyName("limit");
                    writer.WriteValue(icebergPartsFilter.Limit);
                    break;
                case SymbolFilterType.PricePercent:
                    var pricePercentFilter = (MexcSymbolPercentPriceFilter)filter;
                    writer.WritePropertyName("multiplierUp");
                    writer.WriteValue(pricePercentFilter.MultiplierUp);
                    writer.WritePropertyName("multiplierDown");
                    writer.WriteValue(pricePercentFilter.MultiplierDown);
                    writer.WritePropertyName("avgPriceMins");
                    writer.WriteValue(pricePercentFilter.AveragePriceMinutes);
                    break;
                case SymbolFilterType.TrailingDelta:
                    var TrailingDelta = (MexcSymbolTrailingDeltaFilter)filter;
                    writer.WritePropertyName("maxTrailingAboveDelta");
                    writer.WriteValue(TrailingDelta.MaxTrailingAboveDelta);
                    writer.WritePropertyName("maxTrailingBelowDelta");
                    writer.WriteValue(TrailingDelta.MaxTrailingBelowDelta);
                    writer.WritePropertyName("minTrailingAboveDelta");
                    writer.WriteValue(TrailingDelta.MinTrailingAboveDelta);
                    writer.WritePropertyName("minTrailingBelowDelta");
                    writer.WriteValue(TrailingDelta.MinTrailingBelowDelta);
                    break;
                default:
                    Trace.WriteLine($"{DateTime.Now:yyyy/MM/dd HH:mm:ss:fff} | Warning | Can't write symbol filter of type: " + filter.FilterType);
                    break;
            }

            writer.WriteEndObject();
        }
    }
}
