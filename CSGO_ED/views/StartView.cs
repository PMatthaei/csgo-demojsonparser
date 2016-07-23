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
using CSGO_ED.src;
using DemoInfo;

namespace CSGO_ED
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
            if (fileDialogChoose.ShowDialog() == DialogResult.OK)
                textBoxChoose.Text = fileDialogChoose.FileName;
        }

        private void button_parseJSON_Click(object sender, EventArgs e)
        {
            var demoparser = new DemoParser(File.OpenRead(textBoxChoose.Text));
            GameStateGenerator.GenerateJSONFile(demoparser, textBoxChoose.Text);
        }
    }
}
