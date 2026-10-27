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
    public class OperationLogActionAttribute : Attribute
    {
        public string MessageTemplate { get; set; }
        public OperationLogActionAttribute(string messageTemplate)
        {
            MessageTemplate = messageTemplate;
        }
    }
}
