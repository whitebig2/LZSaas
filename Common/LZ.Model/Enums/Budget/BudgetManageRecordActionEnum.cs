using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums.Budget
{
    public enum BudgetManageRecordActionEnum
    {
        管理员充值 = 0,
        服务费充值 = 1,
        客服申请扣款 = 2,
        回滚打款前 = 3,
        管理员提现 = 4,
        划拨划入 = 5,
        划拨划出 = 6,
        总部申请已打款 = 7,
        已打款 = 8,
        作废 = 9,
        回滚作废 = 10,
        总部回滚 = 11,
        系统充值 = 12,
    }
}
