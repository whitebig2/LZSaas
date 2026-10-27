using MailKit.Security;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using MailKit;
using MailKit.Net.Smtp;
using LZ.ApiService.Interface;
using LZ.Core.Interface;

namespace LZ.ApiService.Services
{
    public class MailSenderService : Interface.IMailSenderService, IScopedDependency
    {

        public static void Send(string to, string subject, string body)
        {
            var message = new MimeMessage();
            message.From.Add(new MailboxAddress("测试", "15261300969@163.com"));
            message.To.Add(new MailboxAddress("收件人名称", to));
            message.Subject = subject;
            message.Body = new TextPart("plain") { Text = body };

            using var client = new SmtpClient();
            client.Connect("smtp.163.com", 465, SecureSocketOptions.SslOnConnect);
            client.Authenticate("15261300969@163.com", "JDbJr36G43Kip9w3"); // QQ/163用授权码，不是密码
            client.Send(message);
            client.Disconnect(true);
        }

        /// <summary>
        /// 商保邮箱专用
        /// </summary>
        public async Task<bool> SendUseForInsuranceAsync(
            string to,
            string subject,
            string body,
            string attachFileName, byte[] attachFileBytes, bool isHtml = false)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("幸福职", "18668271258@163.com"));
                message.To.Add(new MailboxAddress("收件人", to));
                message.Subject = subject;
                var builder = new BodyBuilder { HtmlBody = isHtml ? body : null, TextBody = isHtml ? null : body };
                if (!string.IsNullOrWhiteSpace(attachFileName)&& attachFileBytes.Length>0)
                {
                    builder.Attachments.Add(attachFileName, attachFileBytes);
                }
                message.Body = builder.ToMessageBody();
                using var client = new SmtpClient();
                client.Connect("smtp.163.com", 465, SecureSocketOptions.SslOnConnect);
                client.Authenticate("18668271258@163.com", "ZJe3iTfiNziFPKP6"); // QQ/163用授权码，不是密码
               var res= await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }


        public async Task<bool> SendUseForInsuranceAsync(
        string to,
        string subject,
        string body,
        Dictionary<string, byte[]> attachFiles, bool isHtml = false)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("幸福职", "18668271258@163.com"));
                message.To.Add(new MailboxAddress("收件人", to));
                message.Subject = subject;
                var builder = new BodyBuilder { HtmlBody = isHtml ? body : null, TextBody = isHtml ? null : body };
                if (attachFiles!=null&& attachFiles.Count>1)
                {
                    foreach (var fileItem in attachFiles.Keys)
                    {
                        builder.Attachments.Add(fileItem, attachFiles[fileItem]);
                    }
                }
                message.Body = builder.ToMessageBody();
                using var client = new SmtpClient();
                client.Connect("smtp.163.com", 465, SecureSocketOptions.SslOnConnect);
                client.Authenticate("18668271258@163.com", "ZJe3iTfiNziFPKP6"); // QQ/163用授权码，不是密码
                var res = await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }






        public async Task<bool> SendUseForInsuranceAsync(
    string to,
    string subject,
    string body,
     List<MailboxAddress> ccMailboxes,
    string attachFileName, byte[] attachFileBytes)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("幸福职", "18668271258@163.com"));
                message.To.Add(new MailboxAddress("收件人", to));
                message.Subject = subject;

                if (ccMailboxes!=null&& ccMailboxes.Count>0)
                {
                    message.Cc.AddRange(ccMailboxes);
                }
                var builder = new BodyBuilder { HtmlBody = null, TextBody = body };
                if (!string.IsNullOrWhiteSpace(attachFileName) && attachFileBytes.Length > 0)
                {
                    builder.Attachments.Add(attachFileName, attachFileBytes);
                }
                message.Body = builder.ToMessageBody();
                using var client = new SmtpClient();
                client.Connect("smtp.163.com", 465, SecureSocketOptions.SslOnConnect);
                client.Authenticate("18668271258@163.com", "ZJe3iTfiNziFPKP6"); // QQ/163用授权码，不是密码
                var res = await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }






        public async Task<bool> SendUseForInsuranceAsync(
      string to,
      string subject,
      string body, List<MailboxAddress> ccMailboxes,  Dictionary<string, byte[]> attachFiles)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress("幸福职", "18668271258@163.com"));
                message.To.Add(new MailboxAddress("收件人", to));
                if (ccMailboxes != null && ccMailboxes.Count > 0)
                {
                    message.Cc.AddRange(ccMailboxes);
                }
                message.Subject = subject;
                var builder = new BodyBuilder { HtmlBody = null, TextBody = body };
                if (attachFiles != null && attachFiles.Count >= 1)
                {
                    foreach (var fileItem in attachFiles.Keys)
                    {
                        builder.Attachments.Add(fileItem, attachFiles[fileItem]);
                    }
                }
                message.Body = builder.ToMessageBody();
                using var client = new SmtpClient();
                client.Connect("smtp.163.com", 465, SecureSocketOptions.SslOnConnect);
                client.Authenticate("18668271258@163.com", "ZJe3iTfiNziFPKP6"); // QQ/163用授权码，不是密码
                var res = await client.SendAsync(message);
                await client.DisconnectAsync(true);

                return true;
            }
            catch (Exception ex)
            {

                return false;
            }
        }

    }
}
