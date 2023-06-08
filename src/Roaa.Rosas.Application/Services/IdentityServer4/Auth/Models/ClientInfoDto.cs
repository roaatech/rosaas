namespace Roaa.Rosas.Application.Services.IdentityServer4.Auth.Models
{
    public record ClientInfoDto
    {
        public InfoDto? RosasClient { get; set; }
        public InfoDto? RosasProduct { get; set; }
    }
    public record InfoDto
    {
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
    }
}
