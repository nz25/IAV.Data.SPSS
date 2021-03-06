﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAV.Data.SPSS.Enum;

namespace IAV.Data.SPSS.SavFile
{
    public class VariableDisplayParameterRecord
    {
        public int ParameterCount { get; set; }
        public List<VariableParameters> VariableParameters { get; set; }

        public File File { get; set; }


        public VariableDisplayParameterRecord(File file)
        {
            this.File = file;
        }

        public void ReadFromInfoRecord(InfoRecord ir)
        {
            if (ir.RecordSubType != RecordSubType.VariableDisplayParameterRecord || ir.ItemSize != 4 )
            {
                throw new Exception("Invalid items in VariableDisplayParameterRecord");
            }

            // Record can either have 2 or 3 fields per variable:
            int variableCount = this.GetVariableCountWithoutExtentions();
            this.ParameterCount = ir.ItemCount / variableCount;
            if (this.ParameterCount != 2 && this.ParameterCount != 3 && ir.ItemCount % variableCount > 0)
            {
                throw new Exception("Invalid ItemCount in VariableDisplayParameterRecord");
            }

            this.VariableParameters = new List<VariableParameters>();
     
            for (int variableIndex = 0; variableIndex < variableCount; variableIndex++)
            {
                VariableParameters vp = new VariableParameters();
                vp.MeasurementType = (MeasurementType)BitConverter.ToInt32(ir.Items[0],  0);
                if (this.ParameterCount == 3)
                {
                    //Width parameter exists
                    vp.Width = BitConverter.ToInt32(ir.Items[1], 0);
                    vp.Alignment = (Alignment)BitConverter.ToInt32(ir.Items[2], 0);
                }
                else if (this.ParameterCount == 2)
                {
                    //Width parameter is omitted
                    vp.Alignment = (Alignment)BitConverter.ToInt32(ir.Items[1], 0);
                }
                this.VariableParameters.Add(vp);
            }
        }

        private int GetVariableCountWithoutExtentions()
        {
            // Ignores variable extentions
            return (from record in this.File.VariableRecords
                    where record.Type != VariableType.StringExtention
                    select record).Count();
        }

    }
}
