namespace AppWorkflow.Services;

using AppWorkflow.Core.Domain.Data;
using AppWorkflow.Core.DTOs;
using AppWorkflow.Infrastructure.Data.Context;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.Json;

public interface IAuditLogService
    {
        Task LogAsync(AuditLogEntry entry);
        Task<IEnumerable<AuditLog>> GetEntityHistoryAsync(string entityType, Guid entityId);
        Task<IEnumerable<AuditLog>> GetUserActivityAsync(string userId, DateTime from, DateTime to);
        Task<IEnumerable<AuditLog>> SearchAsync(AuditLogFilter filter);
    }

    public class AuditLogService : IAuditLogService
    {
        private readonly WorkflowDbContext _context;
        private readonly ILogger<AuditLogService> _logger;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuditLogService(
            WorkflowDbContext context,
            ILogger<AuditLogService> logger,
            IHttpContextAccessor httpContextAccessor)
        {
            _context = context;
            _logger = logger;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task LogAsync(AuditLogEntry entry)
        {
            var httpContext = _httpContextAccessor.HttpContext;

            var auditLog = new AuditLog
            {
                Id = Guid.NewGuid(),
                EntityType = entry.EntityType,
                EntityId = entry.EntityId,
                Action = entry.Action,
                UserId = entry.UserId,
                UserName = entry.UserName,
                Timestamp = DateTime.UtcNow,
                OldValues = entry.OldValues != null ? JsonSerializer.SerializeToDocument(entry.OldValues) : null,
                NewValues = entry.NewValues != null ? JsonSerializer.SerializeToDocument(entry.NewValues) : null,
                Metadata = entry.Metadata ?? new Dictionary<string, string>(),
                IpAddress = httpContext?.Connection?.RemoteIpAddress?.ToString(),
                UserAgent = httpContext?.Request?.Headers["User-Agent"].ToString(),
                Notes = entry.Notes,
                CreatedAt = DateTime.UtcNow
            };

            _context.AuditLogs.Add(auditLog);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetEntityHistoryAsync(string entityType, Guid entityId)
        {
            return await _context.AuditLogs
                .Where(a => a.EntityType == entityType && a.EntityId == entityId)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> GetUserActivityAsync(string userId, DateTime from, DateTime to)
        {
            return await _context.AuditLogs
                .Where(a => a.UserId == userId && a.Timestamp >= from && a.Timestamp <= to)
                .OrderByDescending(a => a.Timestamp)
                .ToListAsync();
        }

        public async Task<IEnumerable<AuditLog>> SearchAsync(AuditLogFilter filter)
        {
            var query = _context.AuditLogs.AsQueryable();

            if (!string.IsNullOrEmpty(filter.EntityType))
                query = query.Where(a => a.EntityType == filter.EntityType);

            if (filter.EntityId.HasValue)
                query = query.Where(a => a.EntityId == filter.EntityId);

            if (!string.IsNullOrEmpty(filter.UserId))
                query = query.Where(a => a.UserId == filter.UserId);

            if (filter.Actions?.Any() == true)
                query = query.Where(a => filter.Actions.Contains(a.Action));

            if (filter.From.HasValue)
                query = query.Where(a => a.Timestamp >= filter.From);

            if (filter.To.HasValue)
                query = query.Where(a => a.Timestamp <= filter.To);

            return await query
                .OrderByDescending(a => a.Timestamp)
                .Skip(filter.Skip)
                .Take(filter.Take)
                .ToListAsync();
        }
    }