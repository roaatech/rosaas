namespace Roaa.RoSaaS.Email.Domain.Models;

public class MailAttachment
{
    public string Content { get; set; } = string.Empty;


    public string Type { get; set; } = string.Empty;


    public string Filename { get; set; } = string.Empty;


    public string Disposition { get; set; } = string.Empty;


    public string ContentId { get; set; } = string.Empty;
}