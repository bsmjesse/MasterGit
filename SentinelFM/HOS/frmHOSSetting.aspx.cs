using DocumentFormat.OpenXml.Drawing.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
namespace SentinelFM
{
    public partial class HOS_frmHOSSetting : SentinelFMBasePage
    {
        enum Settings
        {
            Login = 1,
            ScanBarcode = 2,
            InspectionHistoryDay = 3,
            InspectionHistoryAmount = 4,
            ImagesLimit = 5,
            EngineHour = 6,
            QuestionSetLevel = 7,
            ViolationThreshold = 8,
            SearchType = 9,
            ScreenLock = 10,
            OdoInput = 11,
            DriverPendingList = 12
        }
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (!Page.IsPostBack)
                {
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref form1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    GuiSecurity(this);

                    LoadValues();
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
                   Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                //lblMessage.Text = Ex.Message;

            }
            lblError.Text = string.Empty;
        }

        private void LoadValues()
        {
            clsHOSManager hosManager = new clsHOSManager();
            System.Data.DataTable da = hosManager.GetCompanyConfigurationMobileByOrgId(sn.User.OrganizationId);
            if (da != null && da.Rows.Count > 0)
            {
                foreach (DataRow dr in da.Rows)
                {
                    switch (int.Parse(dr["ConfigurationId"].ToString()))
                    {
                        case (int)Settings.Login:
                            foreach (ListItem item in radDefault.Items)
                            {
                                if (item.Value.ToLower() == dr["Value"].ToString().ToLower())
                                {
                                    item.Selected = true;
                                    break;
                                }
                            }
                            break;
                        case (int)Settings.InspectionHistoryDay:
                            txtInspectionDay.Text = dr["Value"].ToString();
                            break;
                        case (int)Settings.InspectionHistoryAmount:
                            txtInspectionAmount.Text = dr["Value"].ToString();
                            break;
                        case (int)Settings.ImagesLimit:
                            txtImageLimit.Text = dr["Value"].ToString();
                            break;
                        case (int)Settings.QuestionSetLevel:
                            txtQuestionLevel.Text = dr["Value"].ToString();
                            break;

                        case (int)Settings.ViolationThreshold:
                            txtViolationThreshold.Text = dr["Value"].ToString();
                            break;
                        case (int)Settings.ScreenLock:
                            if (dr["Value"].ToString().ToLower() == "true")
                                radScreenLock.Items.FindByValue("1").Selected = true;
                            if (dr["Value"].ToString().ToLower() == "false")
                                radScreenLock.Items.FindByValue("0").Selected = true;
                            break;

                        case (int)Settings.OdoInput:
                            if (dr["Value"].ToString().ToLower() == "true")
                                radInputOdometer.Items.FindByValue("1").Selected = true;
                            if (dr["Value"].ToString().ToLower() == "false")
                                radInputOdometer.Items.FindByValue("0").Selected = true;
                            break;
                        case (int)Settings.ScanBarcode:
                            if (dr["Value"].ToString().ToLower() == "true")
                                radScanBoardCode.Items.FindByValue("1").Selected = true;
                            if (dr["Value"].ToString().ToLower() == "false")
                                radScanBoardCode.Items.FindByValue("0").Selected = true;
                            break;

                        case (int)Settings.DriverPendingList:
                            txtPendingManualLog.Text = dr["Value"].ToString();
                            break;


                    }
                }
            }

        }
        protected void cmdSave_Click(object sender, EventArgs e)
        {
            try
            {
                    if (!Page.IsValid) { lblError.Text = "Failed to save. Please enter valid data.";lblError.ForeColor = System.Drawing.Color.Red;return;}

                clsHOSManager hosManager = new clsHOSManager();
                foreach (ListItem item in radDefault.Items)
                {
                    if (item.Selected)
                    {
                        hosManager.AddorUpdateCompanyConfigurationMobile(sn.User.OrganizationId, (int)Settings.Login, item.Value);
                        break;
                    }
                }

                if (txtInspectionDay.Text.Trim() != "")
                {
                    int inspectionDay = 0;
                    int.TryParse(txtInspectionDay.Text.Trim(), out inspectionDay);
                    if (inspectionDay > 0)
                        hosManager.AddorUpdateCompanyConfigurationMobile(sn.User.OrganizationId, (int)Settings.InspectionHistoryDay, inspectionDay.ToString());
                }

                if (txtInspectionAmount.Text.Trim() != "")
                {
                    int inspectionAmount = 0;
                    int.TryParse(txtInspectionAmount.Text.Trim(), out inspectionAmount);
                    if (inspectionAmount > 0)
                        hosManager.AddorUpdateCompanyConfigurationMobile(sn.User.OrganizationId, (int)Settings.InspectionHistoryAmount, inspectionAmount.ToString());
                }

                if (txtImageLimit.Text.Trim() != "")
                {
                    int imagesLimit = 0;
                    int.TryParse(txtImageLimit.Text.Trim(), out imagesLimit);
                    if (imagesLimit > 0)
                        hosManager.AddorUpdateCompanyConfigurationMobile(sn.User.OrganizationId, (int)Settings.ImagesLimit, imagesLimit.ToString());
                }

                if (txtQuestionLevel.Text.Trim() != "")
                {
                    int questionLevel = 0;
                    int.TryParse(txtQuestionLevel.Text.Trim(), out questionLevel);
                    if (questionLevel > 0)
                        hosManager.AddorUpdateCompanyConfigurationMobile(sn.User.OrganizationId, (int)Settings.QuestionSetLevel, questionLevel.ToString());
                }

                if (txtViolationThreshold.Text.Trim() != "")
                {
                    int violationThreshold = 0;
                    int.TryParse(txtViolationThreshold.Text.Trim(), out violationThreshold);
                    if (violationThreshold > 0)
                        hosManager.AddorUpdateCompanyConfigurationMobile(sn.User.OrganizationId, (int)Settings.ViolationThreshold, violationThreshold.ToString());
                }

                if (radScreenLock.SelectedIndex >= 0)
                {
                    string selectTxt = "false";
                    if (radScreenLock.SelectedValue == "1") selectTxt = "true";
                    hosManager.AddorUpdateCompanyConfigurationMobile(sn.User.OrganizationId, (int)Settings.ScreenLock, selectTxt);
                }

                if (radInputOdometer.SelectedIndex >= 0)
                {
                    string selectTxt = "false";
                    if (radInputOdometer.SelectedValue == "1") selectTxt = "true";
                    hosManager.AddorUpdateCompanyConfigurationMobile(sn.User.OrganizationId, (int)Settings.OdoInput, selectTxt);
                }

                if (radScanBoardCode.SelectedIndex >= 0)
                {
                    string selectTxt = "false";
                    if (radScanBoardCode.SelectedValue == "1") selectTxt = "true";
                    hosManager.AddorUpdateCompanyConfigurationMobile(sn.User.OrganizationId, (int)Settings.ScanBarcode, selectTxt);
                }

                if (txtPendingManualLog.Text.Trim() != "")
                {
                    float pendingManualLog = 0;
                    float.TryParse(txtPendingManualLog.Text.Trim(), out pendingManualLog);
                    if (pendingManualLog > 0)
                        hosManager.AddorUpdateCompanyConfigurationMobile(sn.User.OrganizationId, (int)Settings.DriverPendingList, pendingManualLog.ToString());
                }


                ServerDBUser.DBUser vdbu = new ServerDBUser.DBUser();
                vdbu.RecordUserAction("HOS Setting", sn.UserID, 0, "CompanyConfigurationMobile",
                                                null,
                                                "Change HOS Setting", this.Context.Request.UserHostAddress,
                                                this.Context.Request.RawUrl,
                                                string.Format("OrganizationId='{0}'", sn.User.OrganizationId));
                lblError.Text = "Saved successfully";
                lblError.ForeColor = System.Drawing.Color.Blue;
            }
            catch (Exception ex)
            {
                lblError.Text = "Failed to save.";
                lblError.ForeColor = System.Drawing.Color.Red;
            }
        }
    }
}