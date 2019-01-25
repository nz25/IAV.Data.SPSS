using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAV.Data.SPSS.SavFile;
using IAV.Data.SPSS.Enum;
using System.Globalization;

namespace IAV.Data.SPSS
{
    public class Dataset
    {
        public File SavFile { get; set; }
        public string ProductName { get; set; }
        public int LayoutCode { get; set; }
        public int NominalCaseSize { get; set; }
        public bool IsCompressed { get; set; }
        public bool WeightVariableExists { get; set; }
        public int WeightVariableIndex { get; set; }
        public bool CaseCountKnown { get; set; }
        public int CaseCount { get; set; }
        public double Bias { get; set; }
        public DateTime CreatedOn { get; set; }
        public string FileLabel { get; set; }
        public string Document { get; set; }
        public int VersionMajor { get; set; }
        public int VersionMinor { get; set; }
        public int VersionRevision { get; set; }
        public int MachineCode { get; set; }
        public FloatingPointRepresentation FloatingPointRepresentation { get; set; }
        public int CompressionCode { get; set; }
        public Endianness Endianness { get; set; }
        public CharacterCode CharacterCode { get; set; }
        public double SystemMissingValue { get; set; }
        public double HighestMissingValue { get; set; }
        public double LowestMissingValue { get; set; }

        public List<Variable> Variables { get; set; }
        public List<VariableGroup> VariableGroups { get; set; }

        public Dictionary<int, int> VariableIndexByRecordIndex { get; set; }
        public Dictionary<int, int> RecordIndexByVariableIndex { get; set; }
        public Dictionary<string, int> VariableIndexByShortName { get; set; }

        public Dataset()
        {
            this.Variables = new List<Variable>();
            this.VariableGroups = new List<VariableGroup>();

            this.VariableIndexByRecordIndex = new Dictionary<int, int>();
            this.RecordIndexByVariableIndex = new Dictionary<int, int>();
            this.VariableIndexByShortName = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        }

        public void ReadFromSavFile(File savFile)
        {
            this.SavFile = savFile;
            this.ParseFileHeaderRecord();
            this.ParseVariableRecords();
            this.ParseValueLabelRecords();
            this.ParseDocumentRecord();
            if (savFile.MachineIntegerInfoRecord != null) this.ParseMachineIntegerInfoRecord();
            if (savFile.MachineFloatingPointInfoRecord != null) this.ParseMachineFloatingPointInfoRecord();
            if (savFile.VariableDisplayParameterRecord != null) this.ParseVariableDisplayParameterRecord();
            if (savFile.LongVariableNamesRecord != null) this.ParseLongVariableNamesRecord();
            if (savFile.VeryLongStringRecord != null) this.ParseVeryLongStringRecord();
            if (savFile.MultipleResponseSetsRecord != null) this.ParseMultipleResponseSetsRecord();
        }

        private void ParseFileHeaderRecord()
        {
            SavFile.FileHeaderRecord fh = this.SavFile.FileHeaderRecord;
            this.ProductName = fh.ProductName.Trim();
            this.LayoutCode = fh.LayoutCode;
            this.NominalCaseSize = fh.NominalCaseSize;
            this.IsCompressed = (fh.Compressed == 1) ? true : false;
            this.WeightVariableExists = (fh.WeightIndex > 0) ? true : false;
            this.WeightVariableIndex = fh.WeightIndex;
            this.CaseCountKnown = (fh.CaseCount > 0) ? true : false;
            this.CaseCount = fh.CaseCount;
            this.Bias = fh.Bias;
            this.CreatedOn = this.GetDateTimeFromDateAndTimeStrings(fh.CreationDateString, fh.CreationTimeString);
            this.FileLabel = fh.FileLabel.Trim();    
        }

        private void ParseVariableRecords()
        {
            // Dataset.Variables collection will contain fewer Variables, because all string extension VariableRecords will be removed

            int variableIndex = 0;
            for (int recordIndex = 0; recordIndex < this.SavFile.VariableRecords.Count; recordIndex++)
            {
                VariableRecord vr = this.SavFile.VariableRecords[recordIndex];
                if (vr.Type != VariableType.StringExtention) // for all numeric variables and string variables
                {
                    Variable v = new Variable(this);
                    v.VariableIndex = variableIndex + 1;
                    v.ReadFromVariableRecord(vr, this.SavFile.VariableRecords.IndexOf(vr));
                    this.Variables.Add(v);
                    this.RecordIndexByVariableIndex.Add(variableIndex , recordIndex);
                    this.VariableIndexByShortName.Add(vr.Name.Trim(), variableIndex);

                    variableIndex += 1;
                    
                }
                else // for string extensions variable records increments the VariableRecordLength field for the last added variable
                {
                    this.Variables[variableIndex - 1].VariableRecordLength += 1;
                }
                this.VariableIndexByRecordIndex.Add(recordIndex, variableIndex - 1);
                
                
            }
        }

        private void ParseValueLabelRecords()
        {
            foreach (ValueLabelsRecord vlr in this.SavFile.ValueLabelsRecords)
            {
                // Determines data type of the first variable in ValueLabelVariablesRecord
                // to be able to cast the value in ValueLabel
                ValueLabelVariablesRecord vlvr = vlr.ValueLabelVariablesRecord;
                int variableIndex = 0;
                this.VariableIndexByRecordIndex.TryGetValue(vlvr.VariableIndices[0] - 1, out variableIndex);
                DataType dt = this.Variables[variableIndex].DataType;

                List<ValueLabel> ValueLabels = new List<ValueLabel>();
                foreach (ShortValueLabel rvl in vlr.ValueLabels)
                {
                    ValueLabel vl = new ValueLabel();
                    vl.Value = (dt == DataType.Numeric) ? (object)rvl.NumericValue : (object)rvl.StringValue;
                    vl.Label = rvl.Label.Trim();
                    ValueLabels.Add(vl);
                }
                
                foreach (int recordIndex in vlvr.VariableIndices)
                {
                    this.VariableIndexByRecordIndex.TryGetValue(recordIndex - 1 , out variableIndex);
                    this.Variables[variableIndex].ValueLabels = ValueLabels;
                }   
            }
        }

        private void ParseDocumentRecord()
        {
            SavFile.DocumentRecord dr = this.SavFile.DocumentRecord;
            if (dr != null)
            {
                foreach (string line in dr.Lines)
                {
                    this.Document += line;
                }
            }
        }

        private void ParseMachineIntegerInfoRecord()
        {
            SavFile.MachineIntegerInfoRecord miir = this.SavFile.MachineIntegerInfoRecord;
            this.VersionMajor = miir.VersionMajor;
            this.VersionMinor = miir.VersionMinor;
            this.VersionRevision = miir.VersionRevision;
            this.MachineCode = miir.MachineCode;
            this.FloatingPointRepresentation = miir.FloatingPointRepresentation;
            this.CompressionCode = miir.CompressionCode;
            this.Endianness = miir.Endianness;
            this.CharacterCode = miir.CharacterCode;
        }

        private void ParseMachineFloatingPointInfoRecord()
        {
            SavFile.MachineFloatingPointInfoRecord mfpir = this.SavFile.MachineFloatingPointInfoRecord;
            this.SystemMissingValue = mfpir.SystemMissingValue;
            this.HighestMissingValue = mfpir.HighestMissingValue;
            this.LowestMissingValue = mfpir.LowestMissingValue;
        }

        private void ParseVariableDisplayParameterRecord()
        {
            SavFile.VariableDisplayParameterRecord vdpr = this.SavFile.VariableDisplayParameterRecord;
            for (int i = 0; i < vdpr.VariableParameters.Count; i++)
            {
                this.Variables[i].ReadFromVariableParameters(vdpr.VariableParameters[i]);
            }
        }

        private void ParseLongVariableNamesRecord()
        {
            SavFile.LongVariableNamesRecord lvnr = this.SavFile.LongVariableNamesRecord;
            foreach (var item in lvnr.LongNamebyShortName)
            {
                int variableIndex = 0; 
                //int recordIndex = 0;
                this.VariableIndexByShortName.TryGetValue(item.Key.Trim(), out variableIndex);

                //int variableIndex = 0;
                //this.VariableIndexByRecordIndex.TryGetValue(recordIndex, out variableIndex);

                this.Variables[variableIndex].Name = item.Value;
            }
        }

        private void ParseVeryLongStringRecord()
        {
            VeryLongStringRecord vlsr = this.SavFile.VeryLongStringRecord;

            foreach (var item in vlsr.StringLengths)
            {
                int variableIndex = 0;
                //int recordIndex = 0;
                this.VariableIndexByShortName.TryGetValue(item.Key.Trim(), out variableIndex);
                this.Variables[variableIndex].VeryLongStringWidth = Int32.Parse(item.Value.Trim());
            }
        }

        private void ParseMultipleResponseSetsRecord()
        {
            MultipleResponseSetsRecord mrsr = this.SavFile.MultipleResponseSetsRecord;

            foreach (var item in mrsr.MultipleResponseSets)
            {
                VariableGroup vg = new VariableGroup(this) { Name = item.Name, Label = item.Label, Value = item.Value };
                vg.VariableGroupType = item.Type.StartsWith("D") ? VariableGroupType.MultipleDichotomy : VariableGroupType.MultipleCategory;
                foreach (var varName in item.Variables)
                {
                    int variableIndex = 0;
                    if (this.VariableIndexByShortName.TryGetValue(varName, out variableIndex))
                    {
                        vg.Variables.Add(this.Variables[variableIndex]);
                    }
                }
                this.VariableGroups.Add(vg);
            }
        }

        private DateTime GetDateTimeFromDateAndTimeStrings(string dateString, string timeString)
        {
            // Expected formats
            // dateString = dd mmm yy
            // timeString = hh:mm:ss
            int currentTwoDigitYear = DateTime.Now.Year % 100;
            int currentCentury = (DateTime.Now.Year - currentTwoDigitYear) / 100;
            int twoDigitYear = Int32.Parse(dateString.Substring(7, 2));
            int fourDigitYear = (twoDigitYear > currentTwoDigitYear) ?
                Int32.Parse((currentCentury - 1).ToString() + twoDigitYear.ToString()) :
                Int32.Parse(currentCentury.ToString() + twoDigitYear.ToString());
            int month = DateTime.ParseExact(dateString.Substring(3, 3), "MMM", CultureInfo.InvariantCulture).Month;
            int day = Int32.Parse(dateString.Substring(0, 2));
            int hour = Int32.Parse(timeString.Substring(0, 2));
            int minute = Int32.Parse(timeString.Substring(3, 2));
            int second = Int32.Parse(timeString.Substring(6, 2));

            return new DateTime(fourDigitYear, month, day, hour, minute, second);
        }
    }
}
