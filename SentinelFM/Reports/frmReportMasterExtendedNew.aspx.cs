using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Globalization;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO;
using VLF.ERRSecurity;
using VLF.Reports;
using System.Configuration;
using System.Text;
using VLF.CLS;

namespace SentinelFM
{
    public partial class Reports_frmReportMasterExtended : SentinelFMBasePage
   {
      //
      //protected System.Web.UI.WebControls.CheckBox chkHistIncludeInvalid;
      //protected System.Web.UI.WebControls.DropDownList cboFromHoursSOS;
      //protected System.Web.UI.WebControls.DropDownList cboToHoursSOS;


      public bool ShowOrganizationHierarchy;
      public string OrganizationHierarchyPath = "";
      private string CurrentUICulture = "en-US";

      protected void Page_Load(object sender, EventArgs e)
      {
         try
         {
             sn.Report.ReportActiveTab = 1;

             HttpCookie aCookie = new HttpCookie(sn.User.OrganizationId.ToString() + "SnReportActiveTab");
             aCookie.Value = "1";
             aCookie.Expires = DateTime.Now.AddYears(1);
             Response.Cookies.Add(aCookie);

             ShowOrganizationHierarchy = false;
             string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
             VLF.PATCH.Logic.PatchOrganizationHierarchy poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
             if (poh.HasOrganizationHierarchy(sn.User.OrganizationId))
                 ShowOrganizationHierarchy = true;
             else
             {
                 ShowOrganizationHierarchy = false;
             }

             string datetime = "";

            if (ShowOrganizationHierarchy)
            {


                clsUtility objUtil;
                objUtil = new clsUtility(sn);
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
                        string nodecode = rowItem["PreferenceValue"].ToString().TrimEnd();
                        poh = new VLF.PATCH.Logic.PatchOrganizationHierarchy(sConnectionString);
                        OrganizationHierarchyPath = poh.GetOrganizationHierarchyPath(sn.User.OrganizationId, nodecode);
                    }
                }

                ReportBasedOption();
            }
            else
            {
                this.organizationHierarchy.Visible = false;
                this.vehicleSelectOption.Visible = false;
                this.trFleet.Visible = true;
            }



            if (!Page.IsPostBack)
            {

                try
                {
                    if (Request[this.txtFrom.UniqueID] != null)
                    {
                        //this.txtFrom.Text = Request[this.txtFrom.UniqueID];
                        datetime = Convert.ToDateTime(Request[this.txtFrom.UniqueID].ToString()).ToString(sn.User.DateFormat);
                        this.txtFrom.SelectedDate = ConvertStringToDateTime(datetime, sn.User.DateFormat, CurrentUICulture);
                    }

                    if (Request[this.txtTo.UniqueID] != null)
                    {
                        //this.txtTo.Text = Request[this.txtTo.UniqueID];
                        datetime = Convert.ToDateTime(Request[this.txtTo.UniqueID].ToString()).ToString(sn.User.DateFormat);
                        this.txtTo.SelectedDate = ConvertStringToDateTime(datetime, sn.User.DateFormat, CurrentUICulture);                        
                    }
                }
                catch
                {
                    //if (!String.IsNullOrEmpty(sn.Report.FromDate))
                    //    this.txtFrom.Text = Convert.ToDateTime(sn.Report.FromDate).ToString("MM/dd/yyyy");
                    //if (!String.IsNullOrEmpty(sn.Report.ToDate))
                    //    this.txtTo.Text = Convert.ToDateTime(sn.Report.ToDate).ToString("MM/dd/yyyy");

                    if (!String.IsNullOrEmpty(sn.Report.FromDate))
                    {
                        datetime = Convert.ToDateTime(sn.Report.FromDate).ToString(sn.User.DateFormat);
                        this.txtFrom.SelectedDate = ConvertStringToDateTime(datetime, sn.User.DateFormat, CurrentUICulture);                        
                    }
                    if (!String.IsNullOrEmpty(sn.Report.ToDate))
                    {
                        datetime = Convert.ToDateTime(sn.Report.ToDate).ToString(sn.User.DateFormat);
                        this.txtTo.SelectedDate = ConvertStringToDateTime(datetime, sn.User.DateFormat, CurrentUICulture);                        
                    }
                }

                CurrentUICulture = System.Globalization.CultureInfo.CurrentUICulture.ToString();

                txtFrom.Culture = System.Globalization.CultureInfo.CurrentUICulture;
                txtFrom.DateInput.Culture = System.Threading.Thread.CurrentThread.CurrentCulture;

                txtTo.Culture = System.Globalization.CultureInfo.CurrentUICulture;
                txtTo.DateInput.Culture = System.Threading.Thread.CurrentThread.CurrentCulture;


                if (System.Globalization.CultureInfo.CurrentUICulture.ToString() == "fr-CA")
                {
                    //txtFrom.CultureInfo.CultureName = "fr-FR";
                    //txtTo.CultureInfo.CultureName = "fr-FR";

                    txtFrom.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_fr;
                    txtFrom.DateInput.DateFormat = sn.User.DateFormat;
                    txtFrom.DateInput.DisplayDateFormat = sn.User.DateFormat;
                    txtFrom.Calendar.DayCellToolTipFormat = "dddd dd, MMMM yyyy";

                    txtTo.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_fr;
                    txtTo.DateInput.DateFormat = sn.User.DateFormat;
                    txtTo.DateInput.DisplayDateFormat = sn.User.DateFormat;
                    txtTo.Calendar.DayCellToolTipFormat = "dddd dd, MMMM yyyy";
                }
                else
                {
                    //txtFrom.CultureInfo.CultureName = System.Globalization.CultureInfo.CurrentUICulture.ToString();
                    //txtTo.CultureInfo.CultureName = System.Globalization.CultureInfo.CurrentUICulture.ToString();

                    txtFrom.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_en;
                    txtFrom.DateInput.DateFormat = sn.User.DateFormat;
                    txtFrom.DateInput.DisplayDateFormat = sn.User.DateFormat;

                    txtTo.DatePopupButton.ToolTip = clsGridFilterMenu.PopupCalendarMsg_en;
                    txtTo.DateInput.DateFormat = sn.User.DateFormat;
                    txtTo.DateInput.DisplayDateFormat = sn.User.DateFormat;
                }



               LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmReportMaster, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
               GuiSecurity(this);
               GetUserReportsTypes();

               try
               {
                   cboReports.SelectedIndex = cboReports.Items.IndexOf(cboReports.Items.FindByValue(sn.Report.GuiId.ToString()));
               }
               catch
               {
               }

               //System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
               //System.Threading.Thread.CurrentThread.CurrentUICulture = ci ;


               if (sn.Report.FromDate != "")
               {
                   //this.txtFrom.Text = Convert.ToDateTime(sn.Report.FromDate).ToString("MM/dd/yyyy");
                   this.txtFrom.SelectedDate = ConvertStringToDateTime(Convert.ToDateTime(sn.Report.FromDate).ToString(sn.User.DateFormat), sn.User.DateFormat, CurrentUICulture);
               }
               else
               {
                   //this.txtFrom.Text = DateTime.Now.ToString("MM/dd/yyyy");
                   this.txtFrom.SelectedDate = ConvertStringToDateTime(DateTime.Now.ToString(sn.User.DateFormat), sn.User.DateFormat, CurrentUICulture);
               }


               if (sn.Report.ToDate != "")
               {
                   //this.txtTo.Text = Convert.ToDateTime(sn.Report.ToDate).ToString("MM/dd/yyyy");
                   this.txtTo.SelectedDate = ConvertStringToDateTime(Convert.ToDateTime(sn.Report.ToDate).ToString(sn.User.DateFormat), sn.User.DateFormat, CurrentUICulture);
               }
               else
               {
                   //this.txtTo.Text = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy");
                   this.txtTo.SelectedDate = ConvertStringToDateTime(DateTime.Now.AddDays(1).ToString(sn.User.DateFormat), sn.User.DateFormat, CurrentUICulture);
               }

               //this.txtFrom.Text=DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy hh:mm");
               //this.txtTo.Text=DateTime.Now.AddDays(1).ToString("MM/dd/yyyy hh:mm");




               this.tblSpeedViolation.Visible = false;
               this.tblCost.Visible = true;

               this.tblFilter.Visible = false;
               this.lblLandmarkCaption.Visible = false;
               this.ddlLandmarks.Visible = false;
               this.tblPoints.Visible = false;
               this.tblDriverOptions.Visible = false;
               trFleet.Visible = false;   
   
               CboFleet_Fill();
               cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));

               if ((sn.Report.FleetId != 0) && (sn.Report.FleetId!=-1))
               {
                  cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.Report.FleetId.ToString()));
                  CboVehicle_Fill(Convert.ToInt32(sn.Report.FleetId));

                  if (sn.Report.LicensePlate!="")
                     cboVehicle.SelectedIndex = cboVehicle.Items.IndexOf(cboVehicle.Items.FindByValue(sn.Report.LicensePlate.ToString()));

                  this.lblVehicleName.Visible = true;
                  this.cboVehicle.Visible = true;
               }
               else if (sn.User.DefaultFleet != -1)
               {
                  cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
                  CboVehicle_Fill(Convert.ToInt32(sn.User.DefaultFleet));
                  this.lblVehicleName.Visible = true;
                  this.cboVehicle.Visible = true;
               }
               else
               {
                  this.lblVehicleName.Visible = false;
                  this.cboVehicle.Visible = false;
               }

               //Devin Aecon Start
               if (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 18)
                   AddReportItem(new ListItem("Dispatch Report", "300"));
              
               
               //cboReports.SelectedIndex = cboReports.Items.IndexOf(cboReports.Items.FindByValue(sn.Report.GuiId.ToString()));

               if (Request.Cookies[sn.User.OrganizationId.ToString() + "ReportMasterExtendedNewSelectedIndex"] != null && Request.Cookies[sn.User.OrganizationId.ToString() + "ReportMasterExtendedNewSelectedIndex"].Value.Trim() != "")
               {
                   try
                   {
                       cboReports.SelectedIndex = int.Parse(Request.Cookies[sn.User.OrganizationId.ToString() + "ReportMasterExtendedNewSelectedIndex"].Value);
                   }
                   catch
                   {                       
                   }
               }
               
               ReportCriteria();

              // if (sn.User.OrganizationId == 1)
              // {
               //cboReports.Items.Add(new ListItem("Fleet Monthly Utilization Report", "42"));
               //cboReports.Items.Add(new ListItem("Average Days Utilized Per Vehicle", "43"));
               //cboReports.Items.Add(new ListItem("Average Service Hrs for Utilized Vehicle", "44"));
               //cboReports.Items.Add(new ListItem("Average Engine ON Hrs for Utilized Vehicle", "45"));
               //cboReports.Items.Add(new ListItem("Unnecessary Idling Hrs. Per Vehicle Per Month", "46"));
               //cboReports.Items.Add(new ListItem("Total Unnecessary Idling Fuel Costs", "47"));
               //cboReports.Items.Add(new ListItem("Average Travelled Distance ", "48"));
                   //cboReports.Items.Add(new ListItem("Geozone Report", "22"));
                   //cboReports.Items.Insert(13, new ListItem("Fleet Utilization Report - Weekday", "15"));
                   //cboReports.Items.Insert(14, new ListItem("Fleet Utilization Report - Weekly", "16"));
              // }


               //if (sn.User.OrganizationId == 123)
               //{
               //    cboReports.Items.Add(new ListItem("Speed Violation Summary Report", "75"));
               //}

               //if (sn.User.OrganizationId == 570)
               //{
               //    //cboReports.Items.Add(new ListItem("Vehicle Status Report (HeartBeat)", "69"));
               //    //cboReports.Items.Add(new ListItem("Activity Outside Landmark", "73"));

               //    //cboReports.Items.Add(new ListItem("Worksite Details Report", "74"));
               //    //cboReports.Items.Add(new ListItem("Worksite Activity Report - July", "70"));
               //    //cboReports.Items.Add(new ListItem("Timesheet Validation Details Report - July", "71"));
               //}

               //if (sn.User.OrganizationId == 489)
               //{
               //    cboReports.Items.Add(new ListItem("Worksite Details Report", "74"));
               //}

		//if (sn.User.OrganizationId == 18)
                 //  AddReportItem(new ListItem("Green House Gas Emission Summary Report", "93"));
            }
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

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
            

            if (sn.Misc.DsReportAllFleets != null && sn.Misc.DsReportAllFleets.Tables.Count > 0 && sn.Misc.DsReportAllFleets.Tables[0].Rows.Count > 0)
            {
                sn.Misc.DsReportAllFleets.Tables[0].DefaultView.Sort = "FleetName";
                this.lstUnAss.DataSource = sn.Misc.DsReportAllFleets.Tables[0].DefaultView;
                lstUnAss.DataBind();
            }
            else
            {
                dsFleets.Tables[0].DefaultView.Sort = "FleetName";
                this.lstUnAss.DataSource = dsFleets.Tables[0].DefaultView;
                lstUnAss.DataBind();
                sn.Misc.DsReportAllFleets = dsFleets;
            }

            if (sn.Misc.DsReportSelectedFleets != null && sn.Misc.DsReportSelectedFleets.Tables.Count > 0)
            {
                this.lstAss.DataSource = sn.Misc.DsReportSelectedFleets.Tables[0].DefaultView;
                lstAss.DataBind();
            }
             
            
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

         }
      }

      private void CboVehicle_Fill(int fleetId)
      {
         try
         {
            cboVehicle.Items.Clear();

            DataSet dsVehicle = new DataSet();
            
            string xml = "";

            using (ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet())
            {
               if (objUtil.ErrCheck(dbf.GetVehiclesShortInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), false))
                  if (objUtil.ErrCheck(dbf.GetVehiclesShortInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), true))
                  {
                     System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No vehicles for fleet:" + fleetId.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                     cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));
                     return;
                  }
            }

            if (String.IsNullOrEmpty(xml))
            {
               System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No vehicles for fleet:" + fleetId.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
               cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));
               return;
            }

            dsVehicle.ReadXml(new StringReader(xml));

            cboVehicle.DataSource = dsVehicle;
            cboVehicle.DataBind();

            cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

         }
      }

      protected void cmdShow_Click(object sender, System.EventArgs e)
      {
        
            CreateReportParams("frmReportViewer.aspx");
        
      }

      private void cboVehicle_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         this.lblMessage.Text = "";
      }

      protected void cboReports_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         try
         {

             HttpCookie aCookie = new HttpCookie(sn.User.OrganizationId.ToString() + "ReportMasterExtendedNewSelectedIndex");
             aCookie.Value = cboReports.SelectedIndex.ToString();
             aCookie.Expires = DateTime.Now.AddYears(1);
             Response.Cookies.Add(aCookie);

             trFleet.Visible = true;
             ReportCriteria();

             //if (Convert.ToInt32(cboReports.SelectedItem.Value) < 10000 && Convert.ToInt32(cboReports.SelectedItem.Value) > 89 && Convert.ToInt32(cboReports.SelectedItem.Value) != 103)
            // {
            //     this.cmdSchedule.Enabled = false;
            //     this.cmdViewScheduled.Enabled = false;
            // }
            // else
             //{
                 this.cmdSchedule.Enabled = true ;
                 this.cmdViewScheduled.Enabled = true;
            // }
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

         }
      }

      /// <summary>
      /// Show/hide controls according to selected report
      /// </summary>
      private void ReportCriteria()
      {
        
      
         this.cboVehicle.Enabled = true;
         FleetVehicleShow(true);

         // build descr. name
         this.LabelReportDescription.Text = "";

         string resourceDescriptionName = "";
         string filter = String.Format("GuiId = '{0}'", this.cboReports.SelectedValue);
         DataRow[] rowsReport = null;
         if (Util.IsDataSetValid(sn.Report.UserExtendedReportsDataSet))
         {
             rowsReport = sn.Report.UserExtendedReportsDataSet.Tables[0].Select(filter);
            if (rowsReport != null && rowsReport.Length > 0)
            {
               resourceDescriptionName = String.Format("Description_{0}", rowsReport[0]["ReportTypesName"]);
               this.LabelReportDescription.Text = base.GetResource(resourceDescriptionName);
               if (String.IsNullOrEmpty(this.LabelReportDescription.Text))
                  this.LabelReportDescription.Text = rowsReport[0]["GuiName"].ToString();
            }
         }
         this.lblToTitle3.Visible = true;
         this.txtTo.Visible = true;
         this.lblToTitle3.Visible = true;
         this.txtTo.Visible = true;
         this.txtFrom.Visible = true;
         this.lblFromTitle3.Visible = true;  
         this.cboFleet.Enabled = true;
         this.tblCost.Visible = false ;
         trFleet.Visible = true;
         this.tblFleets.Visible = false;
         this.tblFilter.Visible = false;
         tblIgnition.Visible = false;
         tblViolationReport.Visible = false;
         this.lblGeozoneCaption.Visible = false;
         this.ddlGeozones.Visible = false ;
         this.tblSpeedViolation.Visible = false;
         tblPoints.Visible = false;
         tblViolationReport.Visible = false;
         tblFuelCost.Visible = false;
         tblIdlingThreshold.Visible = false ;
         this.tblDriverOptions.Visible = false;
         tblSpeedThreshold.Visible = false;
 
        //Devin Aecon Start
         cboFormat.Enabled = true;
         trDispatch.Visible = false;
          //End


	
         this.cmdSchedule.Visible  = true;
         this.cmdViewScheduled.Visible = true;


         if (sn.User.OrganizationId == 570)
         {
             this.chkActiveVehicles.Visible = true ;
             this.chkActiveVehicles.Enabled = true;
             this.chkActiveVehicles.Checked = true;   
         }
         else
            this.chkActiveVehicles.Visible = false;
   
         switch (this.cboReports.SelectedValue)
         {
                  
             case "29":
             case "55":
             case "118":
             
                 FleetVehicleShow(false);
                 break; 
            case "31":
                this.cboVehicle.Enabled = true;
                this.lblVehicleName.Enabled = true;
                this.tblSpeedViolation.Visible = false;
                this.tblCost.Visible = true;
               break;
           case "32":
           case "37":
               this.tblFleets.Visible = true;
               this.cboFleet.Enabled = false ;
               this.cboVehicle.Enabled = false;
               this.lblVehicleName.Enabled = false;
               this.tblSpeedViolation.Visible = false ;
               this.tblCost.Visible = true ;
               this.tblFilter.Visible = true;   
               break;

           case "33":
           case "35":
           case "80":
           case "81":
           case "84":
           case "85":
           
               this.tblFleets.Visible = true;
               this.cboFleet.Enabled = false;
               this.cboVehicle.Enabled = false;
               this.lblVehicleName.Enabled = false;
               this.tblSpeedViolation.Visible = true;
               this.tblCost.Visible = false;
               this.tblFilter.Visible = true;
               break;

           case "38":

               FleetVehicleShow(false);  
               tblIgnition.Visible = true ;
               break;

           case "39":

               this.cboVehicle.Enabled = false;
               tblIgnition.Visible = true;
               break;

           case "42":
          
               this.lblToTitle3.Visible = false ;
               this.txtTo.Visible = false;
               this.cboVehicle.Enabled = false;
               this.lblVehicleName.Enabled = false;
               this.tblCost.Visible = false ;
              break ;

           case "43":
           case "44":
           case "45":
           case "46":
              this.lblToTitle3.Visible = false;
              this.txtTo.Visible = false;
              this.cboVehicle.Enabled = false;
              this.lblVehicleName.Enabled = false;
              this.cboFleet.Enabled = false;
              this.tblCost.Visible = false;
              break;

           case "47":
              this.lblToTitle3.Visible = false;
              this.txtTo.Visible = false;
              this.cboVehicle.Enabled = false;
              this.lblVehicleName.Enabled = false;
              this.cboFleet.Enabled = false;
              this.tblCost.Visible = true;
              break;

           case "49":
           case "58":
           case "76":
           case "77":
           case "121":
              this.lblToTitle3.Visible = false;
              this.txtTo.Visible = false;
              this.lblFromTitle3.Visible = false;
              this.txtFrom.Visible = false;
              this.cboVehicle.Enabled = false;
              this.lblVehicleName.Enabled = false;
              this.cboFleet.Enabled = false;
              this.tblCost.Visible = false;
              break;

           case "52":
              this.lblToTitle3.Visible = true;
              this.txtTo.Visible = true ;
              this.lblFromTitle3.Visible = true;
              this.txtFrom.Visible = true;
              this.cboVehicle.Enabled = true;
              this.lblVehicleName.Enabled = true;
              this.cboFleet.Enabled = true;
              this.tblCost.Visible = false;
              break;
           case "56":
              this.cboVehicle.Enabled = true;
              tblIgnition.Visible = true;
              break;

           case "57":
              this.cboVehicle.Enabled = false ;
              this.tblViolationReport.Visible = true;
              break;
           case "63":
              this.cboVehicle.Enabled = true;
              break;
          

           case "65":
           case "70":
          
           case "73":
           case "90":
           case "91":
           case "10021":
           case "10034":
             //this.lblLandmarkCaption.Visible = true ;
             //this.ddlLandmarks.Visible = true ;
             this.cboVehicle.Enabled = false;
             //LoadLandmarks();
              break;
           case "71":
           case "10010":
           case "10031": 
           case "10032":
           case "10033": 
              this.cboVehicle.Enabled = false;
              sn.Report.XmlParams = "3";
              break; 
           case "64":
           case "66":
           case "67":
           case "68":
           case "69":
           case "79":
              // this.lblToTitle3.Visible = false;
              //this.txtTo.Visible = false;
              //this.lblFromTitle3.Visible = false;
              //this.txtFrom.Visible = false;
              this.cboVehicle.Enabled = false;
            //  this.lblGeozoneCaption.Visible = true;
            //  this.ddlGeozones.Visible = true;  
              break;

           case "72":
           case "123":
              trFleet.Visible = true;
              this.cboVehicle.Enabled = false;
              this.tblPoints.Visible = true;
              tblViolationReport.Visible = true;
              this.cboVehicle.Enabled = false;   
              break;

           case "75":
              this.cboVehicle.Enabled = false;
              this.lblVehicleName.Enabled = false;
              this.tblSpeedViolation.Visible = true;
              break;
           case "92":
           case "93":
           case "96":
           case "115":
           case "116":
           case "117":
           case "119":
           case "120":
           case "10022":
           case "10023":
           case "10037":
           case "10069":
              this.cboVehicle.Enabled = false;
              break;
           case "94":
           case "99":
              this.cboVehicle.Enabled = false;
              this.txtFrom.Visible = false;
              this.txtTo.Visible = false;
              break;


           case "100":
           case "102":
        
              this.cboFleet.Enabled = false;     
              this.cboVehicle.Enabled = false;
              break;
           case "106":
              tblFuelCost.Visible = true;
              this.cboVehicle.Enabled = false;
              break;

           case "107":
              LoadDrivers();
              tblDriverOptions.Visible = true;
              tblIdlingThreshold.Visible = true;  
              this.cboVehicle.Enabled = false;
              break; 
             case "110":
              LoadDrivers();
              tblDriverOptions.Visible = true;
              tblSpeedThreshold.Visible = true; 
              break;

             case "111":
              optReportBased.SelectedIndex = 1;
              FleetVehicleShow(false );
              break;

             case "113":
              tblIdlingThreshold.Visible = true;
              this.cboVehicle.Enabled = false;
              break;

             case "114":
              LoadDrivers();
              tblDriverOptions.Visible = true;
              this.cboVehicle.Enabled = false;
              break;

           //Devin Aecon
           case "300":
             //chkShowDriver.Visible = true;
             cboFormat.Enabled = false;
             trDispatch.Visible = true;
             radDispatch.SelectedIndex = 0;
             FleetVehicleShow(false);
             break;

	    case "126":
             this.cboVehicle.Enabled = false;
             this.cmdSchedule.Visible = false;
             this.cmdViewScheduled.Visible = false;
            break;	

         }

      }

      /// <summary>
      /// Show or hide fleet / vehicle lables and ddl
      /// </summary>
      /// <param name="showControls"></param>
      private void FleetVehicleShow(bool showControls)
      {
         this.cboFleet.Visible = showControls;
         this.lblFleet.Visible = showControls;
         this.lblVehicleName.Visible = showControls;
         this.cboVehicle.Visible = showControls;

         if (ShowOrganizationHierarchy)
         {
             ReportBasedOption();
             this.vehicleSelectOption.Visible = showControls;
         }
      }


      private void LoadLandmarks()
      {
          DataSet dsLandmarks = new DataSet();
          string xmlResult = "";

          using (ServerDBOrganization.DBOrganization organ = new ServerDBOrganization.DBOrganization())
          {

              if (objUtil.ErrCheck(organ.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), false))
                  if (objUtil.ErrCheck(organ.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), true))
                  {
                      System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Landmarks for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                      //RedirectToLogin();
                      return;
                  }


              //if (objUtil.ErrCheck(organ.GetOrganizationLandmark_Public(sn.UserID, sn.SecId, sn.User.OrganizationId, false, ref xmlResult), false))
              //    if (objUtil.ErrCheck(organ.GetOrganizationLandmark_Public(sn.UserID, sn.SecId, sn.User.OrganizationId, false, ref xmlResult), true))
              //    {
              //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Landmarks for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
              //        //RedirectToLogin();
              //        return;
              //    }
          }

          if (String.IsNullOrEmpty(xmlResult))
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Landmarks for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
              return;
          }

          dsLandmarks.ReadXml(new StringReader(xmlResult));

          this.ddlLandmarks.Items.Clear();

          if (Util.IsDataSetValid(dsLandmarks))
          {
              this.ddlLandmarks.DataSource = dsLandmarks;
              this.ddlLandmarks.DataBind();
              this.ddlLandmarks.Items.Insert(0, new ListItem(base.GetLocalResourceObject("ddlLandmarks_Item_0").ToString(), "-1"));
          }
          else
              this.ddlLandmarks.Items.Insert(0, new ListItem(base.GetLocalResourceObject("ddlLandmarks_NoAvailable").ToString(), "-100"));


          if ((sn.Report.LandmarkName != "") && (sn.Report.LandmarkName != " -1"))
              ddlLandmarks.SelectedIndex = ddlLandmarks.Items.IndexOf(ddlLandmarks.Items.FindByValue(sn.Report.LandmarkName.ToString()));

      }


      

      /// <summary>
      /// Get user reports dataset from session, if not valid - use web method
      /// </summary>
      private void  GetUserReportsTypes()
      {
          string xml = "";
          Int16 categoryId = 1;

          if (sn.User.OrganizationId == 123) //CN
              categoryId = 1;
          if (sn.User.OrganizationId == 570) //Brickman
              categoryId = 4;
          else if (sn.SuperOrganizationId == 382) //Wex
              categoryId = 2;
          else if (sn.User.OrganizationId == 622) //CP Rail
              categoryId = 3;
          else if (sn.User.OrganizationId == 18) //Aecon
              categoryId = 5;
          else if ((sn.User.OrganizationId == 951) || (sn.User.OrganizationId == 480)) //UP
              categoryId = 6;
          else if (sn.User.OrganizationId == 327) //Badger Daylighting Inc
              categoryId = 7;
          else if (sn.User.OrganizationId == 489) //Graham Construction
              categoryId = 8;
          else if (sn.User.OrganizationId == 999620) //Datum Exploration Ltd.
              categoryId = 10;
          else if (sn.User.OrganizationId == 698) //CNTL
              categoryId = 11;
          else if (sn.User.OrganizationId == 999630) //MTSAllstream
              categoryId = 12;
          else if (sn.User.OrganizationId == 999603) //E80 Plus Constructors
              categoryId = 13;
          else if (sn.User.OrganizationId == 999693) //Mr. Rooter of Ottawa
              categoryId = 14;
          else if (sn.User.OrganizationId == 480) //SFM 2000
              categoryId = 15;
          else if (sn.User.OrganizationId == 999620) //SA Exploration (Canada) Ltd (Datum)
              categoryId = 16;
          else if (sn.User.OrganizationId == 999692) //Willbros
              categoryId = 17;
          else if (sn.User.OrganizationId == 999650) //Transport SN
              categoryId = 18;
          else if (sn.User.OrganizationId == 952) //G4S
              categoryId = 19;
          else if (sn.User.OrganizationId == 999695) //VanHoute 
              categoryId = 20;
          else if (sn.User.OrganizationId == 999994) //BNSF Railway 
              categoryId = 21;
          //else if (sn.User.OrganizationId == ?????) // Assigned in report table
          //    categoryId = 22;
          //else if (sn.User.OrganizationId == ?????) // Assigned in report table
          //    categoryId = 23;
          //else if (sn.User.OrganizationId == ?????) // Assigned in report table
          //    categoryId = 24;
          else if (sn.User.OrganizationId == 1000010) //Sperry 
              categoryId = 25;
	  else if (sn.User.OrganizationId == 1000041) //Milton
              categoryId = 26;
 else if (sn.User.OrganizationId == 882) //Sintra Inc.
              categoryId = 28;
       
	 else if (sn.User.OrganizationId == 563) //Cummins Eastern Canada LP
              categoryId = 32;
 else if (sn.User.OrganizationId ==  1000088) //City of St. John's
              categoryId = 36;
else if (sn.User.OrganizationId ==  1000096) //Bridges & Tunnels
              categoryId = 37;
else if (sn.User.OrganizationId ==  1000110) //Bell Aliant
              categoryId = 38;
          else if (sn.User.OrganizationId == 1000097) //OmniTrax
              categoryId = 39;
          else if (sn.User.OrganizationId == 1000120) //Beacon Roofing Supply Canada
              categoryId = 40;
          else if (sn.User.OrganizationId == 999722) //Superior Plus Winroc
              categoryId = 41;
          else if (sn.User.OrganizationId == 1000056) //Jean Fournier Inc
              categoryId = 42;
          else if (sn.User.OrganizationId == 342) //Strongco Inc
              categoryId = 43;
          else if (sn.User.OrganizationId == 1000142) //Railworks
              categoryId = 44;
          else if (sn.User.OrganizationId == 1000148) //DiCAN
              categoryId = 45;
          else if (sn.User.OrganizationId == 1000144) //Ville De Pointe-Claire (Securite)
              categoryId = 46;
          else if (sn.User.OrganizationId == 999646) //Ville de Vaudreuil-Dorion
              categoryId = 47;
          else if (sn.User.OrganizationId == 1000164) //Town of Georgina
              categoryId = 48;
          else if (sn.User.OrganizationId == 664) //PVS
              categoryId = 49;
          else if (sn.User.OrganizationId == 1000141) //Gazzola
              categoryId = 50;	
	
          if (Util.IsDataSetValid(sn.Report.UserExtendedReportsDataSet))
          {
              this.cboReports.DataSource = sn.Report.UserExtendedReportsDataSet ;
              this.cboReports.DataBind();
          }
          else
          {
              DataSet dsReports = new DataSet();
             
              using (ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem())
              {
                  if (objUtil.ErrCheck(dbs.GetUserReportsByCategory(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, categoryId, ref xml), false))
                      if (objUtil.ErrCheck(dbs.GetUserReportsByCategory(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, categoryId, ref xml), true))
                      {
                          System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No GUI Controls setttngs for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                          return;
                      }
              }

              if (String.IsNullOrEmpty(xml))
              {
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "No GUI Controls settings for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                  // return;
              }
              else
              {
                               ////    switch (sn.User.OrganizationId)               //if (sn.User.OrganizationId == 999695)
              ////    {
              ////        case 123:                                 // 1:CN/123
              ////            xml = xml.Replace("70", "10021");     // Sensor Avtivity Report
              ////            break;
              ////        case 951:                                 // 6:UP/951
              ////            xml = xml.Replace("70", "10021");     // Sensor Avtivity Report
              ////            break;
              ////        case 622:                                 // 3:CP Rail/622
              ////            xml = xml.Replace("90", "10021");     // Sensor Avtivity Report
              ////            break;
              ////        case 999603:                              // 13:E80 Plus Constructors/999603
              ////            xml = xml.Replace("91", "10021");     // Sensor Avtivity Report
              ////            break;
              ////        case 999695:                              // Van Houtte Coffee Services Inc/999695
              ////case 563:   	
              ////            xml = xml.Replace("119", "10022");    // Speed Distribution Report
              ////            xml = xml.Replace("92", "10037");     // Activity Summary & Green House Gas Emission Report
              ////            break;
              ////        case 1000010:
              ////            xml = xml.Replace("70", "10031");     // Sensor Activities Report w. Sensor name list
              ////            xml = xml.Replace("71", "10033");     // Truck Utilization Summary Report
              ////            break;

              ////        default:
              ////            //xml = xml.Insert(xml.IndexOf("</UserGroup>"), "<GetUserReports>\n<ReportTypesName>Sensor Activity Report</ReportTypesName>\n<GuiId>10021</GuiId>\n<GuiName>Sensor Activity Report</GuiName>\n<ReportTypesId>183</ReportTypesId>\n</GetUserReports>\n");
              ////            break;
              ////    }


                  dsReports.ReadXml(new StringReader(xml));
                  this.cboReports.DataSource = dsReports;
                  sn.Report.UserExtendedReportsDataSet = dsReports;
                  this.cboReports.DataBind();
              }

          }



          //if (categoryId == 4)
          //    cboReports.Items.Add(new ListItem("Fleet Membership Report By User", "76"));

          if (sn.User.OrganizationId == 999695 || categoryId == 20)                                        // 20: 999695: Van Houtte Coffee Services Inc
              AddReportItem(new ListItem("Quarterly Mileage report", "10034"));

          if (categoryId == 12)
          {
              AddReportItem(new ListItem("Activity Summary and Green House Gas Report", "92"));
              AddReportItem(new ListItem("Green House Gas Emission Summary Report", "93"));
              //cboReports.Items.Add(new ListItem("Activity Summary and Green House Gas Report", "92"));
              //cboReports.Items.Add(new ListItem("Green House Gas Emission Summary Report", "93"));
          }
      }

      private void AddReportItem(ListItem NewItem)
      {
          Boolean hasAdd = false;
          for (int index = 0; index < cboReports.Items.Count; index++)
          {
              if (cboReports.Items[index].Text.CompareTo(NewItem.Text) > 0)
              {
                  cboReports.Items.Insert(index, NewItem);
                  hasAdd = true;
                  break;
              }
          }
          if (!hasAdd)
          {
              cboReports.Items.Add(NewItem);
          }
      }     

      protected void cboFleet_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (Convert.ToInt32(cboFleet.SelectedValue) != -1)
         {
            this.cboVehicle.Visible = true;
            this.lblVehicleName.Visible = true;
            CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedValue));
         }
      }

     
      /// <summary>
      /// Create report params and redirect to the next page
      /// </summary>
      /// <param name="redirectUrl">Web page to redirect to</param>
      private void CreateReportParams(string redirectUrl)
      {
          try
          {
              string strFromDate = this.txtFrom.SelectedDate.Value.ToString("MM/dd/yyyy") + " 12:00 AM";
              string strToDate = this.txtTo.SelectedDate.Value.ToString("MM/dd/yyyy") + " 12:00 AM"; 
              DateTime from, to;
              sn.Report.IsFleet = this.cboVehicle.SelectedValue == "0" ? true : false;
              
              const string dateFormat = "MM/dd/yyyy HH:mm:ss";

              this.lblMessage.Text = "";

              # region Validation

             if (!clsUtility.IsNumeric(this.txtCost.Text))
             {
                 this.lblMessage.Visible = true;
                 this.lblMessage.Text = "Cost should be numeric";
                 return;
             }
          
              System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
              //System.Threading.Thread.CurrentThread.CurrentUICulture = ci;

              from = Convert.ToDateTime(strFromDate, ci);
              to = Convert.ToDateTime(strToDate, ci);

              if (from >= to)
              {
                  this.lblMessage.Visible = true;
                  this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidDate");
                  return;
              }
              else
              {
                  this.lblMessage.Visible = false;
                  this.lblMessage.Text = "";
              }

              if ((this.cboVehicle.SelectedIndex == 0 && (this.cboReports.SelectedValue == "3" || this.cboReports.SelectedValue == "82" || this.cboReports.SelectedValue == "86")) ||
                  this.cboVehicle.SelectedIndex == -1)
              {
                  this.lblMessage.Visible = true;
                  this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_SelectVehicle");
                  return;
              }
              else
              {
                  this.lblMessage.Visible = false;
                  this.lblMessage.Text = "";
              }

              // Set Report period based on report.
              int reportDaysLimit = 31;
              string reportdaymsg = " one month ";

              if ("{10052|10034|10094}".IndexOf(this.cboReports.SelectedValue) > 0)       // year
              // Multi-Fleet Annual Mileage Summary Report     10052
              // Quarterly Mileage report (Quarter)            10034
              // Quarterly Mileage report (Annual)             10094
              {
                  reportDaysLimit = 367;
                  reportdaymsg = " one year ";
              }
              else if ("{10002|10011|10020}".IndexOf(this.cboReports.SelectedValue) > 0)  // Quarter
              // Activity Summary Report for Organization      10011
              // Activity Summary Report per Vehicle           10002
              // Individual Vehicle Mileage Report             10020
              {
                  reportDaysLimit = 100;
                  reportdaymsg = " one quarter ";
              }
              else                                                                       // Month
              // Others
              {
                  reportDaysLimit = 31;
                  reportdaymsg = " one month ";
              }

              TimeSpan ts = to - from;

              if (ts.Days > reportDaysLimit)
              {
                  this.lblMessage.Visible = true;
                  this.lblMessage.Text = "Please decrease report date range! A maximum of " + reportdaymsg + " allowed for selected report.";
                  return;
              }

              //if (sn.User.OrganizationId == 999695 && cboReports.SelectedItem.Value == "10034") {
              //    if (ts.Days > 42)
              //    {
              //        this.lblMessage.Visible = true;
              //        this.lblMessage.Text = "Please decrease report date range! A maximum of 6 weeks allowed for selected report.";
              //        return;
              //    }
              //}
              //else
              //{
              //    if (ts.Days > 31)
              //    {
              //        this.lblMessage.Visible = true;
              //        this.lblMessage.Text = GetLocalResourceObject("MessageRptDateRange").ToString();
              //        return;
              //    }
              //}

              DataSet ds = new DataSet();
              string xmlResult = "";
              using (CrystalRpt.CrystalRpt cr = new CrystalRpt.CrystalRpt())
              {
                  if (objUtil.ErrCheck(cr.OrganizationHistoryDateRangeValidation(sn.UserID, sn.SecId, from.ToString(), to.ToString(), ref xmlResult), false))
                      if (objUtil.ErrCheck(cr.OrganizationHistoryDateRangeValidation(sn.UserID, sn.SecId, from.ToString(), to.ToString(), ref xmlResult), true))
                      {
                          System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " OrganizationHistoryDateRangeValidation:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                          return;
                      }
              }

              if (String.IsNullOrEmpty(xmlResult))
              {
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "OrganizationHistoryDateRangeValidation:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                  return;
              }


              ds.ReadXml(new StringReader(xmlResult));
              if (ds.Tables[0].Rows[0]["InValidCall"].ToString() == "1")
              {
                  this.lblMessage.Visible = true;
                  this.lblMessage.Text = "Please decrease report date/time range! The from date should be greather than:" + System.DateTime.Now.AddDays(-Convert.ToInt16(ds.Tables[0].Rows[0]["MaximumDays"].ToString())).ToShortDateString();
                  return;
              }


              # endregion

              this.BusyReport.Visible = true;
             

              # region Reports
              switch (this.cboReports.SelectedValue)
              {
                  
                  case "31":
                      sn.Report.XmlParams = String.Format("{0}={1};", ReportTemplate.RptParamVehicleId, this.cboVehicle.SelectedValue.Trim());
                      sn.Report.XmlParams += String.Format("{0}={1};", ReportTemplate.RptParamCost, this.txtCost.Text);
                      break;
                  case "32":
                  case "37":
                      //sn.Report.XmlParams = String.Format("{0}={1};", ReportTemplate.RptParamCost, this.txtCost.Text);
                      sn.Report.XmlParams = String.Format("{0};{1}", this.txtCost.Text, this.txtColorFilter.Text);
                      break;
                  case "33":
                  case "35":
                  case "75":
                  case "80":
                  case "81":
                  case "84":
                  case "85":
                      //sn.Report.XmlParams = String.Format("{0}={1};", ReportTemplate.RptParamIncludeSummary, this.cboViolationSpeed.SelectedItem.Value);

                      sn.Report.XmlParams = String.Format("{0};{1}", this.cboViolationSpeed.SelectedItem.Value,this.txtColorFilter.Text );

                      break;

                  #region Organization summary
                  case "38":
                      sn.Report.XmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);
                      break;
                  # endregion
                  #region Activity Summary Report per Vehicle
                  case "39":
                  case "92":
                      sn.Report.XmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);
                      break;
                  # endregion


                  
                  case "47":
                      sn.Report.XmlParams = String.Format("{0}={1};", ReportTemplate.RptParamCost, this.txtCost.Text);
                      break;

                  #region Vehicle summary
                  case "56":
                      sn.Report.XmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);
                      break;
                  # endregion

                  case "57":
                  case "72":
                  case "123":
                      sn.Report.XmlParams = CreateViolationParameters(this.cboReports.SelectedValue);
                      break;

                  #region New Trips Summary Report per Vehicle
                  case "63":
                      // xmlParams = String.Format("{0}={1};{2}={3}",
                      //       ReportTemplate.RptParamLicensePlate, this.cboVehicle.SelectedValue,
                      //       ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);

                      sn.Report.XmlParams = this.cboVehicle.SelectedValue.Trim();
                     // sn.Report.XmlParams += ReportTemplate.MakePair(ReportTemplate.RptParamSensorId, this.optEndTrip.SelectedValue);


                      break;
                  # endregion


                  case "69":
                      sn.Report.XmlParams = String.Format("{0};{1}", this.chkActiveVehicles.Checked == true ? "1" : "0", sn.User.OrganizationId);
                      break;

                  case "73":
                      sn.Report.XmlParams = String.Format("{0};{1}", this.chkActiveVehicles.Checked == true ? "1" : "0", sn.Report.VehicleId);
                      break;

                  #region Worksite Activity Report
                  case "64":
                  case "66":
                  case "67":
                  case "77":
                  case "79":
                  case "100":
                  case "102":
                      sn.Report.XmlParams = this.chkActiveVehicles.Checked==true  ? "1": "0";
                    break;
                  # endregion
                  case "76":
                    sn.Report.XmlParams = sn.User.OrganizationId.ToString();   
                    break;
                  case "82":
                  case "86":
                      
                        DataSet dsVehicle = null;
                        string ConnnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
                        using (VLF.DAS.Logic.Vehicle dbVehicle = new VLF.DAS.Logic.Vehicle(ConnnectionString))
                        {
                            dsVehicle = dbVehicle.GetVehicleInfoByVehicleId(Convert.ToInt32(this.cboVehicle.SelectedValue));
                        }

                     
                        if (Util.IsDataSetValid(dsVehicle))
                        {
                            sn.Report.XmlParams = ReportTemplate.MakePair(ReportTemplate.RpDetailedTripFirstParamName, dsVehicle.Tables[0].Rows[0]["LicensePlate"].ToString());
                    
                        }

                        string convFromDate = from.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString(dateFormat);
                        string convToDate = to.AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString(dateFormat);
            
                        sn.Report.XmlParams += CreateTripDetailsParameteres(convFromDate, convToDate);
                      break;

                  case "106":
                      sn.Report.XmlParams = this.txtFuelCost.Text;  
                      break;
                  case "107":
                      sn.Report.XmlParams = this.ddlDrivers.SelectedItem.Value.ToString()+";"+this.cboIdlingThreshold.SelectedItem.Value.ToString();    
                      break;

                  case "110":
                      sn.Report.XmlParams = this.ddlDrivers.SelectedItem.Value.ToString() + ";" + this.cboSpeedThreshold.SelectedItem.Value.ToString();
                      break;

                  case "112":
                      FleetVehicleShow(false);
                      break;

                  case "113":
                      sn.Report.XmlParams =  this.cboIdlingThreshold.SelectedItem.Value.ToString();    
                      break;
                  case "114":
                      sn.Report.XmlParams = this.ddlDrivers.SelectedItem.Value.ToString();
                      break;

                
                  //Devin Aecon Start
                  #region Aecon Report
                  case "300":
                      sn.Report.XmlParams = radDispatch.SelectedValue;
                      break;
                  # endregion

                  //#region Timesheet Validation
                  //case "66":
                  //  sn.Report.XmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamGeozone, this.ddlGeozones.SelectedValue);
                  //  break; 
                  //#endregion

                  #region Server Report Section

                  default:

                      int pid = this.StringToInt32(this.cboReports.SelectedValue.ToString());
                      if (pid >= 10000) {
                          sn.Report.XmlParams = GenerateReportParameters_JSON(pid);
                      }
                      break;

                  #endregion

              }
              # endregion Reports

              sn.Report.GuiId = Convert.ToInt16(this.cboReports.SelectedValue);
              sn.Report.FromDate = from.ToString();
              sn.Report.ToDate = to.ToString();
              sn.Report.ReportFormat = Convert.ToInt32(this.cboFormat.SelectedValue);

              if (ShowOrganizationHierarchy && vehicleSelectOption.Visible && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyFleetId"]!="")
                  sn.Report.FleetId = Convert.ToInt32(Request.Form["OrganizationHierarchyFleetId"]);
              else
                  sn.Report.FleetId = Convert.ToInt32(this.cboFleet.SelectedValue);

              sn.Report.IsFleet = this.cboVehicle.SelectedValue == "0" ? true : false;
              sn.Report.VehicleId  = Convert.ToInt32(this.cboVehicle.SelectedValue);
              sn.Report.FleetName = this.cboFleet.SelectedItem.Text;
              sn.Report.ReportType = cboReports.SelectedValue;
              sn.Report.OrganizationHierarchyNodeCode = Request.Form["OrganizationHierarchyNodeCode"];
              
           }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);

         }

         Response.Redirect(redirectUrl);
      }

     


      
      protected void cmdViewScheduled_Click(object sender, EventArgs e)
      {
          Response.Redirect("../ReportsScheduling/frmScheduleReportList.aspx?back=frmReportMasterExtended.aspx");
      }

      protected void cmdSchedule_Click(object sender, EventArgs e)
       {
            CreateReportParams("../ReportsScheduling/frmReportScheduler.aspx?back=frmReportMasterExtended.aspx");
       }


      protected void cmdAdd_Click(object sender, EventArgs e)
      {

          DataSet ds = sn.Misc.DsReportAllFleets;
          if (sn.Misc.DsReportSelectedFleets ==null ||  sn.Misc.DsReportSelectedFleets.Tables.Count==0)  
              sn.Misc.DsReportSelectedFleets = ds.Clone();

          foreach (ListItem li in lstUnAss.Items)
          {
              if (li.Selected)
              {
                  DataRow dr = sn.Misc.DsReportSelectedFleets.Tables[0].NewRow();
                  dr["FleetId"] = li.Value;
                  dr["FleetName"] = li.Text;
                  sn.Misc.DsReportSelectedFleets.Tables[0].Rows.Add(dr);

                  DataRow[] drColl=ds.Tables[0].Select("FleetId='"+ li.Value+"'");
                  ds.Tables[0].Rows.Remove(drColl[0]);
              }

          }

          if (sn.Misc.DsReportSelectedFleets.Tables.Count > 0)
          {
              sn.Misc.DsReportSelectedFleets.Tables[0].DefaultView.Sort = "FleetName";
              this.lstAss.DataSource = sn.Misc.DsReportSelectedFleets.Tables[0].DefaultView;
              lstAss.DataBind();
          }
          else
          {
              lstAss.Items.Clear();  
              
          }

          if (ds.Tables.Count > 0)
          {
              ds.Tables[0].DefaultView.Sort = "FleetName";
              lstUnAss.DataSource = ds.Tables[0].DefaultView;
              lstUnAss.DataBind();
          }
          else
          {
              
              lstUnAss.Items.Clear();  
          }
          
          sn.Misc.DsReportAllFleets = ds;
      

      }
      protected void cmdAddAll_Click(object sender, EventArgs e)
      {
          sn.Misc.DsReportSelectedFleets = sn.Misc.DsReportAllFleets;
          sn.Misc.DsReportSelectedFleets.Tables[0].DefaultView.Sort = "FleetName";
          this.lstAss.DataSource =sn.Misc.DsReportSelectedFleets.Tables[0].DefaultView ;
          lstAss.DataBind();
          sn.Misc.DsReportAllFleets.Clear();
          lstUnAss.Items.Clear();
          
      }
      protected void cmdRemove_Click(object sender, EventArgs e)
      {
          DataSet ds = sn.Misc.DsReportSelectedFleets;
          if (sn.Misc.DsReportAllFleets == null || sn.Misc.DsReportAllFleets.Tables.Count == 0)
              sn.Misc.DsReportAllFleets = ds.Clone();

          
          foreach (ListItem li in lstAss.Items)
          {
              if (li.Selected)
              {
                  DataRow dr = sn.Misc.DsReportAllFleets.Tables[0].NewRow();
                  dr["FleetId"] = li.Value;
                  dr["FleetName"] = li.Text;


                  sn.Misc.DsReportAllFleets.Tables[0].Rows.Add(dr);

                  DataRow[] drColl = ds.Tables[0].Select("FleetId='" + li.Value + "'");
                  ds.Tables[0].Rows.Remove(drColl[0]);
              }

          
          }

          if (ds.Tables.Count > 0)
          {
              ds.Tables[0].DefaultView.Sort = "FleetName";
              this.lstAss.DataSource = ds.Tables[0].DefaultView;
              lstAss.DataBind();
          }
          else
          {
              lstAss.Items.Clear();  
          }

          if (sn.Misc.DsReportAllFleets.Tables.Count > 0)
          {
              sn.Misc.DsReportAllFleets.Tables[0].DefaultView.Sort = "FleetName";
              lstUnAss.DataSource = sn.Misc.DsReportAllFleets.Tables[0].DefaultView;
              lstUnAss.DataBind();
          }
          else
          {
              lstUnAss.Items.Clear();  
          }

          sn.Misc.DsReportSelectedFleets = ds;
      }

      protected void cmdRemoveAll_Click(object sender, EventArgs e)
      {
            
          sn.Misc.DsReportAllFleets = sn.Misc.DsReportSelectedFleets;
          sn.Misc.DsReportSelectedFleets.Clear() ;
          this.lstAss.DataSource = sn.Misc.DsReportSelectedFleets;
          lstAss.DataBind();
          CboFleet_Fill();
      }


      private string CreateViolationParameters(string reportGuiId)
      {
          int intCriteria = 0;

          if (this.chkSpeedViolation.Checked)
              intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_SPEED;
          if (this.chkHarshAcceleration.Checked)
              intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_HARSHACCELERATION;
          if (this.chkHarshBraking.Checked)
              intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_HARSHBRAKING;
          if (this.chkExtremeAcceleration.Checked)
              intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_EXTREMEACCELERATION;
          if (this.chkExtremeBraking.Checked)
              intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_EXTREMEBRAKING;
          if (this.chkSeatBeltViolation.Checked)
              intCriteria |= VLF.Reports.ReportGenerator.CT_VIOLATION_SEATBELT;

          StringBuilder Params = new StringBuilder();
          Params.Append(intCriteria);

          // violation summary
          if (reportGuiId == "72" || reportGuiId == "123")
          {
              Params.AppendFormat(";{0};{1};{2};{3};{4};{5};{6};{7}",
                 this.txtSpeed120.Text, this.txtSpeed130.Text, this.txtSpeed140.Text,
                 this.txtAccExtreme.Text, this.txtAccHarsh.Text, this.txtBrakingExtreme.Text,
                 this.txtBrakingHarsh.Text, this.txtSeatBelt.Text);
          }

          else
          {

              Params.Append("*");
              string tmpSpeed = "";
              switch (this.DropDownList1.SelectedValue)                     //(this.cboViolationSpeed.SelectedValue)
              {
                  case "1":
                      tmpSpeed = (sn.User.UnitOfMes == 1 ? "100" : "62");
                      break;
                  case "2":
                      tmpSpeed = (sn.User.UnitOfMes == 1 ? "105" : "65");
                      break;
                  case "3":
                      tmpSpeed = (sn.User.UnitOfMes == 1 ? "110" : "68");
                      break;
                  case "4":
                      tmpSpeed = (sn.User.UnitOfMes == 1 ? "120" : "75");
                      break;
                  case "5":
                      tmpSpeed = (sn.User.UnitOfMes == 1 ? "130" : "80");
                      break;
                  case "6":
                      tmpSpeed = (sn.User.UnitOfMes == 1 ? "140" : "85");
                      break;
              }
              Params.Append(tmpSpeed);
          }
          return Params.ToString();
      }

      protected void txtFrom_Load(object sender, EventArgs e)
      {
          txtFrom.DateInput.DateFormat = sn.User.DateFormat;
          txtTo.DateInput.DateFormat = sn.User.DateFormat;
      }

      /// <summary>
      /// Load geozones into ddl
      /// </summary>
      private void LoadGeozones()
      {
          DataSet dsGeozones = new DataSet();
          string xmlResult = "";

          using (ServerDBOrganization.DBOrganization organ = new ServerDBOrganization.DBOrganization())
          {

              if (objUtil.ErrCheck(organ.GetOrganizationGeozones(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), false))
                  if (objUtil.ErrCheck(organ.GetOrganizationGeozones(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), true))
                  {
                      System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Geozones for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                      //RedirectToLogin();
                      return;
                  }

              //if (objUtil.ErrCheck(organ.GetOrganizationGeozones_Public(sn.UserID, sn.SecId, sn.User.OrganizationId, false, ref xmlResult), false))
              //    if (objUtil.ErrCheck(organ.GetOrganizationGeozones_Public(sn.UserID, sn.SecId, sn.User.OrganizationId, false, ref xmlResult), true))
              //    {
              //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Geozones for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
              //        //RedirectToLogin();
              //        return;
              //    }
          }

          if (String.IsNullOrEmpty(xmlResult))
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Geozones for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
              return;
          }

          dsGeozones.ReadXml(new StringReader(xmlResult));

          this.ddlGeozones.Items.Clear();

          if (Util.IsDataSetValid(dsGeozones))
          {
              DataView view = dsGeozones.Tables[0].DefaultView;
              view.Sort = "GeozoneName";
              this.ddlGeozones.DataSource = view;
              this.ddlGeozones.DataBind();
              this.ddlGeozones.Items.Insert(0, new ListItem(base.GetLocalResourceObject("ddlGeozones_Item_0").ToString(), "-1"));
          }
          else
              this.ddlGeozones.Items.Insert(0, new ListItem(base.GetLocalResourceObject("ddlGeozones_NoAvailable").ToString(), "-100"));
      }


      /// <summary>
      /// Trip Details Parameteres
      /// </summary>
      /// <param name="xmlParams"></param>
      private string CreateTripDetailsParameteres(string fromDate, string toDate)
      {
          StringBuilder sb = new StringBuilder();
          sb.Append(ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripSecondParamName : ReportTemplate.RpFleetDetailedTripSecondParamName, fromDate));
          sb.Append(ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripThirdParamName : ReportTemplate.RpFleetDetailedTripThirdParamName, toDate));
          sb.Append(ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripFourthParamName : ReportTemplate.RpFleetDetailedTripFourthParamName, "True"));
          sb.Append(ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripFifthParamName : ReportTemplate.RpFleetDetailedTripFifthParamName, "True"));
          sb.Append(ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripSixthParamName : ReportTemplate.RpFleetDetailedTripSixthParamName, "True"));
          sb.Append(ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripSeventhParamName : ReportTemplate.RpFleetDetailedTripSeventhParamName, "True"));
          sb.Append(ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripEighthParamName : ReportTemplate.RpFleetDetailedTripEighthParamName, "True"));
          sb.Append(ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripNinthParamName : ReportTemplate.RpFleetDetailedTripNinthParamName, "True"));
          sb.Append(ReportTemplate.MakePair(this.cboVehicle.SelectedItem.Value == "0" ? ReportTemplate.RpDetailedTripTenthParamName : ReportTemplate.RpFleetDetailedTripTenthParamName, "3"));
          return sb.ToString();

      }


      /// <summary>
      /// Load drivers into ddl
      /// </summary>
      private void LoadDrivers()
      {
          DataSet dsDrivers = new DataSet();
          string xmlResult = "";

          using (ServerDBDriver.DBDriver drv = new global::SentinelFM.ServerDBDriver.DBDriver())
          {

              if (objUtil.ErrCheck(drv.GetAllDrivers(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), false))
                  if (objUtil.ErrCheck(drv.GetAllDrivers(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), true))
                  {
                      System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Drivers for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                      //RedirectToLogin();
                      return;
                  }
          }

          if (String.IsNullOrEmpty(xmlResult))
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Drivers for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
              return;
          }

          dsDrivers.ReadXml(new StringReader(xmlResult));

          this.ddlDrivers.Items.Clear();

          if (Util.IsDataSetValid(dsDrivers))
          {
              this.ddlDrivers.DataSource = dsDrivers;
              this.ddlDrivers.DataBind();
              this.ddlDrivers.Items.Insert(0, new ListItem(base.GetLocalResourceObject("ddlDrivers_Item_0").ToString(), "-1"));
          }
          else
              this.ddlDrivers.Items.Insert(0, new ListItem(base.GetLocalResourceObject("ddlDrivers_NoAvailable").ToString(), "-100"));


          if ((sn.Report.DriverId != 0) && (sn.Report.DriverId != -1))
              ddlDrivers.SelectedIndex = ddlDrivers.Items.IndexOf(ddlDrivers.Items.FindByValue(sn.Report.DriverId.ToString()));

      }

      private void ReportBasedOption()
      {
          if (optReportBased.SelectedItem.Value == "0")
          {
              trFleet.Visible = false;
              organizationHierarchy.Visible = true;
              sn.Report.OrganizationHierarchySelected = true;
          }
          else
          {
              trFleet.Visible = true;
              organizationHierarchy.Visible = false;
              sn.Report.OrganizationHierarchySelected = false;
          }
      }
      protected void optReportBased_SelectedIndexChanged(object sender, EventArgs e)
      {
          ReportBasedOption();
      }

        #region Server Report Section

        /// <summary>
        /// Build parameters string in json format 
        /// </summary>
        /// <returns></returns>
        public string GenerateReportParameters_JSON(int ReportID)      //string ReportName, string ReportPath, string ReportUri, string ReportType)
        {
            ReportID = Math.Abs(ReportID);

            StringBuilder sbp = new StringBuilder();

            // Basic parameters
            sbp.Append("reportid: " + ReportID.ToString() + ", ");             // this.cboReports.SelectedItem.Value + ", ");
            sbp.Append("reportformat: " + cboFormat.SelectedItem.Text + ", ");              // PDF; EXCEL; WORD;....   .SelectedValue.ToString()
            sbp.Append("reportformatcode: " + cboFormat.SelectedItem.Value + ", ");         // 1;   2;     3;   ....   .SelectedValue.ToString()

            // Credencial Information
            sbp.Append("username: bsmreports" + ", ");
            sbp.Append("password: T0ybhARQ" + ", ");
            sbp.Append("domain: production" + ", ");

            // Application Logon User
            sbp.Append("userid: " + sn.UserID + ", ");

            //Time zone
            sbp.Append("timezone: GMT" + sn.User.TimeZone.ToString() + ", ");

            // User language
            sbp.Append("language: " + sn.SelectedLanguage + ", ");       //  SystemFunctions.GetStandardLanguageCode(HttpContext.Current) + ", ");

            // Organization
            sbp.Append("organization: " + sn.User.OrganizationId + ", ");

            if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyFleetId"] != "")
            {
                sbp.Append("fleetid:  " + Request.Form["OrganizationHierarchyFleetId"] + ", ");
            }
            else
            {
                sbp.Append("fleetid: " + cboFleet.SelectedItem.Value + ", ");
                sbp.Append("fleetname: " + cboFleet.SelectedItem.Text + ", ");
            }

            if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyFleetId"] != "")
                sn.Report.FleetId = Convert.ToInt32(Request.Form["OrganizationHierarchyFleetId"]);
            else
                sn.Report.FleetId = Convert.ToInt32(this.cboFleet.SelectedValue);

            //// Vehicle
            //if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyBoxId"] != "")
            //    standardReport.cboVehicle = Request.Form["OrganizationHierarchyBoxId"];
            //else
            //    standardReport.cboVehicle = cboVehicle.SelectedValue; //cboVehicle.SelectedValue

            //if (ShowOrganizationHierarchy && optReportBased.SelectedIndex == 0 && Request.Form["OrganizationHierarchyVehicleDescription"] != "")
            //    standardReport.cboVehicle_Name = Request.Form["OrganizationHierarchyVehicleDescription"];
            //else
            //    standardReport.cboVehicle_Name = cboVehicle.SelectedItem.Text;
            //sbp.Append("vehicleid: " + cboFleet.SelectedItem.Value + ", ");
            //sbp.Append("vehiclename: " + cboFleet.SelectedItem.Text + ", ");
            // Date range

            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");

            string sFrom = this.txtFrom.SelectedDate.Value.ToString("MM/dd/yyyy") + " 12:00 AM";
            string sTo = this.txtTo.SelectedDate.Value.ToString("MM/dd/yyyy") + " 12:00 AM";

            DateTime dtFrom = Convert.ToDateTime(sFrom, ci);
            DateTime dtTo = Convert.ToDateTime(sTo, ci);

            sbp.Append("datefrom: " + dtFrom.ToString("MM/dd/yyyy hh:mm:ss tt") + ", ");
            sbp.Append("dateto: " + dtTo.ToString("MM/dd/yyyy hh:mm:ss tt") + ", ");

            sbp.Append("unitofvolume: " + sn.User.VolumeUnits + ", ");
            sbp.Append("unitofspeed: " + sn.User.UnitOfMes + ", ");


            // Vehicle infor
            if (this.cboVehicle.Visible && this.cboVehicle.Enabled)
            {
                sbp.Append("vehiclename: " + this.cboVehicle.SelectedItem.Text + ", ");
                sbp.Append("licenseplate: " + this.cboVehicle.SelectedItem.Value + ", ");
            }

            // sensorNum = 3 (default)
            if (this.optEndTrip.Visible && this.optEndTrip.Enabled)
            {
                if (optEndTrip.SelectedIndex >= 0)
                    sbp.Append("sensornumber: " + optEndTrip.SelectedValue + ", ");
                else
                    sbp.Append("sensornumber: 3, ");
            }
            //}
            //else
            //{
            //    sbp.Length = 0;
            //    sbp.Capacity = 0;
            //}

            if (sbp.Length > 0)
                return "{" + sbp.ToString() + "}";
            else
                return "";

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="msReportID"></param>
        /// <returns></returns>
        public bool GetReportDetail(string msReportID)
        {
            string msMessage = "";

            //try
            //{
            //    string cnStr = ConfigurationManager.ConnectionStrings["DBReportConnectionString"].ConnectionString;

            //    using (SqlConnection connection = new SqlConnection(cnStr))
            //    {
            //        connection.Open();

            //        using (SqlCommand command = new SqlCommand("sp_ReportDetails", connection))
            //        {
            //            command.CommandType = CommandType.StoredProcedure;
            //            command.Parameters.Add("@ReportID", SqlDbType.Int);
            //            command.Parameters["@ReportID"].Value = msReportID;

            //            using (SqlDataReader reader = command.ExecuteReader())
            //            {
            //                if (reader.Read())
            //                {
            //                    //msReportCategory = reader[0].ToString();
            //                    //msReportUri = reader[1].ToString();
            //                    //msReportPath = reader[2].ToString();
            //                    //msReportName = reader[3].ToString();
            //                    //msReportPage = reader[4].ToString();
            //                    //msReportType = reader[5].ToString();
            //                    msMessage = "";
            //                }
            //                else
            //                {
            //                    //msReportCategory = "";
            //                    //msReportUri = "";
            //                    //msReportPath = "";
            //                    //msReportName = "";
            //                    //msReportPage = "";
            //                    //msReportType = "";
            //                    //msMessage = "Report not found";
            //                }
            //            }
            //        }
            //    }
            //}
            //catch (SqlException Ex)
            //{
            //    msMessage = Ex.Message.ToString();
            //}
            //catch (Exception Ex)
            //{
            //    msMessage = Ex.Message.ToString();
            //}
            //finally
            //{

            //}

            return (msMessage == string.Empty) ? true : false;
        }

        /// <summary>
        /// for async call parameter
        /// </summary>
        /// <param name="iResult"></param>
        private void wsCallback(IAsyncResult iResult) { }

        /// <summary>
        /// Overloading
        /// </summary>
        /// <param name="DateValue"></param>
        /// <param name="Dateformat"></param>
        /// <returns></returns>
        public DateTime ConvertStringToDateTime(string DateValue, string Dateformat)
        {
            return ConvertStringToDateTime(DateValue, Dateformat, System.Globalization.CultureInfo.CurrentUICulture.ToString());
        }

        /// <summary>
        /// Format Date/Time accouding Current UI Culture.
        /// Support two format: MM/DD/YYYY hh:mm:ss AM|PM (12h, Default) for EN and DD/MM/YYYY HH:MM:SS (24h) for FR. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <param name="cultureinfo"></param>
        /// <returns></returns>
        public DateTime ConvertStringToDateTime(string value, string format, string cultureinfo)
        {

            CultureInfo culture = new CultureInfo(cultureinfo);
            DateTime date = DateTime.Now;
            string err = "";

            try
            {
                if (format.ToLower().IndexOf("hh") >= 0)
                    value = Convert.ToDateTime(value).ToString(format);

                date = DateTime.ParseExact(value, format, null);
                err = "";
            }
            catch (FormatException fx)
            {
                err = fx.Message;
                date = DateTime.Now;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                date = DateTime.Now;
            }
            finally
            {
            }

            return date;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public Int32 StringToInt32(string value)
        {
            Int32 i = 0;
            if (Int32.TryParse(value, out i))
                return i;
            else
                return 0;
        }

      #endregion

   }
}
