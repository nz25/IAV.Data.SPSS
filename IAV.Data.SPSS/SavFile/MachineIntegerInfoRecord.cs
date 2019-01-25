using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.SavFile
{
    public class MachineIntegerInfoRecord
    {
        public int VersionMajor { get; set; }
        public int VersionMinor { get; set; }
        public int VersionRevision { get; set; }
        public int MachineCode { get; set; }
        public FloatingPointRepresentation FloatingPointRepresentation { get; set; }
        public int CompressionCode { get; set; }
        public Endianness Endianness { get; set; }
        public CharacterCode CharacterCode { get; set; }
        
        public File File { get; set; }

        public MachineIntegerInfoRecord(File file)
        {
            this.File = file;
        }

        public void ReadFromInfoRecord(InfoRecord ir)
        {
            if (ir.RecordSubType != RecordSubType.MachineIntegerInfoRecord || ir.ItemSize != 4 || ir.ItemCount != 8)
            {
                throw new Exception("Invalid items in MachineIntegerInfoRecord");
            }
            this.VersionMajor = BitConverter.ToInt32(ir.Items[0], 0);
            this.VersionMinor = BitConverter.ToInt32(ir.Items[1], 0);
            this.VersionRevision = BitConverter.ToInt32(ir.Items[2], 0);
            this.MachineCode = BitConverter.ToInt32(ir.Items[3], 0);
            this.FloatingPointRepresentation = (FloatingPointRepresentation)BitConverter.ToInt32(ir.Items[4], 0);
            this.CompressionCode = BitConverter.ToInt32(ir.Items[5], 0);
            this.Endianness = (Endianness)BitConverter.ToInt32(ir.Items[6], 0);
            this.CharacterCode = (CharacterCode)BitConverter.ToInt32(ir.Items[7], 0);
        }

        public InfoRecord ConvertToInfoRecord()
        {
            InfoRecord ir = new InfoRecord(this.File);
            ir.RecordSubType = RecordSubType.MachineIntegerInfoRecord;
            ir.ItemSize = 4;
            ir.ItemCount = 8;
            ir.Items = new List<byte[]>();
            ir.Items.Add(BitConverter.GetBytes(this.VersionMajor));
            ir.Items.Add(BitConverter.GetBytes(this.VersionMinor));
            ir.Items.Add(BitConverter.GetBytes(this.VersionRevision));
            ir.Items.Add(BitConverter.GetBytes(this.MachineCode));
            ir.Items.Add(BitConverter.GetBytes((Int32)this.FloatingPointRepresentation));
            ir.Items.Add(BitConverter.GetBytes(this.CompressionCode));
            ir.Items.Add(BitConverter.GetBytes((Int32)this.Endianness));
            ir.Items.Add(BitConverter.GetBytes((Int32)this.CharacterCode));

            return ir;

        }
    }
}
