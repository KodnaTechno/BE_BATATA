using AppCommon.DTOs.Modules;
using Application.Common.Handlers;
using Application.Common.Models;
using Application.Features.ControlPanel.Workspace.Commands;
using Application.Features.ControlPanel.Workspace.Mapping;
using Application.Services;
using Application.Services.EventsLogger;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Module;
using Module.Domain.Schema;

namespace Application.Features.ControlPanel.Workspace.Handlers
{
    public class SetWorkspaceModulesCommandHandler : BaseCommandHandler<SetWorkspaceModulesCommand, WorkspaceDto>
    {
        private readonly ModuleDbContext _moduleDbContext;
        private readonly WorkspaceMapper _workspaceMapper;
        private readonly IStringLocalizer<object> _localization;
        private readonly ApplicationManager _applicationManager;
        public SetWorkspaceModulesCommandHandler(IMediator mediator,
            ILogger<BaseCommandHandler<SetWorkspaceModulesCommand, WorkspaceDto>> logger,
            IEventLogger eventLogger,
            IHttpContextAccessor httpContextAccessor,
            ModuleDbContext moduleDbContext,
            IStringLocalizer<object> localization,
            ApplicationManager applicationManager,
            WorkspaceMapper workspaceMapper) : base(mediator, logger, eventLogger, httpContextAccessor)
        {
            _moduleDbContext = moduleDbContext;
            _workspaceMapper = workspaceMapper;
            _localization = localization;
            _applicationManager = applicationManager;
        }

        protected override async Task<ApiResponse<WorkspaceDto>> HandleCommand(SetWorkspaceModulesCommand request, CancellationToken cancellationToken)
        {
            var workspace = await _moduleDbContext.Workspaces.FindAsync([request.WorkspaceId], cancellationToken);
            if (workspace == null)
                return ApiResponse<WorkspaceDto>.Fail(ErrorCodes.NotFound, _localization[ErrorCodes.NotFound]);

            var modules = await _moduleDbContext.Modules.Where(x => request.ModuleIds.Contains(x.Id)).ToListAsync();

            /**Update Module Workspace*/

            //Current Workspace Modules

            var currentWorkspaceModules = _moduleDbContext.WorkspaceModules
                .Include(x => x.Module)
                .ThenInclude(x => x.WorkspaceModules)
                .Where(x => x.WorkspaceId == workspace.Id).ToList();

            //Remove Not Mapped Moduels
            var removedModules = currentWorkspaceModules.Where(x => !request.ModuleIds.Contains(x.ModuleId)).ToList();

            _moduleDbContext.WorkspaceModules.RemoveRange(removedModules);

            //Remove Roles if any of removed modules from Team Type
            await _applicationManager.UnLinkRolesBasedOnTeamModules(removedModules);

            //Dealing With Only New Modules (to prevent duplicate any functionality has been already applied on old linked modules)
            var newModuleIds = request.ModuleIds.Where(m => !currentWorkspaceModules.Select(x => x.ModuleId).Contains(m)).ToList();

            var newWorkspaceModules = newModuleIds.Select(moduleId => new WorkspaceModule
            {
                CreatedAt = DateTime.UtcNow,
                CreatedBy = Guid.Parse(request.UserId),
                ModuleId = moduleId,
                WorkspaceId = workspace.Id,
            }).ToList();

            _moduleDbContext.AddRange(newWorkspaceModules);
            _moduleDbContext.SaveChanges();

            //Fetch Team Type Modules and Reflect Roles based on Them 
            var newWorkspaceModulesFromDb = _moduleDbContext.WorkspaceModules
                .Include(x => x.Module)
                .Include(x => x.Workspace)
                .Where(x => x.WorkspaceId == workspace.Id && newModuleIds.Contains(x.ModuleId)).ToList();

            await _applicationManager.LinkRolesBasedOnTeamModules(newWorkspaceModulesFromDb);


            return ApiResponse<WorkspaceDto>.Success(_workspaceMapper.MapToDto(workspace));
        }
    }
}
