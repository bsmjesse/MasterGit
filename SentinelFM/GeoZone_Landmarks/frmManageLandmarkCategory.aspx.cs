using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SentinelFM
{

    public partial class frmManageLandmarkCategory : SentinelFMBasePage
    {
        string sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            this.lblMessage.Visible = false;
            this.btnDeleteCategory.OnClientClick = string.Format("return ConfirmDelete('{0}');", 
                             (string)base.GetLocalResourceObject("btnDeleteCategoryResource1.ConfirmationMessage"));


            if (this.IsPostBack == false)
            {
                
                this.btnUpdateCategory.Visible = false;
                this.btnCancelUpdateCategory.Visible = false;

                PopulateCategoryList();
            }
        }

        private void DisplayMessage(string message)
        {
            this.lblMessage.Visible = true;
            this.lblMessage.Text = message;
        }

        protected void btnAddCategory_Click(object sender, EventArgs e)
        {
            if (this.txtCategoryName.Text.Trim() != string.Empty)
            {
                int resultCode = AddCategory(this.txtCategoryName.Text.Trim());
                if (resultCode > 0)
                {
                    this.txtCategoryName.Text = string.Empty;
                    PopulateCategoryList();
                    DisplayMessage((string)base.GetLocalResourceObject("lblMessage_Text_Add_Success"));
                }
                else
                {
                    //Failed
                    string strMessage = string.Empty;

                    if (resultCode == -2627)
                    {
                        DisplayMessage((string)base.GetLocalResourceObject("lblMessage_Text_Add_Failed_DuplicateError"));
                    }
                    else
                    {
                        DisplayMessage((string)base.GetLocalResourceObject("lblMessage_Text_Add_Failed"));
                    }
                }
            }
            else
            {
                DisplayMessage((string)base.GetLocalResourceObject("lblMessage_Text_Validation_MissingCategory"));
            }
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            if (this.lbxCategoryList.SelectedItem != null)
            {
                this.txtCategoryName.Text = this.lbxCategoryList.SelectedItem.Text;
                this.txtCategoryId.Text = this.lbxCategoryList.SelectedValue;

                this.btnUpdateCategory.Visible = true;
                this.btnCancelUpdateCategory.Visible = true;
                this.btnAddCategory.Visible = false;

                this.btnEditCategory.Visible = false;
                this.btnDeleteCategory.Visible = false;

                this.lbxCategoryList.Attributes.Add("disabled", "");
            }
            else
            {
                DisplayMessage((string)base.GetLocalResourceObject("lblMessage_Text_Validation_Edit_NoCategorySelected"));
            }
        }

        protected void btnCancelUpdateCategory_Click(object sender, EventArgs e)
        {
            this.btnUpdateCategory.Visible = false;
            this.btnCancelUpdateCategory.Visible = false;
            this.txtCategoryName.Text = string.Empty;
            this.txtCategoryId.Text = string.Empty;
            this.btnAddCategory.Visible = true;

            this.btnEditCategory.Visible = true;
            this.btnDeleteCategory.Visible = true;

            this.lbxCategoryList.Attributes.Remove("disabled");
        }

        protected void btnUpdateCategory_Click(object sender, EventArgs e)
        {
            if (this.txtCategoryName.Text.Trim() != string.Empty)
            {

                int resultCode = UpdateCategory(Convert.ToInt64(this.txtCategoryId.Text), this.txtCategoryName.Text.Trim());
                if (resultCode > 0)
                {
                    this.btnUpdateCategory.Visible = false;
                    this.btnCancelUpdateCategory.Visible = false;

                    this.txtCategoryName.Text = string.Empty;
                    this.txtCategoryId.Text = string.Empty;

                    this.btnEditCategory.Visible = true;
                    this.btnDeleteCategory.Visible = true;
                    this.btnAddCategory.Visible = true;
                    this.lbxCategoryList.Attributes.Remove("disabled");

                    PopulateCategoryList();
                    DisplayMessage((string)base.GetLocalResourceObject("lblMessage_Text_Update_Success"));
                }
                else
                {
                    //Failed
                    string strMessage = string.Empty;

                    if (resultCode == -2627)
                    {
                        DisplayMessage((string)base.GetLocalResourceObject("lblMessage_Text_Update_Failed_DuplicateError"));
                    }
                    else
                    {
                        DisplayMessage((string)base.GetLocalResourceObject("lblMessage_Text_Update_Failed"));
                    }
                }
            }
            else
            {
                DisplayMessage((string)base.GetLocalResourceObject("lblMessage_Text_Validation_MissingCategory"));
            }
            
        }

        protected void btnDeleteCategory_Click(object sender, EventArgs e)
        {
            if(this.lbxCategoryList.SelectedItem != null)
            {
                bool isSuccess = DeleteCategory(Convert.ToInt64(this.lbxCategoryList.SelectedItem.Value));
                if (isSuccess == true)
                {
                    PopulateCategoryList();
                    DisplayMessage((string)base.GetLocalResourceObject("lblMessage_Text_Delete_Success"));
                }
                else
                {
                     //Failed
                    DisplayMessage((string)base.GetLocalResourceObject("lblMessage_Text_Delete_Failed"));
                }
            } else
            {
                DisplayMessage((string)base.GetLocalResourceObject("lblMessage_Text_Validation_Delete_NoCategorySelected"));
            }

        }


        private void PopulateCategoryList()
        {
            try
            {
                VLF.DAS.Logic.Organization org = new VLF.DAS.Logic.Organization(this.sConnectionString);
                DataSet dsOrganization = org.ListOrganizationLandmarkCategory(sn.UserID, sn.User.OrganizationId);
                DataTable dsLandmarkCategory = dsOrganization.Tables["LandmarkCategory"];

                ListItem oneItem = null;
                this.lbxCategoryList.Items.Clear();
                foreach (DataRow oneRow in dsLandmarkCategory.Rows)
                {
                    oneItem = new ListItem(oneRow["MetadataValue"].ToString(), oneRow["DomainMetadataId"].ToString());
                    this.lbxCategoryList.Items.Add(oneItem);
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
            }
        }

        private int AddCategory(string categoryName)
        {
            int resultCode = 0;
            try
            {
                VLF.DAS.Logic.Organization org = new VLF.DAS.Logic.Organization(this.sConnectionString);
                int numberOfAffectedRecords = org.AddOrganizationDomainMetadata(sn.UserID, sn.User.OrganizationId, 1, categoryName);

                resultCode = numberOfAffectedRecords;
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }

            catch (Exception Ex)
            {
                if (Ex.Message.Contains("IX_DomainMetadata_UQ_MetadataValueWithinOrgDomain") == true)
                {
                    resultCode = -2627;
                }

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

            return resultCode;
        }

        private int UpdateCategory(long categoryId, string newCategoryName)
        {
            int resultCode = 0;
            try
            {
                VLF.DAS.Logic.Organization org = new VLF.DAS.Logic.Organization(this.sConnectionString);
                int numberOfAffectedRecords = org.UpdateOrganizationDomainMetadata(sn.UserID, sn.User.OrganizationId,categoryId, newCategoryName);

                resultCode = numberOfAffectedRecords;
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }

            catch (Exception Ex)
            {
                if (Ex.Message.Contains("IX_DomainMetadata_UQ_MetadataValueWithinOrgDomain") == true)
                {
                    resultCode = -2627;
                }

                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

            return resultCode;
        }

        private bool DeleteCategory(long categoryId)
        {
            bool isSuccess = false;
            try
            {
                VLF.DAS.Logic.Organization org = new VLF.DAS.Logic.Organization(this.sConnectionString);
                int numberOfAffectedRecords = org.DeleteOrganizationDomainMetadata(sn.UserID, sn.User.OrganizationId, categoryId);

                if (numberOfAffectedRecords > 0)
                {
                    isSuccess = true;
                }
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            }

            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }

            return isSuccess;
        }

    }
}