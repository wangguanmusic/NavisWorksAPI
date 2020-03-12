using Autodesk.Navisworks.Api.Timeliner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NW = Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.DocumentParts;
using Autodesk.Navisworks.Api.Clash;
using System.Windows.Forms;

namespace AECOM.NavisWorks2019.Addins.Functions
{
    class CreateClashTest
    {
        public static void createClashTest(string testName, List<string> catAs, List<string> catBs)
        {
            //Define target document and elements
            NW.Document doc = NW.Application.ActiveDocument;
            DocumentClash documentClash = doc.GetClash();
            DocumentClashTests oDCT = documentClash.TestsData;
            //Create a clash test
            ClashTest oNewTest = new ClashTest();
            oNewTest.DisplayName = testName;

            //Set SelectionA and SelectionB of the ClashTest
            NW.ModelItemCollection oSelA = new NW.ModelItemCollection();
            NW.ModelItemCollection oSelB = new NW.ModelItemCollection();

            try
            {
                NW.ModelItemEnumerableCollection oModelCollect = doc.Models[0].RootItem.Children;
            }
            catch (Exception e)
            {

                MessageBox.Show(e.Message);
            }
            

            //Search elements for selection A with Category
            NW.Search searchA = new NW.Search();
            searchA.Selection.SelectAll();

            //Search by property
            //List<string> catAs = new List<string>() { "Ducts", "Pipes" };
            foreach (string s in catAs)
            {
                List<NW.SearchCondition> sC_GroupA = new List<NW.SearchCondition>();
                NW.SearchCondition searchCondition = NW.SearchCondition.HasPropertyByDisplayName("Revit Type", "Category").EqualValue(NW.VariantData.FromDisplayString(s));
                sC_GroupA.Add(searchCondition);
                searchA.SearchConditions.AddGroup(sC_GroupA);
            }

            try
            {
                NW.ModelItemCollection itemsA = searchA.FindAll(doc, false);
                foreach (NW.ModelItem i in itemsA)
                {
                    oSelA.Add(i);
                }
                oNewTest.SelectionA.Selection.CopyFrom(oSelA);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

            //Search elements for selection B with Category
            NW.Search searchB = new NW.Search();
            searchB.Selection.SelectAll();

            //Search by property
            //List<string> catBs = new List<string>() { "Ducts", "Pipes" };
            foreach (string s in catBs)
            {
                List<NW.SearchCondition> sC_GroupB = new List<NW.SearchCondition>();
                NW.SearchCondition searchCondition = NW.SearchCondition.HasPropertyByDisplayName("Revit Type", "Category").EqualValue(NW.VariantData.FromDisplayString(s));
                sC_GroupB.Add(searchCondition);
                searchB.SearchConditions.AddGroup(sC_GroupB);

            }


            try
            {
                NW.ModelItemCollection itemsB = searchB.FindAll(doc, false);
                foreach (NW.ModelItem i in itemsB)
                {
                    oSelB.Add(i);
                }
                oNewTest.SelectionB.Selection.CopyFrom(oSelB);

            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
            }

            oDCT.TestsAddCopy(oNewTest);


        }
    }
}
