using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.ApiService.ContractResolvers
{
 
    public class FxqContractResolver : CamelCasePropertyNamesContractResolver
    {
        protected override IList<JsonProperty> CreateProperties(Type type, MemberSerialization memberSerialization)
        {
            //base.NamingStrategy=new DefaultNamingStrategy();
            return base.CreateProperties(type, memberSerialization)
                .OrderBy(p => p.PropertyName) // 按属性名称排序  
                .ToList();
        }
    }
}
