using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model
{
    public class ProductModel
    {
        public List<Shop_SKUs>? skus { get; set; } = null;
        public List<ProductAttributeModel>? attrs { get; set; } = null;
        public Shop_Product? model { get; set; } = null;
    }
    public class ProductAttributeModel
    {
        public int AttributeID { get; set; }

        /// <summary>
        /// 
        /// </summary>		

        public string AttributeName { get; set; }

        /// <summary>
        /// 
        /// </summary>		

        public int? TypeID { get; set; }

        /// <summary>
        /// 
        /// </summary>		

        public int? Type { get; set; }

        /// <summary>
        /// 
        /// </summary>		

        public bool IsImage { get; set; }

        /// <summary>
        /// 
        /// </summary>		

        public int? ProductID { get; set; }

        /// <summary>
        /// 
        /// </summary>		

        public int? Sort { get; set; }

        /// <summary>
        /// 
        /// </summary>		

        public string AttributeStrID { get; set; }
        public List<Shop_AttributeValues>? values { get; set; } = null;
    }

    /// <summary>
    /// 商品评价统计
    /// </summary>
    public class CommentSummary
    {
        public int Count { get; set; }
        public decimal AvgScore { get; set; }
        public int PictureCount { get; set; }
        public int GoodCount { get; set; }
        public int Score1 { get; set; }
        public int Score2 { get; set; }
        public int Score3 { get; set; }
        public int Score4 { get; set; }
        public int Score5 { get; set; }
    }

    /// <summary>
    /// 搜索统计
    /// </summary>
    public class VisitSearchStat
    {
        public string Search { get; set; }
        public string Keyword { get; set; }
        public int Count { get; set; }
    }
}
