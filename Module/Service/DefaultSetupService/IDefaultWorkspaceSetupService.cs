namespace Module.Service.DefaultSetupService
{
    public interface IDefaultWorkspaceSetupService
    {
        void AddDefaultActions(Guid workspaceId, Guid userId);
        void AddDefaultProperties(Guid workspaceId, Guid userId);
        void AddDefaultActionsForWorkspaceModules(Guid workspaceId, Guid userId);

    }
}
