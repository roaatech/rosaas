﻿namespace Roaa.Rosas.Domain.Models.ExternalSystems
{
    public record CreateTenantModel
    {
        public string TenantName { get; set; } = string.Empty;
    }
}
