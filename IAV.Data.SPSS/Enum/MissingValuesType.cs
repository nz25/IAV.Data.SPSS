using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.Enum
{
    public enum MissingValuesType
    {
        NoMissingValues = 1,
        DescreteMissingValues = 2,
        RangeOfMissingValues = 3,
        RangeOfMissingValuesPlusSingleDescreteValue = 4
    }
}
