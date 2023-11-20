using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class ProductCreatedEvent : BaseInternalEvent
    {
        public Product Product { get; set; }

        public ProductCreatedEvent(Product product)
        {
            Product = product;
        }
    }
}
