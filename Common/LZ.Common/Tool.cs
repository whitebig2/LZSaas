using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using QRCoder;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using SixLabors.ImageSharp;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Threading.Tasks;

namespace LZ.Common
{
    public class Tool
    {
        public static T GetAppsettings<T>(string key) where T : class, new()
        {
            string keyDir = System.IO.Directory.GetCurrentDirectory();

            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(keyDir)
                .Add(new JsonConfigurationSource { Path = "appsettings.json", ReloadOnChange = true })
                .Build();
            var appconfig = new ServiceCollection()
                .AddOptions()
                .Configure<T>(config.GetSection(key))
                .BuildServiceProvider()
                .GetService<IOptions<T>>()
                .Value;
            return appconfig;
        }
        public static T GetCommAppsettings<T>(string key) where T : class, new()
        {
            string keyDir = System.IO.Directory.GetCurrentDirectory();

            IConfiguration config = new ConfigurationBuilder()
                .SetBasePath(keyDir)
                .Add(new JsonConfigurationSource { Path = "commsettings.json", ReloadOnChange = true })
                .Build();
            var appconfig = new ServiceCollection()
                .AddOptions()
                .Configure<T>(config.GetSection(key))
                .BuildServiceProvider()
                .GetService<IOptions<T>>()
                .Value;
            return appconfig;
        }

        /// <summary>
        /// 通过反射的方式实现对象之间的映射
        /// </summary>
        /// <typeparam name="TIn"></typeparam>
        /// <typeparam name="TOut"></typeparam>
        /// <param name="tIn"></param>
        /// <returns></returns>
        public static TOut Trans<TIn, TOut>(TIn tIn)
        {
            TOut tOut = Activator.CreateInstance<TOut>();
            foreach (var itemOut in tOut.GetType().GetProperties())
            {
                var propIn = tIn.GetType().GetProperty(itemOut.Name);
                itemOut.SetValue(tOut, propIn.GetValue(tIn));
            }

            foreach (var itemOut in tOut.GetType().GetFields())
            {
                var fieldIn = tIn.GetType().GetField(itemOut.Name);
                itemOut.SetValue(tOut, fieldIn.GetValue(tIn));
            }

            return tOut;
        }
        public static string GetDescription(Enum value)
        {
            var field = value.GetType().GetField(value.ToString());
            var attribute = field.GetCustomAttribute<DescriptionAttribute>();
            return attribute?.Description ?? value.ToString();
        }
        /// <summary>
        /// 是否为日期型字符串
        /// </summary>
        /// <param name="StrSource">日期字符串(2008-05-08)</param>
        /// <returns></returns>
        public static bool IsDate(string StrSource)
        {
            return Regex.IsMatch(StrSource, @"^((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-9]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-))$");
        }

        /// <summary>
        /// 是否为时间型字符串
        /// </summary>
        /// <param name="source">时间字符串(15:00:00)</param>
        /// <returns></returns>
        public static bool IsTime(string StrSource)
        {
            return Regex.IsMatch(StrSource, @"^((20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$");
        }

        /// <summary>
        /// 是否为日期+时间型字符串
        /// </summary>
        /// <param name="source"></param>
        /// <returns></returns>
        public static bool IsDateTime(string StrSource)
        {
            return Regex.IsMatch(StrSource, @"^(((((1[6-9]|[2-9]\d)\d{2})-(0?[13578]|1[02])-(0?[1-9]|[12]\d|3[01]))|(((1[6-9]|[2-9]\d)\d{2})-(0?[13456789]|1[012])-(0?[1-9]|[12]\d|30))|(((1[6-9]|[2-9]\d)\d{2})-0?2-(0?[1-9]|1\d|2[0-8]))|(((1[6-9]|[2-9]\d)(0[48]|[2468][048]|[13579][26])|((16|[2468][048]|[3579][26])00))-0?2-29-)) (20|21|22|23|[0-1]?\d):[0-5]?\d:[0-5]?\d)$ ");
        }
            #region MD5加密
            /// <summary>
            /// 加密
            /// </summary>
            /// <param name="str">字符串</param>
            /// <returns>返回加密后字符串</returns>
            public static string Md5Psw32(string str)
        {
            using (var md5 = MD5.Create())
            {
                var result = md5.ComputeHash(Encoding.UTF8.GetBytes(str));
                var strResult = BitConverter.ToString(result);
                return strResult.Replace("-", "");
            }
        }
        public static string Encrypt(string cleanString)
        {
            byte[] bytes = new UnicodeEncoding().GetBytes(cleanString);
            return BitConverter.ToString(((HashAlgorithm)CryptoConfig.CreateFromName("MD5")).ComputeHash(bytes));
        }
        public static String getPassWord(string v)
        {
            return String.Concat(Encrypt(v), "000000000000000000000000000").Substring(0, 20);
        }
        #endregion

        #region 下载网络图片
        /// <summary>
        /// 下载网络图片，返回字节数组
        /// </summary>
        /// <param name="imageUrl">图片完整 URL</param>
        /// <returns>图片字节；失败返回 null</returns>
        public static async Task<byte[]> DownloadImageAsync(string imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return null;

            try
            {
                using var client = new HttpClient();
                client.Timeout = TimeSpan.FromSeconds(60);
                client.DefaultRequestHeaders.TryAddWithoutValidation("User-Agent", "Mozilla/5.0");

                using var response = await client.GetAsync(imageUrl, HttpCompletionOption.ResponseHeadersRead);
                if (!response.IsSuccessStatusCode)
                    return null;

                var contentType = response.Content.Headers.ContentType?.MediaType ?? string.Empty;
                if (!string.IsNullOrEmpty(contentType) &&
                    !contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase) &&
                    !contentType.Equals("application/octet-stream", StringComparison.OrdinalIgnoreCase))
                {
                    // 部分 CDN 不返回 image/*，仍允许按 URL 扩展名放行
                    var ext = Path.GetExtension(new Uri(imageUrl).AbsolutePath).ToLowerInvariant();
                    if (ext is not (".jpg" or ".jpeg" or ".png" or ".gif" or ".bmp" or ".webp" or ".tif" or ".tiff"))
                        return null;
                }

                return await response.Content.ReadAsByteArrayAsync();
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// 下载网络图片到内存流（调用方负责 Dispose）
        /// </summary>
        public static async Task<MemoryStream> DownloadImageStreamAsync(string imageUrl)
        {
            var bytes = await DownloadImageAsync(imageUrl);
            if (bytes == null || bytes.Length == 0)
                return null;

            return new MemoryStream(bytes);
        }

        /// <summary>
        /// 下载网络图片并保存到本地
        /// </summary>
        /// <param name="imageUrl">图片完整 URL</param>
        /// <param name="savePath">保存目录或完整文件路径；为目录时自动按 URL 生成文件名</param>
        /// <returns>本地相对/绝对路径；失败返回以 error-- 开头的提示</returns>
        public static async Task<string> DownloadImageToFileAsync(string imageUrl, string savePath)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return "error--提示：图片地址为空";
            if (string.IsNullOrWhiteSpace(savePath))
                return "error--提示：保存路径为空";

            try
            {
                var bytes = await DownloadImageAsync(imageUrl);
                if (bytes == null || bytes.Length == 0)
                    return "error--提示：图片下载失败";

                string filePath;
                var hasExtension = !string.IsNullOrEmpty(Path.GetExtension(savePath));
                if (hasExtension && !Directory.Exists(savePath))
                {
                    filePath = savePath;
                    var dir = Path.GetDirectoryName(filePath);
                    if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
                        Directory.CreateDirectory(dir);
                }
                else
                {
                    if (!Directory.Exists(savePath))
                        Directory.CreateDirectory(savePath);

                    var ext = Path.GetExtension(new Uri(imageUrl).AbsolutePath);
                    if (string.IsNullOrWhiteSpace(ext))
                        ext = ".jpg";
                    filePath = Path.Combine(savePath.TrimEnd('\\', '/'), Guid.NewGuid().ToString("N") + ext.ToLowerInvariant());
                }

                await File.WriteAllBytesAsync(filePath, bytes);
                return filePath;
            }
            catch (Exception ex)
            {
                return "error--提示：图片下载失败，失败原因：" + ex.Message;
            }
        }
        #endregion

        #region 二维码
        public static async Task CearteQrcode(int size, string data,string path)
        {
            QRCodeGenerator qrGenerator = new QRCodeGenerator();
            QRCodeData qrCodeData = qrGenerator.CreateQrCode(data, QRCodeGenerator.ECCLevel.Q);
            PngByteQRCode qrcode = new PngByteQRCode(qrCodeData);
            byte[] qrCodeImage = qrcode.GetGraphic(size); // 20是每个点的像素大小
            MemoryStream ms = new MemoryStream(qrCodeImage);
            var image = await Image.LoadAsync(ms);
            await image.SaveAsync(path);
            image.Dispose();
            ms.Dispose();
        }
        #endregion

        #region 随机生成文件名
        public static string RandomName()
        {
            return DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString().PadLeft(2, '0') +
                DateTime.Now.Day.ToString().PadLeft(2, '0') + DateTime.Now.Hour.ToString().PadLeft(2, '0') +
                DateTime.Now.Minute.ToString().PadLeft(2, '0') + DateTime.Now.Second.ToString().PadLeft(2, '0') + DateTime.Now.Millisecond.ToString().PadLeft(3, '0');
        }
        #endregion

        #region 上传文件
        /// <summary>
        /// 
        /// </summary>
        /// <param name="file"></param>
        /// <param name="path"></param>
        /// <param name="type"></param>
        /// <param name="fname"></param>
        /// <returns></returns>
        public static async Task<string> UploadFile(IFormFile file, string path, string type = "|.txt|.doc|.docx|.xls|.xlsx|.ppt|.pptx|.zip|.rar|.jpg|.gif|.png|.bmp|.pdf|.swf|.flv|.7z|.wps|.et|.tiff|.gd|.mp4|.wmv|.dwg|.cdr|", string fname = "")
        {
            path += DateTime.Now.ToString("yyyyMMdd") + "/";
            string FilePath = System.IO.Directory.GetCurrentDirectory() + path;
            string fName = "", fileFolder;
            string fileName, fileExtension, NewfileName;
            fileName = Path.GetFileName(file.FileName);
            if (!string.IsNullOrEmpty(fname))
            {
                NewfileName = fname + "_" + Guid.NewGuid().ToString("N");
            }
            else
            {
                NewfileName = Guid.NewGuid().ToString("N");
            }
            if (fileName != "")
            {
                fileExtension = Path.GetExtension(fileName).ToLower();
                string FileContentType = file.ContentType;
                if (type.IndexOf("|" + fileExtension + "|") != -1)
                {
                    if (!Directory.Exists(FilePath))
                    {
                        Directory.CreateDirectory(FilePath);
                    }
                    fileFolder = FilePath + NewfileName + fileExtension; // 服务器端文件路径
                    if (!File.Exists(fileFolder))
                    {
                        try
                        {
                            using (var stream = new FileStream(fileFolder, FileMode.Create))
                            {
                                await file.CopyToAsync(stream);
                            }
                            fName = path + NewfileName + fileExtension;
                        }
                        catch (Exception ex)
                        {
                            return "error--提示：文件上传失败，失败原因：" + ex.Message;
                        }
                    }
                    else
                    {
                        return "error--提示：文件已经存在，请重命名后上传";
                    }
                }
                else
                {
                    return "error--上传文件类型不正确";
                }
            }
            return fName;
        }

        #endregion

        #region 检测SQL注入
        /// <summary>
        /// 检测SQL注入,数字型
        /// </summary>
        /// <param name="s">参数</param>
        public static int CheckSqlNum(object s)
        {
            string str = "'|;|and|(|)|--|exec|insert|select|delete|update|count|*|%|chr|mid|truncate|char|declare|drop|xp_cmdshell|exec master.dbo.xp_cmdshell|net user";
            string ss = "";
            string[] str1 = str.Split('|');

            if (s != null)
            {
                ss = s.ToString().ToLower();
                for (int i = 0; i < str1.Length; i++)
                {
                    //ss.e
                    if (ss.IndexOf(str1[i]) > 0 && ss.Length == str1[i].Length)
                    {
                        return 0;
                    }
                }
            }
            else
            {
                return 0;
            }
            //int val;

            //Int32.TryParse(s.ToString(),out val);

            return Convert.ToInt32(s);
        }

        /// <summary>
        /// 检测SQL注入,字符串型
        /// </summary>
        /// <param name="s">参数</param>
        public static string CheckSqlString(object s)
        {
            string str1 = "'| and |--|exec |insert |select |delete |update | count | chr | mid | master |truncate |char |declare |drop |xp_cmdshell |exec master.dbo.xp_cmdshell|net user";
            string str2 = "＇| ａnd |－－|ｅxec |ｉnsert |ｓelect |ｄelete |ｕpdate | ｃount | ｃhr | ｍid | ｍaster |ｔruncate |ｃhar |ｄeclare |ｄrop |ｘp_cmdshell |ｅxec ｍaster.dbo.xp_cmdshell|ｎet ｕser";

            string[] str1_1 = str1.Split('|');
            string[] str2_1 = str2.Split('|');

            string result = "";
            if (s != null)
            {
                result = s.ToString().ToLower();
                for (int i = 0; i < str1_1.Length; i++)
                {
                    result = result.Replace(str1_1[i], str2_1[i]);
                }
            }
            return result;
        }

        /// <summary>
        /// 检测SQL注入,字符串型
        /// </summary>
        /// <param name="s">参数</param>
        public static bool CheckString(object s)
        {
            string check = "'|--|exec |insert |select |delete |update | count | chr | mid | master |truncate |char |declare |drop |xp_cmdshell |exec master.dbo.xp_cmdshell|net user";

            string[] checks = check.Split('|');
            if (s != null)
            {
                string result = s.ToString().ToLower();
                for (int i = 0; i < checks.Length; i++)
                {
                    if (result.IndexOf(checks[i]) > 0)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        #endregion

        #region 深度拷贝对象
        public static TOut TransReflection<TIn, TOut>(TIn tIn)
        {
            TOut tOut = Activator.CreateInstance<TOut>();
            var tInType = tIn.GetType();
            foreach (var itemOut in tOut.GetType().GetProperties())
            {
                var itemIn = tInType.GetProperty(itemOut.Name); ;
                if (itemIn != null)
                {
                    itemOut.SetValue(tOut, itemIn.GetValue(tIn));
                }
            }
            return tOut;
        }
        #endregion

        /// <summary>  
        /// 将c# DateTime时间格式转换为Unix时间戳格式  
        /// </summary>  
        /// <param name="time">时间</param>  
        /// <returns>long</returns>  
        public static long ConvertDateTimeToInt(System.DateTime time)
        {
            System.DateTime startTime = TimeZoneInfo.ConvertTime(new System.DateTime(1970, 1, 1, 0, 0, 0, 0), TimeZoneInfo.Local);
            long t = (time.Ticks - startTime.Ticks) / 10000;   //除10000调整为13位      
            return t;
        }
        /// <summary>        
        /// 时间戳转为C#格式时间        
        /// </summary>        
        /// <param name=”timeStamp”></param>        
        /// <returns></returns>        
        public static DateTime ConvertStringToDateTime(string timeStamp)
        {
            DateTime dtStart = TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1), TimeZoneInfo.Local);
            long lTime = long.Parse(timeStamp + "0000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        
        /// <summary>
        /// 将 DateTime 转换为 Unix 时间戳
        /// </summary>
        /// <param name="dateTime">需要转换的时间</param>
        /// <returns>Unix 时间戳</returns>
        public static long DateTimeToUnixTime(DateTime dateTime)
        {
            return (long)(dateTime - TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0,0), TimeZoneInfo.Local)).TotalSeconds;
        }


        /// <summary>
        /// 将 Unix 时间戳转换为 DateTime
        /// </summary>
        /// <param name="timestamp">Unix 时间戳</param>
        /// <returns>需要转换的时间</returns>
        public static DateTime UnixTimeToDateTime(long timestamp)
        {
            if (timestamp < 0)
                throw new ArgumentOutOfRangeException("timestamp is out of range");

            return TimeZoneInfo.ConvertTime(new DateTime(1970, 1, 1, 0, 0, 0,0), TimeZoneInfo.Local).AddSeconds(timestamp);
        }

        #region 清空html
        public static string RemoveHTML(string html)
        {
            string MatchVale = html;
            MatchVale = new Regex("<br>", RegexOptions.IgnoreCase).Replace(MatchVale, "\n");
            foreach (Match s in Regex.Matches(html, "<[^{><}]*>"))
            {
                MatchVale = MatchVale.Replace(s.Value, "");
            }
            //"(<[^{><}]*>)"//@"<[\s\S-! ]*?>"//"<.+?>"//<(.*)>.*<\/\1>|<(.*) \/>//<[^>]+>//<(.|\n)+?> 
            MatchVale = new Regex("\n", RegexOptions.IgnoreCase).Replace(MatchVale, "<br>");
            return MatchVale;
        }

        public static string RemoveAllHTML(string content)
        {
            if (!string.IsNullOrEmpty(content))
            {

                content = Regex.Replace(content, "(<script)[\\s\\S]*?(</script>)|(<style)[\\s\\S]*?(</style>)", " ", RegexOptions.IgnoreCase);
                //htmlStream = RemoveTag(htmlStream, "script");
                //htmlStream = RemoveTag(htmlStream, "style");
                //去掉普通HTML标记:<[^>]+>
                //替换空格:&nbsp;|&amp;|&shy;|&#160;|&#173;
                content = Regex.Replace(content, "<[^>]+>|&nbsp;|&amp;|&shy;|&#160;|&#173;|&bull;|&lt;|&gt;|&ldquo;", " ", RegexOptions.IgnoreCase);
                //htmlStream = RemoveTag(htmlStream);
                //替换左尖括号
                //htmlStream = Regex.Replace(htmlStream, "&lt;", "<");
                //替换右尖括号
                //htmlStream = Regex.Replace(htmlStream, "&gt;", ">");
                //替换空行
                //htmlStream = Regex.Replace(htmlStream, "[\n|\r|\t]", " ");//[\n|\r][\t*| *]*[\n|\r]
                content = Regex.Replace(content, "(\r\n[\r|\n|\t| ]*\r\n)|(\n[\r|\n|\t| ]*\n)", "\r\n");
                content = Regex.Replace(content, "[\t| ]{1,}", " ");

            }
            return content;
        }
        #endregion

        #region 随机数
        public static char[] constant =
     {
        '0','1','2','3','4','5','6','7','8','9',
        'a','b','c','d','e','f','g','h','i','j','k','l','m','n','o','p','q','r','s','t','u','v','w','x','y','z',
        'A','B','C','D','E','F','G','H','I','J','K','L','M','N','O','P','Q','R','S','T','U','V','W','X','Y','Z'
      };
        public static char[] numconstant =
      {
        '0','1','2','3','4','5','6','7','8','9'
      };
        public static Random rd = new Random();
        /// <summary>
        /// 随机数
        /// </summary>
        public static string GetRandomNumber(int count)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(62);

            for (int i = 0; i < count; i++)
            {
                newRandom.Append(constant[rd.Next(62)]);
            }

            return newRandom.ToString();
        }
        /// <summary>
        /// 随机数(数字)
        /// </summary>
        public static string GetRandomPureNumber(int count)
        {
            System.Text.StringBuilder newRandom = new System.Text.StringBuilder(10);

            for (int i = 0; i < count; i++)
            {
                newRandom.Append(numconstant[rd.Next(10)]);
            }

            return newRandom.ToString();
        }
        #endregion


        /// 金额转换成中文大写金额
        /// </summary>
        /// <param name="LowerMoney">eg:10.74</param>
        /// <returns></returns>
        public static string MoneyToUpper(string LowerMoney)
        {
            string functionReturnValue = null;
            bool IsNegative = false; // 是否是负数
            if (LowerMoney.Trim().Substring(0, 1) == "-")
            {
                // 是负数则先转为正数
                LowerMoney = LowerMoney.Trim().Remove(0, 1);
                IsNegative = true;
            }
            string strLower = null;
            string strUpart = null;
            string strUpper = null;
            int iTemp = 0;
            // 保留两位小数 123.489→123.49　　123.4→123.4
            LowerMoney = Math.Round(double.Parse(LowerMoney), 2, MidpointRounding.AwayFromZero).ToString();
            if (LowerMoney.IndexOf(".") > 0)
            {
                if (LowerMoney.IndexOf(".") == LowerMoney.Length - 2)
                {
                    LowerMoney = LowerMoney + "0";
                }
            }
            else
            {
                LowerMoney = LowerMoney + ".00";
            }
            strLower = LowerMoney;
            iTemp = 1;
            strUpper = "";
            while (iTemp <= strLower.Length)
            {
                switch (strLower.Substring(strLower.Length - iTemp, 1))
                {
                    case ".":
                        strUpart = "圆";
                        break;
                    case "0":
                        strUpart = "零";
                        break;
                    case "1":
                        strUpart = "壹";
                        break;
                    case "2":
                        strUpart = "贰";
                        break;
                    case "3":
                        strUpart = "叁";
                        break;
                    case "4":
                        strUpart = "肆";
                        break;
                    case "5":
                        strUpart = "伍";
                        break;
                    case "6":
                        strUpart = "陆";
                        break;
                    case "7":
                        strUpart = "柒";
                        break;
                    case "8":
                        strUpart = "捌";
                        break;
                    case "9":
                        strUpart = "玖";
                        break;
                }

                switch (iTemp)
                {
                    case 1:
                        strUpart = strUpart + "分";
                        break;
                    case 2:
                        strUpart = strUpart + "角";
                        break;
                    case 3:
                        strUpart = strUpart + "";
                        break;
                    case 4:
                        strUpart = strUpart + "";
                        break;
                    case 5:
                        strUpart = strUpart + "拾";
                        break;
                    case 6:
                        strUpart = strUpart + "佰";
                        break;
                    case 7:
                        strUpart = strUpart + "仟";
                        break;
                    case 8:
                        strUpart = strUpart + "万";
                        break;
                    case 9:
                        strUpart = strUpart + "拾";
                        break;
                    case 10:
                        strUpart = strUpart + "佰";
                        break;
                    case 11:
                        strUpart = strUpart + "仟";
                        break;
                    case 12:
                        strUpart = strUpart + "亿";
                        break;
                    case 13:
                        strUpart = strUpart + "拾";
                        break;
                    case 14:
                        strUpart = strUpart + "佰";
                        break;
                    case 15:
                        strUpart = strUpart + "仟";
                        break;
                    case 16:
                        strUpart = strUpart + "万";
                        break;
                    default:
                        strUpart = strUpart + "";
                        break;
                }

                strUpper = strUpart + strUpper;
                iTemp = iTemp + 1;
            }

            strUpper = strUpper.Replace("零拾", "零");
            strUpper = strUpper.Replace("零佰", "零");
            strUpper = strUpper.Replace("零仟", "零");
            strUpper = strUpper.Replace("零零零", "零");
            strUpper = strUpper.Replace("零零", "零");
            strUpper = strUpper.Replace("零角零分", "整");
            strUpper = strUpper.Replace("零分", "整");
            strUpper = strUpper.Replace("零角", "零");
            strUpper = strUpper.Replace("零亿零万零圆", "亿圆");
            strUpper = strUpper.Replace("亿零万零圆", "亿圆");
            strUpper = strUpper.Replace("零亿零万", "亿");
            strUpper = strUpper.Replace("零万零圆", "万圆");
            strUpper = strUpper.Replace("零亿", "亿");
            strUpper = strUpper.Replace("零万", "万");
            strUpper = strUpper.Replace("零圆", "圆");
            strUpper = strUpper.Replace("零零", "零");

            // 对壹圆以下的金额的处理
            if (strUpper.Substring(0, 1) == "圆")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "零")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "角")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "分")
            {
                strUpper = strUpper.Substring(1, strUpper.Length - 1);
            }
            if (strUpper.Substring(0, 1) == "整")
            {
                strUpper = "零圆整";
            }
            functionReturnValue = strUpper;

            if (IsNegative == true)
            {
                return "负" + functionReturnValue;
            }
            else
            {
                return functionReturnValue;
            }
        }

        /// <summary>
        /// 获取枚举的描述
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string GetEnumDescription(Enum value)
        {
            FieldInfo fi = value.GetType().GetField(value.ToString());
            DescriptionAttribute[] attributes = (DescriptionAttribute[])fi.GetCustomAttributes(typeof(DescriptionAttribute), false);
            if (attributes.Length > 0)
            {
                return attributes[0].Description;
            }
            return value.ToString(); // 如果不存在DescriptionAttribute，则返回枚举成员的名称。
        }

    }
}
