using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Common.Localization;

namespace Roaa.Rosas.Common.Models.ResponseMessages
{
    public class ResponseMetadata
    {
        public List<MessageDetail> Errors { get; set; } = new();

        public bool Success => !Errors.Any();

        public void AddError(string errorMessage)
        {
            Errors.Add(MessageDetail.Error(errorMessage));
        }

        public void AddError(Enum sysCode, LanguageEnum locale)
        {
            Errors.Add(MessageDetail.Error(sysCode, locale));
        }

        public void AddError(Exception ex)
        {
            var errorMessage = ex.GetErrorMessage();
            Errors.Add(MessageDetail.Error(errorMessage));
        }

        public void Add(ResponseMetadata responseMessage)
        {
            foreach (var item in responseMessage.Errors)
            {
                Errors.Add(MessageDetail.New(item));
            }
        }
    }
}
