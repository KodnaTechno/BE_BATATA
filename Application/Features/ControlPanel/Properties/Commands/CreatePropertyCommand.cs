using AppCommon.DTOs.Modules;
using AppCommon.DTOs;
using AppCommon.EnumShared;
using Application.Interfaces;
using Events;
using Events.Modules.Properties;

namespace Application.Features.ControlPanel.Properties.Commands
{
    public class CreatePropertyCommand : BaseCommand<PropertyDto>
    {
        public TranslatableValue Title { get; set; }
        public TranslatableValue Description { get; set; }
        public ViewTypeEnum ViewType { get; set; }
        public string Configuration { get; set; }
        public bool IsSystem { get; set; }
        public bool IsInternal { get; set; }
        public string DefaultValue { get; set; }
        public bool IsCalculated { get; set; }
        public bool IsEncrypted { get; set; }
        public bool IsTranslatable { get; set; }
        public int Order { get; set; }

        // Only one of these should be set
        public Guid? ModuleId { get; set; }
        public Guid? WorkspaceId { get; set; }
        public Guid? WorkspaceModuleId { get; set; }
        public Guid? ApplicationId { get; set; }

        public string SystemPropertyPath { get; set; }

        public override IEvent GetEvent(ApiResponse<PropertyDto> response)
            => response.IsSuccess
                ? new PropertyCreatedEvent
                {
                    Id = response.Data.Id,
                    ModuleId = ModuleId,
                    WorkspaceId = WorkspaceId,
                    WorkspaceModuleId = WorkspaceModuleId,
                    ApplicationId = ApplicationId
                }
                : null;
    }
}
