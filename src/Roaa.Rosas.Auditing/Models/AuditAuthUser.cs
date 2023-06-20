namespace Roaa.Rosas.Auditing.Models;

public record AuditAuthUser
{
    public string? JWTId { get; set; }
    public string? UserId { get; set; }
    public string? UserObjectId { get; set; }
    public string? Email { get; set; }
    public List<string>? Specifications { get; set; }
}