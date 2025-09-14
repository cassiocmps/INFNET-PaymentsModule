using PaymentsModule.API.Services;
using PaymentsModule.Persistance.Data;
using PaymentsModule.Persistance.Repositories;
using Microsoft.EntityFrameworkCore;
using PaymentsModule.Domain.Interfaces.Repositories;
using PaymentsModule.Domain.Interfaces.Services;
using PaymentsModule.ExternalPaymentsProvider.Interfaces;
using PaymentsModule.ExternalPaymentsProvider.Services;
using PaymentsModuleAPI.Middleware;

namespace PaymentsModule.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddControllers();
            
            // Register Swagger/OpenAPI services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Configure Entity Framework with SQLite
            builder.Services.AddDbContext<PaymentsDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // Register application services (Dependency Injection)
            builder.Services.AddScoped<IPaymentService, PaymentService>();
            builder.Services.AddScoped<IPaymentRepository, PaymentRepository>();
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();
            builder.Services.AddScoped<ICardRepository, CardRepository>();
            builder.Services.AddScoped<IRefundRepository, RefundRepository>();
            
            // Register external payment provider
            builder.Services.AddScoped<IExternalPaymentProvider, ExternalPaymentProvider>();

            var app = builder.Build();

            // Add exception handling middleware
            app.UseMiddleware<ExceptionHandlingMiddleware>();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}
