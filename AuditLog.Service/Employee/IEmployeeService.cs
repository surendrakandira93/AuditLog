using AuditLog.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Service
{
    public interface IEmployeeService
    {
        Task AddUpdateAsync(EmployeeDto dto);

        List<EmployeeDto> GetAllAsync();
        EmployeeDto GetByIdAsync(int id);
        void DeleteById(int id);
    }
}
