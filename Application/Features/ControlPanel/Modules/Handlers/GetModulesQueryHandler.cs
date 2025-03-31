using AppCommon.DTOs;
using AppCommon.DTOs.Modules;
using AppCommon.GlobalHelpers;
using Application.Common.Handlers;
using Application.Features.ControlPanel.Modules.Mapping;
using Application.Features.ControlPanel.Modules.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Module;

namespace Application.Features.ControlPanel.Modules.Handlers
{
    public class GetModulesQueryHandler(
        IMediator mediator,
        ILogger<BaseQueryHandler<GetModulesQuery, PaginatedList<ModuleDto>>> logger,
        IHttpContextAccessor httpContextAccessor,
        ModuleDbContext moduleDbContext,
        ModuleMapper moduleMapper) : BaseQueryHandler<GetModulesQuery, PaginatedList<ModuleDto>>(mediator, logger, httpContextAccessor)
    {
        private readonly ModuleDbContext _moduleDbContext = moduleDbContext;
        private readonly ModuleMapper _moduleMapper = moduleMapper;

        protected override async Task<ApiResponse<PaginatedList<ModuleDto>>> HandleQuery(GetModulesQuery request, CancellationToken cancellationToken)
        {
            var query = _moduleDbContext.Modules.AsNoTracking().AsQueryable();

            PaginatedList<Module.Domain.Schema.Module> dbModules;

            if (request.ApplicationId.HasValue)
                query = query.Where(x => x.ApplicationId == request.ApplicationId.Value);


            if (request.IsPaging)
                dbModules = await PaginationHelpers.GetPaginatedDataAsync(query, request.PageNumber.Value, request.PageSize.Value, cancellationToken);

            else if (request.IsScrolling)
                dbModules = await PaginationHelpers.GetInfiniteScrollDataAsync(query, request.Offset, request.Limit, cancellationToken);

            else
                dbModules = await PaginationHelpers.GetAllItemsAsync(query, cancellationToken);


            return ApiResponse<PaginatedList<ModuleDto>>.Success(dbModules.MapPaginatedList(_moduleMapper.MapToDto));
        }
    }

}
