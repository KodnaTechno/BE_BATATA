namespace AppWorkflow.Services.Interfaces;

using System.Text;

public interface IDistributedLockManager
    {
        Task<IDisposable> AcquireLockAsync(string lockKey, TimeSpan timeout);
        Task<bool> IsLockedAsync(string lockKey);
        Task ReleaseLockAsync(string lockKey);
    }