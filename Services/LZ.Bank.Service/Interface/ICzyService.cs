using LZ.Model.Bank.CZY;
using LZ.Model.Bank.CZY.ParamSetModels;
using LZ.Model.Bank.CZY.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Bank.Service.Interface
{
    public interface ICzyService
    {
        Task<CzyCmscloud<T0006Response>> QueryAccountBalanceInfo(T0006ParamSetItem paramSetItem);
        Task<CzyCmscloud<T0005Response>> QueryAccountInfo(T0005ParamSetItem paramSetItem);
        Task<CzyCmscloud<T0028Response>> QueryElectronicReceiptList(T0028ParamSetItem paramSetItem);
        Task<CzyCmscloud<T0004Response>> QueryTransactionDetail(T0004ParamSetItem paramSetItem);
        Task<CzyCmscloud<T0001Response>> QueryTransactionHistoryRecords(T0001ParamSetItem paramSetItem);
        Task<CzyCmscloud<T0018Response>> TransBatch(T0018ParamSetItem paramSetItem);
        Task<CzyCmscloud<T0003Response>> TransBatchToPerson(T0003ParamSetItem paramSetItem);
        Task<CzyCmscloud<T0002Response>> TransSingle(T0002ParamSetItem paramSetItem);
    }
}
