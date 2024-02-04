using Roaa.Rosas.Domain.Entities;

namespace Roaa.Rosas.Application.Services.IdentityServer4.Clients.Models
{
    public class GetClientByProductModel
    {
        public GetClientByProductModel(Guid productId, Guid productOwnerClientId)
        {
            ProductId = productId;
            ProductOwnerClientId = productOwnerClientId;
        }

        public Guid ProductId { get; set; }
        public Guid ProductOwnerClientId { get; set; }
    }


    public class GetClientOfExternalSystemModel
    {
        public GetClientOfExternalSystemModel(Guid productId, Guid productOwnerClientId, ClientType clientType)
        {
            ProductId = productId;
            ProductOwnerClientId = productOwnerClientId;
            ClientType = clientType;
        }

        public Guid ProductId { get; set; }
        public Guid ProductOwnerClientId { get; set; }
        public ClientType ClientType { get; set; }
    }
}
