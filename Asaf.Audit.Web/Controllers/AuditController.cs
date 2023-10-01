using Microsoft.AspNetCore.Mvc;
using Asaf.EFCoreAudit.Entities;
using Asaf.EFCoreAudit.Providers;

namespace Asaf.AuditLog.Web.Controllers;

public class AuditController : Controller
{
    private readonly IAuditProvider _auditLogProvider;

    public AuditController(IAuditProvider auditLogProvider)
    {
        _auditLogProvider = auditLogProvider;
    }

    public IActionResult Index()
    {
        var audits = _auditLogProvider.GetAudits().OrderByDescending(x => x.TimeStamp).ToList();
        return View(audits);
    }

    [Route("audit/audits/{tableName}/{recordId}")]
    public IActionResult _AuditLogPartial(string tableName, int recordId)
    {
        List<AuditEntry> auditLogs = _auditLogProvider.GetAudits(tableName, recordId);
        return PartialView(auditLogs);
    }

    public IActionResult ClearAudits()
    {
        _auditLogProvider.ClearAllAudits();
        return RedirectToAction("Index");
    }
}
