using LZ.ApiService.Interface;
using LZ.Core;
using LZ.Model;
using LZ.Model.InsuranceApi.ThirtySix;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Users.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    [AllowAnonymous]
    public class TestController : ApiControllerBase
    {
        private readonly IThirtySixService _thirtySixService;
        private readonly IEntryService _entryService;
        private readonly IBusInsuracneOrderService _orderService;

        public TestController(IEntryService entryService, IThirtySixService thirtySixService, IBusInsuracneOrderService orderService)
        {
            _entryService = entryService;
            _thirtySixService = thirtySixService;
            _orderService = orderService;
        }


        [HttpGet]
        public async Task<IActionResult> LogTest()
        {
            Common.StaticLog.Debug("开始处理积分日志事件");
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> PushTest()
        {
            var records = await _entryService.ListInsurance(f => f.Name == "郑康" && f.Status == 2);
            records = records.Take(1).ToList();
            var req = records.Select(f => new ThirtySixAddRecord()
            {
                InsuredName = f.Name,
                DocumentType = "身份证",
                IdNumber = f.CardID,
                Phone = "",
                ActualInsuredCustomers = "浙江品上智能科技有限公司",
                PolicyNumber = "PZFZ26370215000000008953",
                PlanInfoName = "DD-187雇主1-3类十级10%不扩展24小时50万5万方案（含自费药）限猝50%-16-65周岁",
                Profession = "3-01-01-01_行政办事员",
                EffectiveDate = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"),
                HasSocialS = "否",
                RelatedHeightS = "否",
                Remark = "",
            }).ToList();
            var res = await _thirtySixService.PushAdd(req);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> PushReplaceTest()
        {
            var records = await _entryService.ListView(f => f.Name == "熊丽娜" && f.Status == 2);
            records = records.Take(1).ToList();
            List<ThirtySixReplaceRecord> replaceList = records.Select(f => new ThirtySixReplaceRecord()
            {
                InsuredName = f.Name,
                DocumentType = "身份证",
                IdNumber = f.CardID,
                Phone = "18668586592",
                ActualInsuredCustomers = "宁波安德机械有限公司",
                PolicyNumber = "PZFZ26370215000000008938",
                PlanInfoName = "DD-189雇主1-4类十级10%不扩展24小时80万20万方案-汽车制造行业专案-限猝50%-16-65周岁",
                Profession = "6-25-01-11_电器接插件制造工",
                EffectiveDate = DateTime.Today.AddDays(1).ToString("yyyy-MM-dd"),
                HasSocialS = "否",
                RelatedHeightS = "否",
                Remark = "",
                AttritionInsuredName = "熊丽娜",
                AttritionIdNumber = "411503198803202061",
            }).ToList();
            var res = await _thirtySixService.PushReplace(replaceList);
            return Ok(res);
        }

        [HttpGet]
        public async Task<IActionResult> QueryTest()
        {
            var res = await _thirtySixService.QueryDetail();
            return Ok(res);
        }

        /// <summary>
        /// 按订单号分组批量添加商保订单及参保方案（保费清单全量数据）
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> BatchAddBusInsuranceOrder(DateTime? orderMonth, string insuranceCompany = "大地365")
        {
            var month = orderMonth ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

            // 签章单位、订单号、参保方案、参保方案价格（同订单按方案去重，签章单位汇总到 SignUnitNames）
            var rows = new List<(string OrderNo, string InsuredName, string PlanName, decimal Price)>
            {
                ("PZFZ26370215000000008840", "宁波毅航人力资源有限公司", "DD-187雇主1-3类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-16-65周岁", 24m),
                ("PZFZ26370215000000008942", "宁波卓毅人力资源有限公司", "DD-187雇主4类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-16-65周岁", 38m),
                ("PZFZ26370215000000008953", "浙江品上智能科技有限公司", "DD-187雇主1-3类十级10%不扩展24小时50万5万方案（含自费药）限猝50%-16-65周岁", 21m),
                ("PZFZ26370215000000008955", "宁波乾鑫科技发展有限公司", "DD-187雇主4类十级10%不扩展24小时50万5万方案（含自费药）限猝50%-16-65周岁", 27m),
                ("PZFZ26370215000000008943", "宁波世诺卫浴有限公司", "DD-187雇主特3类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-保洁", 24m),
                ("PZFZ26370215000000008954", "宁波乾鑫科技发展有限公司", "DD-187雇主特3类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-保洁", 24m),
                ("PZFZ26370215000000008954", "宁波乾鑫科技发展有限公司", "DD-187雇主1-3类十级10%不扩展24小时50万5万方案（含自费药）限猝50%-16-65周岁", 21m),
                ("PZFZ26370215000000008954", "宁波乾鑫科技发展有限公司", "DD-187雇主1-3类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-16-65周岁", 24m),
                ("PZFZ26370215000000008955", "宁波乾鑫科技发展有限公司", "DD-187雇主4类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-16-65周岁", 38m),
                ("PZFZ26370215000000008953", "浙江品上智能科技有限公司", "DD-187雇主4类十级10%不扩展24小时50万5万方案（含自费药）限猝50%-16-65周岁", 27m),
                ("PZFZ26370215000000008944", "宁波募贤人力资源有限公司", "DD-187雇主4类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-16-65周岁", 38m),
                ("PZFZ26370215000000008839", "宁波金航线人力资源有限公司", "DD-187雇主1-3类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-16-65周岁", 24m),
                ("PZFZ26370215000000008941", "宁波超亿人力资源有限公司", "DD-187雇主1-3类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-16-65周岁", 24m),
                ("PZFZ26370215000000008945", "宁波超帆人力资源有限公司", "DD-187雇主4类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-16-65周岁", 38m),
                ("PZFZ26370215000000008957", "宁波登发人力资源有限公司", "DD-187雇主1-3类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-16-65周岁", 24m),
                ("PZFZ26370215000000008959", "宁波登发人力资源有限公司", "DD-187雇主4类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-16-65周岁", 38m),
                ("PZFZ26370215000000008958", "宁波登发人力资源有限公司", "DD-187雇主特4类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-保安保洁", 38m),
                ("PZFZ26370215000000008952", "蚁力（上海）人力资源有限公司", "DD-187雇主1-3类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-16-65周岁", 24m),
                ("PZFZ26370215000000008952", "蚁力（上海）人力资源有限公司", "DD-187雇主4类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-16-65周岁", 38m),
                ("PZFZ26370215000000008950", "宁波璟霖供应链管理有限公司", "DD-187雇主1-3类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-16-65周岁", 24m),
                ("PZFZ26370215000000008951", "宁波市鄞州区鼎臻海宴餐饮店（个体工商户）", "DD-187雇主1-3类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-16-65周岁", 24m),
                ("PZFZ26370215000000008841", "宁波莱博友翌供应链管理有限公司", "DD-187雇主1-3类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-16-65周岁", 24m),
                ("PZFZ26370215000000008842", "宁波职联供应链管理有限公司", "DD-187雇主1-3类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-16-65周岁", 24m),
                ("PZFZ26370215000000008949", "宁波君集企业信息咨询有限公司", "DD-187雇主4类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-16-65周岁", 38m),
                ("PZFZ26370215000000008947", "宁波新莱博金浩供应链管理有限公司", "DD-187雇主1-3类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-16-65周岁", 24m),
                ("PZFZ26370215000000008948", "嘉兴鸿博供应链管理有限公司", "DD-187雇主1-3类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-16-65周岁", 24m),
                ("PZFZ26370215000000008946", "台州蓝鹰人力资源外包服务有限公司", "DD-187雇主4类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-16-65周岁", 38m),
                ("PZFZ26370215000000008946", "台州蓝鹰人力资源外包服务有限公司", "DD-187雇主1-3类十级10%不扩展24小时90万9万方案（含自费药）限猝50%-16-65周岁", 24m),
                ("PZFZ26370215000000008938", "宁波卓毅人力资源有限公司", "DD-189雇主1-4类十级10%不扩展24小时80万20万方案-汽车制造行业专案-限猝50%-16-65周岁", 51m),
                ("PZFZ26370215000000008845", "宁波卓毅人力资源有限公司", "DD-189雇主特5类十级10%不扩展24小时80万20万方案-汽车制造行业专案-限猝50%-16-65周岁", 102m),
                ("PZFZ26370215000000008843", "浙江品上智能科技有限公司", "DD-189雇主1-4类十级10%不扩展24小时80万20万方案-汽车制造行业专案-限猝50%-16-65周岁", 51m),
                ("PZFZ26370215000000008846", "宁波莱鑫人力资源有限公司", "DD-189雇主1-4类十级10%不扩展24小时80万20万方案-汽车制造行业专案-限猝50%-16-65周岁", 51m),
                ("PZFZ26370215000000008939", "台州蓝鹰人力资源外包服务有限公司", "DD-189雇主1-4类十级10%不扩展24小时80万20万方案-汽车制造行业专案-限猝50%-16-65周岁", 51m),
                ("PZFZ26370215000000008844", "宁波超帆人力资源有限公司", "DD-189雇主1-4类十级10%不扩展24小时80万20万方案-汽车制造行业专案-限猝50%-16-65周岁", 51m),
                ("PZFZ26370215000000008838", "宁波毅航人力资源有限公司", "DD-189雇主1-4类十级10%不扩展24小时80万20万方案-汽车制造行业专案-限猝50%-16-65周岁", 51m),
                ("PZFZ26370215000000008940", "宁波新莱博金浩供应链管理有限公司", "DD-189雇主1-4类十级10%不扩展24小时80万20万方案-汽车制造行业专案-限猝50%-16-65周岁", 51m),
            };

            var orderNos = rows.Select(f => f.OrderNo).Distinct().ToList();
            var existOrders = await _orderService.List(f => orderNos.Contains(f.OrderNo));
            var existOrderNos = existOrders.Select(f => f.OrderNo).ToHashSet();

            // 订单号+被保险人+方案+保费 去重（工种不同但方案相同只保留一条）
            rows = rows.Distinct().ToList();

            var models = new List<Live_BusInsuracneOrderExtension>();
            var skipped = new List<string>();

            foreach (var orderGroup in rows.GroupBy(f => f.OrderNo))
            {
                if (existOrderNos.Contains(orderGroup.Key))
                {
                    skipped.Add(orderGroup.Key);
                    continue;
                }

                // 同订单下按方案名称去重，保费取首次出现值
                var plans = orderGroup
                    .GroupBy(f => f.PlanName)
                    .Select(g => new Live_BusInsuracneOrderPlan
                    {
                        PlanName = g.Key,
                        PlanNameID = 0,
                        PlanPriceAmount = g.First().Price,
                    })
                    .ToList();

                var signUnitNames = string.Join('、', orderGroup.Select(f => f.InsuredName).Where(f => !string.IsNullOrWhiteSpace(f)).Distinct());

                models.Add(new Live_BusInsuracneOrderExtension
                {
                    InsuranceCompany = insuranceCompany,
                    OrderNo = orderGroup.Key,
                    PolicyNum = orderGroup.Key,
                    OrderMonth = month,
                    StartDate = month,
                    EndDate = month.AddMonths(1).AddDays(-1),
                    PlanName = string.Join('、', plans.Select(f => f.PlanName)),
                    PlanNameID = plans.First().PlanNameID,
                    SignUnitNames = signUnitNames,
                    IsEnable = true,
                    CreateTime = DateTime.Now,
                    IsDayOrder = false,
                    Records = plans,
                });
            }

            if (models.Count > 0)
            {
                await _orderService.AddNav(models);
            }

            return Ok(new
            {
                rowCount = rows.Count,
                orderCount = orderNos.Count,
                added = models.Select(f => new { f.OrderNo, f.SignUnitNames, planCount = f.Records.Count }),
                skipped,
                orderMonth = month,
                insuranceCompany,
            });
        }
    }
}
