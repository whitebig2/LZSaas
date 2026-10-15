using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Finance.Service.Interface
{
    public interface IPlatformNoticeService
    {
        void EmpCashNotice(int userId);
        void EmpPrepaidSalarySendResultNotice(int userId, string name, decimal amount, DateTime appyTime, string remark = "预发工资已发放");
        //void EmpPrepaidSalarySendSucNotice(int userId, string customerName);
        void EmpSalaryDelayNotice(int userId, string delayType);
        void EmpSalaryFailNotice(int userId);
        void EmpSalaryRecordSendSucNotice(int userId,  string customerName);
        void EmpSalarySlipNotice(int userId, DateTime salaryMonth, decimal amount, string customerName);
        void ManagerDefaultNotice(int partnerID, string title, string msg);
        void ManagerInvoicePrintNotice(int partnerID, string msg);
        void ManagerSalaryCustomerPaidNotice(int partnerID, string msg);
        void ManagerSalaryCustomerSendSucNotice(int partnerID, string msg);
    }
}
