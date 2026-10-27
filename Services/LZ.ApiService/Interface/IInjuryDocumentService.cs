using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.ApiService.Interface
{
    public interface IInjuryDocumentService
    {
        Task<string> GenerationTemplate(Model.Live_V_InjuryInfo model);
    }
}
