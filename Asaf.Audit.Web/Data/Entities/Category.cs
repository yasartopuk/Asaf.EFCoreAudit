using Asaf.EFCoreAudit.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asaf.AuditLog.Web.Data.Entities;

public class Category : IAuditable, INameable
{
    public int Id { get; set; }

    public string? Name { get; set; }

    [ForeignKey("CategoryId")]
    public ICollection<Product>? Products { get; set; }

    public Category()
    {
        Products = new HashSet<Product>();
    }
}
