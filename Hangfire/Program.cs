using Module.ServiceFactory;
using Hangfire.Api;
using Hangfire;
using AppCommon.GlobalHelpers;
using Hangfire.Shared;
using JobsProcessor;
using Application.Services.DefaultSetupService;
using AppWorkflow.Services.Interfaces;
using AppWorkflow.Services;
using AppWorkflow.Infrastructure.Repositories.IRepository;
using AppWorkflow.Infrastructure.Repositories;
using AppCommon;
using AppWorkflow.Infrastructure.Data.Context;
using Microsoft.EntityFrameworkCore;
using AppWorkflow.Core.Interfaces.Services;
using AppWorkflow.Services.HealthCheck;
using AppWorkflow.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddModule(builder.Configuration);
builder.Services.AddDistributedMemoryCache();
AppConfigration.Configure(builder.Configuration);
builder.Services.AddDbContext<WorkflowDbContext>(options =>
{
    options.UseSqlServer(AppConfigration.AppDbConnection, sqlOptions =>
    {
        sqlOptions.MigrationsAssembly("AppMigration.SqlServer");
        sqlOptions.MigrationsHistoryTable("__AppWorkFlow_MigrationTable");
    });
});
builder.Services.AddHangfire(builder.Configuration);


builder.Services.AddJobsProcessor();
WorkflowServiceRegistration.AddWorkflowInfrastructure(builder.Services,builder.Configuration);
builder.Services.AddScoped<IDefaultWorkspaceSetupService, DefaultWorkspaceSetupService>();
builder.Services.AddScoped<IWorkflowManagementService,  WorkflowManagementService>();
builder.Services.AddScoped<IDefaultModuleSetupService, DefaultModuleSetupService>();
builder.Services.AddHttpContextAccessor();
var app = builder.Build();

ServiceActivator.Configure(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCustomHangfireDashboard(builder.Configuration);

JobContext.ActivateHangfireJobs();


var summaries = new[]
{
    "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
};

app.MapGet("/weatherforecast", () =>
{
    var forecast = Enumerable.Range(1, 5).Select(index =>
        new WeatherForecast
        (
            DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
            Random.Shared.Next(-20, 55),
            summaries[Random.Shared.Next(summaries.Length)]
        ))
        .ToArray();
    return forecast;
})
.WithName("GetWeatherForecast")
.WithOpenApi();

app.Run();

internal record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}



