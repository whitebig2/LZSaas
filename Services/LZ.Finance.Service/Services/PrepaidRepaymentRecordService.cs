using LZ.Finance.Service.Interface;
using LZ.Model;
using LZ.Model.PagedModel;
using SqlSugar.IOC;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using LZ.Core.Interface;
using LZ.Core;
using LZ.Model.Exceptions;
using LZ.Model.Enums;
using Microsoft.AspNetCore.JsonPatch.Internal;

namespace LZ.Finance.Service.Services
{
    public class PrepaidRepaymentRecordService : ServicesBase<Live_PrepaidRepaymentRecord>,  IPrepaidRepaymentRecordService, IScopedDependency
    {

        public async Task<List<Live_PrepaidRepaymentRecord>> PageList(Expression<Func<Live_PrepaidRepaymentRecord, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await   base.Context.Queryable<Live_PrepaidRepaymentRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
            }
            var list = await   base.Context.Queryable<Live_PrepaidRepaymentRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }


        public async Task<List<Live_PrepaidRepaymentRecord>> List(Expression<Func<Live_PrepaidRepaymentRecord, bool>> exp)
        {

            return await   base.Context.Queryable<Live_PrepaidRepaymentRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        }

        //public async Task<List<Live_V_PrepaidApplyRecord>> ListView(Expression<Func<Live_V_PrepaidApplyRecord, bool>> exp)
        //{
        //    return await   base.Context.Queryable<Live_V_PrepaidApplyRecord>().Where(exp).OrderByDescending(it => it.CreateTime).ToListAsync();
        //}

    

        public async Task<int> Update(List<Live_PrepaidRepaymentRecord> models)
        {
            return await   base.Context.Updateable(models).ExecuteCommandAsync();
        }
        public async Task<bool> Exist(Expression<Func<Live_PrepaidRepaymentRecord, bool>> exp)
        {
            return await   base.Context.Queryable<Live_PrepaidRepaymentRecord>().AnyAsync(exp);
        }


        public async Task<bool> Delete(Expression<Func<Live_PrepaidRepaymentRecord, bool>> exp)
        {
            return await   base.Context.Deleteable<Live_PrepaidRepaymentRecord>().Where(exp).ExecuteCommandAsync()>0;
        }

    }
}
