namespace FileStorge.Providers.FileSystem
{
    public class FileSystemProvider : IDocumentProvider
    {
        private readonly string _basePath;
        private readonly IDocumentVerification _documentVerification;

        public FileSystemProvider(string basePath, IDocumentVerification documentVerification = null)
        {
            _basePath = basePath ?? throw new ArgumentNullException(nameof(basePath));
            _documentVerification = documentVerification ?? new DocumentVerification();

            if (!Directory.Exists(_basePath))
            {
                Directory.CreateDirectory(_basePath);
            }
        }

        public FileModel GetMetaData(string id)
        {
            var filePath = Path.Combine(_basePath, id);
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found with id: {id}");

            var fileInfo = new FileInfo(filePath);
            return new FileModel
            {
                Id = id,
                Name = fileInfo.Name,
                Size = fileInfo.Length,
                ContentType = GetContentType(fileInfo.Extension),
                Owner = Environment.UserName,
                CreatedAt = fileInfo.CreationTime
            };
        }

        public async Task<Stream> DownloadFile(string id)
        {
            var filePath = Path.Combine(_basePath, id);
            if (!File.Exists(filePath))
                throw new FileNotFoundException($"File not found with id: {id}");

            var memoryStream = new MemoryStream();
            using (var fileStream = File.OpenRead(filePath))
            {
                await fileStream.CopyToAsync(memoryStream);
            }
            memoryStream.Position = 0;
            return memoryStream;
        }

        public FileModel UploadFile(byte[] stream, string filename, string contentType, string owner)
        {
            var verificationResult = _documentVerification.Verify(stream, filename);
            if (!verificationResult.IsValid)
                throw new InvalidDataException(verificationResult.Message);

            var id = Guid.NewGuid().ToString();
            var filePath = Path.Combine(_basePath, id);

            File.WriteAllBytes(filePath, stream);

            return new FileModel
            {
                Id = id,
                Name = filename,
                Size = stream.Length,
                ContentType = contentType,
                Owner = owner ?? Environment.UserName,
                CreatedAt = DateTime.UtcNow
            };
        }

        public bool DeleteFile(string id)
        {
            var filePath = Path.Combine(_basePath, id);
            if (!File.Exists(filePath))
                return false;

            File.Delete(filePath);
            return true;
        }

        private string GetContentType(string extension)
        {
            return extension.ToLower() switch
            {
                // Images
                ".jpeg" => "image/jpeg",
                ".jpg" => "image/jpeg",
                ".png" => "image/png",
                ".psd" => "image/vnd.adobe.photoshop",

                // Documents
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                ".doc" => "application/msword",
                ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".ppt" => "application/vnd.ms-powerpoint",
                ".pptx" => "application/vnd.openxmlformats-officedocument.presentationml.presentation",
                ".mpp" => "application/vnd.ms-project",

                // Email formats
                ".msg" => "application/vnd.ms-outlook",
                ".eml" => "message/rfc822",

                // Audio/Video
                ".mp3" => "audio/mpeg",
                ".mp4" => "video/mp4",

                // CAD and 3D
                ".dwg" => "application/acad",  // AutoCAD
                ".skp" => "application/vnd.sketchup.skp",  // SketchUp

                // Backup
                ".bak" => "application/octet-stream",

                // Default binary stream for unknown types
                _ => "application/octet-stream"
            };
        }
    }
}
