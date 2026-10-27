using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Statistics.Service.Services
{
    public class SequenceService
    {

        //public void SalaryCalculateSequenceInit()
        //{

        //    string prefix = "SalaryCalculate_";
        //    string existSql = $@"
        //    SELECT name 
        //    FROM sys.sequences 
        //    WHERE name like '{prefix}%' 
        //      AND schema_name(schema_id) = 'dbo';
        //    ";
        //   var existList= base.Context.Ado.SqlQuery<string>(existSql);
        //    var today = DateTime.Today;
        //    var lastDay= today.AddDays(-1);
        //    var currentMonth = DateTime.Today.AddDays(1 - DateTime.Today.Day);
        //    var lastMonth = currentMonth.AddMonths(-1);
        //    string todayName = prefix + today.ToString("yyyyMMdd");
        //    string monthName = prefix + currentMonth.ToString("yyyyMM");
        //    string yearName = prefix + DateTime.Today.ToString("yyyy");
        //    string lastDayName = prefix + lastDay.ToString("yyyyMMdd");
        //    string lastMonthName = prefix + lastMonth.ToString("yyyyMM");
        //    if (!existList.Any(f=>f==todayName))
        //    {
        //        string sql = $@"
        //        CREATE SEQUENCE dbo.{todayName}
        //          AS BIGINT
        //          START WITH 0
        //          INCREMENT BY 1
        //          CACHE 10;
        //        ";
        //        base.Context.Ado.ExecuteCommandAsync(sql);
        //        if (!existList.Any(f => f == lastDayName))
        //        {
        //            string delSql = $"DROP SEQUENCE IF EXISTS dbo.{lastDayName}";

        //            base.Context.Ado.ExecuteCommandAsync(delSql);
        //        }
        //    }
        //    if (!existList.Any(f => f == monthName))
        //    {
        //        string sql = $@"
        //        CREATE SEQUENCE dbo.{monthName}
        //          AS BIGINT
        //          START WITH 0
        //          INCREMENT BY 1
        //          CACHE 1;
                
        //        ";
        //        base.Context.Ado.ExecuteCommandAsync(sql);


        //        if (!existList.Any(f => f == lastMonthName))
        //        {
        //            string delSql = $"DROP SEQUENCE IF EXISTS dbo.{lastMonthName}";

        //            base.Context.Ado.ExecuteCommandAsync(delSql);
        //        }
        //    }
        //    if (!existList.Any(f => f == yearName))
        //    {
        //        string sql = $@"
        //        CREATE SEQUENCE dbo.{yearName}
        //          AS BIGINT
        //          START WITH 0
        //          INCREMENT BY 1
        //          CACHE 1;
                
        //        ";
        //        base.Context.Ado.ExecuteCommandAsync(sql);
        //    }
        //}




    }
}
