using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAV.Data.SPSS.RecordsLayer;

namespace IAV.Data.SPSS
{
    public class Parser
    {
        private FileStream Stream;
        private BinaryReader BinaryReader;
        public SystemFile SystemFile { get; set; }
        public Encoding Encoding { get; set; }

        public Parser(FileStream stream)
        {
            this.Stream = stream;
            this.SystemFile = new SystemFile();
            this.Encoding = Encoding.Default;
                        this.BinaryReader = new BinaryReader(this.Stream, this.Encoding);
        }

        public void ReadFromStream()
        {
            this.ReadDictionary();
            this.ParseInfoRecords();
            this.ReadCaseData();    
        }

     
        private void ReadDictionary()
        {
    
            RecordType currentRecordType = (RecordType)this.BinaryReader.ReadInt32();
            while (currentRecordType != RecordType.DictionaryTerminationRecord)
            {
                switch (currentRecordType)
                {
                    case RecordType.FileHeaderRecord:
                        this.SystemFile.FileHeaderRecord.ReadFromStream(this.BinaryReader);
                        break;
                    case RecordType.VariableRecord:
                        VariableRecord v = new VariableRecord(this.SystemFile);
                        v.ReadFromStream(this.BinaryReader);
                        this.SystemFile.VariableRecords.Add(v);
                        break;
                    case RecordType.ValueLabelRecord:
                        ValueLabelsRecord vl = new ValueLabelsRecord(this.SystemFile);
                        vl.ReadFromStream(this.BinaryReader);
                        this.SystemFile.ValueLabelsRecords.Add(vl);
                        break;
                    case RecordType.DocumentRecord:
                        this.SystemFile.DocumentRecord.ReadFromStream(this.BinaryReader);
                        break;
                    case RecordType.InfoRecord:
                        InfoRecord ir = new InfoRecord(this.SystemFile);
                        ir.ReadFromStream(this.BinaryReader);
                        this.SystemFile.InfoRecords.Add(ir);
                        break;
                    default:
                        break;
                }

                currentRecordType = (RecordType)this.BinaryReader.ReadInt32();

                this.ParseInfoRecords();

            }
        }

        private void ParseInfoRecords()
        {
            foreach (var infoRecord in this.SystemFile.InfoRecords)
            {
                switch (infoRecord.RecordSubType)
                {
                    case RecordSubType.MachineIntegerInfoRecord:
                        this.SystemFile.MachineIntegerInfoRecord = new MachineIntegerInfoRecord(this.SystemFile);
                        this.SystemFile.MachineIntegerInfoRecord.ReadFromInfoRecord(infoRecord);
                        break;
                    case RecordSubType.MachineFloatingPointInfoRecord:
                        this.SystemFile.MachineFloatingPointInfoRecord = new MachineFloatingPointInfoRecord(this.SystemFile);
                        this.SystemFile.MachineFloatingPointInfoRecord.ReadFromInfoRecord(infoRecord);
                        break;
                    case RecordSubType.VariableDisplayParameterRecord:
                        this.SystemFile.VariableDisplayParameterRecord = new VariableDisplayParameterRecord(this.SystemFile);
                        this.SystemFile.VariableDisplayParameterRecord.ReadFromInfoRecord(infoRecord);
                        break;
                    case RecordSubType.LongVariableNamesRecord:
                        this.SystemFile.LongVariableNamesRecord = new LongVariableNamesRecord(this.SystemFile);
                        this.SystemFile.LongVariableNamesRecord.ReadFromInfoRecord(infoRecord);
                        break;
                    case RecordSubType.VeryLongStringRecord:
                        this.SystemFile.VeryLongStringRecord = new VeryLongStringRecord(this.SystemFile);
                        this.SystemFile.VeryLongStringRecord.ReadFromInfoRecord(infoRecord);
                        break;
                    case RecordSubType.MultipleResponseSetsRecord:
                        this.SystemFile.MultipleResponseSetsRecord = new MultipleResponseSetsRecord(this.SystemFile);
                        this.SystemFile.MultipleResponseSetsRecord.ReadFromInfoRecord(infoRecord);
                        break;
                    default:
                        break;
                }
                
            }
        }

        private void ReadCaseData()
        {
 	        
            //filler record
            this.BinaryReader.ReadInt32();
            this.SystemFile.DataStartPosition = this.Stream.Position;

            int caseNumber = 0;
            byte[] leftOver = new byte[0];

            //Loops through case data
            while (this.BinaryReader.BaseStream.Position != this.BinaryReader.BaseStream.Length)
            {
                caseNumber++;
                if (caseNumber == 24662)
                {
                    
                }
                DataRecord dr = new DataRecord(this.SystemFile);
                dr.ReadFromStream(this.BinaryReader, leftOver); // leftover bytes from the previous data record are passed to the current one for processing
                leftOver = dr.LeftOver; // leftover bytes from the current data record are stored temporarily to be passed to the next data record 
            }
        }
    }
}
