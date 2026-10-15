using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.PagedModel
{
    public class PageResultModel<T> where T : class,new()
    {
        public int Count { get; set; }
        public List<T> Items { get; set; }
        public PageResultModel()
        {
            Items=new List<T>();
        }

        public PageResultModel(int count, List<T> items)
        {
            Count = count;
            Items = items;
        }
    }
}
