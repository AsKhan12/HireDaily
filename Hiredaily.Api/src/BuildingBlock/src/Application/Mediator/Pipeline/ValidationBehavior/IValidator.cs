using Hiredaily.BuildingBlock.Application.Mediator.Requests;

namespace Hiredaily.BuildingBlock.Application.Mediator.Pipeline.ValidationBehaviour;

public interface IValidator<TRequest>
    where TRequest : ICommand
{
    Task<ValidationResult> ValidateAsync(TRequest request, CancellationToken cancellationToken);
}
