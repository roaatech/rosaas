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
}
