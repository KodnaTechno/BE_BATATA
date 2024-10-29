using Microsoft.Graph;
using Microsoft.Identity.Client;
using System.Net.Http.Headers;

namespace FileStorge.Providers.OneDrive;
public class OneDriveFileProvider : IDocumentProvider
{
    private readonly GraphServiceClient _graphClient;
    private readonly string _driveId;
    private readonly IDocumentVerification _documentVerification;

    public OneDriveFileProvider(string clientId, string clientSecret, string tenantId, string driveId, IDocumentVerification documentVerification)
    {
        _driveId = driveId;
        _documentVerification = documentVerification ?? new DocumentVerification();
        var scopes = new[] { "https://graph.microsoft.com/.default" };

        var confidentialClientApplication = ConfidentialClientApplicationBuilder
            .Create(clientId)
            .WithTenantId(tenantId)
            .WithClientSecret(clientSecret)
            .Build();

        var authResult = confidentialClientApplication.AcquireTokenForClient(scopes)
            .ExecuteAsync()
            .Result;
            

        _graphClient = new GraphServiceClient(
            new DelegateAuthenticationProvider((requestMessage) =>
            {
                requestMessage.Headers.Authorization =
                    new AuthenticationHeaderValue("Bearer", authResult.AccessToken);

                return Task.CompletedTask;
            }));
        _graphClient.HttpProvider.OverallTimeout = TimeSpan.FromMinutes(30);
    }

    public OneDriveFileProvider(string clientId, string clientSecret, string tenantId, string driveId) :this(clientId, clientSecret, tenantId, driveId, null)
    {
        
        
    }

    public FileModel GetMetaData(string id)
    {
        var item = _graphClient.Drives[_driveId].Items[id].Request().GetAsync().Result;
        return new FileModel
        {
            Id = item.Id,
            Guid = new Guid(item.Id),
            Name = item.Name,
            Size = item.Size ?? 0,
            ContentType = item.File.MimeType,
            Owner = item.CreatedBy.User?.DisplayName,
            CreatedAt = item.CreatedDateTime.HasValue ? item.CreatedDateTime.Value.DateTime : DateTime.MinValue
        };
    }

    public async Task<Stream> DownloadFile(string id)
    {
        try
        {
            if (string.IsNullOrEmpty(id))
                throw new Exception("File not found.");

            var stream = new MemoryStream();
            var fileStream = await _graphClient.Drives[_driveId].Root
                .ItemWithPath(id)
                .Content
                .Request()
                .GetAsync();

            fileStream.CopyTo(stream);
            stream.Position = 0;

            return stream;
        }
        catch
        {
            throw new Exception("File not found.");
        }
    }


    public FileModel UploadFile(byte[] stream, string filename, string contentType, string owner)
    {
        var verificationResult = _documentVerification.Verify(stream, filename);

        if (!verificationResult.IsValid)
            throw new InvalidDataException(verificationResult.Message);


        string uploadPath =Guid.NewGuid().ToString();

        using var memoryStream = new MemoryStream(stream);
        var uploadedItem = _graphClient.Drives[_driveId].Root.ItemWithPath(uploadPath).Content
            .Request()
            .PutAsync<DriveItem>(memoryStream).Result;

        return new FileModel
        {
            Id = uploadedItem.Id,
            Name = filename,
            Size = uploadedItem.Size ?? 0,
            ContentType = uploadedItem.File.MimeType,
            Owner = uploadedItem.CreatedBy.User?.DisplayName,
            CreatedAt = uploadedItem.CreatedDateTime.HasValue ? uploadedItem.CreatedDateTime.Value.DateTime : DateTime.MinValue
        };
    }

    public bool DeleteFile(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentNullException(nameof(id));
        }

        try
        {
            _graphClient.Drives[_driveId].Root
                .ItemWithPath(id)
                .Request()
                .DeleteAsync()
                .GetAwaiter()
                .GetResult();

            return true;
        }
        catch (Exception ex)
        {
            throw new Exception($"Error deleting document with id: {id}", ex);
        }
    }
}

