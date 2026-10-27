using LZ.Model.Enums;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Core.Attributes
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple=true)]
    public class PlatformLimitAttribute : Attribute
    {
        public PlatformType platform { get; set; }
        public PlatformLimitAttribute(PlatformType platformType)
        {
            platform = platformType;
        }
    }
}
