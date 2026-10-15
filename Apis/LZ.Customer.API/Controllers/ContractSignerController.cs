using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Core.EventData;
using LZ.Core.EventData.Contract;
using LZ.Core.EventData.FxqEventData;
using LZ.Core.Interface;
using LZ.Core.RabbitMQ;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Services;
using LZ.Customer.Service.Interface;
using LZ.Customer.Service.Services;
using LZ.Model;
using LZ.Model.Dtos;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.FxqModel;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Quartz.Util;
using Senparc.CO2NET.Extensions;
using SqlSugar;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace LZ.Customer.API.Controllers
{

    public class ContractSignerController(IContractSignerService service, IFxqService fxqService,IAliyunOSSService aliyunOSSService,ISettingsService settingsService, IEventPublisher eventPublisher) : CustomerControllerBase
    {

        public record SigningAndCooperationUnitQuery(bool? IsSigningUnit,bool? IsCooperationUnit);

        [HttpGet]
        public async Task<IActionResult> PageList(string name,string idCardNo,string companyNo, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_ContractSigner>();
            exp.AndIF(!string.IsNullOrWhiteSpace(name), it => it.Name.Contains(name));
            exp.AndIF(!string.IsNullOrWhiteSpace(idCardNo), it => it.Idno.Contains(idCardNo));
            exp.AndIF(!string.IsNullOrWhiteSpace(companyNo), it => it.Idno.Contains(companyNo));
            var models = await service.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var expable = Expressionable.Create<Model.Live_ContractSigner>();
            expable.And(f => f.IsEnabled);
            var exp = expable.ToExpression();
            var models = await service.PageList(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> UploadFile(IFormFile file)
        {
            var uploadRes = await aliyunOSSService.UploadFile(file);
            uploadRes.Code= 200;
            return Ok(uploadRes);
        }
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Live_ContractSigner model)
        {
            if (model.IsCompany)
            {
                RealNameCompany3EventData realNameCompany3EventData = new RealNameCompany3EventData(model.Name, model.Idno, model.LegalPerson);
                eventPublisher.Publish(realNameCompany3EventData);
            }
            else
            {
                throw new ErrorCodeException(-1, "功能暂时取消");
                //RealName2EventData realName2EventData = new RealName2EventData(model.Name, model.Idno);
                //eventPublisher.Publish(realName2EventData);
            }
            return Ok();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.Live_ContractSigner> Get(Guid id)
        {
            var model = await service.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_ContractSigner model)
        {
            if (!await service.Exist(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await service.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            if (!await service.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await service.Delete(id);
            return Ok();
        }


        [HttpPut]
        public async Task<IActionResult> RealNameAuth( Guid id)
        {
            if (!await service.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            var entity=await service.Get(id);
            switch (entity.RealNameType.Value)
            {
                case RealNameEnumType.公安二要素:
                    //RealName2EventData realName2 = new RealName2EventData(entity.Name, entity.Idno);
                    //eventPublisher.Publish(realName2);
                    break;
                case RealNameEnumType.全网三要素:
                    RealName3EventData realName3 = new RealName3EventData(entity.Name, entity.Idno, entity.Phone);
                    eventPublisher.Publish(realName3);
                    break;
                case RealNameEnumType.银行卡三要素:
                    RealNameBank3EventData realNameBank3 = new RealNameBank3EventData(entity.Name, entity.Idno, entity.BankNo);
                    eventPublisher.Publish(realNameBank3);
                    break;
                case RealNameEnumType.银行卡四要素:
                    RealNameBank4EventData realNameBank4 = new RealNameBank4EventData(entity.Name, entity.Idno, entity.BankNo,entity.Phone);
                    eventPublisher.Publish(realNameBank4);
                    break;
                case RealNameEnumType.企业三要素:
                    RealNameCompany3EventData realNameComoany3 = new RealNameCompany3EventData(entity.Name, entity.Idno, entity.LegalPerson);
                    break;
                case RealNameEnumType.企业四要素:
                    RealNameCompany4EventData realNameComoany4 = new RealNameCompany4EventData(entity.Name, entity.Idno, entity.LegalPerson, entity.LegalPersonIdCardNo);
                    break;
                default:
                    break;
            }
            return Ok();
        }

        /// <summary>
        /// 生成电子签名
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        /// <exception cref="ErrorCodeException"></exception>
        [HttpPut]
        public async Task<IActionResult> GenerateElectronicSignature(Guid id)
        {
            if (!await service.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            var entity = await service.Get(id);
            ElectronicSignatureEventData eventData = new ElectronicSignatureEventData(entity.Name, entity.Idno, entity.IsCompany);
            eventPublisher.Publish(eventData);
            return Ok();
        }


        [HttpGet]
        public async Task<IActionResult> GetByIdno(string idno)
        {
            var model = await service.Get(f => f.Idno == idno && f.IsEnabled);

            return Ok(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetSigner(string name,string idno, bool isCompany)
        {
            var model = await service.GetSigner(name, idno,isCompany);
            return Ok(model);
        }



        [AllowAnonymous]
        [HttpGet]
        public IActionResult TestRealName()
        {

          
           
          
         
            //FxqRealNameEventData realName2EventData = new FxqRealNameEventData()
            //{
            //    Name = "刘涛",
            //    Idno = "500102199003288132",
            //    Phone = "18225199032",
            //    BankCardNo = "6222033100018508286",
            //    CertNo = "11223344",
            //    LegalPerson = "法人张三",
            //    RealNameType = Model.Enums.RealNameEnumType.公安二要素
            //};
            //eventPublisher.Publish(realName2EventData);


            //FxqRealNameEventData realName3EventData = new FxqRealNameEventData()
            //{
            //    Name = "浙江哈迈科技有限公司",
            //    Idno = "9133021255453766XF",
            //    LegalPerson = "法人张三",
            //    RealNameType = Model.Enums.RealNameEnumType.企业三要素
            //};
            //eventPublisher.Publish(realName3EventData);
            return Ok();
        }

        //[AllowAnonymous]
        //[HttpGet]
        //public IActionResult TestContractGenerate()
        //{

        //    Dictionary<string, string> kvDic = new Dictionary<string, string>();
        //    kvDic.Add("zhiwu", "钟静怡");
        //    kvDic.Add("phone", "152000000");


        //    var signer = new List<ContractSignerParty>()
        //    {
        //            new ContractSignerParty( Model.Enums.SignerPartyEnumType.第一当事人,"测试签章单位","10011001011","甲方"),
        //            new ContractSignerParty( Model.Enums.SignerPartyEnumType.第二当事人,"刘涛","500102199003288132","乙方")
        //    };
        //    Guid customerGID=Guid.NewGuid();
        //    Guid relateGID=Guid.NewGuid();
        //    string fileUrl = "https://sign-online-group.oss-cn-hangzhou.aliyuncs.com/sign-open/documentPdf/%E7%A4%BA%E4%BE%8B%E6%96%87%E4%BB%B6/guanjianci.docx";
        //    GenerateContractSignRecordEventData generateContractSignRecordEventData = new GenerateContractSignRecordEventData(fileUrl, customerGID, 0, relateGID, signer, ContractEnumType.入职合同, kvDic);
        //    eventPublisher.Publish(generateContractSignRecordEventData);
        //    return Ok();
        //}

        [AllowAnonymous]
        [HttpGet]
        public IActionResult TestSenSms()
        {
            ContractSigningSendSmsEventData smsEventData = new ContractSigningSendSmsEventData()
            {
                ContractRecordID = Guid.Parse("AB4B78A8-A243-4F1B-BB00-ACADEF93968B"),
                SmsCode = "1001",
                SmsContent = "消息测试",
                SmsName = "刘涛",
                SmsPhone = "15200000000",
            };

            eventPublisher.Publish(smsEventData);
            return Ok();
        }

        [AllowAnonymous]
        [HttpGet]
        public IActionResult TestSigning()
        {
            //ContractSigningEventData smsEventData = new ContractSigningEventData()
            //{
            //    ContractRecordID = Guid.Parse("05D6DB29-EF9E-4972-A4DC-53F3D325E8D6"),
            //    SignerName = "刘涛",
            //    SignerSmsCode = "1001"
            //};

            //eventPublisher.Publish(smsEventData);
            return Ok();
        }



        

        [AllowAnonymous]
        [HttpGet]
        public IActionResult TestEventDatasss(int num=0)
        {
            //if (num == 0)
            //{
            //    #region 生成网签合同记录
            //    Dictionary<string, string> kvDic = new Dictionary<string, string>();
            //    kvDic.Add("zhiwu", "钟静怡");
            //    kvDic.Add("phone", "152000000");

            //    GenerateContractSignRecordEventData generateContractSignRecordEventData = new GenerateContractSignRecordEventData()
            //    {
            //        ContractOssUrl = "https://sign-online-group.oss-cn-hangzhou.aliyuncs.com/sign-open/documentPdf/%E7%A4%BA%E4%BE%8B%E6%96%87%E4%BB%B6/guanjianci.docx",
            //        ContractSignerPartys = new List<ContractSignerParty>() {
            //        new ContractSignerParty( Model.Enums.SignerPartyEnumType.第一当事人,"钟静怡","50010220050816232X","甲方")
            //      },
            //        ContractType = Model.Enums.ContractEnumType.入职合同,
            //        CreateTime = DateTime.Now,
            //        GustomerGID = Guid.NewGuid(),
            //        RelateGID = Guid.NewGuid(),
            //        RelateID = 1001,
            //        KeywordReplaces = kvDic
            //    };
            //    eventPublisher.Publish(generateContractSignRecordEventData);
            //    #endregion
            //}
            //else {
            //    #region 签署
            //    ContractSigningEventData signingEventData = new ContractSigningEventData()
            //    {
            //        ContractRecordID = Guid.Parse("29807A29-2612-483A-B6EB-EDD5768F9CDE")
            //    };
            //    eventPublisher.Publish(signingEventData);
            //    #endregion
            //}

        




            //FxqRealNameEventData eventData2 = new FxqRealNameEventData()
            //{
            //    Name = "钟静怡",
            //    Idno = "50010220050816232X",
            //    RealNameType = Model.Enums.RealNameEnumType.公安二要素
            //};
            //eventPublisher.Publish(eventData2);

            //FxqRealNameEventData eventData3 = new FxqRealNameEventData()
            //{
            //    Name = "钟静怡",
            //    Idno = "50010220050816232X",
            //    RealNameType = Model.Enums.RealNameEnumType.全网三要素,
            //    Phone = "18315116974"
            //};
            //eventPublisher.Publish(eventData3);

            //FxqRealNameEventData eventDataBank3 = new FxqRealNameEventData()
            //{
            //    Name = "钟静怡",
            //    Idno = "50010220050816232X",
            //    RealNameType = Model.Enums.RealNameEnumType.银行卡三要素,
            //    BankCardNo = "6230960000020695784"
            //};
            //eventPublisher.Publish(eventDataBank3);


            //FxqRealNameEventData eventDataBank4 = new FxqRealNameEventData()
            //{
            //    Name = "钟静怡",
            //    Idno = "50010220050816232X",
            //    RealNameType = Model.Enums.RealNameEnumType.银行卡四要素,
            //    BankCardNo = "6230960000020695784",
            //    Phone = "18315116974"
            //};
            //eventPublisher.Publish(eventDataBank4);

            //TestEventData testEventData = new TestEventData() { Name = "钟静怡" };
            //eventPublisher.Publish(testEventData);
            return Ok();
        }


        public record GenerateSealReq(string name, string idno, bool isCompany);

        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GenerateSeal(GenerateSealReq req)
        {
            var setting = await settingsService.GetMasterSettings();
            var record = await service.Get(f => f.Idno == req.idno);
            if (record == null)
            {
                record = new Live_ContractSigner()
                {
                    CreateTime = DateTime.Now,
                    ApplyID = string.Empty,
                    BankNo = string.Empty,
                    Expired = DateTime.MinValue,
                    ID = Guid.Empty,
                    Idno = req.idno,
                    IsCompany = req.isCompany,
                    IsEnabled = true,
                    IsRealNameSuc = true,
                    LegalPerson = string.Empty,
                    LegalPersonIdCardNo = string.Empty,
                    Name = req.name,
                    OssUrl = string.Empty,
                    Phone = string.Empty,
                    RealNameTime = DateTime.MinValue,
                    RealNameType = null,
                    Remark = string.Empty,
                };
            }
            if (!string.IsNullOrWhiteSpace(record.OssUrl))
            {
                return Ok();
            }
            string signerOssUrl = string.Empty;
            if (record.IsCompany)
            {
                var resp = await fxqService.CompanyElectronicSeal(record.Name, null, record.Idno, null, null, null, 0, null);
                if (resp != null && resp.Code == 10000)
                {
                    var fileBase64 = Convert.FromBase64String(resp.Data.Replace("data:image/png;base64,", ""));
                    using (var ms = new MemoryStream(fileBase64))
                    {
                        var ossResp = await aliyunOSSService.UploadFile(ms, $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.png", true, false, "seals");
                        if (ossResp.Code == 0)
                        {
                            signerOssUrl = setting.AliBucketDomain.TrimEnd('/') + ossResp.Data;
                        }
                        else
                        {
                            signerOssUrl = string.Empty;
                            throw new ErrorCodeException(-1, "电子签名获取成功，上传阿里云Oss 失败！");
                        }
                    }
                }
                else
                {
                    throw new ErrorCodeException(-1, $"电子签名获取失败！异常状态码：{resp.Code},异常信息：{resp.Msg}");
                }
            }
            else
            {
                var resp = await fxqService.ElectronicSeal(record.Name, 0, 0, 0, 0);
                if (resp != null && resp.Code == 10000)
                {
                    var fileBase64 = Convert.FromBase64String(resp.Data.Replace("data:image/png;base64,", ""));
                    using (var ms = new MemoryStream(fileBase64))
                    {
                        var ossResp = await aliyunOSSService.UploadFile(ms, $"{DateTime.Now.ToString("yyyyMMddHHmmssfff")}.png", true, false);
                        if (ossResp.Code == 0)
                        {
                            signerOssUrl = setting.AliBucketDomain.TrimEnd('/') + ossResp.Data;
                        }
                        else
                        {
                            throw new ErrorCodeException(-1, "电子签名获取成功，上传阿里云Oss 失败！");
                        }
                    }
                }
                else
                {
                    throw new ErrorCodeException(-1, $"电子签名获取失败！异常状态码：{resp.Code},异常信息：{resp.Msg}");
                }
            }

            if (string.IsNullOrWhiteSpace(signerOssUrl))
            {
                throw new ErrorCodeException(-1, "电子章生成失败！");
            }
            else
            {
                record.OssUrl = signerOssUrl;
                if (record.ID != Guid.Empty)
                {
                    await service.Update(record);
                }
                else
                {
                    record.ID = Guid.NewGuid();
                    await service.Add(record);
                }
                return Ok();
            }

        }
    }
}
