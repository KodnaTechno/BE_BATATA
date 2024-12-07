﻿using Events.Modules.Modules;
using Hangfire;
using MediatR;

namespace Application.Features.ControlPanel.Modules.EventHandlers
{
    public class ModuleCreatedEventHandler : INotificationHandler<ModuleCreatedEvent>
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public ModuleCreatedEventHandler(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        public async Task Handle(ModuleCreatedEvent notification, CancellationToken cancellationToken)
        {
            _backgroundJobClient.Enqueue(() => Console.WriteLine($"Title is {notification.Title}"));

            await Task.CompletedTask;
        }
    }
}
