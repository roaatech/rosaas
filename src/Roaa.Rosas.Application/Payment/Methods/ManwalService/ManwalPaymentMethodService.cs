using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Roaa.Rosas.Application.Interfaces.DbContexts;
using Roaa.Rosas.Application.Payment.Models;
using Roaa.Rosas.Application.Payment.Services;
using Roaa.Rosas.Application.Services.Management.Settings;
using Roaa.Rosas.Authorization.Utilities;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application.Payment.Methods.ManwalService
{
    public class ManwalPaymentMethodService : IPaymentMethodService
    {
        #region Props 
        private readonly ILogger<ManwalPaymentMethodService> _logger;
        private readonly IRosasDbContext _dbContext;
        private readonly IIdentityContextService _identityContextService;
        private readonly IPaymentProcessingService _paymentProcessingService;
        private readonly ISettingService _settingService;
        private readonly ISender _mediator;
        #endregion


        #region Corts
        public ManwalPaymentMethodService(ILogger<ManwalPaymentMethodService> logger,
                                   IRosasDbContext dbContext,
                                   IIdentityContextService identityContextService,
                                   IPaymentProcessingService paymentProcessingService,
                                   ISender mediator,
                                   ISettingService settingService)
        {
            _logger = logger;
            _dbContext = dbContext;
            _identityContextService = identityContextService;
            _paymentProcessingService = paymentProcessingService;
            _settingService = settingService;
            _mediator = mediator;
        }
        #endregion

        public async Task<Result<Order>> CompleteSuccessfulPaymentProcessAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var order = await _dbContext.Orders
                                        .Where(x => x.Id == orderId)
                                        .SingleOrDefaultAsync(cancellationToken);

            await _paymentProcessingService.MarkOrderAsPaidAsync(order, null, PaymentPlatform, cancellationToken);

            return Result<Order>.Successful(order);
        }


        public async Task<Result<PaymentMethodCheckoutResultModel>> CreatePaymentAsync(Order order, bool setAuthorizedPayment, bool storeCardInfo, PaymentMethodType paymentMethodType, CancellationToken cancellationToken = default)
        {
            var resultModel = new PaymentMethodCheckoutResultModel();

            await _paymentProcessingService.MarkOrderAsProcessingAsync(order, PaymentPlatform, paymentMethodType, cancellationToken);

            // Price Is Free
            if (order.OrderTotal == 0)
            {
                var result = await CompleteSuccessfulPaymentProcessAsync(order.Id, cancellationToken);
            }

            return Result<PaymentMethodCheckoutResultModel>.Successful(resultModel);
        }

        public Task<Result> CapturePaymentAsync(Order order, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public PaymentPlatform PaymentPlatform
        {
            get
            {
                return PaymentPlatform.Manwal;
            }
        }

    }
}