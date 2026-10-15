using System;
using System.Collections.Generic;

namespace LZ.Model.Dtos.FinanceDo
{
    public class PaymentAdminSignUnitDto
    {
        public Guid ID { get; set; }
        public Guid PaymentAdminID { get; set; }
        public Guid PaymentSignUnitID { get; set; }
        public bool IsCanView { get; set; }
        public bool IsCanPay { get; set; }
        public bool IsCanManage { get; set; }
    }

    public class PaymentAdminSignUnitReqDto
    {
        public Guid PaymentAdminID { get; set; }
        public List<PaymentAdminSignUnitDto> Units { get; set; }
    }

    /// <summary>
    /// 权限管理推送：按 PaymentAdminID 对签约单位权限做新增/修改/删除同步
    /// </summary>
    public class PaymentAdminSignUnitPermissionReqDto
    {
        public Guid PaymentAdminID { get; set; }
        public List<PaymentAdminSignUnitDto> Units { get; set; }
    }
}
