using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Drawing;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;
using System.IO;
using VLF.CLS.Def;
using VLF.CLS;
using VLF.CLS.Interfaces;
using System.Threading;
using System.Text;

namespace SentinelFM
{
   public partial class frmReefer : SensorInfoPage
   {
      
      protected void Page_Load(object sender, EventArgs e)
      {
         if (!Page.IsPostBack)
         {
            try
            {
               LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref formReefer, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
               if (Request.QueryString["LicensePlate"] == null)
               {
                  lblMessage.Text = "License Plate is empty";
                  return;
               }
               this.LicensePlate = Request.QueryString["LicensePlate"];
               GetProducts();
               SetReeferSensors();
            }
            catch (NullReferenceException Ex)
            {
               System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                  VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
               lblMessage.Text = Ex.Message;
            }
            catch (Exception Ex)
            {
               System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                  VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() +
                  " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
               lblMessage.Text = Ex.Message;
            }
         }
      }

      /// <summary>
      /// Fill a grid of status sensors
      /// </summary>
      /// <param name="rowCollection"></param>
      /// <param name="lowId"></param>
      /// <param name="upperId"></param>
      private void SetStatusSensors(DataRowCollection rowCollection, short lowId, short upperId)
      {
         DataTable dtbl = new DataTable("StatusSensors");
         dtbl.Columns.Add("SensorId", typeof(short));
         dtbl.Columns.Add("SensorName", typeof(String));
         dtbl.Columns.Add("SensorActive", typeof(Boolean));

         foreach (DataRow row in rowCollection)
         {
            if (Convert.ToInt16(row[0]) >= lowId && Convert.ToInt16(row[0]) <= upperId)
               dtbl.Rows.Add(row["SensorId"], row["SensorName"], false);
         }

         gvStatusSensors.DataSource = dtbl;
         gvStatusSensors.DataBind();
      }

      /// <summary>
      /// Fill grid of temper. sensors
      /// </summary>
      /// <param name="rowCollection"></param>
      /// <param name="lowId"></param>
      /// <param name="upperId"></param>
      private void SetTemperatureSensors(DataRowCollection rowCollection, short lowId, short upperId)
      {
         DataTable dtbl = new DataTable("TemperatureSensors");
         dtbl.Columns.Add("SensorId", typeof(short));
         dtbl.Columns.Add("SensorName", typeof(String));
         dtbl.Columns.Add("SensorActive", typeof(Boolean));
         dtbl.Columns.Add("Lower", typeof(float));
         dtbl.Columns.Add("Current", typeof(float));
         dtbl.Columns.Add("Upper", typeof(float));

         foreach (DataRow row in rowCollection)
         {
            if (Convert.ToInt16(row[0]) >= lowId && Convert.ToInt16(row[0]) <= upperId)
               dtbl.Rows.Add(row["SensorId"], row["SensorName"], false, 0.0, 0.0, 0.0);
         }

         this.gvReeferTempSensors.DataSource = dtbl;
         gvReeferTempSensors.DataBind();
      }

      /// <summary>
      /// Fill grid of fuel sensor
      /// </summary>
      /// <param name="dataRowCollection"></param>
      /// <param name="sensorId"></param>
      private void SetFuelSensor(DataRowCollection rowCollection, int sensorId)
      {
         DataTable dtbl = new DataTable("FuelSensor");
         dtbl.Columns.Add("SensorId", typeof(short));
         dtbl.Columns.Add("SensorName", typeof(String));
         dtbl.Columns.Add("SensorActive", typeof(Boolean));
         dtbl.Columns.Add("Lower", typeof(float));
         dtbl.Columns.Add("Current", typeof(float));
         dtbl.Columns.Add("Upper", typeof(float));

         foreach (DataRow row in rowCollection)
         {
            if (Convert.ToInt16(row[0]) == sensorId)
               dtbl.Rows.Add(row["SensorId"], row["SensorName"], false, 0.0, 0.0, 0.0);
         }

         this.gvFuelSensors.DataSource = dtbl;
         this.gvFuelSensors.DataBind();
      }

      /// <summary>
      /// Get all products for user and fill combo box
      /// </summary>
      private void GetProducts()
      { // DataTextField="ProductName" DataValueField="ProductId" 
         try
         {
            DataSet dsProducts = new DataSet();

            if (Session["ProductList"] == null)
            {
               string xml = "";
               
               ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();
               if (objUtil.ErrCheck(dbo.GetOrganizationAllProducts(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                  if (objUtil.ErrCheck(dbo.GetOrganizationAllProducts(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                  {
                     ddlReeferProducts.DataSource = null;
                     return;
                  }

               dsProducts.ReadXml(new StringReader(xml));

               Session["ProductList"] = dsProducts;
            }
            else
               dsProducts = (DataSet)Session["ProductList"];

            ddlReeferProducts.Items.Clear();

            foreach (DataRow productRow in dsProducts.Tables[0].Rows)
            {
               Product productItem = new Product(productRow["ProductID"].ToString(),
                  productRow["ProductName"].ToString(), productRow["Upper"].ToString(), productRow["Lower"].ToString());

               ddlReeferProducts.Items.Add(new ListItem(productItem.Text, productItem.Value));
            }

            ddlReeferProducts.Items.Insert(0, new ListItem("Please Select a Product", "-1")); //(string)base.GetLocalResourceObject("SelectFleet")
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      /// <summary>
      /// Get reefer sensors and populates grids
      /// </summary>
      /// <param name="licensePlate"></param>
      private void SetReeferSensors()
      {
         try
         {
            DataTable dtSensors = GetAllSensorsForVehicle(this.LicensePlate, true);
            if (dtSensors == null || dtSensors.Rows.Count == 0)
            {
               this.tblBody.Visible = false;
               this.lblMessage.Text = "Error getting sensors";
            }
            SetTemperatureSensors(dtSensors.Rows, 273, 277);

            SetFuelSensor(dtSensors.Rows, 281);

            SetStatusSensors(dtSensors.Rows, 257, 263);
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      /// <summary>
      /// Send command to the box to get sensors
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      protected void btnGet_Click(object sender, EventArgs e)
      {
         try
         {
            // send command to a box
            //processId = new Guid();
            //Thread th = new Thread(new ParameterizedThreadStart(SendBoxCommand));
            //th.Start(new ReeferThreadObject(Enums.CommandType.GetReeferSetup, ""));
            SendBoxCommand((short)Enums.CommandType.GetReeferSetup, "");
            // parse parameters into reefer sensors
            //GetReeferSetup();
            //Response.Redirect("frmProgress.aspx?ID=" + processId.ToString());
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
               VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            lblMessage.Text = Ex.Message;
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
               VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() +
               " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            lblMessage.Text = Ex.Message;
         }
      }

      /// <summary>
      /// Send command to the box to set sensors
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      protected void btnSet_Click(object sender, EventArgs e)
      {
         try
         {
            string setupParams = GenerateReeferSetupParams();
            if (!String.IsNullOrEmpty(setupParams))
            {
               SendBoxCommand((short)Enums.CommandType.SetReeferSetup, setupParams);
            }
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
               VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            lblMessage.Text = Ex.Message;
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
               VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() +
               " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            lblMessage.Text = Ex.Message;
         }
      }

      protected void gvTempSensors_RowEditing(object sender, GridViewEditEventArgs e)
      {
         try
         {
            //Check security 
            bool cmd = sn.User.ControlEnable(sn, 31);
            if (!cmd)
            {
               lblMessage.Visible = true;
               lblMessage.Text = "You don't have permissions for this operation.";
               return;
            }

            gvReeferTempSensors.EditIndex = e.NewEditIndex;
            SetReeferSensors();
            gvReeferTempSensors.SelectedIndex = -1;
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      protected void gvTempSensors_RowUpdating(object sender, GridViewUpdateEventArgs e)
      {
         try
         {
            foreach (DictionaryEntry entry in e.NewValues)
            {
               e.NewValues[entry.Key] = Server.HtmlEncode(entry.Value.ToString());
            }

            GridViewRow row = gvReeferTempSensors.Rows[gvReeferTempSensors.EditIndex];

            Int16 SensorID = Convert.ToInt16(gvReeferTempSensors.DataKeys[e.RowIndex].Value);

            this.lblMessage.Visible = true;

            if (String.IsNullOrEmpty(((TextBox)row.FindControl("txtCurrent")).Text))
            {
               this.lblMessage.Text = "Please fill in current temperature";
               return;
            }

            if (String.IsNullOrEmpty(((TextBox)row.FindControl("txtUpper")).Text))
            {
               this.lblMessage.Text = "Please fill in upper temperature";
               return;
            }

            if (String.IsNullOrEmpty(((TextBox)row.FindControl("txtLower")).Text))
            {
               this.lblMessage.Text = "Please fill in lower temperature";
               return;
            }

            this.lblMessage.Text = "";
            this.lblMessage.Visible = false;
            SetReeferSensors();
            gvReeferTempSensors.EditIndex = -1;
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      protected void gvTempSensors_RowCancelingEdit(object sender, GridViewCancelEditEventArgs e)
      {
         try
         {
            this.lblMessage.Text = "";
            gvReeferTempSensors.EditIndex = -1;
            e.Cancel = true;
            SetReeferSensors();
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      protected void ddlReeferProducts_SelectedIndexChanged(object sender, EventArgs e)
      {
         try
         {
            if (ddlReeferProducts.SelectedIndex > 0)
            {
               Product prd = new Product(ddlReeferProducts.SelectedValue);
               lblUpper.Text = prd.UpperLimit.ToString();//((Product)ddlProducts.SelectedItem).UpperLimit.ToString();
               lblLower.Text = prd.LowerLimit.ToString();//((Product)ddlProducts.SelectedItem).LowerLimit.ToString();
            }
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      /// <summary>
      /// Button 'Paste' was clicked
      /// Copy product's temperature limits into a sensor
      /// </summary>
      /// <param name="sender"></param>
      /// <param name="e"></param>
      protected void gvTempSensors_SelectedIndexChanged(object sender, EventArgs e)
      {
         try
         {
            if (!String.IsNullOrEmpty(lblUpper.Text) && !String.IsNullOrEmpty(lblLower.Text))
            {
               (gvReeferTempSensors.SelectedRow.FindControl("chkActive") as CheckBox).Checked = true;
               (gvReeferTempSensors.SelectedRow.FindControl("txtReeferUpper") as TextBox).Enabled = true;
               (gvReeferTempSensors.SelectedRow.FindControl("txtReeferUpper") as TextBox).Text = lblUpper.Text;
               (gvReeferTempSensors.SelectedRow.FindControl("txtReeferLower") as TextBox).Enabled = true;
               (gvReeferTempSensors.SelectedRow.FindControl("txtReeferLower") as TextBox).Text = lblLower.Text;
               (gvReeferTempSensors.SelectedRow.FindControl("ddlTempInterval") as DropDownList).Enabled = true;
            }
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
               VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            lblMessage.Text = Ex.Message;
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
               VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() +
               " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            lblMessage.Text = Ex.Message;
         }
      }

      protected void chkActive_CheckedChanged(object sender, EventArgs e)
      {
         try
         {
            foreach (GridViewRow row in gvReeferTempSensors.Rows)
            {
               (row.FindControl("txtReeferUpper") as TextBox).Enabled = (row.FindControl("chkActive") as CheckBox).Checked;
               (row.FindControl("txtReeferLower") as TextBox).Enabled = (row.FindControl("chkActive") as CheckBox).Checked;
               (row.FindControl("ddlTempInterval") as DropDownList).Enabled = (row.FindControl("chkActive") as CheckBox).Checked;
            }
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
               VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            lblMessage.Text = Ex.Message;
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
               VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() +
               " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            lblMessage.Text = Ex.Message;
         }
      }

      protected void chkFuelActive_CheckedChanged(object sender, EventArgs e)
      {
         try
         {
            GridViewRow row = gvFuelSensors.Rows[0];
            (row.FindControl("txtFuelUpper") as TextBox).Enabled = (row.FindControl("chkFuelActive") as CheckBox).Checked;
            (row.FindControl("txtFuelLower") as TextBox).Enabled = (row.FindControl("chkFuelActive") as CheckBox).Checked;
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
               VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            lblMessage.Text = Ex.Message;
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
               VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() +
               " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            lblMessage.Text = Ex.Message;
         }
      }

      protected void chkFuelLevelActive_CheckedChanged(object sender, EventArgs e)
      {
         try
         {
            this.txtFuelLevel.Enabled = this.chkFuelLevelActive.Checked;
            this.ddlFuelInterval.Enabled = this.chkFuelLevelActive.Checked;
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
               VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            lblMessage.Text = Ex.Message;
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
               VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() +
               " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            lblMessage.Text = Ex.Message;
         }
      }

      private string GenerateReeferSetupParams()
      {
         StringBuilder paramList = new StringBuilder();

         # region Temperature sensors

         // define sensor mask
         byte TemperatureSensorMask = 0, bit = 1;
         string lower = "", upper = "";
         for (int i = 0; i < this.gvReeferTempSensors.Rows.Count; i++)
         {
            CheckBox chkBox = (CheckBox)(gvReeferTempSensors.Rows[i].FindControl("chkActive"));
            if (chkBox.Checked)
            {
               // current sensor is active - bit is on
               TemperatureSensorMask |= (byte)(bit << i);
               // lower temp. limit
               lower = (this.gvReeferTempSensors.Rows[i].FindControl("txtReeferLower") as TextBox).Text;
               // upper temp. limit
               upper = (this.gvReeferTempSensors.Rows[i].FindControl("txtReeferUpper") as TextBox).Text;
               if (!IsRangeValid(lower, upper, -55, 125))
               {
                  this.ShowMessage(lblMessage, "Send Command Failed: " +
                     this.gvReeferTempSensors.Rows[i].Cells[1].Text + " : Invalid Range", Color.Red);
                  this.gvReeferTempSensors.Rows[i].Cells[3].Focus();
                  return "";
               }

               paramList.Append(Util.MakePair(Const.keyReeferLowerThresholdOfTempZone + (i + 1).ToString(), lower));

               paramList.Append(Util.MakePair(Const.keyReeferUpperThresholdOfTempZone + (i + 1).ToString(), upper));

               paramList.Append(Util.MakePair(Const.keyReeferTempZoneTime + (i + 1).ToString(),
                  (this.gvReeferTempSensors.Rows[i].FindControl("ddlTempInterval") as DropDownList).SelectedValue));
            }
         }

         // temp. sensor mask
         paramList.Append(VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyReeferTempZoneEnableMask,
            String.Format("{0:X2}", TemperatureSensorMask)));

         // temp. checking interval mask, now has the same value as the temp. sensor mask
         // because active temp. zone must have a checking interval defined
         paramList.Append(VLF.CLS.Util.MakePair(VLF.CLS.Def.Const.keyReeferTempZoneTimeMask,
            String.Format("{0:X2}", TemperatureSensorMask)));

         # endregion

         # region Fuel threshold

         string fuelMaskValue = "", fuelLower = "", fuelUpper = "";
         // fuel. sensor mask
         CheckBox chkBoxFuel = (CheckBox)(gvFuelSensors.Rows[0].FindControl("chkFuelActive"));
         fuelLower = (this.gvFuelSensors.Rows[0].FindControl("txtFuelLower") as TextBox).Text;
         fuelUpper = (this.gvFuelSensors.Rows[0].FindControl("txtFuelUpper") as TextBox).Text;

         if (chkBoxFuel.Checked)
         {
            fuelMaskValue = "01";
            if (!IsRangeValid(fuelLower, fuelUpper, 0, 100))
            {
               this.ShowMessage(lblMessage, "Send Command Failed: Invalid Fuel Range (0 - 100%)", Color.Red);
               this.gvFuelSensors.Rows[0].Cells[3].Focus();
               return "";
            }

            paramList.Append(Util.MakePair(Const.keyReeferLowerThresholdOfFuel, fuelLower));
            paramList.Append(Util.MakePair(Const.keyReeferUpperThresholdOfFuel, fuelUpper));
         }
         else
            fuelMaskValue = "00";

         paramList.Append(Util.MakePair(VLF.CLS.Def.Const.keyReeferFuelEnableMask, fuelMaskValue));

         # endregion

         # region Fuel Level

         if (this.chkFuelLevelActive.Checked)
         {
            fuelMaskValue = "01";
            if (!base.IsInRange(this.txtFuelLevel.Text, 0, 100))
            {
               this.ShowMessage(lblMessage, "Send Command Failed: Invalid Fuel Level (0 - 100%)", Color.Red);
               this.txtFuelLevel.Focus();
               return "";
            }

            paramList.Append(Util.MakePair(Const.keyReeferFuelLevelChange, this.txtFuelLevel.Text));

            if (Convert.ToInt16(this.ddlFuelInterval.SelectedValue) == 0)
            {
               this.ShowMessage(lblMessage, "Send Command Failed: Please Set Fuel Level Checking Interval", Color.Red);
               return "";
            }
            paramList.Append(Util.MakePair(Const.keyReeferFuelLevelDurationOfChange, this.ddlFuelInterval.SelectedValue));
         }
         else
            fuelMaskValue = "00";

         paramList.Append(Util.MakePair(Const.keyReeferFuelLevelRiseDropMask, fuelMaskValue));
         # endregion

         # region Status sensors

         byte[] StatusSensorMask = new byte[2];

         for (int i = 0; i < this.gvStatusSensors.Rows.Count; i++)
         {
            if (((CheckBox)gvStatusSensors.Rows[i].FindControl("chkActive")).Checked)
               StatusSensorMask[0] |= (byte)(bit << GetStatusSensorBitIndex(i));
         }

         paramList.Append(Util.MakePair(VLF.CLS.Def.Const.keyReeferSensorsEnableMask,
            String.Format("{0:X2} {1:X2}", StatusSensorMask[0], StatusSensorMask[1])));

         # endregion

         # region Reporting interval

         paramList.Append(Util.MakePair(Const.keyReeferReportingInterval, this.ddlReeferInterval.SelectedValue));
         # endregion

         # region Include Position - Feature mask
         paramList.Append(Util.MakePair(Const.keyReeferFeatureMask, this.chkReeferIncludePosition.Checked ? "01 00" : "00 00"));
         # endregion

         return paramList.ToString();
      }

      /// <summary>
      /// Get bit index for reefer status sensors
      /// </summary>
      /// <param name="gridRowIndex">Status Sensor Grid View Row Index</param>
      /// <returns>Status Sensor Bit Index</returns>
      private byte GetStatusSensorBitIndex(int gridRowIndex)
      {
         byte bitIndex = 0;
         switch (gridRowIndex)
         {
            case 0: // Fault Sensor
               bitIndex = 6;
               break;
            case 1: // Out of Range
               bitIndex = 5;
               break;
            case 2: // Cool
               bitIndex = 2;
               break;
            case 3: // Defrost
               bitIndex = 3;
               break;
            case 4: // Heat
               bitIndex = 4;
               break;
            case 5: // Auto Start
               bitIndex = 1;
               break;
            case 6: // Standby Power
               bitIndex = 0;
               break;
            default:
               throw new ArgumentOutOfRangeException("Invalid row index prameter: " + gridRowIndex.ToString());
               break;
         }
         return bitIndex;
      }

      private void SendBoxCommand(short commandType, string paramList)
      {
         bool cmdSent = false;
         Int64 sessionTimeOut = 0;
         string errMsg = "";
         this.lblMessage.Text = "";
         LocationMgr.Location dbl = new LocationMgr.Location();

         // Check Dual mode
         DataSet ds = new DataSet();
         string xml = "";
         
         ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
         if (objUtil.ErrCheck(dbv.GetCommandProtocolTypesInfo(sn.UserID, sn.SecId, sn.Cmd.BoxId, commandType, ref xml), false))
            if (objUtil.ErrCheck(dbv.GetCommandProtocolTypesInfo(sn.UserID, sn.SecId, sn.Cmd.BoxId, commandType, ref xml), true))
            {
               //EnableDualComm(false);
               sn.Cmd.ProtocolTypeId = -1;
               sn.Cmd.CommModeId = -1;
               return;
            }

         if (String.IsNullOrEmpty(xml))
         {
            sn.Cmd.ProtocolTypeId = -1;
            sn.Cmd.CommModeId = -1;
            return;
         }

         ds.ReadXml(new StringReader(xml));

         if ((ds != null) && (ds.Tables.Count > 0) && (ds.Tables[0].Rows.Count == 1))
         {
            sn.Cmd.ProtocolTypeId = Convert.ToInt16(ds.Tables[0].Rows[0]["BoxProtocolTypeId"]);
            sn.Cmd.CommModeId = Convert.ToInt16(ds.Tables[0].Rows[0]["CommModeId"]);
         }
         else
         {
            sn.Cmd.ProtocolTypeId = -1;
            sn.Cmd.CommModeId = -1;

         }

         short CommModeId = sn.Cmd.CommModeId;
         short ProtocolId = sn.Cmd.ProtocolTypeId;
         
         if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, sn.Cmd.BoxId, commandType, paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), false))
            if (errMsg == "")
            {
               if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, sn.Cmd.BoxId, commandType, paramList, ref ProtocolId, ref CommModeId, ref cmdSent, ref sessionTimeOut), true))
               {
                  this.lblMessage.Visible = true;

                  if (errMsg != "")
                     this.lblMessage.Text = errMsg;
                  else
                     this.lblMessage.Text = "Send Command Failed Using " + (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId;
                  //(string)base.GetLocalResourceObject("lblMessage_Text_SendCommandFailedUsing") + " " + (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId;

                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Send command failed.  User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                  return;
               }
            }
            else
            {
               this.lblMessage.Visible = true;
               this.lblMessage.Text = errMsg;
               return;
            }

         sn.Cmd.CommandParams = paramList;
         sn.Cmd.ProtocolTypeId = ProtocolId;
         sn.Cmd.CommModeId = CommModeId;
         sn.Cmd.CommandId = commandType;

         int cmdStatus = 0;
         if (!cmdSent) return;
         sn.Cmd.Status = CommandStatus.Sent;
         cmdStatus = (int)CommandStatus.Sent;

         this.lblMessage.Text = "";

         sn.Map.TimerStatus = true;

         if (sessionTimeOut > 0)
            sn.Cmd.GetCommandStatusRefreshFreq = Convert.ToInt64(Math.Round(sessionTimeOut / 10.0)) * 1000;
         else
            sn.Cmd.GetCommandStatusRefreshFreq = 1000;

         # region Get command status
         //Session["ReeferCommandStatus"] = CommandStatus.Sent;
         AutoResetEvent getCmdComplete = new AutoResetEvent(false);
         Thread tdGetCmdStatus = new Thread(new ParameterizedThreadStart(GetCommandStatusFromServer));
         tdGetCmdStatus.Start(getCmdComplete);
         //string script = "window.showModalDialog('frmProgress.aspx', '', 'dialogHeight: 200px; dialogWidth: 400px; edge: raised; center: yes; help: no; resizable: yes; status: no; scroll: no;');";
         //this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "ProgressDialog", script, true);

         getCmdComplete.WaitOne(60000, false);
         //if (
         //((Convert.ToInt32(this.cboCommand.SelectedValue) == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.BoxReset)) && (optCommMode.Items[1].Selected))
         //|| ((Convert.ToInt32(this.cboCommand.SelectedValue) == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.BoxHardReset)) && (optCommMode.Items[1].Selected))
         //((commandType == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.BoxReset)) /*&& (optCommMode.Items[1].Selected)*/) ||
         //((commandType == Convert.ToInt32(VLF.CLS.Def.Enums.CommandType.BoxHardReset))/* && (optCommMode.Items[1].Selected)*/)
         //)

         //do
         //{
            //System.Threading.Thread.Sleep(new TimeSpan(sn.Cmd.GetCommandStatusRefreshFreq));
            //long index = 0;
            //while (index < 200000000)
            //{
            //   index++;
            //}
            //this.Page.ClientScript.RegisterClientScriptBlock(this.Page.GetType(), "ProgressDialog", script, true);
            //Get Command Status for GPRS
            //if (objUtil.ErrCheck(dbl.GetCommandStatus(sn.UserID, sn.SecId, Convert.ToInt32(sn.Cmd.BoxId), sn.Cmd.ProtocolTypeId, ref cmdStatus), false))
            //   if (objUtil.ErrCheck(dbl.GetCommandStatus(sn.UserID, sn.SecId, Convert.ToInt32(sn.Cmd.BoxId), sn.Cmd.ProtocolTypeId, ref cmdStatus), true))
            //   {
            //      //this.lblMessage.Text = "Send Command Failed Using " + (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId;
            //      //(string)base.GetLocalResourceObject("lblMessage_Text_SendCommandFailedUsing") + " " + (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId;
            //      System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Sent command failed.  User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            //      break;
            //   }
            /*
            short AlternativeProtocol = Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[1]["BoxProtocolTypeId"]);
            sn.Cmd.ProtocolTypeId = AlternativeProtocol;
            CommModeId = Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[1]["CommModeId"]);
            sn.Cmd.CommModeId = CommModeId;
            //

            sessionTimeOut = 0;
            if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, sn.Cmd.BoxId, sn.Cmd.CommandId, sn.Cmd.CommandParams, ref AlternativeProtocol, ref CommModeId, ref cmdSent, ref sessionTimeOut), false))
               if (objUtil.ErrCheck(dbl.SendCommand(sn.UserID, sn.SecId, DateTime.Now, sn.Cmd.BoxId, sn.Cmd.CommandId, sn.Cmd.CommandParams, ref AlternativeProtocol, ref CommModeId, ref cmdSent, ref sessionTimeOut), true))
               {
                  this.lblMessage.Visible = true;
                  this.lblMessage.Text = "Send Command Failed Using " + (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId;
                     //(string)base.GetLocalResourceObject("lblMessage_Text_SendCommandFailedUsing") + " " + (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId;
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Sent command failed.  User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                  return;
               }

            //Get Command Status for Alternative Protocol
            if (objUtil.ErrCheck(dbl.GetCommandStatus(sn.UserID, sn.SecId, Convert.ToInt32(sn.Cmd.BoxId), sn.Cmd.ProtocolTypeId, ref cmdStatus), false))
               if (objUtil.ErrCheck(dbl.GetCommandStatus(sn.UserID, sn.SecId, Convert.ToInt32(sn.Cmd.BoxId), sn.Cmd.ProtocolTypeId, ref cmdStatus), true))
               {
                  this.lblMessage.Visible = true;
                  this.lblMessage.Text = "Send Command Failed Using " + (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId;
                     //(string)base.GetLocalResourceObject("lblMessage_Text_SendCommandFailedUsing") + " " + (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId;
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Sent command failed.  User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                  return;
               }
            
            sn.Cmd.Status = (CommandStatus)cmdStatus;

            if ((cmdStatus == (int)CommandStatus.Ack) || (cmdStatus == (int)CommandStatus.Pending))
               //Msg += " " + (string)base.GetLocalResourceObject("Msg_And") + " " + (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId + " " + (string)base.GetLocalResourceObject("Msg_Success");
               Msg += " And " + (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId + " Success";
            else
               //Msg += " " + (string)base.GetLocalResourceObject("Msg_And") + " " + (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId + " " + (string)base.GetLocalResourceObject("Msg_Failed");
               Msg += " And " + (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId + " Failed";
            */
         //} while ((CommandStatus)cmdStatus == CommandStatus.Sent);
         //(CommandStatus)cmdStatus == CommandStatus.CommTimeout &&
         //(CommandStatus)cmdStatus != CommandStatus.Ack &&
         //(CommandStatus)cmdStatus != CommandStatus.Pending);

         //sn.Cmd.Status = cmdStatus;
            //(CommandStatus)Session["ReeferCommandStatus"];
         # endregion

         string Msg = "Sending Command Using ";
         Color clr;
         switch (sn.Cmd.Status)
         {
            case CommandStatus.Ack:
               Msg += String.Format(" {0} Successful", (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId);
               GetReeferSetup();
               clr = Color.Green;
               break;
            case CommandStatus.CommTimeout:
               Msg += String.Format(" {0} Failed: Timeout", (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId);
               clr = Color.Maroon;
               break;
            default:
               Msg += String.Format(" {0} Failed", (VLF.CLS.Def.Enums.CommMode)sn.Cmd.CommModeId);
               clr = Color.Red;
               break;
         }

         this.ShowMessage(lblMessage, Msg, clr);
      }

      private void GetReeferSetup()
      {
         // send command to get reefer setup
         try
         {
            DataSet dsSetup = new DataSet();

            string xml = "";
            ServerDBVehicle.DBVehicle dbv = new ServerDBVehicle.DBVehicle();
            
            if (objUtil.ErrCheck(dbv.GetLastBoxMessageFromHistoryByLicensePlate(sn.UserID, sn.SecId, this.LicensePlate, (int)Enums.MessageType.SendReeferSetup, ref xml), false))
               if (objUtil.ErrCheck(dbv.GetLastBoxMessageFromHistoryByLicensePlate(sn.UserID, sn.SecId, this.LicensePlate, (int)Enums.MessageType.SendReeferSetup, ref xml), true))
               {
                  return;
               }

            if (xml == "")
            {
               return;
            }

            dsSetup.ReadXml(new StringReader(xml));
            string CustomProp = dsSetup.Tables[0].Rows[0]["CustomProp"].ToString();

            #region Reporting interval

            string reportInterval = Util.PairFindValue(Const.keyReeferReportingInterval, CustomProp);
            int intervalSeconds = 0;
            if (Int32.TryParse(reportInterval, out intervalSeconds))
            {
               //if (Convert.ToInt32(reportInterval) == 0)
               //   this.txtReeferInterval.Text = "";
               //else
               //   this.txtReeferInterval.Text = ((int)(intervalSeconds / 60)).ToString();
               this.ddlReeferInterval.SelectedValue = reportInterval;
            }
            //else
            //{
            //   this.txtReeferInterval.Text = "";
            //}
            #endregion

            # region Include Position - Feature mask
            string featureMask = Util.PairFindValue(Const.keyReeferFeatureMask, CustomProp);
            this.chkReeferIncludePosition.Checked = featureMask == "01 00" ? true : false;
            # endregion

            # region Temperature zones

            string tempZoneSensorMask = Util.PairFindValue(Const.keyReeferTempZoneEnableMask, CustomProp);
            if (!String.IsNullOrEmpty(tempZoneSensorMask))
            {
               byte TemperatureSensorMask = Convert.ToByte(tempZoneSensorMask, 16);
               for (byte i = 0; i < this.gvReeferTempSensors.Rows.Count; i++)
               {
                  CheckBox chkBox = (CheckBox)(gvReeferTempSensors.Rows[i].FindControl("chkActive"));
                  // extract sensor's bit
                  chkBox.Checked = Util.GetBit(TemperatureSensorMask, i) == 1 ? true : false;
                  if (chkBox.Checked)
                  {
                     // get current temperature
                     Label txtBoxCurrent = (Label)(gvReeferTempSensors.Rows[i].FindControl("txtReeferCurrent"));
                     txtBoxCurrent.Text = Util.PairFindValue(Const.keyReeferTempOfZone + (i + 1).ToString(), CustomProp);

                     // get lower temperature
                     TextBox txtBoxLower = (TextBox)(gvReeferTempSensors.Rows[i].FindControl("txtReeferLower"));
                     txtBoxLower.Enabled = true;
                     txtBoxLower.Text = Util.PairFindValue(Const.keyReeferLowerThresholdOfTempZone + (i + 1).ToString(), CustomProp);

                     // get upper temperature
                     TextBox txtBoxUpper = (TextBox)(gvReeferTempSensors.Rows[i].FindControl("txtReeferUpper"));
                     txtBoxUpper.Enabled = true;
                     txtBoxUpper.Text = Util.PairFindValue(Const.keyReeferUpperThresholdOfTempZone + (i + 1).ToString(), CustomProp);

                     // get temp. zone checking interval
                     DropDownList ddlInterval = (DropDownList)(gvReeferTempSensors.Rows[i].FindControl("ddlTempInterval"));
                     ddlInterval.Enabled = true;
                     string chkIntl = Util.PairFindValue(Const.keyReeferTempZoneTime + (i + 1).ToString(), CustomProp);
                     if (ddlInterval.Items.FindByValue(chkIntl) != null)
                        ddlInterval.SelectedValue = chkIntl;
                     else
                        ddlInterval.SelectedValue = "0";
                  }
               }
            }

            # endregion

            # region Fuel thresholds

            string fuelSensorMask = Util.PairFindValue(Const.keyReeferFuelEnableMask, CustomProp);

            if (!String.IsNullOrEmpty(fuelSensorMask))
            {
               CheckBox chkFuel = (CheckBox)(gvFuelSensors.Rows[0].FindControl("chkFuelActive"));
               Label FuelCurrent = (Label)(gvFuelSensors.Rows[0].FindControl("txtFuelCurrent"));
               TextBox FuelLower = (TextBox)(gvFuelSensors.Rows[0].FindControl("txtFuelLower"));
               TextBox FuelUpper = (TextBox)(gvFuelSensors.Rows[0].FindControl("txtFuelUpper"));
               if (fuelSensorMask == "01") // enabled
               {
                  //this.chkFuelActive.Checked = true;
                  chkFuel.Checked = true;
                  FuelCurrent.Text = Util.PairFindValue(Const.keyReeferFuelStatus, CustomProp);
                  FuelLower.Enabled = true;
                  FuelLower.Text = Util.PairFindValue(Const.keyReeferLowerThresholdOfFuel, CustomProp);
                  FuelUpper.Enabled = true;
                  FuelUpper.Text = Util.PairFindValue(Const.keyReeferUpperThresholdOfFuel, CustomProp);
               }
               else
               {
                  FuelCurrent.Text = "";
                  FuelLower.Enabled = false;
                  FuelLower.Text = "";
                  FuelUpper.Enabled = false;
                  FuelUpper.Text = "";
                  //this.chkFuelActive.Checked = false;
                  chkFuel.Checked = false;
               }
            }

            # endregion

            #region Fuel Level

            fuelSensorMask = Util.PairFindValue(Const.keyReeferFuelLevelRiseDropMask, CustomProp);
            if (!String.IsNullOrEmpty(fuelSensorMask))
            {
               if (fuelSensorMask == "01") // enabled
               {
                  this.chkFuelLevelActive.Checked = true;
                  this.txtFuelLevel.Enabled = true;
                  this.txtFuelLevel.Text = Util.PairFindValue(Const.keyReeferFuelLevelChange, CustomProp);
                  this.ddlFuelInterval.Enabled = true;
                  string chkIntl = Util.PairFindValue(Const.keyReeferFuelLevelDurationOfChange, CustomProp);
                  if (ddlFuelInterval.Items.FindByValue(chkIntl) != null)
                     ddlFuelInterval.SelectedValue = chkIntl;
                  else
                     ddlFuelInterval.SelectedValue = "0";
               }
               else
               {
                  this.chkFuelLevelActive.Checked = false;
                  this.txtFuelLevel.Text = "";
                  this.txtFuelLevel.Enabled = false;
                  ddlFuelInterval.SelectedValue = "0";
                  this.ddlFuelInterval.Enabled = false;
               }
            }

            #endregion

            # region Status sensors

            string statusSensorMask = Util.PairFindValue(Const.keyReeferSensorsEnableMask, CustomProp);

            if (!String.IsNullOrEmpty(statusSensorMask))
            {
               byte StatusSensorMask = Convert.ToByte(statusSensorMask.Split(' ')[0], 16);
               for (byte i = 0; i < this.gvStatusSensors.Rows.Count; i++)
               {
                  CheckBox chkBox = (CheckBox)(this.gvStatusSensors.Rows[i].FindControl("chkActive"));
                  byte bitIndex = GetStatusSensorBitIndex(i);
                  chkBox.Checked = Util.GetBit(StatusSensorMask, i) == 1 ? true : false;
               }
            }

            # endregion

            this.lblFWVersionValue.Text = Util.PairFindValue(Const.keyReeferFirmwareVersion, CustomProp);
            this.lblMainBatteryValue.Text = Util.PairFindValue(Const.keyReeferMainBatteryVoltage, CustomProp);
            if (!String.IsNullOrEmpty(this.lblMainBatteryValue.Text))
               this.lblMainBatteryValue.Text += "mV";
            else
               this.lblMainBatteryValue.Text += " N/A";
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
         }
      }

      protected void cmdSensorCommands_Click(object sender, EventArgs e)
      {
         Response.Redirect("frmSensorsInfo.aspx?LicensePlate=" + this.LicensePlate);
      }

      protected void cmdVehicleInfo_Click(object sender, EventArgs e)
      {
         Response.Redirect("frmVehicleDescription.aspx?LicensePlate=" + this.LicensePlate);
      }

      private void GetCommandStatusFromServer(Object data)
      {
         LocationMgr.Location dbl = new LocationMgr.Location();
         int cmdStatus = (int)CommandStatus.Sent;
         do
         {
            Thread.Sleep(2000);
            //Get Command Status for GPRS
            
            if (objUtil.ErrCheck(dbl.GetCommandStatus(sn.UserID, sn.SecId, Convert.ToInt32(sn.Cmd.BoxId), sn.Cmd.ProtocolTypeId, ref cmdStatus), false))
               if (objUtil.ErrCheck(dbl.GetCommandStatus(sn.UserID, sn.SecId, Convert.ToInt32(sn.Cmd.BoxId), sn.Cmd.ProtocolTypeId, ref cmdStatus), true))
               {
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Sent command failed.  User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                  break;
               }
         } while ((CommandStatus)cmdStatus == CommandStatus.Sent);
         sn.Cmd.Status = (CommandStatus)cmdStatus;
         (data as AutoResetEvent).Set();
      }

      protected void btnReset_Click(object sender, EventArgs e)
      {
         try
         {
            // send command to a box
            SendBoxCommand((short)Enums.CommandType.ReeferReset, "");
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
               VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            lblMessage.Text = Ex.Message;
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
               VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() +
               " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            lblMessage.Text = Ex.Message;
         }
      }
   }
}