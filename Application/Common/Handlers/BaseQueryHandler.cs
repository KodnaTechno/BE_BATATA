using Application.Common.Models;
using Application.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Security.Claims;

namespace Application.Common.Handlers
{
    public abstract class BaseQueryHandler<TQuery, TResult> : IRequestHandler<TQuery, ApiResponse<TResult>>
        where TQuery : BaseQuery<TResult>
    {
        protected readonly IMediator _mediator;
        protected readonly ILogger<BaseQueryHandler<TQuery, TResult>> _logger;
        protected readonly IHttpContextAccessor _httpContextAccessor;

        protected BaseQueryHandler(
            IMediator mediator,
            ILogger<BaseQueryHandler<TQuery, TResult>> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _mediator = mediator;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<ApiResponse<TResult>> Handle(TQuery request, CancellationToken cancellationToken)
        {
            try
            {
                SetCorrelationIdAndUserId(request);

                _logger.LogInformation("Handling query {QueryName} with CorrelationId: {CorrelationId}, UserId: {UserId}",
                    typeof(TQuery).Name, request.CorrelationId, request.UserId);

                var result = await HandleQuery(request, cancellationToken);

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error handling query {typeof(TQuery).Name}");
                return ApiResponse<TResult>.Fail("INTERNAL_ERROR", "An unexpected error occurred.");
            }
        }

        protected abstract Task<ApiResponse<TResult>> HandleQuery(TQuery request, CancellationToken cancellationToken);

        private void SetCorrelationIdAndUserId(BaseQuery<TResult> request)
        {
            if (_httpContextAccessor?.HttpContext != null)
            {

                if (_httpContextAccessor.HttpContext.Request.Headers.TryGetValue("X-Correlation-ID", out var correlationIds))
                {
                    request.CorrelationId = correlationIds.FirstOrDefault();
                }
                else
                {
                    request.CorrelationId = _httpContextAccessor.HttpContext.TraceIdentifier;
                }


                if (_httpContextAccessor.HttpContext.User.Identity.IsAuthenticated)
                {
                    request.UserId = _httpContextAccessor.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                }
            }
            else
            {
                request.CorrelationId = Guid.NewGuid().ToString();
                request.UserId = "System";
            }
        }
    }
}
