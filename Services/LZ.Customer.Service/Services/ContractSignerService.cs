using LZ.Model.PagedModel;
using LZ.Model;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Senparc.Weixin.WxOpen.AdvancedAPIs.DataCube;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Customer.Service.Interface;
using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Core.Services;
using LZ.Model.Exceptions;
using Microsoft.AspNetCore.Mvc;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;

namespace LZ.Customer.Service.Services
{
    public class ContractSignerService : ServicesBase<Live_ContractSigner>, IContractSignerService, IScopedDependency
    {

        private readonly IAliyunOSSService _oSSService;
        private readonly ISettingsService _settingsService;
        private readonly IFxqService _fxqService;

        public ContractSignerService(IFxqService fxqService, ISettingsService settingsService, IAliyunOSSService oSSService)
        {
            _fxqService = fxqService;
            _settingsService = settingsService;
            _oSSService = oSSService;
        }

        public async Task<List<Live_ContractSigner>> PageList(Expression<Func<Live_ContractSigner, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_ContractSigner>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_ContractSigner>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<List<Live_ContractSigner>> List(Expression<Func<Live_ContractSigner, bool>> exp)
        {
            return await base.Context.Queryable<Live_ContractSigner>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }
        public async Task<int> Add(Live_ContractSigner model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Guid id)
        {
            return await base.Context.Queryable<Live_ContractSigner>().AnyAsync(it => it.ID == id);
        }
       
        public async Task<bool> Update(Live_ContractSigner model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Update(List<Live_ContractSigner> models)
        {
            return await base.Context.Updateable(models).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }


        public async Task<Live_ContractSigner> Get(Expression<Func<Live_ContractSigner, bool>> exp)
        {
            return await base.Context.Queryable<Live_ContractSigner>().OrderByDescending(f=>f.CreateTime).FirstAsync(exp);
        }


        public async Task<Live_ContractSigner> Get(Guid id)
        {
            return await base.Context.Queryable<Live_ContractSigner>().FirstAsync(it => it.ID == id);
        }
        public async Task<bool> Delete(Guid id)
        {
            return await base.Context.Deleteable<Live_ContractSigner>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }



        public async Task<Live_ContractSigner> GetSigner(string name, string idno, bool isCompany)
        {
            var setting = await _settingsService.GetMasterSettings();
            var record = await Get(f => string.Equals(f.Idno, idno, StringComparison.OrdinalIgnoreCase) && !string.IsNullOrWhiteSpace(f.OssUrl));
            if (record == null)
            {
                record = new Live_ContractSigner()
                {
                    CreateTime = DateTime.Now,
                    ApplyID = string.Empty,
                    BankNo = string.Empty,
                    Expired = DateTime.MinValue,
                    ID = Guid.Empty,
                    Idno = idno,
                    IsCompany = isCompany,
                    IsEnabled = true,
                    IsRealNameSuc = true,
                    LegalPerson = string.Empty,
                    LegalPersonIdCardNo = string.Empty,
                    Name = name,
                    OssUrl = string.Empty,
                    Phone = string.Empty,
                    RealNameTime = DateTime.MinValue,
                    RealNameType = null,
                    Remark = string.Empty,
                };
            }
            if (!string.IsNullOrWhiteSpace(record.OssUrl))
            {
                return record;
            }
            string signerOssUrl = string.Empty;
            if (record.IsCompany)
            {
                var resp = await _fxqService.CompanyElectronicSeal(record.Name, null, record.Idno, null, null, null, 0, null);
                if (resp != null && resp.Code == 10000)
                {
                    var fileBase64 = Convert.FromBase64String(resp.Data.Replace("data:image/png;base64,", ""));
                    using (var ms = new MemoryStream(fileBase64))
                    {
                        var ossResp = await _oSSService.UploadFile(ms, $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.png", true, false, "seals");
                        if (ossResp.Code == 0)
                        {
                            signerOssUrl = setting.AliBucketDomain.TrimEnd('/') + ossResp.Data;
                        }
                        else
                        {
                            signerOssUrl = string.Empty;
                            //throw new ErrorCodeException(-1, "电子签名获取成功，上传阿里云Oss 失败！");

                            return null;
                        }
                    }
                }
                else
                {
                    //throw new ErrorCodeException(-1, $"电子签名获取失败！异常状态码：{resp.Code},异常信息：{resp.Msg}");
                    return null;
                }
            }
            else
            {
                var resp = await _fxqService.ElectronicSeal(record.Name, 0, 0, 0, 0);
                if (resp != null && resp.Code == 10000)
                {
                    var fileBase64 = Convert.FromBase64String(resp.Data.Replace("data:image/png;base64,", ""));
                    using (var ms = new MemoryStream(fileBase64))
                    {
                        var ossResp = await _oSSService.UploadFile(ms, $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.png", true, false);
                        if (ossResp.Code == 0)
                        {
                            signerOssUrl = setting.AliBucketDomain.TrimEnd('/') + ossResp.Data;
                        }
                        else
                        {
                            //throw new ErrorCodeException(-1, "电子签名获取成功，上传阿里云Oss 失败！");
                               return null;
                        }
                    }
                }
                else
                {
                    //throw new ErrorCodeException(-1, $"电子签名获取失败！异常状态码：{resp.Code},异常信息：{resp.Msg}");
                    return null;
                }
            }

            if (string.IsNullOrWhiteSpace(signerOssUrl))
            {
                //throw new ErrorCodeException(-1, "电子章生成失败！");
                return null;
            }
            else
            {
                record.OssUrl = signerOssUrl;
                if (record.ID != Guid.Empty)
                {
                    await Update(record);
                }
                else
                {
                    record.ID = Guid.NewGuid();
                    await Add(record);
                }
                return record;
            }

        }
    }
}
