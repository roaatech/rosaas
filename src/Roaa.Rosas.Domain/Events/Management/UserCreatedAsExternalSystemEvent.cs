using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Identity;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class UserCreatedAsExternalSystemEvent : BaseInternalEvent
    {
        public User User { get; set; }
        public Guid ProductId { get; set; }
        public bool IsMajor { get; set; }

        public UserCreatedAsExternalSystemEvent(User user, Guid productId, bool isMajor = false)
        {
            User = user;
            ProductId = productId;
            IsMajor = isMajor;
        }
    }
}
