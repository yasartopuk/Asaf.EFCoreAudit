using Asaf.AuditLog.Web.Data.Entities;
using Asaf.EFCoreAudit.Configurations;
using Microsoft.EntityFrameworkCore;

namespace Asaf.AuditLog.Web.Data;

public static class AppDbInitializer
{
    public static async Task SeedAsync(IServiceProvider serviceProvider)
    {
        //using var _ = AuditContext.Create();
        //AuditContext.NoAudit();

        await InitializeData(serviceProvider);
    }
    
    private static async Task InitializeData(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

        if (context == null) return;
        if (await context.Products.AnyAsync() && await context.Categories.AnyAsync()) return;

        context.Products.RemoveRange(await context.Products.ToListAsync());
        context.Categories.RemoveRange(await context.Categories.ToListAsync());
        await context.SaveChangesAsync();

        context.Categories.AddRange(GetCategories());
        await context.SaveChangesAsync();

        var category = await context.Categories.FindAsync(2);
        category.Name = "Appliances New";

        var product1 = await context.Products.FindAsync(2);
        product1.Name = "Hair Dryer";
        product1.CategoryId = 2;
        product1.Price += 50;

        var product2 = await context.Products.FindAsync(5);
        product2.Name = "Macbook";
        product2.CategoryId = 1;
        product2.Price += 50;

        var product3 = await context.Products.FindAsync(8);
        product3.Name = "Levi's Jeans";
        product3.Price -= 5;

        context.Products.Remove(await context.Products.FindAsync(10));
        context.Products.Remove(await context.Products.FindAsync(11));

        await context.SaveChangesAsync();
    }

    private static List<Category> GetCategories()
    {
        List<Category> categories = new List<Category>
        {
            new Category { Id = 1, Name = "Electronics" },
            new Category { Id = 2, Name = "Appliances" },
            new Category { Id = 3, Name = "Clothing" },
        };

        var products = GetProducts();

        foreach (var category in categories)
        {
            foreach (var product in products.Where(p => p.CategoryId == category.Id))
            {
                category.Products.Add(product);
            }
        }

        return categories;
    }

    private static List<Product> GetProducts()
    {
        List<Product> products = new List<Product>
        {
            // Electronics
            new Product { Id = 1, Name = "Smartphone", Price = 499.99m, CategoryId = 1 },
            new Product { Id = 2, Name = "Laptop", Price = 899.99m, CategoryId = 1 },
            new Product { Id = 3, Name = "Digital Camera", Price = 299.99m, CategoryId = 1 },
            
            // Appliances
            new Product { Id = 4, Name = "Coffee Maker", Price = 59.99m, CategoryId = 2 },
            new Product { Id = 5, Name = "Toaster", Price = 29.99m, CategoryId = 2 },
            new Product { Id = 6, Name = "Vacuum Cleaner", Price = 149.99m, CategoryId = 2 },
          
            // Clothing
            new Product { Id = 7, Name = "T-Shirt", Price = 19.99m, CategoryId = 3 },
            new Product { Id = 8, Name = "Jeans", Price = 39.99m, CategoryId = 3 },
            new Product { Id = 9, Name = "Dress", Price = 49.99m, CategoryId = 3 },
            new Product { Id = 10, Name = "Pant", Price = 49.99m, CategoryId = 3 },
            new Product { Id = 11, Name = "Shoes", Price = 49.99m, CategoryId = 3 },
        };

        return products;
    }
}
