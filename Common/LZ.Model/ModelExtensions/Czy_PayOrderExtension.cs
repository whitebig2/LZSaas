using SqlSugar;
using System.Collections.Generic;

namespace LZ.Model
{
    [SugarTable("Czy_PayOrder")]
    public class Czy_PayOrderExtension : Czy_PayOrder
    {
        [Navigate(NavigateType.OneToMany, nameof(Czy_PayOrderDetail.PayOrderID))]
        public List<Czy_PayOrderDetail> Records { get; set; }
    }
}
