using LZ.Customer.Service.Interface;
using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;
using System.Text;
using LZ.Model.PagedModel;


namespace LZ.Customer.Service.Services
{
    public class JobsService : ServicesBase<Live_Jobs>, IJobsService, IScopedDependency
    {
        public async Task<List<Live_V_Jobs>> List(Expression<Func<Live_V_Jobs, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var qb = base.Context.Queryable<Live_V_Jobs>();
            qb.Where(exp);
            if (!string.IsNullOrEmpty(query.OrderField))
            {
                if (string.IsNullOrEmpty(query.Order)) query.Order = "asc";
                qb.OrderBy(query.OrderField + " " + query.Order);
            }
            else
            {
                qb.OrderByDescending(it => it.ID);
            }
            if (query.Page == 0)
            {
                if (query.Top > 0)
                {
                    qb.Take(query.Top);
                }
                return await qb.ToListAsync();
            }
            var list = await qb.ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_V_Jobs>> Nearby(Expression<Func<Live_V_Jobs, bool>> exp, PagedModelBase query, double latitude, double longitude, double radiusKm)
        {
            var latDelta = radiusKm / 111.0;
            var cosLat = Math.Cos(latitude * Math.PI / 180.0);
            var lngDelta = Math.Abs(cosLat) < 0.000001 ? 180 : radiusKm / (111.0 * Math.Abs(cosLat));

            var qb = base.Context.Queryable<Live_V_Jobs>();
            qb.Where(exp);
            qb.Where(it => it.Latitude != null && it.Longitude != null
                && it.Latitude >= latitude - latDelta && it.Latitude <= latitude + latDelta
                && it.Longitude >= longitude - lngDelta && it.Longitude <= longitude + lngDelta);

            var list = await qb.ToListAsync();
            var nearby = list
                .Select(it =>
                {
                    var km = GetDistanceKm(latitude, longitude, it.Latitude.Value, it.Longitude.Value);
                    it.Distance = FormatDistance(km);
                    return (job: it, km);
                })
                .Where(it => it.km <= radiusKm)
                .OrderBy(it => it.km)
                .Select(it => it.job)
                .ToList();

            query.Count = nearby.Count;
            if (query.Page == 0)
            {
                if (query.Top > 0)
                {
                    return nearby.Take(query.Top).ToList();
                }
                return nearby;
            }
            return nearby.Skip((query.Page - 1) * query.PageSize).Take(query.PageSize).ToList();
        }

        private static double GetDistanceKm(double lat1, double lng1, double lat2, double lng2)
        {
            const double earthRadiusKm = 6371;
            var dLat = (lat2 - lat1) * Math.PI / 180;
            var dLng = (lng2 - lng1) * Math.PI / 180;
            var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                  + Math.Cos(lat1 * Math.PI / 180) * Math.Cos(lat2 * Math.PI / 180)
                  * Math.Sin(dLng / 2) * Math.Sin(dLng / 2);
            return earthRadiusKm * 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
        }

        private static string FormatDistance(double km)
        {
            var meters = km * 1000;
            if (meters < 1000)
            {
                return $"{Math.Max(1, (int)Math.Round(meters))}米";
            }
            return km < 10 ? $"{Math.Round(km, 1)}公里" : $"{Math.Round(km, 0)}公里";
        }

        public async Task<List<Model.Live_Jobs>> ListImport(Expression<Func<Model.Live_Jobs, bool>> exp, PagedModelBase query)
        {
            var count = new RefAsync<int>();
            var qb = base.Context.Queryable<Live_Jobs>();
            qb.Where(exp);
            if (!string.IsNullOrEmpty(query.OrderField))
            {
                if (string.IsNullOrEmpty(query.Order)) query.Order = "asc";
                qb.OrderBy(query.OrderField + " " + query.Order);
            }
            else
            {
                qb.OrderByDescending(it => it.ID);
            }
            if (query.Page == 0)
            {
                if (query.Top > 0)
                {
                    qb.Take(query.Top);
                }
                return await qb.ToListAsync();
            }
            var list = await qb.ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<string>> GetCity(int companyId)
        {
            return await base.Context.Queryable<Live_V_Jobs>().Where(it => !string.IsNullOrEmpty(it.City) && it.XFZShow && it.CompanyID == companyId).GroupBy(it => it.City).Select(it => it.City).ToListAsync();
        }
        private string FormatTag(Model.Live_Jobs model)
        {
            var tags = new StringBuilder();
            if (!string.IsNullOrEmpty(model.Structure))
            {
                if (model.Structure.Contains("计时"))
                {
                    tags.Append("计时,");
                }
                else if (model.Structure.Contains("计件"))
                {
                    tags.Append("计件,");
                }
            }
            if (!string.IsNullOrEmpty(model.Foot))
            {
                if (model.Foot.Contains("免费"))
                {
                    tags.Append("包吃,");
                }
                else if (model.Foot.Contains("餐补"))
                {
                    tags.Append("有餐补,");
                }
            }
            if (!string.IsNullOrEmpty(model.Hostel))
            {
                if (model.Hostel.Contains("免费"))
                {
                    tags.Append("包住,");
                }
                else if (model.Hostel.Contains("补贴"))
                {
                    tags.Append("有房补,");
                }
            }
            if (!string.IsNullOrEmpty(model.HostelCondition))
            {
                if (model.HostelCondition.Contains("夫妻房"))
                {
                    tags.Append("夫妻房,");
                }
                if (model.HostelCondition.Contains("双人间"))
                {
                    tags.Append("双人间,");
                }
            }
            if (!string.IsNullOrEmpty(model.WorkMode))
            {
                if (model.WorkMode.Contains("坐着"))
                {
                    tags.Append("坐着上班,");
                }
            }
            if (!string.IsNullOrEmpty(model.Workshop))
            {
                if (model.Workshop.Contains("空调车间"))
                {
                    tags.Append("空调车间,");
                }
            }
            if (!string.IsNullOrEmpty(model.Intensity))
            {
                if (model.Intensity.Contains("轻"))
                {
                    tags.Append("工作轻松,");
                }
            }
            if (!string.IsNullOrEmpty(model.WorkTime))
            {
                if (model.WorkTime.Contains("长白班"))
                {
                    tags.Append("长白班,");
                }
            }
            if (!string.IsNullOrEmpty(model.UserAsk))
            {
                if (model.UserAsk.Contains("经验不限"))
                {
                    tags.Append("经验不限,");
                }
                if (model.UserAsk.Contains("学历不限"))
                {
                    tags.Append("学历不限,");
                }
            }
            if (!string.IsNullOrEmpty(model.NightSubsidy))
            {
                tags.Append("夜班补贴,");
            }
            if (tags.Length > 0)
            {
                tags.Length--;
            }
            return tags.ToString();
        }
        public async Task<int> Add(Live_Jobs model)
        {
            model.ShowTags = FormatTag(model);
          return  await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> Exist(int id,int companyId=0)
        {
            var expable = Expressionable.Create<Model.Live_V_Jobs>();
            expable.And(it => it.ID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await base.Context.Queryable<Live_V_Jobs>().AnyAsync(exp);
        }
        public async Task<bool> Update(Live_Jobs model)
        {
            model.ShowTags = FormatTag(model);
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> UpdateList(List<Model.Live_Jobs> list)
        {
            return await base.Context.Updateable(list).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> ChangeStatus(int id,int status)
        {
            return await base.Context.Updateable<Live_Jobs>().SetColumns(it => it.Status == status).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_UserJobQrcode> GetUserQrcode(int id,int userId)
        {
            return await base.Context.Queryable<Live_UserJobQrcode>().FirstAsync(it => it.JobID == id && it.UserID== userId);
        }
        public async Task<int> AddUserQrcode(Live_UserJobQrcode model)
        {
            return await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
        }
        public async Task<bool> ChangeIntegral(int jobId, int integral)
        {
            var has = await base.Context.Updateable<Model.Live_Jobs>().SetColumns(it => it.Integrals ==SqlFunc.IsNull(it.Integrals,0) + integral).Where(it => it.ID == jobId).ExecuteCommandAsync() > 0;
            return has;
        }
        public async Task<Live_Jobs> Get(int id)
        {
            return await base.Context.Queryable<Live_Jobs>().FirstAsync(it=>it.ID == id);
        }
        public async Task<Live_V_Jobs> GetView(int id)
        {
            return await base.Context.Queryable<Live_V_Jobs>().FirstAsync(it => it.ID == id);
        }
        public async Task<bool> Set(int id,string filed, bool enabled)
        {
            var db = base.Context.Updateable<Live_Jobs>();
            if (filed == "min")
            {
                db.SetColumns(it => it.MinShow == enabled);
            }
            else
            {
                db.SetColumns(it => it.XFZShow == enabled);
            }
            return await db.Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_Jobs>().Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
    }
}
