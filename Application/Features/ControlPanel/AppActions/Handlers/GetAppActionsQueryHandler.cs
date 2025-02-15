using AppCommon.DTOs.Modules;
using AppCommon.GlobalHelpers;
using Application.Common.Handlers;
using Application.Common.Models;
using Application.Features.ControlPanel.AppActions.Mapper;
using Application.Features.ControlPanel.AppActions.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Module;

namespace Application.Features.ControlPanel.AppActions.Handlers
{
    public class GetAppActionsQueryHandler(
        IMediator mediator,
        ILogger<BaseQueryHandler<GetActionsQuery, PaginatedList<AppActionDto>>> logger,
        IHttpContextAccessor httpContextAccessor,
        ModuleDbContext context,
        AppActionMapper mapper)
                : BaseQueryHandler<GetActionsQuery, PaginatedList<AppActionDto>>(mediator, logger, httpContextAccessor)
    {
        private readonly ModuleDbContext _context = context;
        private readonly AppActionMapper _mapper = mapper;

        protected override async Task<ApiResponse<PaginatedList<AppActionDto>>> HandleQuery(
            GetActionsQuery request,
            CancellationToken cancellationToken)
        {
            var query = _context.AppActions
                .AsNoTracking()
                .AsQueryable();

            if (request.WorkspaceId.HasValue)
                query = query.Where(a => a.WorkspaceId == request.WorkspaceId.Value);

            if (request.ModuleId.HasValue)
                query = query.Where(a => a.ModuleId == request.ModuleId.Value);

            if (request.WorkspaceModuleId.HasValue)
                query = query.Where(a => a.WorkspaceModuleId == request.WorkspaceModuleId.Value);

            PaginatedList<Module.Domain.Schema.AppAction> dbActions;
            if (request.IsPaging)
            {
                dbActions = await PaginationHelpers.GetPaginatedDataAsync(
                    query,
                    request.PageNumber.Value,
                    request.PageSize.Value,
                    cancellationToken);
            }
            else if (request.IsScrolling)
            {
                dbActions = await PaginationHelpers.GetInfiniteScrollDataAsync(
                    query,
                    request.Offset,
                    request.Limit,
                    cancellationToken);
            }
            else
            {
                dbActions = await PaginationHelpers.GetAllItemsAsync(
                    query,
                    cancellationToken);
            }

            return ApiResponse<PaginatedList<AppActionDto>>.Success(dbActions.MapPaginatedList(_mapper.MapToDto));
        }
    }
}
