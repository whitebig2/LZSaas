using System.Security.Cryptography;
using System.Text;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace LZ.ApiService.Helper
{
    public static class BaiWangHelper
    {
        private static readonly JsonSerializerSettings JsonSettings = new()
        {
            NullValueHandling = NullValueHandling.Ignore,
            ContractResolver = new CamelCasePropertyNamesContractResolver()
        };

        public static string ToJson(object data)
        {
            return JsonConvert.SerializeObject(data, JsonSettings);
        }

        public static string Sign(Dictionary<string, string> parameters, string bodyJson, string appSecret)
        {
            var sorted = new SortedDictionary<string, string>(parameters, StringComparer.Ordinal);
            var signBuilder = new StringBuilder(appSecret);
            foreach (var item in sorted)
            {
                if (string.IsNullOrWhiteSpace(item.Value))
                {
                    continue;
                }
                signBuilder.Append(item.Key).Append(item.Value);
            }
            signBuilder.Append(bodyJson);
            signBuilder.Append(appSecret);
            return ComputeMd5(signBuilder.ToString()).ToUpperInvariant();
        }

        public static string Sha256(string input)
        {
            var hashBytes = SHA256.HashData(Encoding.UTF8.GetBytes(input));
            var builder = new StringBuilder(hashBytes.Length * 2);
            foreach (var b in hashBytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }

        /// <summary>
        /// 百望密码加密：SHA256(明文密码 + 盐值)
        /// </summary>
        public static string EncryptPassword(string password, string salt)
        {
            return Sha256($"{password}{salt}");
        }

        private static string ComputeMd5(string input)
        {
            var hashBytes = MD5.HashData(Encoding.UTF8.GetBytes(input));
            var builder = new StringBuilder(hashBytes.Length * 2);
            foreach (var b in hashBytes)
            {
                builder.Append(b.ToString("x2"));
            }
            return builder.ToString();
        }
    }
}
