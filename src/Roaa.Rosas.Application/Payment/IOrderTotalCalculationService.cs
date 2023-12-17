using Stripe;

namespace Roaa.Rosas.Application.Payment
{
    public partial interface IOrderTotalCalculationService
    {

        Task<(decimal discountAmount, List<Discount> appliedDiscounts, decimal subTotalWithoutDiscount, decimal subTotalWithDiscount,
            SortedDictionary<decimal, decimal> taxRates)> GetShoppingCartSubTotalAsync(IList<object> cart, bool includingTax);

        Task<(decimal adjustedShippingRate, List<Discount> appliedDiscounts)> AdjustShippingRateAsync(decimal shippingRate);

        Task<(decimal taxTotal, SortedDictionary<decimal, decimal> taxRates)> GetTaxTotalAsync(IList<object> cart, bool usePaymentMethodAdditionalFee = true);


        Task<(decimal? shoppingCartTotal, decimal redeemedRewardPointsAmount)> GetShoppingCartTotalAsync(IList<object> cart,
            bool? useRewardPoints = null, bool usePaymentMethodAdditionalFee = true);

        Task<decimal> CalculatePaymentAdditionalFeeAsync(IList<object> cart, decimal fee, bool usePercentage);


        Task UpdateOrderTotalsAsync(object updateOrderParameters, IList<object> restoredCart);


    }





}



