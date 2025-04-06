using Module.Domain.Data;

namespace Application.Services.Rendering
{
    /// <summary>
    /// Common interface for rendering a single PropertyData into a user-facing object.
    /// This interface is async to support external service calls or database lookups.
    /// </summary>
    public interface IValueRenderer
    {
        Task<object> RenderValueAsync(PropertyData propertyData, CancellationToken cancellationToken = default);
    }
}
