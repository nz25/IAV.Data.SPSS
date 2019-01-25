using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAV.Data.SPSS.Enum;

namespace IAV.Data.SPSS.SavFile
{
    public class VeryLongStringRecord
    {
        public Dictionary<string, string> StringLengths { get; set; }

        public File File { get; set; }


        public VeryLongStringRecord(File file)
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
                if (values.Count() == 2)
                {
                    this.StringLengths.Add(values[0], values[1]);
                }
                
            }

        }
    }
}
