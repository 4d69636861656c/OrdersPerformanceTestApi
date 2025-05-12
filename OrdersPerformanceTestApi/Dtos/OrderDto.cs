namespace OrdersPerformanceTestApi.Dtos
{
    public class OrderDto
    {
        public int UserId { get; set; }

        public DateTime DateAdded { get; set; }

        public List<OrderProductDto> OrderProducts { get; set; }
    }
}