using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.SavFile
{
    public class DocumentRecord
    {
        public SavFile File { get; set; }
        public RecordType RecordType { get; set; }
        public Int32 LineCount { get; set; }
        public List<string> Lines { get; set; }

        public DocumentRecord(SavFile file)
        {
            this.RecordType = RecordType.DocumentRecord;
            this.File = file;
        }

        public void ReadFromStream(BinaryReader r)
        {
            this.LineCount = r.ReadInt32();
            this.Lines = new List<string>();
            for (int i = 0; i < this.LineCount; i++)
            {
                this.Lines.Add(new String(r.ReadChars(80)));
            }
        }
    }
}
