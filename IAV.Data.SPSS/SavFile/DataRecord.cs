﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.SavFile
{
    public class DataRecord
    {
        public File File { get; set; }
        public byte[][] Values { get; set; } // decompressed values - 8 bytes per variable record
        
        public DataRecord(File file)
        {
            this.File = file;
            this.Values = new byte[file.VariableRecords.Count][];
        }

       
    }
}
