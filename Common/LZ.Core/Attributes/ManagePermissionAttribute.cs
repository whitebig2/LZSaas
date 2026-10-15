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
    public class ManagePermissionAttribute : Attribute
    {
        public string permission { get; set; }
        public ManagePermissionAttribute(string permissions)
        {
            permission = permissions;
        }
    }
}
