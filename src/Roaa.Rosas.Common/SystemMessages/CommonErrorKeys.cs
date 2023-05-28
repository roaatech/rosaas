using Roaa.Rosas.Common.Localization;

namespace Roaa.Rosas.Common.SystemMessages
{
    public enum CommonErrorKeys
    {
        [Localization(
            En = "parameter is required",
            Ar = "مُعامل إلزامي")]
        ParameterIsRequired = 1001,


        [Localization(
            En = "The user is blocked",
            Ar = "المستخدم محظور")]
        UserIsBlocked = 1002,


        [Localization(
            En = "The user is not exist",
            Ar = "المستخدم غير موجود")]
        UserNotExist = 1003,


        [Localization(
            En = "Operation execution failed",
            Ar = "فشل تنفيذ العملية")]
        OperationFaild = 1004,


        [Localization(
            En = "This Resource does not exist or may be deleted or You don't have access",
            Ar = "هذه البيانات غير موجوده او ليس لديك صلاحيه")]
        ResourcesNotFoundOrAccessDenied = 1005,


        [Localization(
            En = "Invalid Parameters",
            Ar = "مدخلات غير صحيحه.")]
        InvalidParameters = 1006,


        [Localization(
            En = "This resource has already deleted",
            Ar = "تم حذف هذه البيانات بالفعل")]
        ResourceAlreadyDeleted = 1007,


        [Localization(
           En = "This resource has already deleted, or You don't have access",
           Ar = "تم حذف هذه البيانات بالفعل أو ليس لديك صلاحية")]
        ResourceAlreadyDeletedOrAccessDenied = 1008,


        [Localization(
            En = "this resource already exists",
            Ar = "هذه البيانات موجودة سابقاً")]
        ResourceAlreadyExists = 1009,


        [Localization(
            En = "You are not authorized to do this action",
            Ar = "ليس لديك صلاحيه لهذا الإجراء.")]
        UnAuthorizedAction = 1010,


        [Localization(
             En = "Target date should be greater than today",
             Ar = "التاريخ المحدد يجب أن يكون اكبر من تاريخ اليوم")]
        TheDateMustBeFutureDateError = 1011,


        [Localization(
            En = "The id is required",
            Ar = "المعرف إلزامي")]
        IdIsRequired = 1012,


        [Localization(
          En = "Invalid account info",
          Ar = "بيانات الحساب المدخلة غير صحيحة")]
        UserNotExistByAliasMsg = 1013,




        [Localization(
          En = "Accept-Locale header parameter is required",
          Ar = "Accept-Locale header parameter is required")]
        AcceptLocaleRequired = 1014,



        [Localization(
          En = "Client-Id header parameter is required",
          Ar = "Client-Id header parameter is required")]
        ClientIdRequired = 1015,
    }

}
