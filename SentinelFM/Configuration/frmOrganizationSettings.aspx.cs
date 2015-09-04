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
using OboutInc.ColorPicker;
using VLF.DAS.Logic;  
namespace SentinelFM
{
    public partial class Configuration_frmOrganizationSettings : SentinelFMBasePage
    {
        static int NoOfDays = 0;
        protected void Page_Load(object sender, EventArgs e)
        {

           
            foreach (ListItem Event in lstAssEvents.Items)
            {
                
                Event.Attributes["title"] = Event.Text;
            }

            foreach (ListItem UnAssEvents in lstUnAssEvents.Items)
            {
                UnAssEvents.Attributes["title"] = UnAssEvents.Text;
            }

            foreach (ListItem UnAssViolations in lstUnAssViolations.Items)
            {
                UnAssViolations.Attributes["title"] = UnAssViolations.Text;
            }

            foreach (ListItem Violation in lstAssViolations.Items)
            {
                
                Violation.Attributes["title"] = Violation.Text;

            }

        
            
            if (!Page.IsPostBack)
            {
               

                if (sn.User.OrganizationId==999630)
                    DriversReport.Visible=true;
                else
                    DriversReport.Visible=false;

                lstUnAssVehicles_Fill();
                lstAssVehicles_Fill();
                //Changes
                lstUnAssEvents_Fill();
                lstAssEvents_Fill();
                //Changes

                //Changes
                lstUnAssViolations_Fill();
                lstAssViolations_Fill();
                //Changes
  
                GetOrganizationPreferences();
                GuiSecurity(this);
                  
                LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref Form1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
            }
        }

        private void GetOrganizationPreferences()
        {
           
            DataSet ds = new DataSet();
            string xml = "";
            string strIdlingSummary = "";
            string[] tmp ; 

            using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
            {
                if (objUtil.ErrCheck(dbOrganization.GetOrganizationSettings(sn.UserID, sn.SecId,sn.User.OrganizationId,ref xml), false))
                    if (objUtil.ErrCheck(dbOrganization.GetOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true ))
                    {
                        return;
                    }
            }

             
            pickerMenus.InitialColor = pickerMenusColor.Style["background-color"] = sn.User.MenuColor;
            if (sn.User.MenuColor == "")
                pickerMenusColor.Text = (string)base.GetLocalResourceObject("strDefault"); // "Default"

            pickerTabBackground.InitialColor = pickerTabBackgroundColor.Style["background-color"] = sn.User.ConfigTabBackColor;

            if (sn.User.ConfigTabBackColor == "")
                pickerTabBackgroundColor.Text = (string)base.GetLocalResourceObject("strDefault"); // "Default";

            txtZone.Text =  sn.User.TemperatureZone;
            if (sn.User.TemperatureZone == "")
                txtZone.Text = "1"; // "Default"

            if (sn.HomePagePicture != "")
                this.txtFileName.Text = sn.HomePagePicture;


            if (xml == "")
            {
                return;
            }

            StringReader strrXML = new StringReader(xml);
            ds.ReadXml(strrXML);


            if (sn.User.OrganizationId == 999630)
            {

                foreach (DataRow dr in ds.Tables[0].Rows)
                {

                    try
                    {
                        if (dr["OrgPreferenceId"].ToString() == "8")
                            this.cboIdlingThreshold.Items.FindByValue((dr["PreferenceValue"]).ToString()).Selected = true;

                        if (dr["OrgPreferenceId"].ToString() == "7")
                        {
                            strIdlingSummary = dr["PreferenceValue"].ToString();
                            tmp = strIdlingSummary.Split(';');
                            if (tmp.Length == 8)
                            {
                                this.txtSpeed120.Text = tmp[0];
                                this.txtSpeed130.Text = tmp[1];
                                this.txtSpeed140.Text = tmp[2];
                                this.txtAccExtreme.Text = tmp[3];
                                this.txtAccHarsh.Text = tmp[4];
                                this.txtBrakingExtreme.Text = tmp[5];
                                this.txtBrakingHarsh.Text = tmp[6];
                                this.txtSeatBelt.Text = tmp[7];
                            }

                        }


                        if (dr["OrgPreferenceId"].ToString() == "9")
                        {

                            if (dr["PreferenceValue"].ToString()=="0")
                            {
                                this.chkWeeklyReportActive.Checked=false;
                                this.chkMonthlyReportActive.Checked = false;
                            }
                            else if (dr["PreferenceValue"].ToString() == "3")
                            {
                                this.chkWeeklyReportActive.Checked = true ;
                                this.chkMonthlyReportActive.Checked = true;
                            }
                            else if (dr["PreferenceValue"].ToString() == "1")
                            {
                                this.chkWeeklyReportActive.Checked = true;
                                this.chkMonthlyReportActive.Checked = false;
                            }
                            else if (dr["PreferenceValue"].ToString() == "2")
                            {
                                this.chkWeeklyReportActive.Checked = false;
                                this.chkMonthlyReportActive.Checked = true;
                            }
                        }

                    

                    }
                    catch 
                    {

                    }

                }


                string sConnectionString = ConfigurationManager.ConnectionStrings["DWH"].ConnectionString;
                DataSet dsEvt = new DataSet();
                VLF.DAS.Logic.Organization org = new VLF.DAS.Logic.Organization(sConnectionString);
                dsEvt = org.evtEventSetupGet(sn.User.OrganizationId, 3);
                 
                if (dsEvt.Tables.Count>0 && dsEvt.Tables[0].Rows.Count>0)  
                       this.txtFuelConsumptionSpeeding.Text = dsEvt.Tables[0].Rows[0]["Max"].ToString();    
            }

            if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 999955)
            {
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    try
                    {

                        if (dr["OrgPreferenceId"].ToString() == "13")
                            this.txtCustomMsg.Text = dr["PreferenceValue"].ToString();

                    }
                    catch
                    {

                    }
                }

                this.txtCustomMsg.Visible = true;
                this.lblCustomMsg.Visible = true;  
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

        //Changes
        protected void cmdAddViolation_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.lblMessage.Text = "";


                if (this.lstUnAssViolations.SelectedIndex == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectViolation");  //"Please select a violation.";
                    return;
                }

                foreach (ListItem li in lstUnAssViolations.Items)
                {
                    if (li.Selected)
                    {
                        using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                        {
                            if (objUtil.ErrCheck(dbOrganization.AddOrganizationViolationPreference(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Text, Convert.ToInt32(li.Value)), false))
                                if (objUtil.ErrCheck(dbOrganization.AddOrganizationViolationPreference(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Text, Convert.ToInt32(li.Value)), true))
                                {
                                    return;
                                }
                        }
                    }

                }
                try
                {
                    lstUnAssViolations_Fill();
                }
                catch { }
                try
                {
                    lstAssViolations_Fill();
                }
                catch { }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }


            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmOrganizationSettings.aspx"));
            }


        }
        //Changes

        //Changes
        protected void cmdAddEvent_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.lblMessage.Text = "";


                if (this.lstUnAssEvents.SelectedIndex == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectEvent");  //"Please select a event.";
                    return;
                }

                foreach (ListItem li in lstUnAssEvents.Items)
                {
                    if (li.Selected)
                    {
                        using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                        {
                            if (objUtil.ErrCheck(dbOrganization.AddOrganizationEventPreference(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Text,Convert.ToInt32(li.Value)), false))
                                if (objUtil.ErrCheck(dbOrganization.AddOrganizationEventPreference(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Text, Convert.ToInt32(li.Value)), true))
                                {
                                    return;
                                }
                        }
                    }

                }
                try
                {
                    lstUnAssEvents_Fill();
                }
                catch { }
                try
                {
                    lstAssEvents_Fill();
                }
                catch { }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }


            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmOrganizationSettings.aspx"));
            }


        }
        //Changes
        protected void cmdAdd_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.lblMessage.Text = "";
               
                    if (this.lstUnAss.SelectedIndex == -1)
                    {
                        this.lblMessage.Visible = true;
                        this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectSensor");  //"Please select a vehicle.";
                        return;
                    }

                    foreach (ListItem li in lstUnAss.Items)
                    {
                        if (li.Selected)
                        {
                            using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                            {
                                if (objUtil.ErrCheck(dbOrganization.AddOrganizationSensorPreferene(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Value), false))
                                    if (objUtil.ErrCheck(dbOrganization.AddOrganizationSensorPreferene(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Value), true))
                                    {
                                        return;
                                    }
                            }
                        }

                    }
                    try
                    {
                        lstUnAssVehicles_Fill();
                    }
                    catch { }
                    try
                    {
                        lstAssVehicles_Fill();
                    }
                    catch { }
                
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }


            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmOrganizationSettings.aspx"));
            }


        }

        //Changes
        protected void cmdRemoveViolation_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.lblMessage.Text = "";


                if (this.lstAssViolations.SelectedIndex == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectViolation");  //"Please select a vehicle.";
                    return;
                }

                foreach (ListItem li in lstAssViolations.Items)
                {
                    if (li.Selected)
                    {
                        using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                        {
                            if (objUtil.ErrCheck(dbOrganization.DeleteOrganizationViolationPreference(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Text, Convert.ToInt32(li.Value)), false))
                                if (objUtil.ErrCheck(dbOrganization.DeleteOrganizationViolationPreference(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Text, Convert.ToInt32(li.Value)), false))
                                {
                                    return;
                                }
                        }
                    }

                }

                lstUnAssViolations_Fill();
                lstAssViolations_Fill();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmOrganizationSettings.aspx"));
            }

        }
        //Changes

        //Changes
        protected void cmdRemoveEvent_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.lblMessage.Text = "";


                if (this.lstAssEvents.SelectedIndex == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectEvent");  //"Please select a vehicle.";
                    return;
                }

                foreach (ListItem li in lstAssEvents.Items)
                {
                    if (li.Selected)
                    {
                        using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                        {
                            if (objUtil.ErrCheck(dbOrganization.DeleteOrganizationEventPreference(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Text, Convert.ToInt32(li.Value)), false))
                                if (objUtil.ErrCheck(dbOrganization.DeleteOrganizationEventPreference(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Text, Convert.ToInt32(li.Value)), false))
                                {
                                    return;
                                }
                        }
                    }

                }

                lstUnAssEvents_Fill();
                lstAssEvents_Fill();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmOrganizationSettings.aspx"));
            }

        }
        //Changes

        protected void cmdRemove_Click(object sender, System.EventArgs e)
        {
            try
            {
               this.lblMessage.Text = "";


                if (this.lstAss.SelectedIndex == -1)
                {
                    this.lblMessage.Visible = true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectSensor");  //"Please select a vehicle.";
                    return;
                }

                foreach (ListItem li in lstAss.Items)
                {
                    if (li.Selected)
                    {
                        using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                        {
                            if (objUtil.ErrCheck(dbOrganization.DeleteOrganizationSensorPreferene(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Value), false))
                                if (objUtil.ErrCheck(dbOrganization.DeleteOrganizationSensorPreferene(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Value), false))
                                {
                                    return;
                                }
                        }
                    }

                }

                lstUnAssVehicles_Fill();
                lstAssVehicles_Fill();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmOrganizationSettings.aspx"));
            }

        }

        //Changes
        protected void cmdAddAllViolation_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.lblMessage.Text = "";

                foreach (ListItem li in lstUnAssViolations.Items)
                {

                    using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                    {
                        if (objUtil.ErrCheck(dbOrganization.AddOrganizationViolationPreference(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Text, Convert.ToInt32(li.Value)), false))
                            if (objUtil.ErrCheck(dbOrganization.AddOrganizationViolationPreference(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Text, Convert.ToInt32(li.Value)), true))
                            {
                                return;
                            }
                    }
                }

                lstUnAssViolations_Fill();
                lstAssViolations_Fill();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmOrganizationSettings.aspx"));
            }

        }
        //Changes

        //Changes
        protected void cmdAddAllEvent_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.lblMessage.Text = "";

                foreach (ListItem li in lstUnAssEvents.Items)
                {

                    using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                    {
                        if (objUtil.ErrCheck(dbOrganization.AddOrganizationEventPreference(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Text,Convert.ToInt32(li.Value)), false))
                            if (objUtil.ErrCheck(dbOrganization.AddOrganizationEventPreference(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Text, Convert.ToInt32(li.Value)), true))
                            {
                                return;
                            }
                    }
                }

                lstUnAssEvents_Fill();
                lstAssEvents_Fill();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmOrganizationSettings.aspx"));
            }

        }
        //Changes

        protected void cmdAddAll_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.lblMessage.Text = "";

                foreach (ListItem li in lstUnAss.Items)
                {

                    using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                    {
                        if (objUtil.ErrCheck(dbOrganization.AddOrganizationSensorPreferene(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Value), false))
                            if (objUtil.ErrCheck(dbOrganization.AddOrganizationSensorPreferene(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Value), true))
                            {
                                return;
                            }
                    }
                }

                    lstUnAssVehicles_Fill();
                    lstAssVehicles_Fill();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmOrganizationSettings.aspx"));
            }

        }

        //Changes
        protected void cmdRemoveAllViolation_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.lblMessage.Text = "";

                foreach (ListItem li in lstAssViolations.Items)
                {
                    using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                    {
                        if (objUtil.ErrCheck(dbOrganization.DeleteOrganizationViolationPreference(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Text, Convert.ToInt32(li.Value)), false))
                            if (objUtil.ErrCheck(dbOrganization.DeleteOrganizationViolationPreference(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Text, Convert.ToInt32(li.Value)), false))
                            {
                                return;
                            }
                    }
                }
                lstUnAssViolations_Fill();
                lstAssViolations_Fill();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmOrganizationSettings.aspx"));
            }
        }
        //Changes

        //Changes
        protected void cmdRemoveAllEvent_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.lblMessage.Text = "";

                foreach (ListItem li in lstAssEvents.Items)
                {
                    using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                    {
                        if (objUtil.ErrCheck(dbOrganization.DeleteOrganizationEventPreference(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Text, Convert.ToInt32(li.Value)), false))
                            if (objUtil.ErrCheck(dbOrganization.DeleteOrganizationEventPreference(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Text, Convert.ToInt32(li.Value)), false))
                            {
                                return;
                            }
                    }
                }
                lstUnAssEvents_Fill();
                lstAssEvents_Fill();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmOrganizationSettings.aspx"));
            }
        }
        //Changes

        protected void cmdRemoveAll_Click(object sender, System.EventArgs e)
        {
            try
            {
                this.lblMessage.Text = "";

                foreach (ListItem li in lstAss.Items)
                {
                    using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                    {
                        if (objUtil.ErrCheck(dbOrganization.DeleteOrganizationSensorPreferene(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Value), false))
                            if (objUtil.ErrCheck(dbOrganization.DeleteOrganizationSensorPreferene(sn.UserID, sn.SecId, sn.User.OrganizationId, li.Value), false))
                            {
                                return;
                            }
                    }
                }
                lstUnAssVehicles_Fill();
                lstAssVehicles_Fill();
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmOrganizationSettings.aspx"));
            }
        }

        //Changes
        private void lstUnAssEvents_Fill()
        {
            try
            {

                try
                {
                  NoOfDays = sn.User.VehicleNotReported;
                }
                catch
                {
                    NoOfDays = 3;
                }
                string xml = "";                
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                EvtEvents eevent = new EvtEvents(sConnectionString);                
                DataSet dataset = eevent.GetViolationByOrganization(Convert.ToInt32(sn.User.OrganizationId), "Event");
                dataset.Tables[0].Rows.Add("-2", "* Vehicle Not Reported for last " + NoOfDays + " days");
                DataView dv = dataset.Tables[0].DefaultView;
                dv.Sort = "Description ASC";
                DataTable sortedDT = dv.ToTable();
                DataSet dsSorted = new DataSet();
                dsSorted.Tables.Add(sortedDT);

                xml = dsSorted.GetXml();
               
                if (xml == "")
                {
                    this.lstUnAssEvents.Items.Clear();
                    return;
                }
                DataSet dsEvent = new DataSet();
                StringReader strrXML = new StringReader(xml);
                dsEvent.ReadXml(strrXML);
                DataView view = dsEvent.Tables[0].DefaultView;
                view.Sort = "Description";
                this.lstUnAssEvents.DataSource = view;
                this.lstUnAssEvents.DataBind();
                foreach (ListItem UnAssEvents in lstUnAssEvents.Items)
                {
                    UnAssEvents.Attributes["title"] = UnAssEvents.Text;
                }
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmOrganizationSettings.aspx"));
            }
        }

        private void lstAssEvents_Fill()
        {
            try
            {
                StringReader strrXML = null;
                DataSet dsEvent = new DataSet();

                string xml = "";
                using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                {
                    if (objUtil.ErrCheck(dbOrganization.GetOrganizationEventPreferences(sn.UserID, sn.SecId, Convert.ToInt32(sn.User.OrganizationId), ref xml), false))
                        if (objUtil.ErrCheck(dbOrganization.GetOrganizationEventPreferences(sn.UserID, sn.SecId, Convert.ToInt32(sn.User.OrganizationId), ref xml), true))
                        {
                            return;
                        }
                }
                if (xml == "")
                {
                    this.lstAssEvents.Items.Clear();
                    sn.History.DsAssEvents = "";
                    return;
                }

                strrXML = new StringReader(xml);
                dsEvent.ReadXml(strrXML);
                sn.History.DsAssEvents = "";
                DataView view = dsEvent.Tables[0].DefaultView;
                view.Sort = "EventName";
                this.lstAssEvents.DataSource = view;
                this.lstAssEvents.DataBind();
                foreach (ListItem Event in lstAssEvents.Items)
                {
                    sn.History.DsAssEvents += Event + ",";
                    this.lstUnAssEvents.Items.Remove(Event);
                    Event.Attributes["title"] = Event.Text;
                }               
                sn.History.DsAssEvents = sn.History.DsAssEvents.Substring(0, sn.History.DsAssEvents.LastIndexOf(","));

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmOrganizationSettings.aspx"));
            }

        }
        //Changes

        //Changes
        private void lstUnAssViolations_Fill()
        {
            try
            {                
                string xml = "";
                string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                EvtEvents eevent = new EvtEvents(sConnectionString);
                DataSet dataset = eevent.GetViolationByOrganization(Convert.ToInt32(sn.User.OrganizationId), "Violation");
                dataset.Tables[0].Rows.Add("-2", "* Vehicle Not Reported for last " + NoOfDays + " days");
                DataView dv = dataset.Tables[0].DefaultView;
                dv.Sort = "Description ASC";
                DataTable sortedDT = dv.ToTable();
                DataSet dsSorted = new DataSet();
                dsSorted.Tables.Add(sortedDT);

                xml = dsSorted.GetXml();

                if (xml == "")
                {
                    this.lstUnAssViolations.Items.Clear();
                    return;
                }
                DataSet dsEvent = new DataSet();
                StringReader strrXML = new StringReader(xml);
                dsEvent.ReadXml(strrXML);
                DataView view = dsEvent.Tables[0].DefaultView;
                view.Sort = "Description";
                this.lstUnAssViolations.DataSource = view;
                this.lstUnAssViolations.DataBind();
                foreach (ListItem UnAssViolations in lstUnAssViolations.Items)
                {
                    UnAssViolations.Attributes["title"] = UnAssViolations.Text;
                }
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmOrganizationSettings.aspx"));
            }
        }

        private void lstAssViolations_Fill()
        {
            try
            {
                StringReader strrXML = null;
                DataSet dsEvent = new DataSet();

                string xml = "";
                using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                {
                    if (objUtil.ErrCheck(dbOrganization.GetOrganizationViolationPreferences(sn.UserID, sn.SecId, Convert.ToInt32(sn.User.OrganizationId), ref xml), false))
                        if (objUtil.ErrCheck(dbOrganization.GetOrganizationViolationPreferences(sn.UserID, sn.SecId, Convert.ToInt32(sn.User.OrganizationId), ref xml), true))
                        {
                            return;
                        }
                }
                if (xml == "")
                {
                    this.lstAssViolations.Items.Clear();
                    sn.History.DsAssEvents = "";
                    return;
                }

                strrXML = new StringReader(xml);
                dsEvent.ReadXml(strrXML);
                sn.History.DsAssViolations = "";
                DataView view = dsEvent.Tables[0].DefaultView;
                view.Sort = "ViolationName";
                this.lstAssViolations.DataSource = view;
                this.lstAssViolations.DataBind();                

                foreach (ListItem Violation in lstAssViolations.Items)
                {
                    sn.History.DsAssViolations += Violation + ",";
                    this.lstUnAssViolations.Items.Remove(Violation);
                    Violation.Attributes["title"] = Violation.Text;
                    
                }
                sn.History.DsAssViolations = sn.History.DsAssViolations.Substring(0, sn.History.DsAssEvents.LastIndexOf(","));

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmOrganizationSettings.aspx"));
            }

        }
        //Changes

        private void lstUnAssVehicles_Fill()
        {
            try
            {
                string xml = "";
                using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                {
                    if (objUtil.ErrCheck(dbOrganization.GetOrganizationSensor(sn.UserID, sn.SecId, Convert.ToInt32(sn.User.OrganizationId), ref xml), false))
                        if (objUtil.ErrCheck(dbOrganization.GetOrganizationSensor(sn.UserID, sn.SecId, Convert.ToInt32(sn.User.OrganizationId), ref xml), true))
                        {
                            return;
                        }
                }
                if (xml == "")
                {
                    this.lstUnAss.Items.Clear();
                    return;
                }
                DataSet dsSensor = new DataSet();
                StringReader strrXML = new StringReader(xml);
                dsSensor.ReadXml(strrXML);
                DataView view = dsSensor.Tables[0].DefaultView;
                view.Sort = "SensorName";
                this.lstUnAss.DataSource = view;
                this.lstUnAss.DataBind();
            }

            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmOrganizationSettings.aspx"));
            }
        }

        private void lstAssVehicles_Fill()
        {
            try
            {
                StringReader strrXML = null;
                DataSet dsSensor = new DataSet();

                string xml = "";
                 using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                        {
                        if (objUtil.ErrCheck(dbOrganization.GetOrganizationSensorPreferences(sn.UserID, sn.SecId, Convert.ToInt32(sn.User.OrganizationId), ref xml), false))
                            if (objUtil.ErrCheck(dbOrganization.GetOrganizationSensorPreferences(sn.UserID, sn.SecId, Convert.ToInt32(sn.User.OrganizationId), ref xml), true))
                            {
                                return;
                            }
                 }
                if (xml == "")
                {
                    this.lstAss.Items.Clear();
                    sn.History.DsAssSensors = "";
                    return;
                }

                strrXML = new StringReader(xml);
                dsSensor.ReadXml(strrXML);
                sn.History.DsAssSensors = "";
                DataView view = dsSensor.Tables[0].DefaultView;
                view.Sort = "SensorName";
                this.lstAss.DataSource = view;
                this.lstAss.DataBind();
               for (int i=0;i<lstAss.Items.Count; i++)
               {
                   sn.History.DsAssSensors += this.lstAss.Items[i] + ",";
                   this.lstUnAss.Items.Remove(lstAss.Items[i].Value);
               }
               sn.History.DsAssSensors = sn.History.DsAssSensors.Substring(0,sn.History.DsAssSensors.LastIndexOf(","));
               
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmOrganizationSettings.aspx"));
            }

        }

        protected void txtZone_TextChanged(object sender, EventArgs e)
        {
            sn.User.TemperatureZone = txtZone.Text;
        }
             
        protected void pickerTabBackground_ColorPostBack(object sender, ColorPostBackEventArgs e)
        {
            string clr = e.Color;
            pickerTabBackgroundColor.Style["background-color"] = e.Color;
            sn.User.ConfigTabBackColor = e.Color;
            Response.Write("<script language='javascript'> parent.main.location.href='frmOrganizationSettings.aspx'; </script>");
            
        }
        protected void pickerMenus_ColorPostBack(object sender, ColorPostBackEventArgs e)
        {
            string clr = e.Color;
            pickerMenusColor.Style["background-color"] = e.Color;
            sn.User.MenuColor = e.Color;
            Response.Write("<script language='javascript'> parent.menu.location.href='../frmTopMenu.aspx'; </script>");
        }
       
        protected void cmdUpdate_Click(object sender, EventArgs e)
        {
            if (fileUpEx.HasFile)
            {
                try
                {
                    string fileName = Server.MapPath("~/Home/images/") + this.fileUpEx.FileName;
                    this.fileUpEx.PostedFile.SaveAs(fileName);
                    sn.HomePagePicture = fileUpEx.FileName;


                      using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                {
                    if (objUtil.ErrCheck(dbOrganization.UpdateOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(VLF.CLS.Def.Enums.OrganizationPreference.HomePagePicture ), sn.HomePagePicture), false))
                        if (objUtil.ErrCheck(dbOrganization.UpdateOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(VLF.CLS.Def.Enums.OrganizationPreference.HomePagePicture), sn.HomePagePicture), true))
                        {
                            this.lblMessage.Text = "Organization settings update failed";
                            return;
                        }
                }

                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " :: " + Ex.StackTrace));
                }
            }


            if (sn.User.ConfigTabBackColor != "")
            {
                using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                {
                    if (objUtil.ErrCheck(dbOrganization.UpdateOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(VLF.CLS.Def.Enums.OrganizationPreference.ConfTabBackGround), sn.User.ConfigTabBackColor), false))
                        if (objUtil.ErrCheck(dbOrganization.UpdateOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(VLF.CLS.Def.Enums.OrganizationPreference.ConfTabBackGround), sn.User.ConfigTabBackColor), true))
                        {
                            this.lblMessage.Text = "Organization settings update failed";
                            return;
                        }
                }
            }





            if (sn.User.MenuColor!="")
            {
                using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                {
                    if (objUtil.ErrCheck(dbOrganization.UpdateOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(VLF.CLS.Def.Enums.OrganizationPreference.HeaderColor), sn.User.MenuColor), false))
                        if (objUtil.ErrCheck(dbOrganization.UpdateOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(VLF.CLS.Def.Enums.OrganizationPreference.HeaderColor), sn.User.MenuColor), true))
                        {
                            this.lblMessage.Text = "Organization settings update failed";
                            return;
                        }
                }
            }

            //temperature for zones
            sn.User.TemperatureZone = txtZone.Text;
            if (sn.User.TemperatureZone != "")
            {
                using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                {
                    if (objUtil.ErrCheck(dbOrganization.UpdateOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(VLF.CLS.Def.Enums.OrganizationPreference.TempZone), sn.User.TemperatureZone), false))
                        if (objUtil.ErrCheck(dbOrganization.UpdateOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(VLF.CLS.Def.Enums.OrganizationPreference.TempZone), sn.User.TemperatureZone), true))
                        {
                            this.lblMessage.Text = "Organization settings update failed";
                            return;
                        }
                }
            }

            if (sn.User.OrganizationId == 999630)
            {

                using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                {
                    string strViolationsSummary = this.txtSpeed120.Text + ";" + this.txtSpeed130.Text + ";" + this.txtSpeed140.Text + ";" + txtAccExtreme.Text + ";" + this.txtAccHarsh.Text + ";" + this.txtBrakingExtreme.Text + ";" + this.txtBrakingHarsh.Text + ";" + this.txtSeatBelt.Text;

                    if (objUtil.ErrCheck(dbOrganization.UpdateOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(VLF.CLS.Def.Enums.OrganizationPreference.SpeedThresholdDriverReport), strViolationsSummary), false))
                        if (objUtil.ErrCheck(dbOrganization.UpdateOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(VLF.CLS.Def.Enums.OrganizationPreference.SpeedThresholdDriverReport), strViolationsSummary), true))
                        {
                            this.lblMessage.Text = "Organization settings update failed";
                            return;
                        }



                    if (objUtil.ErrCheck(dbOrganization.UpdateOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(VLF.CLS.Def.Enums.OrganizationPreference.IdlingThresholdDriverReport), cboIdlingThreshold.SelectedItem.Value), false))
                        if (objUtil.ErrCheck(dbOrganization.UpdateOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(VLF.CLS.Def.Enums.OrganizationPreference.IdlingThresholdDriverReport), cboIdlingThreshold.SelectedItem.Value), true))
                        {
                            this.lblMessage.Text = "Organization settings update failed";
                            return;
                        }


                    Int16 DriverReportActive = 0;
                    if (this.chkWeeklyReportActive.Checked && this.chkMonthlyReportActive.Checked)
                        DriverReportActive = 3;
                    else if (this.chkWeeklyReportActive.Checked && !this.chkMonthlyReportActive.Checked)
                        DriverReportActive = 1;
                    else if (!this.chkWeeklyReportActive.Checked && this.chkMonthlyReportActive.Checked)
                        DriverReportActive = 2;


                    if (objUtil.ErrCheck(dbOrganization.UpdateOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(VLF.CLS.Def.Enums.OrganizationPreference.ReportFrequencyEmailDriver), DriverReportActive.ToString()), false))
                        if (objUtil.ErrCheck(dbOrganization.UpdateOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(VLF.CLS.Def.Enums.OrganizationPreference.ReportFrequencyEmailDriver), DriverReportActive.ToString()), true))
                        {
                            this.lblMessage.Text = "Organization settings update failed";
                            return;
                        }



                    string sConnectionString = ConfigurationManager.ConnectionStrings["DWH"].ConnectionString;

                    if (clsUtility.IsNumeric(this.txtFuelConsumptionSpeeding.Text))
                    {
                        VLF.DAS.Logic.Organization org = new VLF.DAS.Logic.Organization(sConnectionString);
                        org.evtEventSetup_Update (sn.User.OrganizationId, 3, Convert.ToInt32(this.txtFuelConsumptionSpeeding.Text));
                    }



                }
            }

            if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 999955)
            {
                using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                {
                    if (objUtil.ErrCheck(dbOrganization.UpdateOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, 13,this.txtCustomMsg.Text)  , false))
                        if (objUtil.ErrCheck(dbOrganization.UpdateOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, 13, this.txtCustomMsg.Text), true))
                        {
                            this.lblMessage.Text = "Organization settings update failed";
                            return;
                        }
                }
            }

            this.lblMessage.Text = GetLocalResourceObject("MessageOrgSettingUpdateSucess").ToString();
        }
        protected void cmdRestoreDefaults_Click(object sender, EventArgs e)
        {
            using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
            {
                if (objUtil.ErrCheck(dbOrganization.DeleteOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId), false))
                    if (objUtil.ErrCheck(dbOrganization.DeleteOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId), true ))
                    {
                        this.lblMessage.Text = "Organization settings restoring failed";
                        
                    }
            }
            sn.User.MenuColor = "";
            sn.HomePagePicture = "";
            sn.User.ConfigTabBackColor = "";
            sn.User.TemperatureZone = "";

            pickerMenus.InitialColor = pickerMenusColor.Style["background-color"] = "white";
            pickerMenusColor.Text = (string)base.GetLocalResourceObject("strDefault");

            pickerTabBackground.InitialColor = pickerTabBackgroundColor.Style["background-color"] = "white";
            pickerTabBackgroundColor.Text = (string)base.GetLocalResourceObject("strDefault");

            this.txtFileName.Text = (string)base.GetLocalResourceObject("strDefault");

            txtZone.Text = "1";

            //Response.Write("<script language='javascript'> parent.menu.location.href='../frmTopMenu.aspx'; </script>");
            this.lblMessage.Text = "Organization settings have been restored";

          

        }
        
}
}
