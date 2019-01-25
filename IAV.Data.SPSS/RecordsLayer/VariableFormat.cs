using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.RecordsLayer
{
    public class VariableFormat
    {
        public int NumberOfDecimalPlaces { get; set; }
        public int FieldWidth { get; set; }
        public VariableFormatType FormatType { get; set; }

        public VariableFormat(int value)
        {
            ReadFromInt(value);
        }

        public void ReadFromInt(int value)
        {
            byte[] formatBytes = BitConverter.GetBytes(value);
            this.NumberOfDecimalPlaces = (int)formatBytes[0];
            this.FieldWidth = (int)formatBytes[1];
            this.FormatType = (VariableFormatType)formatBytes[2];
        }
        
    }
}
