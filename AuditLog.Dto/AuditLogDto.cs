using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Dto
{
    public class AuditLogDto
    {
        public  long Id { get; set; }
        public  string KeyId { get; set; }
        public  string ActionType { get; set; }
        public  string TableName { get; set; }
        public  string Changes { get; set; }
        public  DateTime DateTime { get; set; }
    }
}
