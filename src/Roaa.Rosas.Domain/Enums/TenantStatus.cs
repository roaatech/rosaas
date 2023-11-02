namespace Roaa.Rosas.Domain.Enums
{

    public enum TenantStatus
    {
        None = -1,

        RecordCreated = 1,


        SendingCreationRequest = 2,
        Creating = 3,
        CreatedAsActive = 4,


        SendingActivationRequest = 5,
        Activating = 6,
        Active = 7,


        SendingDeactivationRequest = 8,
        Deactivating = 9,
        Inactive = 10,


        SendingDeletionRequest = 11,
        Deleting = 12,
        Deleted = 13,


        Failure = 14,
    }

    public enum TenantStep
    {
        None = -1,

        Creation = 1,
        Activation = 2,
        Deactivation = 3,
        Deletion = 4,
    }
    public enum ExpectedTenantResourceStatus
    {
        None = -1,

        Active = 2,
        Inactive = 3,
        Deleted = 4,
    }
}
