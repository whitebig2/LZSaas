using LZ.Model.Enums;
using Microsoft.EntityFrameworkCore.Query.Internal;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    [SugarTable("Live_SalarySplitSend")]
    public class Live_SalarySplitSendExtension: Live_SalarySplitSend
    {
        [Navigate(NavigateType.OneToMany, nameof(Live_SalarySplitSendRecord.SalarySplitSendID))]
        public List<Live_SalarySplitSendRecord> Records { get; set; }




        public void Split(decimal splitAmount, string remark="")
        {
            try
            {
                if (Records == null)
                {
                    Records = new List<Live_SalarySplitSendRecord>();
                }

                if (splitAmount<=0)
                {
                    throw new Exception("拆分金额必须大于0");
                }

                if (splitAmount <= this.PendingSplitAmount)
                {
                    Live_SalarySplitSendRecord record = new Live_SalarySplitSendRecord()
                    {
                        CreateTime = DateTime.Now,
                        ID = Guid.NewGuid(),
                        Remark = remark,
                        SalarySplitSendID = this.ID,
                        SendTime = null,
                        SplitAmount = splitAmount,
                        Status = Enums.SalarySplitSendRecordStatusEnumType.待发放,
                    };
                    Records.Add(record);


                    Count();
                }
                else
                {

                    throw new Exception("拆分金额大于当前待拆分金额");
                }
            }
            catch (Exception ex)
            {

                throw ex;
            }




        }


        /// <summary>
        /// 设置完成
        /// </summary>
        public void SetComplete()
        {
            try
            {
                if (Records == null)
                {
                    Records = new List<Live_SalarySplitSendRecord>();
                }

                if (this.PendingSplitAmount>0)
                {
                    throw new Exception("存在待拆分金额，无法执行此操作");
                }

                if (Records.Any(f=>f.Status== Enums.SalarySplitSendRecordStatusEnumType.待发放||f.Status== Enums.SalarySplitSendRecordStatusEnumType.发放失败))
                {
                    throw new Exception("存在待发放或发放失败记录");
                }
                var sendTotalAmount = Records.Where(f => f.Status == Enums.SalarySplitSendRecordStatusEnumType.已发放 || f.Status == Enums.SalarySplitSendRecordStatusEnumType.取消发放).Sum(x => x.SplitAmount);

                if (sendTotalAmount<this.TotalAmount)
                {
                    throw new Exception("已发放或者取消发放合计金额与总金额不一致，无法执行此操作，请联系管理员");
                }
                Count();


            }
            catch (Exception ex)
            {

                throw ex;
            }




        }


        /// <summary>
        /// 设置完成
        /// </summary>
        public void SetRecordStatus(Guid recordID, SalarySplitSendRecordStatusEnumType status)
        {
            try
            {
                if (Records == null)
                {
                    Records = new List<Live_SalarySplitSendRecord>();
                }
                if (Records.Any(f=>f.ID== recordID))
                {
                    var record = Records.First(f => f.ID == recordID);



                    if (record.Status == SalarySplitSendRecordStatusEnumType.取消发放)
                    {
                        throw new Exception("【取消发放】状态，无法修改，如需发放，请删除后重新分配金额");
                    }

                    if (record.Status == SalarySplitSendRecordStatusEnumType.已发放)
                    {
                        throw new Exception("已发放记录禁止操作");
                    }

                    record.Status = status;
                    if (status == SalarySplitSendRecordStatusEnumType.已发放)
                    {
                        record.SendTime = DateTime.Now;
                    }
                    else {
                        record.SendTime = null;
                    }
                }

                Count();

            }
            catch (Exception ex)
            {

                throw ex;
            }




        }



        /// <summary>
        /// 设置完成
        /// </summary>
        public void DeleteRecord(Guid recordID)
        {
            try
            {
                if (Records == null)
                {
                    Records = new List<Live_SalarySplitSendRecord>();
                }
                if (Records.Any(f => f.ID == recordID))
                {
                    var record = Records.First(f => f.ID == recordID);

                    if (record.Status== SalarySplitSendRecordStatusEnumType.已发放)
                    {
                        throw new Exception("已发放记录禁止删除");
                    }
                    Records.Remove(record);
                }

                Count();

            }
            catch (Exception ex)
            {

                throw ex;
            }




        }


        private void Count()
        {
            if (Records==null)
            {
                Records = new List<Live_SalarySplitSendRecord>();
            }
            // 待拆分金额
            this.PendingSplitAmount = this.TotalAmount - Records.Sum(x => x.SplitAmount);
            if (this.PendingSplitAmount > 0)
            {
                this.Status = Enums.SalarySplitSendStatusEnumType.处理中;
            }

            if (Records.Any(f => f.Status == SalarySplitSendRecordStatusEnumType.已发放))
            {
                this.SendSucAmount = Records.Where(f => f.Status == SalarySplitSendRecordStatusEnumType.已发放).Sum(f => f.SplitAmount);
            }
            else
            {
                this.SendSucAmount = 0;
            }

           var sucAmount = Records.Where(f => f.Status == SalarySplitSendRecordStatusEnumType.已发放|| f.Status == SalarySplitSendRecordStatusEnumType.取消发放).Sum(f => f.SplitAmount);


            if (sucAmount == this.TotalAmount)
            {
                this.SendTime = DateTime.Now;   
                this.Status = SalarySplitSendStatusEnumType.已处理;
            }


        }
    }
}
