using PaymentsModule.Client.Services;

namespace PaymentsModule.Client
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.
            builder.Services.AddRazorPages();
            
            // Add HttpClient for API calls
            builder.Services.AddHttpClient("PaymentsAPI", client =>
            {
                var apiBaseUrl = builder.Configuration["PaymentsAPI:BaseUrl"] ?? "https://localhost:7164/";
                client.BaseAddress = new Uri(apiBaseUrl);
            });

            // Register payment service
            builder.Services.AddScoped<IPaymentService, PaymentService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.MapStaticAssets();
            app.MapRazorPages()
               .WithStaticAssets();

            app.Run();
        }
    }
}
