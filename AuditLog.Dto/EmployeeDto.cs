using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Dto
{
    public class EmployeeDto
    {
        public  int? Id { get; set; }
        public  int DepartmentId { get; set; }
        public  string Name { get; set; }
        public  string Code { get; set; }
        public  DateTime? DateOfBirth { get; set; }
        public  string Email { get; set; }
        public  string? Department { get; set; }
    }
}
