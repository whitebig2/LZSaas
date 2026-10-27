using LZ.Basic.Service.Jobs;
using LZ.Basic.Service.Jobs.Basic;
using LZ.Basic.Service.Jobs.BusInsurance;
using LZ.Basic.Service.Jobs.Customer;
using LZ.Basic.Service.Jobs.Finance;
using LZ.Basic.Service.Jobs.Users;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ;
using LZ.Core.RabbitMQ.EventBus;
using Quartz;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Basic.Service.Jobs
{
    public class JobFactoryService
    {

          private readonly Dictionary<string, Func<JobBuilder>> _jobCreators = new Dictionary<string, Func<JobBuilder>>();
            public JobFactoryService()
            {
            #region Ca认证
            RegisterJobCreator("Ca认证-人脸文件获取", () => JobBuilder.Create<Customer.HandleCaFaceFileJob>());
            RegisterJobCreator("Ca认证-员工认证状态同步", () => JobBuilder.Create<HandleEntryCaAuthStatusJob>());
            #endregion

            #region 工资记录
            //RegisterJobCreator("工资记录-异常工资处理", () => JobBuilder.Create<Finance.HandleSalaryFailRecordJob>());
            //RegisterJobCreator("工资记录-系统暂缓", () => JobBuilder.Create<Finance.HandleSalaryDelayRecordJob>());
            //RegisterJobCreator("工资记录-结算单", () => JobBuilder.Create<Finance.HandleSalaryRecordSettlementJob>());
            RegisterJobCreator("工资记录-暂缓与异常任务", () => JobBuilder.Create<Finance.HandleChangeSalaryBankJob>());
            RegisterJobCreator("工资记录-现金签收单", () => JobBuilder.Create<Finance.HandleSalaryCashJob>());
            RegisterJobCreator("预发工资-现金签收单", () => JobBuilder.Create<Finance.HandlePrepaidRecordCashJob>());
            RegisterJobCreator("民生代发-暂缓代发", () => JobBuilder.Create<Finance.TransSingleSalaryRecordPushJob>());
            RegisterJobCreator("民生代发-预发工资代发", () => JobBuilder.Create<Finance.TransSinglePrepaidRecordPushJob>());
            RegisterJobCreator("民生代发-批量代发结果查询", () => JobBuilder.Create<Finance.HandleBatchTransDetailJob>());
            RegisterJobCreator("民生代发-单笔代发结果查询", () => JobBuilder.Create<Finance.HandleSingleTransDetailJob>());
            RegisterJobCreator("入离职合同-合同生成", () => JobBuilder.Create<Finance.HandleGenerateEntryContractJob>());
            RegisterJobCreator("入离职合同-发送短信", () => JobBuilder.Create<Finance.HandleEntryContractSendSmsJob>());
            RegisterJobCreator("入离职合同-合同存证", () => JobBuilder.Create<Customer.HandleEntryContractEvidenceJob>());
            RegisterJobCreator("入离职合同-离职通知单", () => JobBuilder.Create<Customer.HandleAutoLeaveJob>());
            RegisterJobCreator("系统通知-工资通知", () => JobBuilder.Create<Finance.HandleSalaryNoticeJob>());

            RegisterJobCreator("工资记录-现金发放签署状态", () => JobBuilder.Create<Finance.HandleSalaryCashStatusJob>());

            RegisterJobCreator("现金承诺函生成", () => JobBuilder.Create<HandleGenerateCashSignRecordJob>());
            RegisterJobCreator("现金承诺函存证", () => JobBuilder.Create<HandleCashSignRecordEvidenceJob>());
            #endregion

            #region 商保
            //RegisterJobCreator("商保01(阳光)-增减替换", () => JobBuilder.Create<BusinessInsuranceMatchJob>());
            //RegisterJobCreator("商保02(阳光)-推送", () => JobBuilder.Create<BusinessInsurancePushJob>());
            //RegisterJobCreator("商保03(阳光)-参保状态同步", () => JobBuilder.Create<BusinessInsurancePlanStatusJob>());


            //RegisterJobCreator("商保月末(阳光)-生成初始保全记录", () => JobBuilder.Create<GenerateUserEntryInitialRecordJob>());
            //RegisterJobCreator("商保月末(阳光)-生成初始增减记录", () => JobBuilder.Create<BusinessInsuranceGenerateByInitialRecordJob>());
            //RegisterJobCreator("商保(阳光)-邮件通知", () => JobBuilder.Create<BusinessInsuranceEmailJob>());



            //RegisterJobCreator("商保推送", () => JobBuilder.Create<BusinessPushJob>());
            //RegisterJobCreator("商保核对", () => JobBuilder.Create<BusinessCheckJob>());



            RegisterJobCreator("商保-工种方案匹配", () => JobBuilder.Create<WorkPlanMatchJob>());
            RegisterJobCreator("商保-众安保险", () => JobBuilder.Create<PushZhongAnProcessJob>());
            RegisterJobCreator("商保-阳光保险", () => JobBuilder.Create<PushYangGuangProcessJob>());
            RegisterJobCreator("商保-大地财险", () => JobBuilder.Create<PushDaDiProcessJob>());
            RegisterJobCreator("商保-安责险_T", () => JobBuilder.Create<PushAnZeTProcessJob>());
            RegisterJobCreator("商保-利宝保险", () => JobBuilder.Create<PushLiBaoProcessJob>());
            RegisterJobCreator("身故意外险", () => JobBuilder.Create<PushDeathInsuranceProcessJob>());
            RegisterJobCreator("入职记录-更新年龄", () => JobBuilder.Create<EntryAgeJob>());
            RegisterJobCreator("商保-5类", () => JobBuilder.Create<HandlePushFiveNoInsuranceJob>());

            #endregion

            #region 入职记录
            RegisterJobCreator("退休年龄计算定时任务", () => JobBuilder.Create<RetireJob>());
            RegisterJobCreator("合同续签定时任务", () => JobBuilder.Create<ContractRenewalJob>());
            RegisterJobCreator("员工参保方案匹配", () => JobBuilder.Create<HandleInsurancePlanNameByWorkNameJob>());
            #endregion

            #region 技术金融服务费
            RegisterJobCreator("技术金融服务费任务", () => JobBuilder.Create<HandleInterestRecordJob>());
            #endregion

            #region 毛利计算

            RegisterJobCreator("毛利计算定时任务", () => JobBuilder.Create<Finance.ProfitCalculateJob>());
            #endregion

            #region 预估服务费

            RegisterJobCreator("预估服务费定时任务", () => JobBuilder.Create<Finance.CustomerServiceChargeJob>());
            #endregion

            #region 销售系统相关
            RegisterJobCreator("定时清理客户负责人二维码", () => JobBuilder.Create<HandleCustomerTopSaleManJob>());
            #endregion


            #region 大屏
            RegisterJobCreator("定时清理日访问量", () => JobBuilder.Create<ClearDayTotalVisits>());
            RegisterJobCreator("定期清理工资计算次数", () => JobBuilder.Create<SalaryCalculateTimesClearJob>());


            #endregion

            #region 银行卡开卡
            RegisterJobCreator("银行卡开卡身份证水印", () => JobBuilder.Create<HandleBankOpenCardWatermarkJob>());
            #endregion


            RegisterJobCreator("测试用", () => JobBuilder.Create<Basic.Test2Job>());
            //RegisterJobCreator("测试用", () => JobBuilder.Create<Users.JobTest>());
            //RegisterJobCreator("GenerateSettlement", () => JobBuilder.Create<Customer.GenerateSettlementJob>());
            //RegisterJobCreator("JobTest", () => JobBuilder.Create<Users.JobTest>());
            //RegisterJobCreator("ContractSendSmsJobEventData", () => JobBuilder.Create<Customer.ContractSendSmsJobEventDataJob>());

        }

            public  List<string> KeyList()
            {
                List<string> keys = new List<string>();
                foreach (var key in _jobCreators)
                 {
                    keys.Add(key.Key);
                 }
                return keys;
            }
            public void RegisterJobCreator(string key, Func<JobBuilder> creator) => _jobCreators[key] = creator;
            public JobBuilder CreateJob(string key) => _jobCreators[key]();
        
    }
    
}
