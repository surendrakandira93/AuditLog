using AuditLog.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace AuditLog.Data
{
    [Table("Department")]
    public class Department: Entity
    {        
        
        public virtual string Name { get; set; }
        public virtual string Code { get; set; }
        public virtual string Description { get; set; }
        public virtual DateTime CreatedDate { get; set; }
        public virtual DateTime ModifyDate { get; set; }
        public virtual ICollection<Employee> Employees { get; set; }
    }
}
