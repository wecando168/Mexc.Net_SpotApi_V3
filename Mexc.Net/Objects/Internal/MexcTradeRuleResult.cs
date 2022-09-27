namespace Mexc.Net.Objects.Internal
{
    internal class MexcTradeRuleResult
    {
        public bool Passed { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? QuoteQuantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? StopPrice { get; set; }
        public string? ErrorMessage { get; set; }
        public static MexcTradeRuleResult CreateTestPassed(decimal? quantity, decimal? quoteQuantity, decimal? price)
        {
            return new MexcTradeRuleResult
            {
                Passed = true,
                Quantity = quantity,
                Price = price,
                QuoteQuantity = quoteQuantity
            };
        }

        public static MexcTradeRuleResult CreatePassed(decimal? quantity, decimal? quoteQuantity, decimal? price)
        {
            return new MexcTradeRuleResult
            {
                Passed = true,
                Quantity = quantity,
                Price = price,
                QuoteQuantity = quoteQuantity
            };
        }

        public static MexcTradeRuleResult CreateFailed(string message)
        {
            return new MexcTradeRuleResult
            {
                Passed = false,
                ErrorMessage = message
            };
        }
    }
}
