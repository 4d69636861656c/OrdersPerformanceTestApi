namespace OrdersPerformanceTestApi.Services
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Caching.Memory;

    using OrdersPerformanceTestApi.Dtos;
    using OrdersPerformanceTestApi.Models;
    using OrdersPerformanceTestApi.Shared;

    public class RepositoryAsync : IRepositoryAsync
    {
        private readonly ApplicationDbContext _dbContext;

        private readonly IMemoryCache _memoryCache;

        public RepositoryAsync(ApplicationDbContext dbContext, IMemoryCache memoryCache)
        {
            this._dbContext = dbContext;
            this._memoryCache = memoryCache;
        }

        public async Task<T> GetByIdAsync<T>(int id) where T : class
        {
            // If the entity is Order, I am including the OrderProducts navigation property,
            // so we can get the products associated with the order in the call.
            if (typeof(T) == typeof(Order))
            {
                return await this._dbContext.Set<Order>()
                           .Include(o => o.OrderProducts)
                           .FirstOrDefaultAsync(o => o.Id == id) as T;
            }

            return await this._dbContext.Set<T>().FindAsync(id);
        }

        public async Task<IEnumerable<T>> GetAllAsync<T>() where T : class
        {
            return await this._dbContext.Set<T>().ToListAsync();
        }

        public async Task AddAsync<T>(T entity) where T : class
        {
            await this._dbContext.Set<T>().AddAsync(entity);
            await this.SaveChangesAsync();
        }

        public async Task UpdateAsync<T>(T entity) where T : class
        {
            this._dbContext.Set<T>().Update(entity);
            await this.SaveChangesAsync();
        }

        public async Task DeleteAsync<T>(int id) where T : class
        {
            var entity = await GetByIdAsync<T>(id);
            if (entity != null)
            {
                this._dbContext.Set<T>().Remove(entity);
                await this.SaveChangesAsync();
            }
        }

        public async Task<PaginatedResponse<ProductSalesReportDto>> GetMostSoldProductsAsync(DateTime startDate, DateTime endDate, PaginationFilter paginationFilter)
        {
            var query = from orderProduct in this._dbContext.OrderProducts
                        join product in this._dbContext.Products on orderProduct.ProductId equals product.Id
                        join order in this._dbContext.Orders on orderProduct.OrderId equals order.Id
                        where order.DateAdded >= startDate && order.DateAdded <= endDate
                        group orderProduct by new { product.Name } into grouped
                        select new ProductSalesReportDto
                        {
                            ProductName = grouped.Key.Name,
                            TotalUnitsSold = grouped.Sum(op => op.Quantity),
                            TotalPrice = grouped.Sum(op => op.Price * op.Quantity)
                        };

            query = query.OrderByDescending(bsp => bsp.TotalPrice).ThenByDescending(bsp => bsp.TotalUnitsSold);

            int totalCount = await query.CountAsync();

            List<ProductSalesReportDto> paginatedData = await query
                                                            .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                                                            .Take(paginationFilter.PageSize)
                                                            .ToListAsync();

            return new PaginatedResponse<ProductSalesReportDto>(paginatedData, totalCount, paginationFilter.PageNumber, paginationFilter.PageSize);
        }

        // To try to optimize the performance of the GetTopBuyersAsync method,
        // I am using pagination, caching, disabling EF tracking, and filtering the data in the database.
        public async Task<PaginatedResponse<TopBuyersDto>> GetTopBuyersAsync(PaginationFilter paginationFilter)
        {
            var cacheKey = $"TopBuyers_Page{paginationFilter.PageNumber}_Size{paginationFilter.PageSize}";
            if (!this._memoryCache.TryGetValue(cacheKey, out PaginatedResponse<TopBuyersDto> cachedResponse))
            {
                var sixMonthsAgo = DateTime.UtcNow.AddMonths(-6);

                var query = this._dbContext.Users
                    .AsNoTracking()
                    .Where(u => u.Orders.Any(o => o.DateAdded >= sixMonthsAgo))
                    .Select(u => new TopBuyersDto
                    {
                        UserId = u.Id,
                        UserName = u.Name,
                        TotalOrderValue = u.Orders
                                             .Where(o => o.DateAdded >= sixMonthsAgo)
                                             .SelectMany(o => o.OrderProducts)
                                             .Sum(op => op.Quantity * op.Price)
                    })
                    .Where(x => x.TotalOrderValue > 1000.00m)
                    .OrderByDescending(x => x.TotalOrderValue);

                var totalCount = await query.CountAsync();

                var paginatedData = await query
                                        .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                                        .Take(paginationFilter.PageSize)
                                        .ToListAsync();

                cachedResponse = new PaginatedResponse<TopBuyersDto>(paginatedData, totalCount, paginationFilter.PageNumber, paginationFilter.PageSize);
                this._memoryCache.Set(cacheKey, cachedResponse, TimeSpan.FromMinutes(5));
            }

            return cachedResponse;
        }

        public async Task SaveChangesAsync()
        {
            await this._dbContext.SaveChangesAsync();
        }
    }
}