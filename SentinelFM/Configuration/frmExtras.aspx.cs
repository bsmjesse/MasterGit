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
using AltaMapWS;
using System.Globalization;
using VLF.CLS.Def;

namespace SentinelFM
{
   /// <summary>
   /// Summary description for frmExtras
   /// </summary>
    public partial class Configuration_frmExtras : SentinelFMBasePage
   {
      protected DataSet dsAlarmSeverity = new DataSet();
      
      protected ServerDBOrganization.DBOrganization orgProxy = new ServerDBOrganization.DBOrganization();

      protected void Page_Load(object sender, System.EventArgs e)
      {
         try
         {
            if (!Page.IsPostBack)
            {
               CboFleet_Fill();
            }
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
               VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            base.RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
               VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() +
               " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            base.RedirectToLogin();
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
             cboFleet.Items.Insert(0, new ListItem("Please select fleet", "-1")); //(string)base.GetLocalResourceObject("SelectFleet")
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
            DataSet dsVehicle = new DataSet();

            string xml = "";
            ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet();
            
            if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetIdFeatures(sn.UserID, sn.SecId, fleetId, (long)Enums.FwAttributes.REEFER, ref xml), false))
               if (objUtil.ErrCheck(dbf.GetVehiclesInfoXMLByFleetIdFeatures(sn.UserID, sn.SecId, fleetId, (long)Enums.FwAttributes.REEFER, ref xml), true))
               {
                  return;
               }

            if (String.IsNullOrEmpty(xml))
            {
               return;
            }

            dsVehicle.ReadXml(new StringReader(xml));
            cboVehicle.DataSource = dsVehicle;
            cboVehicle.DataBind();
            cboVehicle.Items.Insert(0, new ListItem("Please select vehicle", "-1")); //(string)base.GetLocalResourceObject("SelectVehicle")
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

      protected void cboFleet_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (Convert.ToInt32(cboFleet.SelectedItem.Value) != -1)
         {
            //this.dgSensors.DataSource = null;
            //this.dgSensors.DataBind();
            this.cboVehicle.DataSource = null;
            this.cboVehicle.DataBind();
            //this.dgMessages.DataSource = null;
            //this.dgMessages.DataBind();
            this.cboVehicle.Visible = true;
            this.lblVehicleName.Visible = true;
            CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
         }
         else
         {
            this.cboVehicle.DataSource = null;
            this.cboVehicle.DataBind();
            this.cboVehicle.Visible = false;
            //this.dgSensors.DataSource = null;
            //this.dgSensors.DataBind();
            //this.dgMessages.DataSource = null;
            //this.dgMessages.DataBind();
            this.lblVehicleName.Visible = false;
            //this.tblHeader.Visible = false;
         }
      }

      protected void cboVehicle_SelectedIndexChanged(object sender, EventArgs e)
      {
         if (cboVehicle.SelectedIndex != 0)
         {
            //dgSensors.EditItemIndex = -1;
            //dgSensors.SelectedIndex = -1;
            //dgMessages.EditItemIndex = -1;
            //dgMessages.SelectedIndex = -1;
            DgSensors_Fill(cboVehicle.SelectedValue);
            //DgMessage_Fill();
            //this.tblHeader.Visible = true;
         }
         else
         {
            //this.tblHeader.Visible = false;
            //this.dgSensors.DataSource = null;
            //this.dgSensors.DataBind();
            //this.dgMessages.DataSource = null;
            //this.dgMessages.DataBind();
         }
      }

      private void DgSensors_Fill(string licensePlate)
      {
         try
         {
            DataSet dsSensor = new DataSet();
            string xml = "";
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
            
            if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByLang(sn.UserID, sn.SecId, licensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
               if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByLang(sn.UserID, sn.SecId, licensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
               {
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, 
                     VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "frmExtras -> No vehicle sensors: " + licensePlate));
                  return;
               }

            if (String.IsNullOrEmpty(xml))
            {
               System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                  VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "frmExtras -> No vehicle sensors: " + licensePlate));
               return;
            }

            dsSensor.ReadXml(new StringReader(xml));

            // Show SevirityNameOn
            DataColumn dcOn = new DataColumn();
            dcOn.ColumnName = "SeverityNameOn";
            dcOn.DataType = Type.GetType("System.String");
            dcOn.DefaultValue = "";
            dsSensor.Tables[0].Columns.Add(dcOn);

            // Show SevirityNameOff
            DataColumn dcOff = new DataColumn();
            dcOff.ColumnName = "SeverityNameOff";
            dcOff.DataType = Type.GetType("System.String");
            dcOff.DefaultValue = "";
            dsSensor.Tables[0].Columns.Add(dcOff);

            // Show ActionOn
            DataColumn dc = new DataColumn();
            dc.ColumnName = "ActionOn";
            dc.DataType = Type.GetType("System.String");
            dc.DefaultValue = "";
            dsSensor.Tables[0].Columns.Add(dc);

            // Show ActionOff
            dc = new DataColumn();
            dc.ColumnName = "ActionOff";
            dc.DataType = Type.GetType("System.String");
            dc.DefaultValue = "";
            dsSensor.Tables[0].Columns.Add(dc);

            short enumId = 0;

            foreach (DataRow rowItem in dsSensor.Tables[0].Rows)
            {
               enumId = Convert.ToInt16(rowItem["AlarmLevelOn"]);
               rowItem["SeverityNameOn"] = Enum.GetName(typeof(Enums.AlarmSeverity), (Enums.AlarmSeverity)enumId);
               enumId = Convert.ToInt16(rowItem["AlarmLevelOff"]);
               rowItem["SeverityNameOff"] = Enum.GetName(typeof(Enums.AlarmSeverity), (Enums.AlarmSeverity)enumId);

               string[] sensorAction = rowItem["SensorAction"].ToString().Split('/');
               if (sensorAction != null)
               {
                  if (sensorAction.Length > 0) rowItem["ActionOn"] = sensorAction[0];
                  if (sensorAction.Length > 1) rowItem["ActionOff"] = sensorAction[1];
               }
            }

            this.gdSensors.DataSource = dsSensor;
            this.gdSensors.DataBind();
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            base.RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      public int GetAlarmSeverity(int AlarmSeverityId)
      {
         try
         {
            DropDownList cboSensorActions = new DropDownList();
            cboSensorActions.DataValueField = "SeverityId";
            cboSensorActions.DataTextField = "SeverityName";
            DsAlarmSeverity_Fill();
            cboSensorActions.DataSource = dsAlarmSeverity;
            cboSensorActions.DataBind();

            cboSensorActions.SelectedIndex = -1;
            cboSensorActions.Items.FindByValue(AlarmSeverityId.ToString()).Selected = true;
            return cboSensorActions.SelectedIndex;
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            return 0;
         }
      }

      private void DsAlarmSeverity_Fill()
      {
         try
         {
            dsAlarmSeverity.Tables.Clear();
            string xml = "";

            ServerAlarms.Alarms dba = new ServerAlarms.Alarms();
            
            if (objUtil.ErrCheck(dba.GetAlarmSeverityXMLByLang(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
               if (objUtil.ErrCheck(dba.GetAlarmSeverityXMLByLang(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
               {
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, 
                     VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "frmExtras -> Error getting Alarms Severity"));
                  return;
               }

            if (String.IsNullOrEmpty(xml))
            {
               return;
            }

            dsAlarmSeverity.ReadXml(new StringReader(xml));
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
   }
}
