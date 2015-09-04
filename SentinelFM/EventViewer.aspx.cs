using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.IO;
using VLF.CLS.Def;
using System.Web.Script.Serialization;
using VLF.DAS.Logic;

namespace SentinelFM
{
    public partial class EventViewer : SentinelFMBasePage
    {
        public string DefaultOrganizationHierarchyFleetId = "0";
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string OrganizationHierarchyPath = "";
        public string LoadVehiclesBasedOn = "fleet";
        public List<int> ScheduleTimeShowHide;
        string defaultnodecode = string.Empty;
        public string ScheventId;
        public bool MutipleUserHierarchyAssignment;
       
        JavaScriptSerializer serializer;
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                
                serializer = new JavaScriptSerializer();
                LoadVehiclesBasedOn = sn.User.LoadVehiclesBasedOn.ToString();

                if (LoadVehiclesBasedOn == "hierarchy")
                {
                    string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                    VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                    if (!poh.HasOrganizationHierarchy(sn.User.OrganizationId))
                        LoadVehiclesBasedOn = "fleet";
                }

                if (LoadVehiclesBasedOn == "hierarchy")
                {
                    ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();
                    string xml = "";
                    if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), false))
                        if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), true))
                        {
                            LoadVehiclesBasedOn = "fleet";
                            return;
                        }
                    StringReader strrXML = new StringReader(xml);
                    DataSet dsPref = new DataSet();
                    dsPref.ReadXml(strrXML);
                    defaultnodecode = dsPref.Tables[0].Select("PreferenceName='DefaultOrganizationHierarchyNodeCode'")[0]["PreferenceValue"].ToString().TrimEnd();
                }

                if (LoadVehiclesBasedOn == "hierarchy")
                {
                    string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                    VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                    defaultnodecode = defaultnodecode ?? string.Empty;
                    if (defaultnodecode == string.Empty)
                        defaultnodecode = poh.GetRootNodeCode(sn.User.OrganizationId);
                    DefaultOrganizationHierarchyNodeCode = defaultnodecode;
                    DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode).ToString();
                    DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, int.Parse(DefaultOrganizationHierarchyFleetId));
                    OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, defaultnodecode);
                    MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");
                    if (MutipleUserHierarchyAssignment)
                    {
                        DefaultOrganizationHierarchyFleetId = sn.User.PreferFleetIds;
                        DefaultOrganizationHierarchyNodeCode = sn.User.PreferNodeCodes;
                        if (DefaultOrganizationHierarchyFleetId.Trim() == string.Empty)
                            DefaultOrganizationHierarchyFleetName = "";
                        else if (DefaultOrganizationHierarchyFleetId.Contains(','))
                            DefaultOrganizationHierarchyFleetName = "Multiple Hierarchies";
                        else
                            DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, int.Parse(DefaultOrganizationHierarchyFleetId));
                    }
                }

                //getEvents ID from feature permission xls
                ScheduleTimeShowHide = clsPermission.GetFeaturePermissionData(sn, "EventIdForShowHideScheduleTime");
                ScheventId = serializer.Serialize(ScheduleTimeShowHide);
                var volCol = sn.User.ViolationColumns;
                if((volCol == null) || (volCol == ""))
                {
                    string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                    EvtEvents eevent = new EvtEvents(sConnectionString);
                    DataSet dsPref = eevent.GetEventViolationColumns(sn.UserID, "ViolationColumns");
                    if(dsPref != null)
                    {
                        foreach (DataRow rowItem in dsPref.Tables[0].Rows)
                        {
                            if (rowItem["PreferenceRule"].ToString().TrimEnd() != "")
                                sn.User.ViolationColumns = Convert.ToString(rowItem["PreferenceRule"].ToString().TrimEnd());
                        }
                    }
                    
                }
                if (String.IsNullOrEmpty(sn.User.ViolationColumns))
                {
                    sn.User.ViolationColumns = "1,2,3,4,5,6,7,8,9,10,11";
                }

                var eventCol= sn.User.EventColumns;                
                if ((eventCol == null) || (eventCol == ""))
                {
                    string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                    EvtEvents eevent = new EvtEvents(sConnectionString);
                    DataSet dsPref = eevent.GetEventViolationColumns(sn.UserID, "EventColumns");
                    if (dsPref != null)
                    {
                        foreach (DataRow rowItem in dsPref.Tables[0].Rows)
                        {
                            if (rowItem["PreferenceRule"].ToString().TrimEnd() != "")
                                sn.User.EventColumns = Convert.ToString(rowItem["PreferenceRule"].ToString().TrimEnd());
                        }
                    }

                }
                if (String.IsNullOrEmpty(sn.User.EventColumns))
                {
                    sn.User.EventColumns = "1,2,3,4,5,6,7,8,9,10,11";
                }
               
                var RecordsToFetch = sn.User.RecordsToFetch;
                if ((RecordsToFetch == null) || (RecordsToFetch == ""))
                {
                    string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                    EvtEvents eevent = new EvtEvents(sConnectionString);
                    DataSet dsPreference = eevent.GetEventViolationRecordsToFetch(sn.UserID, "RecordsToFetch");
                    if (dsPreference != null)
                    {
                        foreach (DataRow rowItem in dsPreference.Tables[0].Rows)
                        {
                            if (rowItem["PreferenceRule"].ToString().TrimEnd() != "")
                                sn.User.RecordsToFetch = Convert.ToString(rowItem["PreferenceRule"].ToString().TrimEnd());
                        }
                    }

                }
                if (String.IsNullOrEmpty(sn.User.RecordsToFetch))
                {
                    sn.User.RecordsToFetch = "5000";
                }
            }
            catch
            {
                LoadVehiclesBasedOn = "fleet";
                return;
            }
        }
    }
}