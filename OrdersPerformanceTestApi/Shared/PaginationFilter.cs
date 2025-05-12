namespace OrdersPerformanceTestApi.Shared
{
    public class PaginationFilter
    {
        public int PageNumber { get; set; }

        public int PageSize { get; set; }

        public List<ColumnOrder> Sorting { get; set; }

        public PaginationFilter()
        {
            this.PageNumber = 1;
            this.PageSize = 10;
            this.Sorting = new List<ColumnOrder>();
        }
    }

    public class ColumnOrder
    {
        public string Id { get; set; }

        public bool Desc { get; set; }
    }
}