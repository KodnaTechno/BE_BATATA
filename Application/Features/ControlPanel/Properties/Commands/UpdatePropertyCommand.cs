using AppCommon.DTOs.Modules;
using AppCommon.DTOs;
using Application.Common.Models;
using Application.Interfaces;
using Events.Modules.Properties;
using Events;

namespace Application.Features.ControlPanel.Properties.Commands
{
    public class UpdatePropertyCommand : BaseCommand<PropertyDto>
    {
        public Guid PropertyId { get; set; }
        public TranslatableValue Title { get; set; }
        public TranslatableValue Description { get; set; }
        public string Configuration { get; set; }
        public bool IsInternal { get; set; }
        public string DefaultValue { get; set; }
        public bool IsCalculated { get; set; }
        public bool IsEncrypted { get; set; }
        public bool IsTranslatable { get; set; }
        public int Order { get; set; }

        public override IEvent GetEvent(ApiResponse<PropertyDto> response)
            => response.IsSuccess
                ? new PropertyUpdatedEvent
                {
                    Id = PropertyId,
                }
                : null;
    }
}
