using Microsoft.Extensions.Logging;
using Roaa.RoSaaS.Email.Application.Interfaces;
using Roaa.RoSaaS.Email.Domain.Models;
using Roaa.RoSaaS.Email.Domain.Options;
using Roaa.Rosas.Common.ApiConfiguration;
using Roaa.Rosas.Common.Extensions;
using SendGrid;
using SendGrid.Helpers.Mail;

namespace Roaa.RoSaaS.Email.Infrastructure.SendGrid;

public class SendGridEmailSender : IEmailSender
{
    #region Ctors

    public SendGridEmailSender(IApiConfigurationService<SendGridOptions> settings,
        ILogger<SendGridEmailSender> logger)
    {
        _logger = logger;
        _fromEmail = new EmailAddress(settings.Options.FromEmail, settings.Options.FromName);
        _sendGridClient = new SendGridClient(settings.Options.ApiKey);
    }

    #endregion

    #region Props

    private readonly ILogger<SendGridEmailSender> _logger;
    private readonly SendGridClient _sendGridClient;
    private readonly EmailAddress _fromEmail;

    #endregion

    #region Methods

    public async Task<bool> SendEmailAsync(EmailModel email)
    {
        var message = new SendGridMessage();
        message.SetFrom(GetFromEmail(email));
        message.SetSubject(email.Subject);
        message.AddTos(email.Recipients.Select(email => new EmailAddress(email)).ToList());

        if (email.CC is not null && email.CC.Count > 0)
        {
            message.AddCcs(email.CC.Select(m => new EmailAddress(m)).ToList());
        }

        if (email.BCC is not null && email.BCC.Count > 0)
        {
            message.AddCcs(email.BCC.Select(m => new EmailAddress(m)).ToList());
        }

        if (!string.IsNullOrEmpty(email.Text))
        {
            message.AddContent(MimeType.Text, email.Text);
        }

        if (!string.IsNullOrEmpty(email.Html))
        {
            message.AddContent(MimeType.Html, email.Html);
        }

        if (email.Attachments is not null && email.Attachments.Count > 0)
        {
            message.AddAttachments(email.Attachments.Select(att => new Attachment
            {
                Content = att.Content,
                Filename = att.Content ?? $"{DateTime.UtcNow.TimeStamp()}",
                Type = att.Type,
                Disposition = att.Disposition
            }).ToList());
        }

        var _response = await _sendGridClient.SendEmailAsync(message);
        var body = await _response.Body.ReadAsStringAsync();

        if (!_response.IsSuccessStatusCode)
        {
            _logger.LogDebug($"failed to send email due to errors: {_response.StatusCode} - {_response.Body}");
        }

        return true;
    }

    private EmailAddress GetFromEmail(EmailModel model)
    {
        if (!model.UseSystemFromEmail && !string.IsNullOrEmpty(model.From))
        {
            return new EmailAddress(model.From);
        }

        return _fromEmail;
    }

    #endregion
}