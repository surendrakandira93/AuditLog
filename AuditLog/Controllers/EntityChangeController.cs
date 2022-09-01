using AuditLog.Service;
using Microsoft.AspNetCore.Mvc;

namespace AuditLog.Controllers
{
    public class EntityChangeController : Controller
    {
        private readonly IEntityChangeService service;

        public EntityChangeController(IEntityChangeService _service)
        {
            service = _service;
        }

        // GET: Audits
        public IActionResult Index()
        {
            return View(service.GetAll());
        }

        // GET: Audits/Details/5
        public IActionResult Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var audit = service.GetById(id.Value);
            if (audit == null)
            {
                return NotFound();
            }

            return View(audit);
        }

    }
}
