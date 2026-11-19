using ClientApp.Models;
using Microsoft.Extensions.Caching.Hybrid;

namespace InventoryHub
{
    public class ProductCatalog(HybridCache cache)
    {
        private const string CacheKey = "products:all";

        private static readonly HybridCacheEntryOptions Options = new()
        {
            Expiration = TimeSpan.FromMinutes(1),
            LocalCacheExpiration = TimeSpan.FromMinutes(1)
        };

        public async Task<Product[]> GetProductsAsync(CancellationToken cancellationToken = default)
        {
            return await cache.GetOrCreateAsync(
                CacheKey,
                _ => ValueTask.FromResult(LoadProducts()),
                Options,
                cancellationToken: cancellationToken);
        }

        private static Product[] LoadProducts() =>
        [
            new() { Id = 1, Name = "Laptop", Price = 1200.50, Stock = 25, Category = new() { Id = 101, Name = "Electronics" } },
            new() { Id = 2, Name = "Headphones", Price = 50.00, Stock = 100, Category = new() { Id = 102, Name = "Accessories" } },
            new(){ Id = 3, Name = "Smartphone", Price = 800.00, Stock = 50, Category = new() { Id = 101, Name = "Electronics" } },
            new() { Id = 4, Name = "Desk Chair", Price = 150.00, Stock = 30, Category = new() { Id = 103, Name = "Furniture" } },
            new() { Id = 5, Name = "Coffee Maker", Price = 80.00, Stock = 40, Category = new() { Id = 104, Name = "Appliances" } },
            new() { Id = 6, Name = "Monitor", Price = 300.00, Stock = 20, Category = new() { Id = 101, Name = "Electronics" } },
            new() { Id = 7, Name = "Keyboard", Price = 70.00, Stock = 60, Category = new() { Id = 102, Name = "Accessories" } },
            new() { Id = 8, Name = "Mouse", Price = 40.00, Stock = 80, Category = new() { Id = 102, Name = "Accessories" } },
            new() { Id = 9, Name = "Bookshelf", Price = 120.00, Stock = 15, Category = new() { Id = 103, Name = "Furniture" } },
            new() { Id = 10, Name = "Blender", Price = 60.00, Stock = 35, Category = new() { Id = 104, Name = "Appliances" } },
            new() { Id = 11, Name = "Tablet", Price = 400.00, Stock = 45, Category = new() { Id = 101, Name = "Electronics" } },
            new() { Id = 12, Name = "Webcam", Price = 90.00, Stock = 70, Category = new() { Id = 102, Name = "Accessories" } },
            new() { Id = 13, Name = "Dining Table", Price = 500.00, Stock = 10, Category = new() { Id = 103, Name = "Furniture" } },
            new() { Id = 14, Name = "Toaster", Price = 30.00, Stock = 50, Category = new() { Id = 104, Name = "Appliances" } },
            new() { Id = 15, Name = "Smartwatch", Price = 250.00, Stock = 55, Category = new() { Id = 101, Name = "Electronics" } },
            new() { Id = 16, Name = "External Hard Drive", Price = 100.00, Stock = 40, Category = new() { Id = 102, Name = "Accessories" } },
            new() { Id = 17, Name = "Recliner", Price = 350.00, Stock = 12, Category = new() { Id = 103, Name = "Furniture" } },
            new() { Id = 18, Name = "Microwave", Price = 120.00, Stock = 25, Category = new() { Id = 104, Name = "Appliances" } },
            new() { Id = 19, Name = "Gaming Console", Price = 500.00, Stock = 30, Category = new() { Id = 101, Name = "Electronics" } },
            new() { Id = 20, Name = "VR Headset", Price = 300.00, Stock = 20, Category = new() { Id = 101, Name = "Electronics" } },
            new() { Id = 21, Name = "Desk Lamp", Price = 45.00, Stock = 60, Category = new() { Id = 102, Name = "Accessories" } },

        ];
    }
}
