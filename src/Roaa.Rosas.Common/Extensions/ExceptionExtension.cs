namespace Roaa.Rosas.Common.Extensions
{
    public static class ExceptionExtension
    {
        public static string GetErrorMessage(this Exception exception)
        {
            var errors = new List<string>();
            var innerException = exception;
            while (innerException != null)
            {
                errors.Add(innerException.Message);
                innerException = innerException.InnerException;
            }
            errors.Add(exception.Source);
            errors.Add(exception.StackTrace);
            return string.Join(" || ", errors);
        }

    }
}
