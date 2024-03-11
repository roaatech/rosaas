using Roaa.Rosas.Common.Enums;

namespace Roaa.Rosas.Domain.Models
{
    public record CheckoutCreatorModel
    {
        public Guid CheckoutCreatedByUserId { get; set; }
        public UserType CheckoutCreatedByUserType { get; set; }
        public DateTime CreationDate { get; set; }
    }
}
