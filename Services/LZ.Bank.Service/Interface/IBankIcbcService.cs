using LZ.Model.ICBC.Plkh;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Bank.Service.Interface
{
    public interface IBankIcbcService
    {
        /// <summary>
        /// 合作方开卡申请
        /// </summary>
        /// <param name="requestXml"></param>
        /// <returns></returns>
        public Task<PlkhResult> OpenAccountApplyAsync(string filePath, string fileName);
        /// <summary>
        ///  // 开户合作方上送影像申请
        /// </summary>
        /// <param name="requestXml"></param>
        /// <returns></returns>
        public Task<PlkhResult> OpenAccountUploadImageApplyAsync(string filePath, string fileName, DateTime kkrq);
        /// <summary>
        /// 开户作方开卡结果查询
        /// </summary>
        /// <param name="requestXml"></param>
        /// <returns></returns>
        public Task<PlkhResult> OpenAccountQueryDetailAsync(string name, string cardID, DateTime kkrq);
    }
}
