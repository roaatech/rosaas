﻿namespace Roaa.Rosas.Application.IdentityServer4
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
            public const string OsosExternalSystem = "roaa_osos_external_system";

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
                    public const string RosasProductId = "88e67328-3b20-413e-b6e1-010b48fa7bc9";
                }
            }
        }
    }
}
