using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.SavFile
{
    public class ValueLabelsRecord
    {
        public SavFile File { get; set; }
        public RecordType RecordType { get; set; }
        public Int32 ValueLabelCount { get; set; }
        public List<ValueLabel> ValueLabels { get; set; }
        public ValueLabelVariablesRecord ValueLabelVariablesRecord { get; set; }

        public ValueLabelsRecord(SavFile file)
        {
            this.RecordType = RecordType.ValueLabelRecord;
            this.File = file;
        }

        public void ReadFromStream()
        {
            BinaryReader r = this.File.Reader;
            this.ValueLabelCount = r.ReadInt32();
            this.ValueLabels = new List<ValueLabel>();

            for (int i = 0; i < this.ValueLabelCount; i++)
            {
                ValueLabel vl = new ValueLabel();
                vl.ByteArrayValue = r.ReadBytes(8);
                vl.LabelLength = r.ReadByte();
                //Rounding up to nearest multiple of 8 bytes .
                int labelBytes = (((((vl.LabelLength) / 8) + 1) * 8) - 1);
                vl.Label = new String(r.ReadChars(labelBytes));
                this.ValueLabels.Add(vl);
            }

            //Parses corresponding ValueLabelVariablesRecord
            RecordType type = (RecordType)r.ReadInt32();
            if (type != RecordType.ValueLabelVariablesRecord)
            {
                throw new Exception("ValueLabelRecord is not followed by ValueLabelVariablesRecord");
            }

            this.ValueLabelVariablesRecord = new ValueLabelVariablesRecord(this.File);
            this.ValueLabelVariablesRecord.ReadFromStream(r);
             
        }

        public void WriteToStream()
        {
            BinaryWriter w = this.File.Writer;
            w.Write((Int32)this.RecordType);
            w.Write(this.ValueLabelCount);

            foreach (ValueLabel vl in this.ValueLabels)
            {
                w.Write(vl.ByteArrayValue);
                w.Write(vl.LabelLength);
                //Rounding up to nearest multiple of 8 bytes .
                int labelBytes = (((((vl.LabelLength) / 8) + 1) * 8) - 1);
                w.Write(vl.Label.PadRight(labelBytes).ToCharArray());
            }

            //Writes corresponding ValueLabelVariablesRecord
            w.Write((Int32)this.ValueLabelVariablesRecord.RecordType);
            this.ValueLabelVariablesRecord.WriteToStream(w);
        }

    }
}
