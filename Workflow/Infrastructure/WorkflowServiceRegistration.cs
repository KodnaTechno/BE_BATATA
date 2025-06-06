using AppWorkflow.Common.Exceptions;
using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Engine;
using AppWorkflow.Infrastructure.Data.Context;
using AppWorkflow.Infrastructure.Services.Actions;
using AppWorkflow.Infrastructure.Services;
using AppWorkflow.Infrastructure.Triggers;
using AppWorkflow.Services.Interfaces;
using AppWorkflow.Services;
using AppWorkflow.Services.Monitoring;
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
using AppWorkflow.Expressions;
using MediatR;
using Events;
using AppWorkflow.Common;
using Microsoft.EntityFrameworkCore;

namespace AppWorkflow.Infrastructure;

public static class WorkflowServiceRegistration
{
    public static void AddWorkflowInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration, List<Type> actions)
    {
        // Register Options
        services.Configure<WorkflowOptions>(
            configuration.GetSection("WorkflowOptions"));

        //Database Context
        // Core Services -Scoped
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
        services.AddScoped<IWorkflowManagementService, WorkflowManagementService>();

        //// Infrastructure Services - Singleton
        services.AddSingleton<IDistributedLockManager, RedisDistributedLockManager>();
        services.AddSingleton<ITelemetryTracker, TelemetryTracker>();
        //services.AddSingleton<IWorkflowHealthMonitor, WorkflowHealthMonitor>();

        //// Missing registrations
        services.AddScoped<IWorkflowEventHandler, WorkflowEventHandler>();

        services.AddScoped<IStepExecutorFactory, StepExecutorFactory>();
        services.AddScoped<IApprovalService, ApprovalService>();
        services.AddScoped<IApprovalTargetResolver, ApprovalTargetResolver>();
        services.AddScoped<IStepTransactionManager, StepTransactionManager>();
        //services.AddScoped<IExpressionLanguageProvider, ExpressionLanguageProvider>();
        //services.AddScoped<IExpressionParser>();
        //services.AddScoped<IExpressionValidator>();
        
        // Register missing services for monitoring, recovery and checkpoints
        services.AddScoped<IWorkflowCheckpointRepository, WorkflowCheckpointRepository>();
        services.AddScoped<IWorkflowRecoveryService, WorkflowRecoveryService>();
        services.AddScoped<IWorkflowMonitoringService, WorkflowMonitoringService>();        services.AddScoped<AppWorkflow.Triggers.TriggerManager>();
        services.AddScoped<AppWorkflow.Infrastructure.Triggers.TriggerManager>();

        // Register all workflow actions
        RegisterWorkflowActions(services, actions);

        // Register workflow trigger handlers
        RegisterWorkflowTriggers(services);

        // Register EventTriggerListener as a MediatR notification handler for IEvent
        services.AddScoped<INotificationHandler<Events.IEvent>, AppWorkflow.Infrastructure.Triggers.EventTriggerListener>();

        // Register ScheduledTriggerBackgroundService as a hosted service
        services.AddHostedService<AppWorkflow.Infrastructure.Triggers.ScheduledTriggerBackgroundService>();
    }

    private static void RegisterWorkflowActions(IServiceCollection services, List<Type> actions)
    {
        // Only register actions passed in from the host (no hardcoded registrations)
        foreach (var actionType in actions)
        {
            services.AddScoped(typeof(IWorkflowAction), actionType);
        }
    }

    private static void RegisterWorkflowTriggers(IServiceCollection services)
    {
        // Register workflow trigger handlers
        services.AddScoped<IWorkflowTriggerHandler, AppWorkflow.Infrastructure.Triggers.EventTriggerHandler>();
        services.AddScoped<IWorkflowTriggerHandler, AppWorkflow.Infrastructure.Triggers.ScheduledTriggerHandler>();
        services.AddScoped<IWorkflowTriggerHandler, AppWorkflow.Infrastructure.Triggers.ManualTriggerHandler>();
        services.AddScoped<IWorkflowTriggerHandler, AppWorkflow.Infrastructure.Triggers.ApiTriggerHandler>();

        // Additional handler registrations as needed
    }

    // Add UseWorkflowSystem extension method for IApplicationBuilder
    public static IApplicationBuilder UseWorkflowSystem(this IApplicationBuilder app)
    {
        // Initialize any middleware or startup tasks required by the workflow system
        app.UseMiddleware<WorkflowExceptionMiddleware>();
        
        // Initialize workflow engine, triggers, etc.
        var serviceProvider = app.ApplicationServices;
        var logger = serviceProvider.GetRequiredService<ILogger<WorkflowEngine>>();
        
        logger.LogInformation("Workflow system initialized");
        
        return app;
    }
}

// Define the middleware for workflow exceptions
public class WorkflowExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<WorkflowExceptionMiddleware> _logger;

    public WorkflowExceptionMiddleware(RequestDelegate next, ILogger<WorkflowExceptionMiddleware> logger)
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
            _logger.LogError(ex, "Workflow exception occurred");
            
            context.Response.StatusCode = StatusCodes.Status500InternalServerError;
            context.Response.ContentType = "application/json";
            
            var errorResponse = new
            {
                Message = "A workflow error occurred",
                Error = ex.Message,
              
            };
            
            await context.Response.WriteAsJsonAsync(errorResponse);
        }
    }
}