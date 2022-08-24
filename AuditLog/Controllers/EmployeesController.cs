using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuditLog.Service;
using AuditLog.Dto;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AuditLog.Controllers
{
    public class EmployeesController : Controller
    {
        private readonly IEmployeeService service;
        private readonly IDepartmentService departmentService;

        public EmployeesController(IEmployeeService _service, IDepartmentService _departmentService)
        {
            this.service = _service;
            this.departmentService = _departmentService;
        }

        // GET: Employees
        public IActionResult Index()
        {
            return View(service.GetAllAsync());
        }

        // GET: Employees/Details/5
        public IActionResult Details(int? id)
        {

            var employee = service.GetByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewBag.departmentList = departmentService.GetAllAsync();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(EmployeeDto employee)
        {
            await service.AddUpdateAsync(employee);
            return RedirectToAction(nameof(Index));
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = service.GetByIdAsync(id.Value);
            if (employee == null)
            {
                return NotFound();
            }
            ViewBag.departmentList = departmentService.GetAllAsync();
            return View(employee);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(EmployeeDto employee)
        {
            await service.AddUpdateAsync(employee);
            return RedirectToAction(nameof(Index));
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = service.GetByIdAsync(id.Value);

            return View(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            service.DeleteById(id);
            return RedirectToAction(nameof(Index));
        }


    }
}
