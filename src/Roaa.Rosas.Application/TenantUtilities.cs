using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Application
{
    internal class TenantUtilities
    {
        public static CurrencyCode GetDefaultCurrencyType()
        {
            return CurrencyCode.USD;
        }

        public static string GetDefaultCurrencyCode()
        {
            return GetDefaultCurrencyType().ToString();
        }
    }
}
