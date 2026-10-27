using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Customer.Service.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SqlSugar;
using System.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace LZ.Customer.API
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    public class JobsController(IJobsService jobsService,ICompanyConfigService companyConfigService,IEventPublisher publisher, IHttpService httpService,ICustomerService customerService, IJobRecommendService jobRecommendService) : CustomerControllerBase
    {
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> List([FromQuery] JobPagedModel query)
        {
            var expable = await BuildJobListExpression(query);
            List<Live_V_Jobs> models;
            if (query.Latitude.HasValue && query.Longitude.HasValue)
            {
                if (query.Latitude.Value < -90 || query.Latitude.Value > 90 || query.Longitude.Value < -180 || query.Longitude.Value > 180)
                {
                    throw new ErrorCodeException(-1, "坐标不正确");
                }
                var radiusKm = query.Radius.GetValueOrDefault(10);
                if (radiusKm <= 0)
                {
                    throw new ErrorCodeException(-1, "搜索范围不正确");
                }
                if (radiusKm > 100)
                {
                    radiusKm = 100;
                }
                models = await jobsService.Nearby(expable.ToExpression(), query, query.Latitude.Value, query.Longitude.Value, radiusKm);
            }
            else
            {
                models = await jobsService.List(expable.ToExpression(), query);
            }
            return Ok(new { items = models, count = query.Count });
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Nearby([FromQuery] JobPagedModel query)
        {
            if (!query.Latitude.HasValue || !query.Longitude.HasValue)
            {
                throw new ErrorCodeException(-1, "请传入当前坐标");
            }
            if (query.Latitude.Value < -90 || query.Latitude.Value > 90 || query.Longitude.Value < -180 || query.Longitude.Value > 180)
            {
                throw new ErrorCodeException(-1, "坐标不正确");
            }
            var radiusKm = query.Radius.GetValueOrDefault(10);
            if (radiusKm <= 0)
            {
                throw new ErrorCodeException(-1, "搜索范围不正确");
            }
            if (radiusKm > 100)
            {
                radiusKm = 100;
            }
            var expable = await BuildJobListExpression(query);
            var models = await jobsService.Nearby(expable.ToExpression(), query, query.Latitude.Value, query.Longitude.Value, radiusKm);
            return Ok(new { items = models, count = query.Count });
        }

        /// <summary>
        /// 人岗匹配个性化推荐：浏览/搜索/入职行为聚合类目偏好，叠加期望工作地(实时位置)、
        /// 期望薪资、期望岗位、特种证书加权打分；硬性过滤性别/年龄/无证特种岗/在职岗位。
        /// 查询参数与 List 一致（筛选条件作为召回过滤），未登录走冷启动(期望+地理+薪资排序)。
        /// query.SID 传入时翻页顺序稳定。
        /// </summary>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Recommend([FromQuery] JobPagedModel query)
        {
            var expable = await BuildJobListExpression(query);
            var user = CurrentUser;
            var models = await jobRecommendService.Recommend(expable.ToExpression(), query, user == null ? 0 : user.UserID);
            return Ok(new { items = models, count = query.Count });
        }

        /// <summary>
        /// 反向人岗匹配：给岗位找候选人（经纪人派单场景）。
        /// 优先读离线物化清单（Live_JobCandidatePool，按 Rank 排序分页，附匹配信息：
        /// Score/Rank/WorkTypeHit 工种命中/Credibility 到岗率/SeekStatus 求职状态/Distance 距离/LastEntryWorkName 最近工种）；
        /// miss（夜批未跑/新岗位未触发实时计算）回退实时计算，此时匹配信息部分缺省（FromPool=false）。
        /// 读物化路径返回列表末尾附一个 User.UserID=0 的哨兵行，其 Rank=清单总数（分页元数据），前端应过滤掉。
        /// query.SID 传入时翻页顺序稳定。
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> RecommendUsers(int id, [FromQuery] PagedModelBase query)
        {
            var models = await jobRecommendService.RecommendUsers(id, query, 0);
            return Ok(new { items = models, count = query.Count });
        }

        /// <summary>
        /// 会员查看推荐岗位的匹配信息：返回该会员在各岗位物化清单中的匹配记录
        /// （JobID/Score 匹配分/Rank 岗位内名次/WorkTypeHit 命中工种/DistanceText 距离/LastEntryWorkName），
        /// 已自动过滤下架/过期岗位。传 jobID 只看单个岗位；不传查全部匹配岗位（按匹配分降序）。
        /// 小程序推荐列表可与 /Jobs/Recommend 的岗位列表按 JobID 关联，展示"匹配过的岗位信息"。
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> UserMatches(int jobID = 0)
        {
            var user = CurrentUser;
            if (user == null || user.UserID <= 0)
            {
                return Ok(new { items = Array.Empty<object>(), count = 0 });
            }
            var models = await jobRecommendService.GetUserMatches(user.UserID, jobID);
            return Ok(new { items = models, count = models.Count });
        }

        /// <summary>
        /// 岗位浏览/搜索埋点已独立到 VisitController（api/Visit/*），逻辑对齐商城基础服务。
        /// </summary>
        private async Task<Expressionable<Model.Live_V_Jobs>> BuildJobListExpression(JobPagedModel query)
        {
            var user = CurrentUser;
            var guid = HttpContext.Request.Headers["company-guid"].ToString();
            var expable = Expressionable.Create<Model.Live_V_Jobs>();
            if (query.CID.HasValue)
            {
                expable.And(it => it.CompanyID==query.CID);
            }
            CompanyModel companyConfig =null;
            if (!string.IsNullOrEmpty(guid))
            {
                companyConfig = await companyConfigService.Get(new Guid(guid));
            }
                expable.AndIF(query.CustomerID.HasValue, it => it.CustomerID == query.CustomerID);
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.Title.Contains(query.Name));
            }
            if (!string.IsNullOrEmpty(query.Area))
            {
                expable.And(it => it.District == query.Area);
            }
            if (!string.IsNullOrEmpty(query.City) && query.City!="不限")
            {
                expable.And(it => it.City.Contains(query.City));
            }
            if (!string.IsNullOrEmpty(query.CustomerName))
            {
                expable.And(it => it.CustomerName.Contains(query.CustomerName));
            }
            if (!string.IsNullOrEmpty(query.Money))
            {
                var ms = query.Money.Split('-');
                if (ms.Length > 1)
                {
                    if (ms[1] == "0")
                    {
                        var money = Convert.ToInt32(ms[0]);
                        expable.And(it => it.MaxMoney >= money);
                    }
                    else if (ms[0] == "0")
                    {
                        var money = Convert.ToInt32(ms[1]);
                        expable.And(it => it.MaxMoney <= money);
                    }
                    else
                    {
                        var minmoney = Convert.ToInt32(ms[0]);
                        var maxmoney = Convert.ToInt32(ms[1]);
                        expable.And(it => it.MinMoney <= maxmoney && it.MaxMoney >= minmoney);
                    }
                }
                else
                {
                    expable.And(it => it.MaxMoney >= Convert.ToInt32(query.Money.Replace("以上", "")));
                }
            }
            expable.AndIF(query.XFZShow.HasValue, it => it.XFZShow == query.XFZShow);
            expable.AndIF(query.IsDayOver.HasValue, it => it.IsDayOver == query.IsDayOver);
            expable.AndIF(query.AttendanceType.HasValue, it => it.AttendanceType == query.AttendanceType);
            var isMini = user == null || user.UserID == 0 || user.PlatformType == Model.Enums.PlatformType.XFZMini;
            if (isMini)
            {
                //var id = HttpContext.Request.Headers["client-id"].ToString();//幸福职闪聘只显示一个岗位信息
                //if (id == "1")
                //{
                //    expable.And(it => it.ID == 723);
                //}
                //else
                //{
                //    expable.AndIF(companyConfig.Config.EnableJobAudit, it => it.MinShow);
                //}
                expable.AndIF(companyConfig.Config.EnableJobAudit, it => it.MinShow);
                expable.And(it => it.XFZShow);
                
            }
            expable.AndIF(query.LWShow.HasValue, it => it.LWShow == query.LWShow);
            expable.AndIF(query.JCID.HasValue, it => it.JCID == query.JCID);
            if (!string.IsNullOrEmpty(query.Tabs))
            {
                var tabs = query.Tabs.Split(',');
                expable.And(it => tabs.Any(s => it.ShowTags.Contains(s)));
            }
            if (query.IsJoint.HasValue)
            {
                var types = new int[] { (int)Model.Enums.OperateModel.Joint, (int)Model.Enums.OperateModel.PlatformJoint };
                expable.And(it => types.Contains(it.OperateModel.Value));
            }
            if (user!=null && user.CompanyID > 0)
            {
                expable.And(it => it.CompanyID== user.CompanyID);
            }
            if (companyConfig!=null && companyConfig.Company != null)
            {
                expable.And(it => it.CompanyID == companyConfig.Company.CompanyID);
                expable.AndIF((user == null || user.UserID == 0 || user.PlatformType == Model.Enums.PlatformType.XFZMini) && companyConfig.Config.UserIsOnlyJoin, it => it.OperateModel == (int)OperateModel.Joint);
            }
            if (user != null && user.PlatformType == Model.Enums.PlatformType.LZMini && !query.IsJoint.HasValue)
            {
                expable.And(it => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == it.CustomerID && s.UserID==user.UserID && s.IsManage).Any());
            }
            return expable;
        }
        private bool VerifyJob(Model.Live_V_Jobs model)
        {
            if(!model.MinMoney.HasValue || model.MinMoney == 0)
            {
                model.MinMoney = 3000;
            }
            if (!model.MaxMoney.HasValue || model.MaxMoney == 0)
            {
                model.MinMoney = 6000;
            }
            if (!model.PCount.HasValue || model.PCount == 0)
            {
                model.PCount = 2;
            }
            if (string.IsNullOrEmpty(model.OtherAsk.Trim()))
            {
                model.OtherAsk = "有独立的自理能力";
            }
            else
            {
                var strs = new string[] {"族","岁" };
                foreach (var str in strs)
                {
                    if (model.OtherAsk.IndexOf(str)>-1)
                    {
                        throw new ErrorCodeException(-1, "该岗位已违规下架");
                    }
                }
            }
            if (string.IsNullOrEmpty(model.WorkContent.Trim()))
            {
                throw new ErrorCodeException(-1, "该岗位已违规下架");
            }
            if (!model.EndTime.HasValue || model.EndTime < DateTime.Now)//到期时自动续一个月
            {
                model.EndTime = DateTime.Now.Date.AddMonths(1);
            }
            return true;
        }
        [HttpPost]
        [OperationLogAction("新增岗位：{model.Title}")]
        public async Task<IActionResult> Add([FromBody] Model.Live_Jobs model)
        {
            var user = CurrentUser;
            model.CreateTime= DateTime.Now;
           model.ID = await jobsService.Add(model);
            await PublishPrecomputeEvent(model.ID, "add");
            return Ok(model.ID);
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpGet]
        public async Task<int> Integral(int jobId)
        {
            var model = await jobsService.Get(jobId);
            if (model == null || !model.Integrals.HasValue)
            {
                return 0;
            }
            else
            {
                return model.Integrals.Value;
            }

        }
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetCity()
        {
            var guid = HttpContext.Request.Headers["company-guid"].ToString();
            if (string.IsNullOrEmpty(guid))
            {
                throw new ErrorCodeException(-1, "来源错误");
            }
            var companyConfig = await companyConfigService.Get(new Guid(guid));
            if (companyConfig == null || companyConfig.Company==null)
            {
                throw new ErrorCodeException(-1, "来源错误！");
            }
            var list = await jobsService.GetCity(companyConfig.Company.CompanyID);
            string[] moneys = { "0-2000", "2000-3000", "3000-4000", "4000-5000", "5000-6000", "7000-8000", "8000以上" };
            return Ok(new { citys = list, moneys });
        }
        [HttpGet]
        public IActionResult GetParas()
        {
            var paras = @"{
				    'structure': ['计时', '计件'],
                    'foot': [{'name':'包1餐 '},{'name':'包2餐 '},{'name':'包3餐 '},{'name':'餐补   元','type':'edit'}, {'name':'无'}],
                    'hostel': [{'name':'免费提供'},{'name':'住宿补贴   元','type':'edit'}, {'name':'无'}],
                    'hostelCondition': [{'text':'独立卫生间','value':'独立卫生间'},{'text':'夫妻房','value':'夫妻房'},{'text':'公寓式住宿 ','value':'公寓式住宿 '}, {'text':'wifi','value':'wifi'}, {'text':'空调','value':'空调'}, {'text':'无','value':'无'}],
				    'work': ['装配', '包装', '贴片'],
				    'workMode': ['坐着', '站着','可站可坐'],
				    'workTime': [{'name':'长白班'}, {'name':'两班倒'}, {'name':'三班倒'}],
                    'restMode': [{'name':'固定休   天','type':'edit'}, {'name':'排班休   天','type':'edit'}, {'name':'自定义','type':'edit'}],
				    'coverall': ['静电服', '无尘服', '无','普通厂服'],
                    'workshop': [{'text':'噪音','value':'噪音'},{'text':'异味','value':'异味'},{'text':'无异味','value':'无异味'}, {'text':'无噪音','value':'无噪音'},{'text':'空调车间','value':'空调车间'},{'text':'恒温车间','value':'恒温车间'}, {'text':'无尘车间','value':'无尘车间'}],
				    'intensity': ['轻', '重'],
				    'physical': ['无', '先体检后报销','免费体检','需体检自费'],
				    'sex': ['男', '女','不限']
				  }";
            return Ok(paras.Replace("\r\n", "").Replace("\t", ""));
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.Live_V_Jobs> Get(int id)
        {
            var model = await jobsService.GetView(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            var guid = HttpContext.Request.Headers["company-guid"].ToString();
            if (!string.IsNullOrEmpty(guid))
            {
               var companyConfig = await companyConfigService.Get(new Guid(guid));
                if (companyConfig.Config.EnableJobAudit)
                {
                    VerifyJob(model);
                }
            }
            return model;
        }
        [HttpGet]
        public async Task<Model.Live_V_Jobs> GetView(int id)
        {
            var model = await jobsService.GetView(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpGet]
        public async Task<string> GetUserQrcode(int id)
        {
            var user = CurrentUser;
            var model = await jobsService.GetUserQrcode(id,user.UserID);
            if (model == null)
            {
                model =new Model.Live_UserJobQrcode();
                model.UserID = user.UserID;
                model.JobID = id;
                var paras = new Dictionary<string, string>();
                paras.Add("companyId", user.CompanyID.ToString());
                paras.Add("type", "xfz");
                paras.Add("appType", "mini");
                paras.Add("page", $"/pages/job/show?id={id}&from=zxshare&shareuid={user.UserID}");
                var result = await httpService.PostAsync(ServiceNames.ClientsService, "/api/clients/createqrcode", paras);
                if(result!=null && result.Code == 200)
                {
                    model.Qrcode = result.Data.ToString();
                    await jobsService.AddUserQrcode(model);
                }
                else
                {
                    throw new ErrorCodeException(-1, "生成二维码错误!");
                }
            }
            return model.Qrcode;
        }
        [HttpGet]
        public async Task<Model.Live_Jobs> GetModel(int id)
        {
            var model = await jobsService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpGet]
        public ActionResult ShareIntegral(int id, string from, int uid)
        {
            var user = CurrentUser;
            var log = new IntegralLogEventData()
            {
                CompanyID = user.CompanyID,
                CompanyGID = user.CompanyGID.Value,
                FromUserID = user.UserID,
                IntegralType = IntegralType.ShareJob,
                IsUpdate = true
            };
            if (from == "lz")
            {
                log.ToLZUserID = uid;
            }
            else
            {
                log.UserID = uid;
            }
            publisher.Publish(log);
            return Ok();
        }
        [HttpPut]
        [PlatformLimit(Model.Enums.PlatformType.Api)]
        public async Task<bool> ChangeIntegral([FromForm] int id, [FromForm] int integral)
        {
            return await jobsService.ChangeIntegral(id, integral);
        }
        [HttpPut]
        [OperationLogAction("更新岗位：{model.Title}")]
        public async Task<IActionResult> Update([FromBody] Model.Live_Jobs model)
        {
            if (!await jobsService.Exist(model.ID, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            // 修改前取旧值，用于判断是否影响匹配字段（避免改文案/备注也触发重算）
            var old = await jobsService.Get(model.ID);
            await jobsService.Update(model);
            if (AffectsMatching(old, model))
            {
                await PublishPrecomputeEvent(model.ID, "update");
            }
            return Ok();
        }

        /// <summary>
        /// 岗位候选人清单重算事件发布：公司配置 EnablePrecomputeJob=true 时，
        /// 岗位新增/修改影响匹配字段后经 MQ 异步触发单岗位重算（消费端 JobPrecomputeEventHandler）。
        /// </summary>
        private async Task PublishPrecomputeEvent(int jobId, string action)
        {
            if (jobId <= 0)
            {
                return;
            }
            var companyConfig = await companyConfigService.Get(CurrentUser.CompanyGID);
            if (companyConfig?.Config == null || !companyConfig.Config.EnablePrecomputeJob)
            {
                return;
            }
            publisher.Publish(new JobPrecomputeEventData { JobID = jobId, Action = action });
        }

        /// <summary>修改是否影响人岗匹配字段（类目/硬性条件/工作地/薪资/技能/有效期）</summary>
        private static bool AffectsMatching(Model.Live_Jobs oldModel, Model.Live_Jobs newModel)
        {
            if (oldModel == null)
            {
                return true;
            }
            return oldModel.Tabs != newModel.Tabs
                || oldModel.Sex != newModel.Sex
                || oldModel.MinAge != newModel.MinAge
                || oldModel.MaxAge != newModel.MaxAge
                || oldModel.MinMoney != newModel.MinMoney
                || oldModel.MaxMoney != newModel.MaxMoney
                || oldModel.ProvinceID != newModel.ProvinceID
                || oldModel.CityID != newModel.CityID
                || oldModel.DistrictID != newModel.DistrictID
                || oldModel.CustomerID != newModel.CustomerID
                || oldModel.SkillType != newModel.SkillType
                || oldModel.JobSkills != newModel.JobSkills
                || oldModel.Status != newModel.Status
                || oldModel.EndTime != newModel.EndTime;
        }
       
        [HttpPut]
        public async Task<IActionResult> Enable([FromForm] int id, [FromForm] string field)
        {
            var user = CurrentUser;
            if (!await jobsService.Exist(id, user.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要启用的数据不存在");
            }
            if (field == "min")
            {
                var companyConfig = await companyConfigService.Get(user.CompanyGID);
                if (companyConfig.Config.EnableJobAudit)
                {
                    var job = await jobsService.Get(id);
                    if (!job.MinMoney.HasValue || !job.MaxMoney.HasValue || job.MinMoney.Value == 0 || job.MaxMoney.Value == 0 || job.MinMoney.Value > job.MaxMoney.Value)
                    {
                        throw new ErrorCodeException(-1, "综合工资设置不正确");
                    }
                    if (!job.PCount.HasValue || job.PCount.Value <= 0)
                    {
                        throw new ErrorCodeException(-1, "岗位需求设置不正确");
                    }
                    if (string.IsNullOrEmpty(job.Payday))
                    {
                        throw new ErrorCodeException(-1, "发薪日不能为空");
                    }
                    if (string.IsNullOrEmpty(job.WorkContent))
                    {
                        throw new ErrorCodeException(-1, "工作内容不能为空");
                    }
                    //if (!job.MinAge.HasValue || !job.MaxAge.HasValue || job.MinAge.Value == 0 || job.MaxAge.Value == 0 || job.MinAge.Value > job.MaxAge.Value)
                    //{
                    //    throw new ErrorCodeException(-1, "年龄设置不正确");
                    //}
                    if (string.IsNullOrEmpty(job.Workshop))
                    {
                        throw new ErrorCodeException(-1, "车间环境不能为空");
                    }
                    if (string.IsNullOrEmpty(job.Coverall))
                    {
                        throw new ErrorCodeException(-1, "工作服不能为空");
                    }
                    if (string.IsNullOrEmpty(job.WorkMode))
                    {
                        throw new ErrorCodeException(-1, "工作方式不能为空");
                    }
                    if (string.IsNullOrEmpty(job.Structure))
                    {
                        throw new ErrorCodeException(-1, "计薪方式不能为空");
                    }
                    if (string.IsNullOrEmpty(job.Foot))
                    {
                        throw new ErrorCodeException(-1, "伙食情况不能为空");
                    }
                    if (string.IsNullOrEmpty(job.Hostel))
                    {
                        throw new ErrorCodeException(-1, "住宿情况不能为空");
                    }
                    if (string.IsNullOrEmpty(job.RestMode))
                    {
                        throw new ErrorCodeException(-1, "休息方式不能为空");
                    }
                    //if (string.IsNullOrEmpty(job.Sex))
                    //{
                    //    throw new ErrorCodeException(-1, "性别不能为空");
                    //}
                    //if (string.IsNullOrEmpty(job.WorkTime))
                    //{
                    //    throw new ErrorCodeException(-1, "上班时间不能为空");
                    //}
                    var cmodel = await customerService.Get(job.CustomerID.Value);
                    if (cmodel == null)
                    {
                        throw new ErrorCodeException(-1, "该客户不存在");
                    }
                    if (string.IsNullOrEmpty(cmodel.About) || cmodel.About.Length < 100)
                    {
                        throw new ErrorCodeException(-1, "客户介绍不能少于100个字");
                    }
                }
            }
            await jobsService.Set(id, field, true);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Disable([FromForm] int id, [FromForm] string field)
        {
            if (!await jobsService.Exist(id, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要禁用的数据不存在");
            }
            await jobsService.Set(id, field, false);
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpDelete]
        [OperationLogAction("删除岗位id：{id}")]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await jobsService.Exist(id, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await jobsService.Delete(id);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Import()
        {
            List<string> data = new List<string>();
            Guid? cgid = Guid.Parse("00000000-0000-0000-0000-000000000000");
            try
            {
                var files = Request.Form.Files;
                string fn = string.Empty;
                var filePath = "/uploadfiles/file/";
                var expable = Expressionable.Create<Model.Live_Jobs>();
                var list = await jobsService.ListImport(expable.ToExpression(), new PagedModelBase() { Page = 0 });

                foreach (var file in files)
                {
                    fn = await Common.UploadFile.UpFile(file, filePath, true);
                    if (fn.Contains("error"))
                    {
                        throw new ErrorCodeException(-1, "文件上传错误");
                    }
                }
                if (!string.IsNullOrEmpty(fn))
                {

                    DataSet? ds = new DataSet();
                    ds = Common.ExcelHeler.ReadExcelToDataSet(fn, "", 1);
                    List<Live_Jobs> models = new List<Live_Jobs>();
                    if (ds != null && ds.Tables.Count > 0)
                    {
                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            var model = list.Find(t => t.ID == Convert.ToInt32(row[0].ToString()));
                            if (model == null)
                            {
                                continue;

                            }
                            else
                            {
                                model.Title = row[2] != null ? row[2].ToString() : "";
                                model.Payday = row[3] != null ? row[3].ToString() : "";
                                model.WorkContent = row[4] != null ? row[4].ToString() : "";
                                model.Workshop = row[5] != null ? row[5].ToString() : "";
                                model.Coverall = row[6] != null ? row[6].ToString() : "";
                                model.WorkMode = row[7] != null ? row[7].ToString() : "";
                                model.Structure = row[8] != null ? row[8].ToString() : "";
                                model.Foot = row[9] != null ? row[9].ToString() : "";
                                model.Hostel = row[10] != null ? row[10].ToString() : "";
                                model.RestMode = row[11] != null ? row[11].ToString() : "";
                                models.Add(model);
                            }


                        }
                        await jobsService.UpdateList(models);
                        return Ok(new { msg = "导入成功" });

                    }

                }


            }
            catch (Exception ex)
            {
                return Ok(new { msg = ex.ToString() });
            }
            return Ok();
        }




    }
}
