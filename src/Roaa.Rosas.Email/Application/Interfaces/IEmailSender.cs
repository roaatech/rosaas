using Roaa.RoSaaS.Email.Domain.Models;

namespace Roaa.RoSaaS.Email.Application.Interfaces;

public interface IEmailSender
{
    Task<bool> SendEmailAsync(EmailModel email);
}