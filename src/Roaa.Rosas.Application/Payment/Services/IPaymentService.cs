using Roaa.Rosas.Application.Payment.Models;
using Roaa.Rosas.Common.Models.Results;

namespace Roaa.Rosas.Application.Payment.Services
{
    public interface IPaymentService
    {
        Task<Result<CheckoutResultModel>> HandelPaymentProcessAsyncAsync(CheckoutModel model, CancellationToken cancellationToken = default);
    }





}



