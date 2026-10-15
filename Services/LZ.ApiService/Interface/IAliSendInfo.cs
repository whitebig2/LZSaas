using System;
using System.Collections.Generic;
using System.Text;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Core.Http;
using Aliyun.Acs.CCC.Model.V20200701;
using System.Web;
using System.Net;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Threading.Tasks;
using System.Net.Mail;
using System.Net.Mime;

namespace LZ.ApiService.Interface
{
    public interface IAliSendInfo
    {
		string SendAliSms(string phones, string code, string content, Model.SiteSettings config);
		string SendAliEmail(string address, string subject, string text, string html, string file);
    }
}
