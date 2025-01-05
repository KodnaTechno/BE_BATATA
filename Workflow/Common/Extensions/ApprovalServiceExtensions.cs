using AppWorkflow.Infrastructure.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppWorkflow.Common.Extensions
{
    public static class ApprovalServiceExtensions
    {
        public static IServiceCollection AddWorkflowApproval(this IServiceCollection services)
        {
            // Register services
            services.AddScoped<IApprovalService, ApprovalService>();
            //services.AddScoped<IApprovalTargetResolver, ModuleBasedTargetResolver>();
            //services.AddScoped<IApprovalTargetResolver, DynamicExpressionTargetResolver>();

            // Register background service for approval timeout handling
            services.AddHostedService<ApprovalTimeoutService>();

            return services;
        }
    }
}
