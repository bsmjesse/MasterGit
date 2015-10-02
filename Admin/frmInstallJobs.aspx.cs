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
using System.IO;
using System.Xml;
using VLF.CLS;

public partial class frmInstallJobs : BasePage
{
   protected void Page_Load(object sender, EventArgs e)
   {
      if (!Page.IsPostBack)
         LoadJobs();
   }

   protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
   {
      try
      {
         //string xml = "";
         
         //using (ServerDBSystem.DBSystem sysConfig = new ServerDBSystem.DBSystem())
         //{
         //   if (objUtil.ErrCheck(sysConfig.InstallJob_Get(sn.UserID, sn.SecId, Convert.ToInt32(GridView1.DataKeys[GridView1.SelectedIndex].Value), ref xml), false))
         //      if (objUtil.ErrCheck(sysConfig.InstallJob_Get(sn.UserID, sn.SecId, Convert.ToInt32(GridView1.DataKeys[GridView1.SelectedIndex].Value), ref xml), true))
         //      {
         //         ShowMessage(LabelMessage, "Add Install Job Failed", System.Drawing.Color.Red);
         //      }
         //}
         //File.WriteAllText(Server.MapPath("~/tempinst.xml"), xml);
         // todo: open xml file using xslt
         //Page.ClientScript.RegisterClientScriptBlock(this.GetType(), "OpenInstallJob", script, true);
         Response.Redirect("frmInstallJobOpen.aspx?id=" + GridView1.DataKeys[GridView1.SelectedIndex].Value.ToString());
      }
      catch (Exception ex)
      {
         ShowMessage(LabelMessage, ex.Message, System.Drawing.Color.Red);
      }
   }

   protected void GridView1_PageIndexChanging(object sender, GridViewPageEventArgs e)
   {
      GridView1.PageIndex = e.NewPageIndex;
      LoadJobs();
   }

   protected void btnSubmitFile_Click(object sender, EventArgs e)
   {
      try
      {
         if (!FileUploadJobXml.HasFile)
         {
            ShowMessage(LabelMessage, "Please select XML job file", System.Drawing.Color.Red);
         }
         // todo: read the xml file, extract data and upload to DB
         //LabelMessage.Text = FileUploadJobXml.FileName;
         string xml = "";
         using (StreamReader sr = new StreamReader(FileUploadJobXml.FileContent))
         {
            xml = sr.ReadToEnd();
         }

         using (ServerDBSystem.DBSystem sysConfig = new ServerDBSystem.DBSystem())
         {
            if(objUtil.ErrCheck(sysConfig.InstallJob_Add(sn.UserID, sn.SecId, xml, TextDescription.Text.Trim(), "New", DateTime.Now), false))
               if (objUtil.ErrCheck(sysConfig.InstallJob_Add(sn.UserID, sn.SecId, xml, TextDescription.Text.Trim(), "New", DateTime.Now), true))
               {
                  ShowMessage(LabelMessage, "Add Install Job Failed", System.Drawing.Color.Red);
               }
         }
         LoadJobs();
      }
      catch (Exception ex)
      {
         ShowMessage(LabelMessage, ex.Message, System.Drawing.Color.Red);
      }
   }

   private void LoadJobs()
   {
      try
      {
         string xml = "";
         using (ServerDBSystem.DBSystem sysConfig = new ServerDBSystem.DBSystem())
         {
            if (objUtil.ErrCheck(sysConfig.InstallJobs_GetAll(sn.UserID, sn.SecId, ref xml), false))
            {
               if (objUtil.ErrCheck(sysConfig.InstallJobs_GetAll(sn.UserID, sn.SecId, ref xml), true))
               {
                  ShowMessage(LabelMessage, "Get Install Job Failed", System.Drawing.Color.Red);
                  return;
               }
            }
            else
               if (String.IsNullOrEmpty(xml))
               {
                  ShowMessage(LabelMessage, "Result XML is empty!", System.Drawing.Color.Red);
                  return;
               }
         }

         DataSet ds = new DataSet();
         ds.ReadXml(new StringReader(xml));
         if (ds != null && ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
         {
            GridView1.DataSource = ds.Tables[0];
            GridView1.DataBind();
         }
      }
      catch (Exception ex)
      {
         ShowMessage(LabelMessage, ex.Message, System.Drawing.Color.Red);
      }
   }

   protected void GridView1_Sorting(object sender, GridViewSortEventArgs e)
   {
      //LoadJobs();
   }
}
