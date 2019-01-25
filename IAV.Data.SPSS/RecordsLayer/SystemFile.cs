using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.RecordsLayer
{
    public class SystemFile
    {
        public FileHeaderRecord FileHeaderRecord { get; set; }
        public List<VariableRecord> VariableRecords { get; set; }
        public List<ValueLabelsRecord> ValueLabelsRecords { get; set; }
        public DocumentRecord DocumentRecord { get; set; }
        public List<InfoRecord> InfoRecords { get; set; }

        //InfoRecords
        public MachineIntegerInfoRecord MachineIntegerInfoRecord { get; set; }
        public MachineFloatingPointInfoRecord MachineFloatingPointInfoRecord { get; set; }
        public VariableDisplayParameterRecord VariableDisplayParameterRecord { get; set; }
        public LongVariableNamesRecord LongVariableNamesRecord { get; set; }
        public VeryLongStringRecord VeryLongStringRecord { get; set; }
        public MultipleResponseSetsRecord MultipleResponseSetsRecord { get; set; }

        //Case data
        public long DataStartPosition { get; set; }

        public SystemFile()
        {
            this.FileHeaderRecord = new FileHeaderRecord(this);
            this.VariableRecords = new List<VariableRecord>();
            this.ValueLabelsRecords = new List<ValueLabelsRecord>();
            this.DocumentRecord = new DocumentRecord(this);
            this.InfoRecords = new List<InfoRecord>();
        }

        public int GetVariableCount()
        {
            // Ignores variable extentions
            return (from record in this.VariableRecords
                    where record.Type != -1
                    select record).Count();
        }

    }
}
