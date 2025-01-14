namespace AppWorkflow.Infrastructure.Services.Actions;

using System.Text;

public interface IStepTransactionManager
    {
        Task BeginTransactionAsync(Guid instanceId);
        Task CommitTransactionAsync(Guid instanceId);
        Task RollbackTransactionAsync(Guid instanceId);
        Task<bool> IsInTransactionAsync(Guid instanceId);
    }

public class StepTransactionManager : IStepTransactionManager 
{
    public Task BeginTransactionAsync(Guid instanceId)
    {
        throw new NotImplementedException();
    }

    public Task CommitTransactionAsync(Guid instanceId)
    {
        throw new NotImplementedException();
    }

    public Task RollbackTransactionAsync(Guid instanceId)
    {
        throw new NotImplementedException();
    }

    public Task<bool> IsInTransactionAsync(Guid instanceId)
    {
        throw new NotImplementedException();
    }
}