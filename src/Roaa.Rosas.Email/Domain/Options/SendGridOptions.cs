using Roaa.Rosas.Common.Models;

namespace Roaa.RoSaaS.Email.Domain.Options;

public record SendGridOptions : BaseOptions
{
    public const string Section = "SendGrid";
    public string FromEmail { get; set; } = string.Empty;
    public string FromName { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
}