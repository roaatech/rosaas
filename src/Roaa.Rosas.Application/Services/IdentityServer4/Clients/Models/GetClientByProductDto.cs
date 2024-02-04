using Roaa.Rosas.Domain.Entities;

namespace Roaa.Rosas.Application.Services.IdentityServer4.Clients.Models
{
    public class GetClientByProductDto
    {
        public int ClientRecordId { get; set; }
        public string ClientId { get; set; } = string.Empty;
    }



    public class ClientDto
    {
        public int Id { get; set; }
        public int ClientRecordId { get; set; }
        public string ClientId { get; set; } = string.Empty;
        public string DisplayName { get; set; } = string.Empty;
        public string? Description { get; set; } = null;
        public bool IsActive { get; set; }
        public ClientType ClientType { get; set; }
        public DateTime CreatedDate { get; set; }
        public int AccessTokenLifetimeInHour { get; set; }
    }
}
