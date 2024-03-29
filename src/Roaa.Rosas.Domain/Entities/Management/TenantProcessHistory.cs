﻿using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Entities.Management
{
    public class TenantProcessHistory : BaseEntity
    {
        public Guid TenantId { get; set; }

        public Guid ProductId { get; set; }

        public Guid SubscriptionId { get; set; }

        public TenantStatus Status { get; set; }

        public TenantStep Step { get; set; }

        public ExpectedTenantResourceStatus ExpectedResourceStatus { get; set; }

        public TenantProcessType ProcessType { get; set; }

        public string? Data { get; set; }

        public Guid? OwnerId { get; set; }

        public UserType OwnerType { get; set; }

        public DateTime ProcessDate { get; set; }

        public DateTime TimeStamp { get; set; }

        public int UpdatesCount { get; set; } = 1;

        public bool Enabled { get; set; } = true;

        public virtual ICollection<ProcessNote> Notes { get; set; } = new List<ProcessNote>();
    }

    public class ProcessNote
    {
        public ProcessNote(UserType ownerType, string text)
        {
            OwnerType = ownerType;
            Text = text;
        }
        public ProcessNote()
        {
        }
        public UserType OwnerType { get; set; }

        public string Text { get; set; } = string.Empty;
    }


    public enum TenantProcessType
    {
        RecordCreated = 1,
        DataUpdated,
        MetadataUpdated,
        StatusChanged,
        HealthyStatus,
        UnhealthyStatus,
        ExternalSystemSuccessfullyInformed,
        FailedToInformExternalSystem,
        SubscriptionWasSetAsInactiveDueToNonRenewal,
        SpecificationsUpdated,
        SubscriptionResetPrepared,
        SubscriptionResetAppliedDone,
        SubscriptionResetApplicationFailed,
        SubscriptionFeatureLimitReset,
        SubscriptionAutoRenewalEnabled,
        SubscriptionAutoRenewalCanceled,
        SubscriptionRenewed,
        SubscriptionUpgradeRequested, // تم طلب ترقية الاشتراك
        SubscriptionUpgradePrepared,// تم ترقية خطة الاشتراك في نظام رصاص 
        SubscriptionUpgradeBeingApplied,//جارٍ تطبيق ترقية الاشتراك 
        SubscriptionUpgradeApplicationFailed, // فشل عملية تطبيق ترقية الاشتراك في النظام الخارجي
        SubscriptionUpgradeAppliedDone,//تم تطبيق ترقية الاشتراك 
        SubscriptionDowngradeRequested,
        SubscriptionDowngradePrepared,
        SubscriptionDowngradeBeingApplied,
        SubscriptionDowngradeApplicationFailed,
        SubscriptionDowngradeAppliedDone,
    }



}
