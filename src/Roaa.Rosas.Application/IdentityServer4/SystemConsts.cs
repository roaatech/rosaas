namespace Roaa.Rosas.Application.IdentityServer4
{
    public class SystemConsts
    {
        public class Scopes
        {
            public const string Api = "api_scope";
            public const string SuperAdmin = "super_admin_scope";
            public const string ClientAdmin = "client_admin_scope";
            public const string Tenant = "tenant_scope";
            public const string ExternalSystem = "external_system_scope";
        }
        public class Resources
        {
            public const string RosasApi = "rosas_api_resource";
        }

        public class Clients
        {
            public const string AdminPanel = "spa_rosas_admin_panel";
            public const string BetaOsosExternalSystem = "roaa_beta_osos_external_system";
            public const string OsosExternalSystem = "roaa_osos_external_system";
            public const string ShamsExternalSystem = "roaa_shams_external_system";

            public class Claims
            {
                public const string ClaimProductId = "pid";
                public const string ClaimType = "type";
                public const string ClaimAuthenticationMethod = "amth";
                public const string ExternalSystem = "external_system";
            }

            public class Properties
            {
                public const string RosasClientId = "RosasClientId";
                public const string RosasProductId = "RosasProductId";

                public class Vlaue
                {
                    public const string RosasClientId = "88283b02-e969-485a-a5a3-9e5d1d0d3337";
                    public const string BetaOsosProductId = "c50a938c-df94-4d7f-bbb1-6a73c408b300";
                    public const string OsosProductId = "88e67328-3b20-413e-b6e1-010b48fa7bc9";
                    public const string ShamsProductId = "858df12a-9980-4e38-b8e1-3a19ee5bc600";
                }
            }
        }
    }
}
