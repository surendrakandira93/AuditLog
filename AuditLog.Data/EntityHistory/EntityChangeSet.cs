using AuditLog.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuditLog.Data.Auditing;

namespace AuditLog.Data.EntityHistory
{
    [Table("EntityChangeSets")]
    public class EntityChangeSet : Entity<long>, IHasCreationTime
    {
      
        public const int MaxBrowserInfoLength = 512;

       
        public const int MaxClientIpAddressLength = 64;

      
        public const int MaxClientNameLength = 128;

       
        public const int MaxReasonLength = 256;

        [StringLength(MaxBrowserInfoLength)]
        public virtual string BrowserInfo { get; set; }

       
        [StringLength(MaxClientIpAddressLength)]
        public virtual string ClientIpAddress { get; set; }

       
        public virtual string ClientName { get; set; }

       
        public virtual DateTime CreationTime { get; set; }

       
        public virtual string ExtensionData { get; set; }


        public virtual long? ImpersonatorUserId { get; set; }

        [StringLength(MaxReasonLength)]
        public virtual string Reason { get; set; }

       
        public virtual long? UserId { get; set; }

        public virtual IList<EntityChange> EntityChanges { get; set; }

        public EntityChangeSet()
        {
            EntityChanges = new List<EntityChange>();
        }
    }
}
