using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Identity;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class UserCreatedAsTenantAdminEvent : BaseInternalEvent
    {
        public User User { get; set; }
        public Guid TenantId { get; set; }
        public bool IsMajor { get; set; }

        public UserCreatedAsTenantAdminEvent(User user, Guid tenantId, bool isMajor = false)
        {
            User = user;
            TenantId = tenantId;
            IsMajor = isMajor;
        }
    }
}
