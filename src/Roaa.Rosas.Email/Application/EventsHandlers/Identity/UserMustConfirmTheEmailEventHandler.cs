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

public class UserMustConfirmTheEmailEventHandler : IInternalDomainEventHandler<UserSignedUpUserByEmailEvent>
{

    #region Props

    private readonly IEmailSender _emailSender;
    private readonly ILogger<UserMustConfirmTheEmailEventHandler> _logger;
    private readonly RootOptions _settings;

    #endregion


    #region Corts

    public UserMustConfirmTheEmailEventHandler(IEmailSender emailSender,
                                               ILogger<UserMustConfirmTheEmailEventHandler> logger,
                                               IApiConfigurationService<RootOptions> settings)

    {
        _logger = logger;
        _emailSender = emailSender;
        _settings = settings.Options;
    }

    #endregion


    #region Services

    public async Task Handle(UserSignedUpUserByEmailEvent @event, CancellationToken cancellationToken)
    {
        _logger.LogInformation($"Message-Event-Handler:{@event.GetType().Name}",
            JsonConvert.SerializeObject(@event));

        Uri template = new(_settings.TemplateFilesRootPath + "/emails/email-confirmation.html");

        var variables = new Dictionary<string, string>
        {
            {
                "##EmailConfirmationUrl##",
                _settings.EmailConfirmation.WebPageUrl + $"?code={@event.Code}&email={@event.User.Email}"
            }
        };

        await _emailSender.SendEmailAsync(new EmailModel
        {
            UseSystemFromEmail = true,
            Subject = _settings.EmailConfirmation.Subject,
            Html = (await template.LoadContentAsync()).ReplaceVariablesInTemplate(variables),
            Recipients = new List<string> { @event.User.Email }
        });
    }


    #endregion
}