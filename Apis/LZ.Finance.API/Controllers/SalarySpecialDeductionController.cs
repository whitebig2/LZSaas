using LZ.Common;
using LZ.Core;
using LZ.Finance.Service.Interface;
using LZ.Finance.Service.Services;
using LZ.Model.Dtos.SpecialDeductionDtos;
using LZ.Model;
using LZ.Model.Enums;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Senparc.CO2NET.Extensions;
using SqlSugar;
using System.ComponentModel.Design;

namespace LZ.Finance.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class SalarySpecialDeductionController : ApiControllerBase
    {

        private readonly ISalaryCustomerService  _salaryCustomerService;
        private readonly ISalarySpecialDedcutionService  _dedcutionService;
        private readonly IFinanceOtherService  _otherService;

        public SalarySpecialDeductionController(ISalarySpecialDedcutionService dedcutionService, IFinanceOtherService otherService, ISalaryCustomerService salaryCustomerService)
        {
            _dedcutionService = dedcutionService;
            _otherService = otherService;
            _salaryCustomerService = salaryCustomerService;
        }

        [HttpGet]
        public async Task<IActionResult> PageList(string name, string idCardNo, DeductionEnumType? deductionType,DateTime? taxCountMonth, [FromQuery] PagedModelBase query)
        {
            var exp = Expressionable.Create<Model.Live_SalarySpecialDedcution>();
            exp.AndIF(!string.IsNullOrWhiteSpace(idCardNo), f => f.IdCardNo.Contains(idCardNo));
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(query.Name));
            exp.AndIF(deductionType.HasValue, f => f.DeductionType==deductionType.Value);
            exp.AndIF(taxCountMonth.HasValue, f => f.TaxCountMonth== taxCountMonth.Value);
            var models = await _dedcutionService.PageList(exp.ToExpression(), query);
            return Ok(new { items = models, count = query.Count });
        }
        //[HttpPost]
        //public async Task<IActionResult> Add(Model.Live_InvoiceConfig model)
        //{
        //    model.ID = Guid.NewGuid();
        //    model.CreateTime = DateTime.Now;
        //    await _dedcutionService.Add(model);
        //    return Ok();
        //}
        [HttpGet]
        public async Task<IActionResult> Get(int id)
        {
            var model = await _dedcutionService.Get(f => f.ID == id);
            if (model == null)
            {
                throw new ErrorCodeException(-1, "你要读取的数据不存在");
            }
            return Ok(model);
        }
      
        [HttpDelete]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            await _dedcutionService.Delete(id);
            return Ok();
        }
        [HttpPost]
        public async Task<IActionResult> UploadSpecialDeduction(IFormFile file)
        {

            var houseRentCityRes = await _otherService.GetDeductionHouseRentCityListAsync("专项扣除住房租金支出");

            var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
            List<Live_SalarySpecialDedcution> dedcutions = new List<Live_SalarySpecialDedcution>();
            var year = DateTime.Today.Year;
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var excelData = NpoiExcelHelper.ReadExcelFile<SpecialDeductionZNJYDto>(ms, "子女教育支出");

                var list = excelData.Where(f => f.StartMonth.Year == year).Select(f => new Live_SalarySpecialDedcution()
                {
                    BaseAmount = 2000.00m,
                    DeductionPercentage = f.DeductionPercentage,
                    DeductionType = Model.Enums.DeductionEnumType.子女教育支出,
                    EndMonth = f.EndMonth,
                    StartMonth = f.StartMonth,
                    IdCardNo = f.IdCadNo,
                    Name = f.Name,
                    RelationIdCardNo = f.RelationIdCardNo,
                    RelationName = f.RelationName,
                    Relationship = f.Relationship,
                    TaxCountMonth = currentMonth,
                    Remark = string.Empty
                }).ToList();

                if (list.Any())
                {
                    dedcutions.AddRange(list);
                }
            }
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var excelData = NpoiExcelHelper.ReadExcelFile<SpecialDeductionZYZGDto>(ms, "继续教育支出");
                var list = excelData.Where(f => f.StartMonth.Year == year).Select(f => new Live_SalarySpecialDedcution()
                {
                    BaseAmount = 3600,
                    DeductionPercentage = "100%",
                    DeductionType = Model.Enums.DeductionEnumType.继续教育支出,
                    StartMonth = f.StartMonth,// 一次性扣费
                    EndMonth = f.StartMonth,
                    IdCardNo = f.IdCadNo,
                    MonthAmount = 3600,
                    Name = f.Name,
                    RelationIdCardNo = string.Empty,
                    RelationName = string.Empty,
                    Relationship = string.Empty,
                    TaxCountMonth = DateTime.Today,
                    Remark = "职业资格继续教育情况"
                }).ToList();
                if (list.Any())
                {
                    dedcutions.AddRange(list);
                }
            }

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var excelData = NpoiExcelHelper.ReadExcelFile<SpecialDeductionJXJYDto>(ms, "继续教育支出");

                var list = excelData.Where(f => f.StartMonth.Year == year).Select(f => new Live_SalarySpecialDedcution()
                {
                    BaseAmount = 400m,
                    DeductionPercentage = "100%",
                    DeductionType = Model.Enums.DeductionEnumType.继续教育支出,
                    EndMonth = f.EndMonth,
                    StartMonth = f.StartMonth,
                    IdCardNo = f.IdCadNo,
                    MonthAmount = 400m,
                    Name = f.Name,
                    RelationIdCardNo = string.Empty,
                    RelationName = string.Empty,
                    Relationship = string.Empty,
                    TaxCountMonth = DateTime.Today,
                    Remark = "学历（学位）继续教育情况"
                }).ToList();
                if (list.Any())
                {
                    dedcutions.AddRange(list);

                }
            }
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var excelData = NpoiExcelHelper.ReadExcelFile<SpecialDeductionGJJDKDto>(ms, "住房贷款利息支出");
                var list = excelData.Where(f => f.StartMonth.Year == year).Select(f => new Live_SalarySpecialDedcution()
                {
                    BaseAmount = 1000m,
                    DeductionPercentage = f.DeductionPercentage,
                    DeductionType = Model.Enums.DeductionEnumType.住房贷款利息支出,
                    EndMonth = f.EndMonth,
                    StartMonth = f.StartMonth,
                    IdCardNo = f.IdCadNo,
                    MonthAmount = 1000m,
                    Name = f.Name,
                    RelationIdCardNo = string.Empty,
                    RelationName = string.Empty,
                    Relationship = string.Empty,
                    TaxCountMonth = DateTime.Today,
                    Remark = "公积金贷款信息"
                }).ToList();
                if (list.Any())
                {
                    dedcutions.AddRange(list);
                }
            }
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var excelData = NpoiExcelHelper.ReadExcelFile<SpecialDeductionSYDKDto>(ms, "住房贷款利息支出");
                var list = excelData.Where(f => f.StartMonth.Year == year).Select(f => new Live_SalarySpecialDedcution()
                {
                    BaseAmount = 1000,
                    DeductionPercentage = f.DeductionPercentage,
                    DeductionType = Model.Enums.DeductionEnumType.住房贷款利息支出,
                    EndMonth = f.EndMonth,
                    StartMonth = f.StartMonth,
                    IdCardNo = f.IdCadNo,
                    MonthAmount = 1000,
                    Name = f.Name,
                    RelationIdCardNo = string.Empty,
                    RelationName = string.Empty,
                    Relationship = string.Empty,
                    TaxCountMonth = DateTime.Today,
                    Remark = "商业贷款信息"
                }).ToList();
                if (list.Any())
                {
                    dedcutions.AddRange(list);
                }
            }

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var excelData = NpoiExcelHelper.ReadExcelFile<SpecialDeductionZFZJDto>(ms, "住房租金支出");
                var list = excelData.Where(f => f.StartMonth.Year == year).Select(f => new Live_SalarySpecialDedcution()
                {
                    BaseAmount = 0,
                    DeductionPercentage = "100%",
                    DeductionType = Model.Enums.DeductionEnumType.住房租金支出,
                    EndMonth = f.EndMonth,
                    StartMonth = f.StartMonth,
                    IdCardNo = f.IdCadNo,
                    MonthAmount = 0,
                    Name = f.Name,
                    WorkCity = f.WorkCity,
                    RelationIdCardNo = string.Empty,
                    RelationName = string.Empty,
                    Relationship = string.Empty,
                    TaxCountMonth = DateTime.Today,
                    Remark = string.Empty
                }).ToList();

                if (list.Any())
                {
                    dedcutions.AddRange(list);
                }
            }
            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var excelData = NpoiExcelHelper.ReadExcelFile<SpecialDeductionSYLRDto>(ms, "赡养老人支出");
                var list = excelData.Where(f => f.StartMonth.Year == year).Select(f => new Live_SalarySpecialDedcution()
                {
                    BaseAmount = f.BaseAmount,
                    DeductionPercentage = "100%",
                    DeductionType = Model.Enums.DeductionEnumType.赡养老人支出,
                    EndMonth = f.EndMonth,
                    StartMonth = f.StartMonth,
                    IdCardNo = f.IdCadNo,
                    MonthAmount = f.BaseAmount,
                    Name = f.Name,
                    RelationIdCardNo = f.RelationIdCardNo,
                    RelationName = f.RelationName,
                    Relationship = f.Relationship,
                    TaxCountMonth = DateTime.Today,
                    Remark = string.Empty
                }).ToList();

                if (list.Any())
                {
                    dedcutions.AddRange(list);
                }
            }

            using (var ms = new MemoryStream())
            {
                file.CopyTo(ms);
                var excelData = NpoiExcelHelper.ReadExcelFile<SpecialDeductionYYEZHFDto>(ms, "3岁以下婴幼儿照护支出");
                var list = excelData.Where(f => f.StartMonth.Year == year).Select(f => new Live_SalarySpecialDedcution()
                {
                    BaseAmount = 2000.00m,
                    DeductionPercentage = f.DeductionPercentage,
                    DeductionType = Model.Enums.DeductionEnumType.婴幼儿照护支出,
                    EndMonth = f.EndMonth,
                    StartMonth = f.StartMonth,
                    IdCardNo = f.IdCadNo,
                    MonthAmount = 1500,
                    Name = f.Name,
                    RelationIdCardNo = string.Empty,
                    RelationName = string.Empty,
                    Relationship = string.Empty,
                    TaxCountMonth = DateTime.Today,
                    Remark = string.Empty
                }).ToList();

                if (list.Any())
                {
                    dedcutions.AddRange(list);
                }
            }
            var houseRentDeductionList = dedcutions.Where(f => f.DeductionType == Model.Enums.DeductionEnumType.住房租金支出).ToList();

            if (houseRentCityRes.Code == 200 && houseRentCityRes.Data != null)
            {
                foreach (var item in houseRentDeductionList)
                {
                    if (houseRentCityRes.Data.Any(f => f.Name == item.WorkCity))
                    {
                        item.BaseAmount = Convert.ToDecimal(houseRentCityRes.Data.First(f => f.Name == item.WorkCity).NO);
                    }
                    else
                    {
                        throw new Exception($"{item.WorkCity}专项扣除住房租金支出未配置");

                    }
                }
            }
            else
            {


                throw new Exception($"专项扣除住房租金支出数据获取失败");
            }
            if (dedcutions.Any())
            {
                dedcutions.AsParallel().ForAll(dedcution =>
                {
                    dedcution.MonthAmount = dedcution.BaseAmount * Convert.ToDecimal(dedcution.DeductionPercentage.TrimEnd('%')) / 100;
                });
            }
            await _dedcutionService.Add(dedcutions);
            return Ok();


        }

        [HttpGet]
        public async Task<IActionResult> QueryDelete(string name, string idCardNo, DeductionEnumType? deductionType, DateTime? taxCountMonth, [FromQuery] PagedModelBase query)
        {
            if (!taxCountMonth.HasValue)
            {
                throw new ErrorCodeException(-1, "个税月不能未空");
            }

            var exp = Expressionable.Create<Model.Live_SalarySpecialDedcution>();
            exp.AndIF(!string.IsNullOrWhiteSpace(idCardNo), f => f.IdCardNo.Contains(idCardNo));
            exp.AndIF(!string.IsNullOrWhiteSpace(name), f => f.Name.Contains(query.Name));
            exp.AndIF(deductionType.HasValue, f => f.DeductionType == deductionType.Value);
            exp.AndIF(taxCountMonth.HasValue, f => f.TaxCountMonth == taxCountMonth.Value);
            var models = await _dedcutionService.Delete(exp.ToExpression());
            return Ok();
        }
    }
}
