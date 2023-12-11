namespace Roaa.Rosas.Authorization.Utilities
{
    public class AuthPolicy
    {
        public const string SuperAdmin = "SoperAdminPolicy";
        public const string ExternalSystem = "ExternalSystemPolicy";
        public const string Payment = "PaymentPolicy";
        public class Management
        {
            public const string Features = "Features";
            public const string GeneralPlans = "GeneralPlans";
            public const string PlanFeatures = "PlanFeatures";
            public const string PlanPrices = "PlanPrices";
            public const string Plans = "Plans";
            public const string Products = "Products";
            public const string Settings = "Settings";
            public const string Specifications = "Specifications";
            public const string Subscriptions = "Subscriptions";
            public const string Tenants = "Tenants";
            public const string Workflow = "Workflow";
        }

        public class Identity
        {
            public const string TeneatAdminUser = "TeneatAdminUser";
            public const string ProductAdminUser = "ProductAdminUser";
            public const string ClientAdminUser = "ClientAdminUser";
            public const string Users = "Account";
            public const string Account = "Account";
            public const string ClientCredential = "ClientCredential";
        }

    }
}
