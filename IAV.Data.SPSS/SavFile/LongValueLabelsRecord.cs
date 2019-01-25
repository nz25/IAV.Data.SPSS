using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.SavFile
{
    public class LongValueLabelsRecord
    {

        public File File { get; set; }
        public List<LongVariableWithValueLabels> LongVariableWithValueLabels { get; set; }

        public LongValueLabelsRecord(File file)
        {
            this.File = file;
            this.LongVariableWithValueLabels = new List<LongVariableWithValueLabels>();
        }
        
        public void ReadFromInfoRecord(InfoRecord ir)
        {
            var byteArray = (from item in ir.Items select item[0]).ToArray();
            int positionInArray = 0;
            while (positionInArray < byteArray.Length)
            {
                LongVariableWithValueLabels lvwvl = new LongVariableWithValueLabels();

                // first 4-bytes is the length of long variable name
                ArraySegment<byte> arraySegment = new ArraySegment<byte>(byteArray, positionInArray, 4);
                int varNameLength = BitConverter.ToInt32(arraySegment.ToArray(), 0);
                positionInArray += 4;

                // long variable name
                arraySegment = new ArraySegment<byte>(byteArray, positionInArray, varNameLength);
                lvwvl.LongVariableName = Encoding.Default.GetString(arraySegment.ToArray());
                positionInArray += varNameLength;

                // variable width
                arraySegment = new ArraySegment<byte>(byteArray, positionInArray, 4);
                lvwvl.VariableWidth = BitConverter.ToInt32(arraySegment.ToArray(), 0);
                positionInArray += 4;

                // number of value labels
                arraySegment = new ArraySegment<byte>(byteArray, positionInArray, 4);
                int valueLabelCount = BitConverter.ToInt32(arraySegment.ToArray(), 0);
                positionInArray += 4;

                for (int i = 0; i < valueLabelCount; i++)
                {

                    LongValueLabel vl = new LongValueLabel();

                    // length of value code skipped - should equal to variable width
                    positionInArray += 4;

                    // value code
                    arraySegment = new ArraySegment<byte>(byteArray, positionInArray, lvwvl.VariableWidth);
                    vl.Value = Encoding.Default.GetString(arraySegment.ToArray());
                    positionInArray += lvwvl.VariableWidth;

                    // length of value label
                    arraySegment = new ArraySegment<byte>(byteArray, positionInArray, 4);
                    vl.LabelLength = BitConverter.ToInt32(arraySegment.ToArray(), 0);
                    positionInArray += 4;

                    // value label
                    arraySegment = new ArraySegment<byte>(byteArray, positionInArray, vl.LabelLength);
                    vl.Label = Encoding.Default.GetString(arraySegment.ToArray());
                    positionInArray += vl.LabelLength;

                    lvwvl.ValueLabels.Add(vl);
                    
                }

                this.LongVariableWithValueLabels.Add(lvwvl);
            }

        }
    }
}
