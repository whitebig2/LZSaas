using Consul;
using LZ.ApiService.Interface;
using LZ.Core.EventData.FxqEventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Users.Service.Interface;
using LZ.Users.Service.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.ComponentModel.Design;
using System.Security.Cryptography;
using System.Text;

namespace LZ.Users.API.Controllers
{
    public class JDYController(IPersonnelItemsService personnelItemsService,IHttpService httpService, IPersonnelService personnelService,  ICompanyConfigService companyConfigService,IEntryService entryService, IJDYSyncService jdySyncService, IUserService userService, IEventPublisher publisher,IJDYService jDYService) : UsersControllerBase
    {
        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> PersonnelReceive()
        {
            try
            {
                var stream = new MemoryStream();
                await Request.Body.CopyToAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);
                var body = await new StreamReader(stream).ReadToEndAsync();
                var nonce = Request.Query["nonce"];
                var companygid = Request.Query["id"];
                Common.StaticLog.Debug(body);
                if (string.IsNullOrEmpty(companygid))
                {
                    throw new ErrorCodeException(-1, "来源错误");
                }
                Common.StaticLog.Debug(companygid.ToString());
                var company = await companyConfigService.Get(new Guid(companygid.ToString()));
                if (company.JDYForms == null)
                {
                    Common.StaticLog.Error($"{companygid.ToString()};未配置简道云表单字段信息");
                    throw new ErrorCodeException(-1, "未配置简道云表单字段信息");
                }
                var timestamp = Request.Query["timestamp"];
                var signature = Request.Headers["X-JDY-Signature"];
                string SECRET = company.Config.JDYSecret;
                string[] param = { nonce, body, SECRET, timestamp };
                var bytes = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(string.Join(":", param)));
                if (BitConverter.ToString(bytes).Replace("-", "").ToLower().Equals(signature))
                {
                    var payload = (JObject)JsonConvert.DeserializeObject(body);
                    JObject data = payload["data"].ToObject<JObject>();
                    var action = payload["op"].ToString();
                    if (action == "data_create" || action == "data_update")
                    {
                        var jdyJson = new Dictionary<string, object>();
                        var userinfoFrom = company.JDYForms.Find(it => it.Type == "userinfosnync");
                        if (userinfoFrom != null)
                        {
                            foreach (var f in userinfoFrom.Fields)
                            {
                                if (data[f.JDYField] != null)
                                {
                                    if (f.FieldType == "array")
                                    {
                                        var subInfos = data[f.JDYField] as JArray;
                                        if (subInfos.Count > 0)
                                        {
                                            var subinfo = subInfos[0];
                                            foreach (var item in f.Items)
                                            {
                                                if (subinfo[item.JDYField] != null)
                                                {
                                                    jdyJson.Add(item.JDYField, subinfo[item.JDYField].ToString());
                                                }
                                            }
                                        }
                                    }
                                    else if (f.FieldType == "json")
                                    {
                                        if (!string.IsNullOrEmpty(f.Field))
                                        {
                                            jdyJson.Add(f.JDYField, data[f.JDYField][f.Field]);
                                        }
                                        else
                                        {
                                            jdyJson.Add(f.JDYField, data[f.JDYField]);
                                        }
                                    }
                                    else
                                    {
                                        jdyJson.Add(f.JDYField, data[f.JDYField].ToString());
                                    }
                                }
                            }
                        }
                        var strJson = JsonConvert.SerializeObject(jdyJson);
                        var model = await personnelItemsService.Get(it => it.JDYDataID == data["_id"].ToString());
                        if (model != null)
                        {
                            await personnelItemsService.SaveJDYJson(model.ID, strJson, model.JDYDataID);
                            await jdySyncService.PersonnelSync(model.ID);
                        }
                        else
                        {
                            var qrcodeFrom = company.JDYForms.Find(it => it.Type == "sendorder");
                            if (qrcodeFrom == null)
                            {
                                throw new ErrorCodeException(-1, "未配置简道云发单字段信息");
                            }
                            var qrcodeField = qrcodeFrom.Fields.Find(it => it.Type == "isqrcode");
                            if (qrcodeField == null)
                            {
                                throw new ErrorCodeException(-1, "未配置简道云发单是否二维码字段信息");
                            }
                            var isqrcode = data[qrcodeField.JDYField].ToString();
                            Common.StaticLog.Debug($"是否二维码字段:{isqrcode};");
                            if (!string.IsNullOrEmpty(isqrcode) && isqrcode.ToLower() == "需要")
                            {
                                var inmodel = new Live_PersonnelItems();
                                inmodel.JDYDataID = data["_id"].ToString();
                                var customerField = qrcodeFrom.Fields.Find(it => it.Type == "customerName");
                                if (customerField == null)
                                {
                                    throw new ErrorCodeException(-1, "未配置简道云发单客户字段信息");
                                }
                                var customerName = data[customerField.JDYField].ToString();
                                if (!string.IsNullOrEmpty(customerName))
                                {
                                    var cmodels = await httpService.GetAsync<List<Live_V_Customer>>(ServiceNames.CustomerService, $"/api/customer/list?companyId={company.Company.CompanyID}&name={customerName}", isItem: true);
                                    if (cmodels == null || cmodels.Count == 0)
                                    {
                                        throw new ErrorCodeException(-1, "未配置未匹配到客户信息");
                                    }
                                    inmodel.CID = cmodels[0].CustomerID;
                                }
                                var personnelField = qrcodeFrom.Fields.Find(it => it.Type == "personnelName");
                                if (personnelField == null)
                                {
                                    throw new ErrorCodeException(-1, "未配置简道云发单驻厂字段信息");
                                }
                                var personnelName = "";
                                var personnelJDYNumber = "";
                                if (personnelField.FieldType == "json")
                                {
                                    JObject fdata = data[personnelField.JDYField].ToObject<JObject>();
                                    personnelName = fdata["name"].ToString();
                                    personnelJDYNumber = fdata["username"].ToString();
                                }
                                else
                                {
                                    personnelName = data[personnelField.JDYField].ToString();
                                }
                                if (!string.IsNullOrEmpty(personnelName))
                                {
                                    var pmodel = await personnelService.Get(it => it.FullName == personnelName && it.CompanyID == company.Company.CompanyID);
                                    if (pmodel == null)
                                    {
                                        throw new ErrorCodeException(-1, "未配置未匹配到驻厂信息");
                                    }
                                    if (string.IsNullOrEmpty(pmodel.JDYNumber))
                                    {
                                        pmodel.JDYNumber = personnelJDYNumber;//同步简道云用户
                                        await personnelService.UpdateJDYNumber(new List<Live_Personnel>() { pmodel });
                                    }
                                    inmodel.UserID = pmodel.UserID;
                                }
                                inmodel.GID = Guid.NewGuid();
                                inmodel.IsManage = true;
                                inmodel.Enabled = true;
                                inmodel.JDYJson = strJson;
                                //var oldModel = await personnelItemsService.Get(it => it.UserID == inmodel.UserID && it.CID == inmodel.CID);
                                //if (oldModel == null)
                                //{
                                //    await personnelItemsService.Add(inmodel);
                                //}
                                //else
                                //{
                                //    await personnelItemsService.SaveJDYJson(oldModel.ID, strJson, inmodel.JDYDataID);
                                //    var qrcodeUploadField = jDYSync.GetJDYField(qrcodeFrom.Fields, "qrcode");
                                //    var qrcode = data[qrcodeUploadField.JDYField];
                                //    Common.StaticLog.Debug(qrcode);
                                //    if (qrcode == null || (qrcode as JArray).Count==0)
                                //    {
                                //        await jDYSync.PersonnelSync(oldModel.ID);
                                //    }

                                //}
                                await personnelItemsService.Add(inmodel);
                            }
                        }
                    }
                    return Ok("success");
                }
                else
                {
                    // 因为 aspnetcore 中的 Unauthorized 不能附带消息，所以这里用这个方法
                    return new ObjectResult("fail")
                    {
                        StatusCode = 401
                    };
                }
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex.Message, ex);
                return new ObjectResult("fail")
                {
                    StatusCode = 401
                };
            }
        }

        /// <summary>
        /// 同步人员信息
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> SyncUserInfo(Guid companyId)
        {
            try
            {
                var company = await companyConfigService.Get(companyId);
                var reportFrom = company.JDYForms.Find(it => it.Type == "userinfo");
                if (reportFrom == null)
                {
                    throw new ErrorCodeException(-1, "未配置简道云人员资信息字段信息");
                }
                var statuss = new int[] { (int)UserStatus.Entry, (int)UserStatus.WaitEntry };
                var models = await entryService.List(it => it.CompanyGID == companyId && statuss.Contains(it.Status.Value) && !string.IsNullOrEmpty(it.JDYDataID),new Model.PagedModel.UserPagedModel() { Top=100});
                if (models.Count > 0)
                {
                    var ids = models.Select(it => it.JDYDataID);
                    var paras = new Dictionary<string, object>();
                    paras.Add("app_id", reportFrom.AppID);
                    paras.Add("entry_id", reportFrom.FormID);
                    paras.Add("limit", 100);
                    var fields = new List<string>();
                    var statusField = jdySyncService.GetJDYField(reportFrom.Fields, "status");
                    fields.Add(statusField.JDYField);
                    var entryTimeField = jdySyncService.GetJDYField(reportFrom.Fields, "entryTime");
                    fields.Add(entryTimeField.JDYField);
                    var leaveTimeField = jdySyncService.GetJDYField(reportFrom.Fields, "leaveTime");
                    fields.Add(leaveTimeField.JDYField);
                    paras.Add("fields", fields);
                    var filter = new Dictionary<string, object>();
                    filter.Add("rel", "and");
                    var where = new List<object>();
                    where.Add(new { field = "data_id", type = "data_id", method = "in", value = ids });
                    filter.Add("cond", where);
                    paras.Add("filter", filter);
                    Common.StaticLog.Debug(JsonConvert.SerializeObject(paras));
                    var res = await jDYService.QueryMultipleFormData(paras, company.Config);
                    Common.StaticLog.Debug(res);
                    var resObj = (JObject)JsonConvert.DeserializeObject(res);
                    var dataObj = resObj["data"] as JArray;
                    var datas = new List<Dictionary<string, object>>();
                    var userList = new List<Model.Live_UserEntry>();
                    foreach (var item in dataObj)
                    {
                        var model = models.Find(it => it.JDYDataID == item["_id"].ToString());
                        if (model != null)
                        {
                            if (item[statusField.JDYField].ToString() == "在职" && model.Status == (int)UserStatus.WaitEntry || item[statusField.JDYField].ToString() == "离职")
                            {
                                var umodel = new Model.Live_UserEntry();
                                umodel.ID = model.ID;
                                if (item[statusField.JDYField].ToString() == "离职")
                                {
                                    umodel.Status = (int)UserStatus.Leave;
                                    var leaveTime = item[leaveTimeField.JDYField].ToString();
                                    if (!string.IsNullOrEmpty(leaveTime))
                                    {
                                        umodel.LeaveTime = Convert.ToDateTime(leaveTime).AddDays(1);
                                    }
                                    var contact = new UserEntryContractRecordEventData(model.ID, ContractEnumType.离职合同, Guid.NewGuid());
                                    publisher.Publish(contact);
                                }
                                else
                                {
                                    umodel.Status = (int)UserStatus.Entry;
                                    var entryTime = item[entryTimeField.JDYField].ToString();
                                    if (!string.IsNullOrEmpty(entryTime))
                                    {
                                        umodel.EntryTime = Convert.ToDateTime(entryTime).AddDays(1);
                                    }
                                    var entryContact = new UserEntryContractRecordEventData(model.ID, ContractEnumType.入职合同, Guid.NewGuid());
                                    publisher.Publish(entryContact);
                                }
                                userList.Add(umodel);
                            }
                        }
                    }
                    if (userList.Count > 0)
                    {
                        var entryList = userList.Where(it=>it.Status== (int)UserStatus.Entry).ToList();
                        if (entryList.Count > 0)
                        {
                            var has = await entryService.BatchChangeEntryInfo(entryList, it => new { it.Status, it.EntryTime });
                            if (has)
                            {
                                foreach (var entry in entryList)
                                {
                                    var entryContact = new UserEntryContractRecordEventData(entry.ID, ContractEnumType.入职合同, Guid.NewGuid());
                                    publisher.Publish(entryContact);
                                }
                            }
                        }
                        var levelList = userList.Where(it => it.Status == (int)UserStatus.Leave).ToList();
                        if (levelList.Count > 0)
                        {
                           var has = await entryService.BatchChangeEntryInfo(levelList, it => new { it.Status, it.LeaveTime });
                            if (has)
                            {
                                foreach (var entry in levelList)
                                {
                                    var contact = new UserEntryContractRecordEventData(entry.ID, ContractEnumType.离职合同, Guid.NewGuid());
                                    publisher.Publish(contact);
                                }
                            }
                        }
                    }
                }
                return Ok();
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return Ok("错误");
            }
        }

        /// <summary>
        /// 同步人员状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpGet]
        public async Task<IActionResult> SyncUserContract(int id)
        {
            try
            {
                await jdySyncService.UserContractSync(id);
                return Ok();
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return Ok("错误");
            }
        }
        /// <summary>
        /// 批量同步人员状态
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpGet]
        public async Task<IActionResult> BatchSyncUserContract(int id)
        {
            try
            {
                var models = await entryService.List(it => it.CompanyID == 60 && !string.IsNullOrEmpty(it.JDYDataID));
                foreach (var model in models)
                {
                    await jdySyncService.UserContractSync(model.ID);
                    Thread.Sleep(100);
                }
                return Ok();
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return Ok("错误");
            }
        }
        /// <summary>
        /// 接收简道云
        /// </summary>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPost]
        [AllowAnonymous]

        public async Task<IActionResult> UserReceive()
        {
            try
            {
                var stream = new MemoryStream();
                await Request.Body.CopyToAsync(stream);
                stream.Seek(0, SeekOrigin.Begin);
                var body = await new StreamReader(stream).ReadToEndAsync();
                var nonce = Request.Query["nonce"];
                var companygid = Request.Query["id"];
                Common.StaticLog.Debug(body);
                if (string.IsNullOrEmpty(companygid))
                {
                    throw new ErrorCodeException(-1, "来源错误");
                }
                Common.StaticLog.Debug(companygid.ToString());
                var company = await companyConfigService.Get(new Guid(companygid.ToString()));
                if (company.JDYForms == null)
                {
                    Common.StaticLog.Error($"{companygid.ToString()};未配置简道云表单字段信息");
                    throw new ErrorCodeException(-1, "未配置简道云表单字段信息");
                }
                var timestamp = Request.Query["timestamp"];
                var signature = Request.Headers["X-JDY-Signature"];
                string SECRET = company.Config.JDYSecret;
                string[] param = { nonce, body, SECRET, timestamp };
                var bytes = SHA1.Create().ComputeHash(Encoding.UTF8.GetBytes(string.Join(":", param)));
                if (BitConverter.ToString(bytes).Replace("-", "").ToLower().Equals(signature))
                {
                    var payload = (JObject)JsonConvert.DeserializeObject(body);
                    JObject data = payload["data"].ToObject<JObject>();
                    var action = payload["op"].ToString();
                    if (action == "data_create" || action == "data_update")
                    {
                        var model = await entryService.Get(it => it.JDYDataID == data["_id"].ToString() && it.CompanyID == company.Company.CompanyID);
                        if (model != null)
                        {
                            var userinfoFrom = company.JDYForms.Find(it => it.Type == "userinfo");
                            if (userinfoFrom == null)
                            {
                                throw new ErrorCodeException(-1, "未配置简道云人员信息字段信息");
                            }
                            var statusField = jdySyncService.GetJDYField(userinfoFrom.Fields, "status");
                            var userStatus = data[statusField.JDYField].ToString();
                            if (!string.IsNullOrEmpty(userStatus))
                            {
                                var isUpdate = false;
                                if (userStatus == "在职" && model.Status != (int)UserStatus.Entry)
                                {
                                    var entryField = jdySyncService.GetJDYField(userinfoFrom.Fields, "entryTime");
                                    var entryTime = data[entryField.JDYField].ToString();
                                    if (!string.IsNullOrEmpty(entryTime))
                                    {
                                        model.EntryTime = Convert.ToDateTime(entryTime).AddDays(1);
                                    }
                                    if (model.Status != (int)UserStatus.Entry)
                                    {
                                        var entryContact = new UserEntryContractRecordEventData(model.ID, ContractEnumType.入职合同, Guid.NewGuid());
                                        publisher.Publish(entryContact);
                                    }
                                    model.Status = (int)UserStatus.Entry;
                                    isUpdate = true;
                                }
                                else if (userStatus == "离职" && model.Status != (int)UserStatus.Leave)
                                {
                                    var entryField = jdySyncService.GetJDYField(userinfoFrom.Fields, "leaveTime");
                                    var leaveTime = data[entryField.JDYField].ToString();
                                    if (!string.IsNullOrEmpty(leaveTime))
                                    {
                                        model.LeaveTime = Convert.ToDateTime(leaveTime).AddDays(1);
                                    }
                                    if (model.Status != (int)UserStatus.Leave)
                                    {
                                        var contact = new UserEntryContractRecordEventData(model.ID, ContractEnumType.离职合同, Guid.NewGuid());
                                        publisher.Publish(contact);
                                    }
                                    model.Status = (int)UserStatus.Leave;
                                    isUpdate = true;
                                }
                                if (isUpdate)
                                {
                                    await entryService.Update(model);
                                    if(model.IsUser && model.UserID > 0)
                                    {
                                        await userService.ChangeStatus(model.UserID.Value, model.Status.Value);
                                    }
                                    
                                }
                            }
                        }
                        else
                        {
                            Common.StaticLog.Debug($"人员状态:不存在{company.Company.CompanyID};");
                        }
                    }
                    return new ContentResult
                    {
                        Content = "success",
                        ContentType = "text/plain",
                        StatusCode = 200
                    };
                }
                else
                {
                    // 因为 aspnetcore 中的 Unauthorized 不能附带消息，所以这里用这个方法
                    return new ObjectResult("fail")
                    {
                        StatusCode = 401
                    };
                }
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex.Message, ex);
                return new ObjectResult("fail")
                {
                    StatusCode = 401
                };
            }
        }
    }
}
