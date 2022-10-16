using System.Collections.Generic;
using Mexc.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Mexc.Net.Converters
{
    internal class OrderStatusConverter : BaseConverter<OrderStatus>
    {
        public OrderStatusConverter(): this(true) { }
        public OrderStatusConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<OrderStatus, string>> Mapping => new List<KeyValuePair<OrderStatus, string>>
        {
            new KeyValuePair<OrderStatus, string>(OrderStatus.New, "NEW"),
            new KeyValuePair<OrderStatus, string>(OrderStatus.PartiallyFilled, "PARTIALLY_FILLED"),
            new KeyValuePair<OrderStatus, string>(OrderStatus.Filled, "FILLED" ),
            new KeyValuePair<OrderStatus, string>(OrderStatus.Canceled, "CANCELED"),
            new KeyValuePair<OrderStatus, string>(OrderStatus.PartiallyCanceled, "PARTIALLY_CANCELED"),
            new KeyValuePair<OrderStatus, string>(OrderStatus.Eexcuted, "EXECUTED"),
        };
    }
}
