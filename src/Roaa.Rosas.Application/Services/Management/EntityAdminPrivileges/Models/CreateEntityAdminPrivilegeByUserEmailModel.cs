using Roaa.Rosas.Common.Enums;

namespace Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges.Models
{
    public record CreateEntityAdminPrivilegeByUserEmailModel
    {
        public string Email { get; set; } = string.Empty;
        public Guid EntityId { get; set; }
        public EntityType EntityType { get; set; }
        public bool IsMajor { get; set; }
    }
}
