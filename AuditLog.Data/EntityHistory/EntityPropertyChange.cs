using AuditLog.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AuditLog.Core.Extensions;

namespace AuditLog.Data.EntityHistory
{
    [Table("EntityPropertyChanges")]
    public class EntityPropertyChange : Entity<long>
    {
      
        public const int MaxPropertyNameLength = 96;

        
        public const int MaxValueLength = 512;

      
        public const int MaxPropertyTypeFullNameLength = 192;

        public virtual long EntityChangeId { get; set; }

       
        [StringLength(MaxValueLength)]
        public virtual string NewValue { get; set; }

      
        [StringLength(MaxValueLength)]
        public virtual string OriginalValue { get; set; }

      
        [StringLength(MaxPropertyNameLength)]
        public virtual string PropertyName { get; set; }

       
        [StringLength(MaxPropertyTypeFullNameLength)]
        public virtual string PropertyTypeFullName { get; set; }       

      
        public virtual string NewValueHash { get; set; }

     
        public virtual string OriginalValueHash { get; set; }

      
        public virtual void SetNewValue(string newValue)
        {
            NewValueHash = newValue?.ToMd5();
            NewValue = newValue.TruncateWithPostfix(MaxValueLength);
        }

        public virtual void SetOriginalValue(string originalValue)
        {
            OriginalValueHash = originalValue?.ToMd5();
            OriginalValue = originalValue.TruncateWithPostfix(MaxValueLength);
        }

        public virtual bool IsValuesEquals()
        {
            //To support previous data
            if (!NewValueHash.IsNullOrWhiteSpace() && !OriginalValueHash.IsNullOrWhiteSpace())
            {
                return NewValueHash == OriginalValueHash;
            }

            return NewValue == OriginalValue;
        }
    }
}
