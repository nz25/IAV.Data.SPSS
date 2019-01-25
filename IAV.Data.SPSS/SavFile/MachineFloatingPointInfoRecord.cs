using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.SavFile
{
    public class MachineFloatingPointInfoRecord
    {
        public double SystemMissingValue { get; set; }
        public double HighestMissingValue { get; set; }
        public double LowestMissingValue { get; set; }

        public File File { get; set; }

        public MachineFloatingPointInfoRecord(File file)
        {
            this.File = file;
        }

        public void ReadFromInfoRecord(InfoRecord ir)
        {
            if (ir.RecordSubType != RecordSubType.MachineFloatingPointInfoRecord || ir.ItemSize != 8 || ir.ItemCount != 3)
            {
                throw new Exception("Invalid items in MachineFloatingPointInfoRecord");
            }
            this.SystemMissingValue = BitConverter.ToDouble(ir.Items[0], 0);
            this.HighestMissingValue = BitConverter.ToDouble(ir.Items[1], 0);
            this.LowestMissingValue = BitConverter.ToDouble(ir.Items[2], 0);
        }

        public InfoRecord ConvertToInfoRecord()
        {
            InfoRecord ir = new InfoRecord(this.File);
            ir.RecordSubType = RecordSubType.MachineFloatingPointInfoRecord;
            ir.ItemSize = 8;
            ir.ItemCount = 3;
            ir.Items = new List<byte[]>();
            ir.Items.Add(BitConverter.GetBytes(this.SystemMissingValue));
            ir.Items.Add(BitConverter.GetBytes(this.HighestMissingValue));
            ir.Items.Add(BitConverter.GetBytes(this.LowestMissingValue));

            return ir;

        }
    }
}
