﻿using AppCommon.DTOs;
using AppCommon.DTOs.Modules;
using Application.Common.Handlers;
using Application.Features.ControlPanel.Workspace.Commands;
using Application.Features.ControlPanel.Workspace.Mapping;
using Application.Services.EventsLogger;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Module;

namespace Application.Features.ControlPanel.Workspace.Handlers
{
    public class CreateWorkspaceCommandHandler : BaseCommandHandler<CreateWorkspaceCommand, WorkspaceDto>
    {
        private readonly ModuleDbContext _moduleDbContext;
        private readonly WorkspaceMapper _workspaceMapper;

        public CreateWorkspaceCommandHandler(IMediator mediator,
            ILogger<BaseCommandHandler<CreateWorkspaceCommand, WorkspaceDto>> logger,
            IEventLogger eventLogger,
            IHttpContextAccessor httpContextAccessor,
            WorkspaceMapper workspaceMapper,
            ModuleDbContext moduleDbContext) : base(mediator, logger, eventLogger, httpContextAccessor)
        {
            _workspaceMapper = workspaceMapper;
            _moduleDbContext = moduleDbContext;
        }

        protected override async Task<ApiResponse<WorkspaceDto>> HandleCommand(CreateWorkspaceCommand request, CancellationToken cancellationToken)
        {
            var workspace = new Module.Domain.Schema.Workspace
            {
                Title = request.Title,
                Details = request.Details,
                ApplicationId = request.ApplicationId,
                Type = AppCommon.EnumShared.WorkspaceTypeEnum.Custom,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.UserId,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = request.UserId,
            };

            _moduleDbContext.Workspaces.Add(workspace);
            try
            {
                await _moduleDbContext.SaveChangesAsync(cancellationToken);
            }
            catch (Exception e)
            {

                throw;
            }
            

            return ApiResponse<WorkspaceDto>.Success(_workspaceMapper.MapToDto(workspace));
        }
    }
}
