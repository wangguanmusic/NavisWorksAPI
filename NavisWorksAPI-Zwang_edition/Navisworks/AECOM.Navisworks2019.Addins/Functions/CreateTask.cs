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
    class CreateTask
    {

        public static void createTask(NW.Document doc, NW.GroupItem groupItem, DocumentTimeliner doc_TL, int n)
        {
            ////
            int i = 1;

            DateTime startDate = new DateTime(2020, 09, 01);
            while (i < n + 1)
            {
                //Create a task
                TimelinerTask task = new TimelinerTask();
                //Name the task
                task.DisplayName = "Task " + i;
                TimelinerTask childTask = new TimelinerTask();
                //Name the child task
                //childTask.DisplayName = "Child Task " + i;

                //task.Children.Add(childTask);

                //Set task start date
                //task.PlannedStartDate = startDate + new TimeSpan(i, 0, 0, 0);
                ////Set task end date
                //task.PlannedEndDate = startDate + new TimeSpan(i + 1, 0, 0, 0);
                //Set task type
                //task.SimulationTaskTypeName = "Construct";




                //Create search to find element
                NW.Search search = new NW.Search();
                search.Selection.SelectAll();

                search.SearchConditions.Add(NW.SearchCondition.HasPropertyByDisplayName("Element", "4D_Task_ID").EqualValue(NW.VariantData.FromInt32(i)));


                // execute search
                try
                {
                    NW.ModelItemCollection items = search.FindAll(doc, false);
                    string msg = items.Count.ToString();
                    task.Selection.CopyFrom(items);
                    //MessageBox.Show(msg);
                }
                catch (Exception ex)
                {

                    MessageBox.Show(ex.Message);
                }
                
                //Add task to timeliner
                groupItem.Children.Add(task);

                i = i + 1;
            }


            doc_TL.TasksCopyFrom(groupItem.Children);


        }
    }
}
