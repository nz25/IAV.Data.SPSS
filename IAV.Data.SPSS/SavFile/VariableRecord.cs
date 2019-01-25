using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAV.Data.SPSS.Enum;

namespace IAV.Data.SPSS.SavFile
{
    public class VariableRecord
    {
        public File File { get; set; }
        public RecordType RecordType { get; set; }
        public VariableType Type { get; set; }
        public Int32 HasVariableLabel { get; set; }
        public MissingValueCount MissingValueCount { get; set; }
        public VariableFormat PrintFormat { get; set; }
        public VariableFormat WriteFormat { get; set; }
        public string Name { get; set; } // 8
        public Int32 LabelLength { get; set; } // 0-120
        public string Label { get; set; }
        public List<double> MissingValues { get; private set; }

        public VariableRecord(File file)
        {
            this.RecordType = RecordType.VariableRecord;
            this.File = file;
            this.PrintFormat = new VariableFormat();
            this.WriteFormat = new VariableFormat();
        }

        public void ReadFromStream()
        {
            BinaryReader r = this.File.Reader;
            this.Type = (VariableType)r.ReadInt32();
            this.HasVariableLabel = r.ReadInt32();
            this.MissingValueCount = (MissingValueCount)r.ReadInt32();
            this.PrintFormat.ReadFromInt(r.ReadInt32());
            this.WriteFormat.ReadFromInt(r.ReadInt32());
            this.Name = new String(r.ReadChars(8));

            if (this.HasVariableLabel == 1)
            {
                this.LabelLength = r.ReadInt32();
                //Rounding up to nearest multiple of 32 bits.
                int labelBytes = (((this.LabelLength - 1) / 4) + 1) * 4;
                this.Label = new String(r.ReadChars(labelBytes));
            }

            int positiveMissingValueCount = Math.Abs((Int32)this.MissingValueCount);
            this.MissingValues = new List<double>(positiveMissingValueCount);
            for (int i = 0; i < positiveMissingValueCount; i++)
            {
                this.MissingValues.Add(r.ReadDouble());
            }
        }

        public void WriteToStream()
        {
            BinaryWriter w = this.File.Writer;
            w.Write((Int32)this.RecordType);
            w.Write((Int32)this.Type);
            w.Write(this.HasVariableLabel);
            w.Write((Int32)this.MissingValueCount);
            w.Write(this.PrintFormat.WriteToInt());
            w.Write(this.WriteFormat.WriteToInt());
            w.Write(this.Name.ToCharArray());
            if (this.HasVariableLabel == 1)
            {
                w.Write(this.LabelLength);
                //Rounding up to nearest multiple of 32 bits.
                int labelBytes = (((this.LabelLength - 1) / 4) + 1) * 4;
                w.Write(this.Label.PadRight(labelBytes).ToCharArray());
            }

            foreach (double missingValue in this.MissingValues)
	        {
		        w.Write(missingValue);
	        }
        }
    }
}
