namespace Roaa.Rosas.Application.Services.Management.Plans.Models
{
    public record UpdatePlanModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}
/*
 
    public record UpdatePlanServiceModel : UpdatePlanModel
    {
        public Guid Id { get; set; }

        public UpdatePlanServiceModel(UpdatePlanModel model, Guid id)
        {
            Id = id;
            Name = model.Name;
            Description = model.Description;
            DisplayOrder = model.DisplayOrder;
        }
    }

    public record UpdatePlanModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }

        public UpdatePlanServiceModel ToServiceModel(Guid id)
        {
            return new UpdatePlanServiceModel(this, id);
        }
    }
 */