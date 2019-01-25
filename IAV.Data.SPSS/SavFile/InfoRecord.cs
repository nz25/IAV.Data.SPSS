using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.SavFile
{
    public class InfoRecord
    {
        public File File { get; set; }
        public RecordType RecordType { get; set; }
        public RecordSubType RecordSubType { get; set; }
        public int ItemSize { get; set; } 
        public int ItemCount { get; set; } 
        public List<byte[]> Items { get; set; }

        public InfoRecord(File file)
        {
            this.RecordType = RecordType.InfoRecord;
            this.File = file;
        }

        public void ReadFromStream()
        {
            BinaryReader r = this.File.Reader;
            this.RecordSubType = (RecordSubType)r.ReadInt32();
            this.ItemSize = r.ReadInt32();
            this.ItemCount = r.ReadInt32();
            this.Items = new List<byte[]>();
            for (int i = 0; i < this.ItemCount; i++)
            {
                this.Items.Add(r.ReadBytes(this.ItemSize));
            }
        }

        public void WriteToStream()
        {
            BinaryWriter w = this.File.Writer;
            w.Write((Int32)this.RecordType);
            w.Write((Int32)this.RecordSubType);
            w.Write(this.ItemSize);
            w.Write(this.ItemCount);
            foreach (byte[] item in this.Items)
            {
                w.Write(item);
            }
        }
    }
}
