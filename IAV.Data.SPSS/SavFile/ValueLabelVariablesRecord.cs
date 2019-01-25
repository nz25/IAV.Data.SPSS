using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.SavFile
{
    public class ValueLabelVariablesRecord
    {
        public File File { get; set; }
        public RecordType RecordType { get; set; }
        public int VariableCount { get; set; }
        public List<int> VariableIndices { get; set; }

        public ValueLabelVariablesRecord(File file)
        {
            this.RecordType = RecordType.ValueLabelVariablesRecord;
            this.File = file;
        }

        public void ReadFromStream(BinaryReader r)
        {
            this.VariableCount = r.ReadInt32();
            this.VariableIndices = new List<int>();
            for (int i = 0; i < this.VariableCount; i++)
            {
                this.VariableIndices.Add(r.ReadInt32());
            }
        }

        public void WriteToStream(BinaryWriter w)
        {
            w.Write(this.VariableCount);
            foreach (int vi in this.VariableIndices)
            {
                w.Write(vi);    
            }
        }
    }
}
