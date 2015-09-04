using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Telerik.Web.UI;
namespace SentinelFM
{
    public partial class HOS_frmQuestion : SentinelFMBasePage
    {
        string errorInsert = "Failed to saved.";
        string errorDelete = "Failed to delete.";
        string confirmstring = "The question has been assigned to question set, are you sure you want to delete it?";
        string confirmstring_1 = "Are you sure you want to delete this question?";
        public string showFilter = "Show Filter";
        public string hideFilter = "Hide Filter";
        public string msgMajor = "Major";
        public string msgMinor = "Minor";
        public string descriptionisrequired = "Description is required.";
        public string msgSMCSisrequired = "SMCS is required.";
        clsHOSManager hosManager = new clsHOSManager();
        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                {
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../../Login.aspx','_top')</SCRIPT>", false);
                }
                if (!IsPostBack)
                {
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref form1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                    GuiSecurity(this);
                    BindSMCS();
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

        private void BindSMCS()
        {
            cboSMCS.DataSource = hosManager.GetSMCSByOrganizationId(sn.User.OrganizationId);
            cboSMCS.DataBind();
        }
        protected void gdMedia_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            BindQuestions(false);
        }

        protected void gdMedia_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            if (((GridDataItem)e.Item).GetDataKeyValue("RowID") != null)
            {
                try
                {
                    int rowID = int.Parse(((GridDataItem)e.Item).GetDataKeyValue("RowID").ToString());
                    hosManager.DeleteLogdata_Question(rowID, sn.User.OrganizationId);
                    BindQuestions(true);
                }
                catch (Exception Ex)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                    System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                    ExceptionLogger(trace);
                    e.Canceled = true;
                    string errorMsg = string.Format("alert(\"{0}\");", errorDelete);
                    RadAjaxManager1.ResponseScripts.Add(errorMsg);
                }
            }
        }
        protected void gdMedia_ItemDataBound(object sender, GridItemEventArgs e)
        {

            if (e.Item is GridCommandItem)
            { 
                ((Button)e.Item.FindControl("btnAdd")).Attributes.Add("tag", "dynamicInspec");

            }
            if (e.Item is GridDataItem)
            {
                GridDataItem dataItem = e.Item as GridDataItem;
                ImageButton deletebutton = dataItem["DeleteColumn"].Controls[0] as ImageButton;
                ImageButton bntEdit = (ImageButton)dataItem.FindControl("bntEdit");
                Label lblDefectLevel = (Label)dataItem.FindControl("lblDefectLevel");

                string rowID = ((DataRowView)e.Item.DataItem)["RowID"].ToString();
                string defectLevel= ((DataRowView)e.Item.DataItem)["DefectLevel"].ToString();
                Label lblSMCSCode = (Label)dataItem.FindControl("lblSMCSCode");
                Label lblDefect = (Label)dataItem.FindControl("lblDefect");
                if (((DataRowView)e.Item.DataItem)["isUsed"].ToString().ToLower() == "false")
                {
                    deletebutton.Attributes["onclick"] = string.Format("return confirm(\"{0}\");", confirmstring_1);
                }
                else
                {
                    deletebutton.Attributes["onclick"] = string.Format("return confirm(\"{0}\");", confirmstring);
                }

                bntEdit.OnClientClick = String.Format("javascript:return editQuestion('{0}','{1}','{2}','{3}')",
                       rowID, lblDefect.ClientID, lblSMCSCode.ClientID, defectLevel);
                //lblDefectLevel.Text = defectLevel;
                /*if (defectLevel == "1")
                {
                    //((DataRowView)e.Item.DataItem)["DefectLevel"] = msgMajor;
                    lblDefectLevel.Text = msgMajor;
                }
                if (defectLevel == "0") {
                   // ((DataRowView)e.Item.DataItem)["DefectLevel"] = msgMinor;
                    lblDefectLevel.Text = msgMinor;
                }*/
                deletebutton.Attributes.Add("tag", "dynamicInspec");
                bntEdit.Attributes.Add("tag", "dynamicInspec");
            }
        }
        protected void btnaddQuestionAction_Click(object sender, EventArgs e)
        {
            try
            {
                string defect = txtDescription.Text.Trim();
                string SMCScode = cboSMCS.SelectedValue;
                int defectLevel = int.Parse(optDefects.SelectedValue);
                int rowid = -1;
                int.TryParse(hidCurrentRowId.Value, out rowid);
                hosManager.AddOrUpdateLogdata_Question(rowid, defect, defectLevel, SMCScode, sn.User.OrganizationId);
                cboSMCS.ClearSelection();
                cboSMCS.SelectedIndex = 0;
                txtDescription.Text = "";
                optDefects.SelectedValue = "0";
                BindQuestions(true);
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);
                string errorMsg = string.Format("alert(\"{0}\");", errorInsert);
                RadAjaxManager1.ResponseScripts.Add(errorMsg);
            }
        }

        void BindQuestions(Boolean isBind)
        {
            DataTable dt = hosManager.GetLogdata_Question(sn.User.OrganizationId);
            dt.Columns.Add("DefectLevelDesc");
            foreach (DataRow dr in dt.Rows)
            {
                if (dr["DefectLevel"].ToString() == "1")
                {
                    dr["DefectLevelDesc"] = msgMajor;
                }
                if (dr["DefectLevel"].ToString() == "0")
                {
                    dr["DefectLevelDesc"] = msgMinor;
                }
            }
            gdMedia.DataSource = dt;
            if (isBind) gdMedia.DataBind();
        }
        protected void gdMedia_ItemCommand(object sender, GridCommandEventArgs e)
        {
            /*if (e.CommandName == RadGrid.FilterCommandName)
            {
                Pair filterPair = (Pair)e.CommandArgument;
                
                TextBox filterBox = (e.Item as GridFilteringItem)[filterPair.Second.ToString()].Controls[0] as TextBox;
                if (filterBox.Text == msgMajor) filterBox.Text = "1";
                if (filterBox.Text == msgMinor) filterBox.Text = "0";
            }*/
        }
}
}