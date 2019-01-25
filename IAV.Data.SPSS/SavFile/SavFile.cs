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
            this.DataRecords = this.GetDataRecords();
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

        private IEnumerable<DataRecord> GetDataRecords()
        {
            // Filler record
            this.Reader.ReadInt32();

            //int caseNumber = 0;
            byte[] leftOver = new byte[0];

            // Loops through case data
            while (this.Reader.BaseStream.Position != this.Reader.BaseStream.Length)
            {
                //caseNumber++;
                DataRecord dr = new DataRecord(this);
                dr.ReadFromStream(leftOver);// leftover bytes from the previous data record are passed to the current one for processing
                leftOver = dr.LeftOver; //leftover bytes from current data record are saved temporarily to be processed by the next data record
                yield return dr;
            }
        }

        public void WriteToStream(FileStream stream)
        {
            this.Writer = new BinaryWriter(stream, Encoding.Default);

            this.WriteDictionary();
            this.WriteDataRecords();
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

        private void WriteDataRecords()
        {

            // Filler record
            this.Writer.Write(0);

            byte[] leftOver = new byte[0];
            foreach (DataRecord dr in this.DataRecords)
            {
                dr.WriteToStream(leftOver);
                leftOver = dr.LeftOver;
            }
        }


    }
}
