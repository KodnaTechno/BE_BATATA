using FileStorge.Providers.Database;
using FileStorge.Providers.Drive;
using FileStorge.Providers.FileSystem;
using FileStorge.Providers.Mongodb;
using FileStorge.Providers.OneDrive;
using FileStorge.Providers.SharePoint;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Pomelo.EntityFrameworkCore.MySql.Infrastructure;

namespace FileStorge;

public static class FileProviderContext
{

    public static void AddFileProvider(this IServiceCollection services, IConfiguration configuration)
    {
        string connectionString = configuration.GetConnectionString("DefaultConnection");
        string databaseProvider = configuration["DatabaseProvider"];
        string fileProvider = configuration["FileProvider"];

        switch (fileProvider.ToLower())
        {
            case "sharepointfileprovider":
                services.AddSPFileProvider(
                    configuration["SharePoint:SiteUrl"],
                    configuration["SharePoint:DocumentLibrary"],
                    configuration["SharePoint:Username"],
                    configuration["SharePoint:Password"]);
                break;

            case "databasefileprovider" when databaseProvider.Equals("sqlserver", StringComparison.OrdinalIgnoreCase):
                string sqlServerConnectionString = connectionString;
                services.AddDataBaseFileProvider(options =>
                    options.UseSqlServer(sqlServerConnectionString, sqlOptions =>
                    {
                        sqlOptions.MigrationsAssembly("AppMigration.SqlServer");
                        sqlOptions.MigrationsHistoryTable("__File_MigrationTable");
                    }));
                break;

            case "databasefileprovider" when databaseProvider.Equals("mysql", StringComparison.OrdinalIgnoreCase):
                string mySqlConnectionString = connectionString;
                services.AddDataBaseFileProvider(options =>
                    options.UseMySql(mySqlConnectionString,
                        ServerVersion.AutoDetect(mySqlConnectionString),
                        x =>
                        {
                            x.MigrationsAssembly("AppMigration.MySqlServer");
                            x.MigrationsHistoryTable("__File_MigrationTable");
                            x.SchemaBehavior(MySqlSchemaBehavior.Ignore);
                        }));
                break;

            case "databasefileprovider" when databaseProvider.Equals("postgresql", StringComparison.OrdinalIgnoreCase):
                string postgresConnectionString = connectionString;
                services.AddDataBaseFileProvider(options =>
                    options.UseNpgsql(postgresConnectionString, x =>
                    {
                        x.MigrationsAssembly("AppMigration.PostgreSQL");
                        x.MigrationsHistoryTable("__File_MigrationTable");
                    }));
                break;

            case "mongodocumentprovider":
                services.AddMongoFileProvider(
                    configuration["MongoDB:ConnectionString"],
                    configuration["MongoDB:Database"]);
                break;

            case "onedrivedocumentprovider":
                services.AddOneDriveFileProvider(
                    configuration["OneDrive:ClientId"],
                    configuration["OneDrive:ClientSecret"],
                    configuration["OneDrive:TenantId"],
                    configuration["OneDrive:DriveId"]);
                break;

            case "filesystemprovider":
                services.AddFileSystemFileProvider(
                    configuration["FileSystem:BasePath"]);
                break;

            case "googledriveprovider":
                services.AddGoogleDriveFileProvider(
                    configuration["GoogleDrive:CredentialsJson"],
                    configuration["GoogleDrive:FolderId"],
                    configuration["GoogleDrive:ApplicationName"]);
                break;
        }
    }

    private static void AddSPFileProvider(this IServiceCollection services, string siteUrl, string documentLibrary, string username, string password)
    {
        services.AddScoped<IDocumentProvider, SharePointFileProvider>(provider =>
            new SharePointFileProvider(siteUrl,documentLibrary,username, password));            
    }

    private static void AddOneDriveFileProvider(this IServiceCollection services, string clientId, string clientSecret, string tenantId, string driveId)
    {
        services.AddScoped<IDocumentProvider, OneDriveFileProvider>(_ => new OneDriveFileProvider(clientId, clientSecret, tenantId, driveId));            
    }

    private static void AddDataBaseFileProvider(this IServiceCollection services, Action<DbContextOptionsBuilder> dbContextConfigurator)
    {
        services.AddDbContext<FileDbContext>(dbContextConfigurator);
        services.AddScoped<IDocumentProvider, DatabaseFileProvider>();
    }


    private static void AddMongoFileProvider(this IServiceCollection services, string connectionString, string database)
    {
        services.AddScoped<IDocumentProvider, GridFSFileProvider>(provider =>
            new GridFSFileProvider(connectionString, database));
    }


    private static void AddFileSystemFileProvider(this IServiceCollection services, string basePath)
    {
        services.AddScoped<IDocumentProvider, FileSystemProvider>(provider =>
            new FileSystemProvider(basePath));
    }

    private static void AddGoogleDriveFileProvider(this IServiceCollection services, string credentialsJson, string folderId, string applicationName)
    {
        services.AddScoped<IDocumentProvider, GoogleDriveProvider>(provider =>
            new GoogleDriveProvider(credentialsJson, folderId, applicationName));
    }

}