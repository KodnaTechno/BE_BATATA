using AppCommon.DTOs.Modules;
using Application.Common.Handlers;
using Application.Common.Models;
using Application.Features.ControlPanel.Modules.Commands;
using Application.Features.ControlPanel.Workspace.Mapping;
using Application.Services.EventsLogger;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Module;

namespace Application.Features.ControlPanel.Modules.Handlers
{
    public class CreateModuleCommandHandler : BaseCommandHandler<CreateModuleCommand, ModuleDto>
    {
        private readonly ModuleDbContext _moduleDbContext;
        private readonly WorkspaceMapper _workspaceMapper;

        public CreateModuleCommandHandler(IMediator mediator,
            ILogger<BaseCommandHandler<CreateModuleCommand, ModuleDto>> logger,
            IEventLogger eventLogger,
            IHttpContextAccessor httpContextAccessor,
            ModuleDbContext moduleDbContext,
            WorkspaceMapper workspaceMapper) : base(mediator, logger, eventLogger, httpContextAccessor)
        {
            _moduleDbContext = moduleDbContext;
            _workspaceMapper = workspaceMapper;
        }

        protected override async Task<ApiResponse<ModuleDto>> HandleCommand(CreateModuleCommand request, CancellationToken cancellationToken)
        {
            //var workspace = _workspaceMapper.MapToEntity(request);
            //_moduleDbContext.Workspaces.Add(workspace);
            //await _moduleDbContext.SaveChangesAsync(cancellationToken);

            //return ApiResponse<ModuleDto>.Success(_workspaceMapper.MapToDto(workspace));
            return default(ApiResponse<ModuleDto>);
        }
    }
}
