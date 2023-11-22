namespace Roaa.Rosas.Application.Services.IdentityServer4.ClientSecrets.Models
{
    public class CreateClientSecretModel
    {
        public CreateClientSecretModel(string? description, DateTime? expiration)
        {
            Description = description;
            Expiration = expiration;
        }

        public string? Description { get; set; } = null;
        public DateTime? Expiration { get; set; }
    }
}
