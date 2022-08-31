using AuditLog.Data.Auditing;
using AuditLog.Core.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AuditLog.Data.EntityHistory.Extensions
{
    internal static class EntityEntryExtensions
    {
        internal static bool IsCreated(this EntityEntry entityEntry)
        {
            return entityEntry.State == EntityState.Added;
        }

        internal static bool IsDeleted(this EntityEntry entityEntry)
        {
            if (entityEntry.State == EntityState.Deleted)
            {
                return true;
            }
            var entity = entityEntry.Entity;
            return entity is ISoftDelete && entity.As<ISoftDelete>().IsDeleted;
        }
    }
}