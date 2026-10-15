using AutoMapper;
using LZ.Model.Dtos.InvoiceDto;
using LZ.Model.Dtos.SettlementDtos;
using LZ.Model.ExcelTemplateModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace LZ.Model
{
   

    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
        
            // 工资结算单
            CreateMap<SalaryRecordExcelTemplate, Live_SalaryRecord>();
            CreateMap<InvoiceManageDto, Live_Invoice>();
            CreateMap<SettlementManageDto, Live_Settlement>();
            CreateMap<Live_V_SalaryRecord, Live_SalaryRecord>();
        }
    }
}
