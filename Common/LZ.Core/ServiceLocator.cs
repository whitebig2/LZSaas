using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using LZ.Core.Interface;
using SqlSugar.IOC;
using System.Linq.Expressions;

namespace LZ.Core
{
    public static class ServiceLocator
    {
        public static IServiceProvider Instance { get; set; }
    }
}
