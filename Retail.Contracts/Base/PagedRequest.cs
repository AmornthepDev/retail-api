namespace Retail.Contracts.GetAllBase
{
    public class PagedRequest
    {
        public const int DefaultPageNumber = 1;
        public const int DefaultPageSize = 10;

        public string? SortColumn { get; set; }
        public string? SortOrder { get; set; }
        public int? PageNumber { get; set; } = DefaultPageNumber;
        public int? PageSize { get; set; } = DefaultPageSize;
    }
}
