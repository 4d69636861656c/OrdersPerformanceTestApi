namespace OrdersPerformanceTestApi.Services
{
    using OrdersPerformanceTestApi.Dtos;
    using OrdersPerformanceTestApi.Shared;

    public interface IRepositoryAsync
    {
        Task<T> GetByIdAsync<T>(int id) where T : class;

        Task<IEnumerable<T>> GetAllAsync<T>() where T : class;

        Task AddAsync<T>(T entity) where T : class;

        Task UpdateAsync<T>(T entity) where T : class;

        Task DeleteAsync<T>(int id) where T : class;

        Task<PaginatedResponse<ProductSalesReportDto>> GetMostSoldProductsAsync(DateTime startDate, DateTime endDate, PaginationFilter paginationFilter);

        Task<PaginatedResponse<TopBuyersDto>> GetTopBuyersAsync(PaginationFilter paginationFilter);

        Task SaveChangesAsync();
    }
}