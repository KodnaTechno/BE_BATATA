using Localization.ServiceFactory;
using Microsoft.EntityFrameworkCore;
using Module;
using Module.ServiceFactory;
using AppIdentity;
using AppCommon;
using AppIdentity.Database;
using Serilog;
using Infrastructure.Database;
using Import.ServiceFactory;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.


// Configure Serilog
builder.Host.UseSerilog((hostingContext, services, loggerConfiguration) => loggerConfiguration
    .ReadFrom.Configuration(hostingContext.Configuration)
    .Enrich.FromLogContext());



builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCulture();
AppConfigration.Configure(builder.Configuration);
builder.Services.AddModule(builder.Configuration);
builder.Services.AddAppIdentity(builder.Configuration,x=>
 {
     x.UseSqlServer(AppConfigration.IdentityDbConnection, z =>
     {
         z.MigrationsAssembly("AppMigration.SqlServer");
         z.MigrationsHistoryTable("__AppIdentity_MigrationTable");
     });
 });
builder.Services.AddDbContext<ApplicationDbContext>(op => 
{
    op.UseSqlServer(AppConfigration.IdentityDbConnection, z =>
    {
        z.MigrationsAssembly("AppMigration.SqlServer");
        z.MigrationsHistoryTable("__App_MigrationTable");
    });
});

builder.Services.AddImportServices();

//builder.Services.AddDistributedMemoryCache(); // Adds a default in-memory implementation of IDistributedCache
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Set session timeout
    options.Cookie.HttpOnly = true; // Set HttpOnly for security
    options.Cookie.IsEssential = true; // Make the session cookie essential
});
//builder.Services.AddHttpContextAccessor(); // Add IHttpContextAccessor to access session in services

var app = builder.Build();

ApplyMigrations(app, typeof(ModuleDbContext), typeof(AppIdentityDbContext));

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

static void ApplyMigrations(WebApplication app, params Type[] dbContexts)
{
    using var scope = app.Services.CreateScope();
    foreach (var dbContextType in dbContexts)
    {
        var dbContext = (DbContext)scope.ServiceProvider.GetRequiredService(dbContextType);
        dbContext.Database.Migrate();
    }
}