using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Model.Enums.Sale;
using LZ.Statistics.Service.Interface;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Statistics.Service.Services
{
    public class SaleCustomerService : ServicesBase<Model.Live_Customer>, ISaleCustomerService, IScopedDependency
    {
        public async Task<List<Model.Model.SaleCustomerModel>> SaleEntryStatistics(string ids)
        {
            List<Model.Model.SaleCustomerModel> list = new List<Model.Model.SaleCustomerModel>();
            if (!string.IsNullOrEmpty(ids))
            {
                var arr = ids.Split(",");
                List<string> customers = new List<string>();
                foreach (var x in arr)
                {
                    customers.Add(x);
                }
                var customerList = await base.Context.Queryable<Live_Customer>().Where(t => customers.Contains(t.CustomerID.ToString())).ToListAsync();
                var entrylist = await base.Context.Queryable<Live_UserEntry>().Where(t => t.CustomerID.HasValue && customers.Contains(t.CustomerID.Value.ToString())).ToListAsync();
                //var customerlist = await base.Context.Queryable<Live_UserEntry>().Where(t =>  customers.Contains(t.ID.ToString())).ToListAsync();
                var nowtime = DateTime.Now;
                foreach (var x in customerList)
                {
                    Model.Model.SaleCustomerModel item = new Model.Model.SaleCustomerModel();
                    item.CustomerID = Convert.ToInt32(x.CustomerID);
                    item.Status = x.SaleStatus;
                    item.Content = x.SaleContent;
                    item.EntryCount = entrylist.Where(t => t.Status == 2 && t.CustomerID == x.CustomerID).ToList().Count;
                    item.EntryTodayCount = entrylist.Where(t => t.CustomerID == x.CustomerID && t.Status == 2 && t.EntryTime.HasValue && t.EntryTime.Value.ToString("yyyy-MM-dd") == nowtime.ToString("yyyy-MM-dd")).ToList().Count;
                    item.LeaveTodayCount = entrylist.Where(t => t.CustomerID == x.CustomerID && t.Status == 4 && t.LeaveTime.HasValue && t.LeaveTime.Value.ToString("yyyy-MM-dd") == nowtime.ToString("yyyy-MM-dd")).ToList().Count;
                    list.Add(item);
                }
            }



            return list;
        }
        public async Task<string> SaleUpdatePersonnel(int userID, int cid, int type)
        {
            string msg = "OK";
            var personnel = await base.Context.Queryable<Live_Personnel>().Where(t => t.UserID == userID).FirstAsync();
            if (personnel == null)
            {
                return "没有匹配的服务商负责人账号";
            }
            var personnelItems = await base.Context.Queryable<Live_V_PersonnelItems>().Where(t => t.CID == cid).ToListAsync();
            if (personnelItems.Find(t => t.UserID == userID) == null)
            {
                return "该负责人没有客户的二维码";
            }
            var items = personnelItems.Where(t => t.UserID != userID).ToList();

            Dictionary<string, object> objs = new Dictionary<string, object>();
            objs.Add("CustomerID", cid);
            objs.Add("FactoryUserID", personnel.UserID);
            objs.Add("PartnerID", personnel.PartnerID);
            objs.Add("PartnerGID", personnel.PartnerGID);
            if (items.Count > 0)
            {
                string ids = "";
                string idsName = "";
                List<int> arr = new List<int>();
                foreach (var item in items)
                {
                    string dot = !string.IsNullOrEmpty(ids) ? "," : "";
                    arr.Add(item.ID);
                    ids = dot + item.PartnerID;
                    idsName = dot + item.PartnerName;
                }
                await base.Context.Updateable<Live_PersonnelItems>().SetColumns(t => t.IsManage == false).Where(t => arr.Contains(t.ID)).ExecuteCommandAsync();

                if (!string.IsNullOrEmpty(ids) && type == 1)
                {
                    objs.Add("VicePartnerIDS", ids);
                    objs.Add("VicePartnerIDNames", idsName);
                }


            }
            if (type == 1)
            {
                objs.Add("OperateModel", "1");
            }
            int count = await base.Context.Updateable<Model.Live_Customer>(objs).With(SqlWith.UpdLock).ExecuteCommandAsync();
            if (count == 0)
            {
                msg = "更新失败:ID," + cid + "," + ",FactoryUserID:" + personnel.UserID + ",PartnerID:" + personnel.PartnerID;
            }

            return msg;
        }
        public async Task<List<Live_PersonnelItems>> PersonnelTopReserve(DateTime time)
        {
            var customers = await base.Context.Queryable<Live_Customer>().Where(t => t.IsSaleCustomer && t.CreateTime.HasValue && t.CreateTime.Value.ToString("yyyy-MM-dd") == time.ToString("yyyy-MM-dd")).ToListAsync();
            List<Live_PersonnelItems> items = new List<Live_PersonnelItems>();

            if (customers.Count > 0)
            {
                List<int?> arr = new List<int?>();
                foreach (var x in customers)
                {
                    arr.Add(x.CustomerID);
                }
                var personnelList = await base.Context.Queryable<Live_Personnel>().Where(t => t.UserType == 4).ToListAsync();//服务商负责人
                var personnelItems = await base.Context.Queryable<Live_PersonnelItems>().Where(t => arr.Contains(t.CID)).ToListAsync();

                var entrys = await base.Context.Queryable<Live_V_UserEntry>().Where(t => arr.Contains(t.CustomerID)).ToListAsync();


                foreach (var x in customers)
                {
                    List<Model.Model.SaleCustomerModel> sales = new List<Model.Model.SaleCustomerModel>();
                    var pitems = personnelItems.Where(t => t.CID == x.CustomerID).ToList();
                    //if(pitems.Count<4)
                    //{
                    //    continue;
                    //}
                    foreach (var p in pitems)
                    {
                        var personnel = personnelList.Find(t => t.UserID == p.UserID);
                        Model.Model.SaleCustomerModel sale = new Model.Model.SaleCustomerModel();
                        sale.CustomerID = x.CustomerID;
                        sale.PersonnelID = p.UserID;
                        sale.EntryCount = entrys.Where(t => t.CustomerID == x.CustomerID && t.PerPartnerID == personnel.PartnerID).Count();//入职员工的找人账号所属服务商
                        sales.Add(sale);

                    }
                    var newSales = sales.Where(t => t.EntryCount > 0).OrderByDescending(t => t.EntryCount).ToList();
                    List<Model.Model.SaleCustomerModel> deleteSales = new List<Model.Model.SaleCustomerModel>();
                    deleteSales = sales;
                    if (newSales.Count == 0)//所有负责人入职员工人数都为0
                    {
                        await base.Context.Updateable<Live_Customer>().SetColumns(t => t.SaleStatus == (int)SaleCustomerStatusEnum.分配异常).SetColumns(t => t.SaleContent == "所有服务商人数为0").Where(t => t.CustomerID == x.CustomerID).ExecuteCommandAsync();
                    }
                    else if (newSales.Count() <= 3)
                    {
                        deleteSales = sales.Except(newSales).ToList();
                        await base.Context.Updateable<Live_Customer>().SetColumns(t => t.SaleStatus == (int)SaleCustomerStatusEnum.分配成功).SetColumns(t => t.SaleContent == "分配成功").Where(t => t.CustomerID == x.CustomerID).ExecuteCommandAsync();
                    }
                    else
                    {
                        if (newSales[2].EntryCount != newSales[3].EntryCount)
                        {
                            newSales = newSales.Take(3).ToList();
                            deleteSales = sales.Except(newSales).ToList();
                            await base.Context.Updateable<Live_Customer>().SetColumns(t => t.SaleStatus == (int)SaleCustomerStatusEnum.分配成功).SetColumns(t => t.SaleContent == "分配成功").Where(t => t.CustomerID == x.CustomerID).ExecuteCommandAsync();
                        }
                        else
                        {
                            await base.Context.Updateable<Live_Customer>().SetColumns(t => t.SaleStatus == (int)SaleCustomerStatusEnum.分配异常).SetColumns(t => t.SaleContent == "无法正常获取前三个服务商负责人").Where(t => t.CustomerID == x.CustomerID).ExecuteCommandAsync();
                        }
                    }
                    foreach (var ds in deleteSales)
                    {
                        var model = pitems.Find(t => t.UserID == ds.PersonnelID && t.CID == ds.CustomerID);
                        if (model != null)
                        {
                            items.Add(model);
                        }
                    }

                }

            }


            return items;
        }
    }
}
