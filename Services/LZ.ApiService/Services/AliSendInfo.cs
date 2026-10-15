using LZ.ApiService.Interface;
using LZ.Core.Interface;
using Aliyun.Acs.Core;
using Aliyun.Acs.Core.Exceptions;
using Aliyun.Acs.Core.Profile;
using Aliyun.Acs.Core.Http;
using System.Net.Mail;
using System.Net.Mime;
using System.Net;

namespace LZ.ApiService.Services
{
    public class AliSendInfo : IAliSendInfo, IScopedDependency
	{
		private static String ALIYUN_AUTHORITY_ENDPOINT = "https://signin.aliyun.com/oauth2/v1/auth";
		private static String ALIYUN_TOKEN_ENDPOINT = "http://oauth.aliyun.com/v1/token";
		private static String ALIYUN_SIGN_IN_ENDPOINT = "https://signin.aliyun.com/login.htm";
		private static String REGION = "cn-shanghai";
		private static String PRODUCT = "CCC";
		private static String ENDPOINT = "CCC";
		/**
		 * 生产环境的Region是ccc.cn-shanghai.aliyuncs.com
		 */
		private static String DOMAIN = "ccc.cn-shanghai.aliyuncs.com";
		private static String VERSION = "2017-07-05";

		#region 发送阿里短信
		public string SendAliSms(string phones, string code, string content, Model.SiteSettings config)
		{
			IClientProfile profile = DefaultProfile.GetProfile("cn-hangzhou", config.AliAccessID, config.AliAccessSecret);
			DefaultAcsClient client = new DefaultAcsClient(profile);
			CommonRequest request = new CommonRequest();
			request.Method = MethodType.POST;
			request.Domain = "dysmsapi.aliyuncs.com";
			request.Version = "2017-05-25";
			request.Action = "SendSms";
			request.AddQueryParameters("PhoneNumbers", phones);
			request.AddQueryParameters("SignName", config.AliSmsSignName);
			request.AddQueryParameters("TemplateCode", code);
			request.AddQueryParameters("TemplateParam", content);
			try
			{
				CommonResponse response = client.GetCommonResponse(request);
				var tt = System.Text.Encoding.Default.GetString(response.HttpResponse.Content);
                Common.StaticLog.Debug(tt);
                return "ok";
			}
			catch (ServerException e)
			{
				Common.StaticLog.Error(e);
				return "error";
			}
			catch (ClientException e)
			{
                Common.StaticLog.Error(e);
                return "error";
			}
		}
		#endregion

		#region 发送阿里邮件

		public string SendAliEmail(string address, string subject, string text, string html, string file)
		{
			try
			{
				MailMessage mailMsg = new MailMessage();
				mailMsg.From = new MailAddress("all@tomail.cn-labor.cn", "蓝招网");
				mailMsg.To.Add(new MailAddress(address));
				//mailMsg.CC.Add("抄送人地址");
				//mailMsg.Bcc.Add("密送人地址");
				//可选，设置回信地址 
				//mailMsg.ReplyToList.Add("459198791@qq.com");
				mailMsg.Subject = subject;

				// 邮件主题

				// 邮件正文内容
				mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(text, null, MediaTypeNames.Text.Plain));

				if (!string.IsNullOrEmpty(html))
				{
					mailMsg.AlternateViews.Add(AlternateView.CreateAlternateViewFromString(html, null, MediaTypeNames.Text.Html));
				}

				//添加附件
				if (!string.IsNullOrEmpty(file))
				{
					string[] furl = file.Split('|');
					foreach (var url in furl)
					{
						Attachment data = new Attachment(System.IO.Directory.GetCurrentDirectory() + url, MediaTypeNames.Application.Octet);
						mailMsg.Attachments.Add(data);
					}
				}


				//邮件推送的SMTP地址和端口
				//SmtpClient smtpClient = new SmtpClient("smtpdm.aliyun.com", 25);
				//C#官方文档介绍说明不支持隐式TLS方式，即465端口，需要使用25或者80端口(ECS不支持25端口)，另外需增加一行 smtpClient.EnableSsl = true; 故使用SMTP加密方式需要修改如下：
				SmtpClient smtpClient = new SmtpClient("smtpdm.aliyun.com", 80);
				smtpClient.EnableSsl = true;
				// 使用SMTP用户名和密码进行验证
				System.Net.NetworkCredential credentials = new System.Net.NetworkCredential("all@tomail.cn-labor.cn", "LBsale123456");
				smtpClient.Credentials = credentials;
				smtpClient.Send(mailMsg);
				return "ok";
			}
			catch (Exception ex)
			{
				return "error";
			}
		}




  


        #endregion
    }
}
