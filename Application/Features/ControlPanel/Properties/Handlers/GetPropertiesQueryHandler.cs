using AppCommon.DTOs;
using AppCommon.DTOs.Modules;
using AppCommon.GlobalHelpers;
using Application.Common.Handlers;
using Application.Features.ControlPanel.Properties.Mapper;
using Application.Features.ControlPanel.Properties.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Module;

namespace Application.Features.ControlPanel.Properties.Handlers
{
    public class GetPropertiesQueryHandler : BaseQueryHandler<GetPropertiesQuery, PaginatedList<PropertyDto>>
    {
        private readonly ModuleDbContext _moduleDbContext;
        private readonly PropertyMapper _propertyMapper;

        public GetPropertiesQueryHandler(
           IMediator mediator,
           ILogger<BaseQueryHandler<GetPropertiesQuery, PaginatedList<PropertyDto>>> logger,
           IHttpContextAccessor httpContextAccessor,
           ModuleDbContext moduleDbContext,
           PropertyMapper propertyMapper)
           : base(mediator, logger, httpContextAccessor)
        {
            _moduleDbContext = moduleDbContext;
            _propertyMapper = propertyMapper;
        }

        protected override async Task<ApiResponse<PaginatedList<PropertyDto>>> HandleQuery(GetPropertiesQuery request, CancellationToken cancellationToken)
        {
            if (!request.ApplicationId.HasValue &&
                !request.WorkspaceId.HasValue &&
                !request.ModuleId.HasValue &&
                !request.WorkspaceModuleId.HasValue)
            {
                return ApiResponse<PaginatedList<PropertyDto>>.Fail(ErrorCodes.InvalidOperation,
                    "At least one filter (ApplicationId, WorkspaceId, ModuleId, or WorkspaceModuleId) must be provided");
            }

            var query = _moduleDbContext.Properties
                .Where(p => !p.IsDeleted);

            if (request.ApplicationId.HasValue)
                query = query.Where(p => p.ApplicationId == request.ApplicationId);

            if (request.WorkspaceId.HasValue)
                query = query.Where(p => p.WorkspaceId == request.WorkspaceId);

            if (request.ModuleId.HasValue)
                query = query.Where(p => p.ModuleId == request.ModuleId);

            if (request.WorkspaceModuleId.HasValue)
                query = query.Where(p => p.WorkspaceModuleId == request.WorkspaceModuleId);

            query = query.OrderBy(p => p.Order);


            PaginatedList<Module.Domain.Schema.Properties.Property> dbProperties;

            if (request.IsPaging)
                dbProperties = await PaginationHelpers.GetPaginatedDataAsync(query, request.PageNumber.Value, request.PageSize.Value, cancellationToken);

            else if (request.IsScrolling)
                dbProperties = await PaginationHelpers.GetInfiniteScrollDataAsync(query, request.Offset, request.Limit, cancellationToken);

            else
                dbProperties = await PaginationHelpers.GetAllItemsAsync(query, cancellationToken);


            return ApiResponse<PaginatedList<PropertyDto>>.Success(dbProperties.MapPaginatedList(_propertyMapper.MapToDto));
        }
    }
}
