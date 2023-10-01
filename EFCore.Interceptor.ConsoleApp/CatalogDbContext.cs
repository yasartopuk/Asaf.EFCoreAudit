using Microsoft.EntityFrameworkCore;
namespace EFCore.Interceptor.ConsoleApp;

public class CatalogDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder
            .AddInterceptors(new SampleSaveChangesInterceptor())
            .UseInMemoryDatabase("catalog_db");
    }
}
