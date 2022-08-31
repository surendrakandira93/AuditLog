using AuditLog.Data.Entities;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using AuditLog.Core;

namespace AuditLog.Data.EntityHistory
{
    [Table("EntityChanges")]
    public class EntityChange : Entity<long>
    {
      
        public const int MaxEntityIdLength = 48;

      
        public const int MaxEntityTypeFullNameLength = 192;

      
        public virtual DateTime ChangeTime { get; set; }

       
        public virtual EntityChangeType ChangeType { get; set; }

       
        public virtual long EntityChangeSetId { get; set; }

       
        [StringLength(MaxEntityIdLength)]
        public virtual string EntityId { get; set; }

        [StringLength(MaxEntityTypeFullNameLength)]
        public virtual string EntityTypeFullName { get; set; }


        public virtual ICollection<EntityPropertyChange> PropertyChanges { get; set; }

        #region Not mapped

        [NotMapped]
        public virtual object EntityEntry { get; set; }

        #endregion
    }
}
