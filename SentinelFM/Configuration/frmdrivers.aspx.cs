using System;
using System.Data;
using System.Configuration;
using System.Collections;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
//using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using VLF.CLS;
using VLF.CLS.Def;
using System.IO;
using System.Globalization;
using System.Drawing;
using System.Data.OleDb;
//using System.Text;tblDriverDetails
using Telerik.Web.UI;
using VLF.DAS.Logic;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using VLF.DAS.DB;

namespace SentinelFM
{
    public partial class Configuration_frmdrivers : SentinelFMBasePage
    {
        ServerDBDriver.DBDriver driver = new ServerDBDriver.DBDriver();

        private string drvAssgnUrl = "";
        private string drvUrl = "";
        public string showFilter = "Show Filter";
        public string hideFilter = "Hide Filter";
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        ContactManager contactMsg = null;
        protected void Page_Load(object sender, EventArgs e)
        {
            contactMsg = new ContactManager(sConnectionString);
            try
            {
                this.lblMessage.Text = "";
                if (!Page.IsPostBack)
                {
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref form1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    GuiSecurity(this);
                    FillStates();
                    FillEmergencyPlans();
                    if (sn.User.OrganizationId == 999718)
                    {
                        //divCycle.Visible = false;
                        divCycle.Visible = sn.User.ControlEnable(sn, 110);
                    }
                }
                this.drvAssgnUrl = String.Format("~/TempReports/driver_assignments_{0}.xls", sn.UserName);
                this.drvUrl = String.Format("~/TempReports/drivers_{0}.xls", sn.UserName);
                SetDownloadLink(this.linkDriverAssignmentList, this.drvAssgnUrl);
                SetDownloadLink(this.linkDriverList, this.drvUrl);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                //lblMessage.Text = Ex.Message;
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));

            }
            try
            {
                showFilter = HttpContext.GetGlobalResourceObject("Const", "RadGrid_ShowFilter").ToString();
                hideFilter = HttpContext.GetGlobalResourceObject("Const", "RadGrid_HideFilter").ToString();
            }
            catch { };
            chkAssignEmergency.Attributes.Add("onclick", "javascript:chkAssignEmergency_click()");
        }


        private void FillEmergencyPlans()
        {
            ddlEmergencyPlan.DataSource = contactMsg.GetOrganizationContactPlan(sn.User.OrganizationId);
            ddlEmergencyPlan.DataBind();
        }

        # region Private methods

        /// <summary>
        /// Set download drivers link
        /// </summary>
        private void SetDownloadLink(HyperLink link, string url)
        {
            if (File.Exists(Server.MapPath(url)))
            {
                link.Visible = true;
                link.NavigateUrl = url;
            }
            else
                link.Visible = false;
        }

        /// <summary>
        /// Load drivers for the company
        /// </summary>
        private void LoadDrivers(bool isBind)
        {
            DataSet dsDrivers = new DataSet();
            dsDrivers = GetDrivers();
            gdvDrivers.DataSource = dsDrivers;
            if (isBind) gdvDrivers.DataBind();
            //else gdvDrivers.Rebind();
        }

        /// <summary>
        /// Get drivers for the company
        /// </summary>
        private DataSet GetDrivers()
        {

            this.tblViewDrivers.Visible = true;
            this.tblDriverDetails.Visible = false;
            this.tblUploadData.Visible = false;
            DataSet dsDrivers = new DataSet();

            try
            {
                dsDrivers = contactMsg.GetOrganizationDrivers(sn.User.OrganizationId);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                //lblMessage.Text = Ex.Message;
                RedirectToLogin();
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));

            }

            return dsDrivers;

        }

        /// <summary>
        /// Clear Add Driver Controls
        /// </summary>
        private void ClearControls()
        {
            // clear controls
            txtFirstName.Text = "";
            txtLastName.Text = "";
            txtLicense.Text = "";
            txtClass.Text = "";
            txtLicenseIssued.Text = "";
            txtLicenseExpired.Text = "";
            ddlGender.SelectedIndex = 0;
            txtHeight.Text = "";
            txtHomePhone.Text = "";
            txtCellPhone.Text = "";
            txtAdditionalPhone.Text = "";
            txtEmail.Text = "";
            txtSmsid.Text = "";
            txtAddress.Text = "";
            txtCity.Text = "";
            txtZipcode.Text = "";
            //FillStates();
            txtZipcode.Text = "";
            txtKeyFobId.Text = "";

            txtEmergencytel.Text = "";
            ddlEmergencyPlan.SelectedIndex = 0;
            chkAssignEmergency.Checked = false;

            txtTerminationDate.Text = "";
            txtPositionInfo.Text = "";
        }

        private void SetFields(DataRow row)
        {
            //Devin 2014 05,26
            try
            {
                ddlUsaCycle.SelectedIndex = 0;
                ddlCaCycle.SelectedIndex = 0;
                ddlTimeZone.SelectedIndex = 0;
                chkIsSupervisor.Checked = false;
                if (row["USCycle"] != DBNull.Value && int.Parse(row["USCycle"].ToString()) > 0)
                {
                    ddlUsaCycle.SelectedValue = row["USCycle"].ToString();
                }
                if (row["CACycle"] != DBNull.Value && int.Parse(row["CACycle"].ToString()) > 0)
                {
                    ddlCaCycle.SelectedValue = row["CACycle"].ToString();
                }
                if (row["timezone"] != DBNull.Value)
                {
                    if (float.Parse(row["timezone"].ToString()) != 0)
                        ddlTimeZone.SelectedValue = row["timezone"].ToString();
                }
                if (row["IsSupervisor"] != DBNull.Value && Boolean.Parse(row["IsSupervisor"].ToString()))
                {
                    chkIsSupervisor.Checked = true;
                }
            }
            catch (Exception ex) { }


            //FillStates();
            txtFirstName.Text = row["FirstName"].ToString().Trim();
            txtLastName.Text = row["LastName"].ToString().Trim();
            txtLicense.Text = row["License"].ToString().Trim();
            txtClass.Text = row["Class"].ToString().Trim();

            if (row["LicenseIssued"] != DBNull.Value && row["LicenseIssued"].ToString() != "")
            {
                txtLicenseIssued.Text = Convert.ToDateTime(row["LicenseIssued"]).ToString(base.GetLocalResourceObject("resDateFormat").ToString());
            }
            else txtLicenseIssued.Text = "";

            if (row["LicenseExpired"] != DBNull.Value && row["LicenseExpired"].ToString() != "")
                txtLicenseExpired.Text = Convert.ToDateTime(row["LicenseExpired"]).ToString(base.GetLocalResourceObject("resDateFormat").ToString());
            else txtLicenseExpired.Text = "";

            if (row["Gender"] != DBNull.Value && row["Gender"].ToString() != "")
            {
                ddlGender.SelectedValue = row["Gender"].ToString();
            }
            txtHeight.Text = row["Height"].ToString();
            txtHomePhone.Text = row["HomePhone"].ToString().Trim();
            txtCellPhone.Text = row["CellPhone"].ToString().Trim();
            txtAdditionalPhone.Text = row["AdditionalPhone"].ToString().Trim();
            txtEmail.Text = row["Email"].ToString().Trim();
            txtSmsid.Text = row["Smsid"].ToString().Trim();
            ViewState["SMSPwd"] = row["SMSPwd"].ToString().Trim();
            txtSMSPWD.Text = row["SMSPwd"].ToString().Trim();
            txtSMSPWDConfirm.Text = row["SMSPwd"].ToString().Trim();
            txtAddress.Text = row["Address"].ToString().Trim();
            txtCity.Text = row["City"].ToString().Trim();
            txtZipcode.Text = row["Zipcode"].ToString().Trim();
            string state = row["State"].ToString().Trim();
            for (int i = 0; i < ddlState.Items.Count; i++)
            {
                if (ddlState.Items[i].Text.IndexOf(state) > -1)
                {
                    ddlState.SelectedIndex = i;
                    break;
                }
            }
            txtCountry.Text = row["Country"].ToString().Trim();
            txtDescription.Text = row["Description"].ToString().Trim();
            this.txtKeyFobId.Text = row["KeyFobId"].ToString().Trim();

            Boolean isEmergency = false;
            txtEmergencytel.Text = string.Empty;
            ddlEmergencyPlan.SelectedIndex = 0;
            if (row["EmergencyPhone"] != null)
            {
                if (row["EmergencyPhone"].ToString().Trim() != string.Empty)
                {
                    txtEmergencytel.Text = row["EmergencyPhone"].ToString().Trim();
                    isEmergency = true;
                }
            }

            if (row["ContactPlanId"] != null)
            {
                int contactPlanId = 0;
                if (int.TryParse(row["ContactPlanId"].ToString(), out contactPlanId))
                {
                    isEmergency = true;
                    ListItem lstItem = ddlEmergencyPlan.Items.FindByValue(row["ContactPlanId"].ToString());
                    if (lstItem != null)
                    {
                        ddlEmergencyPlan.ClearSelection();
                        lstItem.Selected = true;
                    }
                }
            }

            chkAssignEmergency.Checked = isEmergency;

            //Salman June 27, 2014
            if (row["TerminationDate"] != DBNull.Value)
            {
                txtTerminationDate.Text = Convert.ToDateTime(row["TerminationDate"]).ToString(base.GetLocalResourceObject("resDateFormat").ToString());
                //txtTerminationDate.Text = Convert.ToDateTime(DateTime.UtcNow.ToString()).ToString(base.GetLocalResourceObject("resDateFormat").ToString());
            }

            txtPositionInfo.Text = row["PositionInfo"].ToString().Trim();// +"Salman";
        }

        private void FillStates()
        {
            string xmlStates = "";
            // load states from the db
            using (ServerDBSystem.DBSystem system = new ServerDBSystem.DBSystem())
            {

                if (objUtil.ErrCheck(system.GetAllStateProvinces(sn.UserID, sn.SecId, ref xmlStates), false))
                {
                    if (objUtil.ErrCheck(system.GetAllStateProvinces(sn.UserID, sn.SecId, ref xmlStates), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(Enums.TraceSeverity.Error,
                           " Error getting states info:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        //lblMessage.Text = base.GetLocalResourceObject("resGetStatesError").ToString(); //Error getting states info";
                        return;
                    }
                }
                else
                {
                    DataSet dsStates = new DataSet();
                    if (!String.IsNullOrEmpty(xmlStates))
                        dsStates.ReadXml(new StringReader(xmlStates));
                    if (VLF.CLS.Util.IsDataSetValid(dsStates))
                    {
                        ddlState.DataTextField = "StateProvince";
                        ddlState.DataValueField = "StateProvince";
                        ddlState.DataSource = dsStates;
                        ddlState.DataBind();
                    }
                }
            }
        }

        /// <summary>
        /// Create xls file of all driver asignments
        /// </summary>
        /// <returns>File Url</returns>
        private void DrvAssgn2Excel(DataSet dsDrivers)
        {
            try
            {
                if (!VLF.CLS.Util.IsDataSetValid(dsDrivers))
                {
                    base.ShowMessage(this.lblMessage, base.GetResource("resNoDrivers"), Color.Red);
                    return;
                }

                string srcPath = Server.MapPath("~/App_Data/DriversAssignmentTemplate.xls");
                string destPath = Server.MapPath(this.drvAssgnUrl);
                string connString =
                   String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=Yes;IMEX=1\";", destPath);

                if (File.Exists(srcPath))
                {
                    SetFileAttrNormal(destPath);
                    File.Copy(srcPath, destPath, true);
                    SetFileAttrNormal(destPath);
                }

                string sql = "INSERT INTO [Sheet1$] VALUES (?, ?, ?, ?, ?, '')";

                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();
                    using (OleDbCommand insCmd = new OleDbCommand(sql, conn))
                    {
                        insCmd.CommandType = CommandType.Text;
                        foreach (DataRow row in dsDrivers.Tables[0].Rows)
                        {
                            insCmd.Parameters.Clear();
                            insCmd.Parameters.Add("@id", OleDbType.VarChar).Value = row["DriverId"].ToString();
                            insCmd.Parameters.Add("@fn", OleDbType.VarChar).Value = row["FirstName"].ToString();
                            insCmd.Parameters.Add("@ln", OleDbType.VarChar).Value = row["LastName"].ToString();
                            insCmd.Parameters.Add("@des", OleDbType.VarChar).Value = row["VehicleDescription"].ToString();
                            insCmd.Parameters.Add("@dt", OleDbType.Date).Value = DateTime.UtcNow.ToString();
                            //insCmd.Parameters.Add("@cmt", OleDbType.VarChar).Value = "";
                            insCmd.ExecuteNonQuery();
                        }
                    }
                    conn.Close();
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                   VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                base.ShowMessage(this.lblMessage, Ex.Message, Color.Red);
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                   VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                   Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                base.ShowMessage(this.lblMessage, Ex.Message, Color.Red);
            }
        }

        /// <summary>
        /// Create xls file of all drivers
        /// </summary>
        /// <returns>File Url</returns>
        private void Drv2Excel(DataSet dsDrivers)
        {
            try
            {
                if (!VLF.CLS.Util.IsDataSetValid(dsDrivers))
                {
                    base.ShowMessage(this.lblMessage, base.GetResource("resNoDrivers"), Color.Red);
                    return;
                }

                string srcPath = Server.MapPath("~/App_Data/DriversTemplate.xls");
                string destPath = Server.MapPath(this.drvUrl);
                string connString =
                   String.Format("Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=Yes;IMEX=1\";", destPath);

                if (File.Exists(srcPath))
                {
                    SetFileAttrNormal(destPath);
                    File.Copy(srcPath, destPath, true);
                    SetFileAttrNormal(destPath);
                }

                string sql = "INSERT INTO [Sheet1$] VALUES (?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?, ?)";

                using (OleDbConnection conn = new OleDbConnection(connString))
                {
                    conn.Open();
                    using (OleDbCommand insCmd = new OleDbCommand(sql, conn))
                    {
                        insCmd.CommandType = CommandType.Text;
                        foreach (DataRow row in dsDrivers.Tables[0].Rows)
                        {
                            insCmd.Parameters.Clear();

                            //insCmd.Parameters.Add("@id", OleDbType.VarChar).Value = row["DriverId"].ToString();
                            insCmd.Parameters.Add("@fn", OleDbType.VarChar).Value = row["FirstName"].ToString();
                            insCmd.Parameters.Add("@ln", OleDbType.VarChar).Value = row["LastName"].ToString();
                            insCmd.Parameters.Add("@lic", OleDbType.VarChar).Value = row["License"].ToString();
                            insCmd.Parameters.Add("@cls", OleDbType.VarChar).Value = row["Class"].ToString();
                            insCmd.Parameters.Add("@lic_i", OleDbType.Date).Value = Convert.ToDateTime(row["LicenseIssued"], new CultureInfo("en-US")).ToString("MM-hh-yyyy");
                            insCmd.Parameters.Add("@lic_e", OleDbType.Date).Value = Convert.ToDateTime(row["LicenseExpired"], new CultureInfo("en-US")).ToString("MM-hh-yyyy");
                            insCmd.Parameters.Add("@gen", OleDbType.VarChar).Value = row["Gender"].ToString();
                            insCmd.Parameters.Add("@ht", OleDbType.VarChar).Value = row["Height"].ToString();
                            insCmd.Parameters.Add("@phn_h", OleDbType.VarChar).Value = row["HomePhone"].ToString();
                            insCmd.Parameters.Add("@phn_c", OleDbType.VarChar).Value = row["CellPhone"].ToString();
                            insCmd.Parameters.Add("@phn_a", OleDbType.VarChar).Value = row["AdditionalPhone"].ToString();
                            insCmd.Parameters.Add("@sms", OleDbType.VarChar).Value = row["SMSID"].ToString();
                            insCmd.Parameters.Add("@email", OleDbType.VarChar).Value = row["Email"].ToString();
                            insCmd.Parameters.Add("@adr", OleDbType.VarChar).Value = row["Address"].ToString();
                            insCmd.Parameters.Add("@city", OleDbType.VarChar).Value = row["City"].ToString();
                            insCmd.Parameters.Add("@zip", OleDbType.VarChar).Value = row["ZipCode"].ToString();
                            insCmd.Parameters.Add("@st", OleDbType.VarChar).Value = row["State"].ToString();
                            insCmd.Parameters.Add("@country", OleDbType.VarChar).Value = row["Country"].ToString();
                            insCmd.Parameters.Add("@descr", OleDbType.VarChar).Value = row["Description"].ToString();

                            insCmd.ExecuteNonQuery();
                        }
                    }
                    conn.Close();
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                   VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                base.ShowMessage(this.lblMessage, Ex.Message, Color.Red);
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                   VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                   Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                base.ShowMessage(this.lblMessage, Ex.Message, Color.Red);
            }
        }

        /// <summary>
        /// Check if file is read only and set its attr. to normal
        /// </summary>
        /// <param name="destPath"></param>
        private void SetFileAttrNormal(string destPath)
        {
            if (File.Exists(destPath))
                if ((File.GetAttributes(destPath) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
                    File.SetAttributes(destPath, FileAttributes.Normal);
        }

        # endregion

        private void SaveDriverInRapidLog(string smsid, string firstName, string lastName, string CompanyCode,
                string license, string state, DateTime licenseExpired, string country)
        {
            if (smsid == null || smsid.Trim().Equals(""))
            {
                return;
            }
            else
            {
                string connectionStr = System.Web.Configuration.WebConfigurationManager.ConnectionStrings["RapidLogConnectionString"].ConnectionString;
                RapidLog rapidLog = new RapidLog();

                rapidLog.SaveDriver(connectionStr, smsid, firstName, lastName, CompanyCode, license, state, licenseExpired, country);
            }
            return;
        }


        protected void cmdSaveDriver_Click(object sender, EventArgs e)
        {
            try
            {
                short height = 0;
                int drvId = 0;
                DateTime issuedDT = new DateTime(), expDT = new DateTime();
                DateTime? terminationDate = new DateTime?();

                # region Data Validation
                if (String.IsNullOrEmpty(txtFirstName.Text))
                {
                    lblMessage.Text = this.GetLocalResourceObject("resFillFirstName").ToString(); //"Please fill in the first name";
                    return;
                }
                if (String.IsNullOrEmpty(txtLastName.Text))
                {
                    lblMessage.Text = this.GetLocalResourceObject("resFillLastName").ToString(); //"Please fill in the last name";
                    return;
                }
                if (String.IsNullOrEmpty(txtLicense.Text))
                {
                    lblMessage.Text = this.GetLocalResourceObject("resFillLicense").ToString(); //"Please fill in the license";
                    return;
                }
                if (String.IsNullOrEmpty(txtClass.Text))
                {
                    lblMessage.Text = this.GetLocalResourceObject("resFillClass").ToString(); //"Please fill in the license class";
                    return;
                }
                //if (!DateTime.TryParseExact(txtLicenseIssued.Text, this.GetLocalResourceObject("resDateFormat").ToString(),
                //   CultureInfo.CurrentUICulture, DateTimeStyles.None, out issuedDT))
                //{

                if (String.IsNullOrEmpty(txtLicenseIssued.Text))
                {
                    lblMessage.Text = this.GetLocalResourceObject("resFillLicenseIssued").ToString(); //"Please fill in the license issued date";
                    return;
                }

                //if (!DateTime.TryParseExact(txtLicenseExpired.Text, this.GetLocalResourceObject("resDateFormat").ToString(),
                //   CultureInfo.CurrentUICulture, DateTimeStyles.None, out expDT))
                //{
                if (String.IsNullOrEmpty(txtLicenseExpired.Text))
                {
                    lblMessage.Text = this.GetLocalResourceObject("resFillLicenseExpired").ToString(); //"Please fill in the license expiration date";
                    return;
                }

                //issuedDT = Convert.ToDateTime(txtLicenseIssued.Text);
                //expDT = Convert.ToDateTime(txtLicenseExpired.Text);
                issuedDT = FormatedDate(txtLicenseIssued.Text);
                expDT = FormatedDate(txtLicenseExpired.Text);

                if (!string.IsNullOrEmpty(txtTerminationDate.Text))
                    //terminationDate = Convert.ToDateTime(txtTerminationDate.Text).ToUniversalTime();
                    terminationDate = FormatedDate(txtTerminationDate.Text).ToUniversalTime();

                if (issuedDT >= expDT)
                {
                    lblMessage.Text = this.GetLocalResourceObject("resExpirationDateNotValid").ToString();
                    return;
                }

                if (!String.IsNullOrEmpty(txtHeight.Text))
                {
                    if (!Int16.TryParse(txtHeight.Text, out height))
                    {
                        lblMessage.Text = this.GetLocalResourceObject("resHeightFormat").ToString(); //"The height format is invalid";
                        return;
                    }
                }

                if ((ViewState["SMSPwd"] == null && this.txtSMSPWD.Text != this.txtSMSPWDConfirm.Text) || (ViewState["SMSPwd"] != null && ViewState["SMSPwd"].ToString() != this.txtSMSPWD.Text && this.txtSMSPWD.Text != this.txtSMSPWDConfirm.Text))
                {
                    lblMessage.Text = "The please retype your password";
                    return;
                }

                if (!String.IsNullOrEmpty(txtSMSPWD.Text))
                    ViewState["SMSPwd"] = txtSMSPWD.Text.Trim();
                else
                    ViewState["SMSPwd"] = "";

                # endregion
                ServerDBUser.DBUser vdbu = new ServerDBUser.DBUser();
                if (ViewState["DriverID"] == null)
                {
                    try
                    {
                        # region Parse controls

                        //clsXmlUtil xmlObject = new clsXmlUtil();
                        string firstName = txtFirstName.Text.Trim();
                        string lastName = txtLastName.Text.Trim();
                        string license = txtLicense.Text.Trim();
                        string licenseClass = txtClass.Text.Trim();
                        DateTime licenseIssued = issuedDT.ToUniversalTime();
                        DateTime licenseExpired = expDT.ToUniversalTime();
                        int organizationId = sn.User.OrganizationId;
                        string gender = ddlGender.SelectedValue[0].ToString();
                        //int height =  height;
                        string homePhone = txtHomePhone.Text.Trim();
                        string cellPhone = txtCellPhone.Text.Trim();
                        string additionalPhone = txtAdditionalPhone.Text.Trim();
                        string smsid = txtSmsid.Text.Trim();
                        string smsPWD = ViewState["SMSPwd"].ToString();
                        string email = txtEmail.Text.Trim();
                        string address = txtAddress.Text.Trim();
                        string city = txtCity.Text.Trim();
                        string zipcode = txtZipcode.Text.Trim();
                        string state = ddlState.SelectedItem.Text.Trim();
                        string country = txtCountry.Text.Trim();
                        string description = txtDescription.Text.Trim();
                        string keyFobId = this.txtKeyFobId.Text.Trim();

                        bool isAssignPanic = chkAssignEmergency.Checked;
                        string emergencyPhone = null;
                        int? contactPlanId = null;
                        if (isAssignPanic)
                        {
                            emergencyPhone = txtEmergencytel.Text.Trim();
                            if (ddlEmergencyPlan.SelectedIndex > 0)
                                contactPlanId = int.Parse(ddlEmergencyPlan.SelectedValue);

                            if (string.IsNullOrEmpty(emergencyPhone) && contactPlanId == null) isAssignPanic = false;
                        }

                        # endregion
                        //Devin changed 0n 2014.05.26 for cycle and supervisor
                        int? usaCycle = null;
                        if (ddlUsaCycle.SelectedIndex > 0) usaCycle = int.Parse(ddlUsaCycle.SelectedValue);

                        int? caCycle = null;
                        if (ddlCaCycle.SelectedIndex > 0) caCycle = int.Parse(ddlCaCycle.SelectedValue);

                        float? timeZone = null;
                        if (ddlTimeZone.SelectedIndex > 0) timeZone = float.Parse(ddlTimeZone.SelectedValue);

                        Boolean isSupervisor = false;
                        isSupervisor = chkIsSupervisor.Checked;

                        string positionInfo = txtPositionInfo.Text.Trim();

                        bool bIsDriverValid = Convert.ToBoolean(contactMsg.CheckIfDriverUnique(drvId, organizationId, firstName, lastName, smsid));
                        if (!bIsDriverValid)
                        {
                            if (String.IsNullOrEmpty(smsid))
                                lblMessage.Text = this.GetLocalResourceObject("resAddDriverError").ToString() + " " + this.GetLocalResourceObject("resDriverErrorNameExists").ToString();
                            else
                                lblMessage.Text = this.GetLocalResourceObject("resAddDriverError").ToString() + " " + this.GetLocalResourceObject("resDriverErrorNameAndEmployeeNoExists").ToString();
                            return;
                        }

                        contactMsg.DriverAndContactPlanCycleAdd(firstName, lastName, license, licenseClass, licenseIssued,
                                                           licenseExpired, organizationId, gender[0], height,
                                                           homePhone, cellPhone, additionalPhone,
                                                           smsPWD, smsid, email, address, city, zipcode, state,
                                                           country, description,
                                                           emergencyPhone, contactPlanId, isAssignPanic, keyFobId,
                                                           caCycle, usaCycle, isSupervisor, positionInfo, timeZone);

                        vdbu.RecordUserAction("Driver", sn.UserID, 0, "vlfDriver",
                                                 null,
                                                 "Add driver", this.Context.Request.UserHostAddress,
                                                 this.Context.Request.RawUrl,
                                                 string.Format("FirstName='{0}' AND LastName='{1}'", firstName, lastName));

                        if(sn.User.OrganizationId==123 && "1".Equals(ConfigurationManager.AppSettings["RapidLog"])){
                            SaveDriverInRapidLog(smsid, firstName, lastName, sn.User.OrganizationId.ToString(), license, state, licenseExpired, country);
                        }


                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                           VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                           ex.Message.ToString() + " Add driver error for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));                                //RedirectToLogin();
                        lblMessage.Text = this.GetLocalResourceObject("resAddDriverError").ToString();
                        return;

                    }
                }
                else
                {
                    try
                    {
                        # region Parse controls

                        if (!Int32.TryParse(ViewState["DriverID"].ToString(), out drvId))//Session["driverid"].ToString(), out drvId))
                        {
                            //lblMessage.Text = "The session was expired";
                            RedirectToLogin();
                        }
                        //clsXmlUtil xmlObject = new clsXmlUtil();
                        //xmlObject.CreateNode("DriverId", drvId);
                        string firstName = txtFirstName.Text.Trim();
                        string lastName = txtLastName.Text.Trim();
                        string license = txtLicense.Text.Trim();
                        string licenseClass = txtClass.Text.Trim();
                        DateTime licenseIssued = issuedDT.ToUniversalTime();
                        DateTime licenseExpired = expDT.ToUniversalTime();
                        int organizationId = sn.User.OrganizationId;
                        char gender = ddlGender.SelectedValue[0];
                        //height;
                        string homePhone = txtHomePhone.Text.Trim();
                        string cellPhone = txtCellPhone.Text.Trim();
                        string additionalPhone = txtAdditionalPhone.Text.Trim();
                        string smsid = txtSmsid.Text.Trim();
                        string smsPWD = ViewState["SMSPwd"].ToString();
                        string email = txtEmail.Text.Trim();
                        string address = txtAddress.Text.Trim();
                        string city = txtCity.Text.Trim();
                        string zipcode = txtZipcode.Text.Trim();
                        string state = ddlState.SelectedItem.Text.Trim();
                        string country = txtCountry.Text.Trim();
                        string description = txtDescription.Text.Trim();
                        string keyFobId = this.txtKeyFobId.Text.Trim();

                        bool isAssignPanic = chkAssignEmergency.Checked;
                        string emergencyPhone = null;
                        int? contactPlanId = null;
                        if (isAssignPanic)
                        {
                            emergencyPhone = txtEmergencytel.Text.Trim();
                            if (ddlEmergencyPlan.SelectedIndex > 0)
                                contactPlanId = int.Parse(ddlEmergencyPlan.SelectedValue);
                            if (string.IsNullOrEmpty(emergencyPhone) && contactPlanId == null) isAssignPanic = false;
                        }

                        # endregion
                        //Devin changed 0n 2014.05.26 for cycle and supervisor
                        int? usaCycle = null;
                        if (ddlUsaCycle.SelectedIndex > 0) usaCycle = int.Parse(ddlUsaCycle.SelectedValue);

                        int? caCycle = null;
                        if (ddlCaCycle.SelectedIndex > 0) caCycle = int.Parse(ddlCaCycle.SelectedValue);

                        float? timeZone = null;
                        if (ddlTimeZone.SelectedIndex > 0) timeZone = float.Parse(ddlTimeZone.SelectedValue);

                        Boolean isSupervisor = false;
                        isSupervisor = chkIsSupervisor.Checked;

                        string positionInfo = txtPositionInfo.Text.Trim();
                        //DateTime? _terminationDate = terminationDate.ToUniversalTime();

                        bool bIsDriverValid = Convert.ToBoolean(contactMsg.CheckIfDriverUnique(drvId, organizationId, firstName, lastName, smsid));
                        if (!bIsDriverValid)
                        {
                            if (String.IsNullOrEmpty(smsid))
                                lblMessage.Text = this.GetLocalResourceObject("resUpdateDriverError").ToString() + " " + this.GetLocalResourceObject("resDriverErrorNameExists").ToString();
                            else
                                lblMessage.Text = this.GetLocalResourceObject("resUpdateDriverError").ToString() + " " + this.GetLocalResourceObject("resDriverErrorNameAndEmployeeNoExists").ToString();
                            return;
                        }

                        contactMsg.DriverAndContactPlanCycleUpdate(drvId, firstName, lastName, license, licenseClass,
                              licenseIssued, licenseExpired, organizationId, gender, height, homePhone, cellPhone,
                              additionalPhone, smsPWD, smsid, email, address, city, zipcode, state, country,
                              description, emergencyPhone, contactPlanId, isAssignPanic, keyFobId,
                              caCycle, usaCycle, isSupervisor, positionInfo, terminationDate, timeZone);
                        vdbu.RecordUserAction("Driver", sn.UserID, 0, "vlfDriver",
                                                 string.Format("DriverId={0}", Convert.ToInt32(ViewState["DriverID"])),
                                                 "Update driver", this.Context.Request.UserHostAddress,
                                                 this.Context.Request.RawUrl,
                                                 string.Format("Update driver({0})", Convert.ToInt32(ViewState["DriverID"])));
                        if (ViewState["DriverID"] != null) ViewState.Remove("DriverID");

                        if (sn.User.OrganizationId == 123 && "1".Equals(ConfigurationManager.AppSettings["RapidLog"]))
                        {
                            SaveDriverInRapidLog(smsid, firstName, lastName, sn.User.OrganizationId.ToString(), license, state, licenseExpired, country);
                        }


                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                           VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                           ex.Message.ToString() + " Update driver error for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));                                //RedirectToLogin();

                        //RedirectToLogin();
                        lblMessage.Text = this.GetLocalResourceObject("resUpdateDriverError").ToString();
                        return;

                    }
                }
                tblDriverDetails.Visible = false;
                tblViewDrivers.Visible = true;
                LoadDrivers(true);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                //lblMessage.Text = Ex.Message;
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //lblMessage.Text = Ex.Message;

            }
        }

        protected void cmdAddDriver_Click(object sender, EventArgs e)
        {
            this.tblViewDrivers.Visible = false;
            this.tblDriverDetails.Visible = true;
            this.tblUploadData.Visible = false;
            lblMessage.Text = "";
            if (ViewState["DriverID"] != null) ViewState.Remove("DriverID");
            ClearControls();
        }

        protected void cmdCancel_Click(object sender, EventArgs e)
        {
            tblViewDrivers.Visible = true;
            tblDriverDetails.Visible = false;
            lblMessage.Text = "";
        }

        protected void cmdImportDrivers_Click(object sender, EventArgs e)
        {
            ImportSelect("1");
        }

        protected void cmdUpdateDrivers_Click(object sender, EventArgs e)
        {
            ImportSelect("3");
        }

        private void ImportSelect(string btnClicked)
        {
            try
            {
                this.tblViewDrivers.Visible = false;
                this.tblDriverDetails.Visible = false;
                this.tblUploadData.Visible = true;

                this.LabelImportCaption.Text = btnClicked.Equals("1") ? this.cmdUploadDriverList.Text : btnClicked.Equals("2") ? this.cmdUploadAssignments.Text : this.cmdUpdateDriver.Text;

                this.ImportState.Value = btnClicked;
                if (ViewState["DriverID"] != null) ViewState.Remove("DriverID");
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        protected void cmdCancelUpload_Click(object sender, EventArgs e)
        {
            this.tblViewDrivers.Visible = true;
            this.tblDriverDetails.Visible = false;
            this.tblUploadData.Visible = false;
            lblMessage.Text = "";
        }

        public DataTable ReadDataFromExcelUsingNPOI(string filePath)
        {
            HSSFWorkbook hssfworkbook;

            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }

            HSSFSheet sheet = (HSSFSheet)hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            DataTable dt = new DataTable();
            //for (int j = 0; j < 5; j++)
            //{
            //    dt.Columns.Add(Convert.ToChar(((int)'A') + j).ToString());
            //}

            if (rows.MoveNext())
            {
                HSSFRow row0 = (HSSFRow)rows.Current;
                for (int i = 0; i < row0.LastCellNum; i++)
                {
                    HSSFCell cell = (HSSFCell)row0.GetCell(i);

                    dt.Columns.Add(cell.ToString());
                    //dr[i] = cell.ToString();

                }
            }

            while (rows.MoveNext())
            {
                HSSFRow row = (HSSFRow)rows.Current;
                DataRow dr = dt.NewRow();

                for (int i = 0; i < row.LastCellNum; i++)
                {
                    HSSFCell cell = (HSSFCell)row.GetCell(i);

                    dr[i] = (cell == null) ? "" : cell.ToString();

                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        protected void cmdSaveUpload_Click(object sender, EventArgs e)
        {
            string fileName = "";
            try
            {

                this.lblMessage.Text = "";

                # region Upload excel file

                if (this.filePath.FileName.Trim() == String.Empty)
                {
                    base.ShowMessage(this.lblMessage, base.GetLocalResourceObject("resSelectFile").ToString(), Color.Red);
                    return;
                }
                //if (fileOK)
                //{
                try
                {
                    fileName = Server.MapPath("~/App_Data/") + this.filePath.FileName;
                    this.filePath.PostedFile.SaveAs(fileName);
                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                       VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message + " :: " + Ex.StackTrace));
                    base.ShowMessage(this.lblMessage, base.GetLocalResourceObject("resErrorUploadingFile").ToString(), Color.Red);
                }

                string hdr = "", xmlData = "";
                // first line contains headers
                hdr = this.chkHeaderRow.Checked ? "Yes" : "No";

                // connection string
                string connString = String.Format(
                   "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=Yes;IMEX=1\";",
                   fileName, hdr);

                # endregion

                if (this.ImportState.Value == "1") // drivers
                {
                    DataTable dtDrivers = ReadDataFromExcelUsingNPOI(fileName);
                    DataSet dsDrivers = new DataSet("Drivers");
                    dsDrivers.ReadXmlSchema(Server.MapPath("~/Datasets/DriverImportTemplate.xsd"));

                    # region Parse excel file

                    # endregion

                    # region Send data to db
                    ServerDBUser.DBUser vdbu = new ServerDBUser.DBUser();
                    foreach (DataRow dr in dtDrivers.Rows)
                    {
                        string firstName = "";
                        string lastName = "";
                        string license = "";
                        string licenseClass = "";
                        DateTime licenseIssued = DateTime.MinValue;
                        DateTime licenseExpired = DateTime.MinValue;
                        int organizationId = sn.User.OrganizationId;
                        string gender = "";
                        short height = 0;
                        string homePhone = "";
                        string cellPhone = "";
                        string additionalPhone = "";
                        string smsid = "";
                        string smsPWD = "";
                        string email = "";
                        string address = "";
                        string city = "";
                        string zipcode = "";
                        string state = "";
                        string country = "";
                        string description = "";
                        string keyFobId = "";

                        if (dr["FirstName"] != DBNull.Value)
                            firstName = dr["FirstName"].ToString();
                        if (dr["LastName"] != DBNull.Value)
                            lastName = dr["LastName"].ToString();
                        if (dr["License"] != DBNull.Value)
                            license = dr["License"].ToString();
                        if (dr["Class"] != DBNull.Value)
                            licenseClass = dr["Class"].ToString();
                        if (dr["LicenseIssued"] != DBNull.Value)
                        {
                            DateTime.TryParse(dr["LicenseIssued"].ToString(), out licenseIssued);
                        }
                        if (dr["LicenseExpired"] != DBNull.Value)
                        {
                            DateTime.TryParse(dr["LicenseExpired"].ToString(), out licenseExpired);
                        }
                        if (licenseIssued == DateTime.MinValue)
                            licenseIssued = new DateTime(2010, 01, 01);
                        if (licenseExpired == DateTime.MinValue)
                            licenseExpired = new DateTime(2020, 01, 01);

                        if (dr["Gender"] != DBNull.Value)
                            gender = dr["Gender"].ToString();
                        if (dr["Height"] != DBNull.Value)
                            short.TryParse(dr["Height"].ToString(), out height);
                        if (dr["HomePhone"] != DBNull.Value)
                            homePhone = dr["HomePhone"].ToString();
                        if (dr["CellPhone"] != DBNull.Value)
                            cellPhone = dr["CellPhone"].ToString();
                        if (dr["AdditionalPhone"] != DBNull.Value)
                            additionalPhone = dr["AdditionalPhone"].ToString();
                        if (dr["Email"] != DBNull.Value)
                            email = dr["Email"].ToString();
                        if (dr["City"] != DBNull.Value)
                            city = dr["City"].ToString();
                        if (dr["Address"] != DBNull.Value)
                            address = dr["Address"].ToString();
                        if (dr["State"] != DBNull.Value)
                        {
                            foreach (ListItem lst in ddlState.Items)
                            {
                                if (lst.Value == dr["State"].ToString() || lst.Value.IndexOf("," + dr["State"].ToString()) > 0)
                                {
                                    state = lst.Value;
                                    break;
                                }
                            }
                        }

                        if (state == "") state = ddlState.Items[0].Value;
                        if (dr["ZipCode"] != DBNull.Value)
                            zipcode = dr["ZipCode"].ToString();
                        if (dr["Country"] != DBNull.Value)
                            country = dr["Country"].ToString();
                        if (dr["Description"] != DBNull.Value)
                            description = dr["Description"].ToString();
                        if (dr["SMSID"] != DBNull.Value)
                            smsid = dr["SMSID"].ToString();
                        if (dr["SMSPWD"] != DBNull.Value)
                            smsPWD = dr["SMSPWD"].ToString();
                        if (dr["KeyFob"] != DBNull.Value)
                            keyFobId = dr["KeyFob"].ToString();

                        bool isAssignPanic = false;
                        string emergencyPhone = null;
                        int? contactPlanId = null;

                        if (gender == "") gender = "M";
                    # endregion

                        contactMsg.DriverAndContactPlanAdd(firstName, lastName, license, licenseClass, licenseIssued,
                                                           licenseExpired, organizationId, gender[0], height,
                                                           homePhone, cellPhone, additionalPhone,
                                                           smsPWD, smsid, email, address, city, zipcode, state,
                                                           country, description,
                                                         emergencyPhone, contactPlanId, isAssignPanic, keyFobId);

                        vdbu.RecordUserAction("Driver", sn.UserID, 0, "vlfDriver",
                                                 null,
                                                 "Add driver", this.Context.Request.UserHostAddress,
                                                 this.Context.Request.RawUrl,
                                                 string.Format("FirstName='{0}' AND LastName='{1}'", firstName, lastName));
                    }

                }
                else if (this.ImportState.Value == "3")
                {
                    try
                    {
                    // drivers 
                    DataTable dtDrivers = ReadDataFromExcelUsingNPOI(fileName);
                    DataSet dsDrivers = new DataSet("Drivers");
                    dsDrivers.ReadXmlSchema(Server.MapPath("~/Datasets/DriverUpdateTemplate.xsd"));

                    
                    ServerDBUser.DBUser vdbu = new ServerDBUser.DBUser();
                    foreach (DataRow dr in dtDrivers.Rows)
                    {
                            if (dr["DriverID"] != "")
                            {
                        int drvId = 0;
                        string firstName = "";
                        string lastName = "";
                        string license = "";
                        string licenseClass = "";
                        DateTime licenseIssued = DateTime.MinValue;
                        DateTime licenseExpired = DateTime.MinValue;
                        int organizationId = sn.User.OrganizationId;
                        //string gender = "";
                        string gender = "";
                        //ddlGender.SelectedValue[0];
                        short height = 0;
                        string homePhone = "";
                        string cellPhone = "";
                        string additionalPhone = "";
                        string smsid = "";
                        string smsPWD = "";
                        string email = "";
                        string address = "";
                        string city = "";
                        string zipcode = "";
                        string state = "";
                        string country = "";
                        string description = "";
                        string keyFobId = "";
                        short caCycle = 0;
                        short usaCycle = 0;
                        bool isSupervisor = false;
                        string positionInfo = "";
                        DateTime terminationDate = DateTime.MinValue;
                        float timeZone = 0.0f;

                        if (dr["DriverID"] != DBNull.Value)
                            drvId = Int32.Parse(dr["DriverID"].ToString());
                        if (dr["First Name"] != DBNull.Value)
                            firstName = dr["First Name"].ToString();
                        if (dr["Last Name"] != DBNull.Value)
                            lastName = dr["Last Name"].ToString();
                        if (dr["License"] != DBNull.Value)
                            license = dr["License"].ToString();
                        if (dr["Class"] != DBNull.Value)
                            licenseClass = dr["Class"].ToString();
                        if (dr["License issued"] != DBNull.Value)
                        {
                            DateTime.TryParse(dr["License issued"].ToString(), out licenseIssued);
                        }
                        if (dr["License expired"] != DBNull.Value)
                        {
                            DateTime.TryParse(dr["License expired"].ToString(), out licenseExpired);
                        }
                        if (dr["Termination Date"] != DBNull.Value)
                        {
                            DateTime.TryParse(dr["Termination Date"].ToString(), out terminationDate);
                        }

                        if (dr["Gender"] != DBNull.Value)
                            gender = dr["Gender"].ToString();

                        if (dr["Height"] != DBNull.Value)
                            short.TryParse(dr["Height"].ToString(), out height);
                        if (dr["Home Phone"] != DBNull.Value)
                            homePhone = dr["Home Phone"].ToString();
                        if (dr["Cell Phone"] != DBNull.Value)
                            cellPhone = dr["Cell Phone"].ToString();
                        if (dr["Additional Phone"] != DBNull.Value)
                            additionalPhone = dr["Additional Phone"].ToString();
                                if (dr["E-mail"] != DBNull.Value)
                                    email = dr["E-mail"].ToString();
                        if (dr["City"] != DBNull.Value)
                            city = dr["City"].ToString();
                        if (dr["Address"] != DBNull.Value)
                            address = dr["Address"].ToString();
                        if (dr["State"] != DBNull.Value)
                        {
                            foreach (ListItem lst in ddlState.Items)
                            {
                                if (lst.Value == dr["State"].ToString() || lst.Value.IndexOf("," + dr["State"].ToString()) > 0)
                                {
                                    state = lst.Value;
                                    break;
                                }
                            }
                        }
                        if (state == "") state = ddlState.Items[0].Value;
                        if (dr["Zipcode"] != DBNull.Value)
                            zipcode = dr["Zipcode"].ToString();
                        if (dr["Country"] != DBNull.Value)
                            country = dr["Country"].ToString();
                        if (dr["Description"] != DBNull.Value)
                            description = dr["Description"].ToString();
                        if (dr["SMS Id"] != DBNull.Value)
                            smsid = dr["SMS Id"].ToString();
                        if (dr["SMS Pwd"] != DBNull.Value)
                            smsPWD = dr["SMS Pwd"].ToString();
                        if (dr["Key Fob Id"] != DBNull.Value)
                            keyFobId = dr["Key Fob Id"].ToString();
                                if (dr["US Cycle"] != DBNull.Value && dr["US Cycle"] != "")
                            usaCycle = short.Parse(dr["US Cycle"].ToString());
                        if (dr["CA Cycle"] != DBNull.Value && dr["CA Cycle"] != "")
                            caCycle = short.Parse(dr["CA Cycle"].ToString());
                        if (dr["Postion Info"] != DBNull.Value)
                            positionInfo = dr["Postion Info"].ToString();
                        if (dr["TimeZone"] != DBNull.Value)
                            timeZone = float.Parse(dr["TimeZone"].ToString());

                        bool isAssignPanic = false;
                        string emergencyPhone = "";
                        int? contactPlanId = null;

                        if (gender == "") 
                            gender = "M";

                        DataSet dsDriverInfo = contactMsg.GetDriverByDriverID(drvId);
                        if (dsDriverInfo != null && dsDriverInfo.Tables.Count > 0 && dsDriverInfo.Tables[0].Rows.Count > 0)
                        {
                            if (dsDriverInfo.Tables[0].Rows[0]["OrganizationId"].ToString() == sn.User.OrganizationId.ToString())
                            {
                                if (firstName == "")
                                {
                                    firstName = dsDriverInfo.Tables[0].Rows[0]["FirstName"].ToString();
                                }
                                if (lastName == "")
                                {
                                    lastName = dsDriverInfo.Tables[0].Rows[0]["LastName"].ToString();
                                }
                                if (license == "")
                                {
                                    license = dsDriverInfo.Tables[0].Rows[0]["License"].ToString();
                                }
                                if (licenseClass == "")
                                {
                                    licenseClass = dsDriverInfo.Tables[0].Rows[0]["Class"].ToString();
                                }
                                if (gender == "")
                                {
                                    gender = dsDriverInfo.Tables[0].Rows[0]["Gender"].ToString();
                                }
                                if (height == 0)
                                {
                                    short.TryParse(dsDriverInfo.Tables[0].Rows[0]["Height"].ToString(), out height);
                                }
                                if (homePhone == "")
                                {
                                    homePhone = dsDriverInfo.Tables[0].Rows[0]["HomePhone"].ToString();
                                }
                                if (cellPhone == "")
                                {
                                    cellPhone = dsDriverInfo.Tables[0].Rows[0]["CellPhone"].ToString();
                                }
                                if (additionalPhone == "")
                                {
                                    additionalPhone = dsDriverInfo.Tables[0].Rows[0]["AdditionalPhone"].ToString();
                                }
                                if (smsid == "")
                                {
                                    smsid = dsDriverInfo.Tables[0].Rows[0]["SMSID"].ToString();
                                }
                                if (smsPWD == "")
                                {
                                    smsPWD = dsDriverInfo.Tables[0].Rows[0]["SMSPwd"].ToString();
                                }
                                if (email == "")
                                {
                                    email = dsDriverInfo.Tables[0].Rows[0]["Email"].ToString();
                                }
                                if (address == "")
                                {
                                    address = dsDriverInfo.Tables[0].Rows[0]["Address"].ToString();
                                }
                                if (city == "")
                                {
                                    city = dsDriverInfo.Tables[0].Rows[0]["City"].ToString();
                                }
                                if (zipcode == "")
                                {
                                    zipcode = dsDriverInfo.Tables[0].Rows[0]["ZipCode"].ToString();
                                }
                                if (state == "")
                                {
                                    state = dsDriverInfo.Tables[0].Rows[0]["State"].ToString();
                                }
                                if (country == "")
                                {
                                    country = dsDriverInfo.Tables[0].Rows[0]["Country"].ToString();
                                }
                                if (description == "")
                                {
                                    description = dsDriverInfo.Tables[0].Rows[0]["Description"].ToString();
                                }
                                if (emergencyPhone == "")
                                {
                                    emergencyPhone = dsDriverInfo.Tables[0].Rows[0]["EmergencyPhone"].ToString();

                                    if (emergencyPhone != "")
                                       isAssignPanic = true;
                                }
                                        if (licenseIssued == DateTime.MinValue && dsDriverInfo.Tables[0].Rows[0]["LicenseIssued"] != DBNull.Value)
                                {
                                    licenseIssued = DateTime.Parse(dsDriverInfo.Tables[0].Rows[0]["LicenseIssued"].ToString());
                                        }
                                    if (licenseIssued == DateTime.MinValue)
                                    {
                                        licenseIssued = new DateTime(2010, 01, 01);
                                    }


                                        if (licenseExpired == DateTime.MinValue && dsDriverInfo.Tables[0].Rows[0]["LicenseExpired"] != DBNull.Value)
                                {
                                    licenseExpired = DateTime.Parse(dsDriverInfo.Tables[0].Rows[0]["LicenseExpired"].ToString());
                                        }
                                    if (licenseExpired == DateTime.MinValue)
                                    {
                                        licenseExpired = new DateTime(2050, 01, 01);
                                    }


                                        if (terminationDate == DateTime.MinValue && dsDriverInfo.Tables[0].Rows[0]["TerminationDate"] != DBNull.Value)
                                {
                                    terminationDate = DateTime.Parse(dsDriverInfo.Tables[0].Rows[0]["TerminationDate"].ToString());
                                        }
                                    if (terminationDate == DateTime.MinValue)
                                    {
                                        terminationDate = new DateTime(2050, 01, 01);
                                    }


                                if (contactPlanId == null)
                                {
                                   if (dsDriverInfo.Tables[0].Rows[0]["ContactPlanId"].ToString() != "")
                                    contactPlanId = int.Parse(dsDriverInfo.Tables[0].Rows[0]["ContactPlanId"].ToString());
                                }                                
                                if (keyFobId == "")
                                {
                                    keyFobId = dsDriverInfo.Tables[0].Rows[0]["KeyFobId"].ToString();
                                }
                                        if (caCycle == 0 && dsDriverInfo.Tables[0].Rows[0]["CACycle"] != DBNull.Value)
                                {
                                    short.TryParse(dsDriverInfo.Tables[0].Rows[0]["CACycle"].ToString(), out caCycle);
                                }
                                        if (usaCycle == 0 && dsDriverInfo.Tables[0].Rows[0]["USCycle"] != DBNull.Value)
                                {
                                    short.TryParse(dsDriverInfo.Tables[0].Rows[0]["USCycle"].ToString(), out usaCycle);
                                }
                                        if (isSupervisor == false && dsDriverInfo.Tables[0].Rows[0]["IsSupervisor"] != DBNull.Value)
                                {
                                    isSupervisor = Convert.ToBoolean(dsDriverInfo.Tables[0].Rows[0]["IsSupervisor"].ToString());
                                }
                                if (positionInfo == "")
                                {
                                    positionInfo = dsDriverInfo.Tables[0].Rows[0]["PositionInfo"].ToString();
                                }
                                        /*if (terminationDate == DateTime.MinValue && dsDriverInfo.Tables[0].Rows[0]["TerminationDate"] != DBNull.Value)
                                {
                                    DateTime dbTerminationDate = DateTime.Parse(dsDriverInfo.Tables[0].Rows[0]["TerminationDate"].ToString());
                                    terminationDate = dbTerminationDate;
                                        }*/

                                        if (timeZone == 0.0 && dsDriverInfo.Tables[0].Rows[0]["timezone"] != DBNull.Value)
                                {
                                    timeZone = float.Parse(dsDriverInfo.Tables[0].Rows[0]["timezone"].ToString());
                                }

                                contactMsg.DriverAndContactPlanCycleUpdate(drvId, firstName, lastName, license, licenseClass,
                                licenseIssued, licenseExpired, organizationId, gender[0], height, homePhone, cellPhone,
                                additionalPhone, smsPWD, smsid, email, address, city, zipcode, state, country,
                                description, emergencyPhone, contactPlanId, isAssignPanic, keyFobId,
                                caCycle, usaCycle, isSupervisor, positionInfo, terminationDate, timeZone);


                                vdbu.RecordUserAction("Driver", sn.UserID, 0, "vlfDriver",
                                                         string.Format("DriverId={0}", Convert.ToInt32(ViewState["DriverID"])),
                                                         "Update driver", this.Context.Request.UserHostAddress,
                                                         this.Context.Request.RawUrl,
                                                         string.Format("Update driver({0})", Convert.ToInt32(ViewState["DriverID"])));
                            }
                        }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                          VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                          ex.Message.ToString() + " Update driver error for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));                                //RedirectToLogin();

                        //RedirectToLogin();
                        lblMessage.Text = this.GetLocalResourceObject("resUpdateDriverError").ToString();
                        return;
                    }
                }



                else // assignments
                {
                    # region Parse excel file

                    DataSet dsAssgn = new DataSet("Assignments");
                    DataTable dtAssgn = dsAssgn.Tables.Add("vlfDriverAssignment");
                    dtAssgn.Columns.Add("ID", typeof(int));
                    dtAssgn.Columns.Add("FirstName", typeof(string));
                    dtAssgn.Columns.Add("LastName", typeof(string));
                    dtAssgn.Columns.Add("Description", typeof(string));
                    dtAssgn.Columns.Add("AssignedDate", typeof(string));
                    dtAssgn.Columns.Add("Comments", typeof(string));

                    // get data into dataset
                    using (OleDbConnection conn = new OleDbConnection(connString))
                    {
                        OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", conn);
                        da.Fill(dtAssgn);
                    }

                    # endregion

                    # region Send data to db

                    xmlData = dsAssgn.GetXml();
                    using (ServerDBDriver.DBDriver driver = new ServerDBDriver.DBDriver())
                    {
                        if (objUtil.ErrCheck(driver.AssignDrivers(sn.UserID, sn.SecId, xmlData), false))
                        {
                            if (objUtil.ErrCheck(driver.AssignDrivers(sn.UserID, sn.SecId, xmlData), true))
                            {
                                base.ShowMessage(this.lblMessage, base.GetLocalResourceObject("resErrorUploadingDriversAssignList").ToString(), Color.Red);
                            }
                        }
                        else
                            base.ShowMessage(this.lblMessage, base.GetLocalResourceObject("resDriversAssignUploadSuccess").ToString(), Color.Green);
                    }

                    # endregion
                }

                // load drivers
                LoadDrivers(true);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                //lblMessage.Text = Ex.Message;
                //RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //lblMessage.Text = Ex.Message;
                //RedirectToLogin();
            }
        }

        protected void cmdImportAssignments_Click(object sender, EventArgs e)
        {
            ImportSelect("2");
        }

        protected void cmdDownloadDrivers_Click(object sender, EventArgs e)
        {
            try
            {
                DataSet dsDrivers = new DataSet();
                dsDrivers = GetDrivers();
                DrvAssgn2Excel(dsDrivers);
                Drv2Excel(dsDrivers);
                SetDownloadLink(this.linkDriverAssignmentList, this.drvAssgnUrl);
                SetDownloadLink(this.linkDriverList, this.drvUrl);
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                   VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                   VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

        }
        protected void txtSMSPWD_PreRender(object sender, EventArgs e)
        {
            txtSMSPWD.Attributes["value"] = txtSMSPWD.Text;
        }
        protected void cmdFleets_Click(object sender, EventArgs e)
        {

        }
        protected void cmdScheduledTasks_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmTaskScheduler.aspx");
        }
        protected void gdvDrivers_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
        {
            LoadDrivers(false);
            //RadAjaxManager1.ResponseScripts.Add("aa();");
        }
        protected void gdvDrivers_ItemDataBound(object sender, Telerik.Web.UI.GridItemEventArgs e)
        {
            ImageButton cmdBtn = (ImageButton)e.Item.FindControl("btnDeleteDriver");
            if (cmdBtn != null)
            {
                string confirm = "return confirm('" + (string)base.GetLocalResourceObject("ConfirmDelete") + "')";
                if (cmdBtn.CommandName == "DeleteDriver")
                    cmdBtn.Attributes.Add("onclick", confirm);
            }

            LinkButton lnkEmergency = (LinkButton)e.Item.FindControl("lnkEmergency");
            if (lnkEmergency != null)
            {
                DataRowView drv = (DataRowView)e.Item.DataItem;
                if (drv != null)
                {
                    if (drv["DriverContactPlanId"] != DBNull.Value)
                    {
                        lnkEmergency.Visible = true;
                        string cp = "";
                        if (drv["DriverId"] != DBNull.Value) cp = drv["DriverId"].ToString();
                        lnkEmergency.OnClientClick = "javascript:return OpenEmergency('" + cp + "');";
                    }
                    else lnkEmergency.Visible = false;
                }
            }
        }
        protected void gdvDrivers_ItemCommand(object sender, Telerik.Web.UI.GridCommandEventArgs e)
        {
            if (e.CommandName == "EditDriver")
            {
                try
                {
                    tblViewDrivers.Visible = false;
                    tblDriverDetails.Visible = true;
                    lblMessage.Text = "";

                    // get data
                    if (((GridDataItem)e.Item).GetDataKeyValue("DriverId") == null) return;
                    Int32 dataKey = Int32.Parse(((GridDataItem)e.Item).GetDataKeyValue("DriverId").ToString());

                    if (ViewState["DriverID"] != null) ViewState.Remove("DriverID");
                    ViewState.Add("DriverID", dataKey.ToString());

                    DataSet dsDriver = contactMsg.GetDriverByDriverID(dataKey);
                    // fill controls
                    if (VLF.CLS.Util.IsDataSetValid(dsDriver))
                        SetFields(dsDriver.Tables[0].Rows[0]);
                }
                catch (NullReferenceException Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                    //lblMessage.Text = Ex.Message;
                    RedirectToLogin();
                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    //lblMessage.Text = Ex.Message;

                }
            }
            if (e.CommandName == "DeleteDriver")
            {
                try
                {
                    if (((GridDataItem)e.Item).GetDataKeyValue("DriverId") == null) return;
                    Int32 dataKey = Int32.Parse(((GridDataItem)e.Item).GetDataKeyValue("DriverId").ToString());

                    contactMsg.DriverAndContactPlanDelete(dataKey);
                    ServerDBUser.DBUser vdbu = new ServerDBUser.DBUser();
                    vdbu.RecordUserAction("Driver", sn.UserID, 0, "vlfDriver",
                                                 string.Format("DriverId={0}", dataKey),
                                                 "Delete driver", this.Context.Request.UserHostAddress,
                                                 this.Context.Request.RawUrl,
                                                 string.Format("Delete driver({0})", dataKey));
                    LoadDrivers(true);
                }
                catch (NullReferenceException Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                       " Delete driver error for User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));

                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                    //lblMessage.Text = Ex.Message;
                    RedirectToLogin();
                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    //lblMessage.Text = Ex.Message;
                }

            }
        }

        //Salman (Aug 19, 2014; Mantis# 6210)
        DateTime FormatedDate(string iDateTime)
        {
            DateTime d_t;
            string l_profile = (sn.SelectedLanguage.Contains("fr-") ? "fr-fr" : "en-us");

            //System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo(l_profile);
            System.Globalization.DateTimeFormatInfo dtfi = new System.Globalization.CultureInfo(l_profile, true).DateTimeFormat;
            dtfi.DateSeparator = "/";

            DateTime.TryParseExact(iDateTime.Replace("-", "/"), "d", dtfi, System.Globalization.DateTimeStyles.None, out d_t);

            //Field2.Text = d_t.ToString(ci);
            d_t = Convert.ToDateTime(d_t, dtfi);

            return d_t;
        }
    }
}
