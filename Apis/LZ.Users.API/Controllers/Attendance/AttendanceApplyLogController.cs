using LZ.Core.Attributes;

namespace LZ.Users.API.Controllers.Attendance
{
    [PlatformLimit(Model.Enums.PlatformType.Admin)]
    [PlatformLimit(Model.Enums.PlatformType.MiniAdmin)]
    [PlatformLimit(Model.Enums.PlatformType.LZMini)]
    [PlatformLimit(Model.Enums.PlatformType.XFZMini)]
    public class AttendanceApplyLogController : UsersControllerBase
    {
    }
}
