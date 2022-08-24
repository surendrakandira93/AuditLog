using System.ComponentModel.DataAnnotations.Schema;

namespace AuditLog.Data
{
    [Table("Employee")]
    public class Employee
    {
        public virtual int Id { get; set; }
        public virtual int DepartmentId { get; set; }
        public virtual string Name { get; set; }
        public virtual string Code { get; set; }
        public virtual DateTime DateOfBirth { get; set; }
        public virtual string Email { get; set; }        
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime ModifyDate { get; set; }
        public virtual Department Department { get; set; }
    }
}
