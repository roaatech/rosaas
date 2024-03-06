using Microsoft.Extensions.DependencyInjection;
using Roaa.Rosas.Application.Payment.Factories;
using Roaa.Rosas.Application.Payment.Platforms.ManwalService;
using Roaa.Rosas.Application.Payment.Platforms.StripeService;
using Roaa.Rosas.Application.Payment.Services;
using Roaa.Rosas.Domain.Models.Options;

namespace Roaa.Rosas.Application.Payment
{
    public static class PaymentServicesConfigurations
    {
        public static void AddPaymentServicesConfigurations(this IServiceCollection services, RootOptions rootOptions)
        {
            Stripe.StripeConfiguration.ApiKey = rootOptions.Payment.Stripe.ApiKey;
            services.AddScoped<IPaymentPlatformFactory, PaymentPlatformFactory>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IPaymentProcessingService, PaymentProcessingService>();
            services.AddScoped<IStripePaymentPlatformService, StripePaymentPlatformService>();
            services.AddScoped<StripePaymentPlatformService>();
            services.AddScoped<ManwalPaymentPlatformService>();
        }
    }





}



