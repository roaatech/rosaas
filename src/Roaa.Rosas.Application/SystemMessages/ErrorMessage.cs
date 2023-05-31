using Roaa.Rosas.Common.Localization;

namespace Roaa.Rosas.Application.SystemMessages
{
    public enum ErrorMessage
    {
        [Localization(
          En = "Invalid email or password",
          Ar = "بيانات الحساب المدخلة غير صحيحة")]
        InvalidLogin = 2001,


        [Localization(
            En = "The account is already existed",
            Ar = "الحساب موجود مسبقا")]
        AccountAlreadyExist = 2002,


        [Localization(
            En = "The Email is already used, try another Email",
            Ar = "الايميل مرتبط بحساب آخر، جرب ايميلا مختلفا")]
        EmailAlreadyUsed = 2003,


        [Localization(
            En = "User AccountNot Active. Please contact the administrator.",
            Ar = "تم إيقاف حساب المتسخدم, يرجى الإتصال بمسؤول النظام")]
        UserAccountNotActive = 2004,


        [Localization(
            En = "Can not use the same as the current password, try a new one",
            Ar = "لا يمكن استخدام نفس كلمة المرور الحالية")]
        NewPasswordAndCurrentMustNotEqual = 2005,


        [Localization(
          En = "'Password' must equal to 'Confirm Password'",
          Ar = "غير مطابقة لكلمة المرور")]
        PasswordAndConfirmMustEqual = 2006,


        [Localization(
           En = "You must confirm your email account",
           Ar = "يجب أن تقوم بتوثيق حساب البريد الإلكتروني الخاص بك")]
        UserMustConfirmTheirEmailAccount = 2007,


        [Localization(
           En = "You have already confirmed your email account",
           Ar = "لقد قمت بتوثيق حساب بريدك الإلكتروني مسبقأ")]
        UserAlreadyConfirmedEmailAccount = 2008,


        [Localization(
        En = "The name is already used",
        Ar = "الاسم مستخدم مسبقاً")]
        NameAlreadyUsed = 3001,

    }
}
