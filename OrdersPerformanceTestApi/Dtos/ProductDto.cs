namespace OrdersPerformanceTestApi.Dtos
{
    using Swashbuckle.AspNetCore.Annotations;
    using System.ComponentModel.DataAnnotations;

    public class ProductDto
    {
        [SwaggerSchema("The name of the product.", Nullable = false)]
        [Required]
        public string Name { get; set; }

        [SwaggerSchema("The price of the product.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than zero.")]
        public decimal Price { get; set; }
    }
}