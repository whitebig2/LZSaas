using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Finance.Service.Interface;
using LZ.Model.Enums;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Services
{
    public class PlatformNoticeService : IPlatformNoticeService, IScopedDependency
    {

        private readonly IEventPublisher _eventPublisher;

        public PlatformNoticeService(IEventPublisher eventPublisher)
        {
            _eventPublisher = eventPublisher;
        }

        /// <summary>
        /// 发票已开具通知
        /// </summary>
        /// <param name="partnerID"></param>
        /// <param name="title"></param>
        /// <param name="msg"></param>
        public void ManagerInvoicePrintNotice(int partnerID,string msg)
        {


            var sendNotice1 = new PartnerSendNoticeEventData()
            {
                PartnerID = partnerID,
                Title = "发票已开具",
                Message = msg,
                UserType = PersonnelType.Headed,
            };
            _eventPublisher.Publish(sendNotice1);
            var sendNotice2 = new PartnerSendNoticeEventData()
            {
                PartnerID = partnerID,
                Title = "发票已开具",
                Message = msg,
                UserType = PersonnelType.Factory,
            };
            _eventPublisher.Publish(sendNotice2);

        }

        /// <summary>
        ///  客户已打款通知
        /// </summary>
        /// <param name="partnerID"></param>
        /// <param name="msg"></param>
        public void ManagerSalaryCustomerPaidNotice(int partnerID, string msg)
        {


            var sendNotice1 = new PartnerSendNoticeEventData()
            {
                PartnerID = partnerID,
                Title = "发票已开具",
                Message = msg,
                UserType = PersonnelType.Headed,
            };
            _eventPublisher.Publish(sendNotice1);
            var sendNotice2 = new PartnerSendNoticeEventData()
            {
                PartnerID = partnerID,
                Title = "发票已开具",
                Message = msg,
                UserType = PersonnelType.Factory,
            };
            _eventPublisher.Publish(sendNotice2);

        }


        /// <summary>
        ///  客户已打款通知
        /// </summary>
        /// <param name="partnerID"></param>
        /// <param name="msg"></param>
        public void ManagerSalaryCustomerSendSucNotice(int partnerID, string msg)
        {


            var sendNotice1 = new PartnerSendNoticeEventData()
            {
                PartnerID = partnerID,
                Title = "工资已发放",
                Message = msg,
                UserType = PersonnelType.Headed,
            };
            _eventPublisher.Publish(sendNotice1);
            var sendNotice2 = new PartnerSendNoticeEventData()
            {
                PartnerID = partnerID,
                Title = "工资已发放",
                Message = msg,
                UserType = PersonnelType.Factory,
            };
            _eventPublisher.Publish(sendNotice2);

        }




        public void ManagerDefaultNotice(int partnerID,string title,string msg)
        {

            
            var sendNotice1 = new PartnerSendNoticeEventData()
            {
                PartnerID = partnerID,
                Title = title,
                Message = msg,
                UserType = PersonnelType.Headed,
            };
            _eventPublisher.Publish(sendNotice1);
            var sendNotice2 = new PartnerSendNoticeEventData()
            {
                PartnerID = partnerID,
                Title = title,
                Message = msg,
                UserType = PersonnelType.Factory,
            };
            _eventPublisher.Publish(sendNotice2);

        }

        public void EmpSalaryFailNotice(int userId)
        {
            SendNoticeEventData sendNotice = new SendNoticeEventData()
            {
                UserID = userId,
                Keyword1 = "工资发放异常",
                Keyword2 = "待处理",
                Keyword3 = $"请修改银行卡信息",
                Keyword4 = DateTime.Now.ToString("yyyy-MM-dd"),
                NoticeType = NoticeEnumType.Remind,
                Platform = PlatformType.XFZMini,
                SendType = NoticeSendEnumType.All,
                Page = "pages/salaryFailRecord/index"
            };
            _eventPublisher.Publish(sendNotice);
        }
        public void EmpCashNotice(int userId)
        {
            SendNoticeEventData sendNotice = new SendNoticeEventData()
            {
                UserID = userId,
                Keyword1 = "现金签收单",
                Keyword2 = "待签署",
                Keyword3 = $"请前往幸福职小程序签署",
                Keyword4 = DateTime.Now.ToString("yyyy-MM-dd"),
                NoticeType = NoticeEnumType.Remind,
                Platform = PlatformType.XFZMini,
                SendType = NoticeSendEnumType.All,
                Page = "pages/cashContract/index"
            };
            _eventPublisher.Publish(sendNotice);
        }


        public void EmpSalaryDelayNotice(int userId,string delayType)
        {
            SendNoticeEventData sendNotice = new SendNoticeEventData()
            {
                UserID = userId,
                Keyword1 = "工资已暂缓",
                Keyword2 = "待处理",
                Keyword3 = $"暂缓类型：{delayType}",
                Keyword4 = DateTime.Now.ToString("yyyy-MM-dd"),
                NoticeType = NoticeEnumType.Remind,
                Platform = PlatformType.XFZMini,
                SendType = NoticeSendEnumType.All,
            };
            _eventPublisher.Publish(sendNotice);
        }

        public void EmpSalarySlipNotice(int userId, DateTime salaryMonth, decimal amount, string customerName)
        {

            if (userId > 0)
            {
                SendNoticeEventData sendNotice = new SendNoticeEventData()
                {
                    UserID = userId,
                    Keyword1 = salaryMonth.ToString("yyyy-MM-dd"),
                    Keyword2 = "工资条已生成，请及时查看",
                    Keyword3 = customerName,
                    NoticeType = NoticeEnumType.UserPayslip,
                    Platform = PlatformType.XFZMini,
                    SendType = NoticeSendEnumType.All,
                    Page = "pagesEmployee/employeeSlipRecord/index"
                };

                _eventPublisher.Publish(sendNotice);
            }

        }

        /// <summary>
        /// 预发工资已发放
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="customerName"></param>
        public void EmpPrepaidSalarySendResultNotice(int userId,string name,decimal amount,  DateTime appyTime,string remark="预发工资已发放")
        {
            //SendNoticeEventData sendNotice = new SendNoticeEventData()
            //{
            //    UserID = userId,
            //    Keyword1 =name,
            //    Keyword2 = amount.ToString(),
            //    Keyword3 = appyTime.ToString("yyyy-MM-dd"),
            //    Keyword4 = remark,
            //    NoticeType = NoticeEnumType.UserAdvance,
            //    Platform = PlatformType.XFZMini,
            //    SendType = NoticeSendEnumType.All,
            //};

            SendNoticeEventData sendNotice = new SendNoticeEventData()
            {
                UserID = userId,
                Keyword1 = "预发发放通知",
                Keyword2 = remark.Contains("失败")?"发放失败":"发放成功",
                Keyword3 = remark,
                Keyword4 = DateTime.Now.ToString("yyyy-MM-dd"),
                NoticeType = NoticeEnumType.Remind,
                Platform = PlatformType.XFZMini,
                SendType = NoticeSendEnumType.All,
                Page = "pagesUser/user/usermoney"
            };

            //if ()
            //{
            //    sendNotice.key
            //}

            _eventPublisher.Publish(sendNotice);
        }
        /// <summary>
        /// 工资已发放
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="customerName"></param>
        public void EmpSalaryRecordSendSucNotice(int userId, string customerName)
        {
            SendNoticeEventData sendNotice = new SendNoticeEventData()
            {
                UserID = userId,
                Keyword1 = DateTime.Now.ToString("yyyy-MM-dd"),
                Keyword2 = $"当月工资工资已发放，请查收。",
                Keyword3 = customerName,
                NoticeType = NoticeEnumType.UserPayroll,
                Platform = PlatformType.XFZMini,
                SendType = NoticeSendEnumType.All,
                Page = "pagesEmployee/employeeSlipRecord/index"
            };
            _eventPublisher.Publish(sendNotice);
        }


    }
}
