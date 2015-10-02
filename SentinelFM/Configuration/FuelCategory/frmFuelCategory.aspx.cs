using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Configuration;
using System.Data;
using VLF.DAS.Logic;
using Telerik.Web.UI;

namespace SentinelFM
{
    public partial class Configuration_FuelCategory : SentinelFMBasePage
    {
        //PostBackUrl="~/Configuration/frmFuelCategory.aspx " CssClass="confbutton"
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        string errorInsert = "cannot be saved.";
        string errorUpdate = "cannot be updated.";
        string errorDelete = "Delete failed.";
        string errorDeleteAssign = "Can not be deleted because it has been assigned to vehicle(s).";
        string confirmstring = "The category has been assigned to vehicle(s), are you sure you want to delete this fuel category?";
        string confirmstring_1 = "Are you sure you want to delete this fuel category?";
        public string showFilter = "Show Filter";
        public string hideFilter = "Hide Filter";

        TextBox txtFuelType;
        TextBox txtGHGCategory;
        TextBox txtGHGCategoryDesc;
        RadNumericTextBox txtCO2Factor;

        Button btnSave = null;
        Button btnCancel = null;

        protected void Page_Load(object sender, EventArgs e)
        {
            //check session, to be changed
            //End

	    bool isHgiUser = false;
            if (sn != null && sn.UserName.ToLower().Contains("hgi_"))
            {                
                isHgiUser = true;                
            }
	
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

		 if (!isHgiUser)
                {
                    cmdMapSubscription.Visible = false;
                    cmdOverlaySubscription.Visible = false;
                }

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

        private DataTable GetOrganizationFuelCategory()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MediaId");
            dt.Columns.Add("Description");
            FuelCategoryMgr fuelCategoryMgr = new FuelCategoryMgr(sConnectionString);
            DataSet ds = fuelCategoryMgr.FuelCategory_Select(-1, sn.User.OrganizationId, true);
            if (!ds.Equals(null) && ds.Tables.Count > 0)

            {
                dt = ds.Tables[0];
            }
            return dt;
        }

        protected void gdMedia_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            gdMedia.DataSource = GetOrganizationFuelCategory();
        }
        protected void gdMedia_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            if (((GridDataItem)e.Item).GetDataKeyValue("FuelTypeID") != null)
            {
                try
                {
                    int fuelTypeID = int.Parse(((GridDataItem)e.Item).GetDataKeyValue("FuelTypeID").ToString());
                    FuelCategoryMgr fuelCategoryMgr = new FuelCategoryMgr(sConnectionString);
                    int ret = fuelCategoryMgr.FuelCategory_Delete(fuelTypeID, sn.User.OrganizationId);
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
        protected void gdMedia_ItemDataBound(object sender, GridItemEventArgs e)
        {

            if (e.Item is GridEditFormItem && e.Item.IsInEditMode)
            {
                GridEditFormItem item = (GridEditFormItem)e.Item;
                System.Web.UI.UserControl editFormUserControl = (System.Web.UI.UserControl)item.FindControl(GridEditFormItem.EditFormUserControlID);

                if (editFormUserControl != null)
                {
                    GetControls(editFormUserControl);
                    btnCancel.CommandName = "Cancel";
                    if (e.Item.DataItem is DataRowView )
                    {
                        btnSave.CommandName = "Update";
                        if (!(((DataRowView)e.Item.DataItem)["FuelType"] is DBNull))
                        {
                            txtFuelType.Text = ((DataRowView)e.Item.DataItem)["FuelType"].ToString();
                        }

                        if (!(((DataRowView)e.Item.DataItem)["GHGCategory"] is DBNull))
                        {
                            txtGHGCategory.Text = ((DataRowView)e.Item.DataItem)["GHGCategory"].ToString();
                        }

                        if (!(((DataRowView)e.Item.DataItem)["GHGCategoryDesc"] is DBNull))
                        {
                            txtGHGCategoryDesc.Text = ((DataRowView)e.Item.DataItem)["GHGCategoryDesc"].ToString();
                        }

                        if (!(((DataRowView)e.Item.DataItem)["CO2Factor"] is DBNull))
                        {
                            txtCO2Factor.Text = ((DataRowView)e.Item.DataItem)["CO2Factor"].ToString();
                        }

                    }
                    else btnSave.CommandName = "PerformInsert";
                }
            }
            else
            {
                if (e.Item is GridDataItem)
                {

                    if (((DataRowView)e.Item.DataItem)["hasAssign"].ToString() == "1")
                    {
                        GridDataItem dataItem = e.Item as GridDataItem;
                        //GridDataItem dataItem = e.Item as GridDataItem;
                        // (gdMedia.MasterTableView.GetColumn("DeleteColumn") as GridButtonColumn).ConfirmText = "The category has been assigned to vehicle, are you sure you want to delete this fuel category ";
                        ImageButton button = dataItem["DeleteColumn"].Controls[0] as ImageButton;
                        button.Attributes["onclick"] = string.Format("return confirm(\"{0}\");", confirmstring);
                    }
                    else
                    {
                        GridDataItem dataItem = e.Item as GridDataItem;
                        //GridDataItem dataItem = e.Item as GridDataItem;
                        // (gdMedia.MasterTableView.GetColumn("DeleteColumn") as GridButtonColumn).ConfirmText = "The category has been assigned to vehicle, are you sure you want to delete this fuel category ";
                        ImageButton button = dataItem["DeleteColumn"].Controls[0] as ImageButton;

                        button.Attributes["onclick"] = string.Format("return confirm(\"{0}\");", confirmstring_1);
                    }
                }
            }

        }

        public int? GetVal(object val)
        {
            if (val is DBNull) return null;
            int ret = 0;
            if (int.TryParse(val.ToString(), out ret )) return ret;
            else return null;
        }


        private void GetControls(System.Web.UI.UserControl usercontrol)
        {
            txtFuelType = (TextBox)usercontrol.FindControl("txtFuelType");
            txtGHGCategory = (TextBox)usercontrol.FindControl("txtGHGCategory");
            txtGHGCategoryDesc = (TextBox)usercontrol.FindControl("txtGHGCategoryDesc");
            txtCO2Factor = (RadNumericTextBox)usercontrol.FindControl("txtCO2Factor");

            btnSave = (Button)usercontrol.FindControl("btnSave");
            btnCancel = (Button)usercontrol.FindControl("btnCancel");
        }

        protected void gdMedia_ItemCommand(object sender, GridCommandEventArgs e)
        {
            if (e.CommandName == "InitInsert" || e.CommandName == "Edit" || e.CommandName == "Delete") 
            {
                if (e.CommandName == "Edit") gdMedia.MasterTableView.IsItemInserted = false; 
                gdMedia.MasterTableView.ClearEditItems();
            }
            if (e.Item is GridEditFormItem && e.Item.IsInEditMode)
            {
                GridEditFormItem item = (GridEditFormItem)e.Item;
                System.Web.UI.UserControl editFormUserControl = (System.Web.UI.UserControl)item.FindControl(GridEditFormItem.EditFormUserControlID);
                if ((e.CommandName == "PerformInsert" || e.CommandName == "Update") && editFormUserControl != null)
                {
                    if (!Page.IsValid) return;
                    GetControls(editFormUserControl);
                    string fuelType = txtFuelType.Text.Trim();
                    string GHGCategory = txtGHGCategory.Text.Trim();
                    string GHGCategoryDesc = txtGHGCategoryDesc.Text.Trim();
                    float CO2Factor = (float)txtCO2Factor.Value.Value;

                    if (e.CommandName == "PerformInsert")
                    {
                        try
                        {
                            FuelCategoryMgr fuelCategoryMgr = new FuelCategoryMgr(sConnectionString);
                            fuelCategoryMgr.FuelCategory_Add(sn.User.OrganizationId, fuelType, GHGCategory, GHGCategoryDesc, CO2Factor);
                        }
                        catch (Exception Ex)
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                            ExceptionLogger(trace);
                            e.Canceled = true;
                            string errorMsg = string.Format("alert(\"'{0}' {1}\");", GHGCategory, errorInsert);
                            RadAjaxManager1.ResponseScripts.Add(errorMsg);
                        }
                    }
                    if (e.CommandName == "Update")
                    {
                        try
                        {
                            int fuelTypeID = int.Parse(((GridEditFormItem)e.Item).GetDataKeyValue("FuelTypeID").ToString());
                            FuelCategoryMgr fuelCategoryMgr = new FuelCategoryMgr(sConnectionString);
                            fuelCategoryMgr.FuelCategory_Update(
                                fuelTypeID, sn.User.OrganizationId, fuelType, GHGCategory, GHGCategoryDesc, CO2Factor);
                        }
                        catch (Exception Ex)
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(Ex, true);
                            ExceptionLogger(trace);
                            e.Canceled = true;
                            string errorMsg = string.Format("alert(\"'{0}' {1}\");", GHGCategory, errorUpdate);
                            RadAjaxManager1.ResponseScripts.Add(errorMsg);
                        }

                    }
               }
            }
        }

}
}