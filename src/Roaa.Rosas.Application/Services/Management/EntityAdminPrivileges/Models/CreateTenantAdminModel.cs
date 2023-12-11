using Roaa.Rosas.Common.Enums;

namespace Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges.Models
{
    public record CreateResourceAdminModel
    {
        public Guid EntityId { get; set; }
        public EntityType EntityType { get; set; }
        public Guid UserId { get; set; }
        public UserType UserType { get; set; }
        public bool IsMajor { get; set; }
    }
}
