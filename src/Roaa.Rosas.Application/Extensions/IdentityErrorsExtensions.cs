using Roaa.Rosas.Application.SystemMessages;

namespace Roaa.Rosas.Application.Extensions
{
    public static class IdentityErrorsExtensions
    {
        public static IdentityError ToIdentityError(this string errorCode)
        {
            string[] errors = Enum.GetNames(typeof(IdentityError));

            if (!string.IsNullOrEmpty(errorCode) && errors.Any(e => e == errorCode))
            {
                return (IdentityError)Enum.Parse(typeof(IdentityError), errorCode);
            }
            else
                return IdentityError.DefaultError;
        }

        public static IdentityError ToIdentityError(this IdentityError errorCode, IdentityError inCase, IdentityError output)
        {
            if (errorCode == inCase) return output;
            return errorCode;

        }
    }
}
