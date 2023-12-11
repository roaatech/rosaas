namespace Roaa.Rosas.Domain.Entities.Management
{
    public class EntityAdminPrivilege : BaseAuditableEntity
    {
        public Guid EntityId { get; set; }
        public Guid UserId { get; set; }
        public EntityType EntityType { get; set; }
        public UserType UserType { get; set; }
        public bool IsMajor { get; set; }
    }
}
