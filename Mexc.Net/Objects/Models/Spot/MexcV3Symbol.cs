using System;
using System.Collections.Generic;
using Mexc.Net.Converters;
using Mexc.Net.Enums;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models.Spot
{
    /// <summary>
    /// Symbol info
    /// </summary>
    public class MexcV3Symbol
    {
        /// <summary>
        /// The symbol
        /// 交易对
        /// </summary>
        [JsonProperty("symbol")]
        public string SymbolName { get; set; } = string.Empty;

        /// <summary>
        /// The status of the symbol
        /// 状态
        /// </summary>
        [JsonProperty("status"), JsonConverter(typeof(SymbolStatusConverter))]
        public SymbolStatus Status { get; set; }

        /// <summary>
        /// The base asset
        /// 交易币
        /// </summary>
        [JsonProperty("baseAsset")]
        public string BaseAsset { get; set; } = string.Empty;

        /// <summary>
        /// The precision of the base asset
        /// 交易币精度
        /// </summary>
        [JsonProperty("baseAssetPrecision")]
        public int BaseAssetPrecision { get; set; }

        /// <summary>
        /// The quote asset
        /// 计价币
        /// </summary>
        [JsonProperty("quoteAsset")]
        public string QuoteAsset { get; set; } = string.Empty;

        /// <summary>
        /// The precision of the quote price
        /// 计价币价格精度
        /// </summary>
        [JsonProperty("quotePrecision")]
        public int QuotePrecision { get; set; }

        /// <summary>
        /// The precision of the quote asset
        /// 计价币资产精度
        /// </summary>
        [JsonProperty("quoteAssetPrecision")]
        public int QuoteAssetPrecision { get; set; }

        /// <summary>
        /// 交易币手续费精度
        /// </summary>
        [JsonProperty("baseCommissionPrecision")]
        public int BaseCommissionPrecision { get; set; }

        /// <summary>
        /// 计价币手续费精度
        /// </summary>
        [JsonProperty("quoteCommissionPrecision")]
        public int QuoteCommissionPrecision { get; set; }

        /// <summary>
        /// Allowed order types
        /// 许可的交易订单类型
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(SpotOrderTypeConverter))]
        public IEnumerable<SpotOrderType> OrderTypes { get; set; } = Array.Empty<SpotOrderType>();

        /// <summary>
        /// 是否允许市价委托
        /// </summary>
        [JsonProperty("quoteOrderQtyMarketAllowed")]
        public bool QuoteOrderQtyMarketAllowed { get; set; }

        /// <summary>
        /// Spot trading orders allowed
        /// 是否允许api现货交易
        /// </summary>
        [JsonProperty("isSpotTradingAllowed")]
        public bool IsSpotTradingAllowed { get; set; }

        /// <summary>
        /// Margin trading orders allowed
        /// 是否允许api杠杆交易
        /// </summary>
        [JsonProperty("isMarginTradingAllowed")]
        public bool IsMarginTradingAllowed { get; set; }

        /// <summary>
        /// 最小下单金额（报价金额精度）
        /// </summary>
        [JsonProperty("quoteAmountPrecision")]
        public string QuoteAmountPrecision { get; set; }

        /// <summary>
        /// 最小委托数量（基本尺寸精度）
        /// </summary>
        [JsonProperty("baseSizePrecision")]
        public string BaseSizePrecision { get; set; }

        /// <summary>
        /// Permissions types
        /// 权限(permissions)
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(EnumConverter))]
        public IEnumerable<AccountType> Permissions { get; set; } = Array.Empty<AccountType>();

        /// <summary>
        /// Filters for order on this symbol
        /// 过滤此交易品种的订单(filters)
        /// </summary>
        public IEnumerable<MexcSymbolFilter> Filters { get; set; } = Array.Empty<MexcSymbolFilter>();

        /// <summary>
        /// 最大委托数量
        /// </summary>
        [JsonProperty("maxQuoteAmount")]
        public string MaxQuoteAmount { get; set; }

        /// <summary>
        /// marker手续费
        /// </summary>
        [JsonProperty("makerCommission")]
        public string MakerCommission { get; set; }

        /// <summary>
        /// taker手续费
        /// </summary>
        [JsonProperty("takerCommission")]
        public string TakerCommission { get; set; }
    }
}
