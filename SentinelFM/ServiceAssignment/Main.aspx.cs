using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using SentinelFM.GeomarkServiceRef;
using SentinelFM.ServerDBFleet;
using SentinelFM;
using SentinelFM.ServerDBOrganization;
using SentinelFM.ServerDBVehicle;
using VLF.DAS.Logic;

public partial class ServiceAssignment_Main : System.Web.UI.Page
{
    protected SentinelFMSession sn;
    private DBFleet dbf = null;
    private DBOrganization dbo = null;
    private DBVehicle dbv = null;
    private ListBox FromLists = null;    
    public IList<Dictionary<string, string>> Rules = null;
    public IList<Dictionary<string, string>> ConfiguredServices = null;
    protected int ServiceConfigId = 0;
    protected int ServiceId = 0;
    protected int SelectedConfiguredServiceId = 0;
    private string target = null;
    public int TabOrder = 0;
    public int TabWidth = 400;
    public int AssignmentWidth = 450;
    
    protected void Page_Init(object sender, EventArgs e)
    {        
        try
        {            
            dbf = new DBFleet();
            dbo = new DBOrganization();
            dbv = new DBVehicle();

        }
        catch (Exception exception)
        {
            
            throw new Exception(exception.StackTrace);
        }
    }
    
    protected void Page_Load(object sender, EventArgs e)
    {        
	btnSaveConfig.Visible = false;
        sn = (SentinelFMSession)Session["SentinelFMSession"];
        int organizationId = sn.User.OrganizationId;
        if (organizationId == 0)
        {
            Response.Redirect("../Login.aspx");
        }        
        if (ViewState["Object"] != null)
        {
            if (ViewState["Object"].Equals("Fleets"))
            {
                FromLists = FleetsVehicleList;
                target = "Fleet";
            }
            else
            {
                FromLists = ObjectsList;
                if (ViewState["Object"].Equals("Landmarks"))
                {
                    target = "Landmark";
                }
                else
                {
                    target = "Vehicle";
                }
            }
        }
        IncludeList.SelectionMode = ListSelectionMode.Multiple;
        if (BaseRules_0.Items.Count > 0)
        {
            BaseRules_0.Items.Clear();
        }

        if (!string.IsNullOrEmpty(ServicesList.SelectedValue))
        {
            ServiceId = Convert.ToInt32(ServicesList.SelectedValue);
        }
        if (!IsPostBack)
        {
            if (ServicesList.Items.Count > 0)
            {
                ServicesList.Items.Clear();
            }

            Dictionary<string, string> services = ServiceAssignment.GetServices(organizationId);
            foreach (KeyValuePair<string, string> rawRule in services)
            {
                ListItem listItem = new ListItem(rawRule.Value, rawRule.Key);
                ServicesList.Items.Add(listItem);
            }
        }
        else
        {
            string action = Request.Form["action"] ?? "0";
            if (!string.IsNullOrEmpty(Request.Form["expression"]) && action.Equals("Save"))
            {
                DateTime? expiredD = null;
                if (!string.IsNullOrEmpty(Request.Form["expiredDate"]))
                {
                    expiredD = Convert.ToDateTime(Request.Form["expiredDate"]);
                }

                int isActive = 1;
                if (Request.Form["chkActive"] == null)
                {
                    isActive = 0;
                }
		int isReportable = 1;
                if (Request.Form["chkReportable"] == null)
                {
                    isReportable = 0;
                }
                ServiceConfigId = ServiceAssignment.SaveServiceConfig(Request.Form["expression"], Request.Form["ruleName"],
                    Convert.ToInt32(Request.Form["ServiceId"]), sn.User.OrganizationId, sn.UserID, expiredD, Convert.ToInt32(!string.IsNullOrEmpty(Request.Form["ServiceConfigId"])  ? Request.Form["ServiceConfigId"]: "0"), Convert.ToInt32(string.IsNullOrEmpty(Request.Form["deleteService"]) ? "0" : Request.Form["deleteService"]) > 0, isActive, isReportable);
                if (ServiceConfigId > 0)
                {
                    string emailLevel = null;
                    emailLevel += Request.Form["VehicleLevelEmail"] ?? null;
                    emailLevel += Request.Form["LandmarkLevelEmail"] ?? null;
                    emailLevel += Request.Form["FleetCriticalEmail"] ?? null;
                    emailLevel += Request.Form["FleetWarningEmail"] ?? null;
                    emailLevel += Request.Form["FleetNotifyEmail"] ?? null;
                    emailLevel += Request.Form["FleetAllEmail"] ?? null;

                    string messageBody = Request.Form["txtMessageBody"];
                    ServiceAssignment.SaveNotificationConfig(ServiceConfigId, Request.Form["txtEmailsList"], emailLevel, Request.Form["txtSubject"], messageBody);
                    UpdateCache();
                }
               
                TabOrder = 1;
            }

            if (ServiceId > -1)
            {
                Rules = ServiceAssignment.GetRules(ServiceId);
                ListItem nullListItem = new ListItem("", "");
                BaseRules_0.Items.Add(nullListItem);
                foreach (Dictionary<string, string> rule in Rules)
                {
                    ListItem listItem = new ListItem(rule["RuleName"], rule["RuleName"]);
                    if (rule["RuleName"].Contains("PROP-"))
                    {
                        listItem = new ListItem(string.Format("{0}[{1}]", rule["RuleName"], rule["ToolTip"]), rule["RuleName"]);
                        AssignmentWidth = 500;
                    }                    
                    
                    BaseRules_0.Items.Add(listItem);
                }
                BaseRules_0.DataBind();
            }     
                    
        }
        string selectedRuleValue = RulesList.SelectedValue;
        SelectedConfiguredServiceId = Convert.ToInt32(!string.IsNullOrEmpty(selectedRuleValue) ? selectedRuleValue : "0");
        if (RulesList.Items.Count > 0)
        {
            RulesList.Items.Clear();
        }
        ConfiguredServices = ServiceAssignment.GetConfiguredServices(ServiceId, organizationId, sn.UserID, null);
        if (ConfiguredServices != null)
        {
            if (ConfiguredServices.Count > 0)
            {                
                foreach (Dictionary<string, string> configuredService in ConfiguredServices)
                {
                    ListItem listItem = new ListItem(configuredService["ServiceConfigName"], configuredService["ServiceConfigID"]);
                    if (configuredService["ServiceConfigID"].Equals(selectedRuleValue))
                    {
                        listItem.Selected = true;
                    }
                    RulesList.Items.Add(listItem);
                }
                RulesList.DataBind();
            }
        }       
        /***/
    }

    private IList<Dictionary<string, string>> ParseXml(string myXmlString, string searchSection, IList<string> searchNodes)
    {        
        List<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
        try
        {
            XmlDocument xml = new XmlDocument();
            xml.LoadXml(myXmlString); // suppose that myXmlString contains "<Names>...</Names>"
            XmlNodeList xnList = xml.GetElementsByTagName(searchSection);
            foreach (XmlNode sNode in xnList)
            {
                try
                {
                    
                    if (searchNodes != null)
                    {
                        Dictionary<string, string> result = new Dictionary<string, string>();
                        foreach (string nodeName in searchNodes)
                        {
                            result.Add(nodeName, (sNode[nodeName] != null ? sNode[nodeName].InnerText.Trim() : null));
                        }
                        results.Add(result);
                    }
                   
                }
                catch (Exception exception)
                {
                    Dictionary<string, string> result = new Dictionary<string, string>();
                    result.Add("ERROR", exception.Message);
                    results.Add(result);
                }

            }
        }
        catch (Exception exception)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            result.Add("ERROR", exception.Message);
            results.Add(result);
        }
        return results;
    }
    protected void AddVehiclesAction(object sender, EventArgs e)
    {
        if (dbf == null)
        {
            return;
        }
	btnSaveConfig.Visible = true;
        try
        {
            if (string.IsNullOrEmpty(ObjectsList.SelectedValue))
            {
                return;
            }
            if (ViewState["Object"].Equals("Vehicles"))
            {
                MoveItems(FromLists, IncludeList, true);
            }           
            IncludeList.DataBind();
            TabOrder = 2;
            TabWidth = 600;
        }
        catch (Exception exception)
        {

            throw new Exception(exception.StackTrace);
        }
    }
    protected void ExcludeAction(object sender, EventArgs e)
    {
        if (FromLists == null)
        {
            return;
        }       
        MoveItems(FromLists, ExcludeList, false);
        TabOrder = 2;
        if (Convert.ToBoolean(ViewState["HierarchyView"]))
        {
            TabWidth = 1350;
        }
        else
        {
            TabWidth = 1200;
        }
	btnSaveConfig.Visible = true;
    }


    protected void AssignToFleets(object sender, EventArgs e)
    {
        try
        {
            ViewState["HierarchyView"] = false;
            if (ObjectsList.Items.Count > 0)
            {
                ObjectsList.Items.Clear();
            }
            if (FleetsVehicleList.Items.Count > 0)
            {
                FleetsVehicleList.Items.Clear();
            }
            CleanUpBox();
            ViewState["Object"] = "Fleets";
            showVehicles.Visible = false;
            landmarksPlaceholder.Visible = false;
            showFleets.Visible = true;            
            showFleetsVehiclePlaceholder.Visible = true;
            showIncludeList.Visible = true;
            showExclude.Visible = true;
            IncludesListTitlePlaceholder.Visible = true;
            FleetVehiclesTitlePlaceholder.Visible = true;
            ExcludesListTitlePlaceholder.Visible = true;            
            IncludesListLabel.Text = "Included Fleets";
            ObjectListLabel.Text = "All Fleets List";
            string xml = "";
            dbf.GetFleetsInfoXMLByUserId(sn.UserID, sn.SecId, ref xml);            
            IList<Dictionary<string, string>> fleets = ParseXml(xml, "FleetsInformation", new string[] { "FleetId", "FleetName" });
            foreach (var fleet in fleets)
            {
                ListItem item = new ListItem();
                item.Text = fleet["FleetName"];
                item.Value = fleet["FleetId"];
                ObjectsList.Items.Add(item);
            }
            ObjectsList.SelectionMode = ListSelectionMode.Multiple;
            ObjectsList.DataBind();
            DisplayAssignedAndExcludeVehicles();
            TabOrder = 2;
            if (showExclude.Visible)
            {
                TabWidth = 1200;
            }
            else
            {
                TabWidth = 800;    
            }            
        }
        catch (Exception exception)
        {

            throw new Exception(exception.StackTrace);
        }
    }
    protected void AssignToLandmarks(object sender, EventArgs e)
    {
        try
        {
            ViewState["HierarchyView"] = false;
            if (ObjectsList.Items.Count > 0)
            {
                ObjectsList.Items.Clear();
            }
            CleanUpBox();
            ViewState["Object"] = "Landmarks";
            showVehicles.Visible = false;
            landmarksPlaceholder.Visible = true;
            showFleets.Visible = false;
            showIncludeList.Visible = true;
            IncludesListTitlePlaceholder.Visible = true;
            FleetVehiclesTitlePlaceholder.Visible = false;
            ExcludesListTitlePlaceholder.Visible = false;
            showFleetsVehiclePlaceholder.Visible = false;
            showExclude.Visible = false;
            ObjectListLabel.Text = "All Landmarks List";
            IncludesListLabel.Text = "Included Landmarks";
            string xml = "";
            dbo.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId,sn.User.OrganizationId, ref xml);
            IList<Dictionary<string, string>> landmarks = ParseXml(xml, "LandmarksInformation", new string[] { "LandmarkName", "LandmarkId" });
            foreach (Dictionary<string, string> landmark in landmarks)
            {
                ListItem item = new ListItem();
                item.Text = landmark["LandmarkName"];
                item.Value = landmark["LandmarkId"];
                ObjectsList.Items.Add(item);
            }
            ObjectsList.SelectionMode = ListSelectionMode.Multiple;
            ObjectsList.DataBind();
            DisplayAssignedLandmarks();
            TabOrder = 2;
            TabWidth = 600;
        }
        catch (Exception exception)
        {
            throw new Exception(exception.StackTrace);
        }
    }

    protected void AddFleetsAction(object sender, EventArgs e)
    {
        if (ViewState["triggered"] != null)
        {
            if (ViewState["triggered"].Equals("Vehicles"))
            {
                CleanUpBox();
            }
        }        
        showExclude.Visible = true;
        if (Convert.ToBoolean(ViewState["HierarchyView"]))
        {
            MoveItemsFromHierarchyBox(IncludeList);
            TabWidth = 1350;
        }
        else
        {
            MoveItems(ObjectsList, IncludeList, true);
            TabWidth = 1200;
        }        
        ViewState["triggered"] = "Fleets";
        TabOrder = 2;
        btnSaveConfig.Visible = true;
    }

    protected void RemoveFleetsAction(object sender, EventArgs e)
    {
        showExclude.Visible = false;
        ExcludesListTitlePlaceholder.Visible = false;
        if (Convert.ToBoolean(ViewState["HierarchyView"]))
        {
            MoveHierarchyItemOutIncludeList(IncludeList);
            TabWidth = 950;
        }
        else
        {
            MoveItems(IncludeList, ObjectsList, false);
            TabWidth = 800;
        }
       
        ViewState["triggered"] = "Fleets";
        TabOrder = 2;
        btnSaveConfig.Visible = true;
    }

    private void MoveItems(ListBox from, ListBox to, bool copy)
    {        
        try
        {
            if (from.GetSelectedIndices().Count() > 0)
            {
                IList<ListItem> beingRemovedList = new List<ListItem>();
                foreach (int selectedIndex in from.GetSelectedIndices())
                {

                    ListItem selectedItem = from.Items[selectedIndex];
                    beingRemovedList.Add(selectedItem);                                                    
                }

                foreach (ListItem listItem in beingRemovedList)
                {
                    if (!copy)
                    {
                        from.Items.Remove(listItem);
                    }                    
                    if (!to.Items.Contains(listItem))
                    {
                        to.Items.Add(listItem);
                    }   
                }
                to.SelectionMode = ListSelectionMode.Multiple;
                to.DataBind();
            }
        }
        catch (Exception exception)
        {
            
            throw new Exception(exception.StackTrace);
        }
    }

    private void MoveItemsFromHierarchyBox(ListBox to)
    {
        ListItem listItem = new ListItem();
        listItem.Value = HierarchyTree.Field_OrganizationHierarchyFleetId;
        listItem.Text = HierarchyTree.Field_OrganizationHierarchyFleetName;
        to.Items.Add(listItem);
        
        to.SelectionMode = ListSelectionMode.Multiple;
        to.DataBind();
    }

    public void MoveHierarchyItemOutIncludeList(ListBox from)
    {
        foreach (int selectedIndex in from.GetSelectedIndices())
        {

            ListItem selectedItem = from.Items[selectedIndex];
            from.Items.Remove(selectedItem);
        }
    }

    private void CleanUpBox()
    {       
        if (IncludeList.Items.Count > 0)
        {
            IncludeList.Items.Clear();
        }
        if (ExcludeList.Items.Count > 0)
        {
            ExcludeList.Items.Clear();
        }
    }
    protected void ServicesList_SelectedIndexChanged(object sender, EventArgs e)
    {
        CleanUpBox();
        if (ObjectsList.Items.Count > 0)
        {
            ObjectsList.Items.Clear();    
        }
        showVehicles.Visible = false;
        landmarksPlaceholder.Visible = false;
        showFleets.Visible = false;
        showIncludeList.Visible = false;
        IncludesListTitlePlaceholder.Visible = false;
        FleetVehiclesTitlePlaceholder.Visible = false;
        ExcludesListTitlePlaceholder.Visible = false;
        showFleetsVehiclePlaceholder.Visible = false;
        showExclude.Visible = false;
        TabOrder = 1;
    }    

    protected void AssignToVehicles(object sender, EventArgs e)
    {
        ViewState["HierarchyView"] = false;
        if (ObjectsList.Items.Count > 0)
        {
            ObjectsList.Items.Clear();
        }
        CleanUpBox();        
        string fleetXml = null;
        dbf.GetFleetsInfoXMLByUserId(sn.UserID, sn.SecId, ref fleetXml);
        IList<Dictionary<string, string>> fleets = ParseXml(fleetXml, "FleetsInformation", new string[] { "FleetId", "FleetName" });
        int allFleetsId = 0;
        foreach (var fleet in fleets)
        {
            if (fleet["FleetName"].Equals("All Vehicles") || fleet["FleetName"].Equals("Tous les véhicules"))
            {
                allFleetsId = Convert.ToInt32(fleet["FleetId"]);
                break;
            }
        }

        if (allFleetsId > 0)
        {
            string vehicleXml = null;
            dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, allFleetsId, ref vehicleXml);
            IList<Dictionary<string, string>> vehicles = ParseXml(vehicleXml, "VehiclesInformation",
                                                                         new string[] { "VehicleId", "Description" });
            CleanUpBox();
            showVehicles.Visible = true;            
            showExclude.Visible = false;
            showFleets.Visible = false;            
            landmarksPlaceholder.Visible = false;
            showFleetsVehiclePlaceholder.Visible = false;
            showIncludeList.Visible = true;
            IncludesListTitlePlaceholder.Visible = true;
            FleetVehiclesTitlePlaceholder.Visible = false;
            ExcludesListTitlePlaceholder.Visible = false;
            ObjectListLabel.Text = "All Vehicles";
            IncludesListLabel.Text = "Assigned Vehicles";
            ListItem listItem = new ListItem();
            foreach (var vehicle in vehicles)
            {
                ListItem item = new ListItem();
                item.Text = vehicle["Description"];
                item.Value = vehicle["VehicleId"];
                if (!ObjectsList.Items.Contains(item))
                {
                    ObjectsList.Items.Add(item);
                }
            }
            ViewState["Object"] = "Vehicles";
        }

        DisplayAssignedVehicles();
        TabOrder = 2;
        TabWidth = 600;
    }

    protected void DeleteIncludedVehiclesAction(object sender, EventArgs e)
    {
        if (IncludeList.Items.Count > 0)
        {
            MoveItems(IncludeList, FromLists, false);
        }
        TabOrder = 2;
        TabWidth = 600;
	btnSaveConfig.Visible = true;
    }

    protected void ShowFleetsVehiclesAction(object sender, EventArgs e)
    {        
        showExclude.Visible = true;
        FleetVehiclesTitlePlaceholder.Visible = true;
        ExcludesListTitlePlaceholder.Visible = true;
        FromLists = IncludeList;
        if (FleetsVehicleList.Items.Count > 0)
        {
            FleetsVehicleList.Items.Clear();
        }

        foreach (int selectedIndex in IncludeList.GetSelectedIndices())
        {
            string xml = "";
            dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, Convert.ToInt32(IncludeList.Items[selectedIndex].Value), ref xml);
            if (!string.IsNullOrEmpty(xml))
            {
                IList<Dictionary<string, string>> vehicles = ParseXml(xml, "VehiclesInformation",
                                                                      new string[] { "VehicleId", "Description" });
                ListItem listItem = new ListItem();
                foreach (var vehicle in vehicles)
                {
                    ListItem item = new ListItem();
                    item.Text = vehicle["Description"];
                    item.Value = vehicle["VehicleId"];
                    if (!FleetsVehicleList.Items.Contains(item))
                    {
                        FleetsVehicleList.Items.Add(item);
                    }
                }
                //IncludeList.SelectionMode = ListSelectionMode.Multiple;                
                FleetsVehicleList.DataBind();
                FromLists = FleetsVehicleList;
            }
        }

        if (Convert.ToBoolean(ViewState["HierarchyView"]))
        {
            TabWidth = 1350;
        }
        else
        {
            TabWidth = 1200; 
        }
        TabOrder = 2;
        btnSaveConfig.Visible = true;
    }  
  
    protected void RemoveExcludeAction(object sender, EventArgs e)
    {
        if (ExcludeList.Items.Count > 0)
        {
            MoveItems(ExcludeList, FromLists, false);
        }
        TabOrder = 2;
        if (Convert.ToBoolean(ViewState["HierarchyView"]))
        {
            TabWidth = 1350;
        }
        else
        {
            TabWidth = 1200;
        }
	btnSaveConfig.Visible = true;
    }

    protected void AddLandmarksAction(object sender, EventArgs e)
    {
        if (ObjectsList.Items.Count > 0)
        {
            MoveItems(ObjectsList, IncludeList, true);
        }
        TabOrder = 2;
        TabWidth = 600;
	btnSaveConfig.Visible = true;
    }

    protected void RemoveLandmarksAction(object sender, EventArgs e)
    {
        if (IncludeList.Items.Count > 0)
        {
            MoveItems(IncludeList, ObjectsList, false);
        }
        TabOrder = 2;
        TabWidth = 600;
	btnSaveConfig.Visible = true;
    }

    protected void SaveConfigAction(object sender, EventArgs e)
    {
        ServiceAssignment.SaveObjects(SelectedConfiguredServiceId, target, GetListFromListBox(IncludeList), GetListFromListBox(ExcludeList), DateTime.Now, false);
        UpdateCache();
    }

    private void UpdateCache()
    {        
        try
        {
            SentinelFM.GeomarkServiceRef.GeomarkServiceClient geomarkServiceClient = new GeomarkServiceClient("httpbasic");
            geomarkServiceClient.ReleaseCache();
        }
        catch (Exception exception)
             {
            
            throw new Exception(exception.StackTrace);
        }
    }

    private IList<int> GetListFromListBox(ListBox listBox)
    {
        if (listBox.Items.Count < 1)
        {
            return null;
        }
        IList<int> lists = new List<int>();
        foreach (ListItem item in listBox.Items)
        {
            lists.Add(Convert.ToInt32(item.Value));
        }
        return lists;
    }

    private DateTime GetSelectedConfiguredCreatedDateTime()
    {
        if (SelectedConfiguredServiceId < 1)
        {
            return DateTime.Now;
        }
        foreach (Dictionary<string, string> configuredService in ConfiguredServices)
        {
            if (configuredService["ServiceConfigID"].Equals(Convert.ToString(SelectedConfiguredServiceId)))
            {
                return Convert.ToDateTime(configuredService["CreatedDate"]);
            }
        }
        return DateTime.Now;
    }

    private void DisplayAssignedAndExcludeVehicles()
    {
        Dictionary<string, IList<int>> results =
            ServiceAssignment.GetSelectedFleetsAndVehicles(SelectedConfiguredServiceId, "Fleet");
        if (results.ContainsKey("Include"))
        {
            IncludeList.Visible = true;
            foreach (int fleetId in results["Include"])
            {
                string xml = null;
                dbf.GetFleetInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml);
                if (string.IsNullOrEmpty(xml))
                {
                    continue;
                }
                IList<Dictionary<string, string>> fleetInfo = ParseXml(xml, "FleetInformation", new string[] { "FleetId", "FleetName" });
                ListItem fListItem = new ListItem();
                fListItem.Text = fleetInfo[0]["FleetName"];
                fListItem.Value = fleetInfo[0]["FleetId"];
                if (!IncludeList.Items.Contains(fListItem))
                {
                    IncludeList.Items.Add(fListItem);
                }
            }
            IncludeList.DataBind();
        }
        if (results.ContainsKey("Exclude"))
        {
            showExclude.Visible = true;
            ExcludesListTitlePlaceholder.Visible = true;
            foreach (int vehicleId in results["Exclude"])
            {
                string xmlVehicle = null;
                dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, vehicleId, ref xmlVehicle );
                if (string.IsNullOrEmpty(xmlVehicle))
                {
                    continue;
                }
                IList<Dictionary<string, string>> vehicles = ParseXml(xmlVehicle, "VehicleInformation",
                                                                         new string[] { "VehicleId", "Description" });

                ListItem fListItem = new ListItem();
                fListItem.Text = vehicles[0]["Description"];
                fListItem.Value = Convert.ToString(vehicleId);
                if (!ExcludeList.Items.Contains(fListItem))
                {
                    ExcludeList.Items.Add(fListItem);
                }
            }
            ExcludeList.DataBind();
        }
        
        
    }

    private void DisplayAssignedVehicles()
    {
        Dictionary<string, IList<int>> results =
           ServiceAssignment.GetSelectedFleetsAndVehicles(SelectedConfiguredServiceId, "Vehicle");
        if (results.ContainsKey("Include"))
        {
            IncludeList.Visible = true;
            foreach (int vehicleId in results["Include"])
            {
                string xml = null;
                dbv.GetVehicleInfoXMLByVehicleId(sn.UserID, sn.SecId, vehicleId, ref xml);
                if (string.IsNullOrEmpty(xml))
                {
                    continue;
                }
                IList<Dictionary<string, string>> fleetInfo = ParseXml(xml, "VehicleInformation", new string[] { "VehicleId", "Description" });
                ListItem fListItem = new ListItem();
                fListItem.Text = fleetInfo[0]["Description"];
                fListItem.Value = fleetInfo[0]["VehicleId"];
                if (!IncludeList.Items.Contains(fListItem))
                {
                    IncludeList.Items.Add(fListItem);
                }
            }
            IncludeList.DataBind();
        }
    }

    private void DisplayAssignedLandmarks()
    {
        Dictionary<string, IList<int>> results =
           ServiceAssignment.GetSelectedFleetsAndVehicles(SelectedConfiguredServiceId, "Landmark");
        if (results.ContainsKey("Include"))
        {
            IncludeList.Visible = true;
            IDictionary<int, string> myItems = new Dictionary<int, string>();
            foreach (int landmarkId in results["Include"])
            {
                if (!myItems.ContainsKey(landmarkId))
                {
                    string lname = ServiceAssignment.GetLandmarkNameById(landmarkId);
                    myItems.Add(landmarkId, lname);
                }
            }
            //myItems = myItems.OrderBy(x => x.Value).ToDictionary(x=>x.Key, y=>y.Value);
            var myItems1 = (from dic in myItems orderby dic.Value ascending select dic);
            foreach (KeyValuePair<int, string> item in myItems1)
            {
                ListItem fListItem = new ListItem();
                fListItem.Text = item.Value;
                fListItem.Value = Convert.ToString(item.Key);
                if (!IncludeList.Items.Contains(fListItem))
                {
                    IncludeList.Items.Add(fListItem);
                }
            }
            IncludeList.DataBind();
        }

        
    }
    protected void SwitchToHierarchy(object sender, EventArgs e)
    {        
        TabOrder = 2;
        TabWidth = 1350;
        ViewState["HierarchyView"] = true;
    }

    protected void SwitchToList(object sender, EventArgs e)
    {
        TabOrder = 2;
        TabWidth = 1200;
        ViewState["HierarchyView"] = false;
    }
}