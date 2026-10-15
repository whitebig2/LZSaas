using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Common
{
    public static class WorkDayHelper
    {

        public static bool IsWorkDay2025(DateTime date)
        {


            var holidays = new HashSet<DateTime>
            {
                // 元旦
                new DateTime(2025, 1, 1),
                
                // 春节
                new DateTime(2025, 1, 28),
                new DateTime(2025, 1, 29),
                new DateTime(2025, 1, 30),
                new DateTime(2025, 1, 31),
                new DateTime(2025, 2, 1),
                new DateTime(2025, 2, 2),
                new DateTime(2025, 2, 3),
                
                // 清明节
                new DateTime(2025, 4, 4),
                new DateTime(2025, 4, 5),
                new DateTime(2025, 4, 6),
                
                // 劳动节
                new DateTime(2025, 5, 1),
                new DateTime(2025, 5, 2),
                new DateTime(2025, 5, 3),
                new DateTime(2025, 5, 4),
                new DateTime(2025, 5, 5),
                
                // 端午节
                new DateTime(2025, 5, 31),
                new DateTime(2025, 6, 1),
                new DateTime(2025, 6, 2),
                
                // 中秋节
                new DateTime(2025, 9, 6),
                new DateTime(2025, 9, 7),
                new DateTime(2025, 9, 8),
                
                // 国庆节
                new DateTime(2025, 10, 1),
                new DateTime(2025, 10, 2),
                new DateTime(2025, 10, 3),
                new DateTime(2025, 10, 4),
                new DateTime(2025, 10, 5),
                new DateTime(2025, 10, 6),
                new DateTime(2025, 10, 7)
            };

            var specialWorkDays = new HashSet<DateTime>
            {
                // 春节调休
                new DateTime(2025, 1, 25),  // 星期日上班
                new DateTime(2025, 2, 8),   // 星期六上班
                
                // 劳动节调休
                new DateTime(2025, 4, 27),  // 星期日上班
                new DateTime(2025, 5, 10),  // 星期六上班
                
                // 国庆节调休
                new DateTime(2025, 9, 28),  // 星期日上班
                new DateTime(2025, 10, 11)  // 星期六上班
            };


            // 1. 检查是否为调休工作日（周末但需要工作）
            if (specialWorkDays.Contains(date.Date))
                return true;

            // 2. 检查是否为节假日
            if (holidays.Contains(date.Date))
                return false;

            // 3. 检查是否为周末
            DayOfWeek day = date.DayOfWeek;
            if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)
                return false;

            // 4. 默认情况：周一到周五且非节假日
            return true;
        }

        public static bool IsWorkDay2026(DateTime date)
        {


            var holidays = new HashSet<DateTime>
            {
              
                
                // 端午节
                new DateTime(2026, 6, 19),
                new DateTime(2026, 6, 20),
                new DateTime(2026, 6, 21),
                
                // 中秋节
                new DateTime(2026, 9, 25),
                new DateTime(2026, 9, 26),
                new DateTime(2026, 9, 27),
                
                // 国庆节
                new DateTime(2026, 10, 1),
                new DateTime(2026, 10, 2),
                new DateTime(2026, 10, 3),
                new DateTime(2026, 10, 4),
                new DateTime(2026, 10, 5),
                new DateTime(2026, 10, 6),
                new DateTime(2026, 10, 7),


                // --- 2027年新增数据 ---
    
    // 元旦 (2027年1月1日-3日，共3天，与周末连休)
    new DateTime(2027, 1, 1),
    new DateTime(2027, 1, 2),
    new DateTime(2027, 1, 3),

    // 春节 (2027年2月5日-13日，共9天)
    new DateTime(2027, 2, 5),
    new DateTime(2027, 2, 6),
    new DateTime(2027, 2, 7),
    new DateTime(2027, 2, 8),
    new DateTime(2027, 2, 9),
    new DateTime(2027, 2, 10),
    new DateTime(2027, 2, 11),
    new DateTime(2027, 2, 12),
    new DateTime(2027, 2, 13),

    // 清明节 (2027年4月3日-5日，共3天，与周末连休)
    new DateTime(2027, 4, 3),
    new DateTime(2027, 4, 4),
    new DateTime(2027, 4, 5),

    // 劳动节 (2027年5月1日-5日，共5天)
    new DateTime(2027, 5, 1),
    new DateTime(2027, 5, 2),
    new DateTime(2027, 5, 3),
    new DateTime(2027, 5, 4),
    new DateTime(2027, 5, 5),

    // 端午节 (2027年6月9日，周三，放假1天，不调休)
    new DateTime(2027, 6, 9),

    // 中秋节 (2027年9月15日，周三，放假1天，不调休)
    new DateTime(2027, 9, 15),

    // 国庆节 (2027年10月1日-7日，共7天)
    new DateTime(2027, 10, 1),
    new DateTime(2027, 10, 2),
    new DateTime(2027, 10, 3),
    new DateTime(2027, 10, 4),
    new DateTime(2027, 10, 5),
    new DateTime(2027, 10, 6),
    new DateTime(2027, 10, 7)
            };

            var specialWorkDays = new HashSet<DateTime>
            {
                // 国庆节调休
                new DateTime(2026, 9, 20),  // 星期日上班
                new DateTime(2026, 10, 10),   // 星期六上班
            };


            // 1. 检查是否为调休工作日（周末但需要工作）
            if (specialWorkDays.Contains(date.Date))
                return true;

            // 2. 检查是否为节假日
            if (holidays.Contains(date.Date))
                return false;

            // 3. 检查是否为周末
            DayOfWeek day = date.DayOfWeek;
            if (day == DayOfWeek.Saturday || day == DayOfWeek.Sunday)
                return false;

            // 4. 默认情况：周一到周五且非节假日
            return true;
        }

    }
}
