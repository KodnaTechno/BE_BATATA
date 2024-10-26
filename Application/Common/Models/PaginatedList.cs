namespace Application.Common.Models
{
    public class PaginatedList<T>
    {
        public List<T> Items { get; set; }
        public int TotalCount { get; set; }
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public bool HasNext { get; set; }
        public bool HasPrevious { get; set; }
        public int NextOffset { get; set; }
        public bool HasMore { get; set; }
        public int PageSize { get; set; }
    }
}
