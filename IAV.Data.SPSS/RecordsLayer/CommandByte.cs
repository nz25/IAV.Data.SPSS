﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.RecordsLayer
{
    public enum CommandByte
    {
        Ignored = 0,
        EndOfFile = 252,
        NotCompressibleValue = 253,
        AllSpacesString = 254,
        SystemMissingValue = 255
    }
}
