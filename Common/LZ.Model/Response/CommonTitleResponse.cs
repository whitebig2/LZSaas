using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Response
{
    public class CommonTitleResponse<T>
    {
        public string Title { get; set; }
        public T Items { get; set; }
    }


    public class CommonItem<T>
    {
        public string KeyName { get; set; }
        public T  KeyValue{ get; set; }
    }

    public class PrepaidPartnerGroupItem
    {
        public int ApplyType { get; set; }
        public Guid PartnerGID { get; set; }
        public string PartnerName { get; set; }
        public decimal MonthTotalAmount { get; set; }
        public decimal YearTotalAmount { get; set; }
    }

    public class PrepaidCustomerGroupItem
    {
        public int ApplyType { get; set; }
        public Guid CustomerGID { get; set; }
        public string CustomerName { get; set; }
        public decimal MonthTotalAmount { get; set; }
        public decimal YearTotalAmount { get; set; }
    }
}
