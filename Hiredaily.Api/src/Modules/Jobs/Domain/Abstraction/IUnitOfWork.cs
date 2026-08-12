namespace Hiredaily.Modules.Jobs.Domain.Abstraction;

public interface IUnitOfWork
{
    public Task CommitAsync(CancellationToken cancellationToken= default); 
}
