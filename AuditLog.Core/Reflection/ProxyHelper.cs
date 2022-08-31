using Castle.DynamicProxy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Core.Reflection
{
    public static class ProxyHelper
    {
      
        public static object UnProxy(object obj)
        {
            return ProxyUtil.GetUnproxiedInstance(obj);
        }

      
        public static Type GetUnproxiedType(object obj)
        {
            return ProxyUtil.GetUnproxiedType(obj);
        }
    }
}
