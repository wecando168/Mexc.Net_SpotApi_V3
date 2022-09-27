using Mexc.Net.Objects;
using Mexc.Net.UnitTests.TestImplementations;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading.Tasks;
using CryptoExchange.Net.Interfaces;
using Mexc.Net.Interfaces.Clients;

namespace Mexc.Net.UnitTests
{
    [TestFixture]
    public class MexcV3JsonTests
    {
        private MexcV3JsonToObjectComparer<IMexcV3RestClient> _comparer = new MexcV3JsonToObjectComparer<IMexcV3RestClient>((json) => MexcV3TestHelpers.CreateResponseClient(json, new MexcV3ClientOptions()
        {
            ApiCredentials = new CryptoExchange.Net.Authentication.ApiCredentials("123", "123"),
            SpotApiOptions = new MexcV3ApiClientOptions
            {
                RateLimiters = new List<IRateLimiter>(),
                AutoTimestamp = false,
            },
            FuturesApiOptions = new MexcV3ApiClientOptions
            {
                RateLimiters = new List<IRateLimiter>(),
                AutoTimestamp = false,
            }
        }));

        [Test]
        public async Task ValidateSpotMarketDataCalls()
        {
            await _comparer.ProcessSubject(
                "Spot/MarketData",
                c => c.SpotApi.MarketData,
                useNestedJsonPropertyForCompare: new Dictionary<string, string> {
                    { "MexcV3GetPingAsync", "data" },
                    { "MexcV3GetServerTimeAsync", "data" },
                    { "MexcV3GetServerTimeStampAsync", "data" },
                    { "MexcV3GetExchangeInfoAsync", "data" },
                    { "MexcV3GetOrderBookAsync", "data" },
                    { "MexcV3GetRecentTradesAsync", "data" },
                    { "MexcV3GetTradeHistoryAsync", "data" },
                    { "MexcV3GetAggregatedTradeHistoryAsync", "data" },
                    { "MexcV3GetKlinesAsync", "data" },
                    { "MexcV3GetCurrentAvgPriceAsync", "data" },
                    { "MexcV3GetTickerAsync", "data" },
                    { "MexcV3GetPriceAsync", "data" },
                    { "MexcV3GetBookPricesAsync", "data" }
                },
                parametersToSetNull: new[] { "limit" });
        }

        [Test]
        public async Task ValidateSubAccountCalls()
        {
            await _comparer.ProcessSubject(
                "Spot/SubAccount",
                c => c.SpotApi.SubAccount,
                useNestedJsonPropertyForCompare: new Dictionary<string, string> {
                    { "MexcV3CreateSubAccountAsync", "data" },
                    { "MexcV3QuerySubAccountListAsync", "data" },
                    { "MexcV3CreateAPIKeyAnync", "data" },
                    { "MexcV3QueryAPIKeyAsync", "data" },
                    { "MexcV3DeleteAPIKeyAsync", "data" },
                    { "MexcV3UniversalTransferAsync", "data" },
                    { "MexcV3QueryUniversalTransferAsync", "data" },
                    { "MexcV3EnableFuturesAsync", "data" },
                    { "MexcV3EnableMarginAsync", "data" }
                },
                parametersToSetNull: new[] { "limit" });
        }

        [Test]
        public async Task ValidateAccountTradeCalls()
        {
            await _comparer.ProcessSubject(
                "Spot/AccountTrade",
                c => c.SpotApi.SpotAccountTrade,
                useNestedJsonPropertyForCompare: new Dictionary<string, string> {
                    { "MexcV3PlaceTestOrderAsync", "data" },
                    { "MexcV3PlaceOrderAsync", "data" },
                    { "MexcV3PlaceBatchOrdersAsync", "data" },
                    { "MexcV3CancelOrderAsync", "data" },
                    { "MexcV3CancelOpenOrdersAsync", "data" },
                    { "MexcV3GetOrderAsync", "data" },
                    { "MexcV3GetOpenOrdersAsync", "data" },
                    { "MexcV3GetOrdersAsync", "data" },
                    { "MexcV3GetAccountInfoAsync", "data" },
                    { "MexcV3GetUserTradesAsync", "data" },
                    { "MexcV3GetCurrentAvgPriceAsync", "data" }
                },
                parametersToSetNull: new[] { "limit" });
        }

        [Test]
        public async Task ValidateWalletCalls()
        {
            await _comparer.ProcessSubject(
                "Spot/Wallet",
                c => c.SpotApi.Wallet,
                useNestedJsonPropertyForCompare: new Dictionary<string, string> {
                    { "MexcV3GetQueryCurrencyInfoAsync", "data" },
                    { "MexcV3WithdrawAsync", "data" },
                    { "MexcV3GetDepositHistoryAsync", "data" },
                    { "MexcV3GetWithdrawHistoryAsync", "data" },
                    { "MexcV3GetDepositAddressAsync", "data" },
                    { "MexcV3UserUniversalTransferAsync", "data" },
                    { "MexcV3GetQueryUserUniversalTransferHistoryAsync", "data" }
                },
                parametersToSetNull: new[] { "limit" });
        }

        [Test]
        public async Task ValidateETFCalls()
        {
            await _comparer.ProcessSubject(
                "Spot/ETF",
                c => c.SpotApi.ExchangeTradedFunds,
                useNestedJsonPropertyForCompare: new Dictionary<string, string> {
                    { "MexcV3GetETFInfoAsync", "data" }
                },
                parametersToSetNull: new[] { "limit" });
        }



        [Test]
        public async Task ValidateMarginAccountTradeCalls()
        {   
            await _comparer.ProcessSubject(
                "Spot/MarginAccountTrade",
                c => c.SpotApi.MarginAccountTrade,
                useNestedJsonPropertyForCompare: new Dictionary<string, string> {
                    { "MexcV3MarginTradeModeAsync", "data" },
                    { "MexcV3MarginPlaceOrderAsync", "data" },
                    { "MexcV3MarginLoanAsync", "data" },
                    { "MexcV3MarginGetRepayAsync", "data" },
                    { "MexcV3MarginCancelOpenOrdersAsync", "snapshotVos" },
                    { "MexcV3MarginCancelOpenOrderAsync", "data" },
                    { "MexcV3MarginGetQueryLoanListAsync", "data" },
                    { "MexcV3MarginGetAllOrderAsync", "data" },
                    { "MexcV3MarginGetAccountTradeListAsync", "snapshotVos" },
                    { "MexcV3MarginGetCurrentOpenOrdersAsync", "data" },
                    { "MexcV3MarginGetAccountMaxTransferableAsync", "snapshotVos" },
                    { "MexcV3MarginGetPriceIndexAsync", "data" },
                    { "MexcV3MarginQueryOrderAsync", "data" },
                    { "MexcV3MarginGetIsolatedAccountAsync", "data" },
                    { "MexcV3MarginGetMaxBorrowableAsync", "data" },
                    { "MexcV3MarginRepayAsync", "data" },
                    { "MexcV3MarginGetIsolatedSymbolAsync", "data" },
                    { "MexcV3MarginGetForceLiquidationRecAsync", "data" },
                    { "MexcV3MarginGetIsolatedMarginDataAsync", "data" },
                    { "MexcV3MarginGetIsolatedMarginTierAsync", "data" }
                },
                parametersToSetNull: new[] { "limit" });
        }
    }
}
