﻿using Application.Interfaces;
using Events.Modules.Properties;
using Events;
using AppCommon.DTOs;

namespace Application.Features.ControlPanel.Properties.Commands
{
    public class DeletePropertyCommand : BaseCommand<bool>
    {
        public Guid PropertyId { get; set; }

        public override IEvent GetEvent(ApiResponse<bool> response)
            => response.IsSuccess
                ? new PropertyDeletedEvent
                {
                    Id = PropertyId
                }
                : null;
    }
}
