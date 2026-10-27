using LZ.ApiService.Interface;
using LZ.ApiService.Services;
using LZ.Core.EventData;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Customer.Service.Interface;
using LZ.Customer.Service.Services;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NPOI.SS.UserModel;
using Quartz.Util;
using Senparc.CO2NET.Extensions;
using Senparc.Weixin.MP.AdvancedAPIs.Card;
using SqlSugar;

namespace LZ.Customer.API.Controllers
{

    public class EnterpriseController(IEnterpriseService enterpriseService,IContractSignerService signerService,IEventPublisher eventPublisher,IFxqService fxqService,ICaAuthIdentityService caAuthIdentityService ) : CustomerControllerBase
    {

        public record SigningAndCooperationUnitQuery(bool? IsSigningUnit, bool? IsCooperationUnit, string? Name,int? companyID);

        [HttpGet]
        public async Task<IActionResult> PageList(int? companyID,  [FromQuery] PagedModelBase query)
        {
            var expable = Expressionable.Create<Model.Live_Enterprise>();

            if (companyID.HasValue && companyID.Value > 0)
            {
                expable.And(f => f.CompanyID == companyID.Value);
            }
            else
            {
                expable.AndIF(CurrentUser.PlatformType != PlatformType.Admin && CurrentUser.PlatformType != PlatformType.Api, f => f.CompanyID == CurrentUser.CompanyID);
            }

            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.Name.Contains(query.Name));
            }
            expable.AndIF(!string.IsNullOrWhiteSpace(query.Name), it => it.Name.Contains(query.Name));
            expable.AndIF(!string.IsNullOrWhiteSpace(query.Keyword), it => it.CompanyNo.Contains(query.Keyword));
            var exp = expable.ToExpression();
            var models = await enterpriseService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] SigningAndCooperationUnitQuery query)
        {
            var expable = Expressionable.Create<Model.Live_Enterprise>();
            expable.AndIF(query.IsSigningUnit.HasValue, f => f.IsSigningUnit == query.IsSigningUnit);
            expable.AndIF(query.IsCooperationUnit.HasValue, f => f.IsCooperationUnit == query.IsCooperationUnit);
            expable.AndIF(!string.IsNullOrWhiteSpace(query.Name), f => f.Name == query.Name);
            expable.AndIF(query.companyID.HasValue, f => f.CompanyID == query.companyID.Value);
            var models = await enterpriseService.List(expable.ToExpression());
            return Ok(new { items = models, count = models.Count });
        }


        [HttpGet]
        public async Task<IActionResult> GetByName( string name)
        {
            var model = await enterpriseService.Get(f=>f.IsEnabled&&f.Name==name);
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_Enterprise model)
        {

            //if (model.IsSigningUnit)
            //{
            //    var signer = await signerService.Get(f => f.Name == model.Name && f.IsEnabled);
            //    if (signer == null)
            //    {
            //        throw new ErrorCodeException(-1, $"{model.Name}企业认证信息缺失，请先进行企业认认证");
            //    }
            //}

         
            model.CreateTime = DateTime.Now;
            model.ID = Guid.NewGuid();
            model.CompanyID = CurrentUser.CompanyID;
            await enterpriseService.Add(model);
            return Ok();
        }
        [HttpGet]
        [AllowAnonymous]
        public async Task<Model.Live_Enterprise> Get(Guid id)
        {
            var model = await enterpriseService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> Update([FromBody] Model.Live_Enterprise model)
        {
            if (!await enterpriseService.Exist(model.ID))
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            await enterpriseService.Update(model);
            return Ok();
        }
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            if (!await enterpriseService.Exist(id))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await enterpriseService.Delete(id);
            return Ok();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> RealName( [FromForm]Guid id )
        {

            var model = await enterpriseService.Get(id);
            if (string.IsNullOrWhiteSpace(model.Name))
            {
                throw new ErrorCodeException(-1, "企业名称信息缺失");
            }
            if (string.IsNullOrWhiteSpace(model.CompanyNo))
            {
                throw new ErrorCodeException(-1, "统一社会信用代码信息缺失");
            }
            if (string.IsNullOrWhiteSpace(model.LegalPerson))
            {
                throw new ErrorCodeException(-1, "企业法人信息缺失");
            }
            if (string.IsNullOrWhiteSpace(model.LegalPersonCardID))
            {
                throw new ErrorCodeException(-1, "法人身份证号码信息缺失");
            }



            var companyIdentity = await caAuthIdentityService.Get(f => f.Name == model.Name && f.CardID == model.CompanyNo && f.LegalPerson == model.LegalPerson && f.LegalPersonCardID == model.LegalPersonCardID && f.Status == CaAuthIdentityStatusEnumType.认证成功);
            if (companyIdentity != null)
            {
                throw new ErrorCodeException(-1, $"企业认证已完成，无需重复认证！");
            }

            var legalPersonIdentity = await caAuthIdentityService.Get(f => f.CardID == model.LegalPersonCardID && f.Status == CaAuthIdentityStatusEnumType.认证成功);
            if (legalPersonIdentity==null)
            {
                throw new ErrorCodeException(-1, $"法人：{model.LegalPerson}未完成实名认证");
            }

          var res=await  fxqService.CaCompanyAuthApply(model.Name, model.CompanyNo, model.LegalPerson, model.LegalPersonCardID, legalPersonIdentity.AgreeVerifyId?? legalPersonIdentity.FaceVerifyId,"默认应用");
            if (res.Code == 10000)
            {
                if (!res.Data.match)
                {
                    List<string> failMsg = new List<string>();

                    if (res.Data.entNameMatch != 1)
                    {
                        if (res.Data.entNameMatch == -1)
                        {
                            failMsg.Add("公司名称匹配结果【无法验证】");
                        }
                        else
                        {
                            failMsg.Add("公司名称匹配结果【不匹配】");
                        }
                    }
                    else if (res.Data.entRegNoMatch != 1)
                    {
                        if (res.Data.entRegNoMatch == -1)
                        {
                            failMsg.Add("工商注册号匹配结果【无法验证】");
                        }
                        else
                        {
                            failMsg.Add("工商注册号匹配结果【不匹配】");
                        }
                    }
                    else if (res.Data.repNameMatch != 1)
                    {
                        if (res.Data.repNameMatch == -1)
                        {
                            failMsg.Add("法人姓名匹配结果【无法验证】");
                        }
                        else
                        {
                            failMsg.Add("法人姓名匹配结果【不匹配】");
                        }
                    }
                    else if (res.Data.repIdNoMatch != 1)
                    {
                        if (res.Data.repIdNoMatch == -1)
                        {
                            failMsg.Add("法人身份证匹配结果 【无法验证】");
                        }
                        else
                        {
                            failMsg.Add("法人身份证匹配结果 【不匹配】");
                        }
                    }
                    throw new ErrorCodeException(-1, string.Join('、', failMsg));
                }
                else
                {

                    model.IsRealName = true;
                    Live_CaAuthIdentity authIdentity = new Live_CaAuthIdentity()
                    {
                        CreateTime = DateTime.Now,
                        ID = Guid.NewGuid(),
                        Name = model.Name,
                        UserID = 0,
                        UserGid = Guid.Empty,
                        Phone = model.Phone,
                        CardID = model.CompanyNo,
                        AuthType = "LegalPersonAuth",
                        IsAgree = true,
                        Status = Model.Enums.CaAuthIdentityStatusEnumType.认证成功,
                        IsApp = false,
                        OrderNo = string.Empty,
                        AuthTime = DateTime.Now,
                        IsCompany = true,
                        LegalPerson = model.LegalPerson,
                        LegalPersonCardID = model.LegalPersonCardID,
                        AgreeVerifyId = legalPersonIdentity.AgreeVerifyId,
                    };

                    await enterpriseService.Update(model);
                    await caAuthIdentityService.Add(authIdentity);
                }
            }
            else {

                throw new ErrorCodeException(-1, res.Msg);
            }
            return Ok();
        }

//        [AllowAnonymous]
//        [HttpPost]
//        public async Task<IActionResult> BatchRealName()
//        {

//            List<string> ls = new List<string>() {

//            "贵州盛轩人力资源有限公司",
//"嘉兴鸿博供应链管理有限公司",
//"嘉兴莱博友翌供应链管理有限公司",
//"嘉兴莱博友翌供应链管理有限公司余杭分公司",
//"荆州伯乐遇马科技产业有限公司",
//"宁波观序科技有限公司",
//"宁波合流科技有限公司",
//"宁波集聘人力资源有限公司",
//"宁波璟霖供应链管理有限公司",
//"宁波莱博网络科技有限公司",
//"宁波莱博友联人力资源有限公司",
//"宁波莱博友翌供应链管理有限公司",
//"宁波莱鑫人力资源有限公司",
//"宁波蓝鑫人力资源有限公司",
//"宁波蓝招大数据科技有限公司",
//"宁波启博商务信息咨询有限公司",
//"宁波人服企业管理有限公司",
//"宁波市超越人力资源有限公司",
//"宁波新莱博金浩供应链管理有限公司",
//"宁波新莱博深蓝供应链管理有限公司",
//"宁波新莱博顺合供应链管理有限公司",
//"宁波新莱博友盟供应链管理有限公司",
//"宁波幸福职邦互联网销售有限公司",
//"宁波职联供应链管理有限公司",
//"宁波职南针人力资源有限公司",
//"宁波职引力企业服务有限公司",
//"上海虹林劳务派遣有限公司宁波分公司",
//"舜宇集团有限公司",
//"台州莱博友翌供应链管理有限公司 ",
//"蚁力（上海）人力资源有限公司",


//            };

//          var recordList=await  enterpriseService.List(f => ls.Contains(f.Name.Trim()) && !string.IsNullOrWhiteSpace(f.LegalPersonCardID));

//            foreach (var item in recordList)
//            {
//                try
//                {
//                    await RealName(item.ID);
//                }
//                catch (Exception ex)
//                {
//                    Console.WriteLine($"{item.Name}-- {ex.Message}");
//                }
//            }


//            return Ok();
//        }

    }
}
