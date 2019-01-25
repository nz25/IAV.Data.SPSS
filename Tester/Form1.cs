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
using IAV.Data.SPSS;
using IAV.Data.SPSS.SavFile;
using System.Globalization;

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
            FileStream s = new FileStream("d:\\ExampleDataset.sav", FileMode.Open);
            IAV.Data.SPSS.SavFile.File savFile = new IAV.Data.SPSS.SavFile.File();
            savFile.ReadFromStream(s);

            IAV.Data.SPSS.Dataset ds = new IAV.Data.SPSS.Dataset();
            ds.ReadFromSavFile(savFile);
            
            //int recordcount = 0;
            //foreach (var dataRecord in savFile.DataRecords)
            //{
            //    recordcount++;
            //}            
            //MessageBox.Show(recordcount.ToString());

            // WRITING
            //FileStream t = new FileStream("d:\\test.sav", FileMode.Create);
            //savFile.WriteToStream(t);

            var originalBytes = (from item in savFile.InfoRecords[6].Items select item[0]).ToArray();
            var xxx = Encoding.Default.GetString(originalBytes);
            MessageBox.Show("OK");
        }

        //private void CopyFileStream(FileStream stream, Int32 start, Int32 length)
        //{
        //    FileStream copy = new FileStream("d:\\copy.sav", FileMode.Create);
        //    stream.Seek(start, 0);
        //    stream.CopyTo(copy);
        //    copy.SetLength(length);
        //}

    }
}
