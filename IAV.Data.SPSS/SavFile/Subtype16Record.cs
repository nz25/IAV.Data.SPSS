using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.SavFile
{
    public class Subtype16Record
    {
                
        public File File { get; set; }
        public List<double> DoubleValues { get; set; }

        public Subtype16Record(File file)
        {
            this.File = file;
            this.DoubleValues = new List<double>();
        }
        
        public void ReadFromInfoRecord(InfoRecord ir)
        {

            foreach (var item in ir.Items)
            {
                DoubleValues.Add(BitConverter.ToDouble(item, 0));
            }

        }
    }
}
