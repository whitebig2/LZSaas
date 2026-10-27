using LZ.Users.Service.Interface;
using LZ.Core.Interface;
using LZ.Model;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.EventData;
using LZ.Model.PagedModel;
using SqlSugar.SplitTableExtensions;
using NPOI.SS.Formula.Functions;
using LZ.Core;

namespace LZ.Users.Service.Services
{
    public class PersonnelItemsService(IEventPublisher publisher) : ServicesBase<Live_PersonnelItems>, IPersonnelItemsService, IScopedDependency
    {
        public async Task<List<Live_V_PersonnelItems>> List(Expression<Func<Live_V_PersonnelItems, bool>> exp, Model.PagedModel.UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_V_PersonnelItems>().Where(exp).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_V_PersonnelItems>().Where(exp).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }

        public async Task<List<Live_PersonnelItems>> ListModel(Expression<Func<Live_PersonnelItems, bool>> exp, Model.PagedModel.UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await base.Context.Queryable<Live_PersonnelItems>().Where(exp).ToListAsync();
            }
            var list = await base.Context.Queryable<Live_PersonnelItems>().Where(exp).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<int> Add(Live_PersonnelItems model)
        {
            var id = await base.Context.Insertable(model).ExecuteReturnIdentityAsync();
            if (id > 0)
            {
                SendQrCodeEvent(id);
            }
            return id;
        }
        public async Task<bool> Add(List<Live_PersonnelItems> models)
        {
            var has = await base.Context.Insertable(models).ExecuteCommandIdentityIntoEntityAsync();
            if (has)
            {
                foreach (var model in models)
                {
                    SendQrCodeEvent(model.ID);
                }
            }
            return has;
        }
        public async Task<bool> Exist(int id,int companyId=0)
        {
            var expable = Expressionable.Create<Model.Live_V_PersonnelItems>();
            expable.And(it => it.ID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await base.Context.Queryable<Live_V_PersonnelItems>().AnyAsync(exp);
        }
        public async Task<bool> Exist(Expression<Func<Live_V_PersonnelItems, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_PersonnelItems>().AnyAsync(exp);
        }
        public async Task<Live_V_PersonnelItems> Get(Expression<Func<Live_V_PersonnelItems, bool>> exp)
        {
            return await base.Context.Queryable<Live_V_PersonnelItems>().FirstAsync(exp);
        }
        public async Task<Live_PersonnelItems> GetInfo(Expression<Func<Live_PersonnelItems, bool>> exp)
        {
            return await base.Context.Queryable<Live_PersonnelItems>().FirstAsync(exp);
        }
        public async Task<bool> Update(Live_PersonnelItems model)
        {
            return await base.Context.Updateable(model).IgnoreColumns(ignoreAllNullColumns: true).ExecuteCommandAsync() > 0;
        }
        private void SendQrCodeEvent(int id)
        {
            try
            {
                publisher.Publish(new PersonnelQrcodeEventData()
                {
                    FromID = id
                });
            }
            catch (Exception ex) {
                Common.StaticLog.Error(ex);
            }
            
        }
        public async Task<bool> SaveQrcode(int id, string path)
        {
            return await base.Context.Updateable<Live_PersonnelItems>().SetColumns(it => it.Qrcode == path).Where(it=>it.ID==id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> SaveJDYJson(int id, string json, string jdyId)
        {
            return await base.Context.Updateable<Live_PersonnelItems>().SetColumns(it => it.JDYJson == json).SetColumns(it=>it.JDYDataID==jdyId).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Set(int id, bool enabled)
        {
            return await base.Context.Updateable<Live_PersonnelItems>().SetColumns(it => it.Enabled == enabled).Where(it=>it.ID==id).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> SetManage(int id, bool enabled)
        {
            return await base.Context.Updateable<Live_PersonnelItems>().SetColumns(it => it.IsManage == enabled).Where(it => it.ID == id).ExecuteCommandAsync() > 0;
        }
        public async Task<Live_V_PersonnelItems> Get(Guid id)
        {
            return await base.Context.Queryable<Live_V_PersonnelItems>().FirstAsync(it => it.GID == id);
        }
        public async Task<Live_V_PersonnelItems> Get(int id,int companyId=0)
        {
            var expable = Expressionable.Create<Model.Live_V_PersonnelItems>();
            expable.And(it => it.ID == id);
            if (companyId>0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await base.Context.Queryable<Live_V_PersonnelItems>().FirstAsync(exp);
        }
        public async Task<bool> Delete(int id)
        {
            return await base.Context.Deleteable<Live_PersonnelItems>().Where(it=>it.ID==id).ExecuteCommandAsync() > 0;
        }
        public async Task<int> DeleteList(List<Model.Live_PersonnelItems> list)
        {
            return await base.Context.Deleteable(list).ExecuteCommandAsync();
        }
        public async Task<List<Model.SalePersonnelModel>> SalePersonnelList(int cid)
        {
            var items = await base.Context.Queryable<Live_V_PersonnelItems>().Where(t => t.CID == cid && t.IsManage).ToListAsync();
            var customer = await base.Context.Queryable<Live_Customer>().Where(t => t.CustomerID == cid).FirstAsync();
            DateTime endTime = DateTime.Today;
            if (customer!=null&& customer.CreateTime.HasValue)
            {
                endTime = Convert.ToDateTime(customer.CreateTime.Value.ToString("yyyy-MM-dd")).AddDays(31);
            }
            var entrys = await base.Context.Queryable<Live_V_UserEntry>().Where(t =>t.Status==2 && t.CustomerID == cid &&  t.EntryTime<endTime).ToListAsync();
            List<Model.SalePersonnelModel> list = new List<SalePersonnelModel>();
            foreach(var x in items)
            {
                Model.SalePersonnelModel model = new SalePersonnelModel();
                model.CustomerName = x.CustomerName;
                model.FullName = x.FullName;
                model.UserID = x.UserID;
                model.CID = x.CID;
                model.Count = entrys.Where(t => t.PerPartnerID == x.PartnerID).ToList().Count();
                list.Add(model);
            }
            return list;
        }

        public async Task<List<Live_PersonnelItems>> SaleBatchQR(string phones, int customerID)
        {
            List<Live_PersonnelItems> list = new List<Live_PersonnelItems>();
            List<Live_PersonnelItems> plist = new List<Live_PersonnelItems>();
            var arr = phones.Split(',');
            var PersonnelList = await base.Context.Queryable<Live_Personnel>().Where(t => arr.Contains(t.Phone)).ToListAsync();
            foreach (var x in PersonnelList)
            {
                Live_PersonnelItems item = new Live_PersonnelItems();
                item.GID = Guid.NewGuid();
                item.UserID = x.UserID;
                item.CID = customerID;
                item.Enabled = true;
                item.IsJoin = false;
                item.IsManage = true;
                list.Add(item);
            }
            var count = await base.Context.Insertable(list).ExecuteCommandAsync();
            if (count > 0)
            {

                plist = await base.Context.Queryable<Live_PersonnelItems>().Where(t => t.CID == customerID && t.Enabled).ToListAsync();
                plist = plist.Where(t => t.Qrcode == null).ToList();
                foreach (var x in plist)
                {
                    try
                    {
                        publisher.Publish(new PersonnelQrcodeEventData()
                        {
                            FromID = x.ID
                        });
                    }
                    catch (Exception ex)
                    {
                        Common.StaticLog.Error(ex);
                    }
                }
            }
            return plist;
        }
    }
}
