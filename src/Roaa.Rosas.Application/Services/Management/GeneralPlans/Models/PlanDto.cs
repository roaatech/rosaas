namespace Roaa.Rosas.Application.Services.Management.GeneralPlans.Models
{
    public record PlanDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime EditedDate { get; set; }

    }
}
