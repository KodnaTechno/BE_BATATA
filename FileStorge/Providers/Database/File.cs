using System.ComponentModel.DataAnnotations;

namespace FileStorge.Providers.Database
{
    public class File
    {
        [StringLength(128)]
        public Guid FileId { get; set; }

        [StringLength(128)]
        public string? Name { get; set; }
        public long Size { get; set; }

        public string? ContentType { get; set; }

        [StringLength(10)]
        public string? Extension { get; set; }

        public byte[]? Binary { get; set; }

        [StringLength(100)]
        public string? Owner { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}