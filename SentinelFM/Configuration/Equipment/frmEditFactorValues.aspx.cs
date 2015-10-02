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
using System.Text;
using VLF.CLS;
using Telerik.Web.UI;
using System.Drawing;
using VLF.DAS.Logic;
using System.Collections.Generic;
using VLF.DAS.DB;
namespace SentinelFM
{
    public partial class Configuration_Equipment_frmEditFactorValues : SentinelFMBasePage
    {
        public string overwriteStr1 = "This action will overwrite the values for (1) box, continue?";
        public string overwriteStr2 = "This action will overwrite the values for (n) boxes, continue?";
        public string assignNewStr1 = "This action will assign the values for (1) box, continue?";
        public string assignNewStr2 = "This action will assign the values for (n) boxes, continue?";

        public string selectFleet = "Select a Fleet";
        public string selectVehicleVal = "Please Select a Vehicle";
        public string entireFleet = "Entire Fleet";
        //string factorValues = "Factor Values";
        string errorSave = "Save Failed.";
        string succeedsave = "Saved Successfully.";
        public string updateTo = "Update To";
        string updateText = "Update";
        string assignText = "Assign";
        public string assignTo = "Assign New";
        int entireValue = -1;
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        string vehicleId = "-1";
        string mediaId = "-1";
        string assignmentId = "-1";
        int MediaFactorDecimalDigits = 3;
        string equipmentMediaAssigmentId = "-1";
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../../Login.aspx','_top')</SCRIPT>", false);
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
            if (Request.QueryString["v"] != null && Request.QueryString["m"] != null &&
                Request.QueryString["a"] != null && Request.QueryString["em"] != null )
            {
                vehicleId = Request.QueryString["v"].ToString().Trim();
                mediaId = Request.QueryString["m"].ToString().Trim();
                assignmentId = Request.QueryString["a"].ToString().Trim();
                equipmentMediaAssigmentId = Request.QueryString["em"].ToString().Trim();
                if (Request.QueryString["b"] != null) lblBoxIDName.Text = Request.QueryString["b"].ToString();
                
            }
            if (!IsPostBack)
            {
                legendID.InnerText = updateTo;
                //legendmedia.InnerText = factorValues;
                if (Request.QueryString["v"] != null && Request.QueryString["m"] != null &&
                    Request.QueryString["a"] != null && Request.QueryString["em"] != null)

                {
                   //FillComboBox();
                   FillValueControls();
                }
            }
            MediaFactorDecimalDigits = int.Parse(System.Configuration.ConfigurationManager.AppSettings["MediaFactorDecimalDigits"].ToString());
            txtFactor1.NumberFormat.DecimalDigits = MediaFactorDecimalDigits;
            txtFactor2.NumberFormat.DecimalDigits = MediaFactorDecimalDigits;
            txtFactor3.NumberFormat.DecimalDigits = MediaFactorDecimalDigits;
            txtFactor4.NumberFormat.DecimalDigits = MediaFactorDecimalDigits;
            txtFactor5.NumberFormat.DecimalDigits = MediaFactorDecimalDigits;

        }

        private void FillValueControls()
        {
            MediaManager mediaMg = new MediaManager(sConnectionString);
            VehicleEquipmentAssignmentManager vehicleMg = new VehicleEquipmentAssignmentManager(sConnectionString);
            DataSet ds = vehicleMg.GetVehicleEquipmentFactorsById(int.Parse(assignmentId), sn.UserID);
            if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
            {
                if (!(ds.Tables[0].Rows[0]["UnitOfMeasureAcr"] is DBNull))
                {
                    lblUnit1.Text = ds.Tables[0].Rows[0]["UnitOfMeasureAcr"].ToString();
                    lblUnit2.Text = ds.Tables[0].Rows[0]["UnitOfMeasureAcr"].ToString();
                    lblUnit3.Text = ds.Tables[0].Rows[0]["UnitOfMeasureAcr"].ToString();
                    lblUnit4.Text = ds.Tables[0].Rows[0]["UnitOfMeasureAcr"].ToString();
                    lblUnit5.Text = ds.Tables[0].Rows[0]["UnitOfMeasureAcr"].ToString();
                }

                if (!(ds.Tables[0].Rows[0]["UnitOfMeasureId"] is DBNull))
                {
                    ViewState["UnitOfMeasureId"] = int.Parse(ds.Tables[0].Rows[0]["UnitOfMeasureId"].ToString());
                }

                if (!(ds.Tables[0].Rows[0]["VehicleDescription"] is DBNull))
                {
                    lblVehicleNameText.Text = ds.Tables[0].Rows[0]["VehicleDescription"].ToString();
                }

                if (!(ds.Tables[0].Rows[0]["MediaDescription"] is DBNull))
                {
                    lblMediaName.Text = ds.Tables[0].Rows[0]["MediaDescription"].ToString();
                }
                if (!(ds.Tables[0].Rows[0]["EquipmentDescription"] is DBNull))
                {
                    lblEquipmentName.Text = ds.Tables[0].Rows[0]["EquipmentDescription"].ToString();
                }

                SetControlsValues(lblFactorName1, txtFactor1, lblFactor1,
                       ds.Tables[0].Rows[0]["FactorName1"],
                       ds.Tables[0].Rows[0]["Factor1"], lblUnit1
                     );
                SetControlsValues(lblFactorName2, txtFactor2, lblFactor2,
                       ds.Tables[0].Rows[0]["FactorName2"],
                       ds.Tables[0].Rows[0]["Factor2"], lblUnit2
                     );

                SetControlsValues(lblFactorName3, txtFactor3, lblFactor3,
                       ds.Tables[0].Rows[0]["FactorName3"],
                       ds.Tables[0].Rows[0]["Factor3"], lblUnit3
                     );

                SetControlsValues(lblFactorName4, txtFactor4, lblFactor4,
                       ds.Tables[0].Rows[0]["FactorName4"],
                       ds.Tables[0].Rows[0]["Factor4"], lblUnit4
                     );

                SetControlsValues(lblFactorName5, txtFactor5, lblFactor5,
                       ds.Tables[0].Rows[0]["FactorName5"],
                       ds.Tables[0].Rows[0]["Factor5"], lblUnit5
                     );
            }
        }

        private void SetControlsValues(Label lblFactorName, RadNumericTextBox txtFactor, TextBox lblFactor, object factorName, object factor, Label lblUnit)
        {
            if (factorName is DBNull) return;
            if (string.IsNullOrEmpty(factorName.ToString())) return;
            lblFactorName.Text = factorName.ToString() +":";
            lblFactorName.Visible = true;
            lblUnit.Visible = true;
            if (!(factor is DBNull) && !string.IsNullOrEmpty(factor.ToString()))
            {
                lblFactor.Text = Math.Round(double.Parse(factor.ToString()), MediaFactorDecimalDigits).ToString(); 
                txtFactor.Value = Math.Round(double.Parse(factor.ToString()), MediaFactorDecimalDigits); 
            }
            else
            {
                txtFactor.Value = null;
            }
            lblFactor.Visible = true;
            txtFactor.Visible = true;
        }

        private void FillComboBox()
        {
            DataSet dsFleets = null;
            VehicleEquipmentAssignmentManager vehicleEquipmentAssignmentManager = new VehicleEquipmentAssignmentManager(sConnectionString);
            if (!IsPostBack || (IsPostBack && legendID.InnerText == updateTo))
            {
                dsFleets = vehicleEquipmentAssignmentManager.GetFleetByEquipmentMediaAssigmentId(sn.UserID, int.Parse(equipmentMediaAssigmentId));
            }
            else
            {
                dsFleets = vehicleEquipmentAssignmentManager.GetFleetWithNoEquipmentAssignmentById(sn.UserID, int.Parse(equipmentMediaAssigmentId));
            }

            if (dsFleets != null && dsFleets.Tables[0].Rows.Count > 0)
            {
                if (IsPostBack)
                {
                    cboFleet.DataSource = dsFleets;
                    cboFleet.DataBind();
                    cboFleet.Items.Insert(0, new RadComboBoxItem(selectFleet, "-1"));

                    Boolean hasPassValue = false;
                    if (Request.QueryString["f"] != null)
                    {
                        try
                        {
                            string qysFleetID = Request.QueryString["f"];
                            cboFleet.SelectedIndex = cboFleet.Items.IndexOf(cboFleet.Items.FindItemByValue(qysFleetID));
                            CboVehicle_Fill(Convert.ToInt32(qysFleetID));
                            hasPassValue = true;
                        }
                        catch { }
                    }
                    if (!hasPassValue)
                    {
                        cboFleet.SelectedIndex = 1;
                        CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedValue.ToString()));
                    }
                }
            }
            else
            {
                if (!IsPostBack) btnCopy.Enabled = false;
            }
        }

        private void CboVehicle_Fill(int fleetId)
        {
            try
            {
                cboVehicle.Items.Clear();

                DataSet dsVehicle = new DataSet();
                VehicleEquipmentAssignmentManager vehicleEquipmentAssignmentManager = new VehicleEquipmentAssignmentManager(sConnectionString);
                if (!IsPostBack || (IsPostBack && legendID.InnerText == updateTo))
                {
                    dsVehicle = vehicleEquipmentAssignmentManager.GetVehiclesByFleetIdandEquipmentMediaAssigmentId(fleetId, int.Parse(equipmentMediaAssigmentId));
                }
                else
                {
                    dsVehicle = vehicleEquipmentAssignmentManager.GetVehiclesWithNoEquipmentAssignmentById(fleetId, int.Parse(equipmentMediaAssigmentId));
                }
                DataRow dr = dsVehicle.Tables[0].NewRow();
                dr["Description"] = entireFleet;
                dr["VehicleID"] = entireValue;
                dsVehicle.Tables[0].Rows.InsertAt(dr, 0);
                cboVehicle.DataSource = dsVehicle;
                cboVehicle.DataBind();

                //RadComboBoxItem item = cboVehicle.FindItemByValue(vehicleId);
                //if (item != null)
                //{
                //    cboVehicle.Items.Remove(item);
                //}
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
        protected void cboFleet_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            if (cboFleet.SelectedIndex > 0) CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedValue.ToString()));
            RadAjaxManager1.ResponseScripts.Add("SetCheckBox()");
        }
        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (vehicleId != string.Empty && equipmentMediaAssigmentId != string.Empty)
            {
                try
                {
                    int unitOfMeasureId = -100;
                    if (ViewState["UnitOfMeasureId"] == null) { throw new Exception("ViewState does not exist."); }
                    unitOfMeasureId = (int)ViewState["UnitOfMeasureId"];

                    VehicleEquipmentAssignmentManager vehicleEquipmentAssignmentManager = new VehicleEquipmentAssignmentManager(sConnectionString);
                    vehicleEquipmentAssignmentManager.VehicleEquipmentAssignment_UpdateFactors(vehicleId,
                        txtFactor1.Value,
                        txtFactor2.Value,
                        txtFactor3.Value,
                        txtFactor4.Value,
                        txtFactor5.Value,
                        int.Parse(equipmentMediaAssigmentId),
                        unitOfMeasureId, sn.UserID);
                    string msg = string.Format("SetCheckBox();alert(\"{0}\"); ", succeedsave);
                    hidCopy.Value = "0";
                    RadAjaxManager1.ResponseScripts.Add(msg);
                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                    ExceptionLogger(trace);
                    string errorMsg = string.Format("alert(\"{0}\");", errorSave);
                    RadAjaxManager1.ResponseScripts.Add(errorMsg);
                }
            }
        }

        private string GetunSavedVehicleNames(string unSavedVehicleIds)
        {
            unSavedVehicleIds = "," + unSavedVehicleIds + ",";
            StringBuilder sb = new StringBuilder();
            foreach (RadComboBoxItem item in cboVehicle.Items)
            {
                if (unSavedVehicleIds.Contains("," + item.Value + ","))
                {
                    if (sb.Length == 0) sb.Append(item.Text);
                    else sb.Append("," + item.Text);
                }
            }
            return sb.ToString();
        }

        private string GetCheckedVehicle()
        {
            StringBuilder vehicleIDs = new StringBuilder ();
            foreach (RadComboBoxItem item in cboVehicle.Items)
            {
                CheckBox chkVehicle = (CheckBox)item.FindControl("chkVehicle");
                if (chkVehicle != null && chkVehicle.Checked)
                {
                    if (item.Value == entireValue.ToString()) continue;
                    if (vehicleIDs.Length == 0) vehicleIDs.Append(item.Value);
                    else vehicleIDs.Append("," + item.Value);
                }
            }
            return vehicleIDs.ToString();
        }
        protected void btnSaveCopy_Click(object sender, EventArgs e)
        {
            string vehicleIDs = GetCheckedVehicle();
            if (vehicleIDs != string.Empty && equipmentMediaAssigmentId != string.Empty)
            {
                if (legendID.InnerText == updateTo)
                {
                    try
                    {
                        int unitOfMeasureId = -100;
                        if (ViewState["UnitOfMeasureId"] == null) { throw new Exception("ViewState does not exist."); }
                        unitOfMeasureId = (int)ViewState["UnitOfMeasureId"];

                        VehicleEquipmentAssignmentManager vehicleEquipmentAssignmentManager = new VehicleEquipmentAssignmentManager(sConnectionString);
                        vehicleEquipmentAssignmentManager.VehicleEquipmentAssignment_UpdateFactors(vehicleIDs,
                            txtFactor1.Value,
                            txtFactor2.Value,
                            txtFactor3.Value,
                            txtFactor4.Value,
                            txtFactor5.Value,
                            int.Parse(equipmentMediaAssigmentId),
                            unitOfMeasureId, sn.UserID);
                        string msg = string.Format("ResetCheckValue();SetCheckBox();alert(\"{0}\"); ", succeedsave);
                        RadAjaxManager1.ResponseScripts.Add(msg);
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                        ExceptionLogger(trace);
                        string errorMsg = string.Format("alert(\"{0}\");", errorSave);
                        RadAjaxManager1.ResponseScripts.Add(errorMsg);
                    }
                }

                if (legendID.InnerText == assignTo)
                {
                    try
                    {
                        int unitOfMeasureId = -100;
                        if (ViewState["UnitOfMeasureId"] == null) { throw new Exception("ViewState does not exist."); }
                        unitOfMeasureId = (int)ViewState["UnitOfMeasureId"];
                        VehicleEquipmentAssignmentManager vehicleEquipmentAssignmentManager = new VehicleEquipmentAssignmentManager(sConnectionString);
                        string unSavedVehicleIDs =
                            vehicleEquipmentAssignmentManager.VehicleEquipmentAssignment_AssignNew(vehicleIDs, int.Parse(equipmentMediaAssigmentId),
                            txtFactor1.Value,
                            txtFactor2.Value,
                            txtFactor3.Value,
                            txtFactor4.Value,
                            txtFactor5.Value,
                            unitOfMeasureId, sn.UserID
                            );

                        string msg = string.Format("SetCheckBox();alert(\"{0}\"); ", succeedsave);
                        if (!string.IsNullOrEmpty(unSavedVehicleIDs))
                        {
                            string unSavedVehicleNames = GetunSavedVehicleNames(unSavedVehicleIDs);
                            if (!string.IsNullOrEmpty(unSavedVehicleNames))
                            {
                                msg = string.Format("SetCheckBox();alert(\"{0}\"); ", unSavedVehicleNames + " " + errorSave);
                            }
                        }


                        RadAjaxManager1.ResponseScripts.Add(msg);
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                        ExceptionLogger(trace);
                        string errorMsg = string.Format("alert(\"{0}\");", errorSave);
                        RadAjaxManager1.ResponseScripts.Add(errorMsg);
                    }
                }
            }

        }

        protected void btnCopy_Click(object sender, EventArgs e)
        {
            legendID.InnerText = updateTo;
            btnSaveCopy.Text = updateText;
            hidCopy.Value = "1";
            FillComboBox();
            RadAjaxManager1.ResponseScripts.Add("SetCheckBox()");
        }
        protected void btnAssignNew_Click(object sender, EventArgs e)
        {
            legendID.InnerText = assignTo;
            btnSaveCopy.Text = assignText;
            hidCopy.Value = "1";
            FillComboBox();
            RadAjaxManager1.ResponseScripts.Add("SetCheckBox()");
        }
}
}