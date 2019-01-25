using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.RecordsLayer
{
    public class VeryLongStringRecord
    {
        public Dictionary<string, string> StringLengths { get; set; }

        public SystemFile File { get; set; }


        public VeryLongStringRecord(SystemFile file)
        {
            this.File = file;
        }


        public void ReadFromInfoRecord(InfoRecord ir)
        {
            if (ir.RecordSubType != RecordSubType.VeryLongStringRecord || ir.ItemSize != 1)
            {
                throw new Exception("Invalid items in VeryLongStringRecord");
            }

            this.StringLengths = new Dictionary<string, string>();
            var originalBytes = (from item in ir.Items where item[0] != 0 select item[0]).ToArray();
            var dictionaryString = Encoding.Default.GetString(originalBytes);

            // split on tabs:
            var entries = dictionaryString.Split('\t');

            foreach (var entry in entries)
            {
                var values = entry.Split('=');
                this.StringLengths.Add(values[0], values[1]);
            }

        }
    }
}
