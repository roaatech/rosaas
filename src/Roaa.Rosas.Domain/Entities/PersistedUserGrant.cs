namespace Roaa.Rosas.Domain.Entities
{
    public class PersistedUserGrant
    {
        public string Key { get; set; } = string.Empty;
        public Guid UserId { get; set; }
        public string AuthenticationMethod { get; set; } = string.Empty;
        public string ClientId { get; set; } = string.Empty;
        public string Type { get; set; } = string.Empty;
        public DateTime IssuedAt { get; set; }
        public DateTime Expiration { get; set; }
        public DateTime? ConsumedTime { get; set; }
        public bool IsActive { get; set; }
        public string MetaData { get; set; } = string.Empty;
    }
}
