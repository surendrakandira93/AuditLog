using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace AuditLog.Core.Auditing
{
    public class AuditingContractResolver : CamelCasePropertyNamesContractResolver
    {
        private readonly List<Type> _ignoredTypes;

        public AuditingContractResolver(List<Type> ignoredTypes)
        {
            _ignoredTypes = ignoredTypes;
        }

        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (member.IsDefined(typeof(DisableAuditingAttribute)) || member.IsDefined(typeof(JsonIgnoreAttribute)))
            {
                property.ShouldSerialize = instance => false;
            }

            foreach (var ignoredType in _ignoredTypes)
            {
                if (ignoredType.GetTypeInfo().IsAssignableFrom(property.PropertyType))
                {
                    property.ShouldSerialize = instance => false;
                    break;
                }
            }

            return property;
        }
    }
}
