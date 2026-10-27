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

namespace LZ.Finance.Service.Services
{
    public class CityPartnerBillService : ServicesBase<Live_CityPartnerBill>,  ICityPartnerBillService, IScopedDependency
    {
        public async Task<List<Live_CityPartnerBill>> PageList(Expression<Func<Live_CityPartnerBill, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var list = await   base.Context.Queryable<Live_CityPartnerBill>().Where(exp).OrderByDescending(it => it.CreateTime).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public Task<List<Live_CityPartnerBill>> List(Expression<Func<Model.Live_CityPartnerBill, bool>> exp = null)
        {
            return   base.Context.Queryable<Live_CityPartnerBill>().Where(exp).ToListAsync();
        }
        public async Task<int> Add(Live_CityPartnerBill model)
        {
             await   base.Context.Insertable(model).ExecuteCommandIdentityIntoEntityAsync();
            if (model.ID > 0)
            {
                return model.ID;
            }
            else {

                return 0;
            }
        }

        public async Task<int> Add(List<Live_CityPartnerBill> models)
        {
            return await   base.Context.Insertable(models).ExecuteCommandAsync();
        }
        
        public async Task<bool> Update(Live_CityPartnerBill model)
        {
            return await   base.Context.Updateable<Live_CityPartnerBill>(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_CityPartnerBill> Get(Expression<Func<Live_CityPartnerBill, bool>> exp)
        {
            return await   base.Context.Queryable<Live_CityPartnerBill>().FirstAsync(exp);
        }
        public async Task<bool> Delete(int id)
        {
            return await   base.Context.Deleteable<Live_CityPartnerBill>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
