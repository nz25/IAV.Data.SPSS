using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.RecordsLayer
{
    public class VariableRecord
    {
        public SystemFile File { get; set; }
        public RecordType RecordType { get; set; }
        public Int32 Type { get; set; }
        public bool HasVariableLabel { get; set; }
        public Int32 MissingValueCount { get; set; } //If the variable has no missing values, set to 0. If the variable has one, two, or three discrete missing values, set to 1, 2, or 3, respectively. If the variable has a range for missing variables, set to -2; if the variable has a range for missing variables plus a single discrete value, set to -3. 
        public VariableFormat PrintFormat { get; set; }
        public VariableFormat WriteFormat { get; set; }
        public string Name { get; set; } // 8
        public Int32 LabelLength { get; set; } // 0-120
        public string Label { get; set; }
        public List<double> MissingValues { get; private set; }

        public VariableRecord(SystemFile file)
        {
            this.RecordType = RecordType.VariableRecord;
            this.File = file;
        }

        public void ReadFromStream(BinaryReader r)
        {
            this.Type = r.ReadInt32();
            this.HasVariableLabel = (r.ReadInt32() == 1);
            this.MissingValueCount = r.ReadInt32();
            this.PrintFormat = new VariableFormat(r.ReadInt32());
            this.WriteFormat = new VariableFormat(r.ReadInt32());
            this.Name = new String(r.ReadChars(8));

            if (this.HasVariableLabel)
            {
                this.LabelLength = r.ReadInt32();
                //Rounding up to nearest multiple of 32 bits.
                int labelBytes = (((this.LabelLength - 1) / 4) + 1) * 4;
                this.Label = new String(r.ReadChars(labelBytes));
            }
            this.MissingValues = new List<double>(Math.Abs(this.MissingValueCount));
            for (int i = 0; i < Math.Abs(this.MissingValueCount); i++)
            {
                this.MissingValues.Add(r.ReadDouble());
            }
        }

    }
}
