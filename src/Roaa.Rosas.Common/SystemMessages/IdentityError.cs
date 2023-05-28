using Roaa.Rosas.Common.Localization;

namespace Roaa.Rosas.Application.SystemMessages
{
    /*based on asp.net core identity error types
   see identity errors: https://github.com/aspnet/Identity/blob/c7276ce2f76312ddd7fccad6e399da96b9f6fae1/src/Core/Resources.resx */
    public enum IdentityError
    {
        [Localization(En = "Identity - An error has occured, please try again later.", Ar = "حدث خطأ في العملية، يرجى المحاولة مرة أخرى.")]
        DefaultError = 2201,
        [Localization(En = "Current password is not correct", Ar = "كلمة المرور الحالية غير صحيحة.")]
        PasswordMismatch = 2202,
        [Localization(En = "Code is either expired or not recognized for this email, please try again.", Ar = "الكود المستخدم إما غير فعال أو لا يصلح لهذا البريد الإلكتروني، يرجى المحاولة مرة أخرى.")]
        InvalidToken = 2203,
        [Localization(En = "User name is invalid", Ar = "اسم المستخدم غير صحيح.")]
        InvalidUserName = 2204,
        [Localization(En = "Email is invalid", Ar = "البريد الإلكتروني غير صحيح.")]
        InvalidEmail = 2205,
        [Localization(En = "User name is already used", Ar = "اسم المستخدم موجود مسبقا.")]
        DuplicateUserName = 2206,
        [Localization(En = "Email is already used", Ar = "البريد الإلكتروني موجود مسبقا.")]
        DuplicateEmail = 2207,
        [Localization(En = "Password too short", Ar = "كلمة المرور قصيرة")]
        PasswordTooShort = 2208,
        [Localization(En = "Password requires unique chars", Ar = "كلمة المرور يجب أن تحتوي على حروف مميزة")]
        PasswordRequiresUniqueChars = 2209,
        [Localization(En = "Password requires non alphanumerics", Ar = "كلمة المرور يجب أن لا تحتوي على أرقام أو حروف")]
        PasswordRequiresNonAlphanumeric = 2210,
        [Localization(En = "Password requires lower chars", Ar = "كلمة المرور يجب أن تحتوي على حروف انجليزية صغيرة ")]
        PasswordRequiresLower = 2211,
        [Localization(En = "Password requires upper chars", Ar = "كلمة المرور يجب أن تحتوي انجليزية كبيرة")]
        PasswordRequiresUpper = 2212,
        [Localization(En = " The activation token has expired, sign in again to get a new link", Ar = "رابط التفعيل غير صالح، قم بتسجيل الدخول من جديد لإرسال رابط تفعيل آخر")]
        InvalidConfirmationToken = 2213,
    }

}
