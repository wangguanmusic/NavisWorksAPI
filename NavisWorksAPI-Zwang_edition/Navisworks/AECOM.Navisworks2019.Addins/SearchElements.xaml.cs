using System;
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
using System.IO;

using NW = Autodesk.Navisworks.Api;
using AECOM.NavisWorks2019.Addins.Functions;

namespace AECOM.Navisworks2019.Addins
{
    /// <summary>
    /// Interaction logic for SearchElements.xaml
    /// </summary>
    public partial class SearchElements : System.Windows.Controls.UserControl
    {
        public SearchElements(NW.Document doc)
        {
            InitializeComponent();
            Document = doc;
        }

        //define globle parameters
        public string SearchFolder { get; set; }
        public string ElementIdPath { get; set; }
        public string OutputPath { get; set; }

        List<string> MasterFiles = new List<string>();
        List<string> ElementIDs = new List<string>();
        List<ElementTracking> ElementTrackings = new List<ElementTracking>();

        string Progress = "";
        NW.Document Document;


        private void btSelectSearchFolder_Click(object sender, RoutedEventArgs e)
        {
            #region Set NWD Folder

            FolderBrowserDialog FD = new FolderBrowserDialog();

            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string SearchFolder = FD.SelectedPath;

                string[] allFiles = Directory.GetFiles(SearchFolder, "*.nwd", SearchOption.AllDirectories);
                for (int i = 0; i < allFiles.Length; i++)
                {
                    if (allFiles[i].Contains("MASTER"))
                    {
                        MasterFiles.Add(allFiles[i]);
                    }
                }

            }

            System.Windows.MessageBox.Show(MasterFiles.Count.ToString());
            #endregion
        }

        private void btSelectElementIdsFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog FD = new System.Windows.Forms.OpenFileDialog();

            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string ElementIdPath = FD.FileName;

                using (var reader = new StreamReader(ElementIdPath))
                {
                    while (!reader.EndOfStream)
                    {
                        var line = reader.ReadLine();
                        var values = line.Split(';');
                        foreach (var item in values)
                        {
                            ElementIDs.Add(item);
                        }
                        System.Windows.Forms.MessageBox.Show(ElementIDs.Count.ToString() + " id found.");
                    }
                }

            }
        }

        private void btSelectOutputFile_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog FD = new System.Windows.Forms.OpenFileDialog();

            if (FD.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                OutputPath = FD.FileName;

                System.Windows.Forms.MessageBox.Show(OutputPath + " is set for output report.");
            }
        }

        private void btSearch_Click(object sender, RoutedEventArgs e)
        {
            int fileTotal = MasterFiles.Count();
            int nbFile = 0;
            foreach (string nwFile in MasterFiles)
            {

                Document.OpenFile(nwFile);

                //Search by property

                foreach (string id in ElementIDs)
                {
                    //Search elements with 4D schedule information
                    NW.Search search = new NW.Search();
                    search.Selection.SelectAll();
                    int count;

                    search.SearchConditions.Add(NW.SearchCondition.HasPropertyByDisplayName("Element ID", "Value").EqualValue(NW.VariantData.FromDisplayString(id)));
                    // execute search
                    try
                    {
                        NW.ModelItemCollection items = search.FindAll(Document, true);
                        count = items.Count;
                        ElementTracking elementTracking = new ElementTracking();
                        elementTracking.FilePath = nwFile;
                        if (count > 0)
                        {
                            elementTracking.Existence = true;
                        }
                        else
                        {
                            elementTracking.Existence = false;
                        }
                        elementTracking.Submital = Directory.GetParent(nwFile).Name;
                        elementTracking.EliementID = id;
                        ElementTrackings.Add(elementTracking);

                    }
                    catch (Exception ex)
                    {

                        System.Windows.Forms.MessageBox.Show(ex.Message);
                    }
                }
                nbFile = nbFile + 1;
                Progress = nbFile.ToString() + " out of " + fileTotal.ToString();
                lbProgress.Content = Progress;
            }
            //System.Windows.Forms.MessageBox.Show(OutputPath);
            //write to CSV
            ConvertToCSV.convertToCSV(ElementTrackings, OutputPath);
        }


    }

    public class ElementTracking
    {
        private bool existence;
        private string submital;
        private string filePath;
        private string elementID;

        public bool Existence
        {
            get { return existence; }
            set
            {
                existence = value;
            }
        }
        public string Submital
        {
            get { return submital; }
            set
            {
                submital = value;
            }
        }

        public string FilePath
        {
            get { return filePath; }
            set
            {
                filePath = value;
            }
        }

        public string EliementID
        {
            get { return elementID; }
            set
            {
                elementID = value;
            }
        }
    }
}
