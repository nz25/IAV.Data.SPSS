using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.SavFile
{
    public class LongVariableWithValueLabels
    {
        public string LongVariableName { get; set; }
        public int VariableWidth { get; set; }
        public List<LongValueLabel> ValueLabels { get; set; }

        public LongVariableWithValueLabels()
        {
            this.ValueLabels = new List<LongValueLabel>();
        }
    }
}
