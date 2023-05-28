namespace Roaa.Rosas.Domain.Entities
{
    public abstract class BaseAuditableEntity : BaseEntity
    {
        public DateTime Created { get; set; }

        public DateTime Edited { get; set; }

        public Guid CreatedByUserId { get; set; }

        public Guid EditedByUserId { get; set; }
    }
}
