using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.Services;
using System.Web.UI.WebControls;
using SentinelFM;
using System.Configuration;
using Telerik.Web.UI;
using System.Data;
using System.Web.Script.Serialization;

public partial class HOS_frmDynamicInspections : SentinelFMBasePage
{

    public string msgNoQuestionSelected = "No question selected";
    string confirmstring = "The question form has been assigned to vehicles(s), are you sure you want to delete it?";
    string confirmstring_1 = "Are you sure you want to delete this question form?";
    public string msgConfirmDelete = "Are you sure you want to delect selected question?";
    public string msgNameIsRequired = "Question form name is required.";
    public string msgSelectaQuestion = "Please select a question.";
    //public string msgDescriptionRequired = "Description is required.";
    //public string msgSMCSRequired = "SMCS is required.";
    public string msgQuestionSet = "Question Form";
    public string msgFailedtoSave = "Failed to save";
    public string msgSaveSuccessfully = "Saved successfully";
    public string msgMajor = "Major";
    public string msgMinor = "Minor";
    public string msgSMCS = "SMCS";
    public string msgErrorInsert = "Failed to saved.";
    public string msgErrorDelete = "Failed to delete.";

    public string showFilter = "Show Filter";
    public string hideFilter = "Hide Filter";
    public string inspectionJson = "";
    public string categoryJson = "";
    public string msgQuestionSetLevel = "Only allow input {0} levels.";
    public string msgQuestionBeenUsed = "The question has been used.";
    clsHOSManager hosManager = new clsHOSManager();

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../Login.aspx','_top')</SCRIPT>", false);
            }
            if (!IsPostBack)
            {
                LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref form1, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
                GuiSecurity(this);
                btnClose.Attributes.Add("tag", "dynamicInspec");
                btnSaveName.Attributes.Add("tag", "dynamicInspec");
                btnAddQuestion.Attributes.Add("tag", "dynamicInspec");
                btnEditQuestion.Attributes.Add("tag", "dynamicInspec");
                btnDeleteQuestion.Attributes.Add("tag", "dynamicInspec");
                hidQuestionSetLevel.Value = hosManager.GetCompanyConfigurationMobile(sn.User.OrganizationId, clsHOSManager.QuestionSetlevel).ToString();
            }
            chkScannable.Attributes.Add("onclick", "OnchkScannableClicked(this)");
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

        msgQuestionSetLevel = string.Format(msgQuestionSetLevel, hidQuestionSetLevel.Value); 
    }

    protected void gdMedia_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
    {
        if (gdMedia.Visible )
           BindQuestionsSet(false);
    }

    protected void gdMedia_DeleteCommand(object sender, GridCommandEventArgs e)
    {
        if (((GridDataItem)e.Item).GetDataKeyValue("GroupId") != null)
        {
            try
            {
                int groupId = int.Parse(((GridDataItem)e.Item).GetDataKeyValue("GroupId").ToString());
                hosManager.DeleteLogData_InspectionGroup(groupId, sn.User.OrganizationId);
                BindQuestionsSet(true);
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                ExceptionLogger(trace);
                e.Canceled = true;
                string errorMsg = string.Format("alert(\"{0}\");", msgErrorDelete);
                RadAjaxManager1.ResponseScripts.Add(errorMsg);
            }
        }
    }
    protected void gdMedia_ItemDataBound(object sender, GridItemEventArgs e)
    {
        if (e.Item is GridDataItem)
        {
            GridDataItem dataItem = e.Item as GridDataItem;                          
            LinkButton deletebutton =(LinkButton)dataItem.FindControl("btnDelete");
            LinkButton btnEdit = (LinkButton)dataItem.FindControl("btnEdit");

            string groupId = ((DataRowView)e.Item.DataItem)["GroupId"].ToString();
            if (((DataRowView)e.Item.DataItem)["isUsed"].ToString().ToLower() == "false")
            {
                deletebutton.Attributes["onclick"] = string.Format("return confirm(\"{0}\");", confirmstring_1);
            }
            else
            {
                deletebutton.Attributes["onclick"] = string.Format("return confirm(\"{0}\");", confirmstring);
            }

        }
    }

    void BindQuestionsSet(Boolean isBind)
    {
        gdMedia.DataSource = hosManager.GetLogData_InspectionGroup(sn.User.OrganizationId);
        if (isBind) gdMedia.DataBind();
    }
    protected void btnAdd_Click(object sender, EventArgs e)
    {
        pnlQuestionSet.Visible = true;
        gdMedia.Visible = false;
        lblErrorMessage.Text = "";
        hidGroupId.Value = "";
        txtName.Text = "";
        BindQuestions();
    }
    void BindQuestions()
    {
        if (cboQuestions.Items.Count <= 1)
        {

            cboQuestions.DataSource = hosManager.GetLogdata_Question(sn.User.OrganizationId);
            cboQuestions.DataBind();
            
        }
        cboQuestions.SelectedIndex = -1;
    }

    protected void btnClose_Click(object sender, EventArgs e)
    {
        pnlQuestionSet.Visible = false;
        gdMedia.Visible = true;
        BindQuestionsSet(true);
    }
    [WebMethod]
    public static string AddOrUpdateInspectionGroup(int groupId, string name, int type)
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else return "-1";
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

        try
        {
            clsHOSManager hosManager = new clsHOSManager();
            groupId = hosManager.AddOrUpdateLogData_InspectionGroup(groupId, name, type, sn.User.OrganizationId);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: AddOrUpdateInspectionGroup() Page:frmDynamicInspections"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            return "0";
        }
        return groupId.ToString();
    }

    [WebMethod]
    public static string AddOrUpdateLogData_InspectionGroupItem(int groupId, int categoryid, int inspectionitemId, int parentItemId, int questionID, Boolean isCategory, string path,
        Boolean scannable,  string location)
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else return "-1";
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

        try
        {
            if (groupId <= 0) return "0";
            clsHOSManager hosManager = new clsHOSManager();
            Dictionary<string, string> dt = hosManager.AddOrUpdateLogData_InspectionGroupItem(groupId, categoryid, inspectionitemId, parentItemId, questionID, isCategory, sn.User.OrganizationId, path,
                scannable, location);
            if (dt.Count == 0) return "0";
            else
            {
                JavaScriptSerializer js = new JavaScriptSerializer();
                return js.Serialize(dt);
            }

        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: AddOrUpdateInspectionGroup() Page:frmDynamicInspections"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            return "0";
        }
        return "0";

    }

    [WebMethod]
    public static string DeleteLogData_InspectionGroupItem(int groupId, int categoryid, int inspectionitemId, Boolean isCategory)
    {
        SentinelFMSession sn;
        if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        else return "-1";
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return "-1";

        try
        {
            if (groupId <= 0) return "0";
            clsHOSManager hosManager = new clsHOSManager();
            hosManager.DeleteLogData_InspectionGroupItem(groupId, categoryid, inspectionitemId, isCategory, sn.User.OrganizationId);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + "Web method: AddOrUpdateInspectionGroup() Page:frmDynamicInspections"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            return "0";
        }
        return "1";
    }

    protected void gdMedia_ItemCommand(object sender, GridCommandEventArgs e)
    {
        List<Dictionary<string, string>> inspections = new List<Dictionary<string, string>>();
        List<Dictionary<string, string>> categorys = new List<Dictionary<string, string>>();
        if (e.CommandName == "Edit")
        {
            string scannable = "0";
            string location = "";
            string groupId = ((GridDataItem)e.Item).GetDataKeyValue("GroupId").ToString();
            if (int.Parse(groupId) > 0)
            {
                hidGroupId.Value = groupId;
                DataSet ds = hosManager.GetLogData_InspectionGroupItem(int.Parse(groupId), sn.User.OrganizationId);
                txtName.Text = ds.Tables[0].Rows[0]["Name"].ToString();
                radTypeList.SelectedIndex = int.Parse(ds.Tables[0].Rows[0]["Type"].ToString());
                foreach (DataRow dr in ds.Tables[1].Rows)
                {
                    /*if (dr["Scannable"] != DBNull.Value && ((Boolean)dr["Scannable"]))
                    {
                        scanStr = string.Format("<img src='../images/Camera.ico' />");
                    }*/

                    Dictionary<string, string> category = new Dictionary<string,string>();
                    category.Add("ID", dr["CategoryID"].ToString());
                    String desc = String.Format("{0} " + msgSMCS + ":{1} {2}", dr["Category"] is DBNull ? "":dr["Category"].ToString(), dr["SMCSCode"] is DBNull ?"":dr["SMCSCode"].ToString(),  (dr["DefectLevel"] is DBNull || dr["DefectLevel"].ToString() == "0")?msgMinor:msgMajor);
                    category.Add("Defect", desc);

                    scannable = "0";
                    location = "";
                    if (dr["Scannable"] != DBNull.Value && ((Boolean)dr["Scannable"]))
                    {
                        scannable = "1";
                        if (dr["Location"] != DBNull.Value )
                            location = dr["Location"].ToString();
                    }
                    category.Add("Scannable", scannable);
                    category.Add("Location", location);
                    category.Add("QuestionID", dr["QuestionID"].ToString());
                    categorys.Add(category);
                }

                foreach (DataRow dr in ds.Tables[2].Rows)
                {
                    Dictionary<string, string> inspection = new Dictionary<string, string>();
                    inspection.Add("ID", dr["InspectionItemID"].ToString());
                    String desc = String.Format("{0} " + msgSMCS + ":{1} {2}", dr["Defect"] is DBNull ? "" : dr["Defect"].ToString(), dr["SMCSCode"] is DBNull ? "" : dr["SMCSCode"].ToString(), (dr["DefectLevel"] is DBNull || dr["DefectLevel"].ToString() == "0") ? msgMinor : msgMajor);
                    inspection.Add("Defect", desc);
                    string parentItemID = "";
                    if (!(dr["ParentItemID"] is DBNull)) parentItemID = dr["ParentItemID"].ToString();
                    string  categoryId= "";
                    if (!(dr["CategoryID"] is DBNull)) categoryId = dr["CategoryID"].ToString();
                    inspection.Add("ParentItemID", parentItemID);
                    inspection.Add("CategoryID", categoryId);

                    scannable = "0";
                    location = "";
                    if (dr["Scannable"] != DBNull.Value && ((Boolean)dr["Scannable"]))
                    {
                        scannable = "1";
                        if (dr["Location"] != DBNull.Value)
                            location = dr["Location"].ToString();
                    }
                    inspection.Add("Scannable", scannable);
                    inspection.Add("Location", location);
                    inspection.Add("QuestionID", dr["QuestionID"].ToString());
                    inspections.Add(inspection);
                }
                JavaScriptSerializer js = new JavaScriptSerializer();
                categoryJson = js.Serialize(categorys);
                inspectionJson = js.Serialize(inspections);

                pnlQuestionSet.Visible = true;
                gdMedia.Visible = false;
                lblErrorMessage.Text = "";
                BindQuestions();
                string javascriptStr = string.Format("var var_c={0};  var var_i={1}; showQuestionSet(var_c, var_i) ", categoryJson, inspectionJson);
                RadAjaxManager1.ResponseScripts.Add(javascriptStr);
                e.Canceled = true;
            }
        }
    }

}