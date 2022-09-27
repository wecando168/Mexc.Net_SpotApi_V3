using System.Threading;
using System.Threading.Tasks;
using Mexc.Net.Enums;
using Mexc.Net.Interfaces.Clients.SpotApi;
using CryptoExchange.Net.Logging;
using CryptoExchange.Net.Objects;
using Mexc.Net.Objects.Models.Spot.Margin;
using CryptoExchange.Net.CommonObjects;

namespace Mexc.Net.Clients.SpotApi
{
    /// <inheritdoc />
    public class MexcV3ClientSpotApiMarginAccountTrade : IMexcV3ClientSpotApiMarginAccountTrade 
    {
        private const string marginTradeModeEndpoint = "margin/tradeMode";                          //切换杠杆账户模式
        private const string marginNewOrderEndpoint = "margin/order";                               //杠杆账户下单
        private const string marginLoanEndpoint = "margin/loan";                                    //借贷
        private const string marginRepayEndpoint = "margin/repay";                                  //归还借贷
        private const string margincCncelAllOpenOrderEndpoint = "margin/openOrders";                //撤销单一交易对的所有挂单
        private const string margincCncelOpenOrderEndpoint = "margin/order";                        //撤销订单
        private const string margincQueryLoanListEndpoint = "margin/loan";                          //查询借贷记录
        private const string margincAllOrderEndpoint = "margin/allOrders";                          //查询历史委托记录
        private const string margincAccountTradeListEndpoint = "margin/myTrades";                   //查询历史成交记录
        private const string margincCurrentOpenOrdersEndpoint = "margin/openOrders";                //查询当前挂单记录
        private const string margincAccountMaxTransferableEndpoint = "margin/maxTransferable";      //查询最大可转出额
        private const string margincMarginPriceIndexEndpoint = "margin/priceIndex";                 //查询杠杆价格指数
        private const string margincQueryOrderEndpoint = "margin/order";                            //查询杠杆账户订单详情
        private const string margincIsolatedAccountEndpoint = "margin/isolated/account";            //查询杠杆逐仓账户信息
        private const string margincMaxBorrowableEndpoint = "margin/maxBorrowable/";                //查询账户最大可借贷额度
        private const string margincRepayEndpoint = "margin/repay";                                 //查询还贷记录
        private const string margincSymbolEndpoint = "margin/isolated/pair";                        //查询逐仓杠杆交易对
        private const string margincForceLiquidationRecordEndpoint = "margin/forceLiquidationRec";  //获取账户强制平仓记录
        private const string margincIsolatedMarginDataEndpoint = "margin/isolatedMarginData";       //获取逐仓杠杆利率及限额
        private const string margincIsolatedMarginTierEndpoint = "margin/isolatedMarginTier";       //获取逐仓档位信息

        private const string api = "api";
        private const string publicVersion = "3";
        private const string signedVersion = "3";

        private readonly Log _log;

        private readonly MexcV3ClientSpotApi _baseClient;

        internal MexcV3ClientSpotApiMarginAccountTrade(Log log, MexcV3ClientSpotApi baseClient)
        {
            _log = log;
            _baseClient = baseClient;
        }

        #region 1.switch trademode of margin account

        /// <inheritdoc />
        public async Task<WebCallResult<MexcV3MarginTradeModeResponse>> MarginTradeModeAsync(
            string symbol,
            int tradeMode,
            int? receiveWindow = null,
            CancellationToken ct = default)
        {
            WebCallResult<MexcV3MarginTradeModeResponse>? respance = await _baseClient.MarginTradeModeInternal(
                uri: _baseClient.GetUrl(marginTradeModeEndpoint, api, signedVersion),
                symbol: symbol,
                tradeMode: 0,
                receiveWindow: receiveWindow,
                weight: 1,
                ct: ct).ConfigureAwait(false);
            return respance;
        }

        #endregion

        #region 2.New Margin Order 

        /// <inheritdoc />
        public async Task<WebCallResult<MexcV3MarginPlacedOrderResponse>> MarginPlaceOrderAsync(
            string symbol,
            OrderSide side,
            SpotOrderType type,
            decimal? quantity = null,
            decimal? quoteQuantity = null,
            decimal? price = null,
            string? newClientOrderId = null,
            int? receiveWindow = null,
            CancellationToken ct = default)
        {
            WebCallResult<MexcV3MarginPlacedOrderResponse>? result = await _baseClient.MarginPlacedOrderInternal(
                uri: _baseClient.GetUrl(marginNewOrderEndpoint, api, signedVersion),
                symbol: symbol,
                isIsolated: true,
                side: side,
                spotOrderType: type,
                quantity: quantity,
                quoteOrderQty: quoteQuantity,
                price: price,
                newClientOrderId: newClientOrderId,
                receiveWindow: receiveWindow,
                weight: 1,
                ct: ct).ConfigureAwait(false);
            if (result)
                _baseClient.InvokeOrderPlaced(new OrderId() { SourceObject = result.Data });
            return result;
        }
        #endregion
    }
}
