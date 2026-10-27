using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos
{

    public record SingleFileSealDto
    {
        public Guid ID { get; set; }
        public string CustomerName { get; set; }
        public string Name { get; set; }
        public string CardID { get; set; }
        public string FilePath { get; set; }
        public string SignedFilePath { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string StatusName { get; set; }
    }
}
