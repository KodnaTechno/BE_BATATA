using Application.Features.ControlPanel.Workspace.Mapping;
using Application.Services.EventsLogger;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;
using Application.Features.ControlPanel.AppActions.Mapper;
using Application.Features.ControlPanel.Modules.Mapping;
using Application.Services.DefaultSetupService;
using Application.Features.ControlPanel.Properties.Mapper;
using Application.Services.Rendering;

namespace Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddMediatR(cfg => {
                cfg.RegisterServicesFromAssembly(Assembly.GetExecutingAssembly());

            });
            services.AddScoped<IDefaultWorkspaceSetupService, DefaultWorkspaceSetupService>();
            services.AddScoped<IDefaultModuleSetupService, DefaultModuleSetupService>();
            services.AddScoped<IEventLogger, DbEventLogger>();
            services.AddSingleton<WorkspaceMapper>();
            services.AddSingleton<ModuleMapper>();
            services.AddSingleton<PropertyMapper>();
            services.AddSingleton<AppActionMapper>();

            services.AddSingleton<TextValueRenderer>();
            services.AddSingleton<NumericValueRenderer>();
            services.AddSingleton<BooleanValueRenderer>();
            services.AddSingleton<DateValueRenderer>();

            services.AddScoped<UserValueRenderer>();
            services.AddScoped<LookupValueRenderer>();
            services.AddScoped<AttachmentValueRenderer>();
            services.AddScoped<DynamicValueRenderer>();
            services.AddScoped<ConnectionForeignKeyRenderer>();

            services.AddScoped<IPropertyValueRendererService, PropertyValueRendererService>();

            return services;
        }
    }
}
