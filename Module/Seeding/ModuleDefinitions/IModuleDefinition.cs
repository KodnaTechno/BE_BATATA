using Module.Domain.Schema;
using Module.Domain.Schema.Properties;

namespace Module.Seeding.ModuleDefinitions
{
    public interface IModuleDefinition
    {
        Domain.Schema.Module GetModule();
        IEnumerable<Property> GetProperties();
        IEnumerable<AppAction> GetBaseActions();
    }
}
