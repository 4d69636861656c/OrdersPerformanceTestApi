namespace OrdersPerformanceTestApi.Controllers
{
    using AutoMapper;

    using Microsoft.AspNetCore.Mvc;

    using OrdersPerformanceTestApi.Dtos;
    using OrdersPerformanceTestApi.Models;
    using OrdersPerformanceTestApi.Services;

    // Our REST API controller for Orders.
    // This class adheres to the Single Responsibility Principle (SRP) of SOLID because:
    // 1. The OrderController is responsible only for handling HTTP requests and responses.
    // 2. It delegates database operations to RepositoryAsync and uses AutoMapper for mapping.
    // 3. It does not contain any business logic or data access code, which is handled by the repository.
    // 4. It is easy to test and maintain because it only has a single responsibility (Order endpoints).
    [ApiController]
    [Route("api/[controller]")]
    public class OrderController : ControllerBase
    {
        private readonly IRepositoryAsync _repository;

        private readonly IMapper _mapper;

        // We are using Dependency Injection to inject the repository and mapper into the controller.
        // This allows us to easily replace the repository implementation if needed.
        // We are registering the repository and mapper serivices in the Program.cs file.
        public OrderController(IRepositoryAsync repository, IMapper mapper)
        {
            this._repository = repository;
            this._mapper = mapper;
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var order = await this._repository.GetByIdAsync<Order>(id);

            if (order == null)
            {
                return this.NotFound();
            }

            var orderDto = this._mapper.Map<OrderDto>(order);

            return this.Ok(orderDto);
        }

        [HttpPost]
        public async Task<IActionResult> Create(OrderDto orderDto)
        {
            var order = this._mapper.Map<Order>(orderDto);

            await this._repository.AddAsync(order);
            await this._repository.SaveChangesAsync();

            return this.CreatedAtAction(nameof(GetById), new { id = order.Id }, orderDto);
        }
    }
}