using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAV.Data.SPSS.Enum;

namespace IAV.Data.SPSS.SavFile
{
    public class VariableFormat
    {
        public int NumberOfDecimalPlaces { get; set; }
        public int FieldWidth { get; set; }
        public VariableFormatType FormatType { get; set; }

        public void ReadFromInt(int value)
        {
            byte[] formatBytes = BitConverter.GetBytes(value);
            this.NumberOfDecimalPlaces = (int)formatBytes[0];
            this.FieldWidth = (int)formatBytes[1];
            this.FormatType = (VariableFormatType)formatBytes[2];
        }

        public int WriteToInt()
        {
            byte[] formatBytes = new byte[4];
            formatBytes[0] = (byte)this.NumberOfDecimalPlaces;
            formatBytes[1] = (byte)this.FieldWidth;
            formatBytes[2] = (byte)this.FormatType;
            //formatBytes[3] is not used and set to 0

            return BitConverter.ToInt32(formatBytes, 0);
        }

        
    }
}
