using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Configuration;
using VLF.CLS.Def;
using Com.Mapsolute.Webservices.MapletRemoteControl;

namespace SentinelFM
{
    public partial class Widgets_FleetAssignment : System.Web.UI.Page
    {
        public SentinelFMSession sn = null;

        VLF.PATCH.Logic.PatchLandmark _landmark;
        VLF.PATCH.Logic.PatchOrganizationGeozone _geozone;
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        string objectName;
        int objectId;


        protected void Page_Load(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            _landmark = new VLF.PATCH.Logic.PatchLandmark(sConnectionString);
            objectName = Request["objectName"] ?? string.Empty;
            objectId = Int32.Parse(Request["objectId"] ?? "0");
            objectName = objectName.ToLower();

            if (objectName == "geozoneid")
            {
                _geozone = new VLF.PATCH.Logic.PatchOrganizationGeozone(sConnectionString);
                objectName = "geozone";
                objectId = (Int32)_geozone.PatchGetGeozoneNoByGeozoneId(sn.User.OrganizationId, objectId);
            }

            if (!Page.IsPostBack)
            {
                FleetAssignment_Fill();
            }
        }

        private void FleetAssignment_Fill()
        {
            try
            {
                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                
                DataView FleetView = dsFleets.Tables[0].DefaultView;
                FleetView.RowFilter = "FleetType<>'oh'";
                DataSet ds = new DataSet();
                DataTable dt = FleetView.ToTable();
                ds.Tables.Add(dt);
                ds.DataSetName = "Fleet";

                for (int i = ds.Tables[0].Rows.Count - 1; i >= 0; i--)
                {
                    if (ds.Tables[0].Rows[i]["FleetName"].ToString() == "All Vehicles" || ds.Tables[0].Rows[i]["FleetName"].ToString() == "Tous les véhicules" || ds.Tables[0].Rows[i]["FleetName"].ToString() == "Tous les vehicules")
                        ds.Tables[0].Rows.RemoveAt(i);
                    else if (!_landmark.IfObjectAssignedToFleet(Convert.ToInt32(ds.Tables[0].Rows[i]["FleetId"].ToString()), objectName, objectId))
                        ds.Tables[0].Rows.RemoveAt(i);
                }
                ds.Tables[0].AcceptChanges();

                AssignedFleets.DataSource = ds;
                AssignedFleets.DataBind();


                DataSet dsUnassignedFleet = new DataSet();
                DataTable dtUnassignedFleet = FleetView.ToTable();
                dsUnassignedFleet.Tables.Add(dtUnassignedFleet);

                for (int i = dsUnassignedFleet.Tables[0].Rows.Count - 1; i >= 0; i--)
                {
                    if (dsUnassignedFleet.Tables[0].Rows[i]["FleetName"].ToString() == "All Vehicles"
                        || dsUnassignedFleet.Tables[0].Rows[i]["FleetName"].ToString() == "Tous les véhicules"
                        || dsUnassignedFleet.Tables[0].Rows[i]["FleetName"].ToString() == "Tous les vehicules")
                    {
                        dsUnassignedFleet.Tables[0].Rows.RemoveAt(i);
                    }
                    else if (_landmark.IfObjectAssignedToFleet(Convert.ToInt32(dsUnassignedFleet.Tables[0].Rows[i]["FleetId"].ToString()), objectName, objectId))
                    {
                        dsUnassignedFleet.Tables[0].Rows.RemoveAt(i);
                    }
                }
                dsUnassignedFleet.Tables[0].AcceptChanges();

                ddFleet.DataSource = dsUnassignedFleet;
                ddFleet.DataBind();

                ddFleet.Items.Insert(0, new ListItem("-- Assign to a fleet --", "-1"));
                

                
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        private void CheckBoxFleet_Fill()
        {
            try
            {
                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                CheckBoxFleet.DataSource = dsFleets;
                CheckBoxFleet.DataBind();

                ListItem itemToRemoved = new ListItem();

                foreach (ListItem li in CheckBoxFleet.Items)
                {
                    if (li.Text == "All Vehicles" || li.Text == "Tous les véhicules" || li.Text == "Tous les vehicules")
                    {
                        //CheckBoxFleet.Items.Remove(li);
                        //break;
                        itemToRemoved = li;
                    }
                    else if (_landmark.IfObjectAssignedToFleet(Convert.ToInt32(li.Value), objectName, objectId))
                    {
                        
                            li.Selected = true;
                    }

                }
                CheckBoxFleet.Items.Remove(itemToRemoved);
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));                
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        protected void AssignedFleets_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
        {
            try
            {

                if (objectName.ToLower().Trim() == "landmark")
                    _landmark.UnassignLandmarkFromFleet(sn.User.OrganizationId, objectId, int.Parse(AssignedFleets.DataKeys[e.Item.ItemIndex].ToString()));
                else if (objectName.ToLower().Trim() == "geozone")
                    _geozone.UnassignGeozoneToFleet(sn.User.OrganizationId, objectId, int.Parse(AssignedFleets.DataKeys[e.Item.ItemIndex].ToString()));

                FleetAssignment_Fill();
                
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));                
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        protected void Button1_Click(object sender, EventArgs e)
        {
            if (objectId > 0)
            {
                _landmark.UnassignObjectFromAllFleets(objectName, objectId);
                foreach (ListItem li in CheckBoxFleet.Items)
                {
                    if (li.Selected)
                    {
                        
                        _landmark.AssignObjectToFleet(sn.User.OrganizationId, objectId, Convert.ToInt32(li.Value), objectName);
                        
                    }
                }

                lblMessage.Text = "Sucessfully saved the assignments.";
                lblMessage.ForeColor = System.Drawing.Color.Green;
                lblMessage.Visible = true;

            }
        }


        protected void ddFleet_SelectedIndexChanged(object sender, EventArgs e)
        {
            try
            {

                if (objectName.ToLower().Trim() == "landmark")
                    _landmark.AssignLandmarkToFleet(sn.User.OrganizationId, objectId, int.Parse(ddFleet.SelectedValue.ToString()));
                else if (objectName.ToLower().Trim() == "geozone")
                    _geozone.AssignGeozoneToFleet(sn.User.OrganizationId, objectId, int.Parse(ddFleet.SelectedValue.ToString()));

                FleetAssignment_Fill();

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }
}
}