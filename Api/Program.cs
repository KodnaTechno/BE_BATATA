using Localization.ServiceFactory;
using Module;
using AppIdentity;
using AppIdentity.Database;
using Infrastructure.Database;
using Api.Extensions;
using Api.Middlewares;
using AppCommon.GlobalHelpers;
using FileStorge.Providers.Database;
using Hangfire.Shared;
using AppWorkflow.Infrastructure.Data.Context;
using AppWorkflow.Infrastructure;
using AppCommon;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Microsoft.EntityFrameworkCore;
using AppWorkflow.Services.Interfaces;
using AppWorkflow.Services;
using Microsoft.OpenApi.Models;


var builder = WebApplication.CreateBuilder(args);


builder.AddSerilogConfiguration();
       //.AddConsulConfiguration();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Batata", Version = "v1" });
});
builder.Services.AddCustomServices(builder.Configuration);

builder.Services.AddHangfire(builder.Configuration, false);
builder.Services.AddScoped<IWorkflowManagementService, WorkflowManagementService>();
var app = builder.Build();

ServiceActivator.Configure(app.Services);


app.ApplyMigrations(typeof(ModuleDbContext),
    typeof(AppIdentityDbContext),
    typeof(ApplicationDbContext),
    typeof(FileDbContext),
    typeof(WorkflowDbContext));

app.SeedDatabaseAsync().Wait();

app.ConfigureTimezoneSettings();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Batata v1");
    });
}

app.UseHttpsRedirection();
app.UseCultureMiddleware();
app.UseCulture();
app.UseAppIdentity();

app.UseWorkflowSystem();

app.MapControllers();

app.UseMiddleware<RequestResponseLoggingMiddleware>();


app.UseSession();

app.Run();