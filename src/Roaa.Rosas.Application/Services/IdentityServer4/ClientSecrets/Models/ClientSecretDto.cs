namespace Roaa.Rosas.Application.Services.IdentityServer4.ClientSecrets.Models
{
    public class ClientSecretDto
    {
        public int Id { get; set; }
        public int ClientRecordId { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string? Description { get; set; } = null;
        public DateTime? Expiration { get; set; }
        public DateTime Created { get; set; }
    }
}
