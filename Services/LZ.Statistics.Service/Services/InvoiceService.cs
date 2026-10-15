using LZ.Core;
using LZ.Core.Interface;
using LZ.Model;
using LZ.Statistics.Service.Interface;
using SqlSugar;
using SqlSugar.IOC;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Statistics.Service.Services
{
    internal class InvoiceService: ServicesBase<Model.Live_Invoice>, IInvoiceService, IScopedDependency
    {
        public async Task<int> Count(Expression<Func<Live_Invoice, bool>> exp = null)
        {
            var qb = base.Context.Queryable<Live_Invoice>();
            return await qb.CountAsync(exp);
        }


        public async Task<decimal> SumAmount(Expression<Func<Live_Invoice, bool>> exp)
        {
            var qb = base.Context.Queryable<Live_Invoice>();
            qb.Where(exp);
            return await qb.SumAsync(f => f.InvoiceAmount);
        }
    }
}
