namespace Roaa.Rosas.Auditing.Models;

public class AuditEntity
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public long TimeStamp { get; set; }
    public string? Method { get; set; }
    public string? Action { get; set; }
    public string? UserId { get; set; }
    public string? UserType { get; set; }
    public string? Client { get; set; }
    public string? ExternalSystem { get; set; }
    public int Duration { get; set; }
    public string? JsonData { get; set; }

}
