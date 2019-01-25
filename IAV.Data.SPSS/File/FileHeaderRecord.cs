using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.File
{
    public class FileHeaderRecord
    {
        public SavFile File { get; set; }
        public RecordType RecordType { get; set; }
        public string ProductName { get; set; }
        public Int32 LayoutCode { get; set; }
        public Int32 NominalCaseSize { get; set; }
        public bool IsCompressed { get; set; }
        public Int32 WeightIndex { get; set; }
        public Int32 CaseCount { get; set; }
        public Double Bias { get; set; }
        public string CreationDateString { get; set; } // dd mmm yy
        public string CreationTimeString { get; set; } // hh:mm:ss
        public string FileLabel { get; set; }
        public string Padding { get; set; }


        public FileHeaderRecord(SavFile file)
        {
            this.File = file;
            this.RecordType = RecordType.FileHeaderRecord;
        }

        public void ReadFromStream (BinaryReader r)
        {
            this.ProductName = new String(r.ReadChars(60));
            this.LayoutCode = r.ReadInt32();
            this.NominalCaseSize = r.ReadInt32();
            this.IsCompressed = (r.ReadInt32() == 1);
            this.WeightIndex = r.ReadInt32();
            this.CaseCount = r.ReadInt32();
            this.Bias = r.ReadDouble();
            this.CreationDateString = new String(r.ReadChars(9));
            this.CreationTimeString = new String(r.ReadChars(8));
            this.FileLabel = new String(r.ReadChars(64));
            this.Padding = new String(r.ReadChars(3));

        }

         
    }
}
