using Module.Domain.Schema;
using Module.Domain.Schema.Properties;

namespace Application.Services.DefaultSetupService
{
    public interface IDefaultWorkspaceSetupService
    {
        List<AppAction> AddDefaultActions(Guid workspaceId, Guid userId);
        List<Property> AddDefaultProperties(Guid workspaceId, Guid userId);
        void AddDefaultActionsForWorkspaceModules(Guid workspaceId, Guid userId);
        void AddDefaultWorkflows(List<AppAction> actions, Guid userId,Guid WorkspaceId);

    }
}
