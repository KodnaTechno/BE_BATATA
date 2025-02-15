using AppCommon.DTOs.Modules;
using Application.Common.Handlers;
using Application.Common.Models;
using Application.Features.ControlPanel.Modules.Mapping;
using Application.Features.ControlPanel.Modules.Queries;
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
    public class GetModuleQueryHandler : BaseQueryHandler<GetModuleQuery, ModuleDto>
    {
        private readonly ModuleDbContext _moduleDbContext;
        private readonly ModuleMapper _moduleMapper;
        private readonly IStringLocalizer<object> _localization;
        public GetModuleQueryHandler(
            IMediator mediator,
            ILogger<BaseQueryHandler<GetModuleQuery, ModuleDto>> logger,
            IHttpContextAccessor httpContextAccessor,
            ModuleDbContext moduleDbContext,
            ModuleMapper moduleMapper,
            IEntityCacheService entityCacheService,
            IStringLocalizer<object> localization)
            : base(mediator, logger, httpContextAccessor)
        {
            _moduleDbContext = moduleDbContext;
            _moduleMapper = moduleMapper;
            _localization = localization;
        }

        protected override async Task<ApiResponse<ModuleDto>> HandleQuery(GetModuleQuery request, CancellationToken cancellationToken)
        {

            var module = await _moduleDbContext.Modules.FindAsync([request.ModuleId], cancellationToken);
            if (module == null)
                return ApiResponse<ModuleDto>.Fail(ErrorCodes.NotFound, _localization[ErrorCodes.NotFound]);

            return ApiResponse<ModuleDto>.Success(_moduleMapper.MapToDto(module));
        }
    }
}

