using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Basic.Service.Interface;
using LZ.Core;
using LZ.Core.Attributes;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.Redis;
using LZ.Model;
using LZ.Model.ApiModel.FxqModel;
using LZ.Model.Exceptions;
using LZ.Model.FxqModel;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Senparc.CO2NET.Extensions;
using SqlSugar;
using System.ComponentModel.DataAnnotations;
using System.Runtime.InteropServices;
using System.Text.Json;

namespace LZ.Basic.API.Controllers
{
    [AllowAnonymous]
    public class FxqAppController(IEventPublisher eventPublisher ,IRedisService redisService, IFxqElectronicSealService   electronicSealService,IFxqService  fxqService,IAliyunOSSService aliyunOSSService) : BasicControllerBase
    {


        #region Record
        /// <summary>
        /// 
        /// </summary>
        /// <param name="Name"></param>
        /// <param name="IsCompanySeal"></param>
        public record ElectronicSealRequest([Required]string Name,[Required] bool IsCompanySeal);
        public record ElectronicSealResponse([Required] string Name, [Required] bool IsCompanySeal,string OssUrl,DateTime Expired);
        public record WordKeywordReplaceRequest([Required] string ossUrl,[Required]Dictionary<string,string> Keywords);
        #endregion

     

        /// <summary>
        /// 生成电子签章
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> GenerateElectronicSeal(ElectronicSealRequest request)
        {

            var exp = Expressionable.Create<Live_FxqElectronicSeal>();
            exp.And(f => f.ExpirationDate > DateTime.Today);
            exp.And(f => f.Name == request.Name);
            exp.And(f => f.IsCompanySeal == request.IsCompanySeal);

            ElectronicSealResponse electronicSealResp = null;
            var sealEntity = await electronicSealService.Get(exp.ToExpression());
            if (sealEntity == null)
            {
                FxqResponse<string> fxqResponse = null;
                if (request.IsCompanySeal)
                {
                    fxqResponse = await fxqService.CompanyElectronicSeal(request.Name, null, null, 0, 0, 0, 1, 0);
                }
                else
                {
                    fxqResponse = await fxqService.ElectronicSeal(request.Name, 1, 0, 0, 0);
                }
                if (fxqResponse != null )
                {
                    if (fxqResponse.Code == 10000)
                    {

                        sealEntity = new Live_FxqElectronicSeal()
                        {
                            ID = Guid.NewGuid(),
                            CreateTime = DateTime.Now,
                            ColorType = 0,
                            Name = request.Name,
                            ExpirationDate = DateTime.Now.Date.AddMonths(3),
                            FileExtension = ".png",
                            FontType = 0,
                            IsCompanySeal = request.IsCompanySeal,
                            OssUrl = fxqResponse.Data,
                            ShapeType = 0,
                        };
                        await electronicSealService.Add(sealEntity);

                        electronicSealResp = new ElectronicSealResponse(request.Name, request.IsCompanySeal, fxqResponse.Data, sealEntity.ExpirationDate.Value);
                        // 将电子章保存到数据库
                      
                    }
                    else {
                        throw new Exception($"生成电子签章失败:{fxqResponse.Msg}");
                    }

                }
                else {
                    throw new Exception("生成电子签章响应信息为空！");
                }
            }
            else
            {
                electronicSealResp = new ElectronicSealResponse(sealEntity.Name, sealEntity.IsCompanySeal, sealEntity.OssUrl, sealEntity.ExpirationDate.Value);
            }

            if (electronicSealResp == null)
                throw new Exception($"{request.Name}电子章获取失败");
            return Ok(electronicSealResp);
        }

        /// <summary>
        /// Word文档关键词替换
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> WordKeywordReplace(WordKeywordReplaceRequest request)
        {
            var kv = request.Keywords.OrderBy(f => f.Key).ToDictionary(f => f.Key, f => f.Value);
            var fxqResp = await fxqService.DocxKeywordReplace(request.ossUrl, kv, 1);
            return Ok(fxqResp);
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> WordToPdf(WordKeywordReplaceRequest request)
        {
            var kv = request.Keywords.OrderBy(f => f.Key).ToDictionary(f => f.Key, f => f.Value);
            var fxqResp = await fxqService.DocxKeywordReplace(request.ossUrl, kv, 1);
            return Ok(fxqResp);
        }


        [AllowAnonymous]
        [HttpGet]
        public IActionResult TestEventDatasss(string name)
        {
            FxqRealNameEventData eventData = new FxqRealNameEventData()
            {
                Name = "钟静怡",
                Idno = "50010220050816232X",
                RealNameType = Model.Enums.RealNameEnumType.公安二要素
            };
            eventPublisher.Publish(eventData);
            TestEventData testEventData = new TestEventData() { Name = "钟静怡" };
            eventPublisher.Publish(testEventData);
            return Ok();
        }
        //[HttpPost]
        //public async Task<IActionResult> GeneratePdfContract(GeneratePdfContractRequest request)
        //{
        //    var resp = await fxqService.DocxKeywordReplace(request.ContractOssFilePath, request.Keywords, 1);
        //    return Ok(resp.Data);
        //}


        //[HttpPost]
        //public async Task<IActionResult> SigningPdfContract(SigningPdfContractRequest request)
        //{
        //    var keywordSigners = request.Signers.Select(signerItem => new KeywordSigner(signerItem.Name, signerItem.IdNo, signerItem.ElectronicSealFile, signerItem.SignKeyword, 120, (signerItem.PageSignHeight.HasValue && signerItem.PageSignHeight.Value > 0) ? signerItem.PageSignHeight.Value : null));
        //    var signResp = await fxqService.KeywordPdfContractSign(request.ContractOssFilePath, keywordSigners.ToArray(), 1, null, null, new SignDate(1, null, null, null));
          
        //    return Ok();
        //}



    }
}
