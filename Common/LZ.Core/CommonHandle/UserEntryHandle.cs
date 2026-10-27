using Consul;
using LZ.Core.Services;
using LZ.Model;
using LZ.Model.Enums;
using NPOI.SS.Formula.Functions;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.CommonHandle
{
    public class UserEntryHandle
    {
        public static int[] businessModels = { (int)BusinessModel.Labor, (int)BusinessModel.Epiboly };
        /// <summary>
        /// 蓝招权限设置
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="user"></param>
        /// <returns></returns>
        public static Expression<Func<T, bool>> SetPersonnelAuth<T>(Model.Live_Personnel user) where T : ModelBasic
        {
            Expression<Func<T, bool>> e=null;
            if (!user.UserType.HasValue) 
            {
                user.UserType = (int)Model.Enums.PersonnelType.Personnel;
            }
            var type = (Model.Enums.PersonnelType)user.UserType;
            switch (type)
            {
                case PersonnelType.Personnel:
                case PersonnelType.View:
                case PersonnelType.Daywork:
                case PersonnelType.DayworkManage:
                    {
                        e = it => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == it.CustomerID && s.UserID == user.UserID).Any();
                    }
                    break;
                case PersonnelType.Factory:
                    {
                        // expable.And(it => it.FactoryUserID == user.UserID || it.HandUserID == user.UserID || it.PerUserID == user.UserID);
                        e = it => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == it.CustomerID && s.UserID == user.UserID && s.IsManage).Any() || it.PerUserID == user.UserID;
                    }
                    break;
                case PersonnelType.Headed:
                    {
                        e = it => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == it.CustomerID && s.UserID == user.UserID && s.IsManage).Any() || it.PartnerID == user.PartnerID || it.PerUserID == user.UserID || it.CustomerPartnerID == user.PartnerID;
                    }
                    break;
                case PersonnelType.Manage:
                    break;
                default:
                    e = it => it.PerUserID == user.UserID;
                    break;
            }
            if (e == null) e = it => 1 > 0;
            return e;
        }
        public static Expression<Func<T, bool>> SetPersonnelActionStatistics<T>(Model.Live_Personnel user, string type) where T : ModelBasic
        {
            Expression<Func<T, bool>> e = null;
            if (type == CompanyType.Party.ToString().ToLower())
            {
                e = it => it.CompanyID == user.CompanyID || SqlFunc.Subqueryable<Model.Live_CompanyAuthorize>().Where(s => s.CustomerID == it.CustomerID && s.ToCompanyID == user.CompanyID && s.Enabled).Any();
            }
            else
            {
                e = it => it.CompanyID == user.CompanyID;
            }
            return e;
        }
        public static void SetPersonnelAuthNoFactory(Expressionable<Live_V_UserEntry> expable, Model.Live_Personnel user, string cutType)
        {
            if (cutType == CompanyType.Party.ToString().ToLower())
            {
                expable.And(it => it.CompanyID == user.CompanyID || SqlFunc.Subqueryable<Model.Live_CompanyAuthorize>().Where(s => s.CustomerID == it.CustomerID && s.ToCompanyID == user.CompanyID && s.Enabled).Any());
            }
            else
            {
                expable.And(it => it.CompanyID == user.CompanyID);
            }
            if (!user.UserType.HasValue)
            {
                user.UserType = (int)Model.Enums.PersonnelType.Personnel;
            }
            expable.And(it => businessModels.Contains(it.BusinessModel.Value));
            var type = (Model.Enums.PersonnelType)user.UserType;
            switch (type)
            {
                case PersonnelType.Personnel:
                case PersonnelType.View:
                case PersonnelType.Daywork:
                case PersonnelType.DayworkManage:
                    {
                        expable.And(it => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == it.CustomerID && s.UserID == user.UserID).Any());
                    }
                    break;
                case PersonnelType.Factory:
                    {
                        expable.And(it => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == it.CustomerID && s.UserID == user.UserID && s.IsManage).Any() || it.PerUserID == user.UserID);
                    }
                    break;
                case PersonnelType.Headed:
                    {
                        expable.And(it => it.PartnerID == user.PartnerID);
                    }
                    break;
                case PersonnelType.Manage:
                    break;
                default:
                    expable.And(it => it.PerUserID == user.UserID);
                    break;
            }
        }

        public static int NowMonthInDay(Live_V_UserEntry model,DateTime? now=null)
        {
            var dt= DateTime.Now.Date;
            if (now != null)
            {
                dt = now.Value.Date;
            }
            if (model.EntryTime != null)
            {
                if (model.LeaveTime == null)
                {
                    if (model.EntryTime.Value.Year == dt.Year && model.EntryTime.Value.Month == dt.Month)
                    {
                        return dt.Day - model.EntryTime.Value.Day + 1;
                    }
                    else
                    {

                        return dt.Day;
                    }
                }
                else
                {

                    if (model.LeaveTime.Value.Year == dt.Year && model.LeaveTime.Value.Month == dt.Month)
                    {
                        if (model.LeaveTime.Value.Year == model.EntryTime.Value.Year && model.LeaveTime.Value.Month == model.EntryTime.Value.Month)
                        {
                            return model.LeaveTime.Value.Day - model.EntryTime.Value.Day;

                        }
                        else
                        {
                            return model.LeaveTime.Value.Day;
                        }

                    }
                    else
                    {
                        return 0;
                    }

                }
            }
            return 0;
        }
        public static int NowMonthInDay(Live_UserEntry model, DateTime? now = null)
        {
            var dt = DateTime.Now.Date;
            if (now != null)
            {
                dt = now.Value.Date;
            }
            if (model.EntryTime != null)
            {
                if (model.LeaveTime == null)
                {
                    if (model.EntryTime.Value.Year == dt.Year && model.EntryTime.Value.Month == dt.Month)
                    {
                        return dt.Day - model.EntryTime.Value.Day + 1;
                    }
                    else
                    {

                        return dt.Day;
                    }
                }
                else
                {

                    if (model.LeaveTime.Value.Year == dt.Year && model.LeaveTime.Value.Month == dt.Month)
                    {
                        if (model.LeaveTime.Value.Year == model.EntryTime.Value.Year && model.LeaveTime.Value.Month == model.EntryTime.Value.Month)
                        {
                            return model.LeaveTime.Value.Day - model.EntryTime.Value.Day;

                        }
                        else
                        {
                            return model.LeaveTime.Value.Day;
                        }

                    }
                    else
                    {
                        return 0;
                    }

                }
            }
            return 0;
        }
        public static Expression<Func<T, bool>> SetPersonnelOperateAuth<T>(Model.Live_Personnel user) where T : ModelBasic
        {
            Expression<Func<T, bool>> e = null;
            if (!user.UserType.HasValue)
            {
                user.UserType = (int)Model.Enums.PersonnelType.Personnel;
            }
            var type = (Model.Enums.PersonnelType)user.UserType;
            switch (type)
            {
                case PersonnelType.Personnel:
                case PersonnelType.View:
                case PersonnelType.Daywork:
                case PersonnelType.DayworkManage:
                    {
                        e = it => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == it.CustomerID && s.UserID == user.UserID).Any();
                    }
                    break;
                case PersonnelType.Factory:
                    {
                        e = it => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == it.CustomerID && s.UserID == user.UserID && s.IsManage).Any();
                    }
                    break;
                case PersonnelType.Headed:
                    {
                        e = it => SqlFunc.Subqueryable<Model.Live_PersonnelItems>().Where(s => s.CID == it.CustomerID && s.UserID == user.UserID && s.IsManage).Any() || it.CustomerPartnerID == user.PartnerID;
                    }
                    break;
                case PersonnelType.Manage:
                    break;
                default:
                    e = it => it.PerUserID == user.UserID;
                    break;
            }
            if (e == null) e = it => 1 > 0;
            return e;
        }
    }
}
