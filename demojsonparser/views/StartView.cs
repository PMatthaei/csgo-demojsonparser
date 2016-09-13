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
using DemoInfoModded;

namespace demojsonparser
{
    public partial class StartView : Form
    {
        public StartView()
        {
            InitializeComponent();

            chooseLayout.BackColor = Color.FromArgb(35, Color.Black);
            headline.BackColor = Color.FromArgb(45, Color.Black);
            errorLayout.BackColor = Color.FromArgb(45, Color.Black);
            prettyjson_checkBox.BackColor = Color.FromArgb(45, Color.Black);

            //Provide view to generator for information about generation process
            GameStateGenerator.setView(this);

        }

        /// <summary>
        /// Access errorbox outside startview class for feedback about performance, state or errors
        /// </summary>
        /// <returns></returns>
        public RichTextBox getErrorBox()
        {
            return errorbox;
        }

        public bool getCheckPretty()
        {
            return prettyjson_checkBox.Checked;
        }

        private void choose_demo_Click(object sender, EventArgs e)
        {
            fileDialogChoose.Multiselect = true;
            fileDialogChoose.Filter = "CS:GO Demo File|*.dem";
            fileDialogChoose.FileName = "";

            if (fileDialogChoose.ShowDialog() == DialogResult.OK)
                textBoxChoose.Text = String.Join("", fileDialogChoose.FileNames);
        }

        private void button_parseJSON_Click(object sender, EventArgs e)
        {
            foreach(string path in fileDialogChoose.FileNames)
            {
                errorbox.AppendText("Start parsing: " + path + "\n");
                errorbox.Update();

                using (var demoparser = new DemoParser(File.OpenRead(path))) //Force garbage collection since outputstream of the parser cannot be changed
                {
                    GameStateGenerator.GenerateJSONFile(demoparser, path);
                }
            }

        }

        private void logclear_Click(object sender, EventArgs e)
        {
            errorbox.Clear();
        }


    }
}
