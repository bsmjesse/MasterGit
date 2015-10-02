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
using VLF.CLS.Def;
using VLF.CLS;
using System.IO;
using System.Globalization;
using System.Drawing;

namespace SentinelFM
{
   public partial class Configuration_frmdriversvehicles : SentinelFMBasePage  
   {
      ServerDBDriver.DBDriver driver = new ServerDBDriver.DBDriver();
      
      protected void Page_Load(object sender, EventArgs e)
      {
         try
         {
             if (!Page.IsPostBack)
             {
                 LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref form1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                 GuiSecurity(this);
             }

            lblMessage.Text = "";
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            //lblMessage.Text = Ex.Message;
           
         }
      }

      /// <summary>
      /// Assign current driver
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      protected void cmdAdd_Click(object sender, EventArgs e)
      {
         try
         {
            
            lblMessage.Text = "";

            if (lboAssigned.Items.Count > 0) // already assigned
            {
               ShowMessage(lblMessage, this.GetLocalResourceObject("resVehicleAlreadyAssigned").ToString(), Color.Red);
               return;
            }

            if (lboUnassigned.Items.Count == 0)
            {
               ShowMessage(lblMessage, this.GetLocalResourceObject("resNoDriversAvailable").ToString(), Color.Red);
               return;
            }

            if (lboUnassigned.SelectedIndex >= 0)
            {
                 
               // assign driver
               if (objUtil.ErrCheck(driver.AssignDriver(sn.UserID, sn.SecId,
                  Convert.ToInt64(ddlVehicle.SelectedValue),
                  Convert.ToInt32(lboUnassigned.SelectedValue), "Driver assigned"), false))
                  if (objUtil.ErrCheck(driver.AssignDriver(sn.UserID, sn.SecId,
                     Convert.ToInt64(ddlVehicle.SelectedValue),
                     Convert.ToInt32(lboUnassigned.SelectedValue), "Driver assigned"), true))
                  {
                     System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                        " Add driver error for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                     ShowMessage(lblMessage, this.GetLocalResourceObject("resAssignDriverError").ToString(), Color.Red);
                     return;
                  }
               // update assigned drivers
               GetAssignedDriverForVehicle(Convert.ToInt64(ddlVehicle.SelectedValue));

               // update unassigned drivers
               GetUnassignedDrivers();
            }
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            //lblMessage.Text = Ex.Message;
            
         }
      }

      /// <summary>
      /// Unassign current driver
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      protected void cmdRemove_Click(object sender, EventArgs e)
      {
         try
         {
            
            lblMessage.Text = "";

            if (lboAssigned.SelectedIndex >= 0)
            {
               if (objUtil.ErrCheck(driver.DeleteDriverAssignment(sn.UserID, sn.SecId, Convert.ToInt64(ddlVehicle.SelectedValue),
                  Convert.ToInt32(lboAssigned.SelectedValue), "Driver unassigned"), false))
                  if (objUtil.ErrCheck(driver.DeleteDriverAssignment(sn.UserID, sn.SecId, Convert.ToInt64(ddlVehicle.SelectedValue),
                       Convert.ToInt32(lboAssigned.SelectedValue), "Driver unassigned"), true))
                  {
                     System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                        " Delete driver error for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                     ShowMessage(lblMessage, this.GetLocalResourceObject("resUnassignDriverError").ToString(), Color.Red);
                     return;
                  }

               GetUnassignedDrivers();
               GetAssignedDriverForVehicle(Convert.ToInt64(ddlVehicle.SelectedValue));
            }
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            //lblMessage.Text = Ex.Message;
           
         }
      }

      /// <summary>
      /// Show driver assignment history
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      protected void cmdShowHistory_Click(object sender, EventArgs e)
      {
         DateTime startDT = new DateTime(), endDT = new DateTime();
         
         int driverId = 0;
         try
         {
            lblMessage.Text = "";
            DataSet dsResult = new DataSet();
            string xmlResult = "";

            if (pnlPostfactum.Visible == true)
            {
                if (!DatesValid_NewTZ(ref startDT, ref endDT)) return;
               //startDT = Convert.ToDateTime(txtStartAssgnDate.Text + " " + txtStartAssgnTime.Text + ddlStartAM.SelectedItem.Text, 
               //   new CultureInfo("en-US"));
               //endDT = Convert.ToDateTime(txtEndAssgnDate.Text + " " + txtEndAssgnTime.Text + ddlEndAM.SelectedItem.Text,
               //   new CultureInfo("en-US"));
               if (!DriverValid(lboAllDrivers.SelectedIndex)) return;
               driverId = Convert.ToInt32(lboAllDrivers.SelectedValue);
            }
            else // current assignment show day before and day after
            {
               if (DriverValid(lboAssigned.SelectedIndex))
                  driverId = Convert.ToInt32(lboAssigned.SelectedValue);
               else
               {
                  if (DriverValid(lboUnassigned.SelectedIndex))
                     driverId = Convert.ToInt32(lboUnassigned.SelectedValue);
                  else
                     return;
               }
               startDT = DateTime.UtcNow.AddDays(-1);
               endDT = DateTime.UtcNow.AddDays(1);
               
            }

            if (objUtil.ErrCheck(driver.GetDriverAssignmentHistoryByDates(sn.UserID, sn.SecId, 
               driverId, startDT, endDT, ref xmlResult), false))
               if (objUtil.ErrCheck(driver.GetDriverAssignmentHistoryByDates(sn.UserID, sn.SecId,
                  driverId, startDT, endDT, ref xmlResult), true))
               {
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(Enums.TraceSeverity.Error,
                     " No Vehicles for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                  ShowMessage(lblMessage, this.GetLocalResourceObject("resNoAssignments").ToString(), Color.Red);
                  return;
               }

            if (!String.IsNullOrEmpty(xmlResult))
            {
               dsResult.ReadXmlSchema(Server.MapPath("../Datasets/DriverAssignmentHistory.xsd"));
               dsResult.ReadXml(new System.IO.StringReader(xmlResult), XmlReadMode.InferSchema);
               if (VLF.CLS.Util.IsDataSetValid(dsResult))
               {
                   // Changes for TimeZone Feature start
                   float timeZone = Convert.ToSingle(sn.User.NewFloatTimeZone + sn.User.DayLightSaving);
                   string dateFormat = "MMM dd, yyyy HH:mm";
                   foreach (DataRow r in dsResult.Tables[0].Rows)
                   {
                       string assignedDateTime = r["AssignedDateTime"].ToString();
                       string assignedDateTimeTimeZone = DateTime.Parse(assignedDateTime).AddHours(-timeZone).ToString(dateFormat);
                       r.SetField("AssignedDateTime", assignedDateTimeTimeZone);

                       string unassignedDateTime = r["UnassignedDateTime"].ToString();
                       string unassignedDateTimeTimeZone = DateTime.Parse(unassignedDateTime).AddHours(-timeZone).ToString(dateFormat);
                       r.SetField("UnassignedDateTime", unassignedDateTimeTimeZone);
                   }
                  gdvAssgnHistory.DataSource = dsResult.Tables[0];
               }
               lblMessage.Text = "";
            }
            else
            {
               gdvAssgnHistory.DataSource = null;
               ShowMessage(lblMessage, this.GetLocalResourceObject("resNoAssignments").ToString(), Color.Red);
            }
            gdvAssgnHistory.DataBind();
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
            //lblMessage.Text = Ex.Message;
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            //lblMessage.Text = Ex.Message;
           
         }
      }

      /// <summary>
      /// Post factum assignment
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      protected void cmdAssignDriver_Click(object sender, EventArgs e)
      {
         DateTime stDate = new DateTime(), endDate = new DateTime();
         
         try
         {
            lblMessage.Text = "";
            if (!DatesValid_NewTZ(ref stDate, ref endDate)) return;
            if (lboAllDrivers.SelectedIndex >= 0)
            {
               //stDate = Convert.ToDateTime(txtStartAssgnDate.Text + " " +
               //   txtStartAssgnTime.Text + " " + ddlStartAM.SelectedItem.Text).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving);
               //endDate = Convert.ToDateTime(txtEndAssgnDate.Text + " " +
               //   txtEndAssgnTime.Text + " " + ddlEndAM.SelectedItem.Text).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving);

               // assign driver
                stDate = stDate.AddSeconds(1);
                endDate = endDate.AddSeconds(-1);
               if (objUtil.ErrCheck(driver.AddAssignmentHistory(sn.UserID, sn.SecId,
                  Convert.ToInt32(lboAllDrivers.SelectedValue), Convert.ToInt64(ddlPostVehicle.SelectedValue), DateTime.UtcNow.ToString(),
                  stDate, endDate), false))
                  if (objUtil.ErrCheck(driver.AddAssignmentHistory(sn.UserID, sn.SecId,
                     Convert.ToInt32(lboAllDrivers.SelectedValue), Convert.ToInt64(ddlPostVehicle.SelectedValue), DateTime.UtcNow.ToString(),
                     stDate, endDate), true))
                  {
                     System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                        " Add driver error for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                     //RedirectToLogin();
                     ShowMessage(lblMessage, this.GetLocalResourceObject("resFailedAddHistory").ToString(), Color.Red);
                     return;
                  }
               ShowMessage(lblMessage, this.GetLocalResourceObject("resHistoryAdded").ToString(), Color.DarkGreen);
            }
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            //lblMessage.Text = Ex.Message;
          
         }
      }

      /// <summary>
      /// Show vehicle assignment history
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      protected void cmdShowVehicleHistory_Click(object sender, EventArgs e)
      {
         DateTime startDT = new DateTime(), endDT = new DateTime();
         long vehicleId = 0;
         try
         {
            lblMessage.Text = "";
            DataSet dsResult = new DataSet();
            string xmlResult = "";

            if (pnlPostfactum.Visible == true)
            {
                if (!DatesValid_NewTZ(ref startDT, ref endDT)) return;
               //startDT = Convert.ToDateTime(txtStartAssgnDate.Text + " " + txtStartAssgnTime.Text + ddlStartAM.SelectedItem.Text,
               //   new CultureInfo("en-US"));
               //endDT = Convert.ToDateTime(txtEndAssgnDate.Text + " " + txtEndAssgnTime.Text + ddlEndAM.SelectedItem.Text,
               //   new CultureInfo("en-US"));
               if (!VehicleValid(ddlPostVehicle.SelectedIndex)) return;
               vehicleId = Convert.ToInt64(ddlPostVehicle.SelectedValue);
            }
            else // current assignment show day before and day after
            {
               startDT = DateTime.UtcNow.AddDays(-1);
               endDT = DateTime.UtcNow.AddDays(1);
               if (!VehicleValid(ddlVehicle.SelectedIndex)) return;
               vehicleId = Convert.ToInt64(ddlVehicle.SelectedValue);
            }

            //using (ServerDBFleet.DBFleet fleet = new ServerDBFleet.DBFleet())
            //{
            
            ServerDBDriver.DBDriver driver = new ServerDBDriver.DBDriver();

            if (objUtil.ErrCheck(driver.GetVehicleAssignmentHistoryByDates(sn.UserID, sn.SecId,
               vehicleId, startDT, endDT, ref xmlResult), false))
               if (objUtil.ErrCheck(driver.GetVehicleAssignmentHistoryByDates(sn.UserID, sn.SecId,
                  vehicleId, startDT, endDT, ref xmlResult), true))
               {
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(Enums.TraceSeverity.Error,
                     " No Vehicles for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                  ShowMessage(lblMessage, this.GetLocalResourceObject("resNoAssignments").ToString(), Color.Red);
                  //RedirectToLogin();
                  return;
               }

            if (!String.IsNullOrEmpty(xmlResult))
            {
               dsResult.ReadXmlSchema(Server.MapPath("../Datasets/DriverAssignmentHistory.xsd"));
               dsResult.ReadXml(new System.IO.StringReader(xmlResult), XmlReadMode.InferSchema);
               if (VLF.CLS.Util.IsDataSetValid(dsResult))
               {
                   float timeZone = Convert.ToSingle(sn.User.NewFloatTimeZone + sn.User.DayLightSaving);
                   string dateFormat = "MMM dd, yyyy HH:mm";
                   foreach (DataRow r in dsResult.Tables[0].Rows)
                   {
                       string assignedDateTime = r["AssignedDateTime"].ToString();
                       string assignedDateTimeTimeZone = DateTime.Parse(assignedDateTime).AddHours(-timeZone).ToString(dateFormat);
                       r.SetField("AssignedDateTime", assignedDateTimeTimeZone);

                       string unassignedDateTime = r["UnassignedDateTime"].ToString();
                       string unassignedDateTimeTimeZone = DateTime.Parse(unassignedDateTime).AddHours(-timeZone).ToString(dateFormat);
                       r.SetField("UnassignedDateTime", unassignedDateTimeTimeZone);
                   }
                  gdvAssgnHistory.DataSource = dsResult.Tables[0];
               }
               lblMessage.Text = "";
            }
            else
            {
               gdvAssgnHistory.DataSource = null;
               ShowMessage(lblMessage, this.GetLocalResourceObject("resNoAssignments").ToString(), Color.Red);
            }
            gdvAssgnHistory.DataBind();
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace));
            RedirectToLogin();
            //lblMessage.Text = Ex.Message;
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            //lblMessage.Text = Ex.Message;
          
         }
      }

      # region Private methods

      # region Validation

      // Changes for TimeZone Feature start

      private bool DatesValid_NewTZ(ref DateTime startDT, ref DateTime endDT)
      {
          if (String.IsNullOrEmpty(txtStartAssgnDate.Text))
          {
              lblMessage.Text = this.GetLocalResourceObject("resFillStartDate").ToString(); //;
              return false;
          }
          try
          {
              startDT = Convert.ToDateTime(this.txtStartAssgnDate.Text + " " + this.txtStartAssgnTime.Text + this.ddlStartAM.SelectedItem.Text,
                 new System.Globalization.CultureInfo("en-US")).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving);
          }
          catch
          {
              lblMessage.Text = this.GetLocalResourceObject("resInvStartDate").ToString();//"Invalid start date";
              txtStartAssgnDate.Text = "";
              txtStartAssgnTime.Text = "";
              txtStartAssgnDate.Focus();
              return false;
          }

          //if (!DateTime.TryParseExact(txtStartAssgnDate.Text + " " + txtStartAssgnTime.Text + ddlStartAM.SelectedItem.Text,
          //   "MM/dd/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out startDT))
          //{
          //   lblErrorStartDate.Text = this.GetLocalResourceObject("resInvStartDate").ToString();
          //   return false;
          //}
          if (String.IsNullOrEmpty(txtEndAssgnDate.Text))
          {
              lblMessage.Text = this.GetLocalResourceObject("resFillEndDate").ToString(); //;
              return false;
          }
          try
          {
              endDT = Convert.ToDateTime(this.txtEndAssgnDate.Text + " " + this.txtEndAssgnTime.Text + this.ddlEndAM.SelectedItem.Text,
                 new System.Globalization.CultureInfo("en-US")).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving);
          }
          catch
          {
              lblMessage.Text = this.GetLocalResourceObject("resInvEndDate").ToString();//"Invalid start date";
              txtEndAssgnDate.Text = "";
              txtEndAssgnTime.Text = "";
              txtEndAssgnDate.Focus();
              return false;
          }

          //if (!DateTime.TryParseExact(txtEndAssgnDate.Text + " " + txtEndAssgnTime.Text + ddlEndAM.SelectedItem.Text,
          //   "MM/dd/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out endDT))
          //{
          //   lblErrorEndDate.Text = this.GetLocalResourceObject("resInvEndDate").ToString();
          //   return false;
          //}
          if (startDT > endDT)
          {
              lblMessage.Text = this.GetLocalResourceObject("resStartDateExceed").ToString();
              return false;
          }
          return true;
      }

      // Changes for TimeZone Feature end
      private bool DatesValid(ref DateTime startDT, ref DateTime endDT)
      {
         if (String.IsNullOrEmpty(txtStartAssgnDate.Text))
         {
            lblMessage.Text = this.GetLocalResourceObject("resFillStartDate").ToString(); //;
            return false;
         }
         try
         {
            startDT = Convert.ToDateTime(this.txtStartAssgnDate.Text + " " + this.txtStartAssgnTime.Text + this.ddlStartAM.SelectedItem.Text,
               new System.Globalization.CultureInfo("en-US")).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving);
         }
         catch
         {
            lblMessage.Text = this.GetLocalResourceObject("resInvStartDate").ToString();//"Invalid start date";
            txtStartAssgnDate.Text = "";
            txtStartAssgnTime.Text = "";
            txtStartAssgnDate.Focus();
            return false;
         }

         //if (!DateTime.TryParseExact(txtStartAssgnDate.Text + " " + txtStartAssgnTime.Text + ddlStartAM.SelectedItem.Text,
         //   "MM/dd/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out startDT))
         //{
         //   lblErrorStartDate.Text = this.GetLocalResourceObject("resInvStartDate").ToString();
         //   return false;
         //}
         if (String.IsNullOrEmpty(txtEndAssgnDate.Text))
         {
            lblMessage.Text = this.GetLocalResourceObject("resFillEndDate").ToString(); //;
            return false;
         }
         try
         {
            endDT = Convert.ToDateTime(this.txtEndAssgnDate.Text + " " + this.txtEndAssgnTime.Text + this.ddlEndAM.SelectedItem.Text,
               new System.Globalization.CultureInfo("en-US")).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving);
         }
         catch
         {
            lblMessage.Text = this.GetLocalResourceObject("resInvEndDate").ToString();//"Invalid start date";
            txtEndAssgnDate.Text = "";
            txtEndAssgnTime.Text = "";
            txtEndAssgnDate.Focus();
            return false;
         }

         //if (!DateTime.TryParseExact(txtEndAssgnDate.Text + " " + txtEndAssgnTime.Text + ddlEndAM.SelectedItem.Text,
         //   "MM/dd/yyyy", new CultureInfo("en-US"), DateTimeStyles.None, out endDT))
         //{
         //   lblErrorEndDate.Text = this.GetLocalResourceObject("resInvEndDate").ToString();
         //   return false;
         //}
         if (startDT > endDT)
         {
            lblMessage.Text = this.GetLocalResourceObject("resStartDateExceed").ToString();
            return false;
         }
         return true;
      }

      private bool VehicleValid(int vehicleIndex)
      {
         if (vehicleIndex < 0)
         {
            lblMessage.ForeColor = Color.Red;
            lblMessage.Text = this.GetLocalResourceObject("resSelectVehicle").ToString();
            return false;
         }

         return true;
      }

      private bool DriverValid(int driverIndex)
      {
         if (driverIndex < 0)
         {
            lblMessage.ForeColor = Color.Red;
            lblMessage.Text = this.GetLocalResourceObject("resSelectDriver").ToString();
            return false;
         }

         return true;
      }
      # endregion

      /// <summary>
      /// Get assigned driver for vehicle
      /// </summary>
      /// <param name="vehiclId">Vehicle Id</param>
      private void GetAssignedDriverForVehicle(long vehiclId)
      {
         string xmlResult = "";
         lblMessage.Text = "";

            if (objUtil.ErrCheck(driver.GetDriverAssignment(sn.UserID, sn.SecId, vehiclId, ref xmlResult), false))
               if (objUtil.ErrCheck(driver.GetDriverAssignment(sn.UserID, sn.SecId, vehiclId, ref xmlResult), true))
               {
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                     " Get Driver Assignment error for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                  //RedirectToLogin();
                  return;
               }

         if (String.IsNullOrEmpty(xmlResult))
         {
            lboAssigned.Items.Clear();
            //pnlShowHistory.Visible = false;
            return;
         }

         DataSet dsDriversAssn = new DataSet();
         dsDriversAssn.ReadXml(new StringReader(xmlResult));

         if (!VLF.CLS.Util.IsDataSetValid(dsDriversAssn))
         {
            lboAssigned.Items.Clear();
            //pnlShowHistory.Visible = false;
            return;
         }
         
         lboAssigned.DataSource = dsDriversAssn;
         lboAssigned.DataValueField = "DriverId";
         lboAssigned.DataTextField = "FullName";
         lboAssigned.DataBind();
      }

      /// <summary>
      /// Fill unassigned drivers list
      /// </summary>
      private void GetUnassignedDrivers()
      {
         DataSet dsUdrivers = new DataSet();
         string xmlResult = "";
         lblMessage.Text = "";
         if (objUtil.ErrCheck(driver.GetUnassignedDrivers(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), false))
            if (objUtil.ErrCheck(driver.GetUnassignedDrivers(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlResult), true))
            {
               System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(Enums.TraceSeverity.Error,
                  " Get Unasssigned Drivers error  for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
               //RedirectToLogin();
               return;
            }
         if (!String.IsNullOrEmpty(xmlResult))
         {
            dsUdrivers.ReadXml(new StringReader(xmlResult));
            if (VLF.CLS.Util.IsDataSetValid(dsUdrivers))
            {
               //dsUdrivers.Tables[0].Columns.Add("FullName");
               //foreach (DataRow dr in dsUdrivers.Tables[0].Rows)
               //{
               //   dr["FullName"] = dr["FirstName"] + "  " + dr["LastName"];
               //}
               lboUnassigned.DataSource = dsUdrivers;
               lboUnassigned.DataValueField = "DriverId";
               lboUnassigned.DataTextField = "FullName";
               lboUnassigned.DataBind();
            }
         }
         else
         {
            lboUnassigned.Items.Clear();
         }
      }

      /// <summary>
      /// Fill vehicles ddl for a fleet
      /// </summary>
      /// <param name="fleetId"></param>
      /// <param name="cboVehicle"></param>
      private void GetFleetVehicles(int fleetId, DropDownList cboVehicle)
      {
         lblMessage.Text = "";
         DataSet dsVehicles = new DataSet();
         string xmlResult = "";

         using (ServerDBFleet.DBFleet fleet = new ServerDBFleet.DBFleet())
         {
            if (objUtil.ErrCheck(fleet.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xmlResult), false))
               if (objUtil.ErrCheck(fleet.GetVehiclesInfoXMLByFleetId(sn.UserID, sn.SecId, fleetId, ref xmlResult), true))
               {
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(Enums.TraceSeverity.Error,
                     " No Vehicles for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                  ShowMessage(lblMessage, this.GetLocalResourceObject("resNoVehiclesForFleet").ToString(), Color.Red);
                  cboVehicle.Items.Insert(0, new ListItem(base.GetLocalResourceObject("resSelectVehicle").ToString(), "-1"));
                  return;
               }

            if (String.IsNullOrEmpty(xmlResult))
            {
               ShowMessage(lblMessage, this.GetLocalResourceObject("resNoVehiclesForFleet").ToString(), Color.Red);
               cboVehicle.Items.Insert(0, new ListItem(base.GetLocalResourceObject("resSelectVehicle").ToString(), "-1"));
               return;
            }

            dsVehicles.ReadXml(new System.IO.StringReader(xmlResult));
            if (!Util.IsDataSetValid(dsVehicles))
            {
               ShowMessage(lblMessage, this.GetLocalResourceObject("resNoVehiclesForFleet").ToString(), Color.Red);
               cboVehicle.Items.Insert(0, new ListItem(base.GetLocalResourceObject("resSelectVehicle").ToString(), "-1"));
               return;
            }

            // bind ddl
            cboVehicle.DataSource = dsVehicles;
            cboVehicle.DataBind();

            // insert "Please select..."
            cboVehicle.Items.Insert(0, new ListItem(base.GetLocalResourceObject("resSelectVehicle").ToString(), "-1"));
            cboVehicle.SelectedIndex = 0;

            lblMessage.Text = "";
         }
      }

      /// <summary>
      /// Load drivers for the company
      /// </summary>
      private void GetAllDrivers()
      {
         string xmlDrivers = "";
         lblMessage.Text = "";
         if (objUtil.ErrCheck(driver.GetAllDrivers(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlDrivers), false))
            if (objUtil.ErrCheck(driver.GetAllDrivers(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlDrivers), true))
            {
               System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, Util.TraceFormat(Enums.TraceSeverity.Error,
                  " No drivers for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
               //RedirectToLogin();
               ShowMessage(lblMessage, this.GetLocalResourceObject("resNoDrivers").ToString(), Color.Red);
               return;
            }

         if (String.IsNullOrEmpty(xmlDrivers))
         {
            ShowMessage(lblMessage, this.GetLocalResourceObject("resNoDrivers").ToString(), Color.Red);
            return;
         }

         DataSet dsDrivers = new DataSet();
         dsDrivers.ReadXml(new StringReader(xmlDrivers));
         if (Util.IsDataSetValid(dsDrivers))
         {
            lboAllDrivers.DataSource = dsDrivers;
            lboAllDrivers.DataValueField = "DriverId";
            lboAllDrivers.DataTextField = "FullName";
            lboAllDrivers.DataBind();
         }
         else
         {
            ShowMessage(lblMessage, this.GetLocalResourceObject("resNoDrivers").ToString(), Color.Red);
         }
      }

      /// <summary>
      /// Fills fleets dropdownlist
      /// </summary>
      /// <param name="combo"></param>
      private void GetFleets(DropDownList cboFleet)
      {
            DataSet dsFleets = new DataSet();
            dsFleets = sn.User.GetUserFleets(sn);  
            cboFleet.DataSource = dsFleets;
            cboFleet.DataBind();
            cboFleet.Items.Insert(0, new ListItem(base.GetLocalResourceObject("resSelectFleet").ToString(), "-1"));
      }

      /// <summary>
      /// Clear assignments history grid and show or hide the panel
      /// </summary>
      /// <param name="showPanel">True - show, false - hide</param>
      private void ClearHistory(bool showPanel)
      {
         gdvAssgnHistory.DataSource = null;
         gdvAssgnHistory.DataBind();
         pnlShowHistory.Visible = showPanel;
      }

      # endregion

      # region Drop down list changed

      /// <summary>
      /// Select current or postfactum assignment
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      protected void ddlAssgnType_SelectedIndexChanged(object sender, EventArgs e)
      {
         try
         {
            lblMessage.Text = "";
            //gdvAssgnHistory.DataSource = null;
            //gdvAssgnHistory.DataBind();
            pnlShowHistory.Visible = false;

            switch (ddlAssgnType.SelectedIndex)
            {
               case 0:
                  pnlAssignment.Visible = false;
                  pnlPostfactum.Visible = false;
                  break;
               case 1: // current
                  pnlAssignment.Visible = true;
                  tblAssignment.Visible = false;
                  pnlPostfactum.Visible = false;
                  GetFleets(ddlFleet);
                  if (ddlVehicle.Visible) ddlVehicle.SelectedIndex = 0;
                  GetUnassignedDrivers();
                  break;
               case 2: // postfactum
                  pnlAssignment.Visible = false;
                  pnlPostfactum.Visible = true;
                  GetFleets(ddlPostFleet);
                  if (ddlPostVehicle.Visible) ddlPostVehicle.SelectedIndex = 0;
                  GetAllDrivers();
                  break;
            }
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            //lblMessage.Text = Ex.Message;
           
         }
      }

      /// <summary>
      /// Fleet for post factum assignment
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      protected void ddlPostFleet_SelectedIndexChanged(object sender, EventArgs e)
      {
         try
         {
            postDriverAssgn.Visible = false;
            pnlShowHistory.Visible = false;

            if (ddlPostFleet.SelectedValue == "-1")
            {
               ddlPostVehicle.Visible = false;
               lblPostVehicle.Visible = false;
               return;
            }

            // show up vehicle
            ddlPostVehicle.Visible = true;
            lblPostVehicle.Visible = true;

            // fill vehicles combo acc. to the selected fleet
            GetFleetVehicles(Convert.ToInt32(ddlPostFleet.SelectedValue), ddlPostVehicle);
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            //lblMessage.Text = Ex.Message;
          
         }
      }

      /// <summary>
      /// Fleet for current assignment
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      protected void ddlFleet_SelectedIndexChanged(object sender, EventArgs e)
      {
         try
         {
            pnlShowHistory.Visible = false;
            tblAssignment.Visible = false;

            if (ddlFleet.SelectedValue == "-1")
            {
               //ddlVehicle.SelectedIndex = 0;
               divVehicle.Visible = false;
               return;
            }

            // show up vehicle
            divVehicle.Visible = true;

            // fill vehicles combo acc. to the selected fleet
            GetFleetVehicles(Convert.ToInt32(ddlFleet.SelectedValue), ddlVehicle);
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            //lblMessage.Text = Ex.Message;
            
         }
      }

      /// <summary>
      /// Select vehicle for current assignment
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      protected void ddlVehicle_SelectedIndexChanged(object sender, EventArgs e)
      {
         try
         {
            lblMessage.Text = "";
            if (ddlVehicle.SelectedValue == "-1")
            {
               lboAssigned.Items.Clear();
               // hide the table
               tblAssignment.Visible = false;
               //ClearHistory(false);
               return;
            }
            // show up table with driver lists and assgn. buttons
            tblAssignment.Visible = true;

            // get assigned driver for vehicle
            GetAssignedDriverForVehicle(Convert.ToInt64(ddlVehicle.SelectedValue));

            //ClearHistory(true);
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            //lblMessage.Text = Ex.Message;
           
         }
      }

      /// <summary>
      /// Select vehicle for post factum assignment
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      protected void ddlPostVehicle_SelectedIndexChanged(object sender, EventArgs e)
      {
         try
         {
            lblPostVehicle.Visible = true;
            if (ddlPostVehicle.SelectedIndex == 0)
            {
               postDriverAssgn.Visible = false;
               pnlShowHistory.Visible = false;
               return;
            }
            postDriverAssgn.Visible = true;
            pnlShowHistory.Visible = true;
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            return;
         }
      }

      # endregion
      protected void cmdScheduledTasks_Click(object sender, EventArgs e)
      {
          Response.Redirect("frmTaskScheduler.aspx"); 
      }
}
}
