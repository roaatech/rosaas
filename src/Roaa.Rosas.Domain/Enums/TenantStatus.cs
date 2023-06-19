namespace Roaa.Rosas.Domain.Enums
{

    public enum TenantStatus
    {
        None = -1,
        RecordCreated = 1,
        PreCreating = 2,
        Creating = 3,
        CreatedAsActive = 4,
        PreActivating = 5,
        Activating = 6,
        Active = 7,
        PreDeactivating = 8,
        Deactivating = 9,
        Deactive = 10,
        PreDeleting = 11,
        Deleting = 12,
        Deleted = 13,
    }
}
