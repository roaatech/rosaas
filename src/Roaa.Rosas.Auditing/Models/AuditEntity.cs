namespace Roaa.Rosas.Auditing.Models;

public class AuditEntity
{
    public int Id { get; set; }
    public DateTime CreatedDate { get; set; }
    public long TimeStamp { get; set; }
    public string? Method { get; set; }
    public string? Action { get; set; }
    public Guid? UserId { get; set; }
    public int UserType { get; set; }
    public int Duration { get; set; }
    public string? JsonData { get; set; }
}
