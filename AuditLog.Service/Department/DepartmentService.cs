using AuditLog.Data;
using AuditLog.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Service
{
    public class DepartmentService: IDepartmentService
    {
        private readonly IRepository<Department, int> repo;
        public DepartmentService(IRepository<Department, int> _repo)
        {
            this.repo = _repo;
        }

        public async Task AddUpdateAsync(DepartmentDto dto)
        {

            if (dto.Id.HasValue)
            {
                var department = repo.Get(dto.Id.Value);
                department.Name = dto.Name;
                department.Code = dto.Code;
                department.Description = dto.Description;
                department.ModifyDate = DateTime.Now;
                repo.Update(department);
            }
            else
            {
                var department = new Department();
                department.Name = dto.Name;
                department.Code = dto.Code;
                department.Description = dto.Description;
                department.ModifyDate = DateTime.Now;
                department.CreatedDate = DateTime.Now;
                repo.Insert(department);
            }

            repo.SaveChanges();

        }

        public List<DepartmentDto> GetAllAsync()
        {

            return repo.GetAllList().Select(x => new DepartmentDto()
            {
                Code = x.Code,
                Description = x.Description,
                Name = x.Name,
                Id = x.Id
            }).ToList();



        }

        public DepartmentDto GetByIdAsync(int id)
        {
            DepartmentDto employee = new DepartmentDto();
            var dto = repo.Get(id);
            if (dto != null)
            {
                employee.Name = dto.Name;
                employee.Code = dto.Code;
                employee.Description = dto.Description;
            }

            return employee;

        }

        public void DeleteById(int id)
        {
            var department = repo.Get(id);
            repo.Delete(department);
            repo.SaveChanges();
        }
    }
}
