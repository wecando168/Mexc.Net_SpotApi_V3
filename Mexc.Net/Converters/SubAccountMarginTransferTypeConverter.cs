using System.Collections.Generic;
using Mexc.Net.Enums;
using CryptoExchange.Net.Converters;

namespace Mexc.Net.Converters
{
    internal class SubAccountMarginTransferTypeConverter : BaseConverter<SubAccountMarginTransferType>
    {
        public SubAccountMarginTransferTypeConverter() : this(true)
        {
        }

        public SubAccountMarginTransferTypeConverter(bool quotes) : base(quotes)
        {
        }

        protected override List<KeyValuePair<SubAccountMarginTransferType, string>> Mapping =>
            new List<KeyValuePair<SubAccountMarginTransferType, string>>
            {
                new KeyValuePair<SubAccountMarginTransferType, string>(
                    SubAccountMarginTransferType.FromSubAccountSpotToSubAccountMargin, "1"),
                new KeyValuePair<SubAccountMarginTransferType, string>(
                    SubAccountMarginTransferType.FromSubAccountMarginToSubAccountSpot, "2"),
            };
    }
}