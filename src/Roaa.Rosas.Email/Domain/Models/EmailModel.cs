namespace Roaa.RoSaaS.Email.Domain.Models;

public class EmailModel
{
    public string Subject { get; set; } = string.Empty;

    public string Text { get; set; } = string.Empty;

    public string Html { get; set; } = string.Empty;

    public string ReasonOfSend { get; set; } = string.Empty;

    public bool UseSystemFromEmail { get; set; }

    public string From { get; set; } = string.Empty;

    public List<string> Recipients { get; set; } = new();

    public List<string> CC { get; set; } = new();

    public List<string> BCC { get; set; } = new();

    public List<MailAttachment> Attachments { get; set; } = new();
}