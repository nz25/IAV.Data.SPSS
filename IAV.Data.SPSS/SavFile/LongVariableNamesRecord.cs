﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.SavFile
{
    public class LongVariableNamesRecord
    {
        public Dictionary<string, string> VarNamePairs { get; set; }
        public File File { get; set; }

        public LongVariableNamesRecord(File file)
        {
            this.File = file;
        }

        public void ReadFromInfoRecord(InfoRecord ir)
        {
            if (ir.RecordSubType != RecordSubType.LongVariableNamesRecord || ir.ItemSize != 1)
            {
                throw new Exception("Invalid items in LongVariableNamesRecord");
            }

            var originalBytes = (from item in ir.Items select item[0]).ToArray();
            var dictionaryString = Encoding.Default.GetString(originalBytes); // TO DO: Globalize Encoding??
            // split on tabs:
            var entries = dictionaryString.Split('\t');
            this.VarNamePairs = new Dictionary<string, string>();
            foreach (var entry in entries)
            {
                var values = entry.Split('=');
                this.VarNamePairs.Add(values[0], values[1]);
            }

        }
    }
}
