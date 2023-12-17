using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Payment
{
    public interface IPaymentService
    {
        Task<Result<CheckoutResultModel>> ProcessPaymentAsync(CheckoutModel model, CancellationToken cancellationToken = default);
    }





}



