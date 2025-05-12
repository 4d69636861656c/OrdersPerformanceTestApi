namespace OrdersPerformanceTestApi.Shared
{
    public class PaginatedResponse<T>
    {
        public PaginatedResponse(List<T> data, int count, int page, int pageSize)
        {
            this.Data = data;
            this.CurrentPage = page;
            this.PageSize = pageSize;
            this.TotalPages = (int)Math.Ceiling(count / (double)pageSize);
            this.TotalCount = count;
        }

        public List<T> Data { get; set; }

        public int CurrentPage { get; set; }

        public int TotalPages { get; set; }

        public int TotalCount { get; set; }

        public int PageSize { get; set; }

        public bool HasPreviousPage => this.CurrentPage > 1;

        public bool HasNextPage => this.CurrentPage < this.TotalPages;
    }
}