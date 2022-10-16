using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Mexc.Net.Objects.Models.Spot
{
    /// <summary>
    /// Mexc Currency Info
    /// 现货交易币种信息
    /// </summary>
    public class MexcV3UserAsset
    {
        /// <summary>
        /// Currency Name
        /// 币种名称
        /// </summary>
        [JsonProperty("coin")]
        public string Asset { get; set; } = string.Empty;

        /// <summary>
        /// Currency Full Name
        /// 币种全名
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// Network List
        /// </summary>
        public IEnumerable<MexcV3Network> NetworkList { get; set; } = Array.Empty<MexcV3Network>();
    }

    /// <summary>
    /// Chain network info
    /// </summary>
    public class MexcV3Network
    {
        /// <summary>
        /// Currency Name
        /// </summary>
        [JsonProperty("coin")]
        public string CurrencyName { get; set; } = string.Empty;

        /// <summary>
        /// 充值说明
        /// </summary>
        [JsonProperty("depositDesc")]
        public string? DepositDesc { get; set; } = string.Empty;

        /// <summary>
        /// 是否可充值
        /// </summary>
        [JsonProperty("depositEnable")]
        public bool DepositEnable { get; set; }

        /// <summary>
        /// Min Confirm
        /// 最小确认次数
        /// </summary>
        [JsonProperty("minConfirm")]
        public int MinConfirm { get; set; }

        /// <summary>
        /// Base Chain
        /// 底层链名称
        /// </summary>
        [JsonProperty("name")]
        public string BaseChain { get; set; } = string.Empty;

        /// <summary>
        /// Chain
        /// 币种所支持的网络
        /// </summary>
        [JsonProperty("network")]
        public string Chain { get; set; } = string.Empty;

        /// <summary>
        /// Withdraw Enable
        /// 是否可提币
        /// </summary>
        [JsonProperty("withdrawEnable")]
        public bool WithdrawEnable { get; set; }

        /// <summary>
        /// Withdraw Fee
        /// 提币手续费
        /// </summary>
        [JsonProperty("withdrawFee")]
        public string WithdrawFee { get; set; } = string.Empty;

        /// <summary>
        /// Withdraw Integer Multiple
        /// 提币整数倍数
        /// </summary>
        [JsonProperty("withdrawIntegerMultiple")]
        public string? WithdrawIntegerMultiple { get; set; } = string.Empty;

        /// <summary>
        /// Withdraw Max
        /// 最大提币限额
        /// </summary>
        [JsonProperty("withdrawMax")]
        public string WithdrawMax { get; set; } = string.Empty;

        /// <summary>
        /// Withdraw Min
        /// 最小提币限额
        /// </summary>
        [JsonProperty("withdrawMin")]
        public string WithdrawMin { get; set; } = string.Empty;

        /// <summary>
        /// Same Address
        /// </summary>
        [JsonProperty("sameAddress")]
        public bool SameAddress { get; set; }

        /// <summary>
        /// Contract
        /// 币种智能合约地址
        /// </summary>
        [JsonProperty("contract")]
        public string Contract { get; set; } = string.Empty;
    }
}