using Azure.Core;
using LZ.ApiService.ContractResolvers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace LZ.ApiService.Helper
{

    public static class FxqHelper
    {
        /// <summary>
        /// 基于Md5的自定义加密字符串方法：输入一个字符串，返回一个由32个字符组成的十六进制的哈希散列（字符串）。
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns>加密后的十六进制的哈希散列（字符串）</returns>
        public static string Md5(this string str)
        {


            //将输入字符串转换成字节数组
            var buffer = Encoding.UTF8.GetBytes(str);
            //接着，创建Md5对象进行散列计算
            var data = MD5.Create().ComputeHash(buffer);

            //创建一个新的Stringbuilder收集字节
            var sb = new StringBuilder();

            //遍历每个字节的散列数据
            foreach (var t in data)
            {
                //格式每一个十六进制字符串
                sb.Append(t.ToString("x2"));
            }
            //返回十六进制字符串
            return sb.ToString();
        }

        /// <summary>
        /// 基于Sha1的自定义加密字符串方法：输入一个字符串，返回一个由40个字符组成的十六进制的哈希散列（字符串）。
        /// </summary>
        /// <param name="str">要加密的字符串</param>
        /// <returns>加密后的十六进制的哈希散列（字符串）</returns>
        public static string Sha1(this string str)
        {
            var buffer = Encoding.UTF8.GetBytes(str);
            var data = SHA1.Create().ComputeHash(buffer);

            var sb = new StringBuilder();
            foreach (var t in data)
            {
                sb.Append(t.ToString("x2"));
            }

            return sb.ToString();
        }


        public static string Sign<T>(T item, string accessToken, string nonce)
        {
            var json = ToJson(item);
            var formater = json.Replace("\":", "=")
                .Replace(",\"", "||")
                .Replace("\"", "")
                .TrimStart('{')
                .TrimEnd('}');
            formater += $"||token={accessToken}||nonce={nonce}";

            var signStrByte = System.Text.Encoding.UTF8.GetBytes(formater);
            var base64Str = Convert.ToBase64String(signStrByte);
            var signSha = base64Str.Sha1();
            var signMd5 = signSha.Md5();
            return signMd5;
        }

        public static string SignJson(string json, string accessToken, string nonce)
        {
            var formater = json.Replace("\":", "=")
                .Replace(",\"", "||")
                .Replace("\"", "")
                .TrimStart('{')
                .TrimEnd('}').Replace("},{", "}||{");
            formater += $"||token={accessToken}||nonce={nonce}";

            Console.WriteLine($"放心签签名Json：{json}");
            Console.WriteLine(  $"放心签签名：{formater}");

            var signStrByte = System.Text.Encoding.UTF8.GetBytes(formater);
            var base64Str = Convert.ToBase64String(signStrByte);
            var signSha = base64Str.Sha1();
            var signMd5 = signSha.Md5();
            return signMd5;
        }

        public static string ToJson<T>(T item)
        {

           var json = Newtonsoft.Json.JsonConvert.SerializeObject(item, new Newtonsoft.Json.JsonSerializerSettings()
            {
                NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore,
                ContractResolver = new FxqContractResolver()
            });
            return json;
        }
    }
}
