namespace Roaa.Rosas.Domain.Entities
{
    public abstract class BaseAuditableEntity : BaseEntity
    {
        public DateTime CreationDate { get; set; }

        public DateTime ModificationDate { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid ModifiedByUserId { get; set; }
    }
}
