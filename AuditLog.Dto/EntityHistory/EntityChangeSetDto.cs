using AuditLog.Core;
using AuditLog.Core.Extensions;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Dto
{
    public class EntityChangeSetDto
    {
        public EntityChangeSetDto()
        {
            this.EntityChanges = new List<EntityChangeDto>();
        }

        public long Id { get; set; }
        public string? BrowserInfo { get; set; }


        public string? ClientIpAddress { get; set; }


        public string? ClientName { get; set; }


        public DateTime CreationTime { get; set; }


        public string? ExtensionData { get; set; }
        public string? Reason { get; set; }

        public List<EntityChangeDto> EntityChanges { get; set; }
    }

    public class EntityChangeDto
    {
        public EntityChangeDto()
        {
            this.PropertyChanges = new List<EntityPropertyChangeDto>();
        }
        public virtual EntityChangeType ChangeType { get; set; }


        public virtual long EntityChangeSetId { get; set; }



        public virtual string EntityTypeFullName { get; set; }


        public virtual List<EntityPropertyChangeDto> PropertyChanges { get; set; }


    }

    public class EntityPropertyChangeDto
    {

        public string? NewValue { get; set; }

        public string? OriginalValue { get; set;}

        public string? PropertyName { get; set; }

    }
}
