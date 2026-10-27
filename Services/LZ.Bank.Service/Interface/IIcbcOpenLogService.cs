using LZ.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Bank.Service.Interface
{
    public interface IIcbcOpenLogService
    {
        Task<int> Add(Live_IcbcOpenLog model);
    }
}
