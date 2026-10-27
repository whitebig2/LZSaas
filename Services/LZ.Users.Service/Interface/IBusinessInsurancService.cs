using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Users.Service.Interface
{
    public interface IBusinessInsurancService
    {
        Task CheckProcess();
        Task EmailNotice();
        Task PushProcess();
    }
}
