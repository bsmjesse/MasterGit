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
using System.Configuration;
using System.Text;
using VLF.CLS;
using Telerik.Web.UI;
using System.Drawing;
using VLF.DAS.Logic;

namespace SentinelFM
{
    public partial class Configuration_frmequipment : SentinelFMBasePage
    {
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        string errorInsert = "cannot be saved.";
        string errorUpdate = "cannot be updated.";
        string errorDelete = "Delete failed.";
        string errorDeleteAssign = "Can not be deleted because it has been assigned to vehicle(s).";
        public string showFilter = "Show Filter";
        public string hideFilter = "Hide Filter";

        protected void Page_Load(object sender, EventArgs e)
        {
            //check session, to be changed

            //End
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
                    //if (sn.User.MenuColor != "")
                    //{
                    //    Literal css = new Literal();
                    //    css.Text = @"<style type='text/css'> " +
                    //               @".RadGridtblHeader" +
                    //               @"{" +
                    //               @"background-color:" + sn.User.MenuColor + "!important;" +
                    //               @"background-image:none !important;" +
                    //               @"}" +
                    //               "</style>";
                    //    this.Header.Controls.Add(css);
                    //}

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

        private DataTable GetOrganizationEquipments()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("EquipmentTypeId");
            dt.Columns.Add("Description");
            EquipmentManager equipmentMg = new EquipmentManager(sConnectionString);
            DataSet ds = equipmentMg.GetOrganizationEquipments(sn.User.OrganizationId);
            if (!ds.Equals(null) && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;
        }

        private DataTable GetEquipmentTypes()
        {
            DataTable dt = null;
            EquipmentManager equipmentMg = new EquipmentManager(sConnectionString);
            DataSet ds = equipmentMg.GetEquipmentTypes();
            if (!ds.Equals(null) && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;
        }

        protected void gdEquipment_OnInsertCommand(object source, GridCommandEventArgs e)
        {
            if (e.Item is Telerik.Web.UI.GridEditFormInsertItem)
            {
                TextBox txtDescription = (TextBox)e.Item.FindControl("txtDescription");
                RadComboBox combTypeName = (RadComboBox)e.Item.FindControl("combTypeName");
                if (!txtDescription.Equals(null) && !combTypeName.Equals(null))
                {
                    string description = txtDescription.Text.Trim();

                    int equipmentType = -1;
                    if (combTypeName.SelectedIndex < 0) combTypeName.SelectedIndex = 0;
                    try
                    {
                        equipmentType = int.Parse(combTypeName.SelectedValue);
                        EquipmentManager equipmentMg = new EquipmentManager(sConnectionString);
                        equipmentMg.AddEquipment(sn.User.OrganizationId, description, equipmentType);
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                        ExceptionLogger(trace);
                        e.Canceled = true;
                        string errorMsg = string.Format("alert(\"'{0}' {1}\");", description, errorInsert);
                        RadAjaxManager1.ResponseScripts.Add(errorMsg);
                    }

                }
            }
        }
        protected void gdEquipment_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            gdEquipment.DataSource = GetOrganizationEquipments();
        }
        protected void gdEquipment_UpdateCommand(object sender, GridCommandEventArgs e)
        {
            if (!(e.Item is Telerik.Web.UI.GridEditFormItem)) return;
            if (((GridEditFormItem)e.Item).GetDataKeyValue("EquipmentId") != null)
            {
                TextBox txtDescription = (TextBox)e.Item.FindControl("txtDescription");
                RadComboBox combTypeName = (RadComboBox)e.Item.FindControl("combTypeName");
                if (!txtDescription.Equals(null) && !combTypeName.Equals(null))
                {
                    string description = txtDescription.Text.Trim();
                    int equipmentType = -1;
                    if (combTypeName.SelectedIndex < 0) combTypeName.SelectedIndex = 0;
                    try
                    {
                        equipmentType = int.Parse(combTypeName.SelectedValue);
                        int equipmentId = int.Parse(((GridEditFormItem)e.Item).GetDataKeyValue("EquipmentId").ToString());
                        EquipmentManager equipmentMg = new EquipmentManager(sConnectionString);
                        equipmentMg.UpdateEquipment(equipmentId, sn.User.OrganizationId, description, equipmentType);
                    }
                    catch (Exception Ex)
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                        System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                        ExceptionLogger(trace);
                        e.Canceled = true;
                        string errorMsg = string.Format("alert(\"'{0}' {1}\");", description, errorUpdate);
                        RadAjaxManager1.ResponseScripts.Add(errorMsg);
                    }
                }

            }
        }
        protected void gdEquipment_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            if (((GridDataItem)e.Item).GetDataKeyValue("EquipmentId") != null)
            {
                try
                {
                    int equipmentId = int.Parse(((GridDataItem)e.Item).GetDataKeyValue("EquipmentId").ToString());
                    EquipmentManager equipmentMg = new EquipmentManager(sConnectionString);
                    int ret = equipmentMg.DeleteEquipment(equipmentId);
                    if (ret == -1)
                        RadAjaxManager1.ResponseScripts.Add(string.Format("alert(\"{0}\");", errorDeleteAssign));
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
        protected void gdEquipment_ItemDataBound(object sender, GridItemEventArgs e)
        {
            if (e.Item is GridEditFormItem && e.Item.IsInEditMode)
            {
                GridEditFormItem item = (GridEditFormItem)e.Item;
                RadComboBox combTypeName = (RadComboBox)item.FindControl("combTypeName");
                if (combTypeName != null)
                {
                    DataTable dt = GetEquipmentTypes();
                    if (dt != null)
                    {
                        combTypeName.DataSource = dt;
                        combTypeName.DataBind();
                        if (e.Item.DataItem is DataRowView)
                        {
                            combTypeName.SelectedValue = ((DataRowView)e.Item.DataItem)["EquipmentTypeId"].ToString();
                        }
                    }
                }
            }

        }
        protected void gdEquipment_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "InitInsert" || e.CommandName == "Edit" || e.CommandName == "Delete")
            {
                if (e.CommandName == "Edit") gdEquipment.MasterTableView.IsItemInserted = false;
                gdEquipment.MasterTableView.ClearEditItems();
            }

        }
}
}