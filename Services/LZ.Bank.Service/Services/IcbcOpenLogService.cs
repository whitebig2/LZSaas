using LZ.Bank.Service.Interface;
using LZ.Core.Interface;
using LZ.Core;
using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.ApiService.Interface;
using Senparc.Weixin.WxOpen.AdvancedAPIs.WxApp.WxAppJson;
using System.Security.Cryptography;

namespace LZ.Bank.Service.Services
{
    public class IcbcOpenLogService : ServicesBase<Live_IcbcOpenLog>, IICBCService, IScopedDependency
    {
        public async Task<int> Add(Live_IcbcOpenLog model)
        {
            return await base.Context.Insertable(model).ExecuteCommandAsync();

        }
    }
}
