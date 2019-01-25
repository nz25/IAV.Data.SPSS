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
using IAV.Data.SPSS.SavFile;

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


            // READING

            FileStream s = new FileStream("d:\\prepare DR Commerzbank AT.sav", FileMode.Open);
            //FileStream s = new FileStream("d:\\prepare DR Commerzbank AT_uncompressed.sav", FileMode.Open);

            SavFile savFile = new SavFile();
            savFile.ReadFromStream(s);

            //int recordcount = 0;
            //foreach (var dataRecord in savFile.DataRecords)
            //{
            //    recordcount++;                
            //}
            
            //MessageBox.Show(recordcount.ToString());

            // WRITING
            FileStream t = new FileStream("d:\\test.sav", FileMode.Create);
            savFile.WriteToStream(t);

        }
    }
}
