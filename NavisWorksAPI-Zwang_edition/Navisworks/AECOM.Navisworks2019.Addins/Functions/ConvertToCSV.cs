using Autodesk.Navisworks.Api.Timeliner;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NW = Autodesk.Navisworks.Api;
using System.Windows.Forms;
using System.IO;
using AECOM.Navisworks2019.Addins;

namespace AECOM.NavisWorks2019.Addins.Functions
{
    class ConvertToCSV
    {
        public static void convertToCSV(List<ElementTracking> elementTrackings, string path)
        {
            Type itemType = typeof(ElementTracking);
            var props = itemType.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance).OrderBy(p => p.Name);

            using (var writer = new StreamWriter(path))
            {
                writer.WriteLine(string.Join(", ", props.Select(p => p.Name)));

                foreach (var item in elementTrackings)
                {
                    writer.WriteLine(string.Join(", ", props.Select(p => p.GetValue(item, null))));
                }
            }


            
        }
    }
}
