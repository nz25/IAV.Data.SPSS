using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IAV.Data.SPSS.SavFile
{
    public class VariableRolesRecord
    {
       
        public File File { get; set; }
        public string VariableRoles { get; set; }

        public VariableRolesRecord(File file)
        {
            this.File = file;
        }
        
        public void ReadFromInfoRecord(InfoRecord ir)
        {
          
            var originalBytes = (from item in ir.Items select item[0]).ToArray();
            this.VariableRoles = Encoding.Default.GetString(originalBytes);

        }
    }
}
