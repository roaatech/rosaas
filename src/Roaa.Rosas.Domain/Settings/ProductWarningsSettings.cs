using Roaa.Rosas.Common.Localization;

namespace Roaa.Rosas.Domain.Settings
{
    public class ProductWarningsSettings : ISettings
    {
        public WarningSettingModel DefaultHealthCheckUrl { get; set; } = new WarningSettingModel
        {
            Type = WarningType.Error,
            Message = new LocalizedString
            {
                En = "The Health check Url is required for continuous validation of the availability of the tenant's resources",
                Ar = "رابط فحص الحالة الصحية مطلوب لضمان استمرار التحقق من توفر موارد المستأجر"
            }
        };
        public WarningSettingModel HealthStatusInformerUrl { get; set; } = new WarningSettingModel
        {
            Type = WarningType.Warning,
            Message = new LocalizedString
            {
                En = "This Url is crucial for alerting the external system that one of the tenants is unavailable",
                Ar = "يعد هذا الرابط هاماً لتنبيه النظام الخارجي عندما يكون أحد المستأجرين غير متاح أو غير متوفر"
            }
        };
        public WarningSettingModel CreationUrl { get; set; } = new WarningSettingModel
        {
            Type = WarningType.Error,
            Message = new LocalizedString
            {
                En = "The creation Url is mandatory, and no tenant can be established without it",
                Ar = "رابط الإنشاء إلزامي، ولا يمكن إنشاء مستأجر بدونه"
            }
        };
        public WarningSettingModel ActivationUrl { get; set; } = new WarningSettingModel
        {
            Type = WarningType.Error,
            Message = new LocalizedString
            {
                En = "The activation Url is mandatory, no tenant creation or activation can occur without it",
                Ar = "رابط التنشيط إلزامي، ولا يمكن إنشاء أو تنشيط مستأجر بدونه"
            }
        };
        public WarningSettingModel DeactivationUrl { get; set; } = new WarningSettingModel
        {
            Type = WarningType.Error,
            Message = new LocalizedString
            {
                En = "The activation Url is mandatory, no tenant creation or activation can occur without it",
                Ar = "رابط إلغاء التنشيط إلزامي، ولا يمكن إنشاء أو إلغاء تنشيط مستأجر بدونه"
            }
        };
        public WarningSettingModel DeletionUrl { get; set; } = new WarningSettingModel
        {
            Type = WarningType.Error,
            Message = new LocalizedString
            {
                En = "The activation Url is mandatory, no tenant creation or activation can occur without it",
                Ar = "رابط الحذف إلزامي، ولا يمكن إنشاء أو حذف مستأجر بدونه"
            }
        };
        public WarningSettingModel SubscriptionResetUrl { get; set; } = new WarningSettingModel
        {
            Type = WarningType.Warning,
            Message = new LocalizedString
            {
                En = "You won't be able to reset a tenant's subscription without saving the Subscription Reset Url",
                Ar = "لن تتمكن من إعادة تعيين اشتراك المستأجر دون تخزين رابط لإعادة تعيين الاشتراك"
            }
        };
        public WarningSettingModel SubscriptionUpgradeUrl { get; set; } = new WarningSettingModel
        {
            Type = WarningType.Warning,
            Message = new LocalizedString
            {
                En = "Upgrading a tenant's subscription won't be possible without saving the Subscription Upgrade Url",
                Ar = "لن يكون ترقية اشتراك المستأجر ممكنًا دون تخزين رابط ترقية الاشتراك"
            }
        };
        public WarningSettingModel SubscriptionDowngradeUrl { get; set; } = new WarningSettingModel
        {
            Type = WarningType.Warning,
            Message = new LocalizedString
            {
                En = "Upgrading a tenant's subscription won't be possible without saving the Subscription Upgrade Url",
                Ar = "لن يكون تخفيض اشتراك المستأجر ممكنًا دون تخزين رابط ترقية الاشتراك"
            }
        };
        public WarningSettingModel ApiKey { get; set; } = new WarningSettingModel
        {
            Type = WarningType.Error,
            Message = new LocalizedString
            {
                En = "External system's Api security will be compromised unless you create a private Api's key.",
                Ar = "سيتم اختراق أمان النظام الخارجي ما لم تقم بإنشاء مفتاح خاص."
            }
        };


        public WarningSettingModel PublishedPlans { get; set; } = new WarningSettingModel
        {
            Type = WarningType.Error,
            Message = new LocalizedString
            {
                En = "Published Plans",
                Ar = "خطط متاحة للمستأجر"
            }
        };


        public WarningSettingModel Features { get; set; } = new WarningSettingModel
        {
            Type = WarningType.Error,
            Message = new LocalizedString
            {
                En = "Features",
                Ar = ""
            }
        };

        public WarningSettingModel PublishedPlanPrices { get; set; } = new WarningSettingModel
        {
            Type = WarningType.Error,
            Message = new LocalizedString
            {
                En = "Published Plan's Prices",
                Ar = "اسعار الخطط"
            }
        };

    }


    public class WarningSettingModel
    {
        public LocalizedString Message { get; set; } = new();
        public WarningType Type { get; set; } = new();
    }


    public enum WarningType
    {
        success = 1,
        Information = 2,
        Warning = 3,
        Error = 4,
    }

}
