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
      

      protected void Page_Load(object sender, EventArgs e)
      {
         try
         {
             sn.Report.ReportActiveTab = 1;  
           
           

            //Show Busy Message
            cmdShow.Attributes.Add("onclick", BusyReport.ShowFunctionCall);
            
            this.BusyReport.Title = (string)base.GetLocalResourceObject("BusyPleaseWaitMessage");
            this.BusyReport.Text = (string)base.GetLocalResourceObject("BusyPreparingMessage");
            

            
            int i = 0;

            if (!Page.IsPostBack)
            {

                try
                {
                    if (Request[this.txtFrom.UniqueID] != null)
                    {
                        this.txtFrom.Text = Request[this.txtFrom.UniqueID];
                        this.txtTo.Text = Request[this.txtTo.UniqueID];
                    }
                }
                catch
                {
                    if (!String.IsNullOrEmpty(sn.Report.FromDate))
                        this.txtFrom.Text = Convert.ToDateTime(sn.Report.FromDate).ToString("MM/dd/yyyy");
                    if (!String.IsNullOrEmpty(sn.Report.ToDate))
                        this.txtTo.Text = Convert.ToDateTime(sn.Report.ToDate).ToString("MM/dd/yyyy");
                }


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


               if (sn.Report.FromDate!="")
                  this.txtFrom.Text = Convert.ToDateTime(sn.Report.FromDate).ToString("MM/dd/yyyy");
               else
                  this.txtFrom.Text = DateTime.Now.ToString("MM/dd/yyyy");


               if (sn.Report.ToDate != "")
                  this.txtTo.Text = Convert.ToDateTime(sn.Report.ToDate).ToString("MM/dd/yyyy");
               else
                this.txtTo.Text = DateTime.Now.AddDays(1).ToString("MM/dd/yyyy");

               //this.txtFrom.Text=DateTime.Now.AddDays(-1).ToString("MM/dd/yyyy hh:mm");
               //this.txtTo.Text=DateTime.Now.AddDays(1).ToString("MM/dd/yyyy hh:mm");




               this.tblSpeedViolation.Visible = false;
               this.tblCost.Visible = true;

               this.tblFilter.Visible = false;
               this.lblLandmarkCaption.Visible = false;
               this.ddlLandmarks.Visible = false;
               this.tblPoints.Visible = false;
               this.tblDriverOptions.Visible = false;
   
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

              
               
               //cboReports.SelectedIndex = cboReports.Items.IndexOf(cboReports.Items.FindByValue(sn.Report.GuiId.ToString()));
               
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
            ReportCriteria();


           // if (Convert.ToInt32(cboReports.SelectedItem.Value) > 89 && Convert.ToInt32(cboReports.SelectedItem.Value) != 103)
           // {
          //      this.cmdSchedule.Enabled = false;
          //      this.cmdViewScheduled.Enabled = false;
           // }
           // else
          //  {
                this.cmdSchedule.Enabled = false;
                this.cmdViewScheduled.Enabled = false;
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
             //this.lblLandmarkCaption.Visible = true ;
             //this.ddlLandmarks.Visible = true ;
             this.cboVehicle.Enabled = false;
             //LoadLandmarks();
              break;
           case "71":
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
           case "115":
           case "116":
           case "117":
           case "119":
           
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
           case "111":
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

             case "113":
              tblIdlingThreshold.Visible = true;
              this.cboVehicle.Enabled = false;
              break;

             case "114":
              LoadDrivers();
              tblDriverOptions.Visible = true;
              this.cboVehicle.Enabled = false;
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
      }


      private void LoadLandmarks()
      {
          DataSet dsLandmarks = new DataSet();
          string xmlResult = "";

          using (ServerDBOrganization.DBOrganization organ = new ServerDBOrganization.DBOrganization())
          {

              //if (objUtil.ErrCheck(organ.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), false))
              //    if (objUtil.ErrCheck(organ.GetOrganizationLandmarksXMLByOrganizationId(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), true))
              //    {
              //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Landmarks for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
              //        //RedirectToLogin();
              //        return;
              //    }


              if (objUtil.ErrCheck(organ.GetOrganizationLandmark_Public(sn.UserID, sn.SecId, sn.User.OrganizationId,false,  ref xmlResult), false))
                  if (objUtil.ErrCheck(organ.GetOrganizationLandmark_Public(sn.UserID, sn.SecId, sn.User.OrganizationId, false, ref xmlResult), true))
                  {
                      System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Landmarks for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                      //RedirectToLogin();
                      return;
                  }
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
          else if (sn.User.OrganizationId == 951) //UP
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
	  else if (sn.User.OrganizationId == 999695) //VanHoute 
              categoryId = 20;
	 else if (sn.User.OrganizationId == 882) //Sintra Inc.
              categoryId = 28;
     else if (sn.User.OrganizationId == 1000056) //Jean Fournier Inc.
              categoryId = 42;
          else if (sn.User.OrganizationId == 1000141) //Gazzola
              categoryId = 50;
          else if (sn.User.OrganizationId == 999991) //BSM Test
              categoryId = 52;

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

                  dsReports.ReadXml(new StringReader(xml));
                  this.cboReports.DataSource = dsReports;
                  sn.Report.UserExtendedReportsDataSet = dsReports;
                  this.cboReports.DataBind();
              }

          }



          //if (categoryId == 4)
          //    cboReports.Items.Add(new ListItem("Fleet Membership Report By User", "76"));


          if (categoryId == 12)
          {
              cboReports.Items.Add(new ListItem("Activity Summary and Green House Gas Report", "92"));
              cboReports.Items.Add(new ListItem("Green House Gas Emission Summary Report", "93"));
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
              string strFromDate = this.txtFrom.Text + " 12:00 AM";
              string strToDate = this.txtTo.Text + " 12:00 AM"; 
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


              TimeSpan ts = to - from;
              if (ts.Days > 31)
              {
                  this.lblMessage.Visible = true;
                   this.lblMessage.Text = GetLocalResourceObject("MessageRptDateRange").ToString();
                  return;
              }


           




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


                  //#region Timesheet Validation
                  //case "66":
                  //  sn.Report.XmlParams = ReportTemplate.MakePair(ReportTemplate.RptParamGeozone, this.ddlGeozones.SelectedValue);
                  //  break; 
                  //#endregion
              }
              # endregion Reports

              sn.Report.GuiId = Convert.ToInt16(this.cboReports.SelectedValue);
              sn.Report.FromDate = from.ToString();
              sn.Report.ToDate = to.ToString();
              sn.Report.ReportFormat = Convert.ToInt32(this.cboFormat.SelectedValue);
              sn.Report.FleetId = Convert.ToInt32(this.cboFleet.SelectedValue);
              sn.Report.IsFleet = this.cboVehicle.SelectedValue == "0" ? true : false;
              sn.Report.VehicleId  = Convert.ToInt32(this.cboVehicle.SelectedValue);
              sn.Report.FleetName = this.cboFleet.SelectedItem.Text;
              sn.Report.ReportType = cboReports.SelectedValue;
              
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
          if (reportGuiId == "72")
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
              switch (this.cboViolationSpeed.SelectedValue)
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

      /// <summary>
      /// Load geozones into ddl
      /// </summary>
      private void LoadGeozones()
      {
          DataSet dsGeozones = new DataSet();
          string xmlResult = "";

          using (ServerDBOrganization.DBOrganization organ = new ServerDBOrganization.DBOrganization())
          {

              //if (objUtil.ErrCheck(organ.GetOrganizationGeozones(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), false))
              //    if (objUtil.ErrCheck(organ.GetOrganizationGeozones(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), true))
              //    {
              //        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Geozones for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
              //        //RedirectToLogin();
              //        return;
              //    }

              if (objUtil.ErrCheck(organ.GetOrganizationGeozones_Public(sn.UserID, sn.SecId, sn.User.OrganizationId,false, ref xmlResult), false))
                  if (objUtil.ErrCheck(organ.GetOrganizationGeozones_Public(sn.UserID, sn.SecId, sn.User.OrganizationId, false, ref xmlResult), true))
                  {
                      System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " No Geozones for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                      //RedirectToLogin();
                      return;
                  }
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
}
}
