using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Enums
{
    public enum OpenAccountProcessStatusEnumType
    {
        待开户 = 0,
        开户失败 = 1,
        报表上传失败 = 2,
        影像资料上传失败 = 3,
        待查询开户结果 = 4,
        完成 = 5,
    }
}
