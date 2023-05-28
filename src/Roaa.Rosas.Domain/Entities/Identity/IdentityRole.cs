using Microsoft.AspNetCore.Identity;

namespace Roaa.Rosas.Domain.Entities.Identity
{
    public class Role : IdentityRole<Guid>
    {
        public Role()
        {
            Id = Guid.NewGuid();
        }

        public Role(string roleName) : this()
        {
            Name = roleName;
        }
    }
}