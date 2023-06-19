using Roaa.Rosas.Common.Localization;
using Roaa.Rosas.Common.Models.ResponseMessages;

namespace Roaa.Rosas.Common.Models.Results
{
    public class Result<T> : Result
    {
        public Result(List<MessageDetail> messages)
        {
            Messages = messages;
        }
        public Result()
        {

        }

        public T Data { get; internal set; } = default(T);



        public new static Result<T> New()
        {
            return new Result<T>();
        }
        public new static Result<T> Fail(Enum sysCode, LanguageEnum locale, string parameter)
        {
            return new Result<T>() { Success = false, Messages = new List<MessageDetail> { MessageDetail.Error(sysCode, locale, parameter) } };
        }
        public new static Result<T> Fail(Enum sysCode, LanguageEnum locale)
        {
            return new Result<T>() { Success = false, Messages = new List<MessageDetail> { MessageDetail.Error(sysCode, locale) } };
        }
        public new static Result<T> Fail(List<MessageDetail> messagesDetails)
        {
            return new Result<T>() { Success = false, Messages = messagesDetails };
        }

        public new static Result<T> Fail(string error)
        {
            return new Result<T>() { Success = false, Messages = new List<MessageDetail> { MessageDetail.Error(error) } };
        }

        public new static Result<T> Fail(IEnumerable<string> errors)
        {
            return new Result<T>() { Success = false, Messages = errors.Select(x => MessageDetail.Error(x)).ToList() };
        }

        public new static Result<T> Fail(string error, Enum sysCode)
        {
            return new Result<T>() { Success = false, Messages = new List<MessageDetail> { MessageDetail.Error(error, sysCode) } };
        }
        public static Result<T> Successful(T data)
        {
            return new Result<T>() { Success = true, Data = data };
        }

    }
    public class Result
    {
        public List<MessageDetail> Messages { get; internal set; } = new List<MessageDetail>();

        public bool Success { get; internal set; }
        public Result(List<MessageDetail> messages)
        {
            Messages = messages;
        }
        public Result()
        {
        }
        public static Result New()
        {
            return new Result();
        }
        public static Result Fail(Enum sysCode, LanguageEnum locale, string parameter)
        {
            return new Result() { Success = false, Messages = new List<MessageDetail> { MessageDetail.Error(sysCode, locale, parameter) } };
        }
        public static Result Fail(Enum sysCode, LanguageEnum locale)
        {
            return new Result() { Success = false, Messages = new List<MessageDetail> { MessageDetail.Error(sysCode, locale) } };
        }

        public static Result Fail(List<MessageDetail> messagesDetails)
        {
            return new Result() { Success = false, Messages = messagesDetails };
        }

        public static Result Fail(string error)
        {
            return new Result() { Success = false, Messages = new List<MessageDetail> { MessageDetail.Error(error) } };
        }

        public static Result Fail(IEnumerable<string> errors)
        {
            return new Result() { Success = false, Messages = errors.Select(x => MessageDetail.Error(x)).ToList() };
        }

        public static Result Fail(string error, Enum sysCode)
        {
            return new Result() { Success = false, Messages = new List<MessageDetail> { MessageDetail.Error(error, sysCode) } };
        }

        public static Result Successful()
        {
            return new Result() { Success = true };
        }
    }
}
