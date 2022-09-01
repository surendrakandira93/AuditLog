using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace AuditLog.Data.EntityHistory
{
    public interface IEntityHistoryHelper
    {
        EntityChangeSet CreateEntityChangeSet(ICollection<EntityEntry> entityEntries);


        EntityChangeSet UpdateEntityChangeSet(EntityChangeSet changeSet);
    }
}
