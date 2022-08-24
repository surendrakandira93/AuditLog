using AuditLog.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Service
{
    public interface IAuditLogService
    {
        AuditLogDto GetByIdAsync(long id);

        List<AuditLogDto> GetAllAsync();
    }
}
