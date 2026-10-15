using LZ.Users.Service.Interface;
using LZ.Core.Interface;
using LZ.Core.Redis;
using LZ.Model;
using SqlSugar;
using SqlSugar.IOC;
using System.Linq.Expressions;
using LZ.Core;
using System.ComponentModel.Design;
using Consul;
using Senparc.Weixin.MP.AdvancedAPIs.QrCode;
using LZ.Core.RabbitMQ.EventBus;
using LZ.Core.EventData;
using Polly;
using static Dm.net.buffer.ByteArrayBuffer;

namespace LZ.Users.Service.Services
{
    public class PersonnelService(IEventPublisher publisher) : IPersonnelService, IScopedDependency
    {
        public async Task<List<Live_Personnel>> List(Expression<Func<Live_Personnel, bool>> exp, Model.UserPagedModel query)
        {
            var count = new RefAsync<int>();
            if (query.Page == 0)
            {
                return await DbScoped.SugarScope.Queryable<Live_Personnel>().Where(exp).ToListAsync();
            }
            var list = await DbScoped.SugarScope.Queryable<Live_Personnel>().Where(exp).ToOffsetPageAsync(query.Page, query.PageSize, count);
            query.Count = count.Value;
            return list;
        }
        public async Task<Model.Live_Personnel> Login(string phone,int companyId)
        {
            return await DbScoped.SugarScope.Queryable<Model.Live_Personnel>().FirstAsync(it => it.Phone == phone && it.CompanyID==companyId);
        }
        public async Task<int> Add(Live_Personnel model)
        {
            var models=new List<Live_PersonnelItems>();
            if (!string.IsNullOrEmpty(model.CIDS))
            {
                var ids= model.CIDS.Split(',');
                foreach (var id in ids)
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        var m=new Live_PersonnelItems();
                        m.CID = Convert.ToInt32(id);
                        models.Add(m);
                    }
                }
            }
            var db = DbScoped.SugarScope;
            var result = await db.Ado.UseTranAsync<int>(async () =>
            {
               var id=await db.Insertable(model).ExecuteReturnIdentityAsync();
                foreach (var m in models)
                {
                    m.UserID = id;
                }
                await db.Insertable(models.ToArray()).ExecuteCommandAsync();
                return id;
            });
            if (result.IsSuccess)
            {
                SendQrCodeEvent(models, model.CompanyID.Value);
                return result.Data;
            }
            else
            {
                Common.StaticLog.Error("新增招聘专员错误", result.ErrorException);
                return 0;
            }
        }
        public async Task<bool> Exist(int id,int companyId=0)
        {
            var expable = Expressionable.Create<Model.Live_Personnel>();
            expable.And(it => it.UserID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await DbScoped.SugarScope.Queryable<Live_Personnel>().AnyAsync(exp);
        }
        public async Task<bool> Exist(Expression<Func<Live_Personnel, bool>> exp)
        {
            return await DbScoped.SugarScope.Queryable<Live_Personnel>().AnyAsync(exp);
        }
        public async Task<bool> Update(Live_Personnel model)
        {
            var models = new List<Live_PersonnelItems>();
            if (!string.IsNullOrEmpty(model.CIDS))
            {
                var ids = model.CIDS.Split(',');
                foreach (var id in ids)
                {
                    if (!string.IsNullOrEmpty(id))
                    {
                        var m = new Live_PersonnelItems();
                        m.CID = Convert.ToInt32(id);
                        m.UserID = model.UserID;
                        models.Add(m);
                    }
                }
            }
            var insertModel= new List<Live_PersonnelItems>();
            //var oldModels = await DbScoped.SugarScope.Queryable<Live_PersonnelItems>().Where(it => it.UserID == model.UserID).ToListAsync();
            //insertModel = models.Where(it => !oldModels.Select(it => it.CID).Contains(it.CID)).ToList();
            //var deleteModel = oldModels.Where(it => !models.Select(it => it.CID).Contains(it.CID));
            var db = DbScoped.SugarScope;
            var result = await db.Ado.UseTranAsync<bool>(async () =>
            {
                var has = await db.Updateable(model).Where(it => it.UserID == model.UserID && it.CompanyID == model.CompanyID).ExecuteCommandAsync() > 0;
                if (has)
                {
                    var oldModels = await db.Queryable<Live_PersonnelItems>().Where(it => it.UserID == model.UserID).ToListAsync();
                    insertModel = models.Where(it => !oldModels.Select(it => it.CID).Contains(it.CID)).ToList();
                    var deleteModel = oldModels.Where(it => !models.Select(it => it.CID).Contains(it.CID));
                    if (deleteModel.Count() > 0)
                    {
                        await db.Deleteable<Live_PersonnelItems>().Where(it => deleteModel.Select(it => it.CID).Contains(it.CID) && it.UserID == model.UserID).ExecuteCommandAsync();
                    }
                    if (insertModel.Count() > 0)
                    {
                        await db.Insertable(insertModel.ToArray()).ExecuteCommandAsync();
                    }
                }
                return has;
            });
            if (result.IsSuccess)
            {
                SendQrCodeEvent(insertModel, model.CompanyID.Value);
                return result.Data;
            }
            else
            {
                Common.StaticLog.Error("更新招聘专员错误", result.ErrorException);
                return false;
            }
        }
        private void SendQrCodeEvent(List<Live_PersonnelItems> models,int companyId)
        {
            try
            {
                foreach (var m in models)
                {
                    // 待处理发布事件
                    publisher.Publish(new PersonnelQrcodeEventData()
                    {
                        UserID = m.UserID,
                        CustomerID = m.CID,
                        CompanyID = companyId
                    });
                }
            }
            catch (Exception ex) {
                Common.StaticLog.Error(ex);
            }
            
        }
        public async Task<bool> SaveQrcode(int userId,int customerId, string path)
        {
            return await DbScoped.SugarScope.Updateable<Live_PersonnelItems>().SetColumns(it => it.Qrcode == path).Where(it=>it.UserID==userId && it.CID== customerId).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> Set(int id, bool enabled, int companyId = 0)
        {
            var expable = Expressionable.Create<Model.Live_Personnel>();
            expable.And(it => it.UserID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await DbScoped.SugarScope.Updateable<Live_Personnel>().SetColumns(it => it.Enabled == enabled).Where(exp).ExecuteCommandAsync() > 0;
        }

        public async Task<Live_Personnel> Get(int id,int companyId=0)
        {
            var expable = Expressionable.Create<Model.Live_Personnel>();
            expable.And(it => it.UserID == id);
            if (companyId>0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await DbScoped.SugarScope.Queryable<Live_Personnel>().FirstAsync(exp);
        }
        public async Task<Live_PersonnelOpenIds> GetOpenID(string openid)
        {
            return await DbScoped.SugarScope.Queryable<Live_PersonnelOpenIds>().FirstAsync(it => it.OpenId == openid);
        }
        public async Task<bool> Delete(int id, int companyId = 0)
        {
            var expable = Expressionable.Create<Model.Live_Personnel>();
            expable.And(it => it.UserID == id);
            if (companyId > 0)
            {
                expable.And(it => it.CompanyID == companyId);
            }
            var exp = expable.ToExpression();
            return await DbScoped.SugarScope.Deleteable<Live_Personnel>().Where(exp).ExecuteCommandAsync() > 0;
        }
        public async Task<int> ChangePicture(int uid,string picture)
        {
            return await DbScoped.SugarScope.Updateable<Live_Personnel>().SetColumns(it => it.Picture == picture).Where(it => it.UserID == uid).ExecuteCommandAsync();
        }
    }
}
