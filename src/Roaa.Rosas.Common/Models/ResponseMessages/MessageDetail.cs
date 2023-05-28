using Roaa.Rosas.Common.Localization;

namespace Roaa.Rosas.Common.Models.ResponseMessages
{
    public class MessageDetail
    {
        public string Message { get; private set; } = string.Empty;
        public string SysCode { get; private set; } = string.Empty;
        public string Parameter { get; private set; } = string.Empty;


        public static MessageDetail New(MessageDetail messageDetail)
        {
            return new MessageDetail { Message = messageDetail.Message, SysCode = messageDetail.SysCode, Parameter = messageDetail.Parameter };
        }
        public static MessageDetail Error(Enum sysCode, LanguageEnum locale, string parameter)
        {
            return new MessageDetail
            {

                Message = sysCode.Localize(locale),
                SysCode = Convert.ToInt32(sysCode).ToString(),
                Parameter = parameter
            };
        }
        public static MessageDetail Error(Enum sysCode, LanguageEnum locale)
        {
            return Error(sysCode, locale, string.Empty);
        }
        public static MessageDetail Error(string message)
        {
            return new MessageDetail { Message = message };
        }

        public static MessageDetail ErrorCode(string sysCode)
        {
            return new MessageDetail { SysCode = sysCode };
        }

        public static MessageDetail Error(string message, string sysCode)
        {
            return new MessageDetail { Message = message, SysCode = sysCode };
        }

        public static MessageDetail Error(string message, Enum sysCode)
        {
            return new MessageDetail { Message = message, SysCode = Convert.ToInt32(sysCode).ToString() };
        }

        public static MessageDetail Error(string message, Enum sysCode, string parameter)
        {
            return new MessageDetail { Message = message, SysCode = Convert.ToInt32(sysCode).ToString(), Parameter = parameter };
        }

        public static MessageDetail Error(string message, string sysCode, string parameter)
        {
            return new MessageDetail { Message = message, SysCode = sysCode, Parameter = parameter };
        }
    }
}
