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
    public class PermissionOperateAttribute : Attribute
    {
        public string operate { get; set; }
        public PermissionOperateAttribute(string operates)
        {
            operate = operates;
        }
    }
}
