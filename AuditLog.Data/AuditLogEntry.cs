using Microsoft.EntityFrameworkCore.ChangeTracking;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Data
{
    public class AuditLogEntry
    {
        public AuditLogEntry(EntityEntry entry)
        {
            Entry = entry;
            Changes = new List<AuditDetail>();
        }
        public EntityEntry Entry { get; }
        public string UserId { get; set; }
        public string TableName { get; set; }
        public object KeyValues { get; set; }
        public List<AuditDetail> Changes { get; set; }
        public AuditType AuditType { get; set; }
        public AuditLog ToAudit()
        {
            var audit = new AuditLog();
            audit.ActionType = AuditType.ToString();
            audit.TableName = TableName;
            audit.DateTime = DateTime.Now;
            audit.KeyId = JsonConvert.SerializeObject(KeyValues);
            audit.Changes = this.Changes.Any() ? JsonConvert.SerializeObject(this.Changes) : null;
            return audit;
        }
    }

    public class AuditDetail
    {
        public string FieldName { get; set; }
        public object ValueBefore { get; set; }
        public object ValueAfter { get; set; }
    }
}
