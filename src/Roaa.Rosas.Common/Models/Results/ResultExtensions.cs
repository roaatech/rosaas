using FluentValidation.Results;
using Roaa.Rosas.Common.Models.ResponseMessages;

namespace Roaa.Rosas.Common.Models.Results
{
    public static class ResultExtensions
    {
        public static void WithError(this Result result, string errorMessage)
        {
            result.Messages.Add(MessageDetail.Error(errorMessage));
        }

        public static void WithError(this Result result, string errorMessage, Enum sysCode)
        {
            result.Messages.Add(MessageDetail.Error(errorMessage, sysCode));
        }

        public static void WithErrors(this Result result, List<string> errors)
        {
            result.Messages.AddRange(errors.Select(x => MessageDetail.Error(x)).ToList());
        }
        public static void WithMessage(this Result result, List<MessageDetail> messagesDetails)
        {
            result.Messages.AddRange(messagesDetails.Select(x => x));
        }

        public static void WithError<T>(this Result<T> result, string errorMessage)
        {
            result.Messages.Add(MessageDetail.Error(errorMessage));
        }

        public static void WithError<T>(this Result<T> result, string errorMessage, Enum sysCode)
        {
            result.Messages.Add(MessageDetail.Error(errorMessage, sysCode));
        }

        public static void WithErrors<T>(this Result<T> result, List<string> errors)
        {
            result.Messages.AddRange(errors.Select(x => MessageDetail.Error(x)).ToList());
        }

        public static void WithData<T>(this Result<T> result, T data)
        {
            result.Data = data;
        }

        public static void WithMessage<T>(this Result<T> result, List<MessageDetail> messagesDetails)
        {
            result.Messages.AddRange(messagesDetails.Select(x => x));
        }

        public static Result<T> WithErrors<T>(this Result<T> result, List<ValidationFailure> validationFailures)
        {
            return Result<T>.Fail(validationFailures.Select(x => MessageDetail.Error(x.ErrorMessage, x.ErrorCode, x.PropertyName)).ToList());
        }

        public static Result WithErrors(this Result result, List<ValidationFailure> validationFailures)
        {
            return Result.Fail(validationFailures.Select(x => MessageDetail.Error(x.ErrorMessage, x.ErrorCode, x.PropertyName)).ToList());
        }
    }
}
