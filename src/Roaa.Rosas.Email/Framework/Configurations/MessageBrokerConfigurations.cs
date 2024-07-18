using MediatR;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Roaa.RoSaaS.Email.Domain.Options;
using Roaa.Rosas.Email.Application.EventsHandlers.Identity;
namespace Roaa.RoSaaS.Email.Framework.Configurations;

public static class MessageBrokerConfigurations
{
    public static void AddMessageBrokerConfigurations(this IServiceCollection services,
        IConfiguration configuration,
        IWebHostEnvironment environment,
        RootOptions rootOptions)
    {
        services.AddMediatR(typeof(UserMustConfirmTheEmailEventHandler));
    }
}