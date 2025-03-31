using AppCommon.DTOs;
using AppCommon.DTOs.Modules;
using Application.Common.Handlers;
using Application.Features.ControlPanel.Properties.Mapper;
using Application.Features.ControlPanel.Properties.Queries;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Module;

namespace Application.Features.ControlPanel.Properties.Handlers
{
    public class GetPropertyQueryHandler : BaseQueryHandler<GetPropertyQuery, PropertyDto>
    {
        private readonly ModuleDbContext _moduleDbContext;
        private readonly PropertyMapper _propertyMapper;
        private readonly IStringLocalizer<object> _localization;

        public GetPropertyQueryHandler(
           IMediator mediator,
           ILogger<BaseQueryHandler<GetPropertyQuery, PropertyDto>> logger,
           IHttpContextAccessor httpContextAccessor,
           ModuleDbContext moduleDbContext,
           PropertyMapper propertyMapper,
           IStringLocalizer<object> localization)
           : base(mediator, logger, httpContextAccessor)
        {
            _moduleDbContext = moduleDbContext;
            _propertyMapper = propertyMapper;
            _localization = localization;
        }

        protected override async Task<ApiResponse<PropertyDto>> HandleQuery(GetPropertyQuery request, CancellationToken cancellationToken)
        {
            var property = await _moduleDbContext.Properties
                .FirstOrDefaultAsync(p => p.Id == request.PropertyId && !p.IsDeleted, cancellationToken);

            if (property == null)
                return ApiResponse<PropertyDto>.Fail(ErrorCodes.NotFound, _localization[ErrorCodes.NotFound]);

            return ApiResponse<PropertyDto>.Success(_propertyMapper.MapToDto(property));
        }
    }
}
