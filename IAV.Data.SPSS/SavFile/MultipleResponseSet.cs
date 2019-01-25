using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.SavFile
{
    public class MultipleResponseSet
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public int Value { get; set; }
        public int LabelLength { get; set; }
        public string Label { get; set; }
        public List<string> Variables { get; set; }

        public MultipleResponseSet()
        {
            this.Variables = new List<string>();
        }

        public void ReadFromString(string inputString)
        {
            // parses Name
            int position = inputString.IndexOf('=');
            this.Name = inputString.Substring(0, position);
            inputString = inputString.Substring(position + 1);

            // parses Parameter1
            position = inputString.IndexOf(' ');
            this.Type = inputString.Substring(0, position);
            inputString = inputString.Substring(position + 1);

            //parses Parameter2
            if (this.Type.Substring(0,1) == "D")
            {
                position = inputString.IndexOf(' ');
                this.Value = Int32.Parse(inputString.Substring(0, position));
                inputString = inputString.Substring(position + 1);
            }
 
            //parses LabelLength
            position = inputString.IndexOf(' ');
            this.LabelLength = Int32.Parse(inputString.Substring(0, position));
            inputString = inputString.Substring(position + 1);

            //parses Label
            position = this.LabelLength;
            this.Label = inputString.Substring(0, position);
            inputString = inputString.Substring(position + 1);

            var variables = inputString.Split(' ');
            foreach (var variable in variables)
            {
                this.Variables.Add(variable);
            }
            
        }

    }
}
