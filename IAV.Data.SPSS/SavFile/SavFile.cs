using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.SavFile
{
    public class SavFile
    {
        // Stream
        public BinaryReader Reader { get; set; }
        public BinaryWriter Writer { get; set; }

        // Records
        public FileHeaderRecord FileHeaderRecord { get; set; }
        public List<VariableRecord> VariableRecords { get; set; }
        public List<ValueLabelsRecord> ValueLabelsRecords { get; set; }
        public DocumentRecord DocumentRecord { get; set; }
        public List<InfoRecord> InfoRecords { get; set; }

        // InfoRecords
        public MachineIntegerInfoRecord MachineIntegerInfoRecord { get; set; }
        public MachineFloatingPointInfoRecord MachineFloatingPointInfoRecord { get; set; }
        public VariableDisplayParameterRecord VariableDisplayParameterRecord { get; set; }
        public LongVariableNamesRecord LongVariableNamesRecord { get; set; }
        public VeryLongStringRecord VeryLongStringRecord { get; set; }
        public MultipleResponseSetsRecord MultipleResponseSetsRecord { get; set; }

        // DataRecords
        public IEnumerable<DataRecord> DataRecords { get; set; }

        public SavFile()
        {
            this.VariableRecords = new List<VariableRecord>();
            this.ValueLabelsRecords = new List<ValueLabelsRecord>();
            this.InfoRecords = new List<InfoRecord>();
        }

        public void ReadFromStream(FileStream stream)
        {
            this.Reader = new BinaryReader(stream, Encoding.Default);
            this.ReadDictionary();
            this.ParseInfoRecords();

            //Read DataRecords
            this.Reader.ReadInt32(); // Filler record
            if (this.FileHeaderRecord.Compressed == 1)
            {
                this.DataRecords = this.ReadCompressedDataRecords();            
            }
            else
            {
                this.DataRecords = this.ReadUncompressedDataRecords();
            }

            this.Reader.Dispose();
        }

        private void ReadDictionary()
        {
            RecordType currentRecordType = (RecordType)this.Reader.ReadInt32();
            while (currentRecordType != RecordType.DictionaryTerminationRecord)
            {
                switch (currentRecordType)
                {
                    case RecordType.FileHeaderRecord:
                        this.FileHeaderRecord = new FileHeaderRecord(this);
                        this.FileHeaderRecord.ReadFromStream();
                        break;
                    case RecordType.VariableRecord:
                        VariableRecord v = new VariableRecord(this);
                        v.ReadFromStream();
                        this.VariableRecords.Add(v);
                        break;
                    case RecordType.ValueLabelRecord:
                        ValueLabelsRecord vl = new ValueLabelsRecord(this);
                        vl.ReadFromStream();
                        this.ValueLabelsRecords.Add(vl);
                        // ValueLabelVariablesRecord always follows ValueLabelsRecord, that's why it is stored inside of it.
                        break;
                    case RecordType.DocumentRecord:
                        this.DocumentRecord = new DocumentRecord(this);
                        this.DocumentRecord.ReadFromStream();
                        break;
                    case RecordType.InfoRecord:
                        InfoRecord ir = new InfoRecord(this);
                        ir.ReadFromStream();
                        this.InfoRecords.Add(ir);
                        break;
                    default:
                        break;
                }

                currentRecordType = (RecordType)this.Reader.ReadInt32();
            }
        }

        private void ParseInfoRecords()
        {
            foreach (var infoRecord in this.InfoRecords)
            {
                switch (infoRecord.RecordSubType)
                {
                    case RecordSubType.MachineIntegerInfoRecord:
                        this.MachineIntegerInfoRecord = new MachineIntegerInfoRecord(this);
                        this.MachineIntegerInfoRecord.ReadFromInfoRecord(infoRecord);
                        break;
                    case RecordSubType.MachineFloatingPointInfoRecord:
                        this.MachineFloatingPointInfoRecord = new MachineFloatingPointInfoRecord(this);
                        this.MachineFloatingPointInfoRecord.ReadFromInfoRecord(infoRecord);
                        break;
                    case RecordSubType.VariableDisplayParameterRecord:
                        this.VariableDisplayParameterRecord = new VariableDisplayParameterRecord(this);
                        this.VariableDisplayParameterRecord.ReadFromInfoRecord(infoRecord);
                        break;
                    case RecordSubType.LongVariableNamesRecord:
                        this.LongVariableNamesRecord = new LongVariableNamesRecord(this);
                        this.LongVariableNamesRecord.ReadFromInfoRecord(infoRecord);
                        break;
                    case RecordSubType.VeryLongStringRecord:
                        this.VeryLongStringRecord = new VeryLongStringRecord(this);
                        this.VeryLongStringRecord.ReadFromInfoRecord(infoRecord);
                        break;
                    case RecordSubType.MultipleResponseSetsRecord:
                        this.MultipleResponseSetsRecord = new MultipleResponseSetsRecord(this);
                        this.MultipleResponseSetsRecord.ReadFromInfoRecord(infoRecord);
                        break;
                    default:
                        break;
                }
            }
        }

        private IEnumerable<DataRecord> ReadCompressedDataRecords()
        {
          
            int variableIndex = 0;
            int variableCount = this.VariableRecords.Count;
            byte[] eightSpacesBytes = Encoding.Default.GetBytes("        ");
            byte[] systemMissingBytes = BitConverter.GetBytes(this.MachineFloatingPointInfoRecord.SystemMissingValue);

            DataRecord dr = new DataRecord(this);

            while (this.Reader.BaseStream.Position != this.Reader.BaseStream.Length)
            {

                byte[] commandBytes = this.Reader.ReadBytes(8);

                for (int i = 0; i < commandBytes.Length; i++)
                {
                    switch ((CommandByte)commandBytes[i])
                    {
                        case CommandByte.Ignored:
                            break;
                        case CommandByte.EndOfFile:
                            break;
                        case CommandByte.NotCompressibleValue:
                            byte[] notCompressibleValue = this.Reader.ReadBytes(8);
                            dr.Values[variableIndex] = notCompressibleValue;
                            variableIndex++;
                            break;
                        case CommandByte.EightSpacesString:                      
                            dr.Values[variableIndex] = eightSpacesBytes;
                            variableIndex++;
                            break;
                        case CommandByte.SystemMissingValue:
                            dr.Values[variableIndex] = systemMissingBytes;
                            variableIndex++;
                            break;
                        default: // 1 - 251
                            byte[] numericValueBytes = BitConverter.GetBytes((double)(commandBytes[i] - this.FileHeaderRecord.Bias));
                            dr.Values[variableIndex] = numericValueBytes;
                            variableIndex++;
                            break;
                    }

                    if (variableIndex == variableCount)
                    {
                        yield return dr;
                        variableIndex = 0;
                        dr = new DataRecord(this);
                    }
                }                    
            }
        }

        private IEnumerable<DataRecord> ReadUncompressedDataRecords()
        {

            int variableCount = this.VariableRecords.Count;

            while (this.Reader.BaseStream.Position != this.Reader.BaseStream.Length)
            {
                // in uncompressed file all values are stored in 8 byte chunks
                DataRecord dr = new DataRecord(this);
                for (int variableIndex = 0; variableIndex < variableCount; variableIndex++)
                {
                    byte[] value = this.Reader.ReadBytes(8);
                    dr.Values[variableIndex] = value;
                }
                yield return dr;
            }
        }

        public void WriteToStream(FileStream stream)
        {
            this.Writer = new BinaryWriter(stream, Encoding.Default);
            this.WriteDictionary();

            // Data records
            this.Writer.Write(0);   // Filler record
            if (this.FileHeaderRecord.Compressed == 1)
            {
                this.WriteCompressedDataRecords();
            }
            else
            {
                this.WriteUncompressedDataRecords();
            }

            this.Writer.Dispose();
        }

        private void WriteDictionary()
        {
            this.FileHeaderRecord.WriteToStream();
            foreach (VariableRecord vr in this.VariableRecords)
            {
                vr.WriteToStream();
            }
            foreach (ValueLabelsRecord vl in this.ValueLabelsRecords)
            {
                vl.WriteToStream();
            }
            if (this.DocumentRecord != null)
            {
                this.DocumentRecord.WriteToStream();
            }
            foreach (InfoRecord ir in this.InfoRecords)
            {
                ir.WriteToStream();
            }

            this.Writer.Write((Int32)RecordType.DictionaryTerminationRecord);

        }

        private void WriteCompressedDataRecords()
        {
            int variableCount = this.VariableRecords.Count;
            byte[] eightSpacesBytes = Encoding.Default.GetBytes("        ");
            byte[] systemMissingBytes = BitConverter.GetBytes(this.MachineFloatingPointInfoRecord.SystemMissingValue);
            List<byte> commandBytes = new List<byte>();
            List<byte[]> notCompressibleValues = new List<byte[]>();

            foreach (DataRecord dr in this.DataRecords)
            {
                for (int i = 0; i < variableCount; i++)
                {
                    double numericValue = BitConverter.ToDouble(dr.Values[i], 0) + this.FileHeaderRecord.Bias;

                    if (dr.Values[i].SequenceEqual(eightSpacesBytes))
                    {
                        commandBytes.Add(254);
                    }
                    else if (dr.Values[i].SequenceEqual(systemMissingBytes))
                    {
                        commandBytes.Add(255);
                    }
                    else if (this.VariableRecords[i].Type == VariableType.Numeric && 1 <= numericValue && numericValue <= 251)
                    {
                        commandBytes.Add((byte)numericValue);
                    }
                    else // Value is not compressible
                    {
                        commandBytes.Add(253);
                        notCompressibleValues.Add(dr.Values[i]);
                    }

                    if (commandBytes.Count == 8)
                    {
                        this.Writer.Write(commandBytes.ToArray());
                        foreach (byte[] value in notCompressibleValues)
                        {
                            this.Writer.Write(value);
                        }

                        commandBytes.Clear();
                        notCompressibleValues.Clear();
                    }
                }
            }

            if (commandBytes.Count > 0)
            {
                // pad bytes with 0s to get to 8 bytes block
                for (int i = commandBytes.Count; i < 8; i++)
                {
                    commandBytes.Add(0);
                }
                this.Writer.Write(commandBytes.ToArray());
                foreach (byte[] value in notCompressibleValues)
                {
                    this.Writer.Write(value);
                }
                commandBytes.Clear();
                notCompressibleValues.Clear();
            }
        }

        private void WriteUncompressedDataRecords()
        {
            foreach (DataRecord dr in this.DataRecords)
            {
                foreach (byte[] value in dr.Values)
                {
                    this.Writer.Write(value);
                }
            }
        }
    }
}
