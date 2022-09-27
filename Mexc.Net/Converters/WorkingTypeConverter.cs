﻿using Mexc.Net.Enums;
using CryptoExchange.Net.Converters;
using System.Collections.Generic;

namespace Mexc.Net.Converters
{
    internal class WorkingTypeConverter : BaseConverter<WorkingType>
    {
        public WorkingTypeConverter() : this(true) { }
        public WorkingTypeConverter(bool quotes) : base(quotes) { }

        protected override List<KeyValuePair<WorkingType, string>> Mapping => new List<KeyValuePair<WorkingType, string>>
        {
            new KeyValuePair<WorkingType, string>(WorkingType.Mark, "MARK_PRICE"),
            new KeyValuePair<WorkingType, string>(WorkingType.Contract, "CONTRACT_PRICE"),
        };
    }
}
