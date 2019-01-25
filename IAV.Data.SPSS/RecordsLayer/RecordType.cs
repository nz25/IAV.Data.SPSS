using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.RecordsLayer
{
    public enum RecordType
    {
        FileHeaderRecord = 843859492, //$FL2
        VariableRecord = 2,
        ValueLabelRecord = 3,
        ValueLabelVariablesRecord = 4,
        DocumentRecord = 6,
        InfoRecord = 7,
        DictionaryTerminationRecord = 999
    }
}
