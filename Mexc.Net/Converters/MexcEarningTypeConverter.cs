using Mexc.Net.Enums;
using CryptoExchange.Net.Converters;
using System.Collections.Generic;

namespace Mexc.Net.Converters
{
    internal class MexcEarningTypeConverter : BaseConverter<MexcEarningType>
    {
        public MexcEarningTypeConverter() : this(true) { }
        public MexcEarningTypeConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<MexcEarningType, string>> Mapping => new List<KeyValuePair<MexcEarningType, string>>
        {
            new KeyValuePair<MexcEarningType, string>(MexcEarningType.MiningWallet, "0"),
            new KeyValuePair<MexcEarningType, string>(MexcEarningType.MergedMining, "1"),
            new KeyValuePair<MexcEarningType, string>(MexcEarningType.ActivityBonus, "2"),
            new KeyValuePair<MexcEarningType, string>(MexcEarningType.Rebate, "3"),
            new KeyValuePair<MexcEarningType, string>(MexcEarningType.SmartPool, "4"),
            new KeyValuePair<MexcEarningType, string>(MexcEarningType.MiningAddress, "5"),
            new KeyValuePair<MexcEarningType, string>(MexcEarningType.IncomeTransfer, "6"),
            new KeyValuePair<MexcEarningType, string>(MexcEarningType.PoolSavings, "7"),
            new KeyValuePair<MexcEarningType, string>(MexcEarningType.Transfered, "8"),
            new KeyValuePair<MexcEarningType, string>(MexcEarningType.IncomeTransfer, "31"),
            new KeyValuePair<MexcEarningType, string>(MexcEarningType.HashrateResaleMiningWallet, "32"),
            new KeyValuePair<MexcEarningType, string>(MexcEarningType.HashrateResalePoolSavings, "33")
        };
    }
}
