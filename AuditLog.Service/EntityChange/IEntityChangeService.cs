using AuditLog.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Service
{
    public interface IEntityChangeService
    {
        EntityChangeSetDto GetById(long id);

        List<EntityChangeSetDto> GetAll();

    }
}
