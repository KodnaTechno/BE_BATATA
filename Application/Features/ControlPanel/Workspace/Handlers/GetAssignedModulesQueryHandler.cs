using AppCommon.DTOs;
using AppCommon.DTOs.Modules;
using AppCommon.GlobalHelpers;
using Application.Common.Handlers;
using Application.Features.ControlPanel.Workspace.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Module;

namespace Application.Features.ControlPanel.Workspace.Handlers
{
    public class GetAssignedModulesQueryHandler(
        IMediator mediator,
        ILogger<BaseQueryHandler<GetAssignedModulesQuery, PaginatedList<WorkspaceModuleDto>>> logger,
        IHttpContextAccessor httpContextAccessor,
        ModuleDbContext moduleDbContext)
                : BaseQueryHandler<GetAssignedModulesQuery, PaginatedList<WorkspaceModuleDto>>(mediator, logger, httpContextAccessor)
    {
        private readonly ModuleDbContext _moduleDbContext = moduleDbContext;

        protected override async Task<ApiResponse<PaginatedList<WorkspaceModuleDto>>> HandleQuery(
            GetAssignedModulesQuery request,
            CancellationToken cancellationToken)
        {
            var query = _moduleDbContext.WorkspaceModules
                .Include(wsm => wsm.Module)
                .Where(wsm => wsm.WorkspaceId == request.WorkspaceId)
                .AsNoTracking()
                .AsQueryable();

            PaginatedList<Module.Domain.Schema.WorkspaceModule> dbWorkspaceModules;

            if (request.IsPaging)
            {
                dbWorkspaceModules = await PaginationHelpers.GetPaginatedDataAsync(
                    query,
                    request.PageNumber.Value,
                    request.PageSize.Value,
                    cancellationToken);
            }
            else if (request.IsScrolling)
            {
                dbWorkspaceModules = await PaginationHelpers.GetInfiniteScrollDataAsync(
                    query,
                    request.Offset,
                    request.Limit,
                    cancellationToken);
            }
            else
            {
                dbWorkspaceModules = await PaginationHelpers.GetAllItemsAsync(
                    query,
                    cancellationToken);
            }

            var dtoList = dbWorkspaceModules.MapPaginatedList(wsm =>
            {
                return new WorkspaceModuleDto
                {
                    WorkspaceModuleId = wsm.Id,
                    ModuleId = wsm.ModuleId,
                    Display = wsm.Module.Title.GetLocalizedValue(),
                };
            });

            return ApiResponse<PaginatedList<WorkspaceModuleDto>>.Success(dtoList);
        }
    }
}
