using LZ.ApiService.Interface;
using LZ.Common;
using LZ.Core.Attributes;
using LZ.Core.EventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Customer.Service.Interface;
using LZ.Model;
using LZ.Model.Dtos;
using LZ.Model.Enums;
using LZ.Model.Enums.Attendance;
using LZ.Model.Enums.Sale;
using LZ.Model.Exceptions;
using LZ.Model.IExcelTemplate;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using SqlSugar;
using System.ComponentModel.Design;
using System.Data;
using System.Security.Policy;

namespace LZ.Customer.API
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class CustomerController(ICustomerService customerService, IAuthService authService, IHttpService httpService, IPermissionService permissionService, IFileUploadService fileUploadService, IIMSService iMSService, IEnterpriseService enterpriseService,IFxqService fxqService,IFxqDevService fxqDevService,IContractFileConfigService contractFileConfigService, IEventPublisher eventPublisher,IAliyunOSSService aliyunOSSService,IDayWorkCustomerContractRecordService dayWorkCustomerContractRecordService,IContractSignerService contractSignerService,IJobsDayWorkService jobsDayWorkService,IDayWorkCustomerContractRecordService dayWorkCustomerContractRecordService1,ICompanyConfigService companyConfigService,ISettingsService settingsService) : CustomerControllerBase
    {
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [PlatformLimit(Model.Enums.PlatformType.Api)]
        [HttpGet]
        public async Task<IActionResult> List(string cityPartnerTitle, string salesman, [FromQuery] CustomerPagedModel query)
        {
            var user = CurrentUser;
            var info = await authService.UserInfo<Model.SYS_CompanyUser>();
            var expable = Expressionable.Create<Model.Live_V_Customer>();
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.CustomerName.Contains(query.Name));
            }
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            //搜索未完善的 客户信息
            expable.AndIF(query.IsPerfect.HasValue && !query.IsPerfect.Value, it => SqlFunc.IsNullOrEmpty(it.Address) || SqlFunc.IsNullOrEmpty(it.CompanyNo) || SqlFunc.IsNullOrEmpty(it.Bank) || SqlFunc.IsNullOrEmpty(it.BankAccount) || SqlFunc.IsNullOrEmpty(it.InvoiceName) || it.CompanyNo == "1" || it.Bank == "1" || it.BankAccount == "1" || it.InvoiceName == "1");

            if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var puser = await authService.UserInfo<Model.SYS_CompanyUser>();
                var auth = await permissionService.CompanyLicenseList("customers", "manage,show,service", user.UserID);
                if ((!auth[0] && !auth[1] && !auth[2]) || puser.UserType == (int)CompanyUserType.Partner)
                {

                    if (puser.PartnerID.HasValue)
                    {
                        expable.And(it => it.PartnerID == puser.PartnerID);
                    }
                }
            }
            if (query.From == "PartnerStatistics" && query.PartnerID > 0)
            {
                //expable.And(it => it.CustomerID == SqlFunc.Subqueryable<Model.Live_UserEntry>().Where(s => s.PartnerID == query.PartnerID && s.Status == (int)UserStatus.Entry).Select(s => SqlFunc.AggregateDistinctCount(s.CustomerID)));
                expable.And(it=>SqlFunc.Subqueryable<Model.Live_UserEntry>().Where(s => s.CustomerID == it.CustomerID && s.Status==(int)UserStatus.Entry && s.PartnerID == query.PartnerID).Any());
            }
            else
            {
                expable.AndIF(query.PartnerID > 0, it => it.PartnerID == query.PartnerID);
            }

            expable.AndIF(!string.IsNullOrEmpty(query.PartnerName), it => it.PartnerName.Contains(query.PartnerName));
            expable.AndIF(!string.IsNullOrEmpty(query.CooperationUnitGID), it => it.CooperationUnitGID == Guid.Parse(query.CooperationUnitGID));
            expable.AndIF(query.AttendanceType.HasValue, it => it.AttendanceType == query.AttendanceType);
            expable.AndIF(query.Enabled.HasValue, it => it.Enabled == query.Enabled);
            if (query.From == "sale")
            {
                expable.And(it => it.CustomerID == query.SaleID);
            }
            //if (query.From=="partner" && info.UserType==2)
            //{
            //    Dictionary<string, string> dic = new Dictionary<string, string>();
            //    dic.Add("id", info.PartnerID.ToString());
            //    var obj = await httpService.GetAsync(ServiceNames.BasicService, "/api/partner/get", dic);
            //    Model.Live_Partner customer = new Live_Partner();
            //    if (!string.IsNullOrEmpty(obj.Data.ToString()))
            //    {
            //        var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
            //        customer = JsonConvert.DeserializeObject<Model.Live_Partner>(ii.ToString());
            //    }
            //    if(customer!=null)
            //    {
            //        var arr1 = customer.CIDS.Split(',');
            //        expable.And(it => arr1.Contains(it.CustomerID.ToString()));
            //    }
            //}

            if (!string.IsNullOrEmpty(query.IDS))
            {
                var arr = query.IDS.Split(',');
                expable.And(it => arr.Contains(it.CustomerID.ToString()));
            }
            // 城市合伙人
            expable.AndIF(!string.IsNullOrWhiteSpace(cityPartnerTitle), f => f.CityPartnerTitle.Contains(cityPartnerTitle));
            expable.AndIF(!string.IsNullOrWhiteSpace(salesman), f => f.SalesMan.Contains(salesman));
            var exp = expable.ToExpression();
            var models = await customerService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }



        [AllowAnonymous]
        [HttpPost]
        [OperationLogAction("注册客户：{reg.CompanyName}")]
        public async Task<IActionResult> Register([FromBody] RegisterModel reg)
        {
            if (string.IsNullOrEmpty(reg.CompanyName) || string.IsNullOrEmpty(reg.Contact) || string.IsNullOrEmpty(reg.Phone) || string.IsNullOrEmpty(reg.CompanyNo))
            {
                throw new ErrorCodeException(-1, "请正确填写信息！");
            }
            if (reg.Phone.Length != 11)
            {
                throw new ErrorCodeException(-1, "手机号错误！");
            }
            var model = new Model.Live_Customer();
            model.CustomerName= reg.CompanyName;
            model.CustomerGID= Guid.NewGuid();
            model.Contact= reg.Contact;
            model.Phone= reg.Phone;
            model.CompanyNo= reg.CompanyNo;
            model.Industry= reg.Industry;
            var setting = await settingsService.GetMasterSettings();
            model.CompanyGID = new Guid(setting.DefaultCompanyGID);
            model.CompanyID = setting.DefaultCompanyID;
            if (await customerService.Exist(it => (it.CustomerName == model.CustomerName || it.CompanyNo == model.CompanyNo) && it.CompanyID==model.CompanyID))
            {
                throw new ErrorCodeException(-1, "该公司名称或统一社会信用代码‌已被注册，请联系客服！");
            }
            var phoneRes = await httpService.GetAsync(ServiceNames.UsersService, $"/api/personnel/existphone?phone={model.Phone}");
            if (phoneRes != null && phoneRes.Code == 200 && Convert.ToBoolean(phoneRes.Data))
            {
                throw new ErrorCodeException(-1, "该手机号已被注册！");
            }
            model.CreateTime = DateTime.Now;
            model.Status = 1;
            model.CustomerID = await customerService.Add(model);
            var record = new PartnerEventData();
            record.PartnerName = model.CustomerName + "-默认服务商";
            record.CompanyID = model.CompanyID.Value;
            record.CompanyGID = model.CompanyGID.Value;
            record.CustomerID = model.CustomerID;
            record.CustomerGID = model.CustomerGID.Value;
            record.FullName = model.Contact;
            record.PartnerType = (int)Model.Enums.PartnerType.Ordinary;
            record.Phone = model.Phone;
            eventPublisher.Publish(record);
            return Ok();
        }

        [HttpGet]
        public async Task<IActionResult> ExportQueryList(string cityPartnerTitle, string salesman, [FromQuery] CustomerPagedModel query)
        {

            var user = CurrentUser;
            var info = await authService.UserInfo<Model.SYS_CompanyUser>();
            var expable = Expressionable.Create<Model.Live_V_Customer>();
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.CustomerName.Contains(query.Name));
            }
            if (user.PlatformType == Model.Enums.PlatformType.Admin || user.PlatformType == Model.Enums.PlatformType.Api)
            {
                expable.AndIF(query.CompanyID.HasValue && query.CompanyID > 0, it => it.CompanyID == query.CompanyID);
            }
            else
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            //搜索未完善的 客户信息
            expable.AndIF(query.IsPerfect.HasValue && !query.IsPerfect.Value, it => SqlFunc.IsNullOrEmpty(it.Address) || SqlFunc.IsNullOrEmpty(it.CompanyNo) || SqlFunc.IsNullOrEmpty(it.Bank) || SqlFunc.IsNullOrEmpty(it.BankAccount) || SqlFunc.IsNullOrEmpty(it.InvoiceName) || it.CompanyNo == "1" || it.Bank == "1" || it.BankAccount == "1" || it.InvoiceName == "1");

            if (user.PlatformType == Model.Enums.PlatformType.MiniAdmin)
            {
                var puser = await authService.UserInfo<Model.SYS_CompanyUser>();
                var auth = await permissionService.CompanyLicenseList("customers", "manage,show,service", user.UserID);
                if ((!auth[0] && !auth[1] && !auth[2]) || puser.UserType == (int)CompanyUserType.Partner)
                {

                    if (puser.PartnerID.HasValue)
                    {
                        expable.And(it => it.PartnerID == puser.PartnerID);
                    }
                }
            }
            if (query.From == "PartnerStatistics" && query.PartnerID > 0)
            {
                expable.And(it => SqlFunc.Subqueryable<Model.Live_UserEntry>().Where(s => s.CustomerID == it.CustomerID && s.Status == (int)UserStatus.Entry && s.PartnerID == query.PartnerID).Any());
            }
            else
            {
                expable.AndIF(query.PartnerID > 0, it => it.PartnerID == query.PartnerID);
            }

            expable.AndIF(!string.IsNullOrEmpty(query.PartnerName), it => it.PartnerName.Contains(query.PartnerName));
            expable.AndIF(!string.IsNullOrEmpty(query.CooperationUnitGID), it => it.CooperationUnitGID == Guid.Parse(query.CooperationUnitGID));
            expable.AndIF(query.AttendanceType.HasValue, it => it.AttendanceType == query.AttendanceType);
            expable.AndIF(query.Enabled.HasValue, it => it.Enabled == query.Enabled);
            if (query.From == "sale")
            {
                expable.And(it => it.CustomerID == query.SaleID);
            }
         

            if (!string.IsNullOrEmpty(query.IDS))
            {
                var arr = query.IDS.Split(',');
                expable.And(it => arr.Contains(it.CustomerID.ToString()));
            }
            // 城市合伙人
            expable.AndIF(!string.IsNullOrWhiteSpace(cityPartnerTitle), f => f.CityPartnerTitle.Contains(cityPartnerTitle));
            expable.AndIF(!string.IsNullOrWhiteSpace(salesman), f => f.SalesMan.Contains(salesman));
            var exp = expable.ToExpression();
            query.Page = 0;
            var dataList = await customerService.List(exp, query);


            var exportDataList = dataList.Select(f => new CustomerExcelTemplate()
            {
                SigningUnitName = f.SigningUnitName,
                CooperationUnitName = f.CooperationUnitName,
                PartnerName = f.PartnerName,
                CompanyAddress = f.Address,
                CompanyNo = f.CompanyNo,
                CustomerName = f.CustomerName,
                ProviceAndCity = $"{f.Province ?? string.Empty}/{f.City ?? string.Empty}"
            }).ToList();
            var fileBytes = NpoiExcelHelper.WriteExcel(exportDataList, "客户信息");
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "客户信息.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }




        [HttpPost]
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [PlatformLimit(Model.Enums.PlatformType.Api)]
        [OperationLogAction("新增客户：{model.CustomerName}")]
        public async Task<IActionResult> Add([FromBody] Model.Live_Customer model)
        {
            var user = CurrentUser;
            if (user.PlatformType != PlatformType.Admin)
            {
                model.CompanyGID = user.CompanyGID;
                model.CompanyID = user.CompanyID;
            }
            model.CreateTime = DateTime.Now;
            model.CustomerGID = Guid.NewGuid();
            model.OperateModel = (int)OperateModel.Routine;
            model.SaleStatus = (int)SaleCustomerStatusEnum.非销售系统客户;
            model.Enabled = true;
            if (!model.AttendanceType.HasValue) model.AttendanceType = 1;
            if (!model.BusinessModel.HasValue) model.BusinessModel = (int)Model.Enums.BusinessModel.Labor;

            // 客户单位名称已存在则禁止添加
            if (await customerService.Exist(it => it.CustomerName == model.CustomerName && it.CompanyID == model.CompanyID))
            {
                throw new ErrorCodeException(-1, $"{model.CustomerName} 客户单位信息已存在，无法重复添加！");
            }
            if (user.PlatformType == PlatformType.LZMini)
            {
                var umodel = await authService.UserInfo<Model.Live_Personnel>();
                model.PartnerGID = umodel.PartnerGID;
                model.PartnerID = umodel.PartnerID;
            }
            model.CustomerID = await customerService.Add(model);
            if (user.PlatformType == PlatformType.LZMini)
            {
                //添加客户后自动分配给当前用户
                Live_PersonnelItems item = new Live_PersonnelItems();
                item.UserID = user.UserID;
                item.CID = model.CustomerID;
                item.Enabled = true;
                item.IsManage = true;
                try
                {
                    await httpService.PostAsync(ServiceNames.UsersService, "/api/personnelitems/add", JsonConvert.SerializeObject(item));
                }
                catch(Exception ex)
                {
                    Common.StaticLog.Error(ex);
                    throw new ErrorCodeException(-1, "新增成功，关联客户发生错误");
                }
            }
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpGet]
        public async Task<Model.Live_Customer> Get(int id)
        {
            var model = await customerService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpGet]
        public async Task<Model.Live_V_Customer> GetView(Guid id)
        {
            var model = await customerService.GetView(it=>it.CustomerGID==id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpGet]
        public async Task<IActionResult> GetInfo(int id)
        {
            var model = await customerService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            UserPagedModel query = new UserPagedModel();
            query.Page = 0;
            var exp = Expressionable.Create<Model.Live_FileUpload>();
            exp.And(t => t.FID == id);
            exp.And(t => t.TypeKey == 40 || t.TypeKey == 41 || t.TypeKey == 42);
            var files = await fileUploadService.List(exp.ToExpression(), query);
            return Ok(new { model, files }); ;
        }
        [HttpGet]
        public async Task<IActionResult> GetDayWorkInfo(int id)
        {
            var model = await customerService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            var file = await dayWorkCustomerContractRecordService1.Get(t => t.CustomerID == model.CustomerID && t.Status == (int)DayWorkCustomerContractRecordStatusEnum.已盖章);
            return Ok(new { model, file }); ;
        }
        [HttpGet]
        public async Task<IActionResult> GetName(string name)
        {
            var model = await customerService.Get(name);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(new { customerName = model.CustomerName, address = model.Address, about = model.About, latitude = model.Latitude, longitude = model.Longitude, picture = model.Picture, entryWorkClass = model.EntryWorkClass, entryWorkName = model.EntryWorkName, entryFromType = model.EntryFromType, City = model.City, model.District, model.Province });
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetFex(int id)
        {
            var model = await customerService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            var guid = HttpContext.Request.Headers["company-guid"].ToString();
            if (!string.IsNullOrEmpty(guid))
            {
                var companyConfig = await companyConfigService.Get(new Guid(guid));
                if (companyConfig.Config.EnableJobAudit)
                {
                    if(string.IsNullOrEmpty(model.Address.Trim()) || string.IsNullOrEmpty(model.About.Trim()) || string.IsNullOrEmpty(model.CustomerName.Trim()))
                    {
                        throw new ErrorCodeException(-1, "该岗位已违规下架！");
                    }
                }  
            }
            return Ok(new { customerName = model.CustomerName, address = model.Address, about = model.About, latitude = model.Latitude, longitude = model.Longitude, picture = model.Picture, entryWorkClass = model.EntryWorkClass, entryWorkName = model.EntryWorkName, entryFromType = model.EntryFromType, serviceUserID = model.ServiceUserID,id=id,City=model.City,model.District,model.Province, welfareTags=new[] { "五险一金","专业培训","绩效奖励"} });
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_Customer model)
        {
            if (!await customerService.Exist(model.CustomerID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await customerService.Update(model);
            return Ok();
        }

        

        [HttpDelete]
        [OperationLogAction("删除客户id：{id}")]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await customerService.Delete(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetDomain(Guid customerGid)
        {
            var model = await customerService.GetNav(customerGid);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }

        [HttpPut]
        public async Task<IActionResult> UpdateDomain([FromBody] CustomerContractFileDto modelDto)
        {
            Live_CustomerExtension model = await customerService.GetNav(modelDto.CustomerGID);
            model.ContractConfigs = modelDto.ContractConfigs;
            await customerService.UpdateNav(model);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetByName(string name, int? companyID,Guid? companyGID)
        {
            var exp = Expressionable.Create<Live_V_Customer>();
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.CustomerName == name);
            exp.AndIF(companyID.HasValue, f => f.CompanyID == companyID.Value);
            exp.AndIF(companyGID.HasValue, f => f.CompanyGID == companyGID.Value);
            var model = await customerService.GetView(exp.ToExpression());
            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetByGID(Guid customerGID)
        {
            var model = await customerService.GetView(f => f.CustomerGID == customerGID);
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> SaleCustomerSave([FromForm] string name, [FromForm] Guid gid, [FromForm] string contact, [FromForm] string phone, [FromForm] string companyNo, [FromForm] string bank, [FromForm] string bankAccount, [FromForm] string address, [FromForm] string city, [FromForm] string province, [FromForm] string businessEntity, [FromForm] bool isSynthetical, [FromForm] bool isDirectSign, [FromForm] string invoiceRemarks, [FromForm] DateTime? invoiceTime, [FromForm] string managerName, [FromForm] int businessType, [FromForm] string businessTypeName, [FromForm] string baseType, [FromForm] string wagesValueAddedTax, [FromForm] string wagesAdditionalTax, [FromForm] string serviceValueAddedTax, [FromForm] string serviceAdditionalTax, [FromForm] decimal? tax, [FromForm] string picture, [FromForm] string contract, [FromForm] string contractFile, [FromForm] string personnelPhones, [FromForm] bool invoiceCombine, [FromForm] string wagesInvoiceType, [FromForm] string wagesInvoiceName, [FromForm] string serviceInvoiceType, [FromForm] string serviceInvoiceName)
        {
            Model.Live_Customer customer = new Live_Customer();
            try
            {
                var model = await customerService.Get(t => t.CustomerName == name);
                if (model != null)
                {
                    return Ok(new { msg = "已存在公司名称", code = 1 });
                }
                customer.CustomerGID = gid;
                customer.CustomerName = name;
                customer.CompanyID = 1;
                customer.CompanyGID = Guid.Parse("a17d524e-b0b4-4048-b110-fec475c440cb");
                customer.Contact = contact ?? "";
                customer.Phone = phone ?? "";
                customer.CompanyNo = companyNo ?? "";
                customer.Bank = bank ?? "";
                customer.BankAccount = bankAccount ?? "";
                customer.Address = address ?? "";
                customer.City = city ?? "";
                customer.Province = province ?? "";
                customer.BusinessEntity = businessEntity ?? "";
                customer.IsSynthetical = isSynthetical;
                //customer.InvoiceName = invoiceName;
                //customer.InvoiceType = invoiceType ?? "";
                customer.invoiceRemarks = invoiceRemarks ?? "";
                customer.BusinessType = businessType;
                customer.BusinessTypeName = businessTypeName ?? "";
                customer.BaseType = baseType ?? "";

                customer.WagesValueAddedTax = wagesValueAddedTax ?? "";
                customer.WagesAdditionalTax = wagesAdditionalTax ?? "";
                customer.ServiceAdditionalTax = serviceAdditionalTax ?? "";
                customer.ServiceValueAddedTax = serviceValueAddedTax ?? "";
                customer.InvoiceCombine = invoiceCombine;
                customer.WagesInvoiceType = wagesInvoiceType ?? "";
                customer.WagesInvoiceName = wagesInvoiceName ?? "";
                customer.ServiceInvoiceType = serviceInvoiceType ?? "";
                customer.ServiceInvoiceName = serviceInvoiceName ?? "";

                switch(businessTypeName)
                {
                    case "劳务外包":
                        customer.BusinessModel = (int)BusinessModel.Epiboly;
                        break;
                    case "劳动关系-外包":
                        customer.BusinessModel = (int)BusinessModel.Agent;
                        break;
                    case "劳务派遣":
                        customer.BusinessModel = (int)BusinessModel.Labor;
                        break;
                    case "劳动关系-派遣":
                        customer.BusinessModel = (int)BusinessModel.AgentLabor;
                        break;
                    case "代缴商保":
                        customer.BusinessModel = (int)BusinessModel.Payment;
                        break;
                }
                

                customer.CreateTime = DateTime.Now;
                customer.Enabled = true;
                customer.SalesMan = managerName;
                customer.IsSaleCustomer = true;
                customer.SaleStatus = (int)SaleCustomerStatusEnum.未分配;
                if (tax.HasValue)
                {
                    customer.Tax = tax;
                }

                if (!string.IsNullOrEmpty(customer.BusinessEntity))
                {
                    var cooperationUnit = await enterpriseService.Get(f => f.IsEnabled && f.Name == customer.BusinessEntity);
                    if (cooperationUnit != null)
                    {
                        customer.CooperationUnitGID = cooperationUnit.ID;
                    }
                }

                customer.OperateModel = 2;
                if (isDirectSign)
                {
                    customer.OperateModel = 1;
                    var paras = new Dictionary<string, string>();
                    paras.Add("Page", "0");
                    paras.Add("Phone", personnelPhones);
                    var obj = await httpService.GetAsync(ServiceNames.UsersService, "/api/Personnel/list", paras);
                    try
                    {
                        List<Model.Live_Personnel> personnels = new List<Live_Personnel>();
                        if (!string.IsNullOrEmpty(obj.Data.ToString()))
                        {
                            var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
                            personnels = JsonConvert.DeserializeObject<List<Model.Live_Personnel>>(ii["items"].ToString());
                        }
                        if (personnels.Count == 0)
                        {
                            return Ok(new { msg = "根据手机号,未找到匹配的驻厂人员", code = 1 });
                        }
                        customer.FactoryUserID = personnels[0].UserID;
                        customer.PartnerID = personnels[0].PartnerID;
                        customer.PartnerGID = personnels[0].PartnerGID;
                        customer.OperateModel = 1;
                    }
                    catch (Exception e)
                    {
                        return Ok(new { msg = "直接签约错误", obj = obj.Data });
                    }
                }



                var id = await customerService.Add(customer);
                if (!string.IsNullOrEmpty(personnelPhones))
                {
                    //var plist = await customerService.SaleBatchQR(personnelPhones, id);
                    var paras = new Dictionary<string, string>();
                    paras.Add("phones", personnelPhones);
                    paras.Add("customerID", id.ToString());
                    var obj = await httpService.PostAsync(ServiceNames.UsersService, "/api/PersonnelItems/SaleBatchQR", paras);
                    try
                    {


                        List<Model.Live_PersonnelItems> items = new List<Live_PersonnelItems>();
                        if (!string.IsNullOrEmpty(obj.Data.ToString()))
                        {
                            var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
                            items = JsonConvert.DeserializeObject<List<Model.Live_PersonnelItems>>(ii["items"].ToString());
                        }
                        if (items.Count == 0)
                        {
                            return Ok(new { msg = "生成二维码数量为0", code = 1 });
                        }
                        foreach (var x in items)
                        {
                            try
                            {
                                var res = await iMSService.JoinGroup(x.UserID.Value, 2, x.CID.Value);
                                Common.StaticLog.Debug(res);
                            }
                            catch (Exception ex)
                            {
                                Common.StaticLog.Error(ex);
                            }
                        }
                    }
                    catch (Exception e)
                    {
                        return Ok(new { msg = "二维码生成失败", obj = obj.Data, code = 1 });
                    }


                }


                List<Live_FileUpload> files = new List<Live_FileUpload>();
                if (id > 0)
                {
                    if (!string.IsNullOrEmpty(picture))
                    {

                        //var pics = JsonConvert.DeserializeObject(picture) as JArray;
                        //foreach (var x in pics)
                        //{
                        //    Live_FileUpload file = new Live_FileUpload();
                        //    file.CompanyID = 1;
                        //    file.CompanyGID = Guid.Parse("a17d524e-b0b4-4048-b110-fec475c440cb");
                        //    file.FID = id;
                        //    file.CreateTime = DateTime.Now;
                        //    file.TypeKey = 40;
                        //    file.TypeValue = "客户图片";
                        //    file.FileName = x["name"].ToString();

                        //    file.Path = "https://sale.cn-labor.cn" + x["url"].ToString();
                        //    files.Add(file);
                        //}
                        var pic = JsonConvert.DeserializeObject(picture) as JObject;
                        Live_FileUpload file = new Live_FileUpload();
                        file.CompanyID = 1;
                        file.CompanyGID = Guid.Parse("a17d524e-b0b4-4048-b110-fec475c440cb");
                        file.FID = id;
                        file.CreateTime = DateTime.Now;
                        file.TypeKey = 40;
                        file.TypeValue = "客户图片";
                        file.FileName = pic["name"].ToString();

                        file.Path = "https://sale.cn-labor.cn" + pic["url"].ToString();
                        files.Add(file);
                    }
                    if (!string.IsNullOrEmpty(contract))
                    {

                        var contractmodel = JsonConvert.DeserializeObject(contract) as JObject;
                        Live_FileUpload file = new Live_FileUpload();
                        file.CompanyID = 1;
                        file.CompanyGID = Guid.Parse("a17d524e-b0b4-4048-b110-fec475c440cb");
                        file.FID = id;
                        file.CreateTime = DateTime.Now;
                        file.TypeKey = 41;
                        file.TypeValue = "客户合同";
                        file.FileName = contractmodel["name"].ToString();
                        if (contractmodel["size"] != null)
                        {
                            file.FileLength = contractmodel["size"].ToString() ?? "";
                        }

                        file.Path = "https://sale.cn-labor.cn" + contractmodel["url"].ToString();
                        files.Add(file);
                    }

                    if (!string.IsNullOrEmpty(contractFile))
                    {

                        var contractfile = JsonConvert.DeserializeObject(contractFile) as JObject;
                        Live_FileUpload file = new Live_FileUpload();
                        file.CompanyID = 1;
                        file.CompanyGID = Guid.Parse("a17d524e-b0b4-4048-b110-fec475c440cb");
                        file.FID = id;
                        file.CreateTime = DateTime.Now;
                        file.TypeKey = 42;
                        file.TypeValue = "客户盖章合同";
                        file.FileName = contractfile["name"].ToString();
                        if (contractfile["size"] != null)
                        {
                            file.FileLength = contractfile["size"].ToString() ?? "";
                        }

                        file.Path = "https://sale.cn-labor.cn" + contractfile["url"].ToString();
                        files.Add(file);
                    }

                }
                if (files.Count > 0)
                {

                    var aaa = await fileUploadService.Add(files);

                }

                //var model = await customerService.GetView(f => f.CustomerGID == customerGID);
                return Ok(new { msg = "OK", code = 0, id });
            }
            catch (Exception e)
            {
                return Ok(new { msg = e.ToString(), code = 1 });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DayWorkCustomerSave([FromForm] string companyName, [FromForm] string name, [FromForm] string contact, [FromForm] string phone, [FromForm] string address, [FromForm] int? timeRule, [FromForm] decimal wages, [FromForm] decimal charge, [FromForm] decimal lunchHour, [FromForm] decimal dinnerHour, [FromForm] string post, [FromForm] string province, [FromForm] string city, [FromForm] string district, [FromForm] string workTime, [FromForm] string workContent, [FromForm] string requirement, [FromForm] string remarks, [FromForm] int cycle, [FromForm] string wxName, [FromForm] string personnelPhones, [FromForm] Guid gid)
        {
            Model.Live_Customer customer = new Live_Customer();
            try
            {
                var model = await customerService.Get(t => t.CustomerName == companyName || t.Name==name);
                if (model != null)
                {
                    return Ok(new { msg = "已存在公司名称", code = 1 });
                }
                customer.CustomerGID = gid;
                customer.CustomerName = companyName;
                customer.Name = name;
                customer.CompanyID = 1;
                customer.CompanyGID = Guid.Parse("a17d524e-b0b4-4048-b110-fec475c440cb");
                customer.Address = address ?? "";
                customer.Contact = contact ?? "";
                customer.City = city ?? "";
                customer.Province = province ?? "";
                customer.District = district ?? "";
                customer.AttendanceType = (int)AttendanceCustomerTypeEnum.日结工;
                customer.TimeRule = timeRule ?? 1;
                customer.Phone = phone ?? "";
                customer.Wages = wages;
                customer.Charge = charge;
                customer.ShiftPrice = charge;
                customer.LunchHour = lunchHour;
                customer.DinnerHour = dinnerHour;
                customer.WorkTime = workTime;
                customer.Post = post;
                customer.WorkContent = workContent;
                customer.Requirement = requirement;
                customer.Remarks = remarks;
                customer.Cycle = cycle;
                customer.WXName = wxName;
                customer.OperateModel = 1;
                customer.Enabled = true;
                //var paras = new Dictionary<string, string>();
                //paras.Add("Page", "0");
                //paras.Add("Phone", personnelPhones);
                //var job = await httpService.GetAsync<Model.Live_JobsDayWork>(ServiceNames.CustomerService, "/api/jobsdaywork/get?id=" + id);
                //var obj = await httpService.GetAsync(ServiceNames.UsersService, "/api/Personnel/list", paras);
                //try
                //{
                //    List<Model.Live_Personnel> personnels = new List<Live_Personnel>();
                //    if (!string.IsNullOrEmpty(obj.Data.ToString()))
                //    {
                //        var ii = JsonConvert.DeserializeObject(obj.Data.ToString()) as JObject;
                //        personnels = JsonConvert.DeserializeObject<List<Model.Live_Personnel>>(ii["items"].ToString());
                //    }
                //    if (personnels.Count == 0)
                //    {
                //        return Ok(new { msg = "根据手机号,未找到匹配的驻厂人员", code = 1 });
                //    }
                //    customer.FactoryUserID = personnels[0].UserID;
                //    customer.PartnerID = personnels[0].PartnerID;
                //    customer.PartnerGID = personnels[0].PartnerGID;
                //}
                //catch (Exception e)
                //{
                //    return Ok(new { msg = "直接签约错误", obj = obj.Data });
                //}
                Live_Personnel personnel = new Live_Personnel();
                personnel.FullName = customer.Contact;
                personnel.UserName = customer.Contact;
                personnel.NickName = customer.Contact;
                personnel.CompanyID = 1;
                personnel.Phone = customer.Phone;
                personnel.CompanyGID = Guid.Parse("a17d524e-b0b4-4048-b110-fec475c440cb");
                personnel.Enabled = true;
                personnel.UserType = (int)PersonnelType.DayworkManage;
                personnel.PartnerID = 33;
                personnel.PartnerGID = Guid.Parse("2f0c0bbc-98b9-47c5-8a3f-bd230ddde68e");
                var modeljson = JsonConvert.SerializeObject(personnel);
                var personnelObj = await httpService.PostModelAsync(ServiceNames.UsersService, "/api/Personnel/AddJoin", modeljson);
                var msg = personnelObj.Message;
                if (msg != "Success")
                {
                    return Ok(new { msg, code = 1 });
                }
                customer.FactoryUserID = Convert.ToInt32(personnelObj.Data);
                customer.PartnerID = personnel.PartnerID; //平台公司总部
                customer.PartnerGID = personnel.PartnerGID;
                var id = await customerService.Add(customer);

                 
                return Ok(new { msg = "OK", code = 0, id });

            }
            catch (Exception e)
            {
                return Ok(new { msg = e.ToString(), code = 1 });
            }
        }
        [HttpPost]
        public async Task<IActionResult> SaleCustomerUpdate([FromForm] int id, [FromForm] int saleStatus, [FromForm] string saleContent)
        {
            var model = await customerService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "customerid不存在");
            }
            model.SaleStatus = saleStatus;
            if (!string.IsNullOrEmpty(saleContent))
            {
                model.SaleContent = saleContent;
            }
            await customerService.Update(model);
            return Ok();
        }

        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpGet]
        public async Task<IActionResult> GetDayWorkCustomer()
        {
            var user = CurrentUser;
            // var pmodel = await authService.UserInfo<Model.Live_Personnel>();
            var expable = Expressionable.Create<Live_Customer>();
            expable.And(it => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == it.CustomerID && s.UserID == user.UserID).Any());
            var model = await customerService.Get(expable.ToExpression());
            return Ok(new { model });
        }

        /// <summary>
        /// 企业法人认证
        /// </summary>
        /// /// <param name="name">法人名字</param>
        /// <param name="cardID">身份证</param>
        /// <param name="customerID">客户单位id</param>
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpPut]
        public async Task<IActionResult> RealName([FromForm] string name, [FromForm] string cardID, [FromForm] int customerID)
        {
            var result = await fxqDevService.IdentityAuthentication2(name, cardID);
            if (result.Data.State == 2)
            {
                // throw new ErrorCodeException(-1, "实名认证结果不匹配");
                return Ok(new { code = 1, msg = "实名认证结果不匹配" });
            }
            if (result.Data.State == 2)
            {
                return Ok(new { code = 2, msg = "其他异常情况" });
            }
            var model = await customerService.Get(customerID);
            model.Name = name;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("CustomerID", customerID);
            dic.Add("VerifyName", name);
            dic.Add("VerifyCardID", cardID);
            dic.Add("IsReal", true);
            var i = await customerService.DictionarySet(dic);
            if (i > 0)
            {
                return Ok(new { code = 0, msg = "认证成功" });
            }
            else
            {
                throw new ErrorCodeException(-1, "更新失败");
            }

        }

        /// <summary>
        /// 授权验证
        /// </summary>
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpPost]
        public async Task<IActionResult> BankAuthorize([FromForm] string bank, [FromForm] string bankName, [FromForm] string bankAccount, [FromForm] string name, [FromForm] string cardID, [FromForm] int customerID)
        {
            //实名认证
            var result = await fxqDevService.IdentityAuthentication2(name, cardID);
            if (result.Data.State == 2)
            {
                // throw new ErrorCodeException(-1, "实名认证结果不匹配");
                return Ok(new { code = 1, msg = "实名认证结果不匹配" });
            }
            if (result.Data.State == 2)
            {
                return Ok(new { code = 2, msg = "其他异常情况" });
            }
            //授权认证
            var auth = await fxqDevService.CompanyBankPayment(bankName, bank, bankAccount);
            if (auth.Code == 10000)
            {
                Dictionary<string, object> dic = new Dictionary<string, object>();
                dic.Add("CustomerID", customerID);
                dic.Add("VerifyAmount", auth.Data.amount);
                dic.Add("VerifyCode", auth.Data.verify_code);
                await customerService.DictionarySet(dic);
                return Ok(new {code=0, data=auth.Data,msg="打款成功" });
            }
            else
            {
                return Ok(new {code=3, msg =auth.Msg });
            }
            

        }

        /// <summary>
        ///客户信息只保存不认证
        /// </summary>
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpPut]
        public async Task<IActionResult> UpdateCustomerAuth([FromBody] Model.Model.CustomerAuthModel model)
        {
            var customer = await customerService.Get(model.CustomerID);
            customer.CustomerName = model.CustomerName;
            customer.CompanyNo = model.CompanyNo;
            customer.VerifyName = model.VerifyName;
            customer.VerifyCardID = model.VerifyCardID;
            customer.LegalName = model.LegalName;
            customer.IsAuth = model.IsAuth;
            customer.IsReal = model.IsReal;
            customer.Address = model.Address;
            customer.AuthorizeFilePath = model.AuthorizeFilePath;
            customer.VerifyAmount = model.VerifyAmount;
            customer.VerifyCode = model.VerifyCode;
            customer.BankName = model.BankName;
            customer.Bank = model.Bank;
            customer.BankAccount = model.BankAccount;
            customer.VerifyType = model.VerifyType;
            await customerService.Update(customer);
            return Ok();
            
        }



        /// <summary>
        /// 企业认证并且生成合同模板（未盖章）
        /// </summary>
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpPut]
        public async Task<IActionResult> CompanyAuthorized([FromBody] Model.Model.CustomerAuthModel model)
        {
            var pmodel = await authService.UserInfo<Model.Live_Personnel>();
            var customer = await customerService.Get(model.CustomerID);
            customer.CustomerName = model.CustomerName;
            customer.CompanyNo = model.CompanyNo;
            customer.VerifyName = model.VerifyName;
            customer.VerifyCardID = model.VerifyCardID;
            customer.LegalName = model.LegalName;
            customer.IsAuth = model.IsAuth;
            customer.IsReal = model.IsReal;
            customer.Address = model.Address;
            customer.AuthorizeFilePath = model.AuthorizeFilePath;
            customer.VerifyAmount = model.VerifyAmount;
            customer.VerifyCode = model.VerifyCode;
            customer.BankName = model.BankName;
            customer.Bank = model.Bank;
            customer.BankAccount = model.BankAccount;
            customer.VerifyType = model.VerifyType;


            string ali = "https://oss.cn-labor.cn";
            var jobs = await jobsDayWorkService.List(t => t.CustomerID == customer.CustomerID && t.Status == 2,new PagedModelBase { Page=0 });
            string jobString = "";
            if(jobs.Count>0)
            {
                int index = 1;
                foreach (var j in jobs)
                {
                    string dot = index == jobs.Count ? "" : ",";
                    jobString += j.Title + dot;
                }
            }
            

            //实名认证
            var realNameResult = await fxqDevService.IdentityAuthentication2(customer.VerifyName, customer.VerifyCardID);
            if (realNameResult.Data.State == 2)
            {
                // throw new ErrorCodeException(-1, "实名认证结果不匹配");
                return Ok(new { code = 1, msg = "实名认证结果不匹配" });
            }
            if (realNameResult.Data.State == 2)
            {
                return Ok(new { code = 2, msg = "实名认证其他异常情况" });
            }
            customer.IsReal = true;
            //企业认证
            var result = await fxqDevService.CompanyAuthentication3(customer.CustomerName, customer.CompanyNo, customer.LegalName);
            if (result.Data.Status == 2)
            {
                return Ok(new { code = 3, msg = "公司和法人全不匹配" });
            }
            else if (result.Data.Status == 12)
            {
                return Ok(new { code = 4, msg = "公司匹配&&法人不匹配" });
            }
            else if (result.Data.Status == 13)
            {
                return Ok(new { code = 5, msg = "公司不匹配&&法人匹配" });
            }
            var signResult = await fxqDevService.CompanyElectronicSeal(customer.CustomerName, null, null, 0, 0, 0, 1, 0);
            try
            {
                string statebase64 = "";
                using (HttpClient client = new HttpClient())
                {
                    //文件地址转成base64
                    var imageBytes = await client.GetByteArrayAsync(signResult.Data);
                    statebase64 = Convert.ToBase64String(imageBytes);
                }
                string picUrl = "";
                using (var ms = new MemoryStream(Convert.FromBase64String(statebase64)))
                {
                    //文件流上传到服务器
                    var ressd = await aliyunOSSService.UploadFile(ms, "公司签章图片.png", true, false, "temp");
                    if (ressd.Code != 0)
                    {
                        throw new ErrorCodeException(-1, "生成公司签章图片错误");
                    }
                    picUrl = ali + ressd.Data.ToString();
                }
                var entity = new Live_ContractSigner()
                {
                    ID = Guid.NewGuid(),
                    Name = model.CustomerName,
                    Idno = model.CompanyNo,
                    LegalPerson = model.LegalName,
                    OssUrl = picUrl,
                    IsCompany = true,
                    Phone = string.Empty,
                    IsEnabled = true,
                    Expired = DateTime.Now,
                    CreateTime = DateTime.Now,
                    IsRealNameSuc = true,
                    RealNameTime = DateTime.Now,
                    RealNameType = RealNameEnumType.企业三要素,
                    Remark = string.Empty,
                    BankNo = string.Empty,
                    LegalPersonIdCardNo = string.Empty,

                };


                await contractSignerService.Add(entity);
            }
            catch(Exception e)
            {
                return Ok(new { code = 1, msg = "生成签章错误",error= signResult.Data });
            }
            customer.IsAuth = true;
            await customerService.Update(customer);


            //企业认证后生成合同
            var templateContract = await contractFileConfigService.Get(t => t.ContractEnumType == ContractEnumType.灵活用工客户合同);
            if(templateContract==null)
            {
                throw new ErrorCodeException(-1, "合同模块不存在");
            }
            string docxBase64 = "";
            using (HttpClient client = new HttpClient())
            {
                var imageBytes = await client.GetByteArrayAsync(templateContract.FilePath);
                docxBase64 = Convert.ToBase64String(imageBytes);
            }
            var today = DateTime.Today;
            int cycle = customer.Cycle.HasValue ? customer.Cycle.Value : 12;//周期默认12月，可修改
            var end = today.AddMonths(cycle);
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("address", customer.Address);
            dic.Add("companyno", customer.CompanyNo);
            dic.Add("customername", customer.CustomerName);
            dic.Add("job", jobString);
            dic.Add("phone", pmodel.Phone);
            dic.Add("verifyname", customer.VerifyName);
            dic.Add("syear", today.Year.ToString());
            dic.Add("smonth", today.Month.ToString());
            dic.Add("sday", today.Day.ToString());
            dic.Add("eyear", end.Year.ToString());
            dic.Add("emonth", end.Month.ToString());
            dic.Add("eday", end.Day.ToString());
            dic.Add("wages", customer.Wages.HasValue? customer.Wages.ToString()+"元":" ");

            var newDic = dic.Where(t => t.Value != null && t.Value != "").OrderBy(t => t.Key).ToDictionary();

            var pathBase = await fxqService.CombineDocxKeywordToPdf(docxBase64, newDic);
            if (pathBase.Code != 200)
            {
                throw new ErrorCodeException(-1, "生成合同报错");
            }
            
            Model.Live_DayWorkCustomerContractRecord file = new Model.Live_DayWorkCustomerContractRecord();
            file.GID = Guid.NewGuid();
            file.CompanyID = customer.CompanyID;
            file.CompanyGID = customer.CompanyGID;
            file.CustomerID = customer.CustomerID;
            file.ContractFileConfigID = templateContract.ID.ToString();
            file.CreateTime = DateTime.Now;
            file.StartTime = today;
            file.EndTime = end;
            file.Status = 1;
            file.CompanyNo = model.CompanyNo;
            using (var ms = new MemoryStream(Convert.FromBase64String(pathBase.Data)))
            {

                var ressd = await aliyunOSSService.UploadFile(ms, "客户合同.pdf", true, false, "temp");
                if (ressd.Code != 0)
                {
                    throw new ErrorCodeException(-1, "上传合同错误");
                }
                file.TemplateContract = ali + ressd.Data.ToString();
            }
            
            await dayWorkCustomerContractRecordService.Add(file);


            return Ok(new { code = 0, msg = "公司认证成功并且已生成合同" });

        }

        [HttpGet]
        public async Task<IActionResult> ListByCompanyID(int companyID)
        {
            var user = CurrentUser;
            var info = await authService.UserInfo<Model.SYS_CompanyUser>();
            var exp = Expressionable.Create<Model.Live_V_Customer>();
            exp.And(f => f.CompanyID == companyID);
            var models = await customerService.List(exp.ToExpression(), new PagedModelBase() { Page = 0 });
            return Ok(new { items = models, count = models.Count });
        }

        [HttpGet]
        public async Task<IActionResult> ToSale()
        {
            var exp = Expressionable.Create<Model.Live_V_Customer>();
            exp.And(f => f.CompanyID == 1);
            exp.And(f => !string.IsNullOrEmpty(f.SalesMan) && f.SalesMan != "1");
            exp.And(f => f.CreateTime >= Convert.ToDateTime("2026-04-01"));
            var models = await customerService.List(exp.ToExpression(), new PagedModelBase() { Page = 0 });
            return Ok(new { items = models });
        }


        [HttpGet]
        public async Task<IActionResult> ExportBatch([FromQuery] CustomerPagedModel query)
        {

            var user = CurrentUser;
            var info = await authService.UserInfo<Model.SYS_CompanyUser>();
            var expable = Expressionable.Create<Model.Live_V_Customer>();
            expable.And(t => t.CompanyID == 1);
            var list = await customerService.List(expable.ToExpression(), new PagedModelBase() { Page = 0 });

            var fileBytes = customerService.Export(list);
            string base64String = Convert.ToBase64String(fileBytes);
            return Ok(new { base64String, fileName = "客户信息.xlsx", mimeType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet" });
        }

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
                var expable = Expressionable.Create<Model.Live_Customer>();
                expable.And(t => t.CompanyID == 1);
                var list = await customerService.List(expable.ToExpression(), new PagedModelBase() { Page = 0 });

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
                    ds = Common.ExcelHeler.ReadExcelToDataSet(fn, "", 2);
                    List<Live_Customer> models = new List<Live_Customer>();
                    if (ds != null && ds.Tables.Count > 0)
                    {

                        List<Live_InsuranceEmployee> updatelist = new List<Live_InsuranceEmployee>();

                        foreach (DataRow row in ds.Tables[0].Rows)
                        {
                            var model = list.Find(t => t.CustomerID == Convert.ToInt32(row[2].ToString()));
                            if (model == null)
                            {
                                continue;

                            }
                            else
                            {
                                model.About = row[1] != null ? row[1].ToString() : "";
                                models.Add(model);
                            } 
                            

                        }
                        await customerService.UpdateList(models);
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
