﻿using Application.Features.ControlPanel.Workspace.Mapping;
using Application.Services.EventsLogger;
using JobsProcessor;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

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


            services.AddJobsProcessor();

            return services;
        }
    }
}
