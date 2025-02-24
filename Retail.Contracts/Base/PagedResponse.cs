namespace Retail.Contracts.Base
{
    public class PagedResponse<T>
    {
        public required IEnumerable<T> Data { get; set; } = Enumerable.Empty<T>();
        public required int PageNumber { get; set; }
        public required int PageSize { get; set; }
        public required int Total { get; set; }
        public bool HasNextPage => Total > (PageNumber * PageSize);
    }
}
