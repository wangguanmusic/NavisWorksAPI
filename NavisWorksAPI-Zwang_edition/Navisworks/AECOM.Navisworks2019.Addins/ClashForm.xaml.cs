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

using Autodesk.Navisworks.Api.Plugins;
using NW = Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Timeliner;
using AECOM.NavisWorks2019.Addins;
using AF = AECOM.NavisWorks2019.Addins.Functions;

namespace AECOM.NavisWorks2019.Addins
{
    
/// <summary>
/// Interaction logic for ClashForm.xaml
/// </summary>
public partial class ClashForm : UserControl
    {
        public ClashForm()
        {
            //Initial the form
            InitializeComponent();
            //Create demo data object A
            List<Selection> catCollectionsA = new List<Selection>
           {
               
           };
            //Create demo data object B
            List<Selection> catCollectionsB = new List<Selection>
           {
               
           };

            //Add data to form source
            selectionA.ItemsSource = catCollectionsA;
            selectionB.ItemsSource = catCollectionsB;
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {

            List<string> listA = new List<string>();
            List<string> listB = new List<string>();

            foreach(var item in selectionA.ItemsSource)
            {
                listA.Add(item.ToString());
            }

            foreach (var item in selectionB.ItemsSource)
            {
                listB.Add(item.ToString());
            }
            //MessageBox.Show("number of selectionA " + listA.Count.ToString() + "/n" +
            //                "number of selectionB " + listB.Count.ToString() + "/n");
            string a = TestName.Text;
            AECOM.NavisWorks2019.Addins.Functions.CreateClashTest.createClashTest(a, listA, listB);
            
        }

       

    }

    
    public class Selection
    {
        //public string selection { get; set; }
        public string cat { get; set; }

        public override string ToString()
        {
            return cat;
        }
    }

    public class categoryList : List<string>
    {
        
        public categoryList()
        {
            List<string> uniqueCategories = new List<string>();

            NW.Document doc = NW.Application.ActiveDocument;
            NW.Search search = new NW.Search();
            search.Selection.SelectAll();

            //Search by property

            search.SearchConditions.Add(NW.SearchCondition.HasPropertyByDisplayName("Element", "Category"));

            try
            {
                NW.ModelItemCollection items = search.FindAll(doc, false);
                foreach (NW.ModelItem item in items)
                {
                    string text = item.PropertyCategories.FindPropertyByDisplayName("Element", "Category").Value.ToDisplayString();
                    if (!uniqueCategories.Contains(text))
                    {
                        uniqueCategories.Add(text);

                    }
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

            foreach(string s in uniqueCategories)
            {
                this.Add(s);
            }

            
        }

    }
    
    
}
