using Newtonsoft.Json;

namespace AuditLog.Core.Auditing
{
    public class JsonNetAuditSerializer : IAuditSerializer
    {

        public string Serialize(object obj)
        {
            var options = new JsonSerializerSettings
            {
                ContractResolver = new AuditingContractResolver(new List<Type>())
            };

            return JsonConvert.SerializeObject(obj, options);
        }
    }
}
