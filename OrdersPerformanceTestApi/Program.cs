namespace OrdersPerformanceTestApi
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.OpenApi.Models;

    using OrdersPerformanceTestApi.Services;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Adding needed services to the container.
            builder.Services.AddAuthorization();
            builder.Services.AddMemoryCache();
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("OrdersPerformanceTestApiDb")));
            builder.Services.AddScoped<IRepositoryAsync, RepositoryAsync>();
            builder.Services.AddAutoMapper(typeof(Program));

            builder.Services.AddCors(options =>
                {
                    options.AddPolicy(
                        "AllowAll",
                        policy =>
                            {
                                policy.AllowAnyOrigin()
                                .AllowAnyMethod()
                                .AllowAnyHeader();
                            });
                });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
                {
                    // Adding some basic information to the Swagger UI.
                    options.SwaggerDoc(
                        "v1",
                        new OpenApiInfo
                        {
                            Version = "v1",
                            Title = "Orders Performance Test API",
                            Description = "An ASP.NET Web API used for testing and improving performance for a large DB",
                            Contact = new OpenApiContact
                            {
                                Name = "Mihail Iordache",
                                Email = "iordache.mihail.madalin@gmail.com"
                            }
                        });
                });
            builder.Services.AddControllers();

            var app = builder.Build();

            // CORS middleware to allow our requests from the Swagger UI.
            // We will get a CORS Network Failure on requests without this.
            app.UseCors("AllowAll");

            // Configuring the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI(options => // UseSwaggerUI is called only in Development.
                    {
                        options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
                        options.RoutePrefix = string.Empty;
                    });
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}