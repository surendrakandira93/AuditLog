using AuditLog.Data;
using AuditLog.Dto;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Service
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IRepository<Employee, int> repo;
        public EmployeeService(IRepository<Employee, int> _repo)
        {
            this.repo = _repo;
        }

        public async Task AddUpdateAsync(EmployeeDto dto)
        {

            if (dto.Id.HasValue)
            {
                var employee = repo.Get(dto.Id.Value);
                employee.Name = dto.Name;
                employee.Code = dto.Code;
                employee.Email = dto.Email;
                employee.DateOfBirth = dto.DateOfBirth.Value;
                employee.DepartmentId = dto.DepartmentId;
                employee.ModifyDate = DateTime.Now;
                repo.Update(employee);
            }
            else
            {
                var employee = new Employee();
                employee.Name = dto.Name;
                employee.Code = dto.Code;
                employee.Email = dto.Email;
                employee.DateOfBirth = dto.DateOfBirth.Value;
                employee.DepartmentId = dto.DepartmentId;
                employee.ModifyDate = DateTime.Now;
                employee.CreatedDate = DateTime.Now;
                repo.Insert(employee);
            }
            repo.SaveChanges();

        }

        public List<EmployeeDto> GetAllAsync()
        {

            return repo.GetAllIncluding(x=>x.Department).Select(x => new EmployeeDto()
            {
                Code = x.Code,
                DateOfBirth = x.DateOfBirth,
                Department = x.Department.Name,
                Name = x.Name,
                DepartmentId = x.DepartmentId,
                Email = x.Email,
                Id = x.Id
            }).ToList();



        }

        public EmployeeDto GetByIdAsync(int id)
        {
            EmployeeDto employee = new EmployeeDto();
            var dto = repo.GetIncludingByIdAsyn(x => x.Id == id, x => x.Include(m => m.Department));
            if (dto != null)
            {
                employee.Name = dto.Name;
                employee.Code = dto.Code;
                employee.Email = dto.Email;
                employee.DateOfBirth = dto.DateOfBirth;
                employee.DepartmentId = dto.DepartmentId;
                employee.Department = dto.Department.Name;
            }

            return employee;

        }

        public void DeleteById(int id)
        {
            var employee = repo.Get(id);
            repo.Delete(employee);
            repo.SaveChanges();
        }
    }
}
