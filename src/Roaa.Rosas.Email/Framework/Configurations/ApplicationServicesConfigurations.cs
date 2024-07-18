using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Roaa.RoSaaS.Email.Application.Interfaces;
using Roaa.RoSaaS.Email.Domain.Options;
using Roaa.RoSaaS.Email.Infrastructure.SendGrid;

namespace Roaa.RoSaaS.Email.Framework.Configurations;

public static class ApplicationServicesConfigurations
{
    public static void AddApplicationServicesConfigurations(this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment,
        RootOptions rootOptions)
    {
        services.AddScoped<IEmailSender, SendGridEmailSender>();
    }
}