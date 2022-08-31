using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Data.EntityHistory
{
    public class EntityHistoryStore : IEntityHistoryStore
    {
        private readonly IRepository<EntityChangeSet, long> _changeSetRepository;

        /// <summary>
        /// Creates a new <see cref="EntityHistoryStore"/>.
        /// </summary>
        public EntityHistoryStore(
            IRepository<EntityChangeSet, long> changeSetRepository)
        {
            _changeSetRepository = changeSetRepository;
        }

        public virtual async Task SaveAsync(EntityChangeSet changeSet)
        {
            await _changeSetRepository.InsertAsync(changeSet);

        }

        public virtual void Save(EntityChangeSet changeSet)
        {
            _changeSetRepository.Insert(changeSet);

        }
    }
}
