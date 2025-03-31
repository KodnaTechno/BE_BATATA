using AppCommon.EnumShared;
using Module.Domain.Data;

namespace Application.Services.Rendering
{
    public interface IPropertyValueRendererService
    {
        Task<object> RenderValueAsync(PropertyData propertyData, CancellationToken cancellationToken = default);
    }

    public class PropertyValueRendererService : IPropertyValueRendererService
    {
        private readonly IDictionary<ViewTypeEnum, IValueRenderer> _rendererMap;
        private readonly IValueRenderer _fallbackRenderer;

        public PropertyValueRendererService(
            TextValueRenderer textRenderer,
            NumericValueRenderer numericRenderer,
            BooleanValueRenderer boolRenderer,
            DateValueRenderer dateRenderer,
            UserValueRenderer userRenderer,
            LookupValueRenderer lookupRenderer,
            AttachmentValueRenderer attachmentRenderer,
            DynamicValueRenderer dynamicRenderer,
            ConnectionForeignKeyRenderer connectionRenderer
        )
        {
            _rendererMap = new Dictionary<ViewTypeEnum, IValueRenderer>
            {
                { ViewTypeEnum.Text, textRenderer },
                { ViewTypeEnum.LongText, textRenderer },
                { ViewTypeEnum.RichText, textRenderer },

                { ViewTypeEnum.Int, numericRenderer },
                { ViewTypeEnum.Float, numericRenderer },
                { ViewTypeEnum.Currency, numericRenderer },
                { ViewTypeEnum.Percentage, numericRenderer },

                { ViewTypeEnum.CheckBox, boolRenderer },

                { ViewTypeEnum.Date, dateRenderer },
                { ViewTypeEnum.Time, dateRenderer },
                { ViewTypeEnum.DateTime, dateRenderer },
                { ViewTypeEnum.Month, dateRenderer },
                { ViewTypeEnum.Week, dateRenderer },
                { ViewTypeEnum.Quarter, dateRenderer },

                { ViewTypeEnum.User, userRenderer },
                { ViewTypeEnum.MultiUser, userRenderer },

                { ViewTypeEnum.Lookup, lookupRenderer },
                { ViewTypeEnum.MultiLookup, lookupRenderer },

                { ViewTypeEnum.Attachment, attachmentRenderer },
                { ViewTypeEnum.MultiAttachment, attachmentRenderer },

                { ViewTypeEnum.Api, dynamicRenderer },
                { ViewTypeEnum.Dynamic, dynamicRenderer },
                { ViewTypeEnum.MappedPropertyList, dynamicRenderer },
                { ViewTypeEnum.ExternalDisplayValue, dynamicRenderer },

                { ViewTypeEnum.Connection, connectionRenderer },
                { ViewTypeEnum.ForeignKey, connectionRenderer },
                { ViewTypeEnum.ModuleReference, connectionRenderer },
            };

            _fallbackRenderer = textRenderer;
        }

        public async Task<object> RenderValueAsync(PropertyData propertyData, CancellationToken cancellationToken = default)
        {
            if (_rendererMap.TryGetValue(propertyData.ViewType, out var renderer))
                return await renderer.RenderValueAsync(propertyData, cancellationToken)
                    .ConfigureAwait(false);
            else
                return await _fallbackRenderer.RenderValueAsync(propertyData, cancellationToken)
                    .ConfigureAwait(false);
        }
    }
}
