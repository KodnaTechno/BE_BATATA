using AppCommon.DTOs;
using Application.Common.Handlers;
using Application.Features.ControlPanel.Workspace.Commands;
using Application.Services.EventsLogger;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Module;
using Module.Domain.Schema;

namespace Application.Features.ControlPanel.Workspace.Handlers
{
    public class AssignWorkspaceModulesCommandHandler(
        IMediator mediator,
        ILogger<BaseCommandHandler<AssignWorkspaceModulesCommand, bool>> logger,
        IEventLogger eventLogger,
        IHttpContextAccessor httpContextAccessor,
        ModuleDbContext moduleDbContext)
                : BaseCommandHandler<AssignWorkspaceModulesCommand, bool>(mediator, logger, eventLogger, httpContextAccessor)
    {
        private readonly ModuleDbContext _moduleDbContext = moduleDbContext;

        protected override async Task<ApiResponse<bool>> HandleCommand(
            AssignWorkspaceModulesCommand request,
            CancellationToken cancellationToken)
        {
            var workspace = await _moduleDbContext.Workspaces
                .FirstOrDefaultAsync(w => w.Id == request.WorkspaceId, cancellationToken);

            if (workspace == null)
                return ApiResponse<bool>.Fail(ErrorCodes.NotFound, "Workspace not found.");

            var existingWorkspaceModules = await _moduleDbContext.WorkspaceModules
                .Where(wsm => wsm.WorkspaceId == workspace.Id)
                .ToListAsync(cancellationToken);

            var existingModuleIds = existingWorkspaceModules
                .Select(wsm => wsm.ModuleId)
                .ToList();

            var requestedModuleIds = request.ModuleIds.Distinct().ToList();


            var toRemoveIds = existingModuleIds.Except(requestedModuleIds).ToList();
            var toAddIds = requestedModuleIds.Except(existingModuleIds).ToList();

            if (toRemoveIds.Count != 0)
            {
                var workspaceModulesToRemove = existingWorkspaceModules
                    .Where(wsm => toRemoveIds.Contains(wsm.ModuleId))
                    .ToList();

                _moduleDbContext.WorkspaceModules.RemoveRange(workspaceModulesToRemove);
            }

            if (toAddIds.Count != 0)
            {
                var modulesToAdd = await _moduleDbContext.Modules
                    .Where(m => toAddIds.Contains(m.Id))
                    .ToListAsync(cancellationToken);

                if (modulesToAdd.Count != toAddIds.Count)
                    return ApiResponse<bool>.Fail(ErrorCodes.ValidationError, "One or more Module IDs are invalid.");


                foreach (var mod in modulesToAdd)
                {
                    if (mod.ApplicationId == null || mod.ApplicationId == Guid.Empty)
                    {
                        mod.ApplicationId = workspace.ApplicationId;
                        _moduleDbContext.Modules.Update(mod);
                    }
                    else if (mod.ApplicationId != workspace.ApplicationId)
                    {
                        return ApiResponse<bool>.Fail( ErrorCodes.ValidationError,
                            $"Module '{mod.Key}' is assigned to a different Application."
                        );
                    }
                }

                var now = DateTime.UtcNow;
                foreach (var mod in modulesToAdd)
                {
                    _moduleDbContext.WorkspaceModules.Add(new WorkspaceModule
                    {
                        WorkspaceId = workspace.Id,
                        ModuleId = mod.Id,
                        CreatedBy = request.UserId,
                        UpdatedBy = request.UserId,
                        CreatedAt = now,
                        UpdatedAt = now
                    });
                }
            }

            await _moduleDbContext.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Success(true);
        }
    }
}
