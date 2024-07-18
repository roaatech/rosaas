using Roaa.Rosas.Common.Models;

namespace Roaa.RoSaaS.Email.Domain.Options;

public record RootOptions : BaseOptions
{
    public const string Section = "EmailApi";
    public SendGridOptions SendGrid { get; set; } = new();
    public ResetPasswordOptions ResetPassword { get; set; } = new();
    public EmailConfirmationOptions EmailConfirmation { get; set; } = new();
    public string TemplateFilesRootPath { get; set; } = string.Empty;
}

public record EmailConfirmationOptions : BaseOptions
{
    public const string Section = "EmailConfirmation";
    public string WebPageUrl { get; set; } = string.Empty;
    public string Subject { get; set; } = "RoSaaS Platform - Account Activation";
}
public record ResetPasswordOptions : BaseOptions
{
    public const string Section = "ResetPassword";
    public string WebPageUrl { get; set; } = string.Empty;
    public string Subject { get; set; } = "RoSaaS Platform - Reset your password";
}