using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Core.Services;
using LZ.Model.PagedModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Basic.Service.Interface
{
    public interface ISchedulerService
    {
        Task<List<Model.Live_Scheduler>> List(Expression<Func<Model.Live_Scheduler, bool>> exp, BasicPagedModel query, string order = "");
        Task<int> Add(Model.Live_Scheduler model);
        Task<int> Adds(List<Model.Live_Scheduler> models);
        Task<bool> Update(Model.Live_Scheduler model);
        Task<bool> Exist(Expression<Func<Model.Live_Scheduler, bool>> exp);
        Task<bool> ExistID(int id);
        Task<bool> Delete(int id);
        Task<Model.Live_Scheduler> Get(int id);

        Task<int> AddLog(Model.Live_SchedulerLog model);
        

        Task<bool> RunJob(Model.Live_Scheduler sch, IHttpService httpService, IRedisService redisService);
        Task<bool> PauseJob(Model.Live_Scheduler sch);
        Task<bool> DeleteJob(Model.Live_Scheduler sch);
        Task<List<LZ.Model.Model.SchedulerModel>> Process();

        Task<int> Add(Model.Live_SchedulerLog model);
    }
}
