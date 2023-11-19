using Roaa.Rosas.Domain.Settings;

namespace Roaa.Rosas.Application.Services.Management.Products.Queries.GetProductWarnings
{
    public class ProductWarningsDto
    {
        public string Property { get; set; } = string.Empty;
        public bool IsValid { get; set; }
        public WarningSettingModel? Setting { get; set; } = new();
    }
}
