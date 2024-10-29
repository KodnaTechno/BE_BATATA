using Google.Apis.Auth.OAuth2;
using Google.Apis.Drive.v3;
using Google.Apis.Services;

namespace FileStorge.Providers.Drive
{
    public class GoogleDriveProvider : IDocumentProvider
    {
        private readonly DriveService _driveService;
        private readonly IDocumentVerification _documentVerification;
        private readonly string _folderId;

        public GoogleDriveProvider(string credentialsJson, string folderId, string applicationName , IDocumentVerification documentVerification = null)
        {
            var credential = GoogleCredential.FromJson(credentialsJson)
                .CreateScoped(DriveService.ScopeConstants.DriveFile);

            _driveService = new DriveService(new BaseClientService.Initializer
            {
                HttpClientInitializer = credential,
                ApplicationName = applicationName
            });

            _folderId = folderId;
            _documentVerification = documentVerification ?? new DocumentVerification();
        }

        public FileModel GetMetaData(string id)
        {
            try
            {
                var file = _driveService.Files.Get(id).Execute();
                return new FileModel
                {
                    Id = file.Id,
                    Name = file.Name,
                    Size = file.Size ?? 0,
                    ContentType = file.MimeType,
                    Owner = file.Owners?[0]?.DisplayName,
                    CreatedAt = file.CreatedTime ?? DateTime.MinValue
                };
            }
            catch (Exception ex)
            {
                throw new Exception($"Error retrieving file metadata with id: {id}", ex);
            }
        }

        public async Task<Stream> DownloadFile(string id)
        {
            try
            {
                var memoryStream = new MemoryStream();
                await _driveService.Files.Get(id).DownloadAsync(memoryStream);
                memoryStream.Position = 0;
                return memoryStream;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error downloading file with id: {id}", ex);
            }
        }

        public FileModel UploadFile(byte[] stream, string filename, string contentType, string owner)
        {
            var verificationResult = _documentVerification.Verify(stream, filename);
            if (!verificationResult.IsValid)
                throw new InvalidDataException(verificationResult.Message);

            var fileMetadata = new Google.Apis.Drive.v3.Data.File()
            {
                Name = filename,
                Parents = [_folderId]
            };

            using var memoryStream = new MemoryStream(stream);
            var request = _driveService.Files.Create(fileMetadata, memoryStream, contentType);
            request.Fields = "id, name, size, mimeType, createdTime, owners";

            var file = request.Upload();
            if (file.Status != Google.Apis.Upload.UploadStatus.Completed)
                throw new Exception($"Upload failed: {file.Exception.Message}");

            var uploadedFile = request.ResponseBody;
            return new FileModel
            {
                Id = uploadedFile.Id,
                Name = uploadedFile.Name,
                Size = uploadedFile.Size ?? 0,
                ContentType = uploadedFile.MimeType,
                Owner = uploadedFile.Owners?[0]?.DisplayName ?? owner,
                CreatedAt = uploadedFile.CreatedTime ?? DateTime.UtcNow
            };
        }

        public bool DeleteFile(string id)
        {
            try
            {
                _driveService.Files.Delete(id).Execute();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Error deleting file with id: {id}", ex);
            }
        }
    }
}
