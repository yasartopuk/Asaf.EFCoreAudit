using EFCore.Interceptor.ConsoleApp;
Console.Title = "EF Core Interceptor";

using var context = new CatalogDbContext();
context.Products.AddRange(GetProducts());
context.SaveChanges();

var product = context.Products.Find(1);
product.Name = "Smartphone";
product.Price += 100;
context.SaveChanges();

context.Products.Remove(context.Products.Find(2));
context.SaveChanges();

Console.ReadLine();

List<Product> GetProducts()
{
    return new List<Product>
    {
        new Product { Id = 1, Name = "Pocket PC", Price = 499.99m },
        new Product { Id = 2, Name = "Coffee Maker", Price = 159.99m },
        new Product { Id = 3, Name = "Digital Camera", Price = 299.99m },
    };
}

