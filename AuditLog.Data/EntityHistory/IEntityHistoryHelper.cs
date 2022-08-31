using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AuditLog.Data.EntityHistory
{
    public interface IEntityHistoryHelper
    {
        EntityChangeSet CreateEntityChangeSet(ICollection<EntityEntry> entityEntries);

        Task SaveAsync(EntityChangeSet changeSet);

        void Save(EntityChangeSet changeSet);
    }
}
