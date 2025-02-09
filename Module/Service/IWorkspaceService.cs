using Module.Domain.Schema;

namespace Module.Service
{
    public interface IWorkspaceService
    {
        Task<Workspace> GetWorkspaceByIdAsync(Guid id);
        Task<Workspace> CreateWorkspaceAsync(Workspace workspace, Guid UserId);
        Task<Workspace> UpdateWorkspaceAsync(Workspace workspace, Guid UserId);
        Task<bool> DeleteWorkspaceAsync(Guid id, Guid UserId);
    }
}
