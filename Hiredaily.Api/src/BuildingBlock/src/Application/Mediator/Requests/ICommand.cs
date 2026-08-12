namespace Hiredaily.BuildingBlock.Application.Mediator.Requests;

public interface ICommand
{
    Guid RequestId { get; }
    DateTime RequestedAt { get; }
    string RequestedBy { get; }
}
