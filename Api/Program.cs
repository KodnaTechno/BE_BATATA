using Localization.ServiceFactory;
using Module;
using AppIdentity;
using AppIdentity.Database;
using Serilog;
using Infrastructure.Database;
using Api.Extensions;
using Winton.Extensions.Configuration.Consul;


var builder = WebApplication.CreateBuilder(args);

// Configure Serilog
builder.Host.UseSerilog((hostingContext, services, loggerConfiguration) =>
    loggerConfiguration
        .ReadFrom.Configuration(hostingContext.Configuration)
        .Enrich.FromLogContext());


// Integrate Consul
try
{
    builder.Configuration.AddConsul("config/app", options =>
    {
        options.ConsulConfigurationOptions = cco =>
        {
            cco.Address = new Uri(builder.Configuration["Consul:Host"]);
        };
        options.ReloadOnChange = true;
        options.Optional = true;

    });

}
catch 
{

}


// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Register custom services and configurations
builder.Services.AddCustomServices(builder.Configuration);

var app = builder.Build();

// Apply migrations
app.ApplyMigrations(typeof(ModuleDbContext), typeof(AppIdentityDbContext), typeof(ApplicationDbContext));

app.SeedDatabaseAsync().Wait();


// Initialize and configure timezone settings
app.ConfigureTimezoneSettings();


// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCulture();
app.UseAppIdentity();
app.MapControllers();

app.UseSession(); // Enable session handling
app.Run();