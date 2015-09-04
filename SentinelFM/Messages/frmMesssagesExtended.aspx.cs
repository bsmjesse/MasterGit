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
using System.Globalization;
using System.IO;
using VLF.CLS;
using GarminFMI;

namespace SentinelFM
{
   public partial class Messages_frmMesssagesExtended : SentinelFMBasePage 
   {
      
      

      protected void Page_Load(object sender, EventArgs e)
      {
         try
         {

            this.txtFrom.Text = Request[this.txtFrom.UniqueID];
            this.txtTo.Text = Request[this.txtTo.UniqueID];

            
            if (!Page.IsPostBack)
            {
               LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmMessagesForm, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
               GuiSecurity(this);
               SetVisibleOptions(0); 

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



               if (sn.Message.FleetId != 0)
               {
                  this.cboFleet.SelectedIndex = -1;
                  for (int i = 0; i <= cboFleet.Items.Count - 1; i++)
                  {
                     if (cboFleet.Items[i].Value == sn.Message.FleetId.ToString())
                        cboFleet.Items[i].Selected = true;
                  }
               }

               else
               {
                  if (sn.User.DefaultFleet != -1)
                  {
                     cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindByValue(sn.User.DefaultFleet.ToString()));
                     CboVehicle_Fill(Convert.ToInt32(sn.User.DefaultFleet));
                     this.lblVehicleName.Visible = true;
                     this.cboVehicle.Visible = true;
                  }
               }


               if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
               {

                  CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));

                  this.cboVehicle.SelectedIndex = -1;

                  for (int i = 0; i <= cboVehicle.Items.Count - 1; i++)
                  {
                     if (cboVehicle.Items[i].Value == sn.Message.BoxId.ToString())
                        cboVehicle.Items[i].Selected = true;
                  }

                  this.lblVehicleName.Visible = true;
                  this.cboVehicle.Visible = true;


                  SetHours();
                  SetDirection();
                  CboDrivers_Fill();

                  sn.Message.FromDate = Convert.ToDateTime(sn.Message.FromDate).ToShortDateString() + " " + sn.Message.FromHours;
                  sn.Message.ToDate = Convert.ToDateTime(sn.Message.ToDate).ToShortDateString() + " " + sn.Message.ToHours;
                  dgMessages_Fill_NewTZ();
                  this.BusyReport.Title = (string)base.GetLocalResourceObject("BusyPleaseWaitMessage");

               }

             

            }
         }

         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
      }
      protected void optMessageType_SelectedIndexChanged(object sender, EventArgs e)
      {
          
          FleetVehicleOption(true);
          SetVisibleOptions(optMessageType.SelectedIndex);
         
        
      }

      private void SetVisibleOptions(Int32 optId)
      {
         //this.dgMessages.LayoutSettings.ClientVisible = false;
         //this.dgSched.LayoutSettings.ClientVisible = false;
         //this.dgAlarms.LayoutSettings.ClientVisible = false;
         //dgDriverMsgs.LayoutSettings.ClientVisible = false; 

          this.RequiredGarminVehicle.Enabled = false;   

         switch (optId)
         {
            case 0: // Text Messages
                 this.MultiviewMessages.ActiveViewIndex = 0;  
               break;
            case 1: // Alarms
               this.MultiviewMessages.ActiveViewIndex = 2;
               FleetVehicleOption(false);
               break;
            case 2: // Sceduled Tasks
               FleetVehicleOption(false);
               this.MultiviewMessages.ActiveViewIndex = 1;
               break;

            case 3: // Garmin
               FleetVehicleOption(false);
               this.MultiviewMessages.ActiveViewIndex = 4;
               CboVehicleGarmin_Fill();
               this.RequiredGarminVehicle.Enabled = true ;
               break;

             case 4: // Drivers
               FleetVehicleOption(false);
               this.MultiviewMessages.ActiveViewIndex = 3;
               break;


         }
      }

      private void FleetVehicleOption(bool visible)
      {
         valFleet.Enabled = visible;
         lblFleetTitle.Visible = visible;
         cboFleet.Visible = visible;
         valVehicle.Enabled = visible;
         lblVehicleName.Visible = visible;
         cboVehicle.Visible = visible;

      }
      protected void cmdViewAlarms_Click(object sender, EventArgs e)
      {
          try
          {
              dgAlarms_Fill_NewTZ();
          }
          catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
      }

       // Changes for TimeZone Feature start
      private void dgAlarms_Fill_NewTZ()
      {
          string strFromDate = "";
          string strToDate = "";
          CultureInfo ci = new CultureInfo("en-US");

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

          this.dgAlarms.LayoutSettings.ClientVisible = true;


          string xml = "";
          ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
          float timeZone = Convert.ToSingle(sn.User.NewFloatTimeZone + sn.User.DayLightSaving);

          if (objUtil.ErrCheck(alarms.GetAlarmsXML_NewTZ(sn.UserID, sn.SecId, Convert.ToDateTime(strFromDate), Convert.ToDateTime(strToDate), ref xml), false))
              if (objUtil.ErrCheck(alarms.GetAlarmsXML_NewTZ(sn.UserID, sn.SecId, Convert.ToDateTime(strFromDate), Convert.ToDateTime(strToDate), ref xml), true))
              {
                  sn.Message.DsHistoryAlarms = null;
                  dgAlarms.ClearCachedDataSource();
                  dgAlarms.RebindDataSource();
                  return;
              }

          if (xml == "")
          {
              sn.Message.DsHistoryAlarms = null;
              dgAlarms.ClearCachedDataSource();
              dgAlarms.RebindDataSource();
              return;
          }

          StringReader strrXML = new StringReader(xml);

          DataSet ds = new DataSet();

          //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
          //string strPath = MapPath(DataSetPath) + @"\Alarms.xsd";
          //ds.ReadXmlSchema(strPath);


          ds.ReadXml(strrXML);

          #region Resolve street address in Batch
          try
          {
              string[] addresses = null;
              DataRow[] drArrAddress = ds.Tables[0].Select("StreetAddress='" + VLF.CLS.Def.Const.addressNA + "'");

              if ((sn.User.GeoCodeEngine[0].MapEngineID.ToString().Contains("GeoMicroWeb")) && drArrAddress.Length > 0)
                  addresses = clsMap.ResolveStreetAddressGeoMicroWebBatch(drArrAddress);
              else if ((sn.User.GeoCodeEngine[0].MapEngineID.ToString().Contains("GeoMicro")) && drArrAddress.Length > 0)
                  addresses = clsMap.ResolveStreetAddressGeoMicroBatch(drArrAddress);

              int i = 0;
              foreach (DataRow dr in drArrAddress)
              {
                  dr["StreetAddress"] = addresses[i];
                  i++;
              }

          }
          catch (Exception Ex)
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:clsMap"));
          }
          #endregion

          DataColumn AlarmDate = new DataColumn("AlarmDate", Type.GetType("System.DateTime"));
          ds.Tables[0].Columns.Add(AlarmDate);
          string strStreetAddress = "";

          foreach (DataRow rowItem in ds.Tables[0].Rows)
          {
              rowItem["AlarmDate"] = Convert.ToDateTime(rowItem["TimeCreated"].ToString(), ci);


              strStreetAddress = rowItem["StreetAddress"].ToString().TrimEnd();
              if ((strStreetAddress == VLF.CLS.Def.Const.addressNA))
              {
                  try
                  {
                      rowItem["StreetAddress"] = clsMap.ResolveStreetAddress(sn, rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString());
                  }
                  catch (Exception Ex)
                  {
                      System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                  }

              }


              switch (strStreetAddress)
              {
                  case VLF.CLS.Def.Const.addressNA:
                      rowItem["StreetAddress"] = Resources.Const.InvalidAddress_addressNA;
                      break;

                  case VLF.CLS.Def.Const.noGPSData:
                      rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noGPSData;
                      break;

                  case VLF.CLS.Def.Const.noValidAddress:
                      rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noValidAddress;
                      break;

                  default:
                      break;
              }


              if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
              {
                  rowItem["AlarmDescription"] = LocalizationLayer.GUILocalizationLayer.LocalizeAlarms(rowItem["AlarmDescription"].ToString(), System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                  rowItem["AlarmLevel"] = LocalizationLayer.GUILocalizationLayer.LocalizeSeverity(rowItem["AlarmLevel"].ToString(), System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                  rowItem["AlarmState"] = LocalizationLayer.GUILocalizationLayer.LocalizeAlarmState(rowItem["AlarmState"].ToString(), System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
              }
          }



          // Show Combobox
          DataColumn dc = new DataColumn("chkBox", Type.GetType("System.Boolean"));
          dc.DefaultValue = false;
          ds.Tables[0].Columns.Add(dc);

          this.dgAlarms.LayoutSettings.ClientVisible = true;
          sn.Message.DsHistoryAlarms = ds;
          dgAlarms.ClearCachedDataSource();
          dgAlarms.RebindDataSource();

      }

      // Changes for TimeZone Feature end

      private void dgAlarms_Fill()
      {
         string strFromDate = "";
         string strToDate = "";
         CultureInfo ci = new CultureInfo("en-US");

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

         this.dgAlarms.LayoutSettings.ClientVisible = true;

         
         string xml = "";
         ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();
         Int16 timeZone = Convert.ToInt16(sn.User.TimeZone + sn.User.DayLightSaving);

         if (objUtil.ErrCheck(alarms.GetAlarmsXML(sn.UserID, sn.SecId, Convert.ToDateTime(strFromDate), Convert.ToDateTime(strToDate), ref xml), false))
            if (objUtil.ErrCheck(alarms.GetAlarmsXML(sn.UserID, sn.SecId, Convert.ToDateTime(strFromDate), Convert.ToDateTime(strToDate), ref xml), true))
            {
               sn.Message.DsHistoryAlarms = null;
               dgAlarms.ClearCachedDataSource();
               dgAlarms.RebindDataSource();
               return;
            }

         if (xml == "")
         {
            sn.Message.DsHistoryAlarms = null;
            dgAlarms.ClearCachedDataSource();
            dgAlarms.RebindDataSource();
            return;
         }

         StringReader strrXML = new StringReader(xml);

         DataSet ds = new DataSet();

         //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
         //string strPath = MapPath(DataSetPath) + @"\Alarms.xsd";
         //ds.ReadXmlSchema(strPath);


         ds.ReadXml(strrXML);

         #region Resolve street address in Batch
         try
         {
             string[] addresses = null;
             DataRow[] drArrAddress = ds.Tables[0].Select("StreetAddress='" + VLF.CLS.Def.Const.addressNA + "'");

             if ((sn.User.GeoCodeEngine[0].MapEngineID.ToString().Contains("GeoMicroWeb")) && drArrAddress.Length > 0)
                 addresses =clsMap.ResolveStreetAddressGeoMicroWebBatch(drArrAddress);
             else if ((sn.User.GeoCodeEngine[0].MapEngineID.ToString().Contains("GeoMicro")) && drArrAddress.Length > 0)
                 addresses =clsMap.ResolveStreetAddressGeoMicroBatch(drArrAddress);

             int i = 0;
             foreach (DataRow dr in drArrAddress)
             {
                 dr["StreetAddress"] = addresses[i];
                 i++;
             }

         }
         catch (Exception Ex)
         {
             System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:clsMap"));
         }
         #endregion

         DataColumn AlarmDate = new DataColumn("AlarmDate", Type.GetType("System.DateTime"));
         ds.Tables[0].Columns.Add(AlarmDate);
         string strStreetAddress = "";

         foreach (DataRow rowItem in ds.Tables[0].Rows)
         {
            rowItem["AlarmDate"] = Convert.ToDateTime(rowItem["TimeCreated"].ToString(), ci);


            strStreetAddress = rowItem["StreetAddress"].ToString().TrimEnd();
            if ((strStreetAddress == VLF.CLS.Def.Const.addressNA))
            {
                try
                {
                    rowItem["StreetAddress"] =clsMap.ResolveStreetAddress(sn, rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString());
                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                }

            }


            switch (strStreetAddress)
            {
                case VLF.CLS.Def.Const.addressNA:
                    rowItem["StreetAddress"] = Resources.Const.InvalidAddress_addressNA;
                    break;

                case VLF.CLS.Def.Const.noGPSData:
                    rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noGPSData;
                    break;

                case VLF.CLS.Def.Const.noValidAddress:
                    rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noValidAddress;
                    break;

                default:
                    break;
            }


            if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
            {
                rowItem["AlarmDescription"] = LocalizationLayer.GUILocalizationLayer.LocalizeAlarms(rowItem["AlarmDescription"].ToString(), System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                rowItem["AlarmLevel"] = LocalizationLayer.GUILocalizationLayer.LocalizeSeverity(rowItem["AlarmLevel"].ToString(), System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                rowItem["AlarmState"] = LocalizationLayer.GUILocalizationLayer.LocalizeAlarmState(rowItem["AlarmState"].ToString(), System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
            }
         }



         // Show Combobox
         DataColumn dc = new DataColumn("chkBox", Type.GetType("System.Boolean"));
         dc.DefaultValue = false;
         ds.Tables[0].Columns.Add(dc);

         this.dgAlarms.LayoutSettings.ClientVisible = true;
         sn.Message.DsHistoryAlarms = ds;
         dgAlarms.ClearCachedDataSource();
         dgAlarms.RebindDataSource();  

      }
      protected void cmdShowMessages_Click(object sender, EventArgs e)
      {
         try
         {
            string strFromDate = "";
            string strToDate = "";
            


            if (this.chkAuto.Checked)
            {

               this.txtFrom.Text = DateTime.Now.AddHours(-12).ToShortDateString();
               sn.Message.FromDate = DateTime.Now.AddHours(-12).ToShortDateString();

               this.txtTo.Text = DateTime.Now.ToShortDateString();
               sn.Message.ToDate = DateTime.Now.ToShortDateString();

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



            }

            else
            {
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


            }


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


            sn.Message.FromDate = strFromDate;
            sn.Message.ToDate = strToDate;
            sn.Message.FromHours = this.cboHoursFrom.SelectedItem.Value;
            sn.Message.ToHours = this.cboHoursTo.SelectedItem.Value;
            sn.Message.DsMessages = null;
            sn.Message.VehicleName = this.cboVehicle.SelectedItem.Text.Replace("'", "''");
            sn.Message.FleetName = this.cboFleet.SelectedItem.Text.Replace("'", "''");
            sn.Message.BoxId = Convert.ToInt32(this.cboVehicle.SelectedItem.Value);

            SetDirection();

            dgMessages_Fill_NewTZ();
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
      }
      protected void cmdAccept_Click(object sender, EventArgs e)
      {
         try
         {
            
            DataSet ds = new DataSet();
            ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();


            foreach (string keyValue in dgAlarms.RootTable.GetCheckedRows())
            {
                  if (objUtil.ErrCheck(alarms.AcceptCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(keyValue)), false))
                     if (objUtil.ErrCheck(alarms.AcceptCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(keyValue)), true))
                     {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Alarm Accept failed. User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                     }

            }


            dgAlarms_Fill_NewTZ();
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }
      protected void cmdCloseAlarms_Click(object sender, EventArgs e)
      {
         try
         {
            
            DataSet ds = new DataSet();
            ServerAlarms.Alarms alarms = new ServerAlarms.Alarms();


            foreach (string keyValue in dgAlarms.RootTable.GetCheckedRows())
            {
               if (objUtil.ErrCheck(alarms.CloseCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(keyValue)), false))
                  if (objUtil.ErrCheck(alarms.CloseCurrentAlarm(sn.UserID, sn.SecId, Convert.ToInt32(keyValue)), true))
                  {
                     System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Alarm Accept failed. User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                  }

            }


            dgAlarms_Fill_NewTZ();
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }
      protected void cmdNewMessage_Click(object sender, EventArgs e)
      {
         string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
         strUrl = strUrl + "	var myname='Message';";
         strUrl = strUrl + " var w=560;";
         strUrl = strUrl + " var h=520;";
         strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
         strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
         strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,toolbar=0,scrollbars=0,menubar=0,'; ";
         strUrl = strUrl + " win = window.open(mypage, myname, winprops);} ";

         strUrl = strUrl + " NewWindow('frmNewMessageMain.aspx');</script>";
         Response.Write(strUrl);
      }
      protected void cmdMarkAsRead_Click(object sender, EventArgs e)
      {
         try
         {
            
            DataSet ds = new DataSet();
            ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();
            Int32 BoxId = 0;

            foreach (string keyValue in dgMessages.RootTable.GetCheckedRows())
            {
               #if MDT_NEW
                string[] tmp = keyValue.Split(';');

                DataRow[] drArr = sn.Message.DsHistoryMessages.Tables[0].Select("VehicleId='" + tmp[1] + "' and MsgId='" + tmp[0]+"'");
                if (drArr == null || drArr.Length == 0)
                    continue;
                BoxId =Convert.ToInt32(drArr[0]["BoxId"].ToString());

                if (objUtil.ErrCheck(hist.SetMsgUserId(sn.UserID, sn.SecId, Convert.ToInt32(tmp[0]), BoxId), false))
                    if (objUtil.ErrCheck(hist.SetMsgUserId(sn.UserID, sn.SecId, Convert.ToInt32(tmp[0]),BoxId), true ))
                  {
                     System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Error in closing text message. User:" + sn.UserID.ToString() + " Form:frmMessageInfo.aspx"));
                  }


                 #else
                  if (objUtil.ErrCheck(hist.SetMsgUserId(sn.UserID, sn.SecId, Convert.ToInt32(keyValue)), false))
                      if (objUtil.ErrCheck(hist.SetMsgUserId(sn.UserID, sn.SecId, Convert.ToInt32(keyValue)), true))
                      {
                          System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Error in closing text message. User:" + sn.UserID.ToString() + " Form:frmMessageInfo.aspx"));
                      }
                #endif


            }


            dgMessages_Fill_NewTZ();
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }
      protected void cmdScheduledTasks_Click(object sender, EventArgs e)
      {
         dgSched_Fill();
      }

      protected void cboFleet_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
            CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
         else
            this.cboVehicle.Items.Clear();   
      }
      // Changes for TimeZone Feature start

      private void dgMessages_Fill_NewTZ()
      {
          try
          {

              this.dgMessages.LayoutSettings.ClientVisible = true;
              StringReader strrXML = null;
              ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();
              CultureInfo ci = new CultureInfo("en-US");

              string strFromDT = sn.Message.FromDate;
              string strToDT = sn.Message.ToDate;
              string xml = "";

              if ((Convert.ToInt32(this.cboFleet.SelectedItem.Value) != -1) && (Convert.ToInt32(this.cboVehicle.SelectedItem.Value) == 0))
              {
                  if (objUtil.ErrCheck(hist.GetFleetTextMessagesFullInfo_NewTZ(sn.UserID, sn.SecId, Convert.ToInt32(cboFleet.SelectedItem.Value), Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), false))
                      if (objUtil.ErrCheck(hist.GetFleetTextMessagesFullInfo_NewTZ(sn.UserID, sn.SecId, Convert.ToInt32(cboFleet.SelectedItem.Value), Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), true))
                      {
                          return;
                      }
              }
              else
              {
                  if (objUtil.ErrCheck(hist.GetTextMessagesFullInfo_NewTZ(sn.UserID, sn.SecId, sn.Message.BoxId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), false))
                      if (objUtil.ErrCheck(hist.GetTextMessagesFullInfo_NewTZ(sn.UserID, sn.SecId, sn.Message.BoxId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), true))
                      {
                          return;
                      }
              }

              if (xml == "")
              {
                  sn.Message.DsHistoryMessages = null;
                  dgMessages.RootTable.Rows.Clear();
                  dgMessages.ClearCachedDataSource();
                  dgMessages.RebindDataSource();

                  return;
              }

              strrXML = new StringReader(xml);


              DataSet ds = new DataSet();
              ds.ReadXml(strrXML);


              // Show Combobox
              DataColumn dc = new DataColumn("chkBox", Type.GetType("System.Boolean"));
              dc.DefaultValue = false;
              ds.Tables[0].Columns.Add(dc);

              DataColumn MsgKey = new DataColumn("MsgKey", Type.GetType("System.String"));
              MsgKey.DefaultValue = "";
              ds.Tables[0].Columns.Add(MsgKey);



              DataColumn MsgDate = new DataColumn("MsgDate", Type.GetType("System.DateTime"));
              ds.Tables[0].Columns.Add(MsgDate);

              if (ds.Tables[0].Columns.IndexOf("To") == -1)
              {
                  DataColumn colTo = new DataColumn("To", Type.GetType("System.String"));
                  colTo.DefaultValue = "";
                  ds.Tables[0].Columns.Add(colTo);
              }

              #region Resolve street address in Batch
              try
              {
                  string[] addresses = null;
                  DataRow[] drArrAddress = ds.Tables[0].Select("StreetAddress='" + VLF.CLS.Def.Const.addressNA + "'");

                  if ((sn.User.GeoCodeEngine[0].MapEngineID.ToString().Contains("GeoMicroWeb")) && drArrAddress.Length > 0)
                      addresses = clsMap.ResolveStreetAddressGeoMicroWebBatch(drArrAddress);
                  else if ((sn.User.GeoCodeEngine[0].MapEngineID.ToString().Contains("GeoMicro")) && drArrAddress.Length > 0)
                      addresses = clsMap.ResolveStreetAddressGeoMicroBatch(drArrAddress);

                  int i = 0;
                  foreach (DataRow dr in drArrAddress)
                  {
                      dr["StreetAddress"] = addresses[i];
                      i++;
                  }

              }
              catch (Exception Ex)
              {
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:clsMap"));
              }
              #endregion

              string strStreetAddress = "";
              foreach (DataRow rowItem in ds.Tables[0].Rows)
              {
                  rowItem["MsgKey"] = rowItem["MsgId"].ToString() + ";" + rowItem["Vehicleid"].ToString();
                  rowItem["MsgDate"] = Convert.ToDateTime(rowItem["MsgDateTime"].ToString(), ci);


                  strStreetAddress = rowItem["StreetAddress"].ToString().TrimEnd();
                  if ((strStreetAddress == VLF.CLS.Def.Const.addressNA))
                  {
                      try
                      {
                          rowItem["StreetAddress"] = clsMap.ResolveStreetAddress(sn, rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString());
                      }
                      catch (Exception Ex)
                      {
                          System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                      }

                  }


                  switch (strStreetAddress)
                  {
                      case VLF.CLS.Def.Const.addressNA:
                          rowItem["StreetAddress"] = Resources.Const.InvalidAddress_addressNA;
                          break;

                      case VLF.CLS.Def.Const.noGPSData:
                          rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noGPSData;
                          break;

                      case VLF.CLS.Def.Const.noValidAddress:
                          rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noValidAddress;
                          break;

                      default:
                          break;
                  }

              }


              sn.Message.DsHistoryMessages = ds;
              dgMessages.ClearCachedDataSource();
              dgMessages.RebindDataSource();



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

      // Changes for TimeZone Feature end

      private void dgMessages_Fill()
      {
         try
         {

            this.dgMessages.LayoutSettings.ClientVisible = true;
            StringReader strrXML = null;
            ServerDBHistory.DBHistory hist = new ServerDBHistory.DBHistory();
            CultureInfo ci = new CultureInfo("en-US");

            string strFromDT = sn.Message.FromDate;
            string strToDT = sn.Message.ToDate;
            string xml = "";

            if ((Convert.ToInt32(this.cboFleet.SelectedItem.Value) != -1) && (Convert.ToInt32(this.cboVehicle.SelectedItem.Value) == 0))
            {
               if (objUtil.ErrCheck(hist.GetFleetTextMessagesFullInfo(sn.UserID, sn.SecId, Convert.ToInt32(cboFleet.SelectedItem.Value), Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), false))
                  if (objUtil.ErrCheck(hist.GetFleetTextMessagesFullInfo(sn.UserID, sn.SecId, Convert.ToInt32(cboFleet.SelectedItem.Value), Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), true))
                  {
                     return;
                  }
            }
            else
            {
               if (objUtil.ErrCheck(hist.GetTextMessagesFullInfo(sn.UserID, sn.SecId, sn.Message.BoxId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), false))
                  if (objUtil.ErrCheck(hist.GetTextMessagesFullInfo(sn.UserID, sn.SecId, sn.Message.BoxId, Convert.ToDateTime(strFromDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToDateTime(strToDT, ci).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("MM/dd/yyyy HH:mm:ss"), Convert.ToInt16(sn.Message.MessageDirectionId), ref xml), true))
                  {
                     return;
                  }
            }

            if (xml == "")
            {
               sn.Message.DsHistoryMessages = null;
               dgMessages.RootTable.Rows.Clear();   
               dgMessages.ClearCachedDataSource();
               dgMessages.RebindDataSource();
               
               return;
            }

            strrXML = new StringReader(xml);
            
            
            DataSet ds = new DataSet();
            ds.ReadXml(strrXML);


            // Show Combobox
            DataColumn dc = new DataColumn("chkBox", Type.GetType("System.Boolean"));
            dc.DefaultValue = false;
            ds.Tables[0].Columns.Add(dc);

            DataColumn MsgKey = new DataColumn("MsgKey", Type.GetType("System.String"));
            MsgKey.DefaultValue = "";
            ds.Tables[0].Columns.Add(MsgKey);



            DataColumn MsgDate = new DataColumn("MsgDate", Type.GetType("System.DateTime"));
            ds.Tables[0].Columns.Add(MsgDate);

            if (ds.Tables[0].Columns.IndexOf("To") == -1)
            {
               DataColumn colTo = new DataColumn("To", Type.GetType("System.String"));
               colTo.DefaultValue = "";
               ds.Tables[0].Columns.Add(colTo);
            }

            #region Resolve street address in Batch
            try
            {
                string[] addresses = null;
                DataRow[] drArrAddress = ds.Tables[0].Select("StreetAddress='" + VLF.CLS.Def.Const.addressNA + "'");

                if ((sn.User.GeoCodeEngine[0].MapEngineID.ToString().Contains("GeoMicroWeb")) && drArrAddress.Length > 0)
                    addresses = clsMap.ResolveStreetAddressGeoMicroWebBatch(drArrAddress);
                else if ((sn.User.GeoCodeEngine[0].MapEngineID.ToString().Contains("GeoMicro")) && drArrAddress.Length > 0)
                    addresses = clsMap.ResolveStreetAddressGeoMicroBatch(drArrAddress);

                int i = 0;
                foreach (DataRow dr in drArrAddress)
                {
                    dr["StreetAddress"] = addresses[i];
                    i++;
                }

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:clsMap"));
            }
            #endregion

            string strStreetAddress = "";
            foreach (DataRow rowItem in ds.Tables[0].Rows)
            {
               rowItem["MsgKey"] = rowItem["MsgId"].ToString() + ";" + rowItem["Vehicleid"].ToString();
               rowItem["MsgDate"] = Convert.ToDateTime(rowItem["MsgDateTime"].ToString(), ci);


               strStreetAddress = rowItem["StreetAddress"].ToString().TrimEnd();
               if ((strStreetAddress == VLF.CLS.Def.Const.addressNA))
               {
                   try
                   {
                       rowItem["StreetAddress"] = clsMap.ResolveStreetAddress(sn, rowItem["Latitude"].ToString(), rowItem["Longitude"].ToString());
                   }
                   catch (Exception Ex)
                   {
                       System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                   }

               }


               switch (strStreetAddress)
               {
                   case VLF.CLS.Def.Const.addressNA:
                       rowItem["StreetAddress"] = Resources.Const.InvalidAddress_addressNA;
                       break;

                   case VLF.CLS.Def.Const.noGPSData:
                       rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noGPSData;
                       break;

                   case VLF.CLS.Def.Const.noValidAddress:
                       rowItem["StreetAddress"] = Resources.Const.InvalidAddress_noValidAddress;
                       break;

                   default:
                       break;
               }

            }

           
            sn.Message.DsHistoryMessages = ds;
            dgMessages.ClearCachedDataSource();
            dgMessages.RebindDataSource();
            


         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }

         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
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
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
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
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }

      }
      protected void cmdFormMessages_Click(object sender, EventArgs e)
      {

      }

      private void SetDirection()
      {
         if (Convert.ToInt16(this.cboDirection.SelectedItem.Value) == 0)
            sn.Message.MessageDirectionId = Convert.ToInt32(VLF.CLS.Def.Enums.TxtMsgDirectionType.Both);

         if (Convert.ToInt16(this.cboDirection.SelectedItem.Value) == 1)
            sn.Message.MessageDirectionId = Convert.ToInt32(VLF.CLS.Def.Enums.TxtMsgDirectionType.In);

         if (Convert.ToInt16(this.cboDirection.SelectedItem.Value) == 2)
            sn.Message.MessageDirectionId = Convert.ToInt32(VLF.CLS.Def.Enums.TxtMsgDirectionType.Out);

      }

      private void SetHours()
      {
         if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) < 12)
            sn.Message.FromHours = this.cboHoursFrom.SelectedItem.Value + ":00 AM";

         if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) == 12)
            sn.Message.FromHours = this.cboHoursFrom.SelectedItem.Value + ":00 PM";


         if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) > 12)
            sn.Message.FromHours = Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value) - 12) + ":00 PM";

         if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) < 12)
            sn.Message.ToHours = this.cboHoursTo.SelectedItem.Value + ":00 AM";

         if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) == 12)
            sn.Message.ToHours = this.cboHoursTo.SelectedItem.Value + ":00 PM";


         if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) > 12)
            sn.Message.ToHours = Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value) - 12) + ":00 PM";
      }

    

      protected void dgMessages_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
      {

         if ((sn.Message.DsHistoryMessages!=null) && (optMessageType.SelectedIndex==0))
             e.DataSource = sn.Message.DsHistoryMessages;
         else
          e.DataSource = null;
      }
      protected void dgAlarms_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
      {
         if ((sn.Message.DsHistoryAlarms != null) && (optMessageType.SelectedIndex == 1))
            e.DataSource = sn.Message.DsHistoryAlarms;
         else
            e.DataSource = null;
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

         if (objUtil.ErrCheck(dbs.GetSheduledTasksHistory(sn.UserID, sn.SecId, strFromDate, strToDate, Convert.ToInt32(this.cboFleet.SelectedItem.Value), Convert.ToInt32(this.cboVehicle.SelectedItem.Value), ref xml), false))
            if (objUtil.ErrCheck(dbs.GetSheduledTasksHistory(sn.UserID, sn.SecId, strFromDate, strToDate, Convert.ToInt32(this.cboFleet.SelectedItem.Value), Convert.ToInt32(this.cboVehicle.SelectedItem.Value), ref xml), true))
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
         System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
      }
     }

      protected void dgSched_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
      {
          try
          {
              if ((sn.Message.DsScheduledTasks != null) && (optMessageType.SelectedIndex == 2))
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
      protected void dgMessages_InitializeLayout(object sender, ISNet.WebUI.WebGrid.LayoutEventArgs e)
      {
          try
          {
              if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName != "en")
              {
                  e.Layout.TextSettings.Language = ISNet.WebUI.WebGrid.LanguageMode.UseCustom;
                  if (System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "fr")
                      e.Layout.TextSettings.UseLanguage = "fr-FR";
                  else
                      e.Layout.TextSettings.UseLanguage = System.Globalization.CultureInfo.CurrentUICulture.ToString();
              }
          }
          catch
          {
          }
      }
      protected void dgAlarms_InitializeLayout(object sender, ISNet.WebUI.WebGrid.LayoutEventArgs e)
      {
          try
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
         catch
          {
          }
      }

      private void CboDrivers_Fill()
      {
          //try
          //{

          //    StringReader strrXML = null;
          //    string xml = "";
          //    ServerDBDriver.DBDriver driver = new ServerDBDriver.DBDriver();

          //    if (objUtil.ErrCheck(driver.GetAllDrivers(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
          //        if (objUtil.ErrCheck(driver.GetAllDrivers(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
          //        {
          //            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
          //               " No drivers for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));

          //        }

          //    DataSet dsDrivers = new DataSet();
          //    strrXML = new StringReader(xml);
          //    dsDrivers.ReadXml(strrXML);

          //    DataColumn dc = new DataColumn();
          //    dc.ColumnName = "FullNameAndEmail";
          //    dc.DataType = Type.GetType("System.String");
          //    dc.DefaultValue = "";
          //    dsDrivers.Tables[0].Columns.Add(dc);

          //    foreach (DataRow dr in dsDrivers.Tables[0].Rows)
          //        dr["FullNameAndEmail"] = dr["FullName"] + " <" + dr["Email"].ToString().TrimEnd() + ">";  




          //    cboDrivers.DataSource = dsDrivers;
          //    cboDrivers.DataBind();
          //    cboDrivers.Items.Insert(0, new ListItem("Select a Driver", "-1"));

          //}
          //catch (NullReferenceException Ex)
          //{
          //    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
          //    RedirectToLogin();
          //}
          //catch (Exception Ex)
          //{
          //    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
          //}


      }

      protected void cmdViewDriverMsgs_Click(object sender, EventArgs e)
      {
          string xml = "";
          ServerDBDriver.DBDriver driver = new ServerDBDriver.DBDriver();
          string strFromDate = "";
          string strToDate = "";
          this.lblMessage.Text = "";
  
          if (this.cboDrivers.Value == "")
          {
              this.lblMessage.Visible = true;
              this.lblMessage.Text = "Please select a Driver";
              return;
          }

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


          try
          {

              // Changes for TimeZone Feature start
              if (objUtil.ErrCheck(driver.GetDriverMsgs(sn.UserID, sn.SecId, Convert.ToInt32(this.cboDrivers.Value), Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), ref xml), false))
                  if (objUtil.ErrCheck(driver.GetDriverMsgs(sn.UserID, sn.SecId, Convert.ToInt32(this.cboDrivers.Value), Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), ref xml), true)) // Changes for TimeZone Feature end
                  {
                      sn.Message.DsDriverMessages = null;
                      dgDriverMsgs.ClearCachedDataSource();
                      dgDriverMsgs.RebindDataSource();
                      return;
                  }

              if (xml == "")
              {
                  sn.Message.DsDriverMessages = null;
                  dgDriverMsgs.ClearCachedDataSource();
                  dgDriverMsgs.RebindDataSource();
                  return;
              }

              StringReader strrXML = new StringReader(xml);
              DataSet ds = new DataSet();
              //string DataSetPath = @ConfigurationSettings.AppSettings["DataSetPath"];
              //string strPath = MapPath(DataSetPath) + @"\dstDriverMsgs.xsd";

              string strPath = Server.MapPath("../Datasets/dstDriverMsgs.xsd");

              ds.ReadXmlSchema(strPath);
              ds.ReadXml(strrXML);


              sn.Message.DsDriverMessages = ds;
              dgDriverMsgs.ClearCachedDataSource();
              dgDriverMsgs.RebindDataSource();
              this.dgDriverMsgs.LayoutSettings.ClientVisible = true;
          }
          catch
          {
          }

      }
      protected void dgDriverMsgs_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
      {
          try
          {
              if ((sn.Message.DsDriverMessages != null) && (optMessageType.SelectedIndex == 4))
              {
                  e.DataSource = sn.Message.DsDriverMessages;

              }
              else
                  e.DataSource = null;
          }
          catch
          {
          }
      }

      protected void cboDrivers_InitializeDataSource(object sender, ISNet.WebUI.WebCombo.DataSourceEventArgs e)
      {
          try
          {

              if (optMessageType.SelectedIndex == 4)
              {
                  StringReader strrXML = null;
                  string xml = "";
                  ServerDBDriver.DBDriver driver = new ServerDBDriver.DBDriver();

                  if (objUtil.ErrCheck(driver.GetAllDrivers(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                      if (objUtil.ErrCheck(driver.GetAllDrivers(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                      {
                          System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                             " No drivers for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));

                      }

                  DataSet dsDrivers = new DataSet();
                  strrXML = new StringReader(xml);
                  dsDrivers.ReadXml(strrXML);

                  DataColumn dc = new DataColumn();
                  dc.ColumnName = "FullNameAndEmail";
                  dc.DataType = Type.GetType("System.String");
                  dc.DefaultValue = "";
                  dsDrivers.Tables[0].Columns.Add(dc);
                  string email = "";
                  foreach (DataRow dr in dsDrivers.Tables[0].Rows)
                  {
                      email = dr["Email"].ToString().TrimEnd() == "" ? " No email setup " : dr["Email"].ToString().TrimEnd();
                      dr["FullNameAndEmail"] = dr["FullName"] + " [" + email + "]";
                  }


                  e.DataSource = dsDrivers;

                  //cboDrivers.DataSource = dsDrivers;
                  //cboDrivers.DataBind();
                  //cboDrivers.Items.Insert(0, new ListItem("Select a Driver", "-1"));
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
          }

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

      private void CboVehicleGarmin_Fill()
      {
          try
          {


              StringReader strrXML = null;
              string xml = "";
              DBGarmin.Garmin garmin = new DBGarmin.Garmin();

              if (objUtil.ErrCheck(garmin.GetGarminDevicesByUser(sn.UserID, sn.SecId, ref xml), false))
                  if (objUtil.ErrCheck(garmin.GetGarminDevicesByUser(sn.UserID, sn.SecId, ref xml), true))
                  {
                      System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                         " No Garmin Devices for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));

                  }


              if (xml == "")
              {
                  //cboVehicleGarmin.Items.Insert(0, new ListItem("Select a Vehicle", "-1"));
                  cboVehicleGarmin.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_SelectVehicle"), "-1"));

                  return;
              }


              DataSet dsGarmin = new DataSet();
              strrXML = new StringReader(xml);
              dsGarmin.ReadXml(strrXML);

              sn.Message.DsGarminVehicles = dsGarmin;
              cboVehicleGarmin.DataSource = dsGarmin;
              cboVehicleGarmin.DataBind();
              //cboVehicleGarmin.Items.Insert(0, new ListItem("Select a Vehicle", "-1"));
              cboVehicleGarmin.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("cboVehicle_Item_SelectVehicle"), "-1"));

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
      protected void dgGarminHist_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
      {
          try
          {
              if ((sn.Message.DsGarminMessages != null) && (optMessageType.SelectedIndex == 3))
              {
                  e.DataSource = sn.Message.DsGarminMessages;

              }
              else
                  e.DataSource = null;
          }
          catch
          {
          }
      }
      protected void cmdViewGarminHistory_Click(object sender, EventArgs e)
      {
          string xml = "";
          DBGarmin.Garmin garmin = new DBGarmin.Garmin();
          string strFromDate = "";
          string strToDate = "";
          this.lblMessage.Text = "";

          if (this.cboVehicleGarmin.SelectedItem.Value == "-1")
          {
              this.lblMessage.Visible = true;
              this.lblMessage.Text = "Please select a Garmin";
              return;
          }

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


          try
          {
              if (objUtil.ErrCheck(garmin.GetGarminTextMsgHistory(sn.UserID, sn.SecId, Convert.ToInt32(this.cboVehicleGarmin.SelectedItem.Value), Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving),this.chkShowStatusInfo.Checked,   ref xml), false))
                  if (objUtil.ErrCheck(garmin.GetGarminTextMsgHistory(sn.UserID, sn.SecId, Convert.ToInt32(this.cboVehicleGarmin.SelectedItem.Value), Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving),this.chkShowStatusInfo.Checked, ref xml), false))
                  {
                      sn.Message.DsGarminMessages = null;
                      dgGarminHist.ClearCachedDataSource();
                      dgGarminHist.RebindDataSource();
                      return;
                  }

              if (xml == "")
              {
                  sn.Message.DsGarminMessages = null;
                  dgGarminHist.ClearCachedDataSource();
                  dgGarminHist.RebindDataSource();
                  return;
              }

              StringReader strrXML = new StringReader(xml);
              DataSet ds = new DataSet();

              string strPath = Server.MapPath("../Datasets/GarminMessages.xsd");
              ds.ReadXmlSchema(strPath);
              ds.ReadXml(strrXML);


              // Show Combobox
              DataColumn dc = new DataColumn("data", Type.GetType("System.String"));
              dc.DefaultValue = false;
              ds.Tables[0].Columns.Add(dc);

              dc = new DataColumn("imageUrl", Type.GetType("System.String"));
              dc.DefaultValue = false;
              ds.Tables[0].Columns.Add(dc);

              dc = new DataColumn("CustomUrl", Type.GetType("System.String"));
              dc.DefaultValue = false;
              ds.Tables[0].Columns.Add(dc);

              int msgId = 0;
              int rowCount = 0;
              
              foreach (DataRow dr in ds.Tables[0].Rows)
              {

                  dr["Data"] = VLF.CLS.Util.PairFindValue("TXT", dr["properties"].ToString());

                  //CustomUrl 
                  dr["CustomUrl"] = "javascript:var w =MsgDetails('" + dr["peripheralMessageTypeId"].ToString() + "','" + dr["messageId"].ToString() + "',1)";



                  if ((msgId != Convert.ToInt32(dr["messageId"])) || msgId==0) 
                  {
                      msgId = Convert.ToInt32(dr["messageId"]);
                      DataRow[] drArr = ds.Tables[0].Select("messageId='" + msgId + "'");

                      if (drArr == null || drArr.Length == 0)
                          continue;
                      else if (drArr.Length==1)
                          dr["imageUrl"] = "images/par_nch.png";
                      else if (drArr.Length > 1)
                      {
                          dr["imageUrl"] = "images/par_wch.png";
                          rowCount = drArr.Length-1;
                      }
                  }
                  else 
                  {
                      if (rowCount==1)
                          if (Convert.ToInt32(dr["peripheralMessageTypeId"]) == 33 || Convert.ToInt32(dr["peripheralMessageTypeId"]) == 5)
                          {
                              dr["status"] = VLF.CLS.Util.PairFindValue("RES", dr["properties"].ToString()).Replace("_", " ").ToUpper() ;
                              dr["imageUrl"] = "images/info_nch.png";
                          }
                          else
                              dr["imageUrl"] = "images/ch_nsb.png";
                      else if (rowCount>1)
                          if (Convert.ToInt32(dr["peripheralMessageTypeId"]) == 33 || Convert.ToInt32(dr["peripheralMessageTypeId"]) == 5)
                          {
                              dr["status"] = VLF.CLS.Util.PairFindValue("RES", dr["properties"].ToString()).Replace("_", " ").ToUpper();
                              dr["imageUrl"] = "images/info_wch.png";
                          }
                          else
                              dr["imageUrl"] = "images/ch_wsb.png";

                      rowCount--;
                  }
              }
              sn.Message.DsGarminMessages = ds;
              dgGarminHist.ClearCachedDataSource();
              dgGarminHist.RebindDataSource();
              
          }
          catch
          {
          }
      }
      protected void cmdNewGarminMsg_Click(object sender, EventArgs e)
      {
        

          string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
          strUrl = strUrl + "	var myname='Message';";
          strUrl = strUrl + " var w=560;";
          strUrl = strUrl + " var h=220;";
          strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
          strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
          strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,toolbar=0,scrollbars=0,menubar=0,'; ";
          strUrl = strUrl + " win = window.open(mypage, myname, winprops);} ";

          strUrl = strUrl + " NewWindow('frmNewGarminMsgs.aspx?boxId=" + this.cboVehicleGarmin.SelectedItem.Value + "&sendToAll=" + this.chkSendToAllVehicles.Checked.ToString() + "');</script>";
          Response.Write(strUrl);
      }
      protected void cmdNewLocation_Click(object sender, EventArgs e)
      {
       

          string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
          strUrl = strUrl + "	var myname='Message';";
          strUrl = strUrl + " var w=560;";
          strUrl = strUrl + " var h=420;";
          strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
          strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
          strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,toolbar=0,scrollbars=0,menubar=0,'; ";
          strUrl = strUrl + " win = window.open(mypage, myname, winprops);} ";

          strUrl = strUrl + " NewWindow('frmNewLocation.aspx?boxId=" + this.cboVehicleGarmin.SelectedItem.Value + "&sendToAll="+this.chkSendToAllVehicles.Checked.ToString() +"');</script>";
          Response.Write(strUrl);
      }
      protected void cmdViewLocations_Click(object sender, EventArgs e)
      {
          string xml = "";
          DBGarmin.Garmin garmin = new DBGarmin.Garmin();
          string strFromDate = "";
          string strToDate = "";
          this.lblMessage.Text = "";

          if (this.cboVehicleGarmin.SelectedItem.Value == "-1")
          {
              this.lblMessage.Visible = true;
              this.lblMessage.Text = "Please select a Garmin";
              return;
          }

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


          try
          {
              if (objUtil.ErrCheck(garmin.GetGarminLocationMsgHistory (sn.UserID, sn.SecId, Convert.ToInt32(this.cboVehicleGarmin.SelectedItem.Value), Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving),this.chkShowStatusInfo.Checked, ref xml), false))
                  if (objUtil.ErrCheck(garmin.GetGarminLocationMsgHistory(sn.UserID, sn.SecId, Convert.ToInt32(this.cboVehicleGarmin.SelectedItem.Value), Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving),this.chkShowStatusInfo.Checked, ref xml), false))
                  {
                      sn.Message.DsGarminLocations = null;
                      dgGarminHistLocations.ClearCachedDataSource();
                      dgGarminHistLocations.RebindDataSource();
                      return;
                  }

              if (xml == "")
              {
                  sn.Message.DsGarminLocations = null;
                  dgGarminHistLocations.ClearCachedDataSource();
                  dgGarminHistLocations.RebindDataSource();
                  return;
              }

              StringReader strrXML = new StringReader(xml);
              DataSet ds = new DataSet();


              string strPath = Server.MapPath("../Datasets/GarminMessages.xsd");
              ds.ReadXmlSchema(strPath);
              ds.ReadXml(strrXML);


              // Show Combobox
              DataColumn dc = new DataColumn("data", Type.GetType("System.String"));
              dc.DefaultValue = false;
              ds.Tables[0].Columns.Add(dc);

              
              dc = new DataColumn("location", Type.GetType("System.String"));
              dc.DefaultValue = false;
              ds.Tables[0].Columns.Add(dc);

              dc = new DataColumn("imageUrl", Type.GetType("System.String"));
              dc.DefaultValue = false;
              ds.Tables[0].Columns.Add(dc);



              dc = new DataColumn("CustomUrl", Type.GetType("System.String"));
              dc.DefaultValue = false;
              ds.Tables[0].Columns.Add(dc);

              
              int msgId = 0;
              int rowCount = 0;
              foreach (DataRow dr in ds.Tables[0].Rows)
              {
                  //dr["Data"] = VLF.CLS.Util.PairFindValue(GarminFMI.JKeyWords.TXT.ToString(), dr["properties"].ToString()) + " -> Position - LAT:" + VLF.CLS.Util.PairFindValue("LAT", dr["properties"].ToString()) + " LON:" + VLF.CLS.Util.PairFindValue("LON", dr["properties"].ToString());
                  dr["Data"] = VLF.CLS.Util.PairFindValue(GarminFMI.JKeyWords.TXT.ToString(), dr["properties"].ToString()) ;

                  if (VLF.CLS.Util.PairFindValue(GarminFMI.JKeyWords.LNAME.ToString(), dr["properties"].ToString()) != "")
                      dr["location"] = VLF.CLS.Util.PairFindValue(GarminFMI.JKeyWords.LNAME.ToString(), dr["properties"].ToString());
                  else if (VLF.CLS.Util.PairFindValue(GarminFMI.JKeyWords.ADD.ToString(), dr["properties"].ToString()) != "")
                      dr["location"] = VLF.CLS.Util.PairFindValue(GarminFMI.JKeyWords.ADD.ToString(), dr["properties"].ToString());
                  else
                      dr["location"] = "LAT:" + VLF.CLS.Util.PairFindValue("LAT", dr["properties"].ToString()) + " LON:" + VLF.CLS.Util.PairFindValue("LON", dr["properties"].ToString());


                  //CustomUrl 
                  dr["CustomUrl"] = "javascript:var w =MsgDetails('" + dr["peripheralMessageTypeId"].ToString() + "','" + dr["messageId"].ToString() + "',2)";



                   if ((msgId != Convert.ToInt32(dr["messageId"])) || msgId==0) 
                  {
                      msgId = Convert.ToInt32(dr["messageId"]);
                      DataRow[] drArr = ds.Tables[0].Select("messageId='" + msgId + "'");

                      if (drArr == null || drArr.Length == 0)
                          continue;
                      else if (drArr.Length==1)
                          dr["imageUrl"] = "images/par_nch.png";
                      else if (drArr.Length > 1)
                      {
                          dr["imageUrl"] = "images/par_wch.png";
                          rowCount = drArr.Length-1;
                      }
                  }
                  else 
                  {
                      if (rowCount==1)
                          if (Convert.ToInt32(dr["peripheralMessageTypeId"]) == 6)
                          {
                              dr["status"] = VLF.CLS.Util.PairFindValue("STOP_STATUS", dr["properties"].ToString()).Replace("_", " ").ToUpper();
                              dr["imageUrl"] = "images/info_nch.png";
                              dr["Description"] = "";
                          }
                          else
                              dr["imageUrl"] = "images/ch_nsb.png";
                      else if (rowCount>1)
                          if (Convert.ToInt32(dr["peripheralMessageTypeId"]) == 6)
                          {
                              dr["status"] = VLF.CLS.Util.PairFindValue("STOP_STATUS", dr["properties"].ToString()).Replace("_", " ").ToUpper();
                              dr["imageUrl"] = "images/info_wch.png";
                              dr["Description"] = "";
                          }
                          else
                              dr["imageUrl"] = "images/ch_wsb.png";

                      rowCount--;
                  }
              }

              sn.Message.DsGarminLocations = ds;
              dgGarminHistLocations.ClearCachedDataSource();
              dgGarminHistLocations.RebindDataSource();

          }
          catch
          {
          }
      }
      protected void dgGarminHistLocations_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
      {
          try
          {
              if ((sn.Message.DsGarminLocations  != null) && (optMessageType.SelectedIndex == 3))
              {
                  e.DataSource = sn.Message.DsGarminLocations;

              }
              else
                  e.DataSource = null;
          }
          catch
          {
          }
      }
      protected void dgGarminHist_InitializeRow(object sender, ISNet.WebUI.WebGrid.RowEventArgs e)
      {
          //if (e.Row.Type == ISNet.WebUI.WebGrid.RowType.Record)
          //{
          //  e.Row.Cells[0].Image = e.Row.Cells[1].Text;
          //}


      }

      protected void dgGarminHist_InitializeLayout(object sender, ISNet.WebUI.WebGrid.LayoutEventArgs e)
      {
          try
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
          catch
          {
          }
      }
      protected void dgGarminHistLocations_InitializeLayout(object sender, ISNet.WebUI.WebGrid.LayoutEventArgs e)
      {
          try
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
          catch
          {
          }
      }
}
}