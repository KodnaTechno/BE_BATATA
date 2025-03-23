using AppCommon.DTOs.Modules;
using Application.Common.Handlers;
using Application.Common.Models;
using Application.Features.ControlPanel.Properties.Commands;
using Application.Features.ControlPanel.Properties.Mapper;
using Application.Services.EventsLogger;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Module;

namespace Application.Features.ControlPanel.Properties.Handlers
{
    public class UpdatePropertyCommandHandler : BaseCommandHandler<UpdatePropertyCommand, PropertyDto>
    {
        private readonly ModuleDbContext _moduleDbContext;
        private readonly PropertyMapper _propertyMapper;

        public UpdatePropertyCommandHandler(
            IMediator mediator,
            ILogger<BaseCommandHandler<UpdatePropertyCommand, PropertyDto>> logger,
            IEventLogger eventLogger,
            IHttpContextAccessor httpContextAccessor,
            PropertyMapper propertyMapper,
            ModuleDbContext moduleDbContext)
            : base(mediator, logger, eventLogger, httpContextAccessor)
        {
            _propertyMapper = propertyMapper;
            _moduleDbContext = moduleDbContext;
        }

        protected override async Task<ApiResponse<PropertyDto>> HandleCommand(UpdatePropertyCommand request, CancellationToken cancellationToken)
        {
             var property = await _moduleDbContext.Properties
                    .FirstOrDefaultAsync(p => p.Id == request.PropertyId && !p.IsDeleted, cancellationToken);

            if (property == null)
            {
                return ApiResponse<PropertyDto>.Fail(ErrorCodes.NotFound, $"Property with ID {request.PropertyId} not found");
            }

            bool hasChanged = false;

            if (request.Title != null && !request.Title.Equals(property.Title))
            {
                property.Title = request.Title;
                hasChanged = true;
            }

            if (request.Description != null && !request.Description.Equals(property.Description))
            {
                property.Description = request.Description;
                hasChanged = true;
            }

            if (hasChanged)
            {
                property.UpdatedAt = DateTime.UtcNow;
                property.UpdatedBy = request.UserId;
                await _moduleDbContext.SaveChangesAsync(cancellationToken);
            }

            return ApiResponse<PropertyDto>.Success(_propertyMapper.MapToDto(property));
        }
    }
}
