using Application.Features.ControlPanel.Workspace.Mapping;
using Application.Services.EventsLogger;
using JobsProcessor;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Application.Features.ControlPanel.AppActions.Mapper;
using Application.Features.ControlPanel.Modules.Mapping;
using Module.Service.DefaultSetupService;
using Module.Service;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            });

            services.AddScoped<IEventLogger, DbEventLogger>();
            services.AddSingleton<WorkspaceMapper>();
            services.AddSingleton<ModuleMapper>();
            services.AddSingleton<AppActionMapper>();
            services.AddJobsProcessor();

            return services;
        }
    }
}
