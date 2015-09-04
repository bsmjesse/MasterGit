using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.IO;
using System.Text.RegularExpressions;
using System.Globalization;

namespace SentinelFM
{
    public partial class Configuration_frmTaskScheduler : SentinelFMBasePage
    {
        public int DefaultOrganizationHierarchyFleetId = 0;
        public string DefaultOrganizationHierarchyFleetName = string.Empty;
        public string DefaultOrganizationHierarchyNodeCode = string.Empty;
        public string OrganizationHierarchyPath = "";

        public bool MutipleUserHierarchyAssignment = false;
        
        protected void Page_Load(object sender, EventArgs e)
        {

            bool ShowOrganizationHierarchy = false;
            string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
            if (poh.HasOrganizationHierarchy(sn.User.OrganizationId))
                ShowOrganizationHierarchy = true;
            else
            {
                ShowOrganizationHierarchy = false;
            }

            if (ShowOrganizationHierarchy)
            {
                MutipleUserHierarchyAssignment = clsPermission.FeaturePermissionCheck(sn, "MutipleUserHierarchyAssignment");

                trFleetSelectOption.Visible = true;

                string defaultnodecode = string.Empty;
                ServerDBUser.DBUser dbu = new ServerDBUser.DBUser();

                string xml = "";
                if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetUserPreferencesXML(sn.UserID, sn.SecId, ref xml), true))
                    {

                    }

                StringReader strrXML = new StringReader(xml);
                DataSet dsPref = new DataSet();
                dsPref.ReadXml(strrXML);

                foreach (DataRow rowItem in dsPref.Tables[0].Rows)
                {
                    if (Convert.ToInt16(rowItem["PreferenceId"]) == Convert.ToInt16(VLF.CLS.Def.Enums.Preference.DefaultOrganizationHierarchyNodeCode))
                    {
                        defaultnodecode = rowItem["PreferenceValue"].ToString().TrimEnd();

                    }

                }

                defaultnodecode = defaultnodecode ?? string.Empty;
                if (defaultnodecode == string.Empty)
                {
                    if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 480)
                        defaultnodecode = poh.GetOrganizationHierarchyRootNodeCodeUserID(sn.User.OrganizationId, sn.UserID);
                    else
                        defaultnodecode = poh.GetRootNodeCode(sn.User.OrganizationId);

                }
                if (!IsPostBack)
                {
                    DefaultOrganizationHierarchyNodeCode = defaultnodecode;
                    DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, defaultnodecode);
                    DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                    hidOrganizationHierarchyNodeCode.Value = DefaultOrganizationHierarchyNodeCode;
                    hidOrganizationHierarchyFleetId.Value = DefaultOrganizationHierarchyFleetId.ToString();

                    if (MutipleUserHierarchyAssignment)
                    {
                        hidOrganizationHierarchyFleetId.Value = sn.User.PreferFleetIds;
						hidOrganizationHierarchyNodeCode.Value = sn.User.PreferNodeCodes;
                        //DefaultOrganizationHierarchyFleetId = sn.User.PreferFleetIds;
                    }

                }
                else
                {
                    hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, hidOrganizationHierarchyNodeCode.Value.ToString());
                    btnOrganizationHierarchyNodeCode.Text = hidOrganizationHierarchyNodeCode.Value.ToString();

                    DefaultOrganizationHierarchyNodeCode = hidOrganizationHierarchyNodeCode.Value.ToString();

                    DefaultOrganizationHierarchyFleetId = poh.GetFleetIdByNodeCode(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);
                    DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, DefaultOrganizationHierarchyFleetId);
                }
                if (MutipleUserHierarchyAssignment)
                {

                    if (hidOrganizationHierarchyFleetId.Value.Trim() == string.Empty)
                        DefaultOrganizationHierarchyFleetName = "";
                    else if (hidOrganizationHierarchyFleetId.Value.Contains(","))
                        DefaultOrganizationHierarchyFleetName = GetLocalResourceObject("ResMultipleHierarchies").ToString();
                    else
                        DefaultOrganizationHierarchyFleetName = poh.GetFleetNameByFleetId(sn.User.OrganizationId, int.Parse(hidOrganizationHierarchyFleetId.Value));
                }
                OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, defaultnodecode);

                //btnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName == "" ? DefaultOrganizationHierarchyNodeCode : DefaultOrganizationHierarchyFleetName;
                btnOrganizationHierarchyNodeCode.Text = DefaultOrganizationHierarchyFleetName;
                hidOrganizationHierarchyPath.Value = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, DefaultOrganizationHierarchyNodeCode);

                //if (MutipleUserHierarchyAssignment && hidOrganizationHierarchyNodeCode.Value.Contains(","))
                //    btnOrganizationHierarchyNodeCode.Text = GetLocalResourceObject("ResMultipleHierarchies").ToString(); // "Multiple Hierarchies";
            }
            else
            {
                trFleetSelectOption.Visible = false;
            }
            
            if (!Page.IsPostBack)
            {
             
                GuiSecurity(this);
	        LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref Form1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);	


                if (System.Globalization.CultureInfo.CurrentUICulture.ToString() == "fr-CA")
                {
                    txtFrom.CultureInfo.CultureName = "fr-FR";
                    txtTo.CultureInfo.CultureName = "fr-FR";
                }
                else
                {
                    txtFrom.CultureInfo.CultureName = System.Globalization.CultureInfo.CurrentUICulture.ToString();
                    txtTo.CultureInfo.CultureName = System.Globalization.CultureInfo.CurrentUICulture.ToString();
                }


                clsMisc.cboHoursFill(ref cboHoursFrom);
                clsMisc.cboHoursFill(ref cboHoursTo);
                CboFleet_Fill();

                this.txtFrom.Text = DateTime.Now.AddHours(-12).ToString("MM/dd/yyyy");
                sn.Message.FromDate = DateTime.Now.AddHours(-12).ToShortDateString();

                //this.txtTo.Text = DateTime.Now.ToShortDateString();
                this.txtTo.Text = DateTime.Now.AddHours(1).ToString("MM/dd/yyyy");
                sn.Message.ToDate = DateTime.Now.AddHours(1).ToShortDateString();



                this.cboHoursFrom.SelectedIndex = -1;
                for (int i = 0; i <= cboHoursFrom.Items.Count - 1; i++)
                {
                    if (Convert.ToInt32(cboHoursFrom.Items[i].Value) == DateTime.Now.AddHours(-12).Hour)
                    {
                        cboHoursFrom.Items[i].Selected = true;
                        sn.History.FromHours = DateTime.Now.AddHours(-12).Hour.ToString();
                        break;
                    }
                }

                this.cboHoursTo.SelectedIndex = -1;
                for (int i = 0; i <= cboHoursTo.Items.Count - 1; i++)
                {
                    if (Convert.ToInt32(cboHoursTo.Items[i].Value) == DateTime.Now.AddHours(1).Hour)
                    {
                        cboHoursTo.Items[i].Selected = true;
                        sn.History.ToHours = DateTime.Now.AddHours(1).Hour.ToString();
                        break;
                    }
                }


                CboFleet_Fill();


                if (sn.User.DefaultFleet != -1 || DefaultOrganizationHierarchyFleetId != 0 || hidOrganizationHierarchyFleetId.Value != string.Empty)
                {
                    cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));                    
                    CboVehicle_Fill(Convert.ToInt32(sn.User.DefaultFleet));
                    this.lblVehicleName.Visible = true;
                    this.cboVehicle.Visible = true;
                }


                if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
                    CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));


            }
        }

  
       
        protected void cmdDriver_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmdrivers.aspx"); 
        }
        protected void cmdUsers_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmUsers.aspx");
        }
        protected void cmdVehicles_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmVehicleInfo.aspx");
        }
        protected void cmdFleets_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmEmails.aspx"); 
        }


        private void dgSched_Fill()
        {
            try
            {
                string strFromDate = "";
                string strToDate = "";

                if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) < 12)
                    strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 AM";

                if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 12)
                    strFromDate = this.txtFrom.Text + " " + this.cboHoursFrom.SelectedItem.Value + ":00 PM";

                if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) > 12)
                    strFromDate = this.txtFrom.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) - 12) + ":00 PM";

                if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) < 12)
                    strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 AM";

                if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 12)
                    strToDate = this.txtTo.Text + " " + this.cboHoursTo.SelectedItem.Value + ":00 PM";

                if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) > 12)
                    strToDate = this.txtTo.Text + " " + Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) - 12) + ":00 PM";




                this.lblMessage.Text = "";
                CultureInfo ci = new CultureInfo("en-US");

                if (Convert.ToDateTime(strFromDate, ci) > Convert.ToDateTime(strToDate, ci))
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidFromToDate");
                    return;
                }
                else
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = "";
                }


                dgSched.LayoutSettings.ClientVisible = true;

                DataSet dsSched = new DataSet();

                StringReader strrXML = null;
                string xml = "";
                ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();

                int fleetid = -1;
                string fleetids = string.Empty;

                if (optAssignBased.SelectedItem.Value == "0")
                    fleetid = Convert.ToInt32(this.cboFleet.SelectedItem.Value);
                else
                {
                    fleetids = hidOrganizationHierarchyFleetId.Value;
                    if(!fleetids.Contains(","))
                        fleetid = Convert.ToInt32(fleetids);
                }

                if (objUtil.ErrCheck(dbs.GetSheduledTasksHistory(sn.UserID, sn.SecId, strFromDate, strToDate, fleetid, Convert.ToInt32(this.cboVehicle.SelectedItem.Value), ref xml), false))
                    if (objUtil.ErrCheck(dbs.GetSheduledTasksHistory(sn.UserID, sn.SecId, strFromDate, strToDate, fleetid, Convert.ToInt32(this.cboVehicle.SelectedItem.Value), ref xml), true))
                    {
                        sn.Message.DsScheduledTasks = null;
                        dgSched.ClearCachedDataSource();
                        dgSched.RebindDataSource();

                        return;
                    }
                if (xml == "")
                {
                    sn.Message.DsScheduledTasks = null;
                    dgSched.ClearCachedDataSource();
                    dgSched.RebindDataSource();
                    return;
                }



                string strPath = Server.MapPath("../Datasets/ScheduledTasks.xsd");
                dsSched.ReadXmlSchema(strPath);
                strrXML = new StringReader(xml);
                dsSched.ReadXml(strrXML);

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Column Count:" + dsSched.Tables[0].Columns.Count + " Form:" + Page.GetType().Name));


                // Show Combobox
                DataColumn dc = new DataColumn("chkBox", Type.GetType("System.Boolean"));
                dc.DefaultValue = false;
                dsSched.Tables[0].Columns.Add(dc);


                //if (cboScheduledMessageFilter.SelectedItem.Value == "2")
                //{
                //   DataTable dt = new DataTable();
                //   dt = dsSched.Tables[0].Clone();
                //   DataRow[] drCollections = null;
                //   drCollections = dsSched.Tables[0].Select("MsgOutDateTime<>''", "", DataViewRowState.CurrentRows);
                //   foreach (DataRow dr in drCollections)
                //      dt.ImportRow(dr);

                //   if (sn.Message.DsScheduledTasks != null)
                //      sn.Message.DsScheduledTasks.Clear(); 

                //   dgSched.DataSource = dt;
                //   sn.Message.DsScheduledTasks.Tables.Add(dt);  
                //}
                //else
                //{
                //   if (sn.Message.DsScheduledTasks!=null)
                //      sn.Message.DsScheduledTasks.Clear(); 

                //   dgSched.DataSource = dsSched;
                //   sn.Message.DsScheduledTasks = dsSched;  
                //}

                sn.Message.DsScheduledTasks = dsSched;
                dgSched.LayoutSettings.ClientVisible = true;
                dgSched.ClearCachedDataSource();
                dgSched.RebindDataSource();


            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        protected void dgSched_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
        {
            try
            {
                if (sn.Message.DsScheduledTasks != null) 
                {
                    e.DataSource = sn.Message.DsScheduledTasks;

                }
                else
                    e.DataSource = null;
            }
            catch
            {
            }
        }
        protected void dgSched_InitializeLayout(object sender, ISNet.WebUI.WebGrid.LayoutEventArgs e)
        {
            if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "en")
            {
                e.Layout.TextSettings.Language = ISNet.WebUI.WebGrid.LanguageMode.UseCustom;
                if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
                    e.Layout.TextSettings.UseLanguage = "fr-FR";
                //else
                //   e.Layout.TextSettings.UseLanguage = System.Globalization.CultureInfo.CurrentUICulture.ToString();
            }
        }

        protected void cmdScheduledTasks_Click(object sender, EventArgs e)
        {
            dgSched_Fill();
        }
        protected void cmdDeleteScheduledTasks_Click(object sender, EventArgs e)
        {
            try
          {
              if (dgSched.RootTable.GetCheckedRows().Count < 1)
                  return;
 
              Int64[] tasks = new Int64[dgSched.RootTable.GetCheckedRows().Count];
              bool[] tasksDeleted = new bool[dgSched.RootTable.GetCheckedRows().Count];
              int i = 0;
              foreach (string keyValue in dgSched.RootTable.GetCheckedRows())
              {
                  tasks[i] = Convert.ToInt64(keyValue);
                  i++;
              }


              LocationMgr.Location loc = new LocationMgr.Location();

              if (objUtil.ErrCheck(loc.DeleteTask(sn.UserID, sn.SecId, tasks, ref tasksDeleted), false))
                  if (objUtil.ErrCheck(loc.DeleteTask(sn.UserID, sn.SecId, tasks, ref tasksDeleted), true))
                  {
                  }

              dgSched_Fill();
          }
          catch (NullReferenceException Ex)
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
              RedirectToLogin();
          }
          catch (Exception Ex)
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
          }
        }



        private void CboFleet_Fill()
        {
            try
            {

                DataSet dsFleets = new DataSet();
                dsFleets = sn.User.GetUserFleets(sn);
                cboFleet.DataSource = dsFleets;
                cboFleet.DataBind();
                cboFleet.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboFleet_Item_0"), "-1"));



            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }


        }


        private void CboVehicle_Fill(int fleetId)
        {
            try
            {
                DataSet dsVehicle;
                dsVehicle = new DataSet();

                StringReader strrXML = null;


                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), true))
                    {
                        cboVehicle.Items.Clear();
                        cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));
                        return;
                    }
                if (xml == "")
                {
                    cboVehicle.Items.Clear();
                    cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));
                    return;
                }

                strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);

                cboVehicle.DataSource = dsVehicle;
                cboVehicle.DataBind();

                cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        private void CboVehicle_MultipleFleet_Fill(string fleetIds)
        {
            try
            {
                DataSet dsVehicle;
                dsVehicle = new DataSet();

                StringReader strrXML = null;


                string xml = "";
                ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

                if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByMultipleFleetIds(sn.UserID, sn.SecId, fleetIds, ref xml), false))
                    if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByMultipleFleetIds(sn.UserID, sn.SecId, fleetIds, ref xml), true))
                    {
                        cboVehicle.Items.Clear();
                        if(!fleetIds.Contains(","))
                            cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));
                        return;
                    }
                if (xml == "")
                {
                    cboVehicle.Items.Clear();
                    if (!fleetIds.Contains(","))
                        cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));
                    return;
                }

                strrXML = new StringReader(xml);
                dsVehicle.ReadXml(strrXML);

                cboVehicle.DataSource = dsVehicle;
                cboVehicle.DataBind();
                
                if (!fleetIds.Contains(","))
                    cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "0"));

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }

        protected void cboFleet_SelectedIndexChanged(object sender, EventArgs e)
        {
            /*if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
                CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
            else
                this.cboVehicle.Items.Clear();*/
            refillCboVehicle();
        }

        protected void hidOrganizationHierarchyPostBack_Click(object sender, System.EventArgs e)
        {
            refillCboVehicle();
        }

        private void refillCboVehicle()
        {
            int fleetid = -1;
            string fleetids = string.Empty;
            if (optAssignBased.SelectedItem.Value == "0")
                fleetid = Convert.ToInt32(this.cboFleet.SelectedItem.Value);
            else
            {
                
                if (MutipleUserHierarchyAssignment)
                    fleetids = hidOrganizationHierarchyFleetId.Value;
                else
                    fleetid = Convert.ToInt32(hidOrganizationHierarchyFleetId.Value);
            }

            if (fleetid != -1 || fleetids != string.Empty)
            {
                if (fleetids != string.Empty)
                    CboVehicle_MultipleFleet_Fill(fleetids);
                else
                    CboVehicle_Fill(fleetid);
            }
            else
                this.cboVehicle.Items.Clear();
        }

        protected void optAssignBased_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (optAssignBased.SelectedItem.Value == "0")
            {
                cboFleet.Visible = true;
                btnOrganizationHierarchyNodeCode.Visible = false;
                lblFleetTitle.Text = (string)base.GetLocalResourceObject("lblFleetTitleResource1.Text");
            }
            else
            {
                cboFleet.Visible = false;
                btnOrganizationHierarchyNodeCode.Visible = true;
                lblFleetTitle.Text = (string)base.GetLocalResourceObject("lblOrganizationHierarchyFleet");
            }
            refillCboVehicle();

        }
}
}
