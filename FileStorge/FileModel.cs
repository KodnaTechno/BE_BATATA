namespace FileStorge
{
    public class FileModel
    {
        public string? Id { get; set; }
        public Guid Guid { get; set; }
        public string? Name { get; set; }
        public long Size { get; set; }
        public string? ContentType { get; set; }
        public string? Owner { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Extension
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Name))
                    throw new InvalidOperationException("Invalid file name. File name must has a value");

                return Path.GetExtension(Name).Split('.').Last().ToLower();
            }
        }
    }
}