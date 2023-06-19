using Roaa.Rosas.Domain.Enums;

namespace Roaa.Rosas.Domain.Models
{
    public record TenantStatusResultModel
    {
        public TenantStatus UpdatedStatus { get; set; }
        public List<ActionResultModel> Actions { get; set; } = new();
    }

    public record ActionResultModel
    {
        public ActionResultModel(TenantStatus status, string name)
        {
            Status = status;
            Name = name;
        }
        public TenantStatus Status { get; set; }
        public string Name { get; set; }
    }
}
