using Microsoft.EntityFrameworkCore;
using Asaf.AuditLog.Web.Data.Entities;

namespace Asaf.AuditLog.Web.Data;

public class AppDbContext : DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }

    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    { 

    }
}
