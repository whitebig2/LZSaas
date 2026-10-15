using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.ApiService.Interface
{
    public interface IMailSenderService
    {
        //Task<bool> SendUseForInsuranceAsync(string to, string subject, string body, string attachFileName, byte[] attachFileBytes, bool isHtml = false);
        //Task<bool> SendUseForInsuranceAsync(string to, string subject, string body, Dictionary<string, byte[]> attachFiles, bool isHtml = false);
        /// <summary>
        /// 商保发送邮件
        /// </summary>
        /// <param name="to">收件人</param>
        /// <param name="subject">主体</param>
        /// <param name="body">内容</param>
        /// <param name="ccMailboxes">抄送人</param>
        /// <param name="attachFiles">附件dic</param>
        /// <returns></returns>
        Task<bool> SendUseForInsuranceAsync(string to, string subject, string body, List<MailboxAddress> ccMailboxes, Dictionary<string, byte[]> attachFiles);
        /// <summary>
        /// 商保发送邮件
        /// </summary>
        /// <param name="to">收件人</param>
        /// <param name="subject">主题</param>
        /// <param name="body">内容</param>
        /// <param name="ccMailboxes">抄送人</param>
        /// <param name="attachFileName">附件文件名</param>
        /// <param name="attachFileBytes">附件文件字节</param>
        /// <returns></returns>
        Task<bool> SendUseForInsuranceAsync(string to, string subject, string body, List<MailboxAddress> ccMailboxes, string attachFileName, byte[] attachFileBytes);
    }
}
