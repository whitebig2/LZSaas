namespace LZ.Model.PagedModel
{
    public class UserPagedModel : PagedModelBase
    {

        /// <summary>
        /// 
        /// </summary>
        public string UserName { get; set; } = string.Empty;
        /// <summary>
        /// 
        /// </summary>
        public string Phone { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Money { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public int? UserID { get; set; }
        public Guid? UserGID { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// 
        public DateTime? Time { get; set; }
        public bool? Enable { get; set; }
        public bool? IsUser { get; set; }
        public bool? IsBlack { get; set; }
        public int? Status { get; set; }
        public bool? IsJoint { get; set; }

        public bool? IsBudget { get; set; }
        public int? PartnerID { get; set; }
        public int PartnerType { get; set; }
        public string FromType { get; set; } = string.Empty;
        public string CardID { get; set; } = string.Empty;
        public string PartnerName { get; set; } = string.Empty;
        public string PersonnelName { get; set; } = string.Empty;
        public string CompanyName { get; set; } = string.Empty;
        public bool? Mark { get; set; }
        public int? EntryID { get; set; }
        public int? JobID { get; set; }
        public Guid? CustomerGID { get; set; }
        
        public bool? OperateAuth { get; set; }
        public int? ContractStatus { get; set; }
        public bool IsNoBank { get; set; }
        public DateTime? EntryStartTime { get; set; }
        public DateTime? EntryEndTime { get; set; }
        public DateTime? LeaveStartTime { get; set; }
        public DateTime? LeaveEndTime { get; set; }
        public string PartnerIDS { get; set; } = string.Empty;
        public string CustomerIDS { get; set; } = string.Empty;
        public bool? IsCustomer { get; set; }
        public string SigningUnitIDS { get; set; } = string.Empty;
        public int SummaryType { get; set; }
        public bool? IsManage        { get; set; }
        public string TimeType { get; set; } = string.Empty;
        public bool? IsPartner { get; set; }
        public bool? IsManuallyInsurance { get; set; }
        public int customerID { get; set; }
        public int structureID { get; set; }
        public int? entryID { get; set; }
        public int? AttendanceType { get; set; }
        public DateTime? StartTime { get; set; }

        public DateTime? EndTime { get; set; }
        public string DayWorkType { get; set; }

        public string SignCustomerName { get; set; }
        public string InsuranceCompany1 { get; set; }
        public bool? IsDefault { get; set; }

        public Guid? CityPartnerID { get; set; }

        public int? ShopID { get; set; }
        public int? ShopType { get; set; }
        public bool? IsShopPromotion { get; set; }

        public List<string> UserIds { get; set; }
    }
}
