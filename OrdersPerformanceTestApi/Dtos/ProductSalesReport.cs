namespace OrdersPerformanceTestApi.Dtos
{
    public class ProductSalesReportDto
    {
        public string ProductName { get; set; }

        public int TotalUnitsSold { get; set; }

        public decimal TotalPrice { get; set; }
    }
}