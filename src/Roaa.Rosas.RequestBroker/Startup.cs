using Microsoft.Extensions.DependencyInjection;

namespace Roaa.Rosas.RequestBroker
{
    public static class Startup
    {
        public static void AddRequestBroker(this IServiceCollection services)
        {
            services.AddTransient<IRequestBroker, HttpRequestBroker>();
        }
    }
}
