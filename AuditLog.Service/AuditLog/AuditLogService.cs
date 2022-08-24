using AuditLog.Data;
using AuditLog.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Service
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IRepository<Data.AuditLog, long> repo;
        public AuditLogService(IRepository<Data.AuditLog, long> _repo)
        {
            this.repo = _repo;
        }

        public AuditLogDto GetByIdAsync(long id)
        {
            var x = repo.Get(id);

            return new AuditLogDto()
            {
                Id = x.Id,
                ActionType = x.ActionType,
                Changes = x.Changes,
                DateTime = x.DateTime,
                KeyId = x.KeyId,
                TableName = x.TableName
            };



        }

        public List<AuditLogDto> GetAllAsync()
        {

            return repo.GetAllList().Select(x => new AuditLogDto()
            {
                Id = x.Id,
                ActionType = x.ActionType,
                Changes = x.Changes,
                DateTime = x.DateTime,
                KeyId = x.KeyId,
                TableName = x.TableName
            }).ToList();



        }
    }
}
