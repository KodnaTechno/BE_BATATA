using AppCommon.DTOs;
using AppCommon.DTOs.Modules;
using Application.Common.Handlers;
using Application.Features.ControlPanel.Workspace.Mapping;
using Application.Features.ControlPanel.Workspace.Queries;
using Infrastructure.Caching;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Module;

namespace Application.Features.ControlPanel.Workspace.Handlers
{
    public class GetWorkspaceQueryHandler : BaseQueryHandler<GetWorkspaceQuery, WorkspaceDto>
    {
        private readonly ModuleDbContext _moduleDbContext;
        private readonly WorkspaceMapper _workspaceMapper;
        private readonly IStringLocalizer<object> _localization;
        public GetWorkspaceQueryHandler(
            IMediator mediator,
            ILogger<BaseQueryHandler<GetWorkspaceQuery, WorkspaceDto>> logger,
            IHttpContextAccessor httpContextAccessor,
            ModuleDbContext moduleDbContext,
            WorkspaceMapper workspaceMapper,
            IEntityCacheService entityCacheService,
            IStringLocalizer<object> localization)
            : base(mediator, logger, httpContextAccessor)
        {
            _moduleDbContext = moduleDbContext;
            _workspaceMapper = workspaceMapper;
            _localization = localization;
        }

        protected override async Task<ApiResponse<WorkspaceDto>> HandleQuery(GetWorkspaceQuery request, CancellationToken cancellationToken)
        {
            //var workspace = await _entityCacheService.GetOrSetAsync(
            //    CacheKeys.Workspace(request.WorkspaceId),
            //   async () => await _moduleDbContext.Workspaces.FindAsync([request.WorkspaceId], cancellationToken)
            //);

            var workspace = await _moduleDbContext.Workspaces.FindAsync([request.WorkspaceId], cancellationToken);
            if (workspace == null)
                return ApiResponse<WorkspaceDto>.Fail(ErrorCodes.NotFound, _localization[ErrorCodes.NotFound]);

            return ApiResponse<WorkspaceDto>.Success(_workspaceMapper.MapToDto(workspace));
        }
    }
}

