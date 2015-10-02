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
using System.Globalization; 

namespace SentinelFM
{
   /// <summary>
   /// Summary description for frmHistoryCriteria.
   /// </summary>
   public partial class frmHistoryCrt : SentinelFMBasePage
   {
      protected System.Web.UI.WebControls.Label lblVehicle;
      protected System.Web.UI.WebControls.DataGrid dgVehicleList;

      protected void Page_Load(object sender, System.EventArgs e)
      {
         try
         {
            //Clear IIS cache
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.Now);
            int i = 0;

            if  (Request[this.txtFrom.UniqueID]!=null)
            {
                try
                {
                    this.txtFrom.Text =Convert.ToDateTime(Request[this.txtFrom.UniqueID]).ToShortDateString() ;
                    this.txtTo.Text = Convert.ToDateTime(Request[this.txtTo.UniqueID]).ToShortDateString();
                }
                catch {}
            }

            if ((sn == null) || (sn.UserName == null) || (sn.UserName == ""))
            {
               RedirectToLogin();
               return;
            }

            if (!Page.IsPostBack)
            {           
               LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmHistoryCriteria, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
               GuiSecurity(this);
               //if (System.Globalization.CultureInfo.CurrentUICulture.ToString() == "fr-CA")
               //{
               //   txtFrom.CultureInfo.CultureName = "fr-FR";
               //   txtTo.CultureInfo.CultureName = "fr-FR";
               //}
               //else
               //{
               //   txtFrom.CultureInfo.CultureName = System.Globalization.CultureInfo.CurrentUICulture.ToString();
               //   txtTo.CultureInfo.CultureName = System.Globalization.CultureInfo.CurrentUICulture.ToString();
               //}
               this.tblStopReport.Visible = false;
               clsMisc.cboHoursFill(ref cboHoursFrom);
               clsMisc.cboHoursFill(ref cboHoursTo);
               CboFleet_Fill();

               if (sn.History.FromDate.ToString() != "")
                   this.txtFrom.Text = Convert.ToDateTime(sn.History.FromDate).ToShortDateString();
               else
                   this.txtFrom.Text = DateTime.Now.ToShortDateString();

               if (sn.History.ToDate.ToString() != "")
                   this.txtTo.Text = Convert.ToDateTime(sn.History.ToDate).ToShortDateString();
               else
                   this.txtTo.Text = DateTime.Now.AddDays(1).ToShortDateString();

               if (sn.History.FromHours != "")
               {
                  this.cboHoursFrom.SelectedIndex = -1;
                  for (i = 0; i <= cboHoursFrom.Items.Count - 1; i++)
                  {
                     if (cboHoursFrom.Items[i].Value == sn.History.FromHours)
                     {
                        this.cboHoursFrom.SelectedIndex = i;
                        break;
                     }
                  }
               }
               else
               {
                  //this.cboHoursFrom.SelectedIndex = -1;
                  //for (i = 0; i <= cboHoursFrom.Items.Count - 1; i++)
                  //{
                  //   if (Convert.ToInt32(cboHoursFrom.Items[i].Value) == 8)
                  //   {
                  //      cboHoursFrom.Items[i].Selected = true;
                  //      break;
                  //   }
                  //}

                   cboHoursFrom.Items[0].Selected = true;
               }

               if (sn.History.ToHours != "")
               {
                  this.cboHoursTo.SelectedIndex = -1;
                  for (i = 0; i <= cboHoursTo.Items.Count - 1; i++)
                  {
                     if (cboHoursTo.Items[i].Value == sn.History.ToHours)
                     {
                        this.cboHoursTo.SelectedIndex = i;
                        break;
                     }
                  }
               }
               else
               {
                  //this.cboHoursTo.SelectedIndex = -1;
                  //for (i = 0; i <= cboHoursTo.Items.Count - 1; i++)
                  //{
                  //   if (cboHoursTo.Items[i].Value == DateTime.Now.AddHours(1).Hour.ToString())
                  //   {
                  //      cboHoursTo.Items[i].Selected = true;
                  //      break;
                  //   }
                  //}

                   cboHoursTo.Items[0].Selected = true;
               }

               if (sn.History.FleetId != 0)
               {
                  this.cboFleet.SelectedIndex = -1;
                  for (i = 0; i <= cboFleet.Items.Count - 1; i++)
                  {
                     if (cboFleet.Items[i].Value == sn.History.FleetId.ToString())
                        cboFleet.Items[i].Selected = true;
                  }
                  CboVehicle_Fill(Convert.ToInt32(sn.History.FleetId));
               }
               else if (sn.User.DefaultFleet != -1)
                   {
                       cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
                       CboVehicle_Fill(Convert.ToInt32(sn.User.DefaultFleet));
                       //this.lblVehicleName.Visible = true;
                       //this.cboVehicle.Visible = true;
                   }

               if (sn.History.VehicleId != 0)
               {
                  //CboVehicle_Fill(Convert.ToInt32(sn.History.FleetId));

                  this.cboVehicle.SelectedIndex = -1;

                  for (i = 0; i <= cboVehicle.Items.Count - 1; i++)
                  {
                      if (cboVehicle.Items[i].Value == sn.History.VehicleId.ToString())
                      {
                          cboVehicle.Items[i].Selected = true;
                          break; 
                      }
                  }
                  //this.lblVehicleName.Visible = true;
                  //this.cboVehicle.Visible = true;
                  LoadCommunicationInfo();
               }

               if (cboVehicle.Items.Count==0) 
                   cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));

               if (sn.History.RedirectFromMapScreen)
               {
                  ShowData();
                  sn.History.RedirectFromMapScreen = false;
               }
               lstBoxMsgs_Fill();
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

      #region Web Form Designer generated code
      override protected void OnInit(EventArgs e)
      {
         //
         // CODEGEN: This call is required by the ASP.NET Web Form Designer.
         //
         InitializeComponent();
         base.OnInit(e);
      }

      /// <summary>
      /// Required method for Designer support - do not modify
      /// the contents of this method with the code editor.
      /// </summary>
      private void InitializeComponent()
      {

      }
      #endregion

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
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);
         }
      }

       private void lstBoxMsgs_Fill()
       {
           try
           {
               DataSet dsMsgTypes = new DataSet();
               StringReader strrXML = null;
               string xml = "";
               ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();

               if (objUtil.ErrCheck(dbs.GetBoxMsgInTypesByLang (sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName,ref xml), false))
                   if (objUtil.ErrCheck(dbs.GetBoxMsgInTypesByLang(sn.UserID, sn.SecId,CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                   {
                       System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Failed to GetBoxMsgInTypes for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                       return;
                   }

               if (xml == "")
               {
                   System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, " Failed to GetBoxMsgInTypes for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                   return;
               }

               strrXML = new StringReader(xml);
               dsMsgTypes.ReadXml(strrXML);

               lstMsgTypes.DataSource = dsMsgTypes;
               lstMsgTypes.DataBind();
               lstMsgTypes.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("lstMsgTypes_Item_0"), "-1"));
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
            DataSet dsVehicle;
            dsVehicle = new DataSet();           
            StringReader strrXML = null;

            string xml = "";
            ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();

            if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), false))
               if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xml), true))
               {
                  this.cboVehicle.Items.Clear();
                  cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));
                  return;
               }
            if (xml == "")
            {
               this.cboVehicle.Items.Clear();
               cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));
               return;
            }

            strrXML = new StringReader(xml);
            dsVehicle.ReadXml(strrXML);

            cboVehicle.DataSource = dsVehicle;
            cboVehicle.DataBind();
            //ViewState["dsVehicle"] = dsVehicle;
            sn.History.DsHistoryVehicles = dsVehicle;
            cboVehicle.Visible = true;

            cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));
            cboVehicle.Items.Insert(1, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0_EntireFleet"), "0"));
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
             cboVehicle.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_0"), "-1"));
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);
         }
      }

      private void dgVehicleList_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         this.lblVehicle.Text = dgVehicleList.SelectedItem.Cells[0].Text;
      }

      protected void cboFleet_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
         {
            this.lblVehicleName.Visible = true;
            CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
         }
      }

      protected void cmdShowHistory_Click(object sender, System.EventArgs e)
      {
         try
         {
             switch (this.cboHistoryType.SelectedItem.Value)
             {
                 case "0":
                     sn.History.ShowTrips = false;  
                     sn.History.ShowStops = false;
                     sn.History.ShowIdle = false;
                     sn.History.ShowStopsAndIdle = false;
                     break;
                 case "1":
                     sn.History.ShowTrips = false;
                     sn.History.ShowBreadCrumb = false;
                     sn.History.ShowStopsAndIdle = true;
                     sn.History.ShowStops = true;
                     sn.History.ShowIdle = true;
                     sn.History.ReportstopDuration = Convert.ToInt32(this.cboStopSequence.SelectedItem.Value);
                     break;
                 case "2":
                     sn.History.ShowTrips = false;
                     sn.History.ShowBreadCrumb = false;
                     sn.History.ShowStopsAndIdle = false;
                     sn.History.ShowStops = true;
                     sn.History.ShowIdle = false;
                     sn.History.ReportstopDuration = Convert.ToInt32(this.cboStopSequence.SelectedItem.Value);
                     break;
                 case "3":
                     sn.History.ShowTrips = false;
                     sn.History.ShowBreadCrumb = false;
                     sn.History.ShowStopsAndIdle = false;
                     sn.History.ShowStops = false;
                     sn.History.ShowIdle = true;
                     sn.History.ReportstopDuration = Convert.ToInt32(this.cboStopSequence.SelectedItem.Value);
                     break;
                 case "4":
                     sn.History.ShowTrips = true;
                     sn.History.ShowStops = false;
                     sn.History.ShowIdle = false;
                     sn.History.ShowStopsAndIdle = false;
                     break; 
             }
            ShowData();          
         }
         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);
         }
      }

      private void ShowData()
      {
         try
         {
            string strFromDate = "";
            string strToDate = "";

            Page.Validate();
            if (!Page.IsValid)
               return;

            if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) < 12)
                strFromDate = this.txtFrom.Text  + " " + this.cboHoursFrom.SelectedItem.Value + ":00 AM";

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

            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");

            strFromDate = Convert.ToDateTime(strFromDate, ci).ToString();
            strToDate = Convert.ToDateTime(strToDate, ci).ToString();

            if (Convert.ToDateTime(strFromDate) > Convert.ToDateTime(strToDate))
            {
               this.lblMessage.Visible = true;
               this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMessage_Text_InvalidDate");
               return;
            }
            else
            {
               this.lblMessage.Visible = true;
               this.lblMessage.Text = "";
            }



            string DisableFullHistoryData = ConfigurationManager.AppSettings["DisableFullHistoryData"].ToString();
            TimeSpan currDuration = new TimeSpan(Convert.ToDateTime(strToDate).Ticks  - Convert.ToDateTime(strFromDate).Ticks);
            if (Convert.ToBoolean(DisableFullHistoryData) && (currDuration.TotalHours > 24))    
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = "Due to service loads, HISTORY requests have been temporarily limited to a period of 24 hours.";
                return;
            }


           TimeSpan DateDiff = Convert.ToDateTime(strToDate).Subtract(Convert.ToDateTime(strFromDate));
           if (DateDiff.TotalDays>62 )
            {
                this.lblMessage.Visible = true;
                this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMaximumDays");
                return;
            }

            //if (Convert.ToInt64(this.cboVehicle.SelectedItem.Value) == 0)
            //{
            //    int tempDays =Convert.ToInt32((60 / Convert.ToInt64(this.cboVehicle.Items.Count)));
            //    tempDays = tempDays == 0 ? 1 : tempDays;

            //    if (Convert.ToInt32(DateDiff.TotalDays) > tempDays)
            //    {
            //        this.lblMessage.Visible = true;
            //        this.lblMessage.Text = (string)base.GetLocalResourceObject("lblMaximumDaysEntireFleet") + tempDays.ToString() ;
            //        return;
            //    }
            //}

            if (this.chkLatestMsg.Checked)
                sn.History.SqlTopMsg = "TOP 1";
            else
                sn.History.SqlTopMsg = "";

            sn.History.MsgList = "";
            foreach (ListItem li in lstMsgTypes.Items)
            {
                if ((li.Selected) )
                {
                    if (li.Value == "-1")
                    {
                        sn.History.MsgList = "";
                        break;
                    }
                    else
                        sn.History.MsgList += li.Value + ",";
                }
            }

            if (sn.History.MsgList != "")
                sn.History.MsgList = sn.History.MsgList.Substring(0, sn.History.MsgList.Length - 1);

            sn.History.FromDate = this.txtFrom.Text  ;
            sn.History.ToDate = this.txtTo.Text;
            sn.History.FromHours = this.cboHoursFrom.SelectedItem.Value;
            sn.History.ToHours = this.cboHoursTo.SelectedItem.Value;
            sn.History.VehicleId = Convert.ToInt64(this.cboVehicle.SelectedItem.Value);
            sn.History.FleetId = Convert.ToInt64(this.cboFleet.SelectedItem.Value);
            sn.History.ShowToolTip = true;
            sn.History.CarLatitude = "";
            sn.History.CarLongitude = "";
            sn.History.DsHistoryInfo = null;
            sn.History.VehicleName = this.cboVehicle.SelectedItem.Text.Replace("'", "''");
            sn.History.FleetName = this.cboFleet.SelectedItem.Text.Replace("'", "''");
            if (cboCommMode.Visible)
               sn.History.DclId = Convert.ToInt16(this.cboCommMode.SelectedItem.Value);
            else
               sn.History.DclId = -1;

            sn.History.FromDateTime = strFromDate;
            sn.History.ToDateTime = strToDate;
            if (WebHistoryTab.ActiveTab.Index==1 && this.txtAddress.Text != "")
                sn.History.Address = "%" + this.txtAddress.Text + "%";
            else
                sn.History.Address = "";

            sn.History.TripSensor = Convert.ToInt16(this.optEndTrip.SelectedItem.Value);     

            string str = "frmHistWait.aspx?VehicleId=" + this.cboVehicle.SelectedItem.Value + "&strFromDate=" + strFromDate + "&strToDate=" + strToDate;
            //if (sn.User.MapType != VLF.MAP.MapType.MapsoluteWeb)
            //   Response.Write("<script language='javascript'>  parent.location.href='" + str + "'; </script>");
            //else
               Response.Write("<script language='javascript'>  parent.frmHis.location.href='" + str + "'; </script>");
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

      protected void cboVehicle_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         LoadCommunicationInfo();
      }

      private void LoadCommunicationInfo()
      {
         try
         {
            DataRow[] DataRows = null; ;
            //DataRows = sn.History.DsHistoryVehicles.Tables[0].Select("VehicleId=" + Convert.ToInt32(this.cboVehicle.SelectedItem.Value.ToString().TrimEnd()));
            DataRows = sn.History.DsHistoryVehicles.Tables[0].Select("VehicleId='" + this.cboVehicle.SelectedItem.Value.ToString().TrimEnd()+"'");
            if  ((DataRows!=null) && (DataRows.Length > 0))
            {
               DataRow drVehicle = DataRows[0];
               sn.History.LicensePlate = drVehicle["LicensePlate"].ToString().TrimEnd();
               CboCommMode_Fill(Convert.ToInt32(drVehicle["BoxId"]));
               this.lblCommMode.Visible = true;
               this.cboCommMode.Visible = true;
               //if (sn.User.ControlEnable(sn, 43))
               //{
               //   CboCommMode_Fill(Convert.ToInt32(drVehicle["BoxId"]));
               //   this.lblCommMode.Visible = true;
               //   this.cboCommMode.Visible = true;
               //}
            }
         }
         catch
         {
             this.lblCommMode.Visible = false ;
             this.cboCommMode.Visible = false ;
         }
         //if (sn.User.ControlEnable(sn, 43))
         //{
         //   DataSet ds = sn.History.DsHistoryVehicles;
         //   foreach (DataRow dr in ds.Tables[0].Rows)
         //   {
         //      if (Convert.ToInt32(dr["VehicleId"].ToString().TrimEnd()) == Convert.ToInt32(this.cboVehicle.SelectedItem.Value.ToString().TrimEnd()))
         //      {
         //         CboCommMode_Fill(Convert.ToInt32(dr["BoxId"]));
         //         this.lblCommMode.Visible = true;
         //         this.cboCommMode.Visible = true;
         //         return;
         //      }
         //   }
         //}
      }

      protected void cboHistoryType_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         WebHistoryTab.ActiveTabIndex = 0;          

         if ((cboHistoryType.SelectedItem.Value == "1") && (this.cboVehicle.Visible))
         {
            this.tblStopReport.Visible = true;
            this.lnkMapLegends.Visible = true;
         }
         else if (cboHistoryType.SelectedItem.Value == "4")
         {
             WebHistoryTab.ActiveTabIndex = 2;
         }
         else
         {
             this.tblStopReport.Visible = false;
             this.lnkMapLegends.Visible = false;
         }
      }

      private void CboCommMode_Fill(Int32 BoxId)
      {
         try
         {           
            DataSet ds = new DataSet();
            StringReader strrXML = null;
            string xml = "";
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();

            if (objUtil.ErrCheck(dbv.GetBoxConfigInfo(sn.UserID, sn.SecId, BoxId, ref xml), false))
               if (objUtil.ErrCheck(dbv.GetBoxConfigInfo(sn.UserID, sn.SecId, BoxId, ref xml), true))
               {
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No configuration info for Box:" + sn.Cmd.BoxId.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                  return;
               }
            if (xml == "")
            {
               System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No configuration info for Box:" + sn.Cmd.BoxId.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
               return;
            }
            strrXML = new StringReader(xml);
            ds.ReadXml(strrXML);

            this.cboCommMode.DataSource = ds;
            this.cboCommMode.DataBind();
         }
         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
             System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
             ExceptionLogger(trace);
         }
         finally
         {
            cboCommMode.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboCommMode_Item_0"), "-1"));
         }
      }    
    }
}
