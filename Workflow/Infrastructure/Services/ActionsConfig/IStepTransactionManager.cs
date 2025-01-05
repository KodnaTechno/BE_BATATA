namespace AppWorkflow.Infrastructure.Services.Actions;

using System.Text;

public interface IStepTransactionManager
    {
        Task BeginTransactionAsync(Guid instanceId);
        Task CommitTransactionAsync(Guid instanceId);
        Task RollbackTransactionAsync(Guid instanceId);
        Task<bool> IsInTransactionAsync(Guid instanceId);
    }