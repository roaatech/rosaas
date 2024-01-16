namespace Roaa.Rosas.Domain.Entities
{
    public class ClientCustomDetail
    {
        public int ClientId { get; set; }
        public Guid ProductId { get; set; }
        public Guid ProductOwnerClientId { get; set; }
        public Guid UserId { get; set; }
        public ClientType ClientType { get; set; }
    }

    public enum ClientType
    {
        None = 0,
        ExternalSystem = 101,
        ExternalSystemClient = 102,
    }

}
