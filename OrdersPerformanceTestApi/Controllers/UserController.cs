namespace OrdersPerformanceTestApi.Controllers
{
    using AutoMapper;

    using Microsoft.AspNetCore.Mvc;
    using System.Diagnostics;

    using OrdersPerformanceTestApi.Services;
    using OrdersPerformanceTestApi.Shared;

    // Our REST API controller for Users.
    [ApiController]
    [Route("api/[controller]")]
    public class UserController : ControllerBase
    {
        private readonly IRepositoryAsync _repository;

        private readonly IMapper _mapper;

        public UserController(IRepositoryAsync repository, IMapper mapper)
        {
            this._repository = repository;
            this._mapper = mapper;
        }

        [HttpGet("top-users")]
        public async Task<IActionResult> GetTopUsers([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            // Used a stopwatch to measure the execution time of the method.
            // I am also sending the execution time in the response body.
            var stopwatch = Stopwatch.StartNew();

            PaginationFilter paginationFilter = new PaginationFilter
            {
                PageNumber = pageNumber,
                PageSize = pageSize
            };

            var topUsers = await this._repository.GetTopBuyersAsync(paginationFilter);

            stopwatch.Stop();

            var elapsedTime = stopwatch.ElapsedMilliseconds;
            Trace.WriteLine($"GetTopUsers executed in {elapsedTime} ms");

            return Ok(new
            {
                ExecutionTimeInMilliseconds = elapsedTime,
                Data = topUsers
            });
        }
    }
}