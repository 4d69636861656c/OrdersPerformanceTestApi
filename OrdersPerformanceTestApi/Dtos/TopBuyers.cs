namespace OrdersPerformanceTestApi.Dtos
{
    public class TopBuyersDto
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public decimal TotalOrderValue { get; set; }
    }
}