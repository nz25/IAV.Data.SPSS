using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.SavFile
{
    public enum CommandByte
    {
        Ignored = 0,
        EndOfFile = 252,
        NotCompressibleValue = 253,
        EightSpacesString = 254,
        SystemMissingValue = 255
    }
}
