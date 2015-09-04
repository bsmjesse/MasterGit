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
using System.Drawing;
using System.Text;
using VLF.CLS;
using System.IO;
using System.Globalization;
using Telerik.Web.UI;
using VLF.DAS.Logic;
using System.Web.Services;
using System.Collections.Generic;
using System.Web.Script.Serialization;
using SentinelFM;


public partial class Maintenance_AddServiceTypes : System.Web.UI.UserControl
{
    public string errorLoad = "Failed to load data.";
    public string errorSave = "Save Failed.";
    public string errorDelete = "Delete Failed.";
    public string selectService = "Plase select a service.";

    public string notiTxt = "Notification Type: ";
    public string n1Txt = "Notification1: ";
    public string n2Txt = "Notification2: ";
    public string n3Txt = "Notification3: ";
    public string freqTxt = "Frequency ID:";
    public string inteTxt = "inteTxt";
    public string assignText = "Assign";
    public string unAssignText = "Unassign";
    string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
    SentinelFMSession sn = null;
    string operationStr = null;
    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            errorLoad = HttpContext.GetGlobalResourceObject("Const", "Error_Load").ToString();
            errorSave = HttpContext.GetGlobalResourceObject("Const", "Error_Save").ToString();
            errorDelete = HttpContext.GetGlobalResourceObject("Const", "Error_Delete").ToString();
        }
        catch { }

        gvServicesSource.ClientSettings.Selecting.AllowRowSelect = false;
        gvServicesSource.MasterTableView.ItemStyle.VerticalAlign = VerticalAlign.Top;
        gvServicesDest.ClientSettings.Selecting.AllowRowSelect = false;
        gvServicesDest.MasterTableView.ItemStyle.VerticalAlign = VerticalAlign.Top;


        GetSn();
    }

    private void GetSn()
    {
         if (Session["SentinelFMSession"] == null)
        {
            ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "loginScriptEquip", "<SCRIPT Language='javascript'>window.open('../Login.aspx','_top')</SCRIPT>", false);
            return;
        }

        if (sn == null)
         sn = (SentinelFMSession)Session["SentinelFMSession"];

    }
    public void SetGridAjaxManager(RadAjaxManager RadAjaxManager1)
    {
        gvServicesSource.RadAjaxManagerControl = RadAjaxManager1;
        gvServicesDest.RadAjaxManagerControl = RadAjaxManager1;
        gvServicesDest.Visible = true;
        gvServicesSource.Visible = true;
    }

    public string GetResizeScript()
    {
        SentinelFMBasePage basePage = new SentinelFMBasePage();
        return basePage.SetResizeScript(gvServicesSource.ClientID) + basePage.SetResizeScript(gvServicesDest.ClientID);
    }

    public void BindgvServicesSource(Boolean isBind)
    {
        if (!string.IsNullOrEmpty(hidMccID.Value))
        {
            MCCManager mccMg = new MCCManager(sConnectionString);
            if (string.IsNullOrEmpty(operationStr)) operationStr = SentinelFM.clsAsynGenerateReport.GetOperationTypeStr();
            DataTable dt = mccMg.GetMCCMaintenanceUnAssigment(sn.User.OrganizationId, int.Parse(hidMccID.Value), operationStr, sn.UserID).Tables[0];
            gvServicesSource.DataSource = dt;
            if (isBind) gvServicesSource.DataBind();
        }
    }

    public void BindgvServicesDest(Boolean isBind)
    {
        if (!string.IsNullOrEmpty(hidMccID.Value))
        {
            MCCManager mccMg = new MCCManager(sConnectionString);
            if (string.IsNullOrEmpty(operationStr)) operationStr = SentinelFM.clsAsynGenerateReport.GetOperationTypeStr();
            DataTable dt = mccMg.GetMCCMaintenanceAssigment(sn.User.OrganizationId, int.Parse(hidMccID.Value), operationStr, sn.UserID).Tables[0];
            gvServicesDest.DataSource = dt;
            if (isBind) gvServicesDest.DataBind();
        }
    }

    protected void gvServicesSource_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        if (this.Visible)
        {
            BindgvServicesSource(false);
        }
    }
    protected void gvServicesDest_NeedDataSource(object sender, Telerik.Web.UI.GridNeedDataSourceEventArgs e)
    {
        if (this.Visible)
        {
            BindgvServicesDest(false);
        }
    }
    protected void gvServicesSource_ItemDataBound(object sender, GridItemEventArgs e)
    {
        Button btnAssign = (Button)e.Item.FindControl("btnAssign");
        if (btnAssign != null)
        {
            btnAssign.Attributes.Add("onclick", "return MccAddServiceTypes_Add(" + ((DataRowView)e.Item.DataItem)["MaintenanceId"].ToString() + ")");
        }
    }
    protected void gvServicesDest_ItemDataBound(object sender, GridItemEventArgs e)
    {
        Button btnunAssign = (Button)e.Item.FindControl("btnunAssign");
        if (btnunAssign != null)
        {
            btnunAssign.Attributes.Add("onclick", "return MccAddServiceTypes_Delete(" + ((DataRowView)e.Item.DataItem)["MaintenanceId"].ToString() + ")");
        }

    }
}