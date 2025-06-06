﻿using AppCommon.DTOs;
using AppCommon.DTOs.Modules;
using AppCommon.GlobalHelpers;
using Application.Common.Handlers;
using Application.Features.ControlPanel.Workspace.Mapping;
using Application.Features.ControlPanel.Workspace.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Module;

namespace Application.Features.ControlPanel.Workspace.Handlers
{
    public class GetWorkspacesQueryHandler : BaseQueryHandler<GetWorkspacesQuery, PaginatedList<WorkspaceDto>>
    {
        private readonly ModuleDbContext _moduleDbContext;
        private readonly WorkspaceMapper _workspaceMapper;

        public GetWorkspacesQueryHandler(
            IMediator mediator,
            ILogger<BaseQueryHandler<GetWorkspacesQuery, PaginatedList<WorkspaceDto>>> logger,
            IHttpContextAccessor httpContextAccessor,
            ModuleDbContext moduleDbContext,
            WorkspaceMapper workspaceMapper)
            : base(mediator, logger, httpContextAccessor)
        {
            _moduleDbContext = moduleDbContext;
            _workspaceMapper = workspaceMapper;
        }

        protected override async Task<ApiResponse<PaginatedList<WorkspaceDto>>> HandleQuery(GetWorkspacesQuery request, CancellationToken cancellationToken)
        {
            var query = _moduleDbContext.Workspaces.Where(x => x.ApplicationId == request.ApplicationId).AsNoTracking().AsQueryable();

            PaginatedList<Module.Domain.Schema.Workspace> dbWorkspaces;

            if (request.IsPaging)
                dbWorkspaces = await PaginationHelpers.GetPaginatedDataAsync(query, request.PageNumber.Value, request.PageSize.Value, cancellationToken);

            else if (request.IsScrolling)
                dbWorkspaces = await PaginationHelpers.GetInfiniteScrollDataAsync(query, request.Offset, request.Limit, cancellationToken);

            else
                dbWorkspaces = await PaginationHelpers.GetAllItemsAsync(query, cancellationToken);


            return ApiResponse<PaginatedList<WorkspaceDto>>.Success(dbWorkspaces.MapPaginatedList(_workspaceMapper.MapToDto));
        }
    }

}
