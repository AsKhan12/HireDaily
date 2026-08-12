using Hiredaily.BuildingBlock.Domain.Events;

namespace Hiredaily.Modules.Identity.Domain.User.Events;

public sealed record UserCreatedEvent : IDomainEvent
{
    public User User {get; private set;}

    public UserCreatedEvent(User user)
    {
        User = user;
    }
}