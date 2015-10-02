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
using VLF.CLS;
using VLF.CLS.Def;
using System.IO;

namespace SentinelFM
{
    public partial class Configuration_frmpolicies : SentinelFMBasePage
   {
      
      protected ServerDBOrganization.DBOrganization org = new ServerDBOrganization.DBOrganization();

      protected void Page_Load(object sender, EventArgs e)
      {
         try
         {
            if (!Page.IsPostBack)
            {
               LoadProducts();
            }
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            //lblMessage.Text = Ex.Message;
            base.RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            base.RedirectToLogin();
         }
      }

      private void LoadProducts()
      {
         string xmlProducts = "";

         tblViewProducts.Visible = true;
         tblProductDetails.Visible = false;

         lblNoProductsMessage.Text = "";
         lblMessage.Text = "";
         
         if (objUtil.ErrCheck(org.GetOrganizationAllProducts(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlProducts), false))
            if (objUtil.ErrCheck(org.GetOrganizationAllProducts(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xmlProducts), true))
            {
               System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, Util.TraceFormat(Enums.TraceSeverity.Error,
                  " No products for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
               lblNoProductsMessage.Text = "There are no products defined.";//this.GetLocalResourceObject("resNoProducts").ToString();
               return;
            }

         DataSet dsProducts = new DataSet();
         if (String.IsNullOrEmpty(xmlProducts))
         {
            lblNoProductsMessage.Text = "There are no products defined.";//this.GetLocalResourceObject("resNoProducts").ToString();
            gdvProducts.DataSource = null;
            gdvProducts.DataBind();
            return;
         }

         dsProducts.ReadXml(new StringReader(xmlProducts));

         if (Util.IsDataSetValid(dsProducts))
         {
            gdvProducts.DataSource = dsProducts;
            gdvProducts.DataBind();
         }
         else lblNoProductsMessage.Text = "There are no products defined.";//this.GetLocalResourceObject("resNoProducts").ToString();
      }

      protected void gdvProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
      {
         try
         {
            gdvProducts.PageIndex = e.NewPageIndex;
            LoadProducts();
            gdvProducts.SelectedIndex = -1;
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            base.RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            base.RedirectToLogin();
         }
      }

      protected void btnAddProduct_Click(object sender, EventArgs e)
      {
         tblViewProducts.Visible = false;
         tblProductDetails.Visible = true;
         if (ViewState["ProductID"] != null) ViewState.Remove("ProductID");
         ClearControls();
      }

      private void ClearControls()
      {
         txtLower.Text = "";
         txtProductName.Text = "";
         txtUpper.Text = "";
         lblMessage.Text = "";
      }

      private void SetFields(DataRow row)
      {
         txtLower.Text = row["Lower"].ToString().Trim();
         txtProductName.Text = row["ProductName"].ToString().Trim();
         txtUpper.Text = row["Upper"].ToString().Trim();
      }

      protected void btnCancel_Click(object sender, EventArgs e)
      {
         tblViewProducts.Visible = true;
         tblProductDetails.Visible = false;
         lblMessage.Text = "";
      }

      protected void btnSave_Click(object sender, EventArgs e)
      {
         try
         {
            float upper = 0.0f, lower = 0.0f;
            int productId = 0;
            

            # region Data Validation
            if (String.IsNullOrEmpty(txtProductName.Text))
            {
               lblMessage.Text = "Please fill in a product name"; //this.GetLocalResourceObject("resFillProductName").ToString();
               return;
            }
            if (String.IsNullOrEmpty(txtUpper.Text))
            {
               lblMessage.Text = "Please fill in an upper limit "; //this.GetLocalResourceObject("resFillUpper").ToString(); 
               return;
            }
            if (String.IsNullOrEmpty(txtLower.Text))
            {
               lblMessage.Text = "Please fill in a lower limit"; //this.GetLocalResourceObject("resFillLower").ToString();
               return;
            }

            if (!Single.TryParse(txtUpper.Text, out upper))
            {
               lblMessage.Text = "Please fill in an upper limit "; //this.GetLocalResourceObject("resFillLicenseIssued").ToString();
               return;
            }

            if (!Single.TryParse(txtLower.Text, out lower))
            {
               lblMessage.Text = "Please fill in a lower limit"; //this.GetLocalResourceObject("resFillLicenseExpired").ToString();
               return;
            }

            # endregion

            if (ViewState["ProductID"] == null)
            {
                
               if (objUtil.ErrCheck(org.AddProduct(sn.UserID, sn.SecId, sn.User.OrganizationId, txtProductName.Text, upper, lower), false))
                  if (objUtil.ErrCheck(org.AddProduct(sn.UserID, sn.SecId, sn.User.OrganizationId, txtProductName.Text, upper, lower), true))
                  {
                     System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                        " Add product error for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                     //RedirectToLogin();
                     lblMessage.Text = "Add product error";// this.GetLocalResourceObject("resAddDriverError").ToString();
                     return;
                  }
            }
            else
            {
               if (!Int32.TryParse(ViewState["ProductID"].ToString(), out productId))
               {
                  base.RedirectToLogin();
               }
               if (objUtil.ErrCheck(org.UpdateProductById(sn.UserID, sn.SecId, sn.User.OrganizationId, productId, txtProductName.Text, upper, lower), false))
                  if (objUtil.ErrCheck(org.UpdateProductById(sn.UserID, sn.SecId, sn.User.OrganizationId, productId, txtProductName.Text, upper, lower), true))
                  {
                     System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                        " Update product error for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                     //RedirectToLogin();
                     lblMessage.Text = "Add product error";//this.GetLocalResourceObject("resUpdateDriverError").ToString();
                     return;
                  }
               if (ViewState["ProductID"] != null) ViewState.Remove("ProductID");
            }
            tblProductDetails.Visible = false;
            tblViewProducts.Visible = true;
            LoadProducts();
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
            //lblMessage.Text = Ex.Message;
            base.RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            //lblMessage.Text = Ex.Message;
            base.RedirectToLogin();
         }
      }

      protected void gdvProducts_RowDeleting(object sender, GridViewDeleteEventArgs e)
      {
         try
         {
            

            if (objUtil.ErrCheck(org.DeleteOrganizationProduct(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(gdvProducts.Rows[e.RowIndex].Cells[0].Text)), false))
               if (objUtil.ErrCheck(org.DeleteOrganizationProduct(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(gdvProducts.Rows[e.RowIndex].Cells[0].Text)), true))
               {
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                     " Delete product error for User:" + sn.UserID.ToString() + " Form:frmPolicies.aspx"));
                  return;
               }
            LoadProducts();
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, 
               Ex.StackTrace.ToString()));
            base.RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
               Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:frmPolicies.aspx"));
            base.RedirectToLogin();
         }
      }

      protected void gdvProducts_RowEditing(object sender, GridViewEditEventArgs e)
      {
         try
         {
            tblViewProducts.Visible = false;
            tblProductDetails.Visible = true;
            lblMessage.Text = "";

            // get data
            string xmlResult = "";
            
            if (objUtil.ErrCheck(org.GetOrganizationProductById(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(gdvProducts.Rows[e.NewEditIndex].Cells[0].Text),
               ref xmlResult), false))
               if (objUtil.ErrCheck(org.GetOrganizationProductById(sn.UserID, sn.SecId, sn.User.OrganizationId, Convert.ToInt32(gdvProducts.Rows[e.NewEditIndex].Cells[0].Text),
               ref xmlResult), true))
               {
                  System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                     " Get product error for User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                  //RedirectToLogin();
                  return;
               }

            if (ViewState["ProductID"] != null) ViewState.Remove("ProductID");
            ViewState.Add("ProductID", gdvProducts.Rows[e.NewEditIndex].Cells[0].Text);

            DataSet dsProduct = new DataSet();
            dsProduct.ReadXml(new System.IO.StringReader(xmlResult));
            // fill controls
            if (VLF.CLS.Util.IsDataSetValid(dsProduct))
               SetFields(dsProduct.Tables[0].Rows[0]);
         }
         catch (NullReferenceException Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, 
               Ex.StackTrace.ToString()));
            //lblMessage.Text = Ex.Message;
            base.RedirectToLogin();
         }
         catch (Exception Ex)
         {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, 
               Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            //lblMessage.Text = Ex.Message;
            base.RedirectToLogin();
         }
      }
   }
}