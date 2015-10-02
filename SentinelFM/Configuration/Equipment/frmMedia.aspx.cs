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
    public partial class Configuration_Equipment_Media :SentinelFMBasePage
    {
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        string errorInsert = "cannot be saved.";
        string errorUpdate = "cannot be updated.";
        string errorDelete = "Delete failed.";
        string errorDeleteAssign = "Can not be deleted because it has been assigned to vehicle(s).";
        public string showFilter = "Show Filter";
        public string hideFilter = "Hide Filter";
        int MediaFactorDecimalDigits = 3;
        Label lblFactorName1 = null;
        Label lblFactorName2 = null;
        Label lblFactorName3 = null;
        Label lblFactorName4 = null;
        Label lblFactorName5 = null;

        TextBox txtDescription = null;
        RadNumericTextBox txtFactor1 = null;
        RadNumericTextBox txtFactor2 = null;
        RadNumericTextBox txtFactor3 = null;
        RadNumericTextBox txtFactor4 = null;
        RadNumericTextBox txtFactor5 = null;
        RadComboBox combTypeName = null;
        RadComboBox combMeasureUnit = null;
        Button btnSave = null;
        Button btnCancel = null;

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
            MediaFactorDecimalDigits = int.Parse(System.Configuration.ConfigurationManager.AppSettings["MediaFactorDecimalDigits"].ToString());
        }

        private DataTable GetOrganizationMedias()
        {
            DataTable dt = new DataTable();
            dt.Columns.Add("MediaId");
            dt.Columns.Add("Description");
            MediaManager mediaMg = new MediaManager(sConnectionString);
            DataSet ds = mediaMg.GetOrganizationMedias(sn.User.OrganizationId, sn.UserID);
            if (!ds.Equals(null) && ds.Tables.Count > 0)
            {
                dt = ds.Tables[0];
            }
            return dt;
        }

        protected void gdMedia_NeedDataSource(object sender, GridNeedDataSourceEventArgs e)
        {
            gdMedia.DataSource = GetOrganizationMedias();
        }
        protected void gdMedia_DeleteCommand(object sender, GridCommandEventArgs e)
        {
            if (((GridDataItem)e.Item).GetDataKeyValue("MediaId") != null)
            {
                try
                {
                    int mediaId = int.Parse(((GridDataItem)e.Item).GetDataKeyValue("MediaId").ToString());
                    MediaManager mediaMg = new MediaManager(sConnectionString);
                    int ret =  mediaMg.DeleteMedia(mediaId);
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
                UserControl editFormUserControl = (UserControl)item.FindControl(GridEditFormItem.EditFormUserControlID);

                if (editFormUserControl != null)
                {
                    GetControls(editFormUserControl);
                    btnCancel.CommandName = "Cancel";
                    if (e.Item.DataItem is DataRowView )
                    {
                        btnSave.CommandName = "Update";
                        if (!(((DataRowView)e.Item.DataItem)["UnitOfMeasureId"] is DBNull))
                        {
                            combMeasureUnit.SelectedValue = ((DataRowView)e.Item.DataItem)["UnitOfMeasureId"].ToString();
                            MediaManager mediaMg = new MediaManager(sConnectionString);
                            if (mediaMg.IsMediaUsedInEquipmentMediaAssignment(int.Parse(((DataRowView)e.Item.DataItem)["MediaId"].ToString())))
                            {
                                combMeasureUnit.Enabled = false;
                            }

                        }

                        if (!(((DataRowView)e.Item.DataItem)["MediaTypeId"] is DBNull))
                          combTypeName.SelectedValue = ((DataRowView)e.Item.DataItem)["MediaTypeId"].ToString();

                        if (!(((DataRowView)e.Item.DataItem)["Description"] is DBNull))
                            txtDescription.Text = ((DataRowView)e.Item.DataItem)["Description"].ToString();

                        SetControlsValues(lblFactorName1, txtFactor1, 
                               ((DataRowView)e.Item.DataItem)["FactorName1"],
                               ((DataRowView)e.Item.DataItem)["Factor1"]
                             );
                        SetControlsValues(lblFactorName2, txtFactor2,
                               ((DataRowView)e.Item.DataItem)["FactorName2"],
                               ((DataRowView)e.Item.DataItem)["Factor2"]
                             );

                        SetControlsValues(lblFactorName3, txtFactor3,
                               ((DataRowView)e.Item.DataItem)["FactorName3"],
                               ((DataRowView)e.Item.DataItem)["Factor3"]
                             );

                        SetControlsValues(lblFactorName4, txtFactor4,
                               ((DataRowView)e.Item.DataItem)["FactorName4"],
                               ((DataRowView)e.Item.DataItem)["Factor4"]
                             );

                        SetControlsValues(lblFactorName5, txtFactor5,
                               ((DataRowView)e.Item.DataItem)["FactorName5"],
                               ((DataRowView)e.Item.DataItem)["Factor5"]
                             );

                    }
                    else btnSave.CommandName = "PerformInsert";
                }
            }
            else
            {
                if (e.Item is GridDataItem)
                {
                    Label lblFactor1 = (Label)e.Item.FindControl("lblFactor1");
                    Label lblFactor2 = (Label)e.Item.FindControl("lblFactor2");
                    Label lblFactor3 = (Label)e.Item.FindControl("lblFactor3");
                    Label lblFactor4 = (Label)e.Item.FindControl("lblFactor4");
                    Label lblFactor5 = (Label)e.Item.FindControl("lblFactor5");
                    if (lblFactor1 != null)
                    {
                        if (e.Item.DataItem is DataRowView)
                        {
                            if (((DataRowView)e.Item.DataItem)["FactorName1"] is DBNull ||
                               ((DataRowView)e.Item.DataItem)["FactorName1"].ToString().Trim() == string.Empty)
                            {
                                lblFactor1.Visible = false;
                            }
                            else
                            {
                                lblFactor1.Text = GetFactor(((DataRowView)e.Item.DataItem)["Factor1"],
                                     ((DataRowView)e.Item.DataItem)["UnitOfMeasureAcr"]);
                            }

                            if (((DataRowView)e.Item.DataItem)["FactorName2"] is DBNull ||
                               ((DataRowView)e.Item.DataItem)["FactorName2"].ToString().Trim() == string.Empty)
                            {
                                lblFactor2.Visible = false;
                            }
                            else
                            {
                                lblFactor2.Text = GetFactor(((DataRowView)e.Item.DataItem)["Factor2"],
                                     ((DataRowView)e.Item.DataItem)["UnitOfMeasureAcr"]);
                            }

                            if (((DataRowView)e.Item.DataItem)["FactorName3"] is DBNull ||
                               ((DataRowView)e.Item.DataItem)["FactorName3"].ToString().Trim() == string.Empty)
                            {
                                lblFactor3.Visible = false;
                            }
                            else
                            {
                                lblFactor3.Text = GetFactor(((DataRowView)e.Item.DataItem)["Factor3"],
                                     ((DataRowView)e.Item.DataItem)["UnitOfMeasureAcr"]);
                            }

                            if (((DataRowView)e.Item.DataItem)["FactorName4"] is DBNull ||
                               ((DataRowView)e.Item.DataItem)["FactorName4"].ToString().Trim() == string.Empty)
                            {
                                lblFactor4.Visible = false;
                            }
                            else
                            {
                                lblFactor4.Text = GetFactor(((DataRowView)e.Item.DataItem)["Factor4"],
                                     ((DataRowView)e.Item.DataItem)["UnitOfMeasureAcr"]);
                            }

                            if (((DataRowView)e.Item.DataItem)["FactorName5"] is DBNull ||
                               ((DataRowView)e.Item.DataItem)["FactorName5"].ToString().Trim() == string.Empty)
                            {
                                lblFactor5.Visible = false;
                            }
                            else
                            {
                                lblFactor5.Text = GetFactor(((DataRowView)e.Item.DataItem)["Factor5"],
                                     ((DataRowView)e.Item.DataItem)["UnitOfMeasureAcr"]);
                            }

                        }

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


        private void GetControls(UserControl usercontrol)
        {
            lblFactorName1 = (Label)usercontrol.FindControl("lblFactorName1");
            lblFactorName2 = (Label)usercontrol.FindControl("lblFactorName2");
            lblFactorName3 = (Label)usercontrol.FindControl("lblFactorName3");
            lblFactorName4 = (Label)usercontrol.FindControl("lblFactorName4");
            lblFactorName5 = (Label)usercontrol.FindControl("lblFactorName5");
            txtDescription = (TextBox)usercontrol.FindControl("txtDescription");

            txtFactor1 = (RadNumericTextBox)usercontrol.FindControl("txtFactor1");
            txtFactor2 = (RadNumericTextBox)usercontrol.FindControl("txtFactor2");
            txtFactor3 = (RadNumericTextBox)usercontrol.FindControl("txtFactor3");
            txtFactor4 = (RadNumericTextBox)usercontrol.FindControl("txtFactor4");
            txtFactor5 = (RadNumericTextBox)usercontrol.FindControl("txtFactor5");

            combTypeName = (RadComboBox)usercontrol.FindControl("combTypeName");
            combMeasureUnit = (RadComboBox)usercontrol.FindControl("combMeasureUnit");

            btnSave = (Button)usercontrol.FindControl("btnSave");
            btnCancel = (Button)usercontrol.FindControl("btnCancel");
        }

        private void SetControlsValues(Label lblFactorName, RadNumericTextBox txtFactor, object factorName, object factor)
        {
            if (factorName is DBNull) return;
            if (string.IsNullOrEmpty(factorName.ToString())) return;
            lblFactorName.Text = factorName.ToString();
            lblFactorName.Visible = true;
            if (!(factor is DBNull) && !string.IsNullOrEmpty(factor.ToString()))
            {
                txtFactor.Value = Math.Round(double.Parse(factor.ToString()), MediaFactorDecimalDigits);
            }
            else txtFactor.Value = null;
            txtFactor.Visible = true;
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
                UserControl editFormUserControl = (UserControl)item.FindControl(GridEditFormItem.EditFormUserControlID);
                if ((e.CommandName == "PerformInsert" || e.CommandName == "Update") && editFormUserControl != null)
                {
                    if (!Page.IsValid) return;
                    GetControls(editFormUserControl);
                    string description = txtDescription.Text.Trim();
                    int mediaType = -1;
                    double? factor1 = null;
                    double? factor2 = null;
                    double? factor3 = null;
                    double? factor4 = null;
                    double? factor5 = null;
                    if (e.CommandName == "PerformInsert")
                    {
                        try
                        {
                            mediaType = int.Parse(combTypeName.SelectedValue);
                            if (txtFactor1.Visible) factor1 = txtFactor1.Value;
                            if (txtFactor2.Visible) factor2 = txtFactor2.Value;
                            if (txtFactor3.Visible) factor3 = txtFactor3.Value;
                            if (txtFactor4.Visible) factor4 = txtFactor4.Value;
                            if (txtFactor5.Visible) factor5 = txtFactor5.Value;
                            int baseUnitOfMeasureID = int.Parse(combMeasureUnit.SelectedValue);
                            MediaManager mediaMg = new MediaManager(sConnectionString);
                            mediaMg.AddMedia(sn.User.OrganizationId, description, mediaType, factor1, factor2, factor3, factor4, factor5, baseUnitOfMeasureID, sn.UserID);
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
                    if (e.CommandName == "Update")
                    {
                        try
                        {
                            int mediaId = int.Parse(((GridEditFormItem)e.Item).GetDataKeyValue("MediaId").ToString());
                            mediaType = int.Parse(combTypeName.SelectedValue);
                            if (txtFactor1.Visible) factor1 = txtFactor1.Value;
                            if (txtFactor2.Visible) factor2 = txtFactor2.Value;
                            if (txtFactor3.Visible) factor3 = txtFactor3.Value;
                            if (txtFactor4.Visible) factor4 = txtFactor4.Value;
                            if (txtFactor5.Visible) factor5 = txtFactor5.Value;
                            MediaManager mediaMg = new MediaManager(sConnectionString);
                            int baseUnitOfMeasureID = int.Parse(combMeasureUnit.SelectedValue);
                            mediaMg.UpdateMedia(mediaId, sn.User.OrganizationId, description, mediaType, factor1, factor2, factor3, factor4, factor5, baseUnitOfMeasureID, sn.UserID);
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
        }

        private string GetFactor(object factor, object unit)
        { 
            string ret = "&nbsp;";
            if (!(factor is DBNull))
            {
                double dFactor = double.Parse(factor.ToString()); 
                ret = Math.Round(dFactor, MediaFactorDecimalDigits).ToString();
                if (!(unit is DBNull))
                {
                    ret = ret + unit.ToString();
                }
            }

            return ret;
        }
}
}