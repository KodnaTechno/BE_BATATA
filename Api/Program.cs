using Localization.ServiceFactory;
using Module;
using AppIdentity;
using AppIdentity.Database;
using Infrastructure.Database;
using Api.Extensions;
using Api.Middlewares;
using Hangfire;
using AppCommon.GlobalHelpers;
using FileStorge.Providers.Database;


var builder = WebApplication.CreateBuilder(args);


builder.AddSerilogConfiguration()
       .AddConsulConfiguration();


builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCustomServices(builder.Configuration);

var app = builder.Build();

ServiceActivator.Configure(app.Services);


app.ApplyMigrations(typeof(ModuleDbContext), typeof(AppIdentityDbContext), typeof(ApplicationDbContext), typeof(FileDbContext));

app.SeedDatabaseAsync().Wait();

app.ConfigureTimezoneSettings();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCultureMiddleware();
app.UseCulture();
app.UseAppIdentity();
app.MapControllers();

app.UseMiddleware<RequestResponseLoggingMiddleware>();


app.UseSession();

app.UseHangfireDashboard("/hangfire");
HangfireConfiguration.ActivateHangfireJobs();



app.Run();