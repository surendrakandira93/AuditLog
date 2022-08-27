using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security.Principal;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Data.Entities
{
    public abstract class Entity : Entity<int>, IEntity
    {

    }

    public abstract class Entity<TPrimaryKey> : IEntity<TPrimaryKey>
    {
        
        public virtual TPrimaryKey Id { get; set; }

       
        public virtual bool IsTransient()
        {
            if (EqualityComparer<TPrimaryKey>.Default.Equals(Id, default(TPrimaryKey)))
            {
                return true;
            }

            if (typeof(TPrimaryKey) == typeof(int))
            {
                return Convert.ToInt32(Id) <= 0;
            }

            if (typeof(TPrimaryKey) == typeof(long))
            {
                return Convert.ToInt64(Id) <= 0;
            }

            return false;
        }

        /// <inheritdoc/>
        public virtual bool EntityEquals(object obj)
        {
            if (obj == null || !(obj is Entity<TPrimaryKey>))
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }
            var other = (Entity<TPrimaryKey>)obj;
            if (IsTransient() && other.IsTransient())
            {
                return false;
            }

            var typeOfThis = GetType();
            var typeOfOther = other.GetType();
            if (!typeOfThis.GetTypeInfo().IsAssignableFrom(typeOfOther) && !typeOfOther.GetTypeInfo().IsAssignableFrom(typeOfThis))
            {
                return false;
            }

            return Id.Equals(other.Id);
        }

        public override string ToString()
        {
            return $"[{GetType().Name} {Id}]";
        }
    }
}
