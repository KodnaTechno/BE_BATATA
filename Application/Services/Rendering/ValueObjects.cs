namespace Application.Services.Rendering
{
    public interface IRenderedValue { }

    public class TextValue : IRenderedValue
    {
        public AppCommon.DTOs.TranslatableValue Translatable { get; set; }
        public string DisplayValue { get; set; }
    }
    public class NumericValue : IRenderedValue
    {
        public decimal Value { get; set; }
        public string Formatted { get; set; }
    }
    public class BooleanValue : IRenderedValue
    {
        public bool Value { get; set; }
    }
    public class DateValue : IRenderedValue
    {
        public DateTime? RawValue { get; set; }
        public string DisplayValue { get; set; }
    }
    public class UserValue : IRenderedValue
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
    }
    public class MultiUserValue : IRenderedValue
    {
        public List<UserValue> Users { get; set; } = new();
    }
    public class LookupValue : IRenderedValue
    {
        public Guid Id { get; set; }
        public string DisplayName { get; set; }
    }

    public class MultiLookupValue : IRenderedValue
    {
        public List<LookupValue> Items { get; set; } = new();
    }

    public class AttachmentValue : IRenderedValue
    {
        public Guid AttachmentId { get; set; }
        public string FileName { get; set; }
        public long FileSize { get; set; }
        public string DownloadUrl { get; set; }
    }
    public class MultiAttachmentValue : IRenderedValue
    {
        public List<AttachmentValue> Attachments { get; set; } = new();
    }

    public class DynamicValue : IRenderedValue
    {
        public object Data { get; set; }
    }

    public class ExternalDisplayValue : IRenderedValue
    {
    }

    // Mapped property list
    public class MappedPropertyListValue : IRenderedValue
    {
    }

    public class ConnectionValue : IRenderedValue
    {
    }
    public class ForeignKeyValue : IRenderedValue
    {
    }
    public class ModuleReferenceValue : IRenderedValue
    {
    }
}
