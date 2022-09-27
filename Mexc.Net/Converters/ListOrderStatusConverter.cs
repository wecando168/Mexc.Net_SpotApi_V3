using System.Collections.Generic;
using CryptoExchange.Net.Converters;
using Mexc.Net.Enums;

namespace Mexc.Net.Converters
{
    internal class ListOrderStatusConverter : BaseConverter<ListOrderStatus>
    {
        public ListOrderStatusConverter(): this(true) { }
        public ListOrderStatusConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<ListOrderStatus, string>> Mapping => new List<KeyValuePair<ListOrderStatus, string>>
        {
            new KeyValuePair<ListOrderStatus, string>(ListOrderStatus.Executing, "EXECUTING"),
            new KeyValuePair<ListOrderStatus, string>(ListOrderStatus.Rejected, "REJECT"),
            new KeyValuePair<ListOrderStatus, string>(ListOrderStatus.Done, "ALL_DONE"),
        };
    }
}
