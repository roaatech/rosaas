using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Payment
{
    //public virtual async Task<decimal> CalculatePaymentAdditionalFeeAsync(IList<ShoppingCartItem> cart, decimal fee, bool usePercentage)
    //{
    //    if (!usePercentage || fee <= 0)
    //        return fee;

    //    var orderTotalWithoutPaymentFee = (await GetShoppingCartTotalAsync(cart, usePaymentMethodAdditionalFee: false)).shoppingCartTotal ?? 0;
    //    var result = (decimal)((float)orderTotalWithoutPaymentFee * (float)fee / 100f);

    //    return result;
    //}

    public interface IPaymentMethodFactory
    {
        IPaymentMethod GetPaymentMethod(PaymentMethodType type);
    }





}



