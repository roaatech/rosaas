using Microsoft.Extensions.Hosting;

namespace Roaa.Rosas.Common.Extensions
{
    public static class EnvironmentExtension
    {
        public const string Dev = "Dev";
        public const string Stage = "Stage";
        public const string Prod = "Prod";



        public static bool IsProductionEnvironment(this IHostEnvironment hostEnvironment)
        {
            if (hostEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostEnvironment));
            }

            return hostEnvironment.IsEnvironment(Environments.Production) ||
                   hostEnvironment.IsEnvironment(Prod);
        }

        public static bool IsStageEnvironment(this IHostEnvironment hostEnvironment)
        {
            if (hostEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostEnvironment));
            }

            return hostEnvironment.IsEnvironment(Environments.Staging) ||
                   hostEnvironment.IsEnvironment(Stage);
        }

        public static bool IsDevEnvironment(this IHostEnvironment hostEnvironment)
        {
            if (hostEnvironment == null)
            {
                throw new ArgumentNullException(nameof(hostEnvironment));
            }

            return hostEnvironment.IsEnvironment(Dev);
        }

    }
}
