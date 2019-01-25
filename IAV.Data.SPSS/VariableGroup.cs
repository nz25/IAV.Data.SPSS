using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAV.Data.SPSS.Enum;

namespace IAV.Data.SPSS
{
    public class VariableGroup
    {
        public Dataset Dataset { get; set; }
        public VariableGroupType VariableGroupType { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public int Value { get; set; }
        public List<Variable> Variables { get; set; }

        public VariableGroup(Dataset ds)
        {
            this.Dataset = ds;
            this.Variables = new List<Variable>();
        }
    }
}
