namespace Roaa.Rosas.Domain.Entities
{
    public class ClientCustomDetail
    {
        public int ClientId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductOwnerClientId { get; set; }
    }

}
