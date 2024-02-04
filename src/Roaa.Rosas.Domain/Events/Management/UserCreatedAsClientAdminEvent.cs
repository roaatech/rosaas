using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Identity;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class UserCreatedAsClientAdminEvent : BaseInternalEvent
    {
        public User User { get; set; }
        public bool IsMajor { get; set; }
        public Guid ClientId { get; set; }

        public UserCreatedAsClientAdminEvent(User user, Guid clientId, bool isMajor = false)
        {
            User = user;
            ClientId = clientId;
            IsMajor = isMajor;
        }
    }
}
