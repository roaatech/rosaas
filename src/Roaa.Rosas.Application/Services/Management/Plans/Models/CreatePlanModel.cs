namespace Roaa.Rosas.Application.Services.Management.Plans.Models
{
    public record CreatePlanModel
    {
        public Guid ProductId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
