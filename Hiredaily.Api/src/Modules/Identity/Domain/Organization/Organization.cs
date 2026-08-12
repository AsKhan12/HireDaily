using Hiredaily.BuildingBlock.Domain.Entity;
using Hiredaily.BuildingBlock.Domain.EntityIds;
using Hiredaily.BuildingBlock.Domain.Events;
using Hiredaily.Modules.Identity.Domain.Organization.Events;
using Hiredaily.Modules.Identity.Domain.Organization.ValueObjects;

namespace Hiredaily.Modules.Identity.Domain.Organization;

public class Organization : IEntity<OrganizationId>
{
    private readonly List<IDomainEvent> _events = [];
    public IReadOnlyList<IDomainEvent> Events => _events.AsReadOnly();
    private Organization()
    {
    }

    public Organization(
        string name,
        string username,
        string passwordHash)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Organization name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Email cannot be empty.", nameof(username));

        if (string.IsNullOrWhiteSpace(passwordHash))
            throw new ArgumentException("Password cannot be empty.", nameof(passwordHash));

        Id = new OrganizationId(Guid.NewGuid());
        Name = name.Trim();
        Username = username.Trim();
        PasswordHash = passwordHash;
        Status = OrganizationStatusEnum.Active;
        CreatedAt = DateTime.UtcNow;
        Address = OrganizationAddress.Empty();

        _events.Add(new OrganizationCreatedEvent(Id, null, CreatedAt, OrganizationStatusEnum.Active, Address, null, name));
    }

    public OrganizationId Id { get; private set; } = default!;

    public string Name { get; private set; } = string.Empty;
    public string Username { get; private set; } = default!;
    public string? Description { get; private set; }

    public OrganizationAddress Address { get; private set; } = OrganizationAddress.Empty();

    public OrganizationStatusEnum Status { get; private set; }

    public DateTime CreatedAt { get; private set; }

    public DateTime? UpdatedAt { get; private set; }

    public string PasswordHash { get; private set; } = default!;

    public void Rename(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException("Organization name is required.", nameof(name));

        Name = name;
        UpdatedAt = DateTime.UtcNow;
        _events.Add(new OrganizationRenamedEvent(Id, Name));
    }

    public void UpdateDescription(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Organization description is required.", nameof(description));

        Description = description.Trim();
        UpdatedAt = DateTime.UtcNow;
    }

    public void ChangeAddress(OrganizationAddress address)
    {
        Address = address ?? throw new ArgumentNullException(nameof(address));
        UpdatedAt = DateTime.UtcNow;
        // _events.Add(new OrganizationAddressUpdatedEvent(Id, Address));
    }

    public void Suspend()
    {
        Status = OrganizationStatusEnum.Suspended;
        UpdatedAt = DateTime.UtcNow;
        _events.Add(new OrganizationSuspendedEvent(Id));
    }

    public void Activate()
    {
        Status = OrganizationStatusEnum.Active;
        UpdatedAt = DateTime.UtcNow;
        // _events.Add(new OrganizationActivatedEvent(Id));
    }

    public void ClearEvents()
    {
        _events.Clear();
    }
}
