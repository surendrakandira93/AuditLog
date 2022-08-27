﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Data.Entities
{
    public interface IEntity<TPrimaryKey>
    {
        
        TPrimaryKey Id { get; set; }       
        bool IsTransient();
    }
}
