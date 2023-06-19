using Microsoft.AspNetCore.Identity;
using Roaa.Rosas.Common.Localization;
using Roaa.Rosas.Common.Models.ResponseMessages;
using Roaa.Rosas.Common.Models.Results;
using Roaa.Rosas.RequestBroker.Models;

namespace Roaa.Rosas.Application.Extensions
{
    public static class ResultExtensions
    {
        public static Result FailResult(this IdentityResult identityResult, LanguageEnum locale)
        {
            var messages = identityResult.Errors
                                         .Select(e => MessageDetail.Error(e.Code.ToIdentityError(), locale))
                                         .ToList();

            return Result.Fail(messages);
        }
        public static Result FailResult(this IdentityResult identityResult, SystemMessages.IdentityError inCase, SystemMessages.IdentityError output, LanguageEnum locale)
        {
            var messages = identityResult.Errors
                                         .Select(e => MessageDetail.Error(e.Code.ToIdentityError().ToIdentityError(inCase, output), locale))
                                         .ToList();

            return Result.Fail(messages);
        }




        public static Result<T> GetResult<T>(this RequestResult<T> requestResult)
        {
            if (requestResult.Success)
            {
                return Result<T>.Successful(requestResult.Data);
            }

            var sss = requestResult.Errors.Select(x => x.Value.Select(val => MessageDetail.Error(val, x.Key))).SelectMany(x => x).ToList();

            return Result<T>.Fail(sss);
        }
    }
}
