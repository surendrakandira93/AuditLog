using Microsoft.AspNetCore.Mvc;
using AuditLog.Service;

namespace AuditLog.Controllers
{
    public class AuditsController : Controller
    {
        private readonly IAuditLogService service;

        public AuditsController(IAuditLogService _service)
        {
            service = _service;
        }

        // GET: Audits
        public async Task<IActionResult> Index()
        {
              return View(service.GetAllAsync()) ;
        }

        // GET: Audits/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var audit = service.GetByIdAsync(id.Value);
            if (audit == null)
            {
                return NotFound();
            }

            return View(audit);
        }

       
    }
}
