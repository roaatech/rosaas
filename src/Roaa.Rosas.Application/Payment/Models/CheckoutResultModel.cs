﻿namespace Roaa.Rosas.Application.Payment.Models
{
    public record CheckoutResultModel
    {
        public string? NavigationUrl { get; set; }
        public Guid? TenantId { get; set; }
    }


    public record PaymentMethodCheckoutResultModel
    {
        public string? PaymentLink { get; set; }
    }





}



