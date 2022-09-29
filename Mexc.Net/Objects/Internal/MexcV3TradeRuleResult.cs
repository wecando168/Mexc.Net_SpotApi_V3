namespace Mexc.Net.Objects.Internal
{
    internal class MexcV3TradeRuleResult
    {
        public bool Passed { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? QuoteQuantity { get; set; }
        public decimal? Price { get; set; }
        public decimal? StopPrice { get; set; }
        public string? ErrorMessage { get; set; }
        public static MexcV3TradeRuleResult CreateTestPassed(decimal? quantity, decimal? quoteQuantity, decimal? price)
        {
            return new MexcV3TradeRuleResult
            {
                Passed = true,
                Quantity = quantity,
                Price = price,
                QuoteQuantity = quoteQuantity
            };
        }

        public static MexcV3TradeRuleResult CreatePassed(decimal? quantity, decimal? quoteQuantity, decimal? price)
        {
            return new MexcV3TradeRuleResult
            {
                Passed = true,
                Quantity = quantity,
                Price = price,
                QuoteQuantity = quoteQuantity
            };
        }

        public static MexcV3TradeRuleResult CreateFailed(string message)
        {
            return new MexcV3TradeRuleResult
            {
                Passed = false,
                ErrorMessage = message
            };
        }
    }
}
