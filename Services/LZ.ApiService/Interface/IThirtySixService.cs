using LZ.Model.Exceptions;
using LZ.Model.InsuranceApi.ThirtySix;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace LZ.ApiService.Interface
{
    public interface IThirtySixService
    {

        record TokenRecord(string Id, string Account, string password);
        Task<ThirtySixCusResponse> PushReplace(List<ThirtySixReplaceRecord> recordList);
        Task<ThirtySixCusResponse> PushAdd(List<ThirtySixAddRecord> records);
        Task<ThirtySixResponse<List<ThirtySixResponseRecord>>> QueryDetail();
    }
}
