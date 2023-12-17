using Microsoft.Extensions.DependencyInjection;

namespace Roaa.Rosas.Application.Payment
{
    public static class PaymentServicesConfigurations
    {
        public static void AddPaymentServicesConfigurations(this IServiceCollection services)
        {
            services.AddScoped<IPaymentMethodFactory, PaymentMethodFactory>();
            services.AddScoped<IPaymentService, PaymentService>();
            services.AddScoped<IStripePaymentMethod, StripePaymentMethod>();
            services.AddScoped<StripePaymentMethod>();
        }
    }





}



