using AppCommon.DTOs;
using AppCommon.DTOs.Modules;
using Application.Common.Handlers;
using Application.Common.Models;
using Application.Features.ControlPanel.Properties.Commands;
using Application.Features.ControlPanel.Properties.Mapper;
using Application.Services.EventsLogger;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Module;
using Module.Service;

namespace Application.Features.ControlPanel.Properties.Handlers
{
    public class CreatePropertyCommandHandler : BaseCommandHandler<CreatePropertyCommand, PropertyDto>
    {
        private readonly ModuleDbContext _moduleDbContext;
        private readonly PropertyMapper _propertyMapper;
        private readonly PropertyKeyGenerator _keyGenerator;


        public CreatePropertyCommandHandler(
            IMediator mediator,
            ILogger<BaseCommandHandler<CreatePropertyCommand, PropertyDto>> logger,
            IEventLogger eventLogger,
            IHttpContextAccessor httpContextAccessor,
            PropertyMapper propertyMapper,
            ModuleDbContext moduleDbContext,
            PropertyKeyGenerator keyGenerator)
            : base(mediator, logger, eventLogger, httpContextAccessor)
        {
            _propertyMapper = propertyMapper;
            _moduleDbContext = moduleDbContext;
            _keyGenerator = keyGenerator;
        }

        protected override async Task<ApiResponse<PropertyDto>> HandleCommand(CreatePropertyCommand request, CancellationToken cancellationToken)
        {
            int targetCount = 0;
            if (request.ModuleId.HasValue) targetCount++;
            if (request.WorkspaceId.HasValue) targetCount++;
            if (request.WorkspaceModuleId.HasValue) targetCount++;
            if (request.ApplicationId.HasValue) targetCount++;

            if (targetCount != 1)
                return ApiResponse<PropertyDto>.Fail(ErrorCodes.InvalidOperation, "Exactly one target entity (Module, Workspace, WorkspaceModule, or Application) must be specified");


            if (request.Title == null || (string.IsNullOrWhiteSpace(request.Title.En) && string.IsNullOrWhiteSpace(request.Title.Ar)))
                return ApiResponse<PropertyDto>.Fail(ErrorCodes.ValidationError, "Property title is required");


            var (key, normalizedKey) = await _keyGenerator.GeneratePropertyKey(
                request.Title,
                request.ModuleId,
                request.WorkspaceId,
                request.WorkspaceModuleId,
                request.ApplicationId);

            var dataType = PropertyTypeMapper.GetDataTypeForViewType(request.ViewType);

            string configuration = request.Configuration;
            if (string.IsNullOrEmpty(configuration) && PropertyTypeMapper.RequiresConfiguration(request.ViewType))
                configuration = PropertyTypeMapper.GetDefaultConfiguration(request.ViewType);

            var property = new Module.Domain.Schema.Properties.Property
            {
                Title = request.Title,
                Key = key,
                NormalizedKey = normalizedKey,
                Description = request.Description,
                ViewType = request.ViewType,
                DataType = dataType,
                Configuration = configuration,
                IsSystem = request.IsSystem,
                IsInternal = request.IsInternal,
                DefaultValue = request.DefaultValue,
                IsCalculated = request.IsCalculated,
                IsEncrypted = request.IsEncrypted,
                IsTranslatable = request.IsTranslatable,
                Order = request.Order,
                ModuleId = request.ModuleId,
                WorkspaceId = request.WorkspaceId,
                WorkspaceModuleId = request.WorkspaceModuleId,
                ApplicationId = request.ApplicationId,
                SystemPropertyPath = request.SystemPropertyPath,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = request.UserId,
                UpdatedAt = DateTime.UtcNow,
                UpdatedBy = request.UserId
            };

            _moduleDbContext.Properties.Add(property);
            await _moduleDbContext.SaveChangesAsync(cancellationToken);

            return ApiResponse<PropertyDto>.Success(_propertyMapper.MapToDto(property));
        }
    }
}
