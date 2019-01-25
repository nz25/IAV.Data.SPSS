using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAV.Data.SPSS.Enum;

namespace IAV.Data.SPSS.SavFile
{
    public class VariableParameters
    {
        public MeasurementType MeasurementType { get; set; }
        public int Width { get; set; }
        public Alignment Alignment { get; set; }
    }
}
