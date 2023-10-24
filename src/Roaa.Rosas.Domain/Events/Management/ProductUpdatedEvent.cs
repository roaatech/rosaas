using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class ProductUpdatedEvent : BaseInternalEvent
    {
        public Product UpdatedProduct { get; set; }
        public Product OldProduct { get; set; }

        public ProductUpdatedEvent(Product updatedProduct, Product oldProduct)
        {
            UpdatedProduct = updatedProduct;
            OldProduct = oldProduct;
        }
    }
}
