using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LZ.Model.Dtos
{
    public record CustomizeFieldRecord(string Title,string Field,object Value);
    public record InsuranceEntryLimitResult(bool IsCanEntry, string InsuranceCompany, string EntryRule, string Message);

}
