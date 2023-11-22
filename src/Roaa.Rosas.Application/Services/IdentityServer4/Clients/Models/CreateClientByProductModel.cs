namespace Roaa.Rosas.Application.Services.IdentityServer4.Clients.Models
{
    public class CreateClientByProductModel
    {
        public Guid ProductId { get; set; }
        public Guid ProductOwnerClientId { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
    }
}
