using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.ApiModel.FxqModel
{

    public record Signer(string Name,string IdNo,bool IsCompany,bool IsNeedDate,bool IsNeedPageSign,string ElectronicSealFile);
    public class GeneratePdfContractRequest
    {
        /// <summary>
        /// 关键字
        /// </summary>
        public Dictionary<string, string> Keywords { get; set; }
        public string ContractOssFilePath { get; set; }
    }
    public class SigningPdfContractRequest
    {
        public string ContractOssFilePath { get; set; }
        public List<Signer> Signers { get; set; }
    }



}
