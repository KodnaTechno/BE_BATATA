using AppCommon.DTOs.Modules;
using Application.Common.Handlers;
using Application.Common.Models;
using Application.Features.ControlPanel.Workspace.Commands;
using Application.Features.ControlPanel.Workspace.Mapping;
using Application.Services.EventsLogger;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Module;

namespace Application.Features.ControlPanel.Workspace.Handlers
{
    public class CreateWorkspaceCommandHandler : BaseCommandHandler<CreateWorkspaceCommand, WorkspaceDto>
    {
        private readonly ModuleDbContext _moduleDbContext;
        private readonly WorkspaceMapper _workspaceMapper;

        public CreateWorkspaceCommandHandler(IMediator mediator,
            ILogger<BaseCommandHandler<CreateWorkspaceCommand, WorkspaceDto>> logger,
            IEventLogger eventLogger,
            IHttpContextAccessor httpContextAccessor,
            ModuleDbContext moduleDbContext,
            WorkspaceMapper workspaceMapper) : base(mediator, logger, eventLogger, httpContextAccessor)
        {
            _moduleDbContext = moduleDbContext;
            _workspaceMapper = workspaceMapper;
        }

        protected override async Task<ApiResponse<WorkspaceDto>> HandleCommand(CreateWorkspaceCommand request, CancellationToken cancellationToken)
        {
            var workspace = _workspaceMapper.MapToEntity(request);
            _moduleDbContext.Workspaces.Add(workspace);
            await _moduleDbContext.SaveChangesAsync(cancellationToken);

            return ApiResponse<WorkspaceDto>.Success(_workspaceMapper.MapToDto(workspace));
        }
    }
}
