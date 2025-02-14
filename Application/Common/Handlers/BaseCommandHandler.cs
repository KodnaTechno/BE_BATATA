using AppCommon;
using Application.Common.Models;
using Application.Interfaces;
using Application.Services.EventsLogger;
using Events;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Application.Common.Handlers
{
    public abstract class BaseCommandHandler<TCommand, TResult> : IRequestHandler<TCommand, ApiResponse<TResult>>
        where TCommand : BaseCommand<TResult>
    {
        protected readonly IMediator _mediator;
        protected readonly ILogger<BaseCommandHandler<TCommand, TResult>> _logger;
        protected readonly IEventLogger _eventLogger;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        protected BaseCommandHandler(
            IMediator mediator,
            ILogger<BaseCommandHandler<TCommand, TResult>> logger,
            IEventLogger eventLogger,
            IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _logger = logger;
            _eventLogger = eventLogger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<TResult>> Handle(TCommand request, CancellationToken cancellationToken)
        {
            try
            {
                SetCorrelationAndUserId(request);

                var result = await HandleCommand(request, cancellationToken);

                if (result.IsSuccess)
                {
                    var @event = request.GetEvent(result);
                    if (@event != null)
                    {
                        @event.CorrelationId = request.CorrelationId;
                        @event.UserId = request.UserId;

                        await _eventLogger.LogEventAsync(@event, cancellationToken);

                        try
                        {
                            await _mediator.Publish(@event, cancellationToken);
                            await _eventLogger.UpdateEventStatusAsync(@event.EventId, "Processed", null, cancellationToken);
                        }
                        catch (Exception ex)
                        {
                            await _eventLogger.UpdateEventStatusAsync(@event.EventId, "Failed", ex, cancellationToken);
                            _logger.LogError(ex, $"Error publishing event {@event.GetType().Name}");
                        }
                    }
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error handling command {typeof(TCommand).Name}");
                return ApiResponse<TResult>.Fail("INTERNAL_ERROR", "An unexpected error occurred.");
            }
        }

        protected abstract Task<ApiResponse<TResult>> HandleCommand(TCommand request, CancellationToken cancellationToken);

        private void SetCorrelationAndUserId(TCommand request)
        {
            if (_httpContextAccessor?.HttpContext != null)
            {
                var httpContext = _httpContextAccessor.HttpContext;


                if (httpContext.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationIds))
                {
                    request.CorrelationId = correlationIds.FirstOrDefault();
                }
                else
                {
                    request.CorrelationId = httpContext.TraceIdentifier;
                }


                if (httpContext.User.Identity.IsAuthenticated)
                {
                    request.UserId = Guid.Parse(httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value);
                }
                else
                {
                    request.UserId = SystemUsers.AnonymousUserId;
                }
            }
            else
            {
                request.CorrelationId = Guid.NewGuid().ToString();
                request.UserId = SystemUsers.SystemUserId;
            }
        }
    }
}
