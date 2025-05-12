namespace OrdersPerformanceTestApi.Mappings
{
    using AutoMapper;

    using OrdersPerformanceTestApi.Dtos;
    using OrdersPerformanceTestApi.Models;

    public class OrderMappingProfile : Profile
    {
        public OrderMappingProfile()
        {
            this.CreateMap<Order, OrderDto>().ForMember(
                orderDto => orderDto.OrderProducts,
                opt => opt.MapFrom(src => src.OrderProducts));

            this.CreateMap<OrderDto, Order>();

            this.CreateMap<OrderProduct, OrderProductDto>();

            this.CreateMap<OrderProductDto, OrderProduct>();
        }
    }
}