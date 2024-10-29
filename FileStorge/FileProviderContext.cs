using FileStorge.Providers.Database;
using FileStorge.Providers.Drive;
using FileStorge.Providers.FileSystem;
using FileStorge.Providers.Mongodb;
using FileStorge.Providers.OneDrive;
using FileStorge.Providers.SharePoint;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FileStorge;

public static class FileProviderContext
{
    public static void AddSPDocumentProvider(this IServiceCollection services, string siteUrl, string documentLibrary, string username, string password)
    {
        services.AddScoped<IDocumentProvider, SharePointFileProvider>(provider =>
            new SharePointFileProvider(siteUrl,documentLibrary,username, password));            
    }
    
    public static void AddOneDriveDocumentProvider(this IServiceCollection services, string clientId, string clientSecret, string tenantId, string driveId, string oneDrivePath)
    {
        services.AddScoped<IDocumentProvider, OneDriveFileProvider>(_ => new OneDriveFileProvider(clientId, clientSecret, tenantId, driveId, oneDrivePath));            
    }
    
    public static void AddDataBaseDocumentProvider(this IServiceCollection services, Action<DbContextOptionsBuilder> dbContextConfigurator)
    {
        services.AddDbContext<FileDbContext>(dbContextConfigurator);
        services.AddScoped<IDocumentProvider, DatabaseFileProvider>();
    }


    public static void AddMongoDocumentProvider(this IServiceCollection services, string connectionString, string database)
    {
        services.AddScoped<IDocumentProvider, GridFSFileProvider>(provider =>
            new GridFSFileProvider(connectionString, database));
    }


    public static void AddFileSystemDocumentProvider(this IServiceCollection services, string basePath)
    {
        services.AddScoped<IDocumentProvider, FileSystemProvider>(provider =>
            new FileSystemProvider(basePath));
    }

    public static void AddGoogleDriveDocumentProvider(this IServiceCollection services, string credentialsJson, string folderId, string applicationName)
    {
        services.AddScoped<IDocumentProvider, GoogleDriveProvider>(provider =>
            new GoogleDriveProvider(credentialsJson, folderId, applicationName));
    }

}