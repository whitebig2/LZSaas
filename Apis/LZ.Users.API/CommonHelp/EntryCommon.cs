using LZ.Model.Exceptions;
using System.Globalization;

namespace LZ.Users.API.CommonHelp
{
    public class EntryCommon
    {
        public static void NoEntryTime(Model.CompanyConfig config)
        {
            if (!string.IsNullOrEmpty(config.NoEntryTimes))
            {
                var now = DateTime.Now;
                var times = config.NoEntryTimes.Split('-');
                if (times.Length > 1)
                {
                    var startTime = Convert.ToDateTime(now.ToString("yyyy-MM-dd") + " " + times[0]);
                    var endTime = Convert.ToDateTime(now.ToString("yyyy-MM-dd") + " " + times[1]);
                    if (startTime >= endTime) endTime=endTime.AddDays(1);
                    if (now >= startTime && now <= endTime)
                    {
                        throw new ErrorCodeException(-1, "在" + config.NoEntryTimes + "时间段不可办理入职");
                    }
                }
            }
        }
    }
}
