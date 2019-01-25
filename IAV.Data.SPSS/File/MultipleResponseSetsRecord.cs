using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.File
{
    public class MultipleResponseSetsRecord
    {
        public List<MultipleResponseSet> MultipleResponseSets { get; set; }
        public SavFile File { get; set; }

        public MultipleResponseSetsRecord(SavFile file)
        {
            this.File = file;
        }

        public void ReadFromInfoRecord(InfoRecord ir)
        {
            if (ir.RecordSubType != RecordSubType.MultipleResponseSetsRecord || ir.ItemSize != 1)
            {
                throw new Exception("Invalid items in MultipleResponseSetsRecord");
            }

            var originalBytes = (from item in ir.Items select item[0]).ToArray();
            var dictionaryString = Encoding.Default.GetString(originalBytes); // TO DO: Globalize Encoding??
            // split on tabs:
            var entries = dictionaryString.Split('\n');
            this.MultipleResponseSets = new List<MultipleResponseSet>();
            foreach (var entry in entries)
            {
                if (entry.Length > 0)
                {
                    MultipleResponseSet mrs = new MultipleResponseSet();
                    mrs.ReadFromString(entry);
                    this.MultipleResponseSets.Add(mrs);
                }
            }

        }
    }
}
