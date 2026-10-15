using LZ.Core.Attributes;
using LZ.Model.Enums;
using LZ.Statistics.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Statistics.API.Controllers
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    public class SalaryStatisticsController : StatisticsControllerBase
    {

        private readonly ISalaryStatisticsService _salaryStatisticsService;

        public SalaryStatisticsController(ISalaryStatisticsService salaryStatisticsService)
        {
            _salaryStatisticsService = salaryStatisticsService;
        }

        public record CasjRecordDto(string CustomerName, string Name, string IdCardNo, string StatusName, decimal TotalAmount);
        [HttpGet]
        public async Task<IActionResult> SalaryCashRecords(CashSendStatusEnumType status)
        {



            var records = await _salaryStatisticsService.ListSalaryCashRecord(f => f.Status == status);
            var recordDtos = records.Select(f => new CasjRecordDto(f.CustomerName, f.Name, f.IdCardNo, f.Status.ToString(), f.TotalAmount)).ToList();
            return Ok(recordDtos);
        }

        
    }
}
