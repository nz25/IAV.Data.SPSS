using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.SavFile
{
    public enum RecordSubType
    {
        MachineIntegerInfoRecord = 3,
        MachineFloatingPointInfoRecord = 4,
        MultipleResponseSetsRecord = 7,
        VariableDisplayParameterRecord = 11,
        LongVariableNamesRecord = 13,
        VeryLongStringRecord = 14
    }
}
