using Events.Modules.Workspace;
using Microsoft.Extensions.DependencyInjection;

namespace JobsProcessor
{
    public static class DI
    {
        public static void AddJobsProcessor(this IServiceCollection services)
        {
            services.AddTransient<IWorkspaceJob, WorkspaceJob>();
        }
    }
}
