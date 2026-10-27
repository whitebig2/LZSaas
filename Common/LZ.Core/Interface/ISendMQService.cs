using LZ.Model.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using LZ.Model;

namespace LZ.Core.Interface
{
    public interface ISendMQService
    {
        void SendStatisticsMessage(string json);
    }
}
