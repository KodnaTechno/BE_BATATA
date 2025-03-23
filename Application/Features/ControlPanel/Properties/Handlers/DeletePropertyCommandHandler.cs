using Application.Common.Handlers;
using Application.Common.Models;
using Application.Features.ControlPanel.Properties.Commands;
using Application.Services.EventsLogger;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging;
using Module;

namespace Application.Features.ControlPanel.Properties.Handlers
{
    public class DeletePropertyCommandHandler : BaseCommandHandler<DeletePropertyCommand, bool>
    {
        private readonly ModuleDbContext _moduleDbContext;
        private readonly IStringLocalizer<object> _localization;

        public DeletePropertyCommandHandler(
            IMediator mediator,
            ILogger<BaseCommandHandler<DeletePropertyCommand, bool>> logger,
            IEventLogger eventLogger,
            IHttpContextAccessor httpContextAccessor,
            ModuleDbContext moduleDbContext,
            IStringLocalizer<object> localization)
            : base(mediator, logger, eventLogger, httpContextAccessor)
        {
            _moduleDbContext = moduleDbContext;
            _localization = localization;
        }

        protected override async Task<ApiResponse<bool>> HandleCommand(DeletePropertyCommand request, CancellationToken cancellationToken)
        {
            var property = await _moduleDbContext.Properties
                .FirstOrDefaultAsync(p => p.Id == request.PropertyId && !p.IsDeleted, cancellationToken);

            if (property == null)
                return ApiResponse<bool>.Fail(ErrorCodes.NotFound, _localization[ErrorCodes.NotFound]);

            if (property.IsSystem)
                return ApiResponse<bool>.Fail(ErrorCodes.InvalidOperation, _localization[ErrorMessages.SystemPropertyDeleteMessage]);

            property.IsDeleted = true;
            property.DeletedAt = DateTime.UtcNow;
            property.DeletedBy = request.UserId;

            await _moduleDbContext.SaveChangesAsync(cancellationToken);

            return ApiResponse<bool>.Success(true);
        }
    }
}
