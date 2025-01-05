using AppWorkflow.Common.Exceptions;
using AppWorkflow.Common.Extensions;
using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Engine;
using AppWorkflow.Infrastructure.Data.Context;
using AppWorkflow.Infrastructure.Services.Actions;
using AppWorkflow.Infrastructure.Services.Engine;
using AppWorkflow.Infrastructure.Services;
using AppWorkflow.Infrastructure.Triggers;
using AppWorkflow.Services.Interfaces;
using AppWorkflow.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using System.Net.Http;
using AppWorkflow.Infrastructure.Repositories.IRepository;
using AppWorkflow.Infrastructure.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using AppWorkflow.Services.HealthCheck;
using Module;
namespace AppWorkflow.Infrastructure;

public static class WorkflowServiceRegistration
{
    public static void AddWorkflowInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        // Register Options
        services.Configure<WorkflowOptions>(
            configuration.GetSection("WorkflowOptions"));

        // Database Context

        ConfigureDB(services, configuration);

        // Core Services - Scoped
        services.AddScoped<IWorkflowEngine, WorkflowEngine>();
        services.AddScoped<IWorkflowStateManager, WorkflowStateManager>();
        services.AddScoped<IWorkflowValidator, WorkflowValidator>();
        services.AddScoped<IWorkflowVersionManager, WorkflowVersionManager>();
        services.AddScoped<IWorkflowMigrationService, WorkflowMigrationService>();
        services.AddScoped<IStepExecutor, TransactionalStepExecutor>();
        services.AddScoped<IActionResolver, ActionResolver>();
        services.AddScoped<IAuditLogService, AuditLogService>();
        services.AddScoped<IWorkflowRepository, WorkflowRepository>();
        services.AddScoped<IWorkflowDataRepository, WorkflowDataRepository>();
        services.AddScoped<IExpressionEvaluator, ExpressionEvaluator>();

        // Infrastructure Services - Singleton
        services.AddSingleton<IDistributedLockManager, RedisDistributedLockManager>();
        services.AddSingleton<ITelemetryTracker, TelemetryTracker>();
       // services.AddSingleton<IWorkflowHealthMonitor, WorkflowHealthMonitor>();

        // Register all workflow actions
        RegisterWorkflowActions(services);

        // Register workflow trigger handlers
        RegisterWorkflowTriggers(services);
    }

    private static void RegisterWorkflowActions(IServiceCollection services)
    {
        // Register all implementations of IWorkflowAction
        var actionTypes = typeof(IWorkflowAction).Assembly
            .GetTypes()
            .Where(t => !t.IsAbstract && !t.IsInterface
                && typeof(IWorkflowAction).IsAssignableFrom(t));

        foreach (var actionType in actionTypes)
        {
            services.AddScoped(actionType);
            services.AddScoped(
                serviceProvider => (IWorkflowAction)ActivatorUtilities
                    .CreateInstance(serviceProvider, actionType));
        }
    }

    private static void RegisterWorkflowTriggers(IServiceCollection services)
    {
        services.AddScoped<IWorkflowTrigger, ScheduledTrigger>();
        // Add other trigger implementations
    }

    private static void ConfigureDB(IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection");
        string databaseProvider = configuration["DatabaseProvider"];

        switch (databaseProvider)
        {
            case "SqlServer":
                ConfigureSqlServerDbContext(services, connectionString);
                break;
            case "PostgreSQL":
                //ConfigurePostgreSQLDbContext(services, connectionString);
                break;
            default:
                throw new Exception("Invalid database provider");
        }
    }

    private static void ConfigureSqlServerDbContext(IServiceCollection services, string connectionString)
    {
        services.AddDbContext<WorkflowDbContext>(options =>
        {
            options.UseSqlServer(connectionString, sqlOptions =>
            {
                sqlOptions.MigrationsAssembly("AppMigration.SqlServer");
                sqlOptions.MigrationsHistoryTable("__Workflow_MigrationTable");
            });
        });
    }

    //public static void ConfigurePostgreSQLDbContext(IServiceCollection services, string connectionString)
    //{
    //    services.AddDbContext<WorkflowDbContext>(options =>
    //    {
    //        options.UseNpgsql(connectionString);
    //    });
    //}
}

// Extension method for middleware registration
public static class WorkflowMiddlewareExtensions
{
    public static IApplicationBuilder UseWorkflowSystem(
        this IApplicationBuilder app)
    {
        // Add workflow middleware
        app.UseMiddleware<WorkflowExceptionMiddleware>();
        app.UseMiddleware<WorkflowAuthorizationMiddleware>();
        app.UseMiddleware<WorkflowTelemetryMiddleware>();

        return app;
    }
}

// Required middleware implementations
public class WorkflowExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<WorkflowExceptionMiddleware> _logger;

    public WorkflowExceptionMiddleware(
        RequestDelegate next,
        ILogger<WorkflowExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (WorkflowException ex)
        {
            _logger.LogError(ex, "Workflow error occurred");
            context.Response.StatusCode = StatusCodes.Status400BadRequest;
            await context.Response.WriteAsJsonAsync(new
            {
                error = ex.Message,
                workflowId = ex.WorkflowId,
                instanceId = ex.InstanceId
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled error occurred");
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            await context.Response.WriteAsJsonAsync(new
            {
                error = "An unexpected error occurred"
            });
        }
    }
}

public class WorkflowAuthorizationMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<WorkflowAuthorizationMiddleware> _logger;

    public WorkflowAuthorizationMiddleware(
    RequestDelegate next,
        ILogger<WorkflowAuthorizationMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Add authorization logic here
        await _next(context);
    }
}

public class WorkflowTelemetryMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ITelemetryTracker _telemetry;

    public WorkflowTelemetryMiddleware(
        RequestDelegate next,
        ITelemetryTracker telemetry)
    {
        _next = next;
        _telemetry = telemetry;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var sw = Stopwatch.StartNew();
        try
        {
            await _next(context);
        }
        finally
        {
            sw.Stop();
            // Track telemetry
        }
    }
}