using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Identity;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class UserSignedUpUserByEmailEvent : BaseInternalEvent
    {
        public User User { get; set; }
        public string Code { get; set; } = string.Empty;

        public UserSignedUpUserByEmailEvent(User user, string code)
        {
            User = user;
            Code = code;
        }
    }
}
