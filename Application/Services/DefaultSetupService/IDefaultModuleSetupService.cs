namespace Application.Services.DefaultSetupService
{
    public interface IDefaultModuleSetupService
    {
        void AddDefaultActions(Guid moduleId, Guid userId);
        void AddDefaultProperties(Guid moduleId, Guid userId);


    }
}
