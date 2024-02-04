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

        public async Task<Result<Order>> CompletePaymentProcessAsync(Guid orderId, CancellationToken cancellationToken = default)
        {
            var order = await _dbContext.Orders
                                        .Where(x => x.Id == orderId)
                                        .SingleOrDefaultAsync(cancellationToken);

            await _paymentProcessingService.MarkOrderAsPaidAsync(order);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return Result<Order>.Successful(order);
        }


        public async Task<Result<CheckoutResultModel>> HandelPaymentProcessAsync(Order order, CancellationToken cancellationToken = default)
        {
            var resultModel = new CheckoutResultModel();

            // Price Is Free
            if (order.OrderTotal == 0)
            {
                var result = await CompletePaymentProcessAsync(order.Id, cancellationToken);

                resultModel.TenantId = result.Data.TenantId;
            }

            return Result<CheckoutResultModel>.Successful(resultModel);
        }


        public async Task<DateTime> GetPaymentProcessingExpirationDate(CancellationToken cancellationToken = default)
        {
            return DateTime.UtcNow.AddMinutes(5);
        }


        public PaymentMethodType PaymentMethodType
        {
            get
            {
                return PaymentMethodType.Manwal;
            }
        }

    }
}