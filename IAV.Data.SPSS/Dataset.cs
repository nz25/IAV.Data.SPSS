using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAV.Data.SPSS.SavFile;
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
        public Dictionary<int, int> VariableByRecord { get; set; }
        public Dictionary<int, int> RecordByVariable { get; set; }

        public List<Variable> Variables { get; set; }

        public Dataset()
        {
            this.Variables = new List<Variable>();
            this.VariableByRecord = new Dictionary<int, int>();
            this.RecordByVariable = new Dictionary<int, int>();
        }

        public void ReadFromSavFile(File savFile)
        {
            this.SavFile = savFile;
            this.ParseFileHeaderRecord();
            this.ParseVariableRecords();
            this.ParseValueLabelRecords();
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
                    Variable v = new Variable();
                    v.ReadFromVariableRecord(vr, this.SavFile.VariableRecords.IndexOf(vr));
                    this.Variables.Add(v);
                    this.RecordByVariable.Add(variableIndex , recordIndex);

                    variableIndex += 1;
                    
                }
                else // for string extensions variable records increments the VariableRecordLength field for the last added variable
                {
                    this.Variables[variableIndex - 1].VariableRecordLength += 1;
                }
                this.VariableByRecord.Add(recordIndex, variableIndex - 1);
                
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
                this.VariableByRecord.TryGetValue(vlvr.VariableIndices[0] - 1, out variableIndex);
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
                    this.VariableByRecord.TryGetValue(recordIndex - 1 , out variableIndex);
                    this.Variables[variableIndex].ValueLabels = ValueLabels;
                }   
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
