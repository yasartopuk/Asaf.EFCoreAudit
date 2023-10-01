using Asaf.EFCoreAudit.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Asaf.AuditLog.Web.Data.Entities;

public class Product : IAuditable<int>
{
    public int Id { get; set; }

    public string Name { get; set; }

    public decimal Price { get; set; }

    [ForeignKey("Category")]

    public int CategoryId { get; set; }


    public virtual Category? Category { get; set; }
}
