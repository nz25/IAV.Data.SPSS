using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.File
{
    public class InfoRecord
    {
        public SavFile File { get; set; }
        public RecordType RecordType { get; set; }
        public RecordSubType RecordSubType { get; set; }
        public int ItemSize { get; set; } 
        public int ItemCount { get; set; } 
        public List<byte[]> Items { get; set; }

        public InfoRecord(SavFile file)
        {
            this.RecordType = RecordType.InfoRecord;
            this.File = file;
        }

        public void ReadFromStream(BinaryReader r)
        {
            this.RecordSubType = (RecordSubType)r.ReadInt32();
            this.ItemSize = r.ReadInt32();
            this.ItemCount = r.ReadInt32();
            this.Items = new List<byte[]>();
            for (int i = 0; i < this.ItemCount; i++)
            {
                this.Items.Add(r.ReadBytes(this.ItemSize));
            }
        }
    }
}
