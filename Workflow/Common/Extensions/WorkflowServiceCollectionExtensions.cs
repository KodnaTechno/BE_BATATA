namespace AppWorkflow.Common.Extensions;

using AppWorkflow.Engine;
using AppWorkflow.Services.Interfaces;
using AppWorkflow.Services;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using AppWorkflow.Infrastructure.Services.Actions;
using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Infrastructure.Services.Engine;
using AppWorkflow.Core.Domain.Schema;

public class WorkflowOptions
    {
        public string ConnectionString { get; set; }
        public bool EnableDistributedLocking { get; set; } = true;
        public TimeSpan LockTimeout { get; set; } = TimeSpan.FromMinutes(5);
        public bool EnableCaching { get; set; } = true;
        public TimeSpan CacheDuration { get; set; } = TimeSpan.FromMinutes(30);
        public RetryPolicy DefaultRetryPolicy { get; set; } = new RetryPolicy
        {
            MaxRetries = 3,
            RetryInterval = TimeSpan.FromSeconds(5),
            ExponentialBackoff = true
        };
    }
    public static class WorkflowServiceCollectionExtensions
    {
        public static IServiceCollection AddWorkflowSystem(
            this IServiceCollection services,
            Action<WorkflowOptions> configure)
        {
            var options = new WorkflowOptions();
            configure?.Invoke(options);

            //services.AddScoped<IWorkflowEngine, WorkflowEngine>();
            //services.AddScoped<IWorkflowService, WorkflowService>();
            //services.AddScoped<IWorkflowVersionManager, WorkflowVersionManager>();
            //services.AddScoped<IWorkflowMigrationService, WorkflowMigrationService>();
            //services.AddScoped<IStepTransactionManager, StepTransactionManager>();
            //services.AddScoped<IDistributedLockManager, RedisDistributedLockManager>();
            //services.AddScoped<IWorkflowHealthMonitor, WorkflowHealthMonitor>();

            // Register all workflow actions
            services.Scan(scan => scan
                .FromAssemblyOf<IWorkflowAction>()
                .AddClasses(classes => classes.AssignableTo<IWorkflowAction>())
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }
    }