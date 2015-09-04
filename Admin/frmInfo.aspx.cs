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
using System.Text;
using SentinelFM;

public partial class frmInfo : System.Web.UI.Page
{
   protected SentinelFMSession sn = null;

   protected void Page_Load(object sender, EventArgs e)
   {
      try
      {
         sn = (SentinelFMSession)Session["SentinelFMSession"];
         if ((sn == null) || (sn.UserName == ""))
         {
            Response.Write("<SCRIPT Language='javascript'>window.open('frmAdminLogin.aspx','_top') </SCRIPT>");
            return;
         }
         if (!Page.IsPostBack)
         {
         }
      }
      catch (Exception ex)
      {
         lblError.Text = "Error loading info page :: " + ex.Message;
      }
   }

   protected void ddlFwType_SelectedIndexChanged(object sender, EventArgs e)
   {
   }

   protected void sqlFwList_Filtering(object sender, SqlDataSourceFilteringEventArgs e)
   {
      if (ddlFwType.SelectedValue == "0") // all types - display all
         sqlFwList.FilterExpression = " ";
      else
         sqlFwList.FilterExpression = "FwTypeId = {0}";
   }

   protected void sqlInfo_Filtering(object sender, SqlDataSourceFilteringEventArgs e)
   {
      sqlInfo.FilterExpression = BuildInfoFilter();// "OrganizationId = {0}";
   }

   protected void CheckBoxList1_SelectedIndexChanged(object sender, EventArgs e)
   {
      for (int i = 0; i < CheckBoxList1.Items.Count; i++)
      {
         gdvwData.Columns[i].Visible = CheckBoxList1.Items[i].Selected;
      }
   }

   /// <summary>
   /// Build SqlDataSource (sqlInfo) FilterExpression based on the ddl values
   /// </summary>
   /// <returns></returns>
   private string BuildInfoFilter()
   {
      StringBuilder filter = new StringBuilder();
      
      // Company filter
      if (ddlCompanyList.SelectedValue != "0")
         filter.AppendFormat("OrganizationId = {0}", ddlCompanyList.SelectedValue);

      // Fw Type + Fw filter
      if (ddlFwType.SelectedValue != "0")
      {
         if (filter.Length > 0) filter.Append(" AND ");
         filter.AppendFormat("FwTypeId = {0}", ddlFwType.SelectedValue);

         if (ddlFirmware.SelectedValue != "0")
         {
            filter.AppendFormat(" AND FwId = {0}", ddlFirmware.SelectedValue);
         }
      }

      // Port filter
      if (!String.IsNullOrEmpty(txtPort.Text))
      {
         if (filter.Length > 0) filter.Append(" AND ");
         filter.AppendFormat("Port = '{0}'", txtPort.Text.Trim());
      }

      return filter.ToString();
   }

   protected void ddlFirmware_SelectedIndexChanged(object sender, EventArgs e)
   {
   }

   protected void ddlCompanyList_SelectedIndexChanged(object sender, EventArgs e)
   {
      gdvwData.PageIndex = 0;
   }

   protected void btnExport_Click(object sender, EventArgs e)
   {
      try
      {
         gdvwData.AllowPaging = false;
         PrepareGridViewForExport(gdvwData);
         ExportGrid();
      }
      catch (Exception ex)
      {
         lblError.Text = ex.Message;
      }
      finally
      {
         gdvwData.AllowPaging = true;
      }
   }

   protected void btnView_Click(object sender, EventArgs e)
   {
      try
      {
         sqlInfo.FilterExpression = BuildInfoFilter();
         gdvwData.PageIndex = 0;
      }
      catch (Exception ex)
      {
         lblError.Text = "Error filtering info page :: " + ex.Message;
      }
   }

   /// <summary>
   /// For Excel file export
   /// </summary>
   /// <param name="control"></param>
   public override void VerifyRenderingInServerForm(Control control)
   {
      //base.VerifyRenderingInServerForm(control);
   }

   protected void gdvwData_PageIndexChanging(object sender, GridViewPageEventArgs e)
   {
      try
      {
         gdvwData.PageIndex = e.NewPageIndex;
         gdvwData.DataBind();
      }
      catch (Exception ex)
      {
         lblError.Text = "Error loading info page :: " + ex.Message;
      }
   }

   private void PrepareGridViewForExport(Control gv)
   {
      Literal l = new Literal();
      for (int i = 0; i < gv.Controls.Count; i++)
      {
         switch (gv.Controls[i].GetType().ToString())
         {
            case "LinkButton":
               l.Text = (gv.Controls[i] as LinkButton).Text;
               gv.Controls.Remove(gv.Controls[i]);
               gv.Controls.AddAt(i, l);
               break;

            case "TextBox":
               l.Text = (gv.Controls[i] as TextBox).Text;
               gv.Controls.Remove(gv.Controls[i]);
               gv.Controls.AddAt(i, l);
               break;

            case "HyperLink":
               l.Text = (gv.Controls[i] as HyperLink).Text;
               gv.Controls.Remove(gv.Controls[i]);
               gv.Controls.AddAt(i, l);
               break;

            case "DropDownList":
               l.Text = (gv.Controls[i] as DropDownList).SelectedItem.Text;
               gv.Controls.Remove(gv.Controls[i]);
               gv.Controls.AddAt(i, l);
               break;

            case "CheckBox":
               l.Text = (gv.Controls[i] as CheckBox).Checked ? "True" : "False";
               gv.Controls.Remove(gv.Controls[i]);
               gv.Controls.AddAt(i, l);
               break;
         }

         if (gv.Controls[i].HasControls())
         {
            PrepareGridViewForExport(gv.Controls[i]);
         }
      }
   }

   private void ExportGrid()
   {
      Response.ClearContent();
      Response.AppendHeader("content-disposition", "attachment; filename=BoxesReport.xls");
      Response.ContentType = "application/ms-excel";
      Response.Charset = "";
      Response.Cache.SetCacheability(HttpCacheability.NoCache);
      Response.BufferOutput = true;

      System.IO.StringWriter stringWriter = new System.IO.StringWriter();
      System.Web.UI.HtmlTextWriter htmlWriter = new HtmlTextWriter(stringWriter);

      //HtmlForm frm = new HtmlForm();
      //gdvwData.Parent.Controls.Add(frm);
      //frm.Attributes["runat"] = "server";
      //frm.Controls.Add(gdvwData);
      //frm.RenderControl(htmlWriter);

      gdvwData.RenderControl(htmlWriter);
      Response.Write(stringWriter.ToString());

      htmlWriter.Close();
      stringWriter.Close();
      //Response.Flush();
      Response.End();
   }
}
