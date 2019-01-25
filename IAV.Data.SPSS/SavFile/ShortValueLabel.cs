using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.SavFile
{
    public class ShortValueLabel
    {
        private byte[] _byteArrayValue;
        public byte LabelLength { get; set; }
        public string Label { get; set; }

        public byte[] ByteArrayValue
        {
            get { return _byteArrayValue; }
            set { _byteArrayValue = value; }
        }

        public string StringValue
        {
            get { return Encoding.Default.GetString(_byteArrayValue); }
        }

        public double NumericValue
        {
            get { return BitConverter.ToDouble(_byteArrayValue, 0); }
        }
    }
}
