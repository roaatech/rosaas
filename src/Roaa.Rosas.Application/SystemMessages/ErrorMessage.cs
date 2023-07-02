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
           En = "Invalid client credential",
           Ar = "بيانات اعتماد العميل غير صحيحة")]
        InvalidClientCredential = 2002,


        [Localization(
            En = "The account is already existed",
            Ar = "الحساب موجود مسبقا")]
        AccountAlreadyExist = 2003,


        [Localization(
            En = "The account is deactivated. Please contact the system administrator",
            Ar = "تم إيقاف الحساب, يرجى الإتصال بمسؤول النظام")]
        AccountDeactivated = 2004,


        [Localization(
             En = "You must confirm your email account",
             Ar = "يجب أن تقوم بتوثيق حساب البريد الإلكتروني الخاص بك")]
        UserMustConfirmTheirEmailAccount = 2005,



        [Localization(
        En = "The name is already used",
        Ar = "الاسم مستخدم مسبقاً")]
        NameAlreadyUsed = 3001,


        [Localization(
        En = "The Tenant has already been activated.",
        Ar = "تم التفعيل بالفعل")]
        TenantaAlreadyActivated = 3002,


        [Localization(
        En = "The Tenant has already been deactivated.",
        Ar = "تم إلغاء التفعيل بالفعل")]
        TenantaAlreadyDeactivated = 3003,


        [Localization(
        En = "The Tenant has already been deleted.",
        Ar = "تم الحذف بالفعل")]
        TenantaAlreadyDeleted = 3003,


        [Localization(
           En = "The Url is already existed",
           Ar = "عنوان الرابط موجود مسبقا")]
        UrlAlreadyExist = 3004,


        [Localization(
           En = "Deleting is not allowed",
           Ar = "الحذف غير مسموح به")]
        DeletingIsNotAllowed = 3005,


    }
}
