using Mexc.Net.Enums;
using CryptoExchange.Net.Converters;
using System.Collections.Generic;

namespace Mexc.Net.Converters
{
    internal class BSwapOperationConverter : BaseConverter<BSwapOperation>
    {
        public BSwapOperationConverter() : this(true) { }
        public BSwapOperationConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<BSwapOperation, string>> Mapping => new List<KeyValuePair<BSwapOperation, string>>
        {
            new KeyValuePair<BSwapOperation, string>(BSwapOperation.Add, "ADD"),
            new KeyValuePair<BSwapOperation, string>(BSwapOperation.Remove, "REMOVE")
        };
    }
}
