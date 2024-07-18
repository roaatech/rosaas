using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Identity;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class UserForgotPasswordMessageEvent : BaseInternalEvent
    {
        public User User { get; set; }
        public string Code { get; set; } = string.Empty;

        public UserForgotPasswordMessageEvent(User user, string code)
        {
            User = user;
            Code = code;
        }
    }
}
