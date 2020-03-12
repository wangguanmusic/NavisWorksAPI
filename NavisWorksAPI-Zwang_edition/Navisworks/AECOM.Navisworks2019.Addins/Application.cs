using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.IO;
using System.Dynamic;

// add autodesk namespace
using NW = Autodesk.Navisworks.Api;
using Autodesk.Navisworks.Api.Plugins;
using Autodesk.Navisworks.Api.Timeliner;
using AECOM.NavisWorks2019.Addins;
using ComApi = Autodesk.Navisworks.Api.Interop.ComApi;
using ComBridge = Autodesk.Navisworks.Api.ComApi.ComApiBridge;


//Add json converter
//using Newtonsoft.Json;

//custom functions
using AECOM.NavisWorks2019.Addins.Functions;
using System.Text.RegularExpressions;

namespace AECOM.Navisworks2019.Addins
{

    [Plugin("AECOMAddins", "Xiaoxuan Peng", DisplayName = "Xiaoxuan")]

    [Strings("CustomRibbon_V2019.name")]

    [RibbonLayout("CustomRibbon_V2019.xaml")]

    [RibbonTab("ID_CustomTab_1", DisplayName = "AECOM")]


    [Command("ID_Button_1", DisplayName = "Create Timeline Task", Icon = "Schedule_16.png", LargeIcon = "Schedule_32.png", ToolTip = "Create tasks based on object properties", ExtendedToolTip = "Properties need to be created from Revit and read from NavisWorks.")]
    [Command("ID_Button_2", DisplayName = "Create Clash Test", Icon = "Clash_16.png", LargeIcon = "Clash_32.png", ToolTip = "Create clash test", ExtendedToolTip = "Create clash test with code")]
    [Command("ID_Button_3", DisplayName = "Search Elements in Files", Icon = "Clash_16.png", LargeIcon = "Clash_32.png", ToolTip = "Create clash test", ExtendedToolTip = "Create clash test with code")]
    [Command("ID_Button_4", DisplayName = "Group Task By Property", Icon = "Group_16.png", LargeIcon = "Group_32.png", ToolTip = "Group Task By Property", ExtendedToolTip = "Group schedule tasks by seleted property.")]
    [Command("ID_Button_5", DisplayName = "Checker", Icon = "Group_16.png", LargeIcon = "Group_32.png", ToolTip = "Group Task By Property", ExtendedToolTip = "Group schedule tasks by seleted property.")]
    [Command("ID_Button_6", DisplayName = "Checker", Icon = "Group_16.png", LargeIcon = "Group_32.png", ToolTip = "Split selected model with zone/room bounding boxex.", ExtendedToolTip = "plit selected model with zone/room bounding boxex.")]
    [Command("ID_Button_7", DisplayName = "4D Modeling", Icon = "4D_16.png", LargeIcon = "4D_32.png", ToolTip = "4D modeling based on csv file", ExtendedToolTip = "generate 4D modeling based on CSV file.")]
    [Command("ID_Button_8", DisplayName = "Import MegaData", Icon = "MegaData_16.png", LargeIcon = "MegaData_32.png", ToolTip = "Import MegaData based on csv file", ExtendedToolTip = "Import MegaData based on CSV file.")]
    public class AECOMAddins : CommandHandlerPlugin
    {
        /// <summary>
        /// Constructor, just initialises variables.
        /// </summary>
        public AECOMAddins()
        {

        }

        /// <summary>
        /// Executes a command when a button in the ribbon is pressed.
        /// </summary>
        /// <param name="commandId">Identifies the command associated with the button 
        /// that was pressed, by the Id defined in the command attribute.</param>
        /// <param name="parameters">Not currently used by Navisworks. If command is
        /// invoked programmatically by plugin author it can be used to pass additional
        /// information.</param>
        /// <returns>Not used by Navisworks. If command is invoked programmatically by 
        /// plugin author then it can be used to return additional information.</returns>
        public override int ExecuteCommand(string commandId, params string[] parameters)
        {
            switch (commandId)
            {
                case "ID_Button_1":
                    {
                        //Define target document and elements
                        NW.Document doc = NW.Application.ActiveDocument;
                        DocumentTimeliner doc_TL = NW.Application.ActiveDocument.GetTimeliner();
                        NW.GroupItem groupItem = doc_TL.TasksRoot.CreateCopy() as NW.GroupItem;

                        //Get Object Information and create objects to pass in WPF
                        List<ObjectInfo> objectInfos = new List<ObjectInfo>();
                        int count = 0;


                        //Search elements with 4D schedule information
                        NW.Search search = new NW.Search();
                        search.Selection.SelectAll();
                        //Search by property
                        search.SearchConditions.Add(NW.SearchCondition.HasPropertyByDisplayName("Element", "4D_Task_ID"));

                        // execute search
                        try
                        {
                            NW.ModelItemCollection items = search.FindAll(doc, true);
                            count = items.Count;

                            //MessageBox.Show(count.ToString());
                            foreach (NW.ModelItem item in items)
                            {
                                ObjectInfo objectInfo = new ObjectInfo();
                                objectInfo.objectName = item.DisplayName;

                                //Get element's 4D Task ID
                                objectInfo.objectTask = item.PropertyCategories.FindPropertyByDisplayName("Element", "4D_Task_ID").Value.ToInt32().ToString();
                                objectInfo.objectId = item.PropertyCategories.FindPropertyByDisplayName("Element", "Id").Value.ToInt32().ToString();
                                objectInfo.objectPhase = item.PropertyCategories.FindPropertyByDisplayName("Element", "4D_Task_Phase").Value.ToInt32().ToString();

                                objectInfos.Add(objectInfo);

                            }


                        }
                        catch (Exception ex)
                        {

                            MessageBox.Show(ex.Message);
                        }

                        //Initiate window
                        //ScheduleSetting schedule = new ScheduleSetting(objectInfos, doc, doc_TL, groupItem, count);
                        ScheduleSetting schedule = new ScheduleSetting(objectInfos, doc, doc_TL, groupItem, 100);
                        schedule.BeginInit();

                        Window window = new Window()
                        {
                            Content = schedule,
                            Name = "Schedule",
                            Title = "Schedule"
                        };

                        if (window.ShowDialog() == true)
                        {
                            //schedule.ScheduleResult();
                            schedule.CalculateValue();

                            MessageBox.Show("Window is ok");
                        }



                        break;
                    }
                case "ID_Button_2":
                    {
                        //AECOM.NavisWorks2018.Addins.Functions.GetCategories getCategories = new NavisWorks2018.Addins.Functions.GetCategories();
                        //var uniqueCategories = getCategories.categoryList;
                        //StringBuilder output = new StringBuilder();
                        //foreach (string s in uniqueCategories) output.AppendLine(s);
                        //MessageBox.Show(output.ToString());
                        ClashForm clashForm = new ClashForm();

                        clashForm.BeginInit();
                        Window window = new Window()
                        {
                            Content = clashForm,
                            Name = "ClashTestForm",
                            Title = "Clash Test Form"
                        };

                        if (window.ShowDialog() == true)
                        {
                            //schedule.ScheduleResult();


                            System.Windows.MessageBox.Show("Window is ok");
                        }

                        break;
                    }
                case "ID_Button_3":
                    {
                        //get current document
                        NW.Document doc = NW.Application.ActiveDocument;

                        SearchElements searchElements = new SearchElements(doc);

                        searchElements.BeginInit();
                        Window window = new Window()
                        {
                            Content = searchElements,
                            Name = "SearchElement",
                            Title = "Search Elements from Files"
                        };

                        if (window.ShowDialog() == true)
                        {
                            //schedule.ScheduleResult();


                            System.Windows.MessageBox.Show("Window is ok");
                        }

                        //AECOM.NavisWorks2018.Addins.Functions.CreateClashTest.createClashTest("Mechanical Pipes VS Mechanical Ducts", "Pipes", "Ducts");

                        break;
                    }
                case "ID_Button_4":
                    {
                        
                        //get current document
                        NW.Document doc = NW.Application.ActiveDocument;                      
                        NW.DocumentParts.IDocumentTimeliner tl = doc.Timeliner;
                        DocumentTimeliner doc_TL = (DocumentTimeliner)tl;
                        NW.GroupItem groupItem = doc_TL.TasksRoot.CreateCopy() as NW.GroupItem;


                        //init UI for operation

                        
                        CreateTaskWithGroups form = new CreateTaskWithGroups(doc, doc_TL, groupItem);
                       
                        form.BeginInit();

                        Window window = new Window()
                        {
                            Content = form,
                            Name = "Task",
                            Title = "Task with group",
                            Width = 450,
                            Height = 250
                        };

                        if (window.ShowDialog() == true)
                        {
                            
                            MessageBox.Show("Window is ok");
                        };
                        
                        break;
                    }
                case "ID_Button_5":
                    {
                        //Get the current document
                        NW.Document doc = NW.Application.ActiveDocument;
                        
                        //find items that has geometry
                        IEnumerable<NW.ModelItem> items = from x in doc.Models.RootItemDescendantsAndSelf
                                                          where x.HasGeometry
                                                          select x;
                        //Dictionary<string, HashSet<string>> all = new Dictionary<string, HashSet<string>>();
                        //var properties = new HashSet<string>();

                        
                        #region using for each to loop through model
                        int ind = 0;
                        foreach (NW.ModelItem item in items)
                        {

                            //item = items.ElementAt(i);
                            //string name = item.DisplayName +  "_" + i;
                            string itemProperties = "{ \"ID\": " + ind + ",";


                            //loop through elements to get information. 
                            foreach (NW.PropertyCategory pc in item.PropertyCategories)
                            {
                                if (pc.DisplayName == "ECPipeSystemData")
                                {

                                }
                                else
                                {
                                    string catProperties = "";
                                    catProperties = catProperties + "\"" + pc.DisplayName + "\": {";
                                    foreach (var p in pc.Properties)
                                    {

                                        //MessageBox.Show(pc.DisplayName);
                                        if (p.Value.IsDisplayString)
                                        {
                                            string cleanName = p.DisplayName.Replace('"', ' ');
                                            string cleanValue = p.Value.ToDisplayString().Replace('"', ' ');
                                            cleanValue = cleanValue.Replace(@"\", @"\\");
                                            //concatinate properties as string
                                            catProperties = catProperties + "\"" + cleanName + "\": " + "\"" + cleanValue + "\",";

                                        }
                                    }
                                    //catProperties.TrimEnd(',');
                                    if (!catProperties[catProperties.Length - 1].Equals('{'))
                                    {
                                        catProperties = catProperties.Substring(0, catProperties.Length - 1);

                                    }

                                    catProperties = catProperties + "},"; //drop last , 
                                                                          //MessageBox.Show(catProperties);
                                    itemProperties = itemProperties + catProperties;
                                }
                               
                                
                            }

                            //clean string for items properties
                            itemProperties = itemProperties.Substring(0, itemProperties.Length - 1);
                            itemProperties = itemProperties + "},";
                            double a = ind / 100000;
                            int fileNumber = Convert.ToInt32(Math.Ceiling(a));

                            //write the string to file     
                            using (StreamWriter streamWriter = new StreamWriter(@"C:\Users\xpeng\Desktop\DataFromNW\items_"+ fileNumber.ToString()+".txt", true))
                            {
                                streamWriter.WriteLine(itemProperties);
                            }

                            //add to index
                            ind += 1;

                        }

                        #endregion

                        #region using for to control the process. 
                        //for (int i = 0; i < 1000; i++)
                        //{


                        //    NW.ModelItem item = items.ElementAt(i+30900);
                        //    //string name = item.DisplayName +  "_" + i;
                        //    string itemProperties = "{ \"ID\": " + i + ",";


                        //    //loop through elements to get information. 
                        //    foreach (NW.PropertyCategory pc in item.PropertyCategories)
                        //    {
                        //        string catProperties = "";
                        //        catProperties = catProperties + "\"" + pc.DisplayName + "\": {";
                        //        foreach (var p in pc.Properties)
                        //        {

                        //            //MessageBox.Show(pc.DisplayName);
                        //            if (p.Value.IsDisplayString)
                        //            {
                        //                string cleanName = p.DisplayName.Replace('"', ' ');
                        //                string cleanValue = p.Value.ToDisplayString().Replace('"', ' ');
                        //                 cleanValue = cleanValue.Replace(@"\", @"\\");
                        //                //concatinate properties as string
                        //                catProperties = catProperties + "\"" + cleanName + "\": " + "\"" + cleanValue + "\",";

                        //            }
                        //        }
                        //        //catProperties.TrimEnd(',');
                        //        if (!catProperties[catProperties.Length - 1].Equals('{'))
                        //        {
                        //            catProperties = catProperties.Substring(0, catProperties.Length - 1);

                        //        }

                        //        catProperties = catProperties + "},"; //drop last , 
                        //        //MessageBox.Show(catProperties);
                        //        itemProperties = itemProperties + catProperties;
                        //    }


                        //    itemProperties = itemProperties.Substring(0, itemProperties.Length - 1);
                        //    itemProperties = itemProperties + "},";

                        //    //write the string to file     
                        //    using (StreamWriter streamWriter = new StreamWriter(@"C:\Users\xpeng\Desktop\DataFromNW\items.txt", true))
                        //    {
                        //        streamWriter.WriteLine(itemProperties);
                        //    }

                        //}
                        #endregion

                        //MessageBox.Show(all.Count.ToString());
                        //MessageBox.Show(items.Count().ToString());


                        break;
                    }
                case "ID_Button_6":
                    {
                        //Get the current document
                        NW.Document doc = NW.Application.ActiveDocument;

                        NW.ModelItemEnumerableCollection rootModels = doc.Models.RootItemDescendants;
                        int ind = 0;
                        
                        foreach (NW.ModelItem item in rootModels)
                        {
                            NW.ModelItem parentItem = item.Parent;
                            String parentID = parentItem == null ? null : parentItem.InstanceHashCode.ToString(); //return null if the item is root elements;
                            NW.ModelItemEnumerableCollection childItems = item.Children;//Children only get down one level, descendants go all the way to the root.
                            string guid = item.InstanceHashCode.ToString();
                            //MessageBox.Show("Children are: " + childItems.Count().ToString());

                            string displayName = item.DisplayName.Replace("\"", "inch");
                            String childList = "["; //initiate list to store all chilren hashcode
                            foreach (NW.ModelItem childItem in childItems)
                            {
                                //childList.Append(childItem.InstanceHashCode.ToString());
                                childList += childItem.InstanceHashCode.ToString()+ ",";

                            }
                            childList = childList.Length > 2? childList.Substring(0, childList.Length - 1) : childList;
                            childList += "]";//close the array for child item hashcode

                            string itemProperties = "{ \"ID\": " + ind + "," +
                                                        "\"Display Name\":\"" + displayName +"\","; //add ID to track exporting
                            itemProperties = itemProperties + "\"Parent ID\":" + parentID.ToString() + ","
                                                + "\"HashCode ID\":" + guid + "," 
                                                + "\"Children\":" + childList +","; //add parent Id, guid to properties

                            //Check duplication of category name
                            List<string> cat = new List<string>();
                            //get every property from current item
                            foreach (NW.PropertyCategory pc in item.PropertyCategories)
                            {
                                
                                if (pc.DisplayName == "ECPipeSystemData")
                                {

                                }
                                else
                                {
                                    string catProperties = "";
                                    string categoryName = pc.DisplayName;

                                    //Manage duplication of property category name
                                    int duplicateIndex = 1;
                                    if (cat.Contains(categoryName))
                                    {
                                        //Random random = new Random();
                                        //categoryName = categoryName + duplicateIndex.ToString() + random.Next().ToString();
                                        //cat.Add(categoryName);
                                        //duplicateIndex += 1;
                                    }
                                    else
                                    {
                                        cat.Add(categoryName);
                                        catProperties = catProperties + "\"" + categoryName + "\": {";
                                    foreach (var p in pc.Properties)
                                    {

                                        //MessageBox.Show(pc.DisplayName);
                                        if (p.Value.IsDisplayString)
                                        {
                                            string cleanName = p.DisplayName.Replace('"', ' ');
                                            string cleanValue = p.Value.ToDisplayString().Replace('"', ' ');
                                            cleanValue = cleanValue.Replace(@"\", @"\\");
                                            //concatinate properties as string
                                            catProperties = catProperties + "\"" + cleanName + "\": " + "\"" + cleanValue + "\",";

                                        }
                                    }
                                    //catProperties.TrimEnd(',');
                                    if (!catProperties[catProperties.Length - 1].Equals('{'))
                                    {
                                        catProperties = catProperties.Substring(0, catProperties.Length - 1);

                                    }

                                    catProperties = catProperties + "},"; //drop last , 
                                                                          //MessageBox.Show(catProperties);
                                    itemProperties = itemProperties + catProperties;

                                    }

                                    
                                }


                            }

                            //clean string for items properties
                            itemProperties = itemProperties.Substring(0, itemProperties.Length - 1);
                            itemProperties = itemProperties + "},";
                            double a = ind / 5000;
                            int fileNumber = Convert.ToInt32(Math.Ceiling(a));

                            //write the string to file     
                            using (StreamWriter streamWriter = new StreamWriter(@"F:\Navisworks\items_tree_" + fileNumber.ToString() + ".txt", true))
                            {
                                streamWriter.WriteLine(itemProperties);
                            }

                            //MessageBox.Show(itemProperties);
                            ind += 1;
                        }

                        


                        //find items that has geometry
                        IEnumerable<NW.ModelItem> items = from x in doc.Models.RootItemDescendantsAndSelf
                                                          where x.HasGeometry
                                                          select x;
                        MessageBox.Show(rootModels.Count().ToString());

                        break;
                    }
                case "ID_Button_7":
                    {
                        //Get the current document
                        //get current document
                        NW.Document doc = NW.Application.ActiveDocument;
                        NW.DocumentParts.IDocumentTimeliner tl = doc.Timeliner;
                        DocumentTimeliner doc_TL = (DocumentTimeliner)tl;
                        NW.GroupItem groupItem = doc_TL.TasksRoot.CreateCopy() as NW.GroupItem;
                        

                        NW.ModelItemEnumerableCollection rootModels = doc.Models.RootItemDescendants;

                        Dictionary<string, int> repeatMapName = new Dictionary<string, int>();

                        //string fileName = @"C:\Users\xpeng\Desktop\4D Model\FireDrill\DataMapping.csv";
                        string fileName = @"C:\Users\zongguan.wang\OneDrive - AECOM Directory\AECOM\JPM\JPM_Export\MOM\TimeSchedule_new.csv";
                        StreamReader streamReader = new StreamReader(fileName);
                        string strLine; // = streamReader.ReadLine();//skip header                               

                        //MessageBox.Show(strLine);
                       
                        while (!streamReader.EndOfStream)
                        {
                            strLine = streamReader.ReadLine();//read each line except for header
                                                              //define the serarator in CSV file
                                                              //string[] _task = strLine.Split(',');


                            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                            string[] _task = CSVParser.Split(strLine);



                           
                            
                            string startDate = _task[3];
                            string finishDate = _task[4];
                            string locationID = _task[8];
                            string mapName = _task[9];
                            //MessageBox.Show(_task[3]);
                            /*
                            foreach (string elementName in mapName)
                            {

                            }
                            */
                        
                           
;                            if (finishDate.Equals("Never reported"))
                            {

                            }
                            else
                            {
                                int countNumber = mapName.Length;
                                MessageBox.Show(countNumber.ToString());
                                
                                TimelinerTask task = new TimelinerTask();

                                //add property to task
                                task.DisplayName = mapName; //get display name as piece mark
                                task.DisplayId = mapName;
                                if (!string.IsNullOrEmpty(startDate))
                                {
                                    task.PlannedStartDate = Convert.ToDateTime(startDate);
                                    task.ActualStartDate = Convert.ToDateTime(startDate); //subtract as start date
                                }

                                if (!string.IsNullOrEmpty(finishDate))
                                {
                                    task.PlannedEndDate = Convert.ToDateTime(finishDate);
                                    task.ActualEndDate = Convert.ToDateTime(finishDate); //get installation date
                                }

                                Boolean testResult = _task[2].Equals("Not In Model");



                                
                                
                                task.DisplayId = _task[2];
                               
                                if (testResult == false)
                                {
                                    Int32 eleID = Convert.ToInt32(_task[2]);

                                    //Set task type
                                    task.SimulationTaskTypeName = "Construct";

                                    //select elements to add to task
                                    //Create search to find element
                                    NW.Search search = new NW.Search();
                                    search.Selection.SelectAll();

                                    //search.SearchConditions.Add(NW.SearchCondition.HasPropertyByDisplayName("Element ID", "Value").EqualValue(NW.VariantData.FromInt32(eleID)));
                                    search.SearchConditions.Add(NW.SearchCondition.HasPropertyByDisplayName("Element ID", "Value").EqualValue(NW.VariantData.FromDisplayString(_task[2])));

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
                                }
                                
                                

                                //Add task to timeliner



                                //groupItem.Children.Add(task);

                           }


                            doc_TL.TasksCopyFrom(groupItem.Children);

                            //System.Windows.MessageBox.Show(task.GetType().GetProperty(selProperty).GetValue(task).ToString());
                        
                        }


                        break;
                    }
                case "ID_Button_8":
                    {

                       
                        
                        List<string> GeoID = new List<string>();
                        List<string> LocationID = new List<string>();
                        List<string> ActivityID = new List<string>();
                        List<string> ActivityName = new List<string>();
                        List<string> Duration = new List<string>();
                        List<string> Start = new List<string>();
                        List<string> End = new List<string>();
                        List<string> Trade = new List<string>();
                        List<string> Block = new List<string>();
                        List<string> Zone = new List<string>();
                        List<string> Level = new List<string>();
                        List<string> Confirmation = new List<string>();
                        List<string> Intersection = new List<string>();
                        List<int> GeoIndex = new List<int>();
                        List<int> DateIndex = new List<int>();
                        


                        //List<DM> DataMangement = new List<DM>();
                        

                       

                        




                        NW.Document doc = NW.Application.ActiveDocument;
                        NW.ModelItemEnumerableCollection rootModels = doc.Models.RootItems;
                        NW.ModelItemEnumerableCollection rootModels_Geo = doc.Models.RootItemDescendants;
                        NW.ModelItemEnumerableCollection childItems;
                        foreach (NW.ModelItem item in rootModels)
                        {
                            //NW.ModelItem parentItem = item.Parent;
                            //String parentID = parentItem == null ? null : parentItem.InstanceHashCode.ToString(); //return null if the item is root elements;
                            childItems = item.Children;//Children only get down one level, descendants go all the way to the root.
                            
                            foreach (NW.ModelItem childItem in childItems)
                            {
                               
                                string displayName = childItem.DisplayName;
                                GeoID.Add(displayName);


                                //MessageBox.Show(displayName);

                            }
                            //var message = string.Join(Environment.NewLine, GeoID);
                            //MessageBox.Show(message);

                        }
                        


                        string fileName = @"C:\Users\zongguan.wang\OneDrive - AECOM Directory\AECOM\JPM\JPM_Export\MOM\ExcelToNW.csv";
                        StreamReader streamReader = new StreamReader(fileName);
                        string strLine; // = streamReader.ReadLine();//skip header


                        //read header and convert to array;

                       
                        string getheadLine = streamReader.ReadLine();
                        string[] MegaDataName = getheadLine.Split(',');
                        //MessageBox.Show(getheadLine);

                        while (!streamReader.EndOfStream)
                        {

                            strLine = streamReader.ReadLine();
                            Regex CSVParser = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
                            string[] _DM = CSVParser.Split(strLine);
                            DM dateManagement = new DM();

                            string ID = _DM[9]+".3dm";
                            string activityID=_DM[0];
                            string activityName = _DM[1];
                            string duration = _DM[2];
                            string start = _DM[3];
                            string end = _DM[4];
                            string trade = _DM[5];
                            string block = _DM[6];
                            string zone = _DM[7];
                            string level = _DM[8];
                            string confirmation = _DM[10];
                            //dateManagement.Start = _DM[3];
                            //dateManagement.Finish = _DM[4];
                            //dateManagement.Map_Name = _DM[9];
                            LocationID.Add(ID);
                            ActivityID.Add(activityID);
                            ActivityName.Add(activityName);
                            Duration.Add(duration);
                            Start.Add(start);
                            End.Add(end);
                            Trade.Add(trade);
                            Block.Add(block);
                            Zone.Add(zone);
                            Level.Add(level);
                            Confirmation.Add(confirmation);

                        }
                            //DataMangement=dateManagement.tolist
                            Intersection = LocationID.Intersect(GeoID).ToList();
                            //var message = string.Join(Environment.NewLine, Intersection);
                            //MessageBox.Show(message);
                            //DateTest.FindIndex(Intersection);
                            /*
                            foreach(string aa in Intersection)
                            {
                                int x = LocationID.IndexOf(aa);
                                int y = GeoID.IndexOf(aa);

                                //MessageBox.Show(aa);
                                //MessageBox.Show(Start[x]);
                                //MessageBox.Show(End[x]);
                                DateIndex.Add(x);
                                GeoIndex.Add(y);


                            }
                            */
                            //var message = string.Join(Environment.NewLine, GeoIndex);
                            //MessageBox.Show(message);


                        /*
                        if (doc.CurrentSelection.SelectedItems.Count > 0) ;
                        {
                            MessageBox.Show("good");

                            StringBuilder output = new StringBuilder(1000);
                            output.Append("Add properties 1");
                            NW.ModelItem allItem = doc.CurrentSelection.SelectedItems[0];
                            string itemName = allItem.DisplayName;
                            //MessageBox.Show(itemName);
                            foreach(NW.PropertyCategory pc in allItem.PropertyCategories)
                            {

                            }

                        }
                        */

                            ComApi.InwOpState10 state;
                            state = ComBridge.State;

                            // Get selection
                            //NW.ModelItemCollection modelItemCollectionIn = new NW.ModelItemCollection(doc.CurrentSelection.SelectedItems);
                            //NW.ModelItemCollection modelItemCollectionIn = new NW.ModelItemCollection(doc.);
                            int customProTabIndex = 1;


                        foreach (NW.ModelItem itemP in rootModels_Geo)
                        {
                            NW.ModelItem parentItem = itemP.Parent;

                            childItems = itemP.Children;
                            if (Intersection.Exists(x => x.Equals(itemP.DisplayName)))
                            {

                                int j = LocationID.IndexOf(itemP.DisplayName);




                                foreach (NW.ModelItem item in childItems)
                                {



                                //countNumber++;

                                


                                   







                                        // Get selection in COM
                                        //ComApi.InwOpSelection comSelectionOut = ComBridge.ToInwOpSelection(item);


                                        //ComApi.InwSelectionPathsColl oPaths = comSelectionOut.Paths();
                                        //ComApi.InwOaPath3 oPath = oPathcount;
                                        ComApi.InwOaPath oPath = ComBridge.ToInwOaPath(item);

                                        // Get properties collection of the path
                                        ComApi.InwGUIPropertyNode2 propn = (ComApi.InwGUIPropertyNode2)state.GetGUIPropertyNode(oPath, false);

                                        // Create new properties collection of path
                                        // New Tab
                                        ComApi.InwOaPropertyVec newPvec = (ComApi.InwOaPropertyVec)state.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOaPropertyVec, null, null);
                                        // New Properties

                                        //ComApi.InwOaProperty newP = (ComApi.InwOaProperty)state.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOaProperty, null, null);

                                        ComApi.InwGUIAttribute2 customTab = null;
                                        foreach (ComApi.InwGUIAttribute2 nwAtt in propn.GUIAttributes())
                                        {
                                            if (!nwAtt.UserDefined) continue;

                                            if (nwAtt.ClassUserName == "Construction Management")
                                            {
                                                customTab = nwAtt;
                                                break;
                                            }
                                            customProTabIndex += 1;
                                        }

                                        List<ComApi.InwOaProperty> objP = new List<ComApi.InwOaProperty>();

                                        for (int i = 0; i < MegaDataName.Length; i++)
                                        {
                                            ComApi.InwOaProperty newP = (ComApi.InwOaProperty)state.ObjectFactory(ComApi.nwEObjectType.eObjectType_nwOaProperty, null, null);
                                            //objP[i] = newP;
                                            //objP[i].name = "Para";
                                            //objP[i].UserName = "Para";
                                            newP.name = MegaDataName[i];
                                            newP.UserName = MegaDataName[i];
                                            if (i == 0)
                                            {
                                                newP.value = ActivityID[j];
                                            }
                                            if (i == 1)
                                            {
                                                newP.value = ActivityName[j];
                                            }
                                            if (i == 2)
                                            {
                                                newP.value = Duration[j];
                                            }
                                            if (i == 3)
                                            {
                                                newP.value = Start[j];
                                            }
                                            if (i == 4)
                                            {
                                                newP.value = End[j];
                                            }
                                            if (i == 5)
                                            {
                                                newP.value = Trade[j];
                                            }
                                            if (i == 6)
                                            {
                                                newP.value = Block[j];
                                            }
                                            if (i == 7)
                                            {
                                                newP.value = Zone[j];
                                            }
                                            if (i == 8)
                                            {
                                                newP.value = Level[j];
                                            }
                                            if (i == 9)
                                            {
                                                newP.value = LocationID[j];
                                            }
                                            if (i == 10)
                                            {
                                                newP.value = Confirmation[j];
                                            }



                                            newPvec.Properties().Add(newP);
                                            //count = i;
                                            //customTabIndex += 1;

                                        }











                                        if (customTab == null)
                                        {
                                            propn.SetUserDefined(0, "Construction Management", "Construction Management", newPvec);
                                        }
                                        else
                                        {
                                            propn.SetUserDefined(customProTabIndex, "Construction Management", "Construction Management", newPvec);
                                        }

                                    

                                }
                            } //propn.
                              
                        }
                            //propn.SetUserDefined(0, "Construction Management", "Construction Management", newPvec);

                            



                       

                            



                        break;
                    }
                default:
                    {
                        MessageBox.Show("This is a default message!");
                        break;
                    }
            }

            return 0;
        }

    }

    
    //[Plugin("XPAddins", "XP", DisplayName = "ESACM_Te")]
    //public class Application : AddInPlugin
    //{
    //    public override int Execute(params string[] parameters)
    //    {
    //        //MessageBox.Show("Hello", "Execute", MessageBoxButtons.OK, MessageBoxIcon.Information);
    //        Document doc = Autodesk.Navisworks.Api.Application.ActiveDocument;
    //        SearchElements searchElements = new SearchElements(doc);

    //        searchElements.BeginInit();
    //        Window window = new Window()
    //        {
    //            Content = searchElements,
    //            Name = "SearchElement",
    //            Title = "Search Elements from Files"
    //        };

    //        if (window.ShowDialog() == true)
    //        {
    //            //schedule.ScheduleResult();


    //            System.Windows.MessageBox.Show("Window is ok");
    //        }

    //        return 0;
    //    }

    //}

    public class DM
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
        public string location_ID { get; set; }
        public string Map_Name { get; set; }
        public string Type { get; set; }
    }


}
