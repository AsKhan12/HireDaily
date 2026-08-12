using Hiredaily.BuildingBlock.Domain.Entity;
using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Domain.Events;
using Hiredaily.BuildingBlock.Domain.ValueObjects;
using Hiredaily.Modules.Identity.Domain.User.Events;
using Hiredaily.Modules.Identity.Domain.User.ValueObject;

namespace Hiredaily.Modules.Identity.Domain.User;

public class User : IEntity<UserId>
{
    private readonly List<IDomainEvent> _events = [];
    private readonly List<Skill> _skills = [];

    private User()
    {
    }

    public User(
        string name,
        string username,
        string? passwordHash = null)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty.", nameof(username));

        Name = name.Trim();
        Username = username.Trim();

        Id = new UserId(Guid.NewGuid());
        CreatedAt = DateTime.UtcNow;

        PasswordHash = passwordHash;
        Address = UserAddress.Empty();

        // _events.Add(new UserCreatedEvent(Id));
    }

    public UserId Id { get; private set; } = default!;

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }
    public string Name { get; private set; } = default!;
    public string Username { get; private set; } = default!;
    public UserAddress Address { get; private set; } = UserAddress.Empty();

    public IReadOnlyList<Skill> Skills  => _skills.AsReadOnly();

    public IReadOnlyList<IDomainEvent> Events => _events.AsReadOnly();

    public string? PasswordHash { get; private set; }

    public void UpdateName(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Name cannot be empty.", nameof(name));

        Name = name.Trim();
        UpdatedAt = DateTime.UtcNow;

        // _events.Add(new UserNameUpdatedEvent(Id));
    }

    public void UpdateAddress(UserAddress address)
    {
        Address = address ?? throw new ArgumentNullException(nameof(address));

        UpdatedAt = DateTime.UtcNow;

        _events.Add(new UserAddressUpdatedEvent(Id));
    }

    public void AddSkill(Skill skill)
    {
        ArgumentNullException.ThrowIfNull(skill);
        _skills.Add(skill);
        UpdatedAt = DateTime.UtcNow;
        // _events.Add(new UserSkillsUpdatedEvent(Id));
    }
    public bool RemoveSkill(Skill skill)
    {
        ArgumentNullException.ThrowIfNull(skill);
        if(_skills.Remove(skill))
        {
            UpdatedAt = DateTime.UtcNow;
            // _events.Add(new UserSkillsUpdatedEvent(Id));
            return true;
        }
        return false;
    }

    public void ClearEvents()
    {
        _events.Clear();
    }
}
