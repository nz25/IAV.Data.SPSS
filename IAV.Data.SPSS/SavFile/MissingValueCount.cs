using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.SavFile
{
    public enum MissingValueCount
    {
        NoMissingValue = 0,
        OneMissingValue = 1,
        TwoMissingValues = 2,
        ThreeMissingValues = 3,
        RangeOfMissingValues = -2,
        RangeOfMissingValuesPlusSingleDescreteValue = -3
    }
}
