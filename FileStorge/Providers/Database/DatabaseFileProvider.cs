namespace FileStorge.Providers.Database
{
    public class DatabaseFileProvider : IDocumentProvider, IDisposable
    {
        private readonly FileDbContext _dbContext;
        private readonly IDocumentVerification _documentVerification;
        public DatabaseFileProvider(FileDbContext dbContext)
        {
            _dbContext = dbContext;
            _documentVerification = new DocumentVerification();
        }

        public async Task<Stream> DownloadFile(string id)
        {
            var file = await _dbContext.Files.FindAsync(Guid.Parse(id)) ?? throw new FileNotFoundException("invalid file");
            if (file.Binary == null) return new MemoryStream();

            return new MemoryStream(file.Binary);
        }

        public FileModel UploadFile(byte[] stream, string filename, string contentType, string owner)
        {
            if (string.IsNullOrEmpty(filename))
                throw new ArgumentNullException("filename");

            var verificationResult = _documentVerification.Verify(stream, filename);

            if (!verificationResult.IsValid)
                throw new InvalidDataException(verificationResult.Message);

            var file = new File
            {
                Name = filename,
                ContentType = contentType,
                Binary = stream,
                Size = stream.Length,
                Extension = Path.GetExtension(filename),
                Owner = owner,
                CreatedAt = DateTime.UtcNow
            };

            _dbContext.Files.Add(file);
            _dbContext.SaveChanges();
            _dbContext.Dispose();

            return file.Meta();
        }

        public FileModel GetMetaData(string id)
        {
            var file = _dbContext.Files.Find(Guid.Parse(id));

            return file == null ? throw new FileNotFoundException("invalid file") : file.Meta();
        }

        public void Dispose()
        {
            _dbContext?.Dispose();

            GC.SuppressFinalize(this);
        }

        public bool DeleteFile(string id)
        {
            var file = _dbContext.Files.Find(Guid.Parse(id)) ?? throw new FileNotFoundException("invalid file");
            _dbContext.Files.Remove(file);
            return _dbContext.SaveChanges() > 0;
        }
    }
}