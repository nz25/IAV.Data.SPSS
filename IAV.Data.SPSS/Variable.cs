using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IAV.Data.SPSS.SavFile;
using IAV.Data.SPSS.Enum;

namespace IAV.Data.SPSS
{
    public class Variable
    {
        public Dataset Dataset { get; set; }
        public DataType DataType { get; set; }
        public int StringWidth { get; set; }
        public int VariableRecordIndex { get; set; }
        public int VariableIndex { get; set; }
        public int VariableRecordLength { get; set; }
        public bool HasVariableLabel { get; set; }
        public MissingValuesType MissingValuesType { get; set; }
        public List<double> MissingValues { get; set; }
        public VariableFormat PrintFormat { get; set; }
        public VariableFormat WriteFormat { get; set; }
        public string ShortName { get; set; }
        public string Name { get; set; }
        public string Label { get; set; }
        public MeasurementType MeasurementType { get; set; }
        public int Width { get; set; }
        public int VeryLongStringWidth { get; set; }
        public Alignment Alignment { get; set; }

        public List<ValueLabel> ValueLabels { get; set; }

        public Variable(Dataset ds)
        {
            this.MissingValues = new List<double>();
            this.ValueLabels = new List<ValueLabel>();
        }

        public void ReadFromVariableRecord(VariableRecord vr, int variableRecordIndex)
        {
            this.DataType = (vr.Type == VariableType.Numeric) ? DataType.Numeric : DataType.String;
            this.StringWidth = (vr.Type == VariableType.Numeric) ? 0 : (int)(vr.Type);
            this.VariableRecordIndex = variableRecordIndex;
            this.VariableRecordLength = 1;
            this.HasVariableLabel = (vr.HasVariableLabel == 1) ? true : false;
            this.MissingValuesType = this.GetMissingValueTypeFromMissingValueCount(vr.MissingValueCount);
            this.MissingValues = vr.MissingValues;
            this.PrintFormat = vr.PrintFormat;
            this.WriteFormat = vr.WriteFormat;
            this.ShortName = vr.Name.Trim();
            this.Label = this.HasVariableLabel ? vr.Label.Trim() : vr.Label;
        }

        public void ReadFromVariableParameters(VariableParameters vp)
        {
            this.MeasurementType = vp.MeasurementType;
            this.Width = vp.Width;
            this.Alignment = vp.Alignment;
        }

        private MissingValuesType GetMissingValueTypeFromMissingValueCount(MissingValueCount mvc)
        {
            switch (mvc)
            {
                case MissingValueCount.NoMissingValue:
                    return MissingValuesType.NoMissingValues;
                case MissingValueCount.OneMissingValue:
                    return MissingValuesType.DescreteMissingValues;
                case MissingValueCount.TwoMissingValues:
                    return MissingValuesType.DescreteMissingValues;
                case MissingValueCount.ThreeMissingValues:
                    return MissingValuesType.DescreteMissingValues;
                case MissingValueCount.RangeOfMissingValues:
                    return MissingValuesType.RangeOfMissingValues;
                case MissingValueCount.RangeOfMissingValuesPlusSingleDescreteValue:
                    return MissingValuesType.RangeOfMissingValuesPlusSingleDescreteValue;
                default:
                    return MissingValuesType.NoMissingValues;
            }

        }
    }
}
