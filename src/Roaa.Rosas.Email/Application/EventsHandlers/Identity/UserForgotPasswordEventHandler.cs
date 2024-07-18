using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Roaa.RoSaaS.Email.Application.Interfaces;
using Roaa.RoSaaS.Email.Domain.Models;
using Roaa.RoSaaS.Email.Domain.Options;
using Roaa.Rosas.Application.Interfaces;
using Roaa.Rosas.Common.ApiConfiguration;
using Roaa.Rosas.Common.Extensions;
using Roaa.Rosas.Domain.Events.Management;

namespace Roaa.Rosas.Email.Application.EventsHandlers.Identity;

public class UserForgotPasswordEventHandler : IInternalDomainEventHandler<UserForgotPasswordMessageEvent>
{

    #region Props

    private readonly IEmailSender _emailSender;
    private readonly ILogger<UserForgotPasswordEventHandler> _logger;
    private readonly RootOptions _settings;

    #endregion


    #region Corts 
    public UserForgotPasswordEventHandler(IEmailSender emailSender,
                                          ILogger<UserForgotPasswordEventHandler> logger,
                                          IApiConfigurationService<RootOptions> settings)
    {
        _logger = logger;
        _emailSender = emailSender;
        _settings = settings.Options;
    }

    #endregion


    #region Services

    public async Task Handle(UserForgotPasswordMessageEvent @event, CancellationToken cancellationToken)
    {

        _logger.LogInformation($"Message-Event-Handler:{@event.GetType().Name}", JsonConvert.SerializeObject(@event));

        Uri template = new(_settings.TemplateFilesRootPath + "/emails/reset-password.html");

        var variables = new Dictionary<string, string>
        {
            {
                "##ResetPasswordUrl##",
                _settings.ResetPassword.WebPageUrl + $"?code={@event.Code}&email={@event.User.Email}"
            }
        };

        await _emailSender.SendEmailAsync(new EmailModel
        {
            UseSystemFromEmail = true,
            Subject = _settings.ResetPassword.Subject,
            Html = (await template.LoadContentAsync()).ReplaceVariablesInTemplate(variables),
            Recipients = new List<string> { @event.User.Email }
        });
    }

    #endregion
}