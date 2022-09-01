using AuditLog.Data;
using AuditLog.Data.EntityHistory;
using AuditLog.Dto;

namespace AuditLog.Service
{
    public class EntityChangeService : IEntityChangeService
    {
        private readonly IRepository<EntityChangeSet, long> repo;
        private readonly IRepository<EntityChange, long> repoEntity;
        public EntityChangeService(IRepository<EntityChangeSet, long> _repo, IRepository<EntityChange, long> _repoEntity)
        {
            this.repo = _repo;
            this.repoEntity = _repoEntity;
        }

        public EntityChangeSetDto GetById(long id)
        {
            var x = repo.Get(id);
            var allEntity = repoEntity.GetAllIncluding(x => x.EntityChangeSetId == id, x => x.PropertyChanges).ToList();
            return new EntityChangeSetDto
            {
                Id = x.Id,
                BrowserInfo = x.BrowserInfo,
                ClientIpAddress = x.ClientIpAddress,
                ClientName = x.ClientName,
                CreationTime = x.CreationTime,
                Reason = x.Reason,
                EntityChanges = allEntity.Select(s => new EntityChangeDto()
                {
                    ChangeType = s.ChangeType,
                    EntityTypeFullName = s.EntityTypeFullName,
                    PropertyChanges = s.PropertyChanges.Select(p => new EntityPropertyChangeDto()
                    {
                        PropertyName = p.PropertyName,
                        NewValue = p.NewValue,
                        OriginalValue = p.OriginalValue
                    }).ToList()
                }).ToList()
            };



        }

        public List<EntityChangeSetDto> GetAll()
        {
            var allSet = repo.GetAllList().OrderByDescending(o=>o.CreationTime).ToList();
            var allEntity = repoEntity.GetAllIncluding(x => x.PropertyChanges).ToList();

            return allSet.Select(x => new EntityChangeSetDto()
            {
                Id = x.Id,
                BrowserInfo = x.BrowserInfo,
                ClientIpAddress = x.ClientIpAddress,
                ClientName = x.ClientName,
                CreationTime = x.CreationTime,
                Reason = x.Reason,
                EntityChanges = allEntity.Where(w => w.EntityChangeSetId == x.Id).Select(s => new EntityChangeDto()
                {
                    ChangeType = s.ChangeType,
                    EntityTypeFullName = s.EntityTypeFullName,
                    PropertyChanges = s.PropertyChanges.Select(p => new EntityPropertyChangeDto()
                    {
                        PropertyName = p.PropertyName,
                        NewValue = p.NewValue,
                        OriginalValue = p.OriginalValue
                    }).ToList()
                }).ToList()
            }).ToList();



        }
    }
}
