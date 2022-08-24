using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Data
{
    [Table("AuditLog")]
    public class AuditLog
    {
        public virtual long Id { get; set; }
        public virtual string KeyId { get; set; }
        public virtual string ActionType { get; set; }
        public virtual string TableName { get; set; }
        public virtual string Changes { get; set; }
        public virtual DateTime DateTime { get; set; }

    }
}
