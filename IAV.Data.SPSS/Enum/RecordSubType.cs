using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.Enum
{
    public enum RecordSubType
    {
        MachineIntegerInfoRecord = 3,
        MachineFloatingPointInfoRecord = 4,
        MultipleResponseSetsRecord = 7,
        VariableDisplayParameterRecord = 11,
        LongVariableNamesRecord = 13,
        VeryLongStringRecord = 14,
        VariableRolesRecord = 18,
        CodePageRecord = 20,
        LongValueLabelsRecord = 21
    }
}
