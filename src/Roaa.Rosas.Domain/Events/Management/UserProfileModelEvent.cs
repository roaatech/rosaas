using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Models;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class UserProfileModelEvent : BaseInternalEvent
    {
        public Guid UserId { get; set; }

        //  public UserProfileModel OldProfile { get; set; }
        public UserProfileModel UpdatedProfile { get; set; }

        public UserProfileModelEvent(Guid userId, UserProfileModel updatedProfile/*, UserProfileModel oldProfile*/)
        {
            UserId = userId;
            //   OldProfile = oldProfile;
            UpdatedProfile = updatedProfile;
        }
    }
}
