using LZ.Core.Interface;
using LZ.Core;
using LZ.Statistics.Service.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Model;
using System.Linq.Expressions;

namespace LZ.Statistics.Service.Services
{
    public class JobVedioServicecs : ServicesBase<Model.Live_V_JobVideo>, IJobVedioService, IScopedDependency
    {
        public async Task<int> InfoCount(Expression<Func<Model.Live_V_JobVideo, bool>> exp = null)
        {
            var qb = base.Context.Queryable<Live_V_JobVideo>();
            return await qb.CountAsync(exp);
        }
    }
}
