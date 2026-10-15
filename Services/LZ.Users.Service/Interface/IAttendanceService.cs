using LZ.Model;
using LZ.Model.Model;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static LZ.Model.Model.AttendanceModel;

namespace LZ.Users.Service.Interface
{
    public interface IAttendanceService
    {
        Task<byte[]> StatisticsExportExcel(List<Live_V_AttendanceRecord> list, List<Model.Live_V_AttendanceApply> applys, List<Model.Live_V_UserEntry> entrys, int structureID, DateTime start, DateTime end, UserPagedModel userPagedModel);
    }
}
