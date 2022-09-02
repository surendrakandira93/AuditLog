using AuditLog.Data;
using AuditLog.Dto;

namespace AuditLog.Service
{
    public class AuditLogService : IAuditLogService
    {
        private readonly IRepository<Data.Auditing.AuditLog, long> repo;
        public AuditLogService(IRepository<Data.Auditing.AuditLog, long> _repo)
        {
            this.repo = _repo;
        }

        public AuditLogDto GetByIdAsync(long id)
        {
            var x = repo.Get(id);

            return new AuditLogDto()
            {
                Id = x.Id,
                BrowserInfo = x.BrowserInfo,
                ClientIpAddress = x.ClientIpAddress,
                ClientName = x.ClientName,
                CustomData = x.CustomData,
                Exception = x.Exception,
                ExceptionMessage = x.ExceptionMessage,
                ExecutionDuration = x.ExecutionDuration,
                ExecutionTime = x.ExecutionTime,
                MethodName = x.MethodName,
                ServiceName = x.ServiceName,
                Parameters = x.Parameters,
                ReturnValue = x.ReturnValue,
                UserId = x.UserId
            };            

        }

        public List<AuditLogDto> GetAllAsync()
        {
            var allRecords = repo.GetAll();
            return allRecords.Select(x => new AuditLogDto()
            {
                Id = x.Id,
                BrowserInfo = x.BrowserInfo,
                ClientIpAddress = x.ClientIpAddress,
                ClientName = x.ClientName,
                CustomData = x.CustomData,
                Exception = x.Exception,
                ExceptionMessage = x.ExceptionMessage,
                ExecutionDuration = x.ExecutionDuration,
                ExecutionTime = x.ExecutionTime,
                MethodName = x.MethodName,
                ServiceName = x.ServiceName,
                Parameters = x.Parameters,
                ReturnValue = x.ReturnValue,
                UserId = x.UserId
            }).ToList();



        }
    }
}
