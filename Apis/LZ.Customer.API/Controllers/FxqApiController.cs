using Consul;
using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Common;
using LZ.Core.EventData;
using LZ.Core.EventData.FxqEventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Redis;
using LZ.Core.Services;
using LZ.Customer.Service.EventHandler;
using LZ.Customer.Service.Interface;
using LZ.Customer.Service.Services;
using LZ.Model;
using LZ.Model.Dtos;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.FxqModel;
using LZ.Model.ModelExtensions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NPOI.HPSF;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using SqlSugar;
using System.ComponentModel.DataAnnotations;
using System.Drawing;
using System.Security.Policy;
using static System.Net.WebRequestMethods;

namespace LZ.Customer.API.Controllers
{
    [AllowAnonymous]
    public class FxqApiController(
        IEventPublisher eventPublisher, 
        IRedisService redisService,  
        IFxqService fxqService,
        IAliyunOSSService aliyunOSSService,
        IContractFileConfigService contractFileConfigService,
        ICustomerContractConfigService customerContractConfigService,
        IContractSignerService  contractSignerService,
        IEnterpriseService   enterpriseService,
        ICustomerService    customerService,
        IContractRecordService contractSignRecordService,
        ISettingsService settingsService,
        IFxqDevService fxqDevService,
        IAuthService authService,
        IHttpService httpService,
        ILogger<FxqApiController> logger


        ) : CustomerControllerBase
    {
        #region Record
        public record ElectronicSealRequest([Required] string Name, [Required] bool IsCompanySeal);
        public record ElectronicSealResponse([Required] string Name, [Required] bool IsCompanySeal, string OssUrl, DateTime Expired);
        public record WordKeywordReplaceRequest([Required] string ossUrl, [Required] Dictionary<string, string> Keywords);
        public record ContractSigner(string Name,string IdNo,bool IsCompany, SignerPartyEnumType SignerPartyType,bool IsNeedSendSms, string SmsPhone,string sealOssFilePath);
        public record GeneratePdfContractByFileTypeRequest([Required] Guid CustomerGID, [Required] ContractEnumType ContractType, [Required] Dictionary<string, string> Keywords, List<ContractSigner> Signers);
        public record GeneratePdfContractByFileIdRequest([Required] Guid CustomerGID, [Required] Guid ContractFileID, [Required] Dictionary<string, string> Keywords, List<ContractSigner> Signers);
        public record RealNamePerson2Request(string Name,string IdCardNo);
        public record RealNamePerson3Request(string Name,string IdCardNo, string Phone);
        public record RealNamePersonBank3Request(string Name, string IdCardNo, string BankNo);
        public record RealNamePersonBank4Request(string Name, string IdCardNo,string BankNo,string Phone);
        public record RealNameCompany3Request(string Name, string CompanyNo, string LegalPerson);
        public record RealNameCompany4Request(string Name, string CompanyNo, string LegalPerson, string Phone);

        public record RealNameMessageRequest(string PersonName, string IdNo, string Phone, string IdType, string RedirectUrl);
        public record CompanyMessageRequest(string LegalName, string LegalIdNo, string CompanyName, string CreditNo, string RedirectUrl);


        #endregion
        /// <summary>
        /// Word文档关键词替换
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [HttpPost]
        public async Task<IActionResult> WordKeywordReplace(WordKeywordReplaceRequest request)
        {
            var kv = request.Keywords.OrderBy(f => f.Key).ToDictionary(f => f.Key, f => f.Value);
            var fxqResp = await fxqService.DocxKeywordReplace(request.ossUrl, kv, 1);
            return Ok(fxqResp);
        }

        [HttpPost]
        public async Task<IActionResult> WordToPdf(WordKeywordReplaceRequest request)
        {
            var kv = request.Keywords.OrderBy(f => f.Key).ToDictionary(f => f.Key, f => f.Value);
            var fxqResp = await fxqService.DocxKeywordReplace(request.ossUrl, kv, 1);
            return Ok(fxqResp);
        }

        #region 实名认证

        /// <summary>
        /// 公安二要素认证（个人）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RealNamePerson2(RealNamePerson2Request request)
        {
            Live_ContractSigner entity = await contractSignerService.Get(f => f.Name == request.Name && f.Idno == request.IdCardNo && f.IsRealNameSuc && f.RealNameType== RealNameEnumType.公安二要素);
            if (entity == null)
            {
                entity = new Live_ContractSigner()
                {
                    ID = Guid.NewGuid(),
                    Name = request.Name,
                    Idno = request.IdCardNo,
                    IsEnabled = false,
                    Expired = DateTime.Now,
                    CreateTime = DateTime.Now,
                    IsRealNameSuc = false,
                    OssUrl = string.Empty,
                    RealNameTime = null,
                    RealNameType =  RealNameEnumType.公安二要素,
                    Remark = string.Empty,
                    LegalPerson = string.Empty,
                    BankNo = string.Empty,
                    LegalPersonIdCardNo = string.Empty,
                    Phone = string.Empty,
                    IsCompany = false
                };
                var realNameResp = await fxqService.IdentityAuthentication2(entity.Name, entity.Idno);
                if (realNameResp == null)
                {
                    entity.Remark = $"{entity.Name}进行{entity.RealNameType},认证失败,返回值为null";
                    entity.IsRealNameSuc = false;
                }
                else
                {
                    if (realNameResp.Data != null && realNameResp.Data.State == 1)
                    {
                        entity.Remark = $"{entity.Name}进行{entity.RealNameType},认证成功";
                        entity.IsRealNameSuc = true;
                        entity.RealNameTime = DateTime.Now;

                        logger.LogInformation(entity.Remark);
                    }
                    else
                    {
                        entity.Remark = $"{entity.Name}进行{entity.RealNameType},认证失败,返回值为:{realNameResp.Msg}";
                        entity.IsRealNameSuc = false;
                        logger.LogInformation(entity.Remark);
                    }
                }
                var failList = await contractSignerService.List(f => f.Name == entity.Name && f.RealNameType == entity.RealNameType && f.Idno == entity.Idno && f.IsRealNameSuc == false);
                if (failList.Any())
                {
                    foreach (var item in failList)
                    {
                        await contractSignerService.Delete(item.ID);
                    }
                }
                await contractSignerService.Add(entity);
                if (entity.IsRealNameSuc)
                {
                    var electronicSignatureEventData = new ElectronicSignatureEventData(entity.Name, entity.Idno, false);
                    // 生成电子签名事件触发
                    eventPublisher.Publish(electronicSignatureEventData);
                }
            }
            return Ok(entity);
        }

        
        /// <summary>
        /// 全网三要素认证（个人）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RealNamePerson3(RealNamePerson3Request request)
        {
            Live_ContractSigner entity = await contractSignerService.Get(f =>
            f.Name == request.Name
            && f.Idno == request.IdCardNo
            && f.Phone == request.Phone
            && f.IsRealNameSuc
            && f.RealNameType == RealNameEnumType.全网三要素);
            if (entity == null)
            {
                entity = new Live_ContractSigner()
                {
                    ID = Guid.NewGuid(),
                    Name = request.Name,
                    Idno = request.IdCardNo,
                    Phone = request.Phone,
                    IsEnabled = false,
                    Expired = DateTime.Now,
                    CreateTime = DateTime.Now,
                    IsRealNameSuc = false,
                    OssUrl = string.Empty,
                    RealNameTime = null,
                    RealNameType = RealNameEnumType.全网三要素,
                    Remark = string.Empty,
                    LegalPerson = string.Empty,
                    BankNo = string.Empty,
                    LegalPersonIdCardNo = string.Empty,
                    IsCompany = false
                };
                var realNameResp = await fxqService.IdentityAuthentication3(entity.Name, entity.Idno, entity.Phone);
                if (realNameResp == null)
                {
                    entity.Remark = $"{entity.Name}进行{entity.RealNameType},认证失败,返回值为null";
                    entity.IsRealNameSuc = false;
                    logger.LogInformation(entity.Remark);
                }
                else
                {
                    if (realNameResp.Data != null && realNameResp.Data.State == 1)
                    {
                        entity.Remark = $"{entity.Name}进行{entity.RealNameType},认证成功";
                        entity.IsRealNameSuc = true;
                        entity.RealNameTime = DateTime.Now;

                        logger.LogInformation(entity.Remark);
                    }
                    else
                    {
                        entity.Remark = $"{entity.Name}进行{entity.RealNameType},认证失败,返回值为:{realNameResp.Msg}";
                        entity.IsRealNameSuc = false;
                        logger.LogInformation(entity.Remark);
                    }
                }
                var failList = await contractSignerService.List(f => f.Name == request.Name
            && f.Idno == entity.Idno
            && f.Phone == entity.Phone
            && f.RealNameType == entity.RealNameType
            && f.IsRealNameSuc == false);
                if (failList.Any())
                {
                    foreach (var item in failList)
                    {
                        await contractSignerService.Delete(item.ID);
                    }
                }
                await contractSignerService.Add(entity);
                logger.LogInformation($"{entity.Name}进行{entity.RealNameType}结束");
                if (entity.IsRealNameSuc)
                {
                    var electronicSignatureEventData = new ElectronicSignatureEventData(entity.Name, entity.Idno, false);
                    // 生成电子签名事件触发
                    eventPublisher.Publish(electronicSignatureEventData);
                }

            }

            return Ok(entity);
        }

        /// <summary>
        /// 银行卡三要素认证（个人）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RealNamePersonBank3(RealNamePersonBank3Request request)
        {
             
                Live_ContractSigner entity = await contractSignerService.Get(f => f.Name == request.Name && f.Idno == request.IdCardNo && f.BankNo == request.BankNo && f.IsRealNameSuc && f.RealNameType== RealNameEnumType.银行卡三要素);
                if (entity == null)
                {
                    entity = new Live_ContractSigner()
                    {
                        ID = Guid.NewGuid(),
                        Name = request.Name,
                        Idno = request.IdCardNo,
                        BankNo = request.BankNo,
                        Phone = string.Empty,


                        IsEnabled = false,
                        Expired = DateTime.Now,
                        CreateTime = DateTime.Now,
                        IsRealNameSuc = false,
                        OssUrl = string.Empty,
                        RealNameTime = null,
                        RealNameType =  RealNameEnumType.银行卡三要素,
                        Remark = string.Empty,
                        LegalPerson = string.Empty,
                        LegalPersonIdCardNo = string.Empty,
                    };
                    var realNameResp = await fxqService.IdentityBankAuthentication3(request.Name, request.IdCardNo, request.BankNo);
                    if (realNameResp == null)
                    {
                        entity.Remark = $"{request.Name}进行{entity.RealNameType},认证失败,返回值为null";
                        entity.IsRealNameSuc = false;
                        logger.LogInformation(entity.Remark);
                    }
                    else
                    {
                        if (realNameResp.Data != null && realNameResp.Data.State == 1)
                        {
                            entity.Remark = $"{entity.Name}进行{entity.RealNameType},认证成功";
                            entity.IsRealNameSuc = true;
                            entity.RealNameTime = DateTime.Now;

                            logger.LogInformation(entity.Remark);
                        }
                        else
                        {
                            entity.Remark = $"{entity.Name}进行{entity.RealNameType},认证失败,返回值为:{realNameResp.Msg}";
                            entity.IsRealNameSuc = false;
                            logger.LogInformation(entity.Remark);
                        }
                    }
                    var failList = await contractSignerService.List(f => f.Name == entity.Name && f.BankNo == request.BankNo && f.RealNameType == entity.RealNameType && f.Idno == entity.Idno && f.IsRealNameSuc == false);
                    if (failList.Any())
                    {
                        foreach (var item in failList)
                        {
                            await contractSignerService.Delete(item.ID);
                        }
                    }
                    await contractSignerService.Add(entity);
                    logger.LogInformation($"{request.Name}进行{entity.RealNameType}结束");
                    if (entity.IsRealNameSuc)
                    {
                        var electronicSignatureEventData = new ElectronicSignatureEventData(entity.Name, entity.Idno, false);
                        // 生成电子签名事件触发
                        eventPublisher.Publish(electronicSignatureEventData);
                    }

                }
            return Ok();
        }

        /// <summary>
        /// 银行卡四要素认证（个人）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RealNamePersonBank4(RealNamePersonBank4Request request)
        {
           
                
                Live_ContractSigner entity = await contractSignerService.Get(f =>
                f.Name == request.Name
                && f.Idno == request.IdCardNo
                && f.BankNo == request.BankNo
                && f.Phone == request.Phone
                && f.IsRealNameSuc
                && f.RealNameType== RealNameEnumType.银行卡四要素);
                if (entity == null)
                {
                    entity = new Live_ContractSigner()
                    {
                        ID = Guid.NewGuid(),
                        Name = request.Name,
                        Idno = request.IdCardNo,
                        BankNo = request.BankNo,
                        Phone = request.Phone,
                        IsEnabled = false,
                        Expired = DateTime.Now,
                        CreateTime = DateTime.Now,
                        IsRealNameSuc = false,
                        OssUrl = string.Empty,
                        RealNameTime = null,
                        RealNameType =  RealNameEnumType.银行卡四要素,
                        Remark = string.Empty,
                        LegalPerson = string.Empty,
                        LegalPersonIdCardNo = string.Empty,
                    };
                    var realNameResp = await fxqService.IdentityBankAuthentication4(request.Name, request.IdCardNo, request.BankNo, request.Phone);
                    if (realNameResp == null)
                    {
                        entity.Remark = $"{request.Name}进行{entity.RealNameType},认证失败,返回值为null";
                        entity.IsRealNameSuc = false;
                        logger.LogInformation(entity.Remark);
                    }
                    else
                    {
                        if (realNameResp.Data != null && realNameResp.Data.State == 1)
                        {
                            entity.Remark = $"{entity.Name}进行{entity.RealNameType},认证成功";
                            entity.IsRealNameSuc = true;
                            entity.RealNameTime = DateTime.Now;
                            logger.LogInformation(entity.Remark);
                        }
                        else
                        {
                            entity.Remark = $"{entity.Name}进行{entity.RealNameType},认证失败,返回值为:{realNameResp.Msg}";
                            entity.IsRealNameSuc = false;
                            logger.LogInformation(entity.Remark);
                        }
                    }
                    var failList = await contractSignerService.List(f =>
                    f.Name == request.Name
                && f.Idno == request.IdCardNo
                && f.BankNo == request.BankNo
                && f.Phone == request.Phone
                && f.IsRealNameSuc == false);
                    if (failList.Any())
                    {
                        foreach (var item in failList)
                        {
                            await contractSignerService.Delete(item.ID);
                        }
                    }
                    await contractSignerService.Add(entity);
                    logger.LogInformation($"{entity.Name}进行{entity.RealNameType}结束");
                    if (entity.IsRealNameSuc)
                    {
                        var electronicSignatureEventData = new ElectronicSignatureEventData(entity.Name, entity.Idno, false);
                        // 生成电子签名事件触发
                        eventPublisher.Publish(electronicSignatureEventData);
                    }

                }
            return Ok(entity);
        }

        /// <summary>
        /// 个人实名CA认证
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<string> PersonRealNameMessage(RealNameMessageRequest request)
        {
           
            Live_ContractSigner entity = await contractSignerService.Get(f => f.Name == request.PersonName && f.Idno == request.IdNo && f.IsRealNameSuc && f.RealNameType == RealNameEnumType.个人实名CA认证);
            if (entity == null)
            {
                entity = new Live_ContractSigner()
                {
                    ID = Guid.NewGuid(),
                    Name = request.PersonName,
                    Idno = request.IdNo,
                    IsEnabled = false,
                    Expired = DateTime.Now,
                    CreateTime = DateTime.Now,
                    IsRealNameSuc = false,
                    OssUrl = string.Empty,
                    RealNameTime = null,
                    RealNameType = RealNameEnumType.个人实名CA认证,
                    Remark = string.Empty,
                    LegalPerson = string.Empty,
                    BankNo = string.Empty,
                    LegalPersonIdCardNo = string.Empty,
                    Phone = request.Phone,
                    IsCompany = false,
                };
                string redirectUrl = request.RedirectUrl;
                var realNameResp = await fxqService.PersonIdentityMessageAuthentication(entity.Name, entity.Idno, entity.Phone, "0", redirectUrl);
                if (realNameResp == null)
                {
                    entity.Remark = $"{entity.Name}进行{entity.RealNameType},认证失败,返回值为null";
                    entity.IsRealNameSuc = false;
                }
                else
                {
                    return JsonConvert.SerializeObject(realNameResp); ;
                    //if (realNameResp.Code == 10000 && realNameResp.Data.Success)
                    //{
                    //    entity.Remark = $"{entity.Name}进行{entity.RealNameType},请求成功";
                    //    entity.IsRealNameSuc = true;
                    //    entity.RealNameTime = DateTime.Now;
                    //    entity.ApplyID = realNameResp.Data.ApplyId;
                    //    logger.LogInformation(entity.Remark);
                    //}
                    //else
                    //{
                    //    entity.Remark = $"{entity.Name}进行{entity.RealNameType},认证失败,状态码为:{realNameResp.Code},返回信息为:{realNameResp.Data.Msg}";
                    //    entity.IsRealNameSuc = false;
                    //    logger.LogInformation(entity.Remark);
                    //}
                }
                var failList = await contractSignerService.List(f => f.Name == entity.Name && f.RealNameType == entity.RealNameType && f.Idno == entity.Idno && f.IsRealNameSuc == false);
                if (failList.Any())
                {
                    foreach (var item in failList)
                    {
                        await contractSignerService.Delete(item.ID);
                    }
                }
                await contractSignerService.Add(entity);
                if (entity.IsRealNameSuc)
                {
                    var electronicSignatureEventData = new ElectronicSignatureEventData(entity.Name, entity.Idno, false);
                    // 生成电子签名事件触发
                    eventPublisher.Publish(electronicSignatureEventData);
                }
            }
            return JsonConvert.SerializeObject(entity);
        }

        /// <summary>
        /// 企业三要素认证（企业）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RealNameCompany3(RealNameCompany3Request request)
        {


           
              
                Live_ContractSigner entity = await contractSignerService.Get(f =>
                f.Name == request.Name
                && f.Idno == request.CompanyNo
                && f.LegalPerson == request.LegalPerson
                && f.IsRealNameSuc
                &&  f.RealNameType== RealNameEnumType.企业三要素);
                if (entity == null)
                {
                    entity = new Live_ContractSigner()
                    {
                        ID = Guid.NewGuid(),
                        Name = request.Name,
                        Idno = request.CompanyNo,
                        LegalPerson = request.LegalPerson,
                         
                        IsCompany = true,
                        Phone = string.Empty,
                        IsEnabled = false,
                        Expired = DateTime.Now,
                        CreateTime = DateTime.Now,
                        IsRealNameSuc = false,
                        OssUrl = string.Empty,
                        RealNameTime = null,
                        RealNameType =  RealNameEnumType.企业三要素,
                        Remark = string.Empty,
                        BankNo = string.Empty,
                        LegalPersonIdCardNo = string.Empty,
                    };
                var realNameResp = await fxqService.CompanyAuthentication3(request.Name, request.CompanyNo, request.LegalPerson);
                if (realNameResp != null)
                {
                    if (realNameResp.Code == 10000)
                    {
                        if (realNameResp.Data.Status == 1)
                        {
                            entity.RealNameTime = DateTime.Now;
                            entity.IsRealNameSuc = true;
                        }
                        else
                        {
                            entity.IsRealNameSuc = false;
                            if (realNameResp.Data.Status == 2)
                            {
                                entity.Remark = "全不匹配";
                            }
                            else if (realNameResp.Data.Status == 12)
                            {
                                entity.Remark = "公司匹配|法人不匹配";
                            }
                            else if (realNameResp.Data.Status == 13)
                            {
                                entity.Remark = "公司不匹配|法人匹配";
                            }
                        }
                    }
                    else
                    {
                        entity.Remark = $"{request.Name}进行{entity.RealNameType},认证失败,返回值为:{realNameResp.Msg}";
                        entity.IsRealNameSuc = false;

                    }
                    logger.LogInformation(entity.Remark);

                }
                else
                {
                    entity.Remark = $"{request.Name}进行{entity.RealNameType},认证失败,返回值为null";
                    entity.IsRealNameSuc = false;
                    logger.LogInformation(entity.Remark);
                }
                logger.LogInformation(entity.Remark);

            }
            else
            {
                entity.Remark = $"{request.Name}进行{entity.RealNameType},认证失败,返回值为null";
                entity.IsRealNameSuc = false;
                logger.LogInformation(entity.Remark);
            }
            var failList = await contractSignerService.List(f => f.Name == request.Name
        && f.Name == request.Name
        && f.Idno == request.CompanyNo
        && f.LegalPerson == request.LegalPerson
        && f.RealNameType == entity.RealNameType
        && f.IsRealNameSuc == false);
            if (failList.Any())
            {
                foreach (var item in failList)
                {
                    await contractSignerService.Delete(item.ID);
                }
            }
            await contractSignerService.Add(entity);
            logger.LogInformation($"{request.Name}进行{entity.RealNameType}结束");
            if (entity.IsRealNameSuc)
            {
                var electronicSignatureEventData = new ElectronicSignatureEventData(entity.Name, entity.Idno, true);
                // 生成电子签名事件触发
                eventPublisher.Publish(electronicSignatureEventData);
            }

            return Ok(entity);
        }

        /// <summary>
        /// 企业四要素认证（企业）
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> RealNameCompany4(RealNameCompany4Request request)
        {
                Live_ContractSigner entity = await contractSignerService.Get(f =>
                f.Name == request.Name
                && f.Idno == request.CompanyNo
                && f.LegalPerson == request.LegalPerson
                && f.IsRealNameSuc
                &&  f.RealNameType== RealNameEnumType.企业四要素);
                if (entity == null)
                {
                    entity = new Live_ContractSigner()
                    {
                        ID = Guid.NewGuid(),
                        Name = request.Name,
                        Idno = request.CompanyNo,
                        LegalPerson = request.LegalPerson,

                        IsCompany = true,
                        Phone = string.Empty,
                        IsEnabled = false,
                        Expired = DateTime.Now,
                        CreateTime = DateTime.Now,
                        IsRealNameSuc = false,
                        OssUrl = string.Empty,
                        RealNameTime = null,
                        RealNameType =  RealNameEnumType.企业四要素,
                        Remark = string.Empty,
                        BankNo = string.Empty,
                        LegalPersonIdCardNo = string.Empty,
                    };

                    var realNameResp = await fxqService.CompanyAuthentication4(entity.Name, entity.Idno, entity.LegalPerson, entity.LegalPersonIdCardNo);

                    if (realNameResp != null)
                    {
                    if (realNameResp.Code == 10000)
                    {

                        if (realNameResp.Data.entNameMatch == 1
                            && realNameResp.Data.entRegNoMatch == 1
                            && realNameResp.Data.repNameMatch == 1
                            && realNameResp.Data.repIdNoMatch == 1)
                        {
                            entity.IsRealNameSuc = true;
                            entity.RealNameTime = DateTime.Now;
                        }
                        else
                        {
                            if (realNameResp.Data.entNameMatch == 2)
                            {
                                entity.Remark += "|公司名称不匹配";
                            }
                            if (realNameResp.Data.entRegNoMatch == 2)
                            {
                                entity.Remark += "|工商注册号不匹配";
                            }
                            if (realNameResp.Data.repNameMatch == 2)
                            {
                                entity.Remark += "|法人姓名不匹配";
                            }
                            if (realNameResp.Data.repIdNoMatch == 2)
                            {
                                entity.Remark += "|法人身份证不匹配";
                            }

                        }

                    }
                    else
                    {
                        entity.Remark = $"{entity.Name}进行{entity.RealNameType},认证失败,返回值为:{realNameResp.Msg}";
                        entity.IsRealNameSuc = false;

                    }
                    logger.LogInformation(entity.Remark);

                }
                    else
                    {
                        entity.Remark = $"{entity.Name}进行{entity.RealNameType},认证失败,返回值为null";
                        entity.IsRealNameSuc = false;
                        logger.LogInformation(entity.Remark);
                    }



                var failList = await contractSignerService.List(f => f.Name == entity.Name
            && f.Idno == entity.Idno
            && f.Name == entity.Name
            && f.LegalPerson == entity.LegalPerson
            && f.LegalPersonIdCardNo == entity.LegalPersonIdCardNo
            && f.RealNameType == entity.RealNameType
            && f.IsRealNameSuc == false);
                if (failList.Any())
                {
                    foreach (var item in failList)
                    {
                        await contractSignerService.Delete(item.ID);
                    }
                }
                await contractSignerService.Add(entity);
                logger.LogInformation($"{entity.Name}进行{entity.RealNameType}结束");
                if (entity.IsRealNameSuc)
                {
                    var electronicSignatureEventData = new ElectronicSignatureEventData(entity.Name, entity.Idno, entity.IsCompany);
                    // 生成电子签名事件触发
                    eventPublisher.Publish(electronicSignatureEventData);
                }
            }


            return Ok(entity);
        }
        #endregion



        [HttpPost]
        public async Task<IActionResult> GeneratePdfContractByFileID(GeneratePdfContractByFileIdRequest request)
        {

            var setting =await settingsService.GetMasterSettings();

            Live_ContractFileConfigExtension contractFile = null;
            List<Live_ContractRecordSigner> contractRecordSigners = new List<Live_ContractRecordSigner>();
            Live_ContractRecordExtension contractRecord = null;

             var customer = await customerService.Get(f=>f.CustomerGID== request.CustomerGID);
            // var customerConfig = await customerContractConfigService.GetByCustomerGID(request.CustomerGID);

            //if (customerConfig == null)
            //    throw new ErrorCodeException(-1, "客户合同文件未配置！");

            contractFile = await contractFileConfigService.GetNav(f => f.ID == request.ContractFileID);
            if (contractFile==null)
                throw new ErrorCodeException(-1,"合同文件不存在！");
            Live_ContractSigner contractSigner;
            if (request.Signers != null)
            {
                foreach (var signerItem in request.Signers)
                {
                    contractSigner = await contractSignerService.GetSigner(signerItem.Name, signerItem.IdNo, signerItem.IsCompany);
                    if (contractSigner == null || !contractSigner.IsEnabled)
                    {
                        if (signerItem.IsCompany)
                        {
                            throw new ErrorCodeException(-1, $"{signerItem.Name}企业认证未通过，无法生成合同！");
                        }
                        else
                        {
                            throw new ErrorCodeException(-1, $"{signerItem.Name}个人实名认证未通过，无法生成合同！");
                        }
                    }
                    if (contractFile.Signers != null && contractFile.Signers.Any(f => f.SignerPartyType == signerItem.SignerPartyType))
                    {
                        contractRecordSigners = contractFile.Signers.Select(f => new Live_ContractRecordSigner()
                        {
                            SignerName = contractSigner.Name,
                            SignerIdno = contractSigner.Idno,
                            ContractSignerID = contractSigner.ID,
                            IsContainSignDate = f.IsSignTime,
                            PageSignHeight = f.PageSignHeight,
                            SignKeyword = f.SignKeyword,
                            SignOssUrl = string.IsNullOrWhiteSpace(signerItem.sealOssFilePath) ? contractSigner.OssUrl: signerItem.sealOssFilePath,
                            SignPartyType = signerItem.SignerPartyType,
                            CustomerGID = request.CustomerGID,
                            SmsPhone = signerItem.SmsPhone,
                            FxqSmsTime = string.Empty,
                            Remark = string.Empty,
                            SmsCode = string.Empty,
                            SmsContent = string.Empty,
                            SmsSendTime = null,
                            FxqSmsId = string.Empty,

                        }).ToList();
                    }
                    else {
                        throw new ErrorCodeException(-1, $"{signerItem.SignerPartyType}合同文件签署方未配置！");
                    }
                }
            }
            contractRecord = new Live_ContractRecordExtension()
            {

                BatchNo = Guid.NewGuid(),
                StartDate = DateTime.Today,
                TemplateOssUrl = contractFile.FilePath,
                ContractOssUrl = contractFile.FilePath,
                ContractType = contractFile.ContractEnumType,
                CreateTime = DateTime.Now,
                ContractEndDate = null,
                ContractStartDate = null,
                CustomerCompanyNo = string.Empty,
                CustomerGID = customer.CustomerGID,
                CustomerName = customer.CustomerName,
                IsNeedKeywordReplace = false,
                EndDate = null,
                FxqHash = string.Empty,
                FxqSignedOssUrl = string.Empty,
                FxqTransactionId = string.Empty,
                KeywordReplaceContent = string.Empty,
                RelateGID = null,
                RelateID = null,
                Remark = string.Empty,
            };

            if (contractRecordSigners.Any())
            {
                contractRecord.Signers=contractRecordSigners;
            }

            if (request.Keywords!=null&& request.Keywords.Count>0)
            {
                contractRecord.IsNeedKeywordReplace = true  ;
                contractRecord.KeywordReplaceContent=Newtonsoft.Json.JsonConvert.SerializeObject(request.Keywords);
            }

            // 关键词替换
            if (contractRecord.IsNeedKeywordReplace)
            {
                var docxResp = await fxqService.DocxKeywordReplace(contractFile.FilePath, request.Keywords, 1);
                if (docxResp != null)
                {
                    if (docxResp.Code == 10000)
                    {
                        contractRecord.ContractOssUrl = docxResp.Data;
                    }
                    else
                    {
                        throw new ErrorCodeException(-1, docxResp.Msg);
                    }
                }
                else
                {

                    throw new ErrorCodeException(-1, "合同关键词替换失败");
                }
            }

            // 合同文件下载

            var docxFileBase64 = await fxqService.DownloadFile(contractRecord.ContractOssUrl);
            if (string.IsNullOrWhiteSpace(docxFileBase64))
            {
                throw new ErrorCodeException(-1, "合同文件下载失败");
            }

            var pdfResp = await fxqService.DocxToPdf(docxFileBase64);

            if (pdfResp != null)
            {
                if (pdfResp.Code == 1000)
                {
                    var pdfFileBase64 = Convert.FromBase64String(pdfResp.Data);
                    using (var ms = new MemoryStream(pdfFileBase64))
                    {
                        var ossResp = await aliyunOSSService.UploadFile(ms, $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.pdf", true, false, "temps");
                        if (ossResp.Code == 0)
                        {
                            contractRecord.ContractOssUrl = setting.AliBucketDomain + ossResp.Data;
                        }
                        else
                        {
                            throw new ErrorCodeException(-1, "合同文件上传Oss服务器失败");
                        }
                    }
                }
                else
                {
                    throw new ErrorCodeException(-1, "合同文件转换PDF 失败");
                }
            }



            if (request.Signers!=null&& request.Signers.Any(f => f.IsNeedSendSms))
            {
                contractRecord.Status = SignStatusEnumType.待发送短信;
            }
            else
            {
                contractRecord.Status = SignStatusEnumType.待签署;
            }


            await contractSignRecordService.AddNav(contractRecord);
            return Ok(contractRecord);
        }

        [HttpPost]
        public async Task<IActionResult> GeneratePdfContractByFileType(GeneratePdfContractByFileTypeRequest request)
        {
            var setting = await settingsService.GetMasterSettings();

            Live_ContractFileConfigExtension contractFile = null;
            List<Live_ContractRecordSigner> contractRecordSigners = new List<Live_ContractRecordSigner>();
            Live_ContractRecordExtension contractRecord = null;

            var customer = await customerService.Get(f => f.CustomerGID == request.CustomerGID);
            var customerConfig = await customerContractConfigService.GetByCustomerGID(request.CustomerGID);

            if (customerConfig == null)
                throw new ErrorCodeException(-1, "客户合同文件未配置！");

            switch (request.ContractType)
            {
                case ContractEnumType.入职声明:
                    contractFile = await contractFileConfigService.GetNav(f => f.ID == customerConfig.EntryDeclareFileID.Value);
                    break;
                case ContractEnumType.入职合同:
                    contractFile = await contractFileConfigService.GetNav(f => f.ID == customerConfig.EntryFileID.Value);


                    break;
                case ContractEnumType.离职合同:
                    contractFile = await contractFileConfigService.GetNav(f => f.ID == customerConfig.LeaveFileID.Value);

                    break;
                //case ContractEnumType.入职返聘合同:
                //    contractFile = await contractFileConfigService.GetNav(f => f.ID == customerConfig.ReEntryFileID.Value);
                //    break;
                //case ContractEnumType.离职返聘合同:
                case ContractEnumType.现金签收单:
                case ContractEnumType.借款单个人:
                case ContractEnumType.借款单公司:
                case ContractEnumType.工伤事故报告:
                case ContractEnumType.出险通知书:
                case ContractEnumType.赔偿协议:
                case ContractEnumType.领取赔款授权书:
                    contractFile = await contractFileConfigService.GetNav(f => f.ContractEnumType == request.ContractType);
                    break;
                default:
                    break;
            }
            if (contractFile == null)
                throw new ErrorCodeException(-1, "合同文件不存在！");
            Live_ContractSigner contractSigner;
            if (request.Signers != null)
            {
                foreach (var signerItem in request.Signers)
                {
                    contractSigner = await contractSignerService.GetSigner(signerItem.Name, signerItem.IdNo, signerItem.IsCompany);
                    if (contractSigner == null || !contractSigner.IsEnabled)
                    {
                        if (signerItem.IsCompany)
                        {
                            throw new ErrorCodeException(-1, $"{signerItem.Name}企业认证未通过，无法生成合同！");
                        }
                        else
                        {
                            throw new ErrorCodeException(-1, $"{signerItem.Name}个人实名认证未通过，无法生成合同！");
                        }
                    }

                    if (contractFile.Signers != null && contractFile.Signers.Any(f => f.SignerPartyType == signerItem.SignerPartyType))
                    {
                        contractRecordSigners = contractFile.Signers.Select(f => new Live_ContractRecordSigner()
                        {
                            SignerName = contractSigner.Name,
                            SignerIdno = contractSigner.Idno,
                            ContractSignerID = contractSigner.ID,
                            IsContainSignDate = f.IsSignTime,
                            PageSignHeight = f.PageSignHeight,
                            SignKeyword = f.SignKeyword,
                            SignOssUrl = string.IsNullOrWhiteSpace(signerItem.sealOssFilePath) ? contractSigner.OssUrl : signerItem.sealOssFilePath,
                            SignPartyType = signerItem.SignerPartyType,
                            CustomerGID = request.CustomerGID,
                            SmsPhone = signerItem.SmsPhone,
                            FxqSmsTime = string.Empty,
                            Remark = string.Empty,
                            SmsCode = string.Empty,
                            SmsContent = string.Empty,
                            SmsSendTime = null,
                            FxqSmsId = string.Empty,

                        }).ToList();
                    }
                    else
                    {
                        throw new ErrorCodeException(-1, $"{signerItem.SignerPartyType}合同文件签署方未配置！");
                    }
                }
            }


            contractRecord = new Live_ContractRecordExtension()
            {

                BatchNo = Guid.NewGuid(),
                StartDate = DateTime.Today,
                TemplateOssUrl = contractFile.FilePath,
                ContractOssUrl = contractFile.FilePath,
                ContractType = contractFile.ContractEnumType,
                CreateTime = DateTime.Now,
                ContractEndDate = null,
                ContractStartDate = null,
                CustomerCompanyNo = string.Empty,
                CustomerGID = customer.CustomerGID,
                CustomerName = customer.CustomerName,
                IsNeedKeywordReplace = false,
                EndDate = null,
                FxqHash = string.Empty,
                FxqSignedOssUrl = string.Empty,
                FxqTransactionId = string.Empty,
                KeywordReplaceContent = string.Empty,
                RelateGID = null,
                RelateID = null,
                Remark = string.Empty,
            };

            if (contractRecordSigners.Any())
            {
                contractRecord.Signers = contractRecordSigners;
            }

            if (request.Keywords != null && request.Keywords.Count > 0)
            {
                contractRecord.IsNeedKeywordReplace = true;
                contractRecord.KeywordReplaceContent = Newtonsoft.Json.JsonConvert.SerializeObject(request.Keywords);
            }

            // 关键词替换
            if (contractRecord.IsNeedKeywordReplace)
            {
                var docxResp = await fxqService.DocxKeywordReplace(contractFile.FilePath, request.Keywords, 1);
                if (docxResp != null)
                {
                    if (docxResp.Code == 10000)
                    {
                        contractRecord.ContractOssUrl = docxResp.Data;
                    }
                    else
                    {
                        throw new ErrorCodeException(-1, docxResp.Msg);
                    }
                }
                else
                {

                    throw new ErrorCodeException(-1, "合同关键词替换失败");
                }
            }

            // 合同文件下载

            var docxFileBase64 = await fxqService.DownloadFile(contractRecord.ContractOssUrl);
            if (string.IsNullOrWhiteSpace(docxFileBase64))
            {
                throw new ErrorCodeException(-1, "合同文件下载失败");
            }

            var pdfResp = await fxqService.DocxToPdf(docxFileBase64);

            if (pdfResp != null)
            {
                if (pdfResp.Code == 1000)
                {
                    var pdfFileBase64 = Convert.FromBase64String(pdfResp.Data);
                    using (var ms = new MemoryStream(pdfFileBase64))
                    {
                        var ossResp = await aliyunOSSService.UploadFile(ms, $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.pdf", true, false, "temps");
                        if (ossResp.Code == 0)
                        {
                            contractRecord.ContractOssUrl = setting.AliBucketDomain + ossResp.Data;
                        }
                        else
                        {
                            throw new ErrorCodeException(-1, "合同文件上传Oss服务器失败");
                        }
                    }
                }
                else
                {
                    throw new ErrorCodeException(-1, "合同文件转换PDF 失败");
                }
            }

            if (request.Signers != null && request.Signers.Any(f => f.IsNeedSendSms))
            {
                contractRecord.Status = SignStatusEnumType.待发送短信;
            }
            else
            {
                contractRecord.Status = SignStatusEnumType.待签署;
            }

            contractRecord.Status = SignStatusEnumType.待发送短信;
            await contractSignRecordService.AddNav(contractRecord);
            return Ok(contractRecord);
        }

        [HttpPost]
        public async Task<IActionResult> DayWorkContractCreate([FromForm] int id)
        {
            var job = await httpService.GetAsync<Model.Live_JobsDayWork>(ServiceNames.CustomerService, "/api/jobsdaywork/get?id=" + id);
            if (job == null)
            {
                throw new ErrorCodeException(-1, "日结岗位不存在！");
            }
            var user = CurrentUser;
            var umodel = await authService.UserInfo<Model.Live_User>();
            string ossUrl = "https://oss.cn-labor.cn//files/20260108/76f3f7e359eb4c87bdfd499fa3d40869.docx";//入职合同
            string docxBase64 = "";
            using (HttpClient client = new HttpClient())
            {
                var imageBytes = await client.GetByteArrayAsync(ossUrl);
                docxBase64 = Convert.ToBase64String(imageBytes);
            }
            Dictionary<string, string> dic = new Dictionary<string, string>();
            dic.Add("cardid", umodel.CardID??"");
            dic.Add("name", umodel.FullName??"");
           
            //var path = await fxqDevService.DocxKeywordReplace(ossUrl, dic, 1);
            var pathBase = await fxqDevService.CombineDocxKeywordToPdf(docxBase64, dic);
            if (pathBase.Code != 200)
            {
                throw new ErrorCodeException(-1, "生成协议报错");
            }
            var items = new { note = ossUrl, contract = pathBase.Data, jobNo = job.JobNo + '-' + Tool.RandomName() };
            return Ok(new { items });
        }






    }
}
