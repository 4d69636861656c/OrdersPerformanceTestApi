namespace OrdersPerformanceTestApi.Controllers
{
    using AutoMapper;

    using Microsoft.AspNetCore.Mvc;

    using OrdersPerformanceTestApi.Dtos;
    using OrdersPerformanceTestApi.Models;
    using OrdersPerformanceTestApi.Services;
    using OrdersPerformanceTestApi.Shared;

    // Our REST API controller for Products.
    [ApiController]
    [Route("api/[controller]")]
    public class ProductController : ControllerBase
    {
        private readonly IRepositoryAsync _repository;

        private readonly IMapper _mapper;

        public ProductController(IRepositoryAsync repository, IMapper mapper)
        {
            this._repository = repository;
            this._mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            var paginationFilter = new PaginationFilter { PageNumber = pageNumber, PageSize = pageSize };

            var products = await this._repository.GetAllAsync<Product>();
            var paginatedProducts = products
                .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize)
                .ToList();

            var totalCount = products.Count();
            var paginatedResponse = new PaginatedResponse<ProductDto>(
                this._mapper.Map<List<ProductDto>>(paginatedProducts),
                totalCount,
                paginationFilter.PageNumber,
                paginationFilter.PageSize
            );

            return this.Ok(paginatedResponse);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var product = await this._repository.GetByIdAsync<Product>(id);
            if (product == null)
            {
                return this.NotFound();
            }

            var productDto = _mapper.Map<ProductDto>(product);
            return this.Ok(productDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProductDto productDto)
        {
            var product = this._mapper.Map<Product>(productDto);

            await this._repository.AddAsync(product);
            await this._repository.SaveChangesAsync();

            var createdProductDto = _mapper.Map<ProductDto>(product);
            return this.CreatedAtAction(nameof(GetById), new { id = product.Id }, createdProductDto);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, ProductDto productDto)
        {
            var product = await this._repository.GetByIdAsync<Product>(id);
            if (product == null)
            {
                return this.NotFound();
            }

            _mapper.Map(productDto, product);

            await this._repository.UpdateAsync(product);
            await this._repository.SaveChangesAsync();

            return this.NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await this._repository.GetByIdAsync<Product>(id);
            if (product == null)
            {
                return this.NotFound();
            }

            await this._repository.DeleteAsync<Product>(id);
            await this._repository.SaveChangesAsync();

            return this.NoContent();
        }

        // By default, this will display bestsellers for the last 30 days.
        // Can be customized via the parameters to display for any time frame.
        [HttpGet("bestsellers")]
        public async Task<IActionResult> GetMostSoldProducts(
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10)
        {
            startDate ??= DateTime.UtcNow.AddDays(-30);
            endDate ??= DateTime.UtcNow;

            var paginationFilter = new PaginationFilter { PageNumber = pageNumber, PageSize = pageSize };

            PaginatedResponse<ProductSalesReportDto> report =
                await this._repository.GetMostSoldProductsAsync(startDate.Value, endDate.Value, paginationFilter);

            return this.Ok(report);
        }
    }
}