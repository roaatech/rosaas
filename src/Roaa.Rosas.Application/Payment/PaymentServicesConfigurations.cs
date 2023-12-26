using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Application.Payment.Factories;
using Roaa.Rosas.Application.Payment.Methods.ManwalService;
using Roaa.Rosas.Application.Payment.Methods.StripeService;
using Roaa.Rosas.Application.Payment.Services;
using Roaa.Rosas.Domain.Models.Options;

namespace Roaa.Rosas.Application.Payment
{
    public static class PaymentServicesConfigurations
    {
        public static void AddPaymentServicesConfigurations(this IServiceCollection services, RootOptions rootOptions)
        {
            Stripe.StripeConfiguration.ApiKey = rootOptions.Payment.Stripe.ApiKey;
            services.AddScoped<IPaymentMethodFactory, PaymentMethodFactory>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IPaymentProcessingService, PaymentProcessingService>();
            services.AddScoped<IStripePaymentMethodService, StripePaymentMethodService>();
            services.AddScoped<StripePaymentMethodService>();
            services.AddScoped<ManwalPaymentMethodService>();
        }
    }





}



