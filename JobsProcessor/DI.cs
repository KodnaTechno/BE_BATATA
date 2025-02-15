using Events.Modules.Modules;
using Events.Modules.Workspace;
using JobsProcessor.Modules;
using JobsProcessor.Workspace;
using Microsoft.Extensions.DependencyInjection;

namespace JobsProcessor
{
    public static class DI
    {
        public static void AddJobsProcessor(this IServiceCollection services)
        {
            services.AddTransient<IWorkspaceJob, WorkspaceJob>();
            services.AddTransient<IModuleJob, ModuleJob>();
        }
    }
}
