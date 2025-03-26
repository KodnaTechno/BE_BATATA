using Module.Domain.Schema;

namespace Application.Services.DefaultSetupService
{
    public interface IDefaultModuleSetupService
    {
        List<AppAction> AddDefaultActions(Guid workspaceId, Guid userId);
        void AddDefaultProperties(Guid moduleId, Guid userId);
        void AddDefaultWorkflows(List<AppAction> actions, Guid userId);

    }
}
