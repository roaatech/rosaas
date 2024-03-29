﻿using Newtonsoft.Json;

namespace Roaa.Rosas.Domain.Models.ExternalSystems
{
    public record CreateTenantModel
    {
        [JsonProperty(PropertyName = "tenantName")]
        public string TenantName { get; set; } = string.Empty;
        public Dictionary<string, dynamic> Specifications { get; set; } = new();
    }
}
