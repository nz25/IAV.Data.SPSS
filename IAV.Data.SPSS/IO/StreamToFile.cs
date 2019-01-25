using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAV.Data.SPSS.File;

namespace IAV.Data.SPSS.IO
{
    public class StreamToFile
    {
        private FileStream Stream;
        private BinaryReader BinaryReader;
        public SavFile SavFile { get; set; }

        public StreamToFile(FileStream stream)
        {
            this.Stream = stream;
            this.SavFile = new SavFile();
            this.BinaryReader = new BinaryReader(this.Stream, Encoding.Default);
        }

        public void ReadFromStream()
        {
            this.ReadDictionary();
            this.ParseInfoRecords();
            this.SavFile.DataRecords = this.GetDataRecords();    
        }

        private void ReadDictionary()
        {
            RecordType currentRecordType = (RecordType)this.BinaryReader.ReadInt32();
            while (currentRecordType != RecordType.DictionaryTerminationRecord)
            {
                switch (currentRecordType)
                {
                    case RecordType.FileHeaderRecord:
                        this.SavFile.FileHeaderRecord.ReadFromStream(this.BinaryReader);
                        break;
                    case RecordType.VariableRecord:
                        VariableRecord v = new VariableRecord(this.SavFile);
                        v.ReadFromStream(this.BinaryReader);
                        this.SavFile.VariableRecords.Add(v);
                        break;
                    case RecordType.ValueLabelRecord:
                        ValueLabelsRecord vl = new ValueLabelsRecord(this.SavFile);
                        vl.ReadFromStream(this.BinaryReader);
                        this.SavFile.ValueLabelsRecords.Add(vl);
                        break;
                    case RecordType.DocumentRecord:
                        this.SavFile.DocumentRecord.ReadFromStream(this.BinaryReader);
                        break;
                    case RecordType.InfoRecord:
                        InfoRecord ir = new InfoRecord(this.SavFile);
                        ir.ReadFromStream(this.BinaryReader);
                        this.SavFile.InfoRecords.Add(ir);
                        break;
                    default:
                        break;
                }

                currentRecordType = (RecordType)this.BinaryReader.ReadInt32();
            }
        }

        private void ParseInfoRecords()
        {
            foreach (var infoRecord in this.SavFile.InfoRecords)
            {
                switch (infoRecord.RecordSubType)
                {
                    case RecordSubType.MachineIntegerInfoRecord:
                        this.SavFile.MachineIntegerInfoRecord = new MachineIntegerInfoRecord(this.SavFile);
                        this.SavFile.MachineIntegerInfoRecord.ReadFromInfoRecord(infoRecord);
                        break;
                    case RecordSubType.MachineFloatingPointInfoRecord:
                        this.SavFile.MachineFloatingPointInfoRecord = new MachineFloatingPointInfoRecord(this.SavFile);
                        this.SavFile.MachineFloatingPointInfoRecord.ReadFromInfoRecord(infoRecord);
                        break;
                    case RecordSubType.VariableDisplayParameterRecord:
                        this.SavFile.VariableDisplayParameterRecord = new VariableDisplayParameterRecord(this.SavFile);
                        this.SavFile.VariableDisplayParameterRecord.ReadFromInfoRecord(infoRecord);
                        break;
                    case RecordSubType.LongVariableNamesRecord:
                        this.SavFile.LongVariableNamesRecord = new LongVariableNamesRecord(this.SavFile);
                        this.SavFile.LongVariableNamesRecord.ReadFromInfoRecord(infoRecord);
                        break;
                    case RecordSubType.VeryLongStringRecord:
                        this.SavFile.VeryLongStringRecord = new VeryLongStringRecord(this.SavFile);
                        this.SavFile.VeryLongStringRecord.ReadFromInfoRecord(infoRecord);
                        break;
                    case RecordSubType.MultipleResponseSetsRecord:
                        this.SavFile.MultipleResponseSetsRecord = new MultipleResponseSetsRecord(this.SavFile);
                        this.SavFile.MultipleResponseSetsRecord.ReadFromInfoRecord(infoRecord);
                        break;
                    default:
                        break;
                }
                
            }
        }

        private IEnumerable<DataRecord> GetDataRecords()
        {
            //filler record
            this.BinaryReader.ReadInt32();

            //this.SystemFile.DataStartPosition = this.Stream.Position;

            int caseNumber = 0;
            byte[] leftOver = new byte[0];

            //Loops through case data
            while (this.BinaryReader.BaseStream.Position != this.BinaryReader.BaseStream.Length)
            {
                caseNumber++;
                DataRecord dr = this.ReadNextDataRecord(leftOver); // leftover bytes from the previous data record are passed to the current one for processing
                leftOver = dr.LeftOver; //leftover bytes from current data record are saved temporarily to be processed by the next data record
                yield return dr;
            }
        }

        private DataRecord ReadNextDataRecord(byte[] leftOver)
        {
            DataRecord dr = new DataRecord(this.SavFile);
            dr.ReadFromStream(this.BinaryReader, leftOver); 
            return dr;
        }
    }
}
