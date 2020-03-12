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
using NW = Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Timeliner;
using Autodesk.Navisworks.Api.Plugins;
using AECOM.NavisWorks2019.Addins;



namespace AECOM.NavisWorks2019.Addins
{
    /// <summary>
    /// Interaction logic for ScheduleSetting.xaml
    /// </summary>
    public partial class ScheduleSetting : UserControl
    {
        public object ScheduleResult { get; set; }

        public double CalculateValue()
        {
            return 0;
        }

        public NW.Document doc;
        public DocumentTimeliner doc_TL;
        public NW.GroupItem _groupItem;
        int _taskNumber;

        public ScheduleSetting(List<ObjectInfo> objectInfos, NW.Document document, DocumentTimeliner documentTimeliner, NW.GroupItem groupItem, int taskNumber)
        {
            
            
            InitializeComponent();
            doc = document;
            doc_TL = documentTimeliner;
            _groupItem = groupItem;
            _taskNumber = taskNumber;
                    
            ProjectPhase.ItemsSource = objectInfos;


            
        }

        private void Button_CreateTask_Click(object sender, RoutedEventArgs e)
        {
            
            AECOM.NavisWorks2019.Addins.Functions.CreateTask.createTask(doc,  _groupItem, doc_TL, _taskNumber);
        }

        
    }


    public class Task
    {
        public string taskName { get; set; }
        public string taskID { get; set; }

    }

    public class ObjectInfo
    {
        public string objectName { get; set; }
        public string objectId { get; set; }
        public string objectPhase { get; set; }
        public string objectTask { get; set; }
    }
}
