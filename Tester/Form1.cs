using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using IAV.Data.SPSS.IO;

namespace Tester
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {

            FileStream s = new FileStream("d:\\prepare DR Commerzbank AT.sav", FileMode.Open);
            //FileStream s = new FileStream("d:\\prepare DR Commerzbank AT_uncompressed.sav", FileMode.Open);

            StreamToFile s2f = new StreamToFile(s);
            s2f.ReadFromStream();

            int recordcount = 0;
            foreach (var dataRecord in s2f.SavFile.DataRecords)
            {
                recordcount++;                
            }
            
            MessageBox.Show(recordcount.ToString());
        }
    }
}
