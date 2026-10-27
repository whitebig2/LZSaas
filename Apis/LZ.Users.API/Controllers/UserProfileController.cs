using LZ.Core;
using LZ.Core.Interface;
using LZ.Model.Exceptions;
using LZ.Model.PagedModel;
using LZ.Users.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LZ.Users.API.Controllers
{
    /// <summary>
    /// 用户画像（会员侧）：离线物化画像的查询与展示。
    /// 画像数据由岗位匹配服务夜间预计算写入（JobRecommendPrecomputeJob），
    /// 本控制器只读：列表（分页筛选）与详情（偏好解析 + 岗位匹配记录）。
    /// </summary>
    public class UserProfileController(IUserProfileService userProfileService) : UsersControllerBase
    {
        /// <summary>
        /// 画像列表：分页 + 筛选。
        /// CID 公司（默认当前用户公司）/ SeekStatus 求职状态(0未知 1在职稳定 2在职看机会 3求职中 4沉默) /
        /// EntryJobID 工种（匹配工种偏好）。按画像计算时间倒序。
        /// 行字段：姓名手机号、WorkTypeTop 偏好工种、信誉分、秒退次数、平均在岗、求职状态、流动性、薪资心理价位等。
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> List([FromQuery] PagedModelBase query, byte? seekStatus = null, int entryJobID = 0)
        {
            var user = CurrentUser;
            var companyId = query.CID ?? (user?.CompanyID ?? 0);
            var models = await userProfileService.List(companyId, seekStatus, entryJobID, query);
            return Ok(new { items = models, count = query.Count });
        }

        /// <summary>
        /// 画像详情（单人）：工种/类目偏好解析为明细列表（ID+名称+分值，降序），
        /// 附信誉（入职/投递/秒退/平均在岗）、状态（求职状态/流动性）、薪资心理价位、
        /// 以及该会员的岗位匹配记录（JobMatches，过滤下架/过期岗位）。
        /// 查询键：优先 userGID（GUID），未传时按 userID。无画像记录（未跑批/无行为）返回空。
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> Detail(int userID = 0, Guid? userGID = null)
        {
            if ((userGID == null || userGID == Guid.Empty) && userID <= 0)
            {
                throw new ErrorCodeException(-1, "参数错误");
            }
            var model = await userProfileService.Get(userID, userGID);
            if (model == null)
            {
                return Ok(new { });
            }
            return Ok(model);
        }
    }
}
