using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using demojsonparser.src;
using DemoInfo;

namespace demojsonparser
{
    public partial class StartView : Form
    {
        public StartView()
        {
            InitializeComponent();
        }

        private void label1_Click(object sender, EventArgs e)
        {

        }

        private void choose_demo_Click(object sender, EventArgs e)
        {
            fileDialogChoose.Multiselect = true;
            fileDialogChoose.Filter = "CS:GO Demo File|*.dem";

            if (fileDialogChoose.ShowDialog() == DialogResult.OK)
                textBoxChoose.Text = String.Join("", fileDialogChoose.FileNames);
        }

        private void button_parseJSON_Click(object sender, EventArgs e)
        {
            foreach(string path in fileDialogChoose.FileNames)
            {
                using (var demoparser = new DemoParser(File.OpenRead(path))) //Force garbage collection since outputstream of the parser cannot be changed
                {
                    src.GameStateGenerator.GenerateJSONFile(demoparser, path);
                }
            }
            
        }
    }
}
