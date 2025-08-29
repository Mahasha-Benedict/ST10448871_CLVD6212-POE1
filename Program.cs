using ABCRetailers.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;

namespace ABCRetailers
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllersWithViews();

            // Register Azure storage service with configuration binding
            builder.Services.AddSingleton<IAzureStorageService>(sp =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var connectionString = config["AzureStorage:ConnectionString"];
                return new AzureStorageService(connectionString);
            });

            var app = builder.Build();

            // Ensure Azure Tables and Blob Containers exist
            using (var scope = app.Services.CreateScope())
            {
                var storage = scope.ServiceProvider.GetRequiredService<IAzureStorageService>();
                await storage.CreateTablesIfNotExistsAsync();
            }

            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthorization();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            await app.RunAsync();
        }
    }
}
