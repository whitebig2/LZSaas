using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IBusInsuracneService
    {
        Task GenerateBusinsuranceRecordByApi();
        Task GenerateTestRecord();
        Task<bool> JobAddOrUpdate(int customerId, string workName, string workClass);
        Task<List<Live_EntryJob>> ListEntryJob();
        Task MatchByInsuranceCompany(string insuranceCompany);
        Task MatchByOrderNo(string insuranceCompany,string orderNo,bool isGenerateDayMatchRecord=true);
   
        Task PushAnZeT();
        Task PushAnZeTProcess();
        Task PushCompleted(string insuranceCompany);
        Task PushDaDi();
        Task PushDaDiProcess();
        Task PushLiBao();
        Task PushLiBaoProcess();
        Task PushYangGuang();
        Task PushYangGuangProcess();
        Task PushZhongAn();
        Task PushZhongAnProcess();
        Task SendEmailTest();
        Task WorkPlanMatch();
        Task WorkPlanMatchByInsuranceCompany(string insuranceCompany);
        Task YangGuangPushResultNoticeByEmail();
        Task FiveNoInsurancePush();
        Task PushDaDi365Process();
        Task PushCompleted36k(List<int> dayMatchRecordIds, List<int> delRecordIds);
    }
}
