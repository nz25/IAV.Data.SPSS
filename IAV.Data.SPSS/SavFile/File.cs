using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.SavFile
{
    public class File
    {
        // Streams
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
        public Subtype16Record Subtype16Record { get; set; }
        public CodePageRecord CodePageRecord { get; set; }
        public LongValueLabelsRecord LongValueLabelsRecord { get; set; }

        // DataRecords
        public IEnumerable<DataRecord> DataRecords { get; set; }

        public File()
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

            //this.Reader.Dispose();
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
                    case RecordSubType.Subtype16Record:
                        this.Subtype16Record = new Subtype16Record(this);
                        this.Subtype16Record.ReadFromInfoRecord(infoRecord);
                        break;
                    case RecordSubType.CodePageRecord:
                        this.CodePageRecord = new CodePageRecord(this);
                        this.CodePageRecord.ReadFromInfoRecord(infoRecord);
                        break;
                    case RecordSubType.LongValueLabelsRecord:
                        this.LongValueLabelsRecord = new LongValueLabelsRecord(this);
                        this.LongValueLabelsRecord.ReadFromInfoRecord(infoRecord);
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

                byte[] compressionCodes = this.Reader.ReadBytes(8);

                for (int i = 0; i < compressionCodes.Length; i++)
                {
                    switch ((CompressionCode)compressionCodes[i])
                    {
                        case CompressionCode.Ignored:
                            break;
                        case CompressionCode.EndOfFile:
                            break;
                        case CompressionCode.NotCompressibleValue:
                            byte[] notCompressibleValue = this.Reader.ReadBytes(8);
                            dr.Values[variableIndex] = notCompressibleValue;
                            variableIndex++;
                            break;
                        case CompressionCode.EightSpacesString:                      
                            dr.Values[variableIndex] = eightSpacesBytes;
                            variableIndex++;
                            break;
                        case CompressionCode.SystemMissingValue:
                            dr.Values[variableIndex] = systemMissingBytes;
                            variableIndex++;
                            break;
                        default: // 1 - 251
                            byte[] numericValueBytes = BitConverter.GetBytes((double)(compressionCodes[i] - this.FileHeaderRecord.Bias));
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

            //this.Writer.Dispose();
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
            List<byte> compressionCodes = new List<byte>();
            List<byte[]> notCompressibleValues = new List<byte[]>();

            foreach (DataRecord dr in this.DataRecords)
            {
                for (int i = 0; i < variableCount; i++)
                {
                    double numericValue = BitConverter.ToDouble(dr.Values[i], 0) + this.FileHeaderRecord.Bias;

                    if (dr.Values[i].SequenceEqual(eightSpacesBytes))
                    {
                        compressionCodes.Add(254);
                    }
                    else if (dr.Values[i].SequenceEqual(systemMissingBytes))
                    {
                        compressionCodes.Add(255);
                    }
                    else if (this.VariableRecords[i].Type == VariableType.Numeric && 1 <= numericValue && numericValue <= 251)
                    {
                        compressionCodes.Add((byte)numericValue);
                    }
                    else // Value is not compressible
                    {
                        compressionCodes.Add(253);
                        notCompressibleValues.Add(dr.Values[i]);
                    }

                    if (compressionCodes.Count == 8)
                    {
                        this.Writer.Write(compressionCodes.ToArray());
                        foreach (byte[] value in notCompressibleValues)
                        {
                            this.Writer.Write(value);
                        }

                        compressionCodes.Clear();
                        notCompressibleValues.Clear();
                    }
                }
            }

            if (compressionCodes.Count > 0)
            {
                // pad bytes with zeros to get to 8 bytes block
                for (int i = compressionCodes.Count; i < 8; i++)
                {
                    compressionCodes.Add((Int32)CompressionCode.Ignored);
                }
                this.Writer.Write(compressionCodes.ToArray());
                foreach (byte[] value in notCompressibleValues)
                {
                    this.Writer.Write(value);
                }
                compressionCodes.Clear();
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
