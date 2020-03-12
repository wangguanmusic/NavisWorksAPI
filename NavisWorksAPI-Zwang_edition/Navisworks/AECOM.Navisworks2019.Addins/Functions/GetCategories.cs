using Autodesk.Navisworks.Api.Timeliner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NW = Autodesk.Navisworks.Api;
using System.Windows.Forms;

namespace AECOM.NavisWorks2019.Addins.Functions
{
    public class GetCategories
    {
        public static int NumberOfEmployees;
        private static int counter;
        private string name; 

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public static int Counter
        {
            get { return counter; }
        }

        public GetCategories()
        {
            counter = 21;
        }

        //public List<string> uniqueCategories;
        //public List<string> UniqueCategories
        //{
        //    get { return uniqueCategories; }
        //    set { uniqueCategories = value; }
        //}

        //public static List<string> getCategories(NW.Document document, List<string> )
        //{
        //    List<string> textList = new List<string>();
        //    textList.Add("adfese");
        //    textList.Add("ad324");
        //    textList.Add("3224ers");
            
        //    return textList;

            

        //}
    }
}
