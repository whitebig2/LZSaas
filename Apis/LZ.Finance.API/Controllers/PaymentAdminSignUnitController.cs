using LZ.Core;
using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.Dtos.FinanceDo;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System.Linq;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class PaymentAdminSignUnitController : ApiControllerBase
    {
        private readonly IPaymentAdminSignUnitService _service;
        private readonly IPaymentAdminService _adminService;
        private readonly IPaymentSignUnitService _signUnitService;
        private readonly IPaymentApplicationService _applicationService;

        public PaymentAdminSignUnitController(
            IPaymentAdminSignUnitService service,
            IPaymentAdminService adminService,
            IPaymentSignUnitService signUnitService,
            IPaymentApplicationService applicationService)
        {
            _service = service;
            _adminService = adminService;
            _signUnitService = signUnitService;
            _applicationService = applicationService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(Guid? paymentAdminID, Guid? paymentSignUnitID, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Live_PaymentAdminSignUnit>();
            exp.AndIF(paymentAdminID.HasValue && paymentAdminID.Value != Guid.Empty, f => f.PaymentAdminID == paymentAdminID.Value);
            exp.AndIF(paymentSignUnitID.HasValue && paymentSignUnitID.Value != Guid.Empty, f => f.PaymentSignUnitID == paymentSignUnitID.Value);
            var models = await _service.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }

        [HttpGet]
        public async Task<IActionResult> List(Guid? paymentAdminID, Guid? paymentSignUnitID)
        {
            var exp = Expressionable.Create<Live_PaymentAdminSignUnit>();
            exp.AndIF(paymentAdminID.HasValue && paymentAdminID.Value != Guid.Empty, f => f.PaymentAdminID == paymentAdminID.Value);
            exp.AndIF(paymentSignUnitID.HasValue && paymentSignUnitID.Value != Guid.Empty, f => f.PaymentSignUnitID == paymentSignUnitID.Value);
            var models = await _service.List(exp.ToExpression());
            return Ok(models);
        }

        [HttpGet]
        public async Task<IActionResult> Get(Guid id)
        {
            var model = await _service.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }

        [HttpPost]
        public async Task<IActionResult> Add(PaymentAdminSignUnitDto dto)
        {
            await EnsureRelationValid(dto.PaymentAdminID, dto.PaymentSignUnitID);
            var isExist = await _service.Exist(f => f.PaymentAdminID == dto.PaymentAdminID && f.PaymentSignUnitID == dto.PaymentSignUnitID);
            if (isExist)
            {
                throw new ErrorCodeException(-1, "该管理员已绑定此签约单位，无法重复创建！");
            }

            var model = new Live_PaymentAdminSignUnit
            {
                ID = Guid.NewGuid(),
                PaymentAdminID = dto.PaymentAdminID,
                PaymentSignUnitID = dto.PaymentSignUnitID,
                IsCanView = dto.IsCanView,
                IsCanPay = dto.IsCanPay,
                IsCanManage = dto.IsCanManage,
                CreateTime = DateTime.Now
            };
            await _service.Add(model);
            return Ok();
        }

        [HttpPut]
        public async Task<IActionResult> Update(PaymentAdminSignUnitDto dto)
        {
            var model = await _service.Get(f => f.ID == dto.ID);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            var admin = await _adminService.Get(f => f.ID == model.PaymentAdminID && !f.IsDelete);
            if (admin == null)
            {
                throw new ErrorCodeException(-1, "支付管理员不存在");
            }
            await EnsureCurrentUserMatchAdmin(admin);
            model.IsCanView = dto.IsCanView;
            model.IsCanPay = dto.IsCanPay;
            model.IsCanManage = dto.IsCanManage;
            await _service.Update(model);
            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateSignUnits(PaymentAdminSignUnitReqDto req)
        {
            var admin = await _adminService.Get(f => f.ID == req.PaymentAdminID && !f.IsDelete);
            if (admin == null)
            {
                throw new ErrorCodeException(-1, "支付管理员不存在");
            }
            await EnsureCurrentUserMatchAdmin(admin);

            var units = (req.Units ?? new List<PaymentAdminSignUnitDto>())
                .Where(f => f.PaymentSignUnitID != Guid.Empty)
                .ToList();
            foreach (var item in units)
            {
                await EnsureRelationValid(req.PaymentAdminID, item.PaymentSignUnitID);
            }

            await _service.Delete(f => f.PaymentAdminID == req.PaymentAdminID);
            var models = units
                .Select(f => new Live_PaymentAdminSignUnit
                {
                    ID = Guid.NewGuid(),
                    PaymentAdminID = req.PaymentAdminID,
                    PaymentSignUnitID = f.PaymentSignUnitID,
                    IsCanView = f.IsCanView,
                    IsCanPay = f.IsCanPay,
                    IsCanManage = f.IsCanManage,
                    CreateTime = DateTime.Now
                }).ToList();
            if (models.Count > 0)
            {
                await _service.Add(models);
            }
            return Ok();
        }

        /// <summary>
        /// 权限管理：根据推送数据对管理员签约单位权限做新增、修改或删除
        /// </summary>
        [HttpPost]
        public async Task<IActionResult> ManagePermission(PaymentAdminSignUnitPermissionReqDto req)
        {
            if (req.PaymentAdminID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "支付管理员不能为空");
            }

            var admin = await _adminService.Get(f => f.ID == req.PaymentAdminID && !f.IsDelete);
            if (admin == null)
            {
                throw new ErrorCodeException(-1, "支付管理员不存在");
            }
            await EnsureCurrentUserMatchAdmin(admin);

            var pushUnits = (req.Units ?? new List<PaymentAdminSignUnitDto>())
                .Where(f => f.PaymentSignUnitID != Guid.Empty)
                .ToList();

            // 同一签约单位只保留一条，避免重复推送
            var duplicateSignUnit = pushUnits
                .GroupBy(f => f.PaymentSignUnitID)
                .FirstOrDefault(g => g.Count() > 1);
            if (duplicateSignUnit != null)
            {
                throw new ErrorCodeException(-1, "推送数据中存在重复的签约单位");
            }

            var existing = await _service.List(f => f.PaymentAdminID == req.PaymentAdminID);
            var existingById = existing.ToDictionary(f => f.ID);
            var existingBySignUnitId = existing.ToDictionary(f => f.PaymentSignUnitID);

            var keepIds = new HashSet<Guid>();
            var addList = new List<Live_PaymentAdminSignUnit>();

            foreach (var item in pushUnits)
            {
                await EnsureRelationValid(req.PaymentAdminID, item.PaymentSignUnitID);

                Live_PaymentAdminSignUnit? target = null;
                if (item.ID != Guid.Empty && existingById.TryGetValue(item.ID, out var byId))
                {
                    target = byId;
                }
                else if (existingBySignUnitId.TryGetValue(item.PaymentSignUnitID, out var bySignUnit))
                {
                    target = bySignUnit;
                }

                if (target != null)
                {
                    target.PaymentSignUnitID = item.PaymentSignUnitID;
                    target.IsCanView = item.IsCanView;
                    target.IsCanPay = item.IsCanPay;
                    target.IsCanManage = item.IsCanManage;
                    await _service.Update(target);
                    keepIds.Add(target.ID);
                }
                else
                {
                    var model = new Live_PaymentAdminSignUnit
                    {
                        ID = Guid.NewGuid(),
                        PaymentAdminID = req.PaymentAdminID,
                        PaymentSignUnitID = item.PaymentSignUnitID,
                        IsCanView = item.IsCanView,
                        IsCanPay = item.IsCanPay,
                        IsCanManage = item.IsCanManage,
                        CreateTime = DateTime.Now
                    };
                    addList.Add(model);
                    keepIds.Add(model.ID);
                }
            }

            if (addList.Count > 0)
            {
                await _service.Add(addList);
            }

            var deleteIds = existing.Where(f => !keepIds.Contains(f.ID)).Select(f => f.ID).ToList();
            foreach (var id in deleteIds)
            {
                await _service.Delete(id);
            }

            return Ok(new
            {
                addCount = addList.Count,
                updateCount = keepIds.Count - addList.Count,
                deleteCount = deleteIds.Count
            });
        }

        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] Guid id)
        {
            var model = await _service.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "数据不存在");
            }
            var admin = await _adminService.Get(f => f.ID == model.PaymentAdminID && !f.IsDelete);
            if (admin == null)
            {
                throw new ErrorCodeException(-1, "支付管理员不存在");
            }
            await EnsureCurrentUserMatchAdmin(admin);
            await _service.Delete(id);
            return Ok();
        }

        private async Task EnsureRelationValid(Guid paymentAdminID, Guid paymentSignUnitID)
        {
            if (paymentAdminID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "支付管理员不能为空");
            }
            if (paymentSignUnitID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "签约单位不能为空");
            }

            var admin = await _adminService.Get(f => f.ID == paymentAdminID && !f.IsDelete);
            if (admin == null)
            {
                throw new ErrorCodeException(-1, "支付管理员不存在");
            }
            if (!admin.IsEnable)
            {
                throw new ErrorCodeException(-1, "支付管理员未启用");
            }
            await EnsureCurrentUserMatchAdmin(admin);
            var signUnit = await _signUnitService.Get(f => f.ID == paymentSignUnitID && !f.IsDelete);
            if (signUnit == null)
            {
                throw new ErrorCodeException(-1, "签约单位不存在");
            }
            if (!signUnit.IsEnable)
            {
                throw new ErrorCodeException(-1, "签约单位未启用");
            }
            if (admin.PaymentApplicationID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "支付管理员未绑定支付应用");
            }
            if (signUnit.PaymentApplicationID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "签约单位未绑定支付应用");
            }
            if (signUnit.PaymentApplicationID != admin.PaymentApplicationID)
            {
                throw new ErrorCodeException(-1, "管理员与签约单位不属于同一支付应用");
            }
        }

        private async Task EnsureCurrentUserMatchAdmin(Live_PaymentAdmin admin)
        {
            if (admin.PaymentApplicationID == Guid.Empty)
            {
                throw new ErrorCodeException(-1, "支付管理员未绑定支付应用");
            }
            var application = await _applicationService.Get(f => f.ID == admin.PaymentApplicationID && !f.IsDelete);
            if (application == null)
            {
                throw new ErrorCodeException(-1, "支付应用不存在");
            }
            var currentCompanyGid = CurrentUser.CompanyGID ?? Guid.Empty;
            if (application.CompanyID != CurrentUser.CompanyID || application.CompanyGID != currentCompanyGid)
            {
                throw new ErrorCodeException(-1, "当前用户公司与支付应用公司不匹配");
            }
        }
    }
}
