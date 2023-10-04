using Roaa.Rosas.Common.Localization;

namespace Roaa.Rosas.Application.Services.Management.Specifications.Validators
{
    public class ValidationHelper
    {
        public static bool ValidateLocalizedName(LocalizedString localizedName)
        {
            if (localizedName is null)
            {
                return false;
            }
            if (string.IsNullOrWhiteSpace(localizedName.Ar) && string.IsNullOrWhiteSpace(localizedName.En))
            {
                return false;
            }
            return true;
        }
    }
}

