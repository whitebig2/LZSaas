using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LZ.Core.Interface;

namespace LZ.Core
{
    [ApiController]
    [Route("api/[controller]")]
    public abstract class ApiControllerBase : ControllerBase
    {
        public ICurrentUser CurrentUser
        {
            get
            {
                return HttpContext.RequestServices.GetService<ICurrentUser>();
            }
        }
    }
}
