using System;
using System.Collections.Generic;
using Mexc.Net.Enums;
using Mexc.Net.Interfaces;
using CryptoExchange.Net.Converters;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models.Spot
{
    /// <summary>
    /// Information about an account
    /// </summary>
    public class MexcV3AccountInfo
    {
        /// <summary>
        /// 错误提示消息
        /// </summary>
        [JsonProperty("msg")]
        public string? ErrorMessage { get; set; } = string.Empty;

        /// <summary>
        /// 错误编码
        /// </summary>
        [JsonProperty("code")]
        public long? ErrorCode { get; set; }

        /// <summary>
        /// 错误扩展信息
        /// </summary>
        [JsonProperty("_extend")]
        public string? Extend { get; set; }

        /// <summary>
        /// Fee percentage to pay when making trades
        /// maker 费率
        /// </summary>
        [JsonProperty("makerCommission")]
        public decimal MakerFee { get; set; }

        /// <summary>
        /// Fee percentage to pay when taking trades
        /// taker 费率
        /// </summary>
        [JsonProperty("takerCommission")]
        public decimal TakerFee { get; set; }

        /// <summary>
        /// Fee percentage to pay when buying
        /// 买入费率
        /// </summary>
        [JsonProperty("buyerCommission")]
        public decimal BuyerFee { get; set; }

        /// <summary>
        /// Fee percentage to pay when selling
        /// 卖出费率
        /// </summary>
        [JsonProperty("sellerCommission")]
        public decimal SellerFee { get; set; }

        /// <summary>
        /// Boolean indicating if this account can trade
        /// 是否可交易
        /// </summary>
        [JsonProperty("canTrade")]
        public bool CanTrade { get; set; }

        /// <summary>
        /// Boolean indicating if this account can withdraw
        /// 是否可提现
        /// </summary>
        [JsonProperty("canWithdraw")]
        public bool CanWithdraw { get; set; }

        /// <summary>
        /// Boolean indicating if this account can deposit
        /// 是否可充值
        /// </summary>
        [JsonProperty("canDeposit")]
        public bool CanDeposit { get; set; }

        /// <summary>
        /// The time of the update
        /// 更新时间
        /// </summary>
        [JsonProperty("updateTime"), JsonConverter(typeof(DateTimeConverter))]
        public DateTime UpdateTime { get; set; }

        /// <summary>
        /// The type of account
        /// 	账户类型
        /// </summary>
        [JsonProperty("accountType"), JsonConverter(typeof(EnumConverter))]
        public AccountType AccountType { get; set; }

        /// <summary>
        /// Permissions types
        /// 权限
        /// </summary>
        [JsonProperty(ItemConverterType = typeof(EnumConverter))]
        public IEnumerable<AccountType> Permissions { get; set; } = Array.Empty<AccountType>();

        /// <summary>
        /// List of assets with their current balances
        /// 余额
        /// </summary>
        [JsonProperty("balances")]
        public IEnumerable<MexcV3Balance> Balances { get; set; } = Array.Empty<MexcV3Balance>();
    }

    /// <summary>
    /// Information about an asset balance
    /// </summary>
    public class MexcV3Balance : IMexcV3Balance
    {
        /// <summary>
        /// The asset this balance is for
        /// 资产币种
        /// </summary>
        [JsonProperty("asset")]
        public string Asset { get; set; } = string.Empty;

        /// <summary>
        /// The quantity that isn't locked in a trade
        /// 可用数量
        /// </summary>
        [JsonProperty("free")]
        public decimal Available { get; set; }

        /// <summary>
        /// The quantity that is currently locked in a trade
        /// 冻结数量
        /// </summary>
        public decimal Locked { get; set; }

        /// <summary>
        /// The total balance of this asset (Free + Locked)
        /// 总数量（可用数量+冻结数量）
        /// </summary>
        public decimal Total => Available + Locked;
    }
}
