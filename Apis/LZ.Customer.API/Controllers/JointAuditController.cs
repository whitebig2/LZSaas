using Microsoft.AspNetCore.Mvc;
using LZ.Core;
using LZ.Model.Exceptions;
using SqlSugar;
using LZ.Customer.Service.Interface;
using LZ.Customer.API;
using LZ.Core.Attributes;
using LZ.Customer.Service.Services;
using Microsoft.AspNetCore.Authorization;
using LZ.Core.Interface;
using Pipelines.Sockets.Unofficial.Buffers;
using System.Collections.Generic;
using LZ.Model.PagedModel;
using LZ.Model.Enums;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.EventData;
using NPOI.SS.Formula.Functions;

namespace LZ.Customer.API
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    public class JointAuditController(IJointAuditService jointAuditService,ICompanyConfigService companyConfigService,ICustomerService customerService, IEventPublisher publisher) : CustomerControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query)
        {
            var expable = Expressionable.Create<Model.Live_V_JointAudit>();
            if (query.CID.HasValue)
            {
                expable.And(it => it.CompanyID==query.CID);
            }
            if (!string.IsNullOrEmpty(query.Name))
            {
                expable.And(it => it.CustomerName.Contains(query.Name));
            }
            var user = CurrentUser;
            if (user.PlatformType != PlatformType.Admin)
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
             var exp = expable.ToExpression();
            var models = await jointAuditService.List(exp, query);
            return Ok(new { items = models, count = query.Count });
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [PlatformLimit(Model.Enums.PlatformType.LZMini)]
        [HttpPost]
        public async Task<IActionResult> Add([FromBody] Model.Live_JointAudit model)
        {
            var cus = await customerService.Get(model.CustomerID.Value);
            if (cus.JointAuditStatus == (int)JointAuditStatus.Audit)
            {
                throw new ErrorCodeException(-1, "该客户的联营还在审核中，请耐心等待！");
            }
            if (model.OperateModel == 1)
            {
                cus.OperateModel = model.OperateModel;
                await customerService.Update(cus);
                return Ok();
            }
            var user = CurrentUser;
            if (user.PlatformType != Model.Enums.PlatformType.Admin)
            {
                model.CompanyGID = user.CompanyGID;
                model.CompanyID = user.CompanyID;
            }
            var isApp = false;
            var companyModel = await companyConfigService.Get(model.CompanyGID);
            var config = companyModel.Config;
            if (!config.CustomerIsJoint)
            {
                throw new ErrorCodeException(-1, "该人力公司未启动联营模式");
            }
          
            if (user.PlatformType == Model.Enums.PlatformType.LZMini)
            {
                model.UserID = user.UserID;
                if (model.JointCount < config.CustomerJointCount)
                {
                    throw new ErrorCodeException(-1, "申请人数不得低于" + config.CustomerJointCount + "人");
                }
            }
            isApp = config.CustomerIsJointAudit;
            if (isApp)
            {
                model.CreateTime = DateTime.Now;
                model.Status = 1;
                model.ID = await jointAuditService.Add(model);
                cus.JointAuditStatus = (int)Model.Enums.JointAuditStatus.Audit;
                await customerService.Update(cus);
                return Ok(model.ID);
            }
            else
            {
                cus.JointPartnerID = model.PartnerID;
                cus.JointPartnerGID= model.PartnerGID;
                cus.PartnerIDS=model.PartnerIDS;
                cus.PartnerNames=model.PartnerNames;
                cus.OperateModel = model.OperateModel;
                cus.JointObjType= model.JointObjType;
                await customerService.Update(cus);
                //publisher.Publish(new CustomerJointEventData()
                //{
                //    CustomerID = model.CustomerID.Value,
                //    PartnerIDS = model.PartnerIDS,
                //    JointObjType = model.JointObjType,
                //});
                return Ok();
            }
        }
        [HttpGet]
        public async Task<Model.Live_JointAudit> Get(int id)
        {
            var model = await jointAuditService.Get(id);
            if (model==null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return model;
        }
        [HttpPut]
        public async Task<IActionResult> ChangePartner([FromForm] int id, [FromForm] int partnerId, [FromForm] Guid partnerGID)
        {
            var model = await jointAuditService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要修改的数据不存在");
            }
            if (model.Status != (int)Model.Enums.JointAuditStatus.Audit)
            {
                throw new ErrorCodeException(-1, "该记录已审核,不可修改！");
            }
            var has = await jointAuditService.ChangePartner(id, partnerId, partnerGID);
            return Ok();
        }
        [HttpPut]
        public async Task<IActionResult> ChangeStatus([FromForm] int id, [FromForm] int status, [FromForm] string remarks)
        {
            var model=await jointAuditService.Get(id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要操作的数据不存在");
            }
            if(model.Status!= (int)Model.Enums.JointAuditStatus.Audit)
            {
                throw new ErrorCodeException(-1, "该记录已审核！");
            }
            var has= await jointAuditService.ChangeStatus(id, status, remarks);
            if (has && status == (int)Model.Enums.JointAuditStatus.Pass)
            {
                var cmodel = await customerService.Get(model.CustomerID.Value);
                cmodel.JointPartnerID = model.PartnerID;
                cmodel.JointPartnerGID = model.PartnerGID;
                cmodel.PartnerIDS = model.PartnerIDS;
                cmodel.PartnerNames = model.PartnerNames;
                cmodel.OperateModel = model.OperateModel;
                cmodel.JointObjType = model.JointObjType;
                cmodel.JointAuditStatus = (int)Model.Enums.JointAuditStatus.Pass;
                await customerService.Update(cmodel);
            }
            //if (has && status==(int)Model.Enums.JointAuditStatus.Pass)
            //{
            //    publisher.Publish(new CustomerJointEventData()
            //    {
            //        CustomerID = model.CustomerID.Value,
            //        PartnerIDS = model.PartnerIDS,
            //        JointObjType = model.JointObjType,
            //    });
            //}
            return Ok();
        }
        [PlatformLimit(Model.Enums.PlatformType.Admin)]
        [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            if (!await jointAuditService.Exist(id, CurrentUser.CompanyID))
            {
                throw new ErrorCodeException(-1, "你要删除的数据不存在");
            }
            await jointAuditService.Delete(id);
            return Ok();
        }
    }
}
