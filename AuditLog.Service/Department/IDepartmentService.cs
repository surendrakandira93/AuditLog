using AuditLog.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Service
{
    public interface IDepartmentService
    {
        Task AddUpdateAsync(DepartmentDto dto);

        List<DepartmentDto> GetAllAsync();

        DepartmentDto GetByIdAsync(int id);

        void DeleteById(int id);
    }
}
