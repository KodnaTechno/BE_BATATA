using Localization.ServiceFactory;
using Microsoft.EntityFrameworkCore;
using Module;
using Module.ServiceFactory;
using AppIdentity;
using AppCommon;
using AppIdentity.Database;
var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCulture();

builder.Services.AddModule(builder.Configuration);
builder.Services.AddAppIdentity(builder.Configuration,x=>x.UseSqlServer(AppConfigration.IdentityDbConnection));
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