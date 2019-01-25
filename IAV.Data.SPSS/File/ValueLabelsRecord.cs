using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.File
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

        public void ReadFromStream(BinaryReader r)
        {
            this.ValueLabelCount = r.ReadInt32();
            this.ValueLabels = new List<ValueLabel>();

            for (int i = 0; i < this.ValueLabelCount; i++)
            {
                ValueLabel vl = new ValueLabel();
                vl.ByteArrayValue = r.ReadBytes(8);
                vl.LabelLength = r.ReadByte();
                //Rounding up to nearest multiple of 32 bits.
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
    }
}
