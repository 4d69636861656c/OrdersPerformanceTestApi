namespace OrdersPerformanceTestApi.Mappings
{
    using AutoMapper;

    using OrdersPerformanceTestApi.Dtos;
    using OrdersPerformanceTestApi.Models;

    public class ProductMappingProfile : Profile
    {
        public ProductMappingProfile()
        {
            this.CreateMap<Product, ProductDto>();

            this.CreateMap<ProductDto, Product>();
        }
    }
}