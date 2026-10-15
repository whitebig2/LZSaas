using LZ.ApiService.Interface;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Users.Service.Interface;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace LZ.Users.Service.Services
{
    public class JDYSyncService(IPersonnelItemsService personnelItemsService, IHttpService httpService, IPersonnelService personnelService, IEntryService entryService, ICompanyConfigService companyConfigService, IJDYService jDYService, ISettingsService settingsService): IJDYSyncService, IScopedDependency
    {
        public async Task PersonnelSync(int id)
        {
            try
            {
                var model = await personnelItemsService.Get(it => it.ID == id);
                var company = await companyConfigService.Get(model.CompanyGID);
                if (company.Config.EnableDockJDY && company.JDYForms != null && !string.IsNullOrEmpty(model.JDYDataID))
                {
                    var qrcodeFrom = company.JDYForms.Find(it => it.Type == "sendorder");
                    if (qrcodeFrom == null)
                    {
                        throw new ErrorCodeException(-1, "未配置简道云发单字段信息");
                    }
                    var qrcodeField = GetJDYField(qrcodeFrom.Fields, "qrcode");
                    var transaction_id = Guid.NewGuid().ToString();
                    var imageParas = new Dictionary<string, object>();
                    imageParas.Add("app_id", qrcodeFrom.AppID);
                    imageParas.Add("entry_id", qrcodeFrom.FormID);
                    imageParas.Add("transaction_id", transaction_id);
                    var imageToken = await jDYService.GetUploadToken(imageParas, company.Config);
                    var imageTokenObj = (JObject)JsonConvert.DeserializeObject(imageToken);
                    var tokens = imageTokenObj["token_and_url_list"] as JArray;
                    if (tokens == null || tokens.Count == 0)
                    {
                        Common.StaticLog.Debug(imageToken);
                        throw new ErrorCodeException(-1, "上传二维码出错");
                    }
                    var uploadUrl = tokens[0]["url"].ToString();
                    var uploadToken = tokens[0]["token"].ToString();
                    var setting = await settingsService.GetMasterSettings();
                    var image = await Common.Tool.DownloadImageStreamAsync(setting.AliBucketDomain + model.Qrcode);
                    var imageKey = await jDYService.UploadFileByTokenAsync(uploadUrl, uploadToken, image, "二维码", "");
                    var imageKeyObj = (JObject)JsonConvert.DeserializeObject(imageKey);
                    var paras = new Dictionary<string, object>();
                    paras.Add("app_id", qrcodeFrom.AppID);
                    paras.Add("entry_id", qrcodeFrom.FormID);
                    paras.Add("data_id", model.JDYDataID);
                    paras.Add("transaction_id", transaction_id);
                    var data = new Dictionary<string, object>();
                    var value = new Dictionary<string, object>();
                    var list = new List<string>();
                    list.Add(imageKeyObj["key"].ToString());
                    value.Add("value", list);
                    data.Add(qrcodeField.JDYField, value);
                    paras.Add("data", data);
                    await jDYService.ChangeFormData(paras, company.Config);
                }

            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);

            }
        }

        /// <summary>
        /// 同步简道云
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        public async Task UserSync(int id)
        {
            try
            {
                var model = await entryService.View(it => it.ID == id);
                var company = await companyConfigService.Get(model.CompanyGID);
                if (company.Config.EnableDockJDY && company.JDYForms != null)
                {
                    var userinfoFrom = company.JDYForms.Find(it => it.Type == "userinfo");
                    if (userinfoFrom == null)
                    {
                        throw new ErrorCodeException(-1, "未配置简道云人员信息字段信息");
                    }
                    //var customer = await httpService.GetAsync<Model.Live_V_Customer>(ServiceNames.CustomerService, $"/api/customer/getview?id={model.CustomerGID}");
                    //if (customer == null)
                    //{
                    //    throw new ErrorCodeException(-1, "客户信息不存在");
                    //}
                    var personnel = await personnelService.Get(model.PerUserID.Value);
                    if (personnel == null)
                    {
                        throw new ErrorCodeException(-1, "驻厂信息不存在");
                    }
                    Model.Live_PersonnelItems personnelItem = null;
                    if (model.QrcodeGID.HasValue)
                    {
                        personnelItem = await personnelItemsService.GetInfo(it => it.GID == model.QrcodeGID.Value);
                    }
                    if (personnelItem == null)
                    {
                        personnelItem = await personnelItemsService.GetInfo(it => it.CID == model.CustomerID && it.UserID == model.PerUserID);
                    }
                    if (personnelItem == null)
                    {
                        throw new ErrorCodeException(-1, "发单信息不存在");
                    }
                    var transaction_id = Guid.NewGuid().ToString();
                    var paras = new Dictionary<string, object>();
                    paras.Add("app_id", userinfoFrom.AppID);
                    paras.Add("entry_id", userinfoFrom.FormID);
                    if (!string.IsNullOrEmpty(model.JDYDataID))
                    {
                        paras.Add("data_id", model.JDYDataID);
                    }
                    paras.Add("transaction_id", transaction_id);
                    var data = new Dictionary<string, object>();

                    var reportInfo = await GetReportInfo(model.Name, DateTime.Now.ToString("yyyy-MM-dd"), company);//获取人员报备
                    if (reportInfo != null)
                    {
                        var supplierField = GetJDYField(userinfoFrom.Fields, "supplier");
                        if (reportInfo.ContainsKey("supplier"))
                        {
                            data.Add(supplierField.JDYField, new { value = reportInfo["supplier"] });
                        }
                        var channelField = GetJDYField(userinfoFrom.Fields, "channel");
                        if (reportInfo.ContainsKey("channel"))
                        {
                            data.Add(channelField.JDYField, new { value = reportInfo["channel"] });
                        }
                    }
                    //var statusField = GetJDYField(userinfoFrom.Fields, "status");
                    //var status = "";
                    //if (model.Status == (int)UserStatus.Entry)
                    //{
                    //    status = "在职";
                    //}
                    //else if (model.Status == (int)UserStatus.Leave)
                    //{
                    //    status = "离职";
                    //}

                  
                    //data.Add(statusField.JDYField, new { value = status });
                    var nameField = GetJDYField(userinfoFrom.Fields, "name");
                    data.Add(nameField.JDYField, new { value = model.Name });
                    var cardidField = GetJDYField(userinfoFrom.Fields, "cardid");
                    data.Add(cardidField.JDYField, new { value = model.CardID });
                    var ageField = GetJDYField(userinfoFrom.Fields, "age");
                    data.Add(ageField.JDYField, new { value = model.Age });
                    var phoneField = GetJDYField(userinfoFrom.Fields, "phone");
                    data.Add(phoneField.JDYField, new { value = model.Phone });
                    var customerField = GetJDYField(userinfoFrom.Fields, "customer");
                    data.Add(customerField.JDYField, new { value = model.CustomerName });
                    var isminientryField = GetJDYField(userinfoFrom.Fields, "isminientry");
                    data.Add(isminientryField.JDYField, new { value = "是" });
                    //var customersField = GetJDYField(userinfoFrom.Fields, "customers");
                    //data.Add(customersField.JDYField, new { value = customer.GroupName });

                    //var recruiterField = GetJDYField(userinfoFrom.Fields, "recruiter");
                    //data.Add(recruiterField.JDYField, new { value = model.PersonnelName });

                    var personnelNameField = GetJDYField(userinfoFrom.Fields, "personnelName");
                    data.Add(personnelNameField.JDYField, new { value = model.PersonnelName });
                    var personnelField = GetJDYField(userinfoFrom.Fields, "personnel");
                    data.Add(personnelField.JDYField, new { value = personnel.JDYNumber });

                    //var creatorField = GetJDYField(userinfoFrom.Fields, "creator");
                    //data.Add(creatorField.JDYField, new { value = personnel.JDYNumber });

                    if (!string.IsNullOrEmpty(personnelItem.JDYJson))
                    {
                        var userinfoSnync = company.JDYForms.Find(it => it.Type == "userinfosnync");
                        if (userinfoSnync != null)
                        {
                            var jsonData = (JObject)JsonConvert.DeserializeObject(personnelItem.JDYJson);
                            foreach (var f in userinfoSnync.Fields)
                            {
                                if (f.FieldType == "array")
                                {
                                    foreach (var item in f.Items)
                                    {
                                        if (jsonData[item.JDYField] != null)
                                        {
                                            data.Add(item.ToJDYField, new { value = jsonData[item.JDYField].ToString() });
                                        }
                                    }
                                }
                                else if (f.FieldType == "json")
                                {
                                    if (jsonData[f.JDYField] != null)
                                    {
                                        data.Add(f.ToJDYField, new { value = jsonData[f.JDYField] });
                                    }

                                }
                                else
                                {
                                    if (jsonData[f.JDYField] != null)
                                    {
                                        data.Add(f.ToJDYField, new { value = jsonData[f.JDYField].ToString() });
                                    }
                                }
                            }
                        }
                    }
                    var personnelPhoneField = GetJDYField(userinfoFrom.Fields, "personnelPhone");
                    data.Add(personnelPhoneField.JDYField, new { value = personnel.Phone });
                    var nationField = GetJDYField(userinfoFrom.Fields, "nation");
                    data.Add(nationField.JDYField, new { value = model.Nation });
                    var emergencyContactField = GetJDYField(userinfoFrom.Fields, "emergencyContact");
                    data.Add(emergencyContactField.JDYField, new { value = model.EmergencyContact });
                    var emergencyContactPhoneField = GetJDYField(userinfoFrom.Fields, "emergencyContactPhone");
                    data.Add(emergencyContactPhoneField.JDYField, new { value = model.EmergencyContactPhone });

                    var setting = await settingsService.GetMasterSettings();
                    var imageParas = new Dictionary<string, object>();
                    imageParas.Add("app_id", userinfoFrom.AppID);
                    imageParas.Add("entry_id", userinfoFrom.FormID);
                    imageParas.Add("transaction_id", transaction_id);
                    var imageToken = await jDYService.GetUploadToken(imageParas, company.Config);
                    var imageTokenObj = (JObject)JsonConvert.DeserializeObject(imageToken);
                    var tokens = imageTokenObj["token_and_url_list"] as JArray;
                    if (tokens == null || tokens.Count == 0)
                    {
                        Common.StaticLog.Debug(imageToken);
                        throw new ErrorCodeException(-1, "获得上传图片token出错");
                    }
                    if (!string.IsNullOrEmpty(model.IdcardFrontImage))
                    {
                        var uploadUrl = tokens[0]["url"].ToString();
                        var uploadToken = tokens[0]["token"].ToString();
                        var image = await Common.Tool.DownloadImageStreamAsync(setting.AliBucketDomain + model.IdcardFrontImage);
                        var imageKey = await jDYService.UploadFileByTokenAsync(uploadUrl, uploadToken, image, "身份证人像面原件", "");
                        var idcardFrontImageField = GetJDYField(userinfoFrom.Fields, "idcardFrontImage");
                        var imageKeyObj = (JObject)JsonConvert.DeserializeObject(imageKey);
                        var keylist = new List<string>();
                        keylist.Add(imageKeyObj["key"].ToString());
                        data.Add(idcardFrontImageField.JDYField, new { value = keylist });
                    }
                    if (!string.IsNullOrEmpty(model.IdcardBackImage))
                    {
                        var uploadUrl = tokens[1]["url"].ToString();
                        var uploadToken = tokens[1]["token"].ToString();
                        var image = await Common.Tool.DownloadImageStreamAsync(setting.AliBucketDomain + model.IdcardBackImage);
                        var imageKey = await jDYService.UploadFileByTokenAsync(uploadUrl, uploadToken, image, "身份证国徽面原件", "");
                        var idcardBackImageField = GetJDYField(userinfoFrom.Fields, "idcardBackImage");
                        var imageKeyObj = (JObject)JsonConvert.DeserializeObject(imageKey);
                        var keylist = new List<string>();
                        keylist.Add(imageKeyObj["key"].ToString());
                        data.Add(idcardBackImageField.JDYField, new { value = keylist });
                    }
                    if (!string.IsNullOrEmpty(model.BankImage))
                    {
                        var uploadUrl = tokens[2]["url"].ToString();
                        var uploadToken = tokens[2]["token"].ToString();
                        var image = await Common.Tool.DownloadImageStreamAsync(setting.AliBucketDomain + model.BankImage);
                        var imageKey = await jDYService.UploadFileByTokenAsync(uploadUrl, uploadToken, image, "银行卡", "");
                        var bankImageField = GetJDYField(userinfoFrom.Fields, "bankImage");
                        var imageKeyObj = (JObject)JsonConvert.DeserializeObject(imageKey);
                        var keylist = new List<string>();
                        keylist.Add(imageKeyObj["key"].ToString());
                        data.Add(bankImageField.JDYField, new { value = keylist });
                    }
                    var bankNumberField = GetJDYField(userinfoFrom.Fields, "bankNumber");
                    data.Add(bankNumberField.JDYField, new { value = model.BankNumber });
                    var sexField = GetJDYField(userinfoFrom.Fields, "sex");
                    data.Add(sexField.JDYField, new { value = model.Sex });
                    var interviewTimeField = GetJDYField(userinfoFrom.Fields, "interviewTime");
                    if (!model.IntDate.HasValue) model.IntDate = DateTime.Now;
                    data.Add(interviewTimeField.JDYField, new { value = model.IntDate.Value.ToString("yyyy-MM-dd") });
                    var birthdayField = GetJDYField(userinfoFrom.Fields, "birthday");
                    data.Add(birthdayField.JDYField, new { value = model.BirthDate });
                    var addressField = GetJDYField(userinfoFrom.Fields, "address");
                    data.Add(addressField.JDYField, new { value = model.IdcardAddress });
                    var authorityField = GetJDYField(userinfoFrom.Fields, "authority");
                    data.Add(authorityField.JDYField, new { value = model.Authority });
                    var cardStartDateField = GetJDYField(userinfoFrom.Fields, "cardStartDate");
                    data.Add(cardStartDateField.JDYField, new { value = model.CardStartDate });
                    var cardEndDateField = GetJDYField(userinfoFrom.Fields, "cardEndDate");
                    data.Add(cardEndDateField.JDYField, new { value = model.CardEndDate });
                    var bankUserNameField = GetJDYField(userinfoFrom.Fields, "bankUserName");
                    data.Add(bankUserNameField.JDYField, new { value = model.Name });
                    var bankNameField = GetJDYField(userinfoFrom.Fields, "bankName");
                    data.Add(bankNameField.JDYField, new { value = model.BankName });
                    //var fromTypeField = GetJDYField(userinfoFrom.Fields, "fromType");
                    //data.Add(fromTypeField.JDYField, new { value = model.FromType });
                    paras.Add("data", data);
                    Common.StaticLog.Debug(JsonConvert.SerializeObject(paras));
                    if (!string.IsNullOrEmpty(model.JDYDataID))
                    {
                        var res = await jDYService.ChangeFormData(paras, company.Config);
                        Common.StaticLog.Debug(res);
                    }
                    else
                    {
                        //if(!string.IsNullOrEmpty(personnel.JDYNumber)) paras.Add("data_creator", personnel.JDYNumber);
                        var res = await jDYService.CreateFormData(paras, company.Config);
                        Common.StaticLog.Debug(res);
                        var resObj = (JObject)JsonConvert.DeserializeObject(res);
                        var dataObj = resObj["data"] as JObject;
                        var JdyDataID = dataObj["_id"].ToString();
                        await entryService.UpdateJDYDataID(model.ID, JdyDataID);
                    }

                }
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);

            }
        }
        /// <summary>
        /// 同步简道云
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        public async Task UserContractSync(int id)
        {
            try
            {
                var model = await entryService.View(it => it.ID == id);
                var company = await companyConfigService.Get(model.CompanyGID);
                if (company.Config.EnableDockJDY && company.JDYForms != null && !string.IsNullOrEmpty(model.JDYDataID) && model.ContractStatus.HasValue)
                {
                    var userinfoFrom = company.JDYForms.Find(it => it.Type == "userinfo");
                    if (userinfoFrom == null)
                    {
                        throw new ErrorCodeException(-1, "未配置简道云人员信息字段信息");
                    }
                    var transaction_id = Guid.NewGuid().ToString();
                    var paras = new Dictionary<string, object>();
                    paras.Add("app_id", userinfoFrom.AppID);
                    paras.Add("entry_id", userinfoFrom.FormID);
                    if (!string.IsNullOrEmpty(model.JDYDataID))
                    {
                        paras.Add("data_id", model.JDYDataID);
                    }
                    paras.Add("transaction_id", transaction_id);
                    var data = new Dictionary<string, object>();

                    var contractStatusField = GetJDYField(userinfoFrom.Fields, "contractStatus");
                    var about= Common.Tool.GetDescription((ContractStatus)model.ContractStatus);
                    data.Add(contractStatusField.JDYField, new { value = about });
                    paras.Add("data", data);
                    var res = await jDYService.ChangeFormData(paras, company.Config);
                }
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);

            }
        }
        /// <summary>
        /// 同步简道云
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        public async Task<Dictionary<string, object>> GetReportInfo(string name,string time,CompanyModel company)
        {
            try
            {
                var reportFrom = company.JDYForms.Find(it => it.Type == "report");
                if (reportFrom == null)
                {
                    throw new ErrorCodeException(-1, "未配置简道云报备信息字段信息");
                }
                var paras = new Dictionary<string, object>();
                paras.Add("app_id", reportFrom.AppID);
                paras.Add("entry_id", reportFrom.FormID);
                paras.Add("limit", 10);
                var fields = reportFrom.Fields.Select(f => f.JDYField);
                paras.Add("fields", fields);
                var filter = new Dictionary<string, object>();
                filter.Add("rel", "and");
                var where = new List<object>();
                var nameField = GetJDYField(reportFrom.Fields, "name");
                where.Add(new { field = nameField.JDYField, type = "text", method = "eq", value = name });
                var timeField = GetJDYField(reportFrom.Fields, "interviewTime");
                var dTime = new List<string>();
                dTime.Add(time);
                where.Add(new { field = timeField.JDYField, type = "datetime", method = "eq", value = dTime });
                filter.Add("cond", where);
                paras.Add("filter", filter);
                var res = await jDYService.QueryMultipleFormData(paras, company.Config);
                Common.StaticLog.Debug(res);
                var resObj = (JObject)JsonConvert.DeserializeObject(res);
                var dataObj = resObj["data"] as JArray;
                if (dataObj.Count > 0)
                {
                    var firstData = dataObj[0] as JObject;
                    var data = new Dictionary<string, object>();
                    var supplierField = GetJDYField(reportFrom.Fields, "supplier");
                    if (firstData[supplierField.JDYField] != null)
                    {
                        data.Add("supplier", firstData[supplierField.JDYField].ToString());
                    }
                    var channelField = GetJDYField(reportFrom.Fields, "channel");
                    if (firstData[channelField.JDYField] != null)
                    {
                        data.Add("channel", firstData[channelField.JDYField].ToString());
                    }
                    return data;
                }
                return null;
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return null;
            }
        }
        /// <summary>
        /// 获取工资记录
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        public async Task<List<Dictionary<string, object>>> GetWageInfo(string cardId, CompanyModel company)
        {
            try
            {
                var reportFrom = company.JDYForms.Find(it => it.Type == "wage");
                if (reportFrom == null)
                {
                    throw new ErrorCodeException(-1, "未配置简道云工资信息字段信息");
                }
                var paras = new Dictionary<string, object>();
                paras.Add("app_id", reportFrom.AppID);
                paras.Add("entry_id", reportFrom.FormID);
                paras.Add("limit", 10);
                var fields = reportFrom.Fields.Select(f => f.JDYField);
                paras.Add("fields", fields);
                var filter = new Dictionary<string, object>();
                filter.Add("rel", "and");
                var where = new List<object>();
                var cardidField = GetJDYField(reportFrom.Fields, "cardid");
                where.Add(new { field = cardidField.JDYField, type = "text", method = "eq", value = cardId });
                filter.Add("cond", where);
                paras.Add("filter", filter);
                var res = await jDYService.QueryMultipleFormData(paras, company.Config);
                Common.StaticLog.Debug(res);
                var resObj = (JObject)JsonConvert.DeserializeObject(res);
                var dataObj = resObj["data"] as JArray;
                var datas = new List<Dictionary<string, object>>();
                foreach ( var item in dataObj)
                {
                    var data = new Dictionary<string, object>();
                    foreach (var field in reportFrom.Fields)
                    {
                        if (item[field.JDYField] != null)
                        {
                            data.Add(field.Name, item[field.JDYField].ToString());
                        }
                    }
                    datas.Add(data);
                }
                return datas;
            }
            catch (Exception ex)
            {
                Common.StaticLog.Error(ex);
                return null;
            }
        }
        public JDYFieldModel GetJDYField(List<JDYFieldModel> list, string name)
        {
            var field = list.Find(it => it.Type == name);
            if (field == null)
            {
                throw new ErrorCodeException(-1, $"未配置简道云{name}字段信息");
            }
            return field;
        }
    }
}
