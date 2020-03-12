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
using System.Dynamic;

//Autodesk reference
using NW = Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Timeliner;
using Autodesk.Navisworks.Api.Plugins;

//Custom reference
using AECOM.NavisWorks2019.Addins;
using System.Text.RegularExpressions;

namespace AECOM.Navisworks2019.Addins
{
    /// <summary>
    /// Interaction logic for CreateTaskWithGroups.xaml
    /// </summary>
    public partial class CreateTaskWithGroups : System.Windows.Controls.UserControl
    {
        //Global parameter
        public NW.Document doc;
        public DocumentTimeliner doc_TL;
        public NW.GroupItem _groupItem;
        List<Task> tasks = new List<Task>();
        List<String> parentTasks = new List<String>();
        string selProperty;

        //Set up form init parameter
        public CreateTaskWithGroups(NW.Document document, DocumentTimeliner documentTimeliner, NW.GroupItem groupItem)
        {
            InitializeComponent();
            doc = document;
            doc_TL = documentTimeliner;
            _groupItem = groupItem;
            
        }

        #region event handler
        //handle select csv file 
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();

            //filter for csv file only 
            dlg.DefaultExt = ".csv";

            //display open file dialog
            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                string fileName = dlg.FileName;
                filePath.Text = System.IO.Path.GetFileName(fileName);


                //add group properties to combo box
                //string[] _properties = getProperties(fileName, tasks);
                string[] _properties = { "ActivityID", "ActivityName", "OriginalDuration", "Start", "Finish", "Trade_Bid", "Block", "Zone", "Level" };
                foreach (string p in _properties)
                {
                   
                    groupProperty.Items.Add(p);
                }


                //compile list of tasks
                //init stream reader for csv
                StreamReader streamReader = new StreamReader(fileName);
                string strLine = streamReader.ReadLine();//skip header                               

                //start reading lines and creating tasks
                while (!streamReader.EndOfStream)
                {
                    strLine = streamReader.ReadLine();//read each line except for header
                                                      //define the serarator in CSV file
                    //string[] _task = strLine.Split(',');


                    Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                    string[] _task  = CSVParser.Split(strLine);


                    Task task = new Task();

                    //add property to task
                    task.ActivityID = _task[0];
                    task.ActivityName = _task[1];
                    task.OriginalDuration = _task[2];
                    task.Start = _task[3];
                    task.Finish = _task[4];
                    task.Trade_Bid = _task[5];
                    task.Block = _task[6];
                    task.Zone = _task[7];
                    task.Level = _task[8];

                    //add task to task list
                    tasks.Add(task);
                    //System.Windows.MessageBox.Show(task.GetType().GetProperty(selProperty).GetValue(task).ToString());

                }

            }
            
        }

        //handle combo box selection change
        private void groupProperty_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            
            selProperty = groupProperty.SelectedItem.ToString();
            //int count = tasks.Count;                     

            //System.Windows.MessageBox.Show(parentTasks.Count.ToString());
                       
        }

        //clean current timeliner
        private void cleanTask_Click(object sender, RoutedEventArgs e)
        {
            doc_TL.Clear();
        }

        //create tasks
        private void creatTask_Click(object sender, RoutedEventArgs e)
        {
            foreach (Task t in tasks)
            {
                
                //System.Windows.MessageBox.Show(t.GetType().GetProperty(selProperty).GetValue(t).ToString());
                string childPro = t.GetType().GetProperty(selProperty).GetValue(t).ToString();
                //System.Windows.MessageBox.Show(childPro);
                if (!parentTasks.Contains(childPro))
                {
                    parentTasks.Add(childPro);
                    //add parent task
                    TimelinerTask parentTask = new TimelinerTask();
                    parentTask.DisplayName = childPro;

                    //create child task 
                    TimelinerTask childTask = createChildTask(t);

                    parentTask.Children.Add(childTask);
                    _groupItem.Children.Add(parentTask);

                }
                else
                {

                    //create child task 
                    TimelinerTask childTask = createChildTask(t);

                    //add child task to parent task
                    TimelinerTask parentTask = _groupItem.Children.First(p => p.DisplayName == childPro) as TimelinerTask;
                    parentTask.Children.Add(childTask);
                    //_groupItem.Children.Add(parentTask);
                }


            }

            doc_TL.TasksCopyFrom(_groupItem.Children);
        }

        #endregion

        #region local functions

        private static String[] getProperties(string fileName, List<Task> tasks)
        {
            string _filePath = fileName;

            //init stream reader for csv
            StreamReader streamReader = new StreamReader(_filePath);
            string strLine = streamReader.ReadLine();
            //System.Windows.MessageBox.Show(strLine);
            //get individual values
            //string[] _value = strLine.Split(',');   //need to ignore "," within quote

            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            string[] _value = CSVParser.Split(strLine);
            
            return _value;            
        }

        private static TimelinerTask createChildTask(Task task)
        {
            //create child task 
            TimelinerTask childTask = new TimelinerTask();
            //Name the child task
            childTask.DisplayName = task.ActivityName;
            childTask.DisplayId = task.ActivityID;
            if (task.Start != "")
            {
                childTask.PlannedStartDate = Convert.ToDateTime(task.Start);
            }
            if (task.Finish != "")
            {
                childTask.PlannedEndDate = Convert.ToDateTime(task.Finish);
            }
           
            return childTask;
        }


        #endregion

       
    }
    #region custome classes
    public class Task
    {
       
        public string ActivityID { get; set; }
        public string ActivityName { get; set; }
        public string OriginalDuration { get; set; }
        public string Start { get; set; }
        public string Finish { get; set; }
        public string Trade_Bid { get; set; }
        public string Block { get; set; }
        public string Zone { get; set; }
        public string Level { get; set; }
        

    }

    #endregion
}
