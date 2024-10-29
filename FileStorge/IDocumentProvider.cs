using FileStorge.VerificationProviders;

namespace FileStorge
{
    public class Document
    {
        public string? Identifier { get; set; }
        public string? Name { get; set; }
        public string? ContentType { get; set; }
        public string? Extension { get; set; }
    }

    public interface IDocumentProvider
    {
        public FileModel GetMetaData(string id);
        public Task<Stream> DownloadFile(string id);
        public FileModel UploadFile(byte[] stream, string filename, string contentType, string owner);        
        public bool DeleteFile(string id);
    }

    public interface IDocumentVerification
    {
        public (bool IsValid, string Message) Verify(byte[] stream, string filename);
    }

    public class DocumentVerification : IDocumentVerification
    {
        private readonly HeaderCheckVerificationService _verificationService;

        public DocumentVerification()
        {
            _verificationService = new HeaderCheckVerificationService();
        }

        public (bool IsValid, string Message) Verify(byte[] stream, string filename)
        {
             var isValid = _verificationService.What(new MemoryStream(stream), filename);
             return (isValid.IsVerified, isValid.Description);
        }
    }
}