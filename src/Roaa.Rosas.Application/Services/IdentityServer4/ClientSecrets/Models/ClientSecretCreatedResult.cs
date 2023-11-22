namespace Roaa.Rosas.Application.Services.IdentityServer4.ClientSecrets.Models
{
    public class ClientSecretCreatedResult
    {
        public ClientSecretCreatedResult(int id, string secrest)
        {
            Id = id;
            Secrest = secrest;
        }

        public int Id { get; set; }
        public string Secrest { get; set; } = string.Empty;
    }
}
