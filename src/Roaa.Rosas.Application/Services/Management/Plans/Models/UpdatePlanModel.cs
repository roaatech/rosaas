﻿namespace Roaa.Rosas.Application.Services.Management.Plans.Models
{
    public record UpdatePlanModel
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int DisplayOrder { get; set; }
    }
}