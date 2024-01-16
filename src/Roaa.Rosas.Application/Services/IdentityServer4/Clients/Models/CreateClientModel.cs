using Roaa.Rosas.Domain.Entities;

namespace Roaa.Rosas.Application.Services.IdentityServer4.Clients.Models
{
    public class CreateClientModel
    {
        public Guid ProductId { get; set; }
        public Guid ProductOwnerClientId { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public ClientType ClientType { get; set; }
        public int AccessTokenLifetimeInHour { get; set; } = 1;
        public bool AllowGenerateClientId { get; set; }
    }

}
