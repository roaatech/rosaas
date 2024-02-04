namespace Roaa.Rosas.Application.Services.IdentityServer4.Clients.Models
{
    public class UpdateClientByProductModel
    {
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int AccessTokenLifetimeInHour { get; set; }
    }

}
