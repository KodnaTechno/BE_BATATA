namespace FileStorge.VerificationProviders
{
    public class HeaderCheckVerificationService
    {
        private readonly IEnumerable<FileType> _types;

        public HeaderCheckVerificationService()
        {
            _types = new List<FileType>
            {
                new Jpeg(),
                new Png(),
                new Mp3(),
                new PDF(),
                new Txt(),
                new Docx(),
                new Doc(),
                new Mp4(),
                new Ppt(),
                new Pptx(),
                new Xls(),
                new Xlsx(),
                new Mpp(),
                new Msg(),
                new Psd(),
                new Bak(),
                new Dwg(),
                new Skp(),
                new Eml(),
            }
                .OrderByDescending(x => x.SignatureLength)
                .ToList();
        }

        private static FileTypeVerifyResult Unknown = new FileTypeVerifyResult
        {
            Name = "Unknown",
            Description = "Unknown File Type",
            IsVerified = false
        };

        public FileTypeVerifyResult What(Stream stream, string fileName)
        {
            FileTypeVerifyResult result = null;

            foreach (var fileType in _types)
            {
                result = fileType.Verify(stream);
                if (result.IsVerified &&
                    fileType.PossibleExtensions.Contains(Path.GetExtension(fileName).ToLower().Split('.').Last()))
                    break;
            }

            return result?.IsVerified == true
                   ? result
                   : Unknown;
        }
    }
}