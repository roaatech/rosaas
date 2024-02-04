using Roaa.Rosas.Common.Enums;
using Roaa.Rosas.Domain.Common;
using Roaa.Rosas.Domain.Entities.Management;

namespace Roaa.Rosas.Domain.Events.Management
{
    public class ProductCreatedEvent : BaseInternalEvent
    {
        public Product Product { get; set; }
        public Guid UserId { get; set; }
        public UserType UserType { get; set; }

        public ProductCreatedEvent(Product product, Guid userId, UserType userType)
        {
            Product = product;
            UserId = userId;
            UserType = userType;
        }
    }
}
