using MathNet.Numerics;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Org.BouncyCastle.Math.EC.ECCurve;

namespace System
{
    public static class CommonExtension
    {


        public static decimal ToFixed(this decimal amount, int decimals)
        {
            return decimal.Round(amount, decimals, MidpointRounding.AwayFromZero);
        }
        public static string ToCamelCaseJson(this object item)
        {

            var json = Newtonsoft.Json.JsonConvert.SerializeObject(item, new Newtonsoft.Json.JsonSerializerSettings()
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            });
            return json;
        }
    }
}
