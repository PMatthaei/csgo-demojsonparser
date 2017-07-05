using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Forms;
using JSON;
using DemoInfo;
using GameStateGenerator;
using Newtonsoft.Json;
 
namespace DemojsonparserGUI
{

    public class DemoListEntry
    {
        public string FileName { get; set; }

        public string FilePath { get; set; }

    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    /// 
    public partial class MainWindow : Window
    {
        private OpenFileDialog fdlg;

        private FolderBrowserDialog fldlg;

        private List<DemoListEntry> items = new List<DemoListEntry>();

        public MainWindow()
        {
            InitializeComponent();

            if (fdlg == null)
                initFileChooser();
        }


        private void initFileChooser()
        {
            // Configure open file dialog box
            fdlg = new OpenFileDialog();
            fdlg.Multiselect = true;
            fdlg.FileName = ""; // Default file name
            fdlg.DefaultExt = ".dem"; // Default file extension
            fdlg.Filter = "CSGO Demofile (.dem)|*.dem"; // Filter files by extension
        }

        private void OnChooseSrcClick(object sender, RoutedEventArgs e)
        {
              // Show open file dialog box
            DialogResult result = fdlg.ShowDialog();

            // Process open file dialog box results
            if (result == System.Windows.Forms.DialogResult.OK)
                // Open document
                pathBox.Text = String.Join("", fdlg.FileNames);


            foreach (string dem in fdlg.FileNames)
                items.Add(new DemoListEntry() { FileName = System.IO.Path.GetFileName(dem), FilePath = dem });


            ChoosenListView.ItemsSource = items;

        }

        private void OnChooseDestClick(object sender, RoutedEventArgs e)
        {
            if (fldlg == null)
                fldlg = new FolderBrowserDialog();

            // Show open file dialog box
            DialogResult result = fldlg.ShowDialog();

            // Process open file dialog box results
            if (result == System.Windows.Forms.DialogResult.OK)
                // Open document
                alternateTextBox.Text = fldlg.SelectedPath;
        }


        private void OnParseClick(object sender, RoutedEventArgs e)
        {
            var selectedItem = (DemoListEntry)ChoosenListView.SelectedItem;
            if (selectedItem != null)
            {
                parseSelectedItem(selectedItem);
                return;
            } else if(items.Count == 0) //If nothing is selected to parse and the list is empty -> Nothing to parse
                eventlog.AppendText("No files to parse !\n");
            
            //Parse the whole list
            for (int entryid = items.Count - 1; entryid >= 0; entryid--)
            {
                var parseEntry = items[entryid];
                eventlog.AppendText("Start parsing: " + parseEntry.FilePath + " !\n");
                eventlog.ScrollToEnd();

                if (!Directory.Exists(alternateTextBox.Text))
                    alternateTextBox.Text = parseEntry.FilePath.Replace(parseEntry.FileName, ""); //Fails with multiple occurencies of filename in path (who does this?)

                this.parseEntryAt(entryid, parseEntry);
            }

            items.Clear();
            ChoosenListView.ItemsSource = items;
            ChoosenListView.Items.Refresh();
        }


        private void OnDoubleClickEntry(object sender, MouseButtonEventArgs e)
        {
            var selectedItem = (DemoListEntry)ChoosenListView.SelectedItem;
            parseSelectedItem(selectedItem);
            items.Remove(selectedItem);
            ChoosenListView.ItemsSource = items;
            ChoosenListView.Items.Refresh();
        }

        private void parseSelectedItem(DemoListEntry selectedItem)
        {
            if (selectedItem == null)
            {
                eventlog.AppendText("No item selected\n");
                return;
            }

            DemoListEntry d = selectedItem;
            eventlog.AppendText("Start parsing selected item: " + d.FileName + "\n");

            if (!Directory.Exists(alternateTextBox.Text))
                alternateTextBox.Text = d.FilePath.Replace(d.FileName, ""); //Fails with multiple occurencies of filename in path (who does this?)

            parse(d);
        }

        private void parse(DemoListEntry d)
        {
            using (var parser = new DemoParser(File.OpenRead(d.FilePath)))
            {
                ParseTask p = new ParseTask
                {
                    destpath = alternateTextBox.Text + "\\" + d.FileName,
                    srcpath = d.FilePath,
                    usepretty = prettyCheckBox.IsChecked.Value,
                    showsteps = stepsCheckBox.IsChecked.Value,
                    specialevents = playerSpecialCheckBox.IsChecked.Value,
                    highdetailplayer = precisionCheckBox.IsChecked.Value,
                    positioninterval = Int32.Parse(poscount.Text)
                };

                CSGOGameStateGenerator.GenerateJSONFile(parser, p);
            }

            eventlog.AppendText("Parsing was successful!\n");
            eventlog.ScrollToEnd();
            items.Remove(d);
            ChoosenListView.ItemsSource = items;
            ChoosenListView.Items.Refresh();
        }

        private void parseEntryAt(int entryid, DemoListEntry parseEntry)
        {
            using (var parser = new DemoParser(File.OpenRead(parseEntry.FilePath)))
            {
                ParseTask p = new ParseTask
                {
                    destpath = alternateTextBox.Text + "\\" + parseEntry.FileName,
                    srcpath = parseEntry.FilePath,
                    showsteps = stepsCheckBox.IsChecked.Value,
                    specialevents = playerSpecialCheckBox.IsChecked.Value,
                    highdetailplayer = precisionCheckBox.IsChecked.Value,
                    positioninterval = Int32.Parse(poscount.Text),
                    settings = new JsonSerializerSettings()
                };

                CSGOGameStateGenerator.GenerateJSONFile(parser, p);
            }

            eventlog.AppendText("Parsing was successful!\n");
            eventlog.ScrollToEnd();
            items.RemoveAt(entryid);
            ChoosenListView.ItemsSource = items;
            ChoosenListView.Items.Refresh();
        }
    }
}
