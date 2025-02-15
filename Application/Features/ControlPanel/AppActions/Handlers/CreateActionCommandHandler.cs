using Application.Common.Handlers;
using Application.Common.Models;
using Application.Services.EventsLogger;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Module.Domain.Schema;
using Module;
using Application.Features.ControlPanel.AppActions.Commands;
using AppCommon.DTOs.Modules;
using AppCommon.EnumShared;
using Microsoft.EntityFrameworkCore;
using Application.Features.ControlPanel.AppActions.Mapper;

namespace Application.Features.ControlPanel.AppActions.Handlers
{
    public class CreateActionCommandHandler(
        IMediator mediator,
        ILogger<BaseCommandHandler<CreateAppActionCommand, AppActionDto>> logger,
        IEventLogger eventLogger,
        IHttpContextAccessor httpContextAccessor,
        ModuleDbContext context,
        AppActionMapper mapper)
              : BaseCommandHandler<CreateAppActionCommand, AppActionDto>(mediator, logger, eventLogger, httpContextAccessor)
    {
        private readonly ModuleDbContext _context = context;
        private readonly AppActionMapper _mapper = mapper;

        protected override async Task<ApiResponse<AppActionDto>> HandleCommand(
            CreateAppActionCommand request,
            CancellationToken cancellationToken)
        {
            switch (request.ScopeType)
            {
                case ScopeTypeEnum.Workspace:
                    {
                        bool exists = await _context.Workspaces
                            .AnyAsync(w => w.Id == request.ScopeId, cancellationToken);
                        if (!exists)
                            return ApiResponse<AppActionDto>.Fail(ErrorCodes.NotFound, "Workspace not found.");
                        break;
                    }
                case ScopeTypeEnum.Module:
                    {
                        bool exists = await _context.Modules
                            .AnyAsync(m => m.Id == request.ScopeId, cancellationToken);
                        if (!exists)
                            return ApiResponse<AppActionDto>.Fail(ErrorCodes.NotFound, "Module not found.");
                        break;
                    }
                case ScopeTypeEnum.WorkspaceModule:
                    {
                        bool exists = await _context.WorkspaceModules
                            .AnyAsync(wsm => wsm.Id == request.ScopeId, cancellationToken);
                        if (!exists)
                            return ApiResponse<AppActionDto>.Fail(ErrorCodes.NotFound, "WorkspaceModule not found.");
                        break;
                    }
                default:
                    return ApiResponse<AppActionDto>.Fail(ErrorCodes.InvalidOperation, "Invalid scope type.");
            }

            var now = DateTime.UtcNow;
            var appAction = new AppAction
            {
                Name = request.Name,
                Description = request.Description,
                Type = ActionType.Custom,
                CreatedAt = now,
                UpdatedAt = now,
                CreatedBy = request.UserId,
                UpdatedBy = request.UserId
            };

            switch (request.ScopeType)
            {
                case ScopeTypeEnum.Workspace:
                    appAction.WorkspaceId = request.ScopeId;
                    break;
                case ScopeTypeEnum.Module:
                    appAction.ModuleId = request.ScopeId;
                    break;
                case ScopeTypeEnum.WorkspaceModule:
                    appAction.WorkspaceModuleId = request.ScopeId;
                    break;
            }

            _context.AppActions.Add(appAction);
            await _context.SaveChangesAsync(cancellationToken);

            return ApiResponse<AppActionDto>.Success(_mapper.MapToDto(appAction));
        }
    }
}
