using Roaa.Rosas.Common.Enums;

namespace Roaa.Rosas.Domain.Models
{
    public record BaseUserAccountItem
    {
        public Guid Id { get; set; }
        public string Email { get; set; } = string.Empty;
        public UserType UserType { get; set; }
        public string Locale { get; set; } = string.Empty;
        public bool EmailConfirmed { get; set; }
    }
}
