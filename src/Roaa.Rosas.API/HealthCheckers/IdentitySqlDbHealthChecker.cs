using Roaa.Rosas.Common.ApiConfiguration;
using Roaa.Rosas.Common.HealthChecks.Checkers;
using Roaa.Rosas.Domain.Models.Options;

namespace Roaa.Rosas.Framework.HealthCheckers
{
    public class IdentitySqlDbHealthChecker : SqlDbHealthChecker
    {
        public IdentitySqlDbHealthChecker(IApiConfigurationService<ConnectionStringsOptions> dbSettings) :
            base(dbSettings.Options.IdentityDb)
        {
        }
    }
}
