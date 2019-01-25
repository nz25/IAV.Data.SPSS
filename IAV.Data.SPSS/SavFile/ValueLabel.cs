using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.SavFile
{
    public class ValueLabel
    {
        public byte[] ByteArrayValue { get; set; }
        public byte LabelLength { get; set; }
        public string Label { get; set; }
    }
}
