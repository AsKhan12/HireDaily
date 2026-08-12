namespace Hiredaily.Modules.Identity.Application.Models;

/// <summary>
/// Represents a stored refresh token with its associated user or organization.
/// </summary>
public class RefreshTokenStore
{
    public RefreshTokenStore()
    {
    }

    public RefreshTokenStore(Guid id, Guid? userId, Guid? organizationId, string token, DateTime expiresAt)
    {
        Id = id;
        UserId = userId;
        OrganizationId = organizationId;
        Token = token;
        ExpiresAt = expiresAt;
        CreatedAt = DateTime.UtcNow;
    }

    public Guid Id { get; set; }
    public Guid? UserId { get; set; }
    public Guid? OrganizationId { get; set; }
    public string Token { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
    public DateTime CreatedAt { get; set; }
}
