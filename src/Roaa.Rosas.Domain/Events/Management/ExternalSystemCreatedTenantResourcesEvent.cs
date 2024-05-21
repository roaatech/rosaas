using Roaa.Rosas.Domain.Common;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class ExternalSystemCreatedTenantResourcesEvent : BaseInternalEvent
    {
        public ExternalSystemCreatedTenantResourcesEvent(string tenantSystemName)
        {
            TenantSystemName = tenantSystemName;
        }


        public ExternalSystemCreatedTenantResourcesEvent()
        {
        }
        public string TenantSystemName { get; set; }
    }
}
