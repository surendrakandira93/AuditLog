using AuditLog.Dto;
using AuditLog.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AuditLog.Controllers
{
    public class DepartmentController : Controller
    {
        private readonly IDepartmentService service;

        public DepartmentController(IDepartmentService _service)
        {
            service = _service;
        }

        // GET: Department
        public IActionResult Index()
        {
            return View(service.GetAllAsync());
        }

        // GET: Department/Details/5
        public IActionResult Details(int? id)
        {

            var department = service.GetByIdAsync(id.Value);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Department/Create
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DepartmentDto department)
        {
            await service.AddUpdateAsync(department);
            return RedirectToAction(nameof(Index));
        }

        // GET: Department/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = service.GetByIdAsync(id.Value);
            if (department == null)
            {
                return NotFound();
            }
            return View(department);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(DepartmentDto department)
        {
            await service.AddUpdateAsync(department);
            return RedirectToAction(nameof(Index));
        }

        // GET: Department/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = service.GetByIdAsync(id.Value);

            return View(department);
        }

        // POST: Department/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            service.DeleteById(id);
            return RedirectToAction(nameof(Index));
        }
    }
}
