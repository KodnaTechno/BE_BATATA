using AppCommon.DTOs.Modules;

namespace AppCommon.Shared
{
    public interface IPropertiesProvider
    {
        List<PropertyDto> GetProperties(List<string> PropertiesGUIDs);

    }
}
