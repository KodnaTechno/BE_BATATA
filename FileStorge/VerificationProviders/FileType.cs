namespace FileStorge.VerificationProviders
{
    public abstract class FileType
    {
        protected string Description { get; set; }
        protected string Name { get; set; }
        protected int SignatureOffset { get; set; } = 0;

        private List<string> Extensions { get; }
            = new List<string>();

        private List<byte[]> Signatures { get; }
            = new List<byte[]>();

        public int SignatureLength => Signatures.Max(m => m.Length);

        protected FileType AddSignatures(params byte[][] bytes)
        {
            Signatures.AddRange(bytes);
            return this;
        }

        protected FileType AddExtensions(params string[] extensions)
        {
            Extensions.AddRange(extensions);
            return this;
        }

        public FileTypeVerifyResult Verify(Stream stream)
        {
            stream.Position = SignatureOffset;
            var reader = new BinaryReader(stream);
            var headerBytes = reader.ReadBytes(SignatureLength);

            return new FileTypeVerifyResult
            {
                Name = Name,
                Description = Description,
                IsVerified = Signatures.Any(signature =>
                    headerBytes.Take(signature.Length)
                        .SequenceEqual(signature)
                )
            };
        }

        public List<string> PossibleExtensions
        {
            get
            {
                return Extensions.Select(t => t.ToLower()).ToList();
            }
        }
    }
}
