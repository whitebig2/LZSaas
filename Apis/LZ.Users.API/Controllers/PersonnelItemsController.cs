using LZ.ApiService.Interface;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Enums.Sale;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;

namespace LZ.Users.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    public class PersonnelItemsController(IPersonnelItemsService personnelItemsService, IEventPublisher publisher,IIMSService iMSService, IHttpService httpService,IPersonnelService personnelService,IEntryService entryService,ISendMQService sendMQService,IJDYSyncService jdySyncService) : UsersControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] UserPagedModel query)
        {
            var user = CurrentUser;
            var expable = Expressionable.Create<Model.Live_V_PersonnelItems>();
            if (query.Enabled.HasValue)
            {
                expable.And(it => it.Enabled == query.Enabled);
            }
            if (query.UserID.HasValue)
            {
                expable.And(it => it.UserID == query.UserID);
            }
            else if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                expable.And(it => it.UserID == user.UserID);
            }
            if (query.CID.HasValue)
            {
                expable.And(it => it.CID==query.CID);
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => query.Name.Contains(it.CustomerName));
            }
            if (!string.IsNullOrEmpty(query.Type))
            {
                expable.And(it => it.UserType == Convert.ToInt32(query.Type));
            }
            if(!string.IsNullOrEmpty(query.Types))
            {
                var arr = query.Types.Split(',');
                expable.And(it => arr.Contains(it.UserType.ToString()));
            }

            expable.AndIF(query.IsManage.HasValue, it => it.IsManage == query.IsManage);
            var exp = expable.ToExpression();
            var models = await personnelItemsService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_PersonnelItems model)
        {
            var expable = Expressionable.Create<Model.Live_V_PersonnelItems>();
            var user = CurrentUser;
            if (user.PlatformType != Model.Enums.PlatformType.Admin && user.PlatformType!=Model.Enums.PlatformType.Api)
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            expable.And(it => it.UserID == model.UserID);
            expable.And(it => it.CID == model.CID);
            var exp = expable.ToExpression();
            if (await personnelItemsService.Exist(exp))
            {
                throw new ErrorCodeException(-1, "该关联已存在!");
            }
            model.GID = Guid.NewGuid();
            model.ID = await personnelItemsService.Add(model);
            if (model.ID > 0)
            {
                try
                {
                   var res = await iMSService.JoinGroup(model.UserID.Value, 2, model.CID.Value);
                    Common.StaticLog.Debug(res);
                }
                catch (Exception ex) { 
                    Common.StaticLog.Error(ex);
                }
                
            }
            return Ok();
        }
      
        [HttpGet]
        [PlatformLimit(Model.Enums.PlatformType.Api)]
        public async Task<Model.Live_V_PersonnelItems> Get(int id)
        {
            return await personnelItemsService.Get(id, CurrentUser.CompanyID);
        }
        [HttpPost]
        [PlatformLimit(Model.Enums.PlatformType.Api)]
        public async Task<IActionResult> BatchSendQrCodeEvent(int companyId)
        {
            var models = await personnelItemsService.List(it=>it.CompanyID== companyId,new Model.PagedModel.UserPagedModel() { Page=0});
            foreach(var m in models)
            {
                publisher.Publish(new PersonnelQrcodeEventData()
                {
                    FromID = m.ID
                });
                Thread.Sleep(200);
            }
           return  Ok();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.Live_V_PersonnelItems> GetInfo(Guid id, string city="")
        {
            if (!string.IsNullOrEmpty(city))
            {
                sendMQService.SendStatisticsMessage("{\"type\":\"scanCode\",\"city\":\""+ city + "\"}");
            }
            return await personnelItemsService.Get(id);
        }

        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_PersonnelItems model)
        {
            if (!await personnelItemsService.Exist(model.ID, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await personnelItemsService.Update(model);
            return Ok();
        }
       
        [HttpPut]
        [PlatformLimit(Model.Enums.PlatformType.Api)]
        public async Task<IActionResult> SaveQrcode([FromForm] int id, [FromForm] string qrcode)
        {
            
           var has = await personnelItemsService.SaveQrcode(id, qrcode);
            if (has)
            {
                await jdySyncService.PersonnelSync(id);
                return Ok();
            }
            else
            {
                throw new ErrorCodeException(-1, "保存失败");
            }
        }
        [HttpPut]
        public async Task<IActionResult> Enable([FromForm] int id)
        {
            if (!await personnelItemsService.Exist(id, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要启用的数据不存在");
            }
            await personnelItemsService.Set(id, true);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Manage([FromForm] int id, [FromForm] bool has)
        {
            if (!await personnelItemsService.Exist(id, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要更新的数据不存在");
            }
            await personnelItemsService.SetManage(id, has);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> Disable([FromForm] int id)
        {
            if (!await personnelItemsService.Exist(id, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要禁用的数据不存在");
            }
            await personnelItemsService.Set(id, false);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await personnelItemsService.Exist(id, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await personnelItemsService.Delete(id);
            return Ok();
        }
        [HttpGet]
        public async Task<IActionResult> SaleQRList(int cid)
        {
            List<Model.SalePersonnelModel> list = new List<Model.SalePersonnelModel>();
            list = await personnelItemsService.SalePersonnelList(cid);
            return Ok(new { items = list,cid });
        }
        [HttpPost]
        public async Task<IActionResult> SaleBatchQR([FromForm] string phones, [FromForm]int customerID)
        {
            List<Model.Live_PersonnelItems> list = new List<Model.Live_PersonnelItems>();
            list = await personnelItemsService.SaleBatchQR(phones,customerID);
            return Ok(new { items = list });
        }

        /// <summary>
        /// 销售系统
        /// 一个月保存前三销售员
        /// </summary>
        /// <returns></returns>
        /// 
        [HttpPost]
        public async Task<IActionResult> HandleCustomerTopSaleMan([FromForm] int cid)
        {
            List<Model.Model.SaleCustomerModel> test = new List<Model.Model.SaleCustomerModel>();
            string msg = "";
            //var date = DateTime.Now.AddDays(-30);
            var paras = new Dictionary<string, string>();
            paras.Add("Page", "0");
            paras.Add("From", "sale");
            paras.Add("SaleID", cid.ToString());
            var obj = await httpService.GetAsync(ServiceNames.CustomerService, "/api/customer/list", paras);
            List<Model.Live_V_Customer> customers = new List<Live_V_Customer>();
            try
            {


               
                if (!string.IsNullOrEmpty(obj.Data.ToString()))
                {
                    var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
                    customers = JsonConvert.DeserializeObject<List<Model.Live_V_Customer>>(ii["items"].ToString());
                }
            }
            catch (Exception e)
            {
                return Ok(new { msg = "获取customer错误", obj = obj.Data });
            }
            List<Live_PersonnelItems> items = new List<Live_PersonnelItems>();

            if (customers.Count > 0)
            {
                List<int?> arr = new List<int?>();
                foreach (var x in customers)
                {
                    arr.Add(x.CustomerID);
                }
                UserPagedModel query = new UserPagedModel();
                query.Page = 0;

                var expable_Personnel = Expressionable.Create<Model.Live_V_Personnel>();
                expable_Personnel.And(t => t.UserType == 4);
                var personnelList = await personnelService.List(expable_Personnel.ToExpression(), query);

                var expable_PersonnelItems = Expressionable.Create<Model.Live_PersonnelItems>();
                expable_PersonnelItems.And(t => arr.Contains(t.CID));
                var personnelItems = await personnelItemsService.ListModel(expable_PersonnelItems.ToExpression(), query);

                var expable_UserEntry = Expressionable.Create<Model.Live_V_UserEntry>();
                expable_UserEntry.And(t => arr.Contains(t.CustomerID));
                var entrys = await entryService.List(expable_UserEntry.ToExpression(), query);


                foreach (var x in customers)
                {
                    try
                    {
                        List<Model.Model.SaleCustomerModel> sales = new List<Model.Model.SaleCustomerModel>();
                        var pitems = personnelItems.Where(t => t.CID == x.CustomerID && t.IsManage).ToList();
                        var pitems_joint = personnelItems.Where(t => t.CID == x.CustomerID && !t.IsManage).ToList();
                        DateTime endTime = DateTime.Today;
                        if (x.CreateTime.HasValue)
                        {
                            endTime = Convert.ToDateTime(x.CreateTime.Value.ToString("yyyy-MM-dd")).AddDays(31);
                        }
                        foreach (var p in pitems)
                        {
                            var personnel = personnelList.Find(t => t.UserID == p.UserID);
                            if (personnel != null)
                            {
                                Model.Model.SaleCustomerModel sale = new Model.Model.SaleCustomerModel();
                                sale.CustomerID = x.CustomerID;
                                sale.PersonnelID = p.UserID;
                                sale.EntryCount = entrys.Where(t => t.CustomerID == x.CustomerID && t.PerPartnerID == personnel.PartnerID && t.EntryTime < endTime).ToList().Count();//入职员工的找人账号所属服务商
                                sales.Add(sale);
                                test.Add(sale);
                            }
                            

                        }
                        var newSales = sales.Where(t => t.EntryCount > 0).OrderByDescending(t => t.EntryCount).ToList();
                        List<Model.Model.SaleCustomerModel> deleteSales = new List<Model.Model.SaleCustomerModel>();
                        deleteSales = sales;
                        Dictionary<string, string> dics = new Dictionary<string, string>();
                        dics.Add("id", x.CustomerID.ToString());
                        if (newSales.Count == 0)//所有负责人入职员工人数都为0
                        {
                            dics.Add("saleStatus", ((int)SaleCustomerStatusEnum.分配异常).ToString());
                            dics.Add("saleContent", "所有服务商人数为0");
                            msg = "所有服务商人数为0";

                        }
                        else if (newSales.Count() <= 3)
                        {

                            deleteSales = sales.Except(newSales).ToList();
                            dics.Add("saleStatus", ((int)SaleCustomerStatusEnum.分配成功).ToString());
                            dics.Add("saleContent", "分配成功");
                            msg = "分配成功";
                        }
                        else
                        {
                            if (newSales[2].EntryCount != newSales[3].EntryCount)
                            {
                                newSales = newSales.Take(3).ToList();
                                deleteSales = sales.Except(newSales).ToList();
                                dics.Add("saleStatus", ((int)SaleCustomerStatusEnum.分配成功).ToString());
                                dics.Add("saleContent", "分配成功");
                                msg = "分配成功";
                            }
                            else
                            {
                                dics.Add("saleStatus", ((int)SaleCustomerStatusEnum.分配异常).ToString());
                                dics.Add("saleContent", "无法正常获取前三个服务商负责人");
                                msg = "无法正常获取前三个服务商负责人";
                            }
                        }
                        await httpService.PostAsync(ServiceNames.CustomerService, "/api/customer/SaleCustomerUpdate", dics);//最终上线开放
                        foreach (var ds in deleteSales)
                        {
                            var model = pitems.Find(t => t.UserID == ds.PersonnelID && t.CID == ds.CustomerID);
                            if (model != null)
                            {
                                items.Add(model);
                            }
                        }
                        items.AddRange(pitems_joint);
                    }
                    catch(Exception e)
                    {
                        return Ok(new {msg = e.ToString()});
                    }

                }
                //return Ok(new { customers, personnelItems, entrys, items, test, msg });//测试用
            }
            else
            {
                msg = "不存在客户单位";
                return Ok(new {msg});
            }
            await personnelItemsService.DeleteList(items);//删除多余的二维码，最终上线开放
            return Ok(new { msg, test });
        }

    }
}
