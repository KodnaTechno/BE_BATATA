using AppCommon.DTOs.Modules;
using Application.Interfaces;
using Events;
using AppCommon.DTOs;
using AppCommon.EnumShared;
using Events.Modules.AppActions;

namespace Application.Features.ControlPanel.AppActions.Commands
{
    public class CreateAppActionCommand : BaseCommand<AppActionDto>
    {
        public ScopeTypeEnum ScopeType { get; set; }
        public Guid ScopeId { get; set; }

        public TranslatableValue Name { get; set; }
        public TranslatableValue Description { get; set; }

        public override IEvent GetEvent(ApiResponse<AppActionDto> response)
            => response.IsSuccess
            ? new AppActionCreatedEvent
            {
                Id = response.Data.Id
            } : null;
    }
}
