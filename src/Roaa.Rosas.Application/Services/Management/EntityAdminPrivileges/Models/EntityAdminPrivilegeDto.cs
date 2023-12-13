using Roaa.Rosas.Common.Enums;

namespace Roaa.Rosas.Application.Services.Management.EntityAdminPrivileges.Models
{
    public record EntityAdminPrivilegeDto
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public UserType UserType { get; set; }
        public bool IsMajor { get; set; }
    }
}
