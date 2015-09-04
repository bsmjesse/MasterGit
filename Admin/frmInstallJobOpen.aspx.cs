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
using System.IO;
using System.Text;
using System.Xml;

public partial class frmInstallJobOpen : BasePage
{
   protected void Page_Load(object sender, EventArgs e)
   {
      try
      {
         string xml = "";
         if (!IsPostBack)
         {
            int id = 0;
            if (this.Request.QueryString["id"] != null)
            {
               if (!Int32.TryParse(this.Request.QueryString["id"], out id))
               {
                  ShowMessage(LabelMessage, "Invalid Query String Parameter: id= " + id.ToString(), Color.Red);
                  return;
               }
            }
            else
            {
               ShowMessage(LabelMessage, "Query String Parameter [id] is NULL", Color.Red);
               return;
            }
            using (ServerDBSystem.DBSystem sysConfig = new ServerDBSystem.DBSystem())
            {
               if (objUtil.ErrCheck(sysConfig.InstallJob_Get(sn.UserID, sn.SecId, id, ref xml), false))
                  if (objUtil.ErrCheck(sysConfig.InstallJob_Get(sn.UserID, sn.SecId, id, ref xml), true))
                  {
                     ShowMessage(LabelMessage, "Add Install Job Failed", System.Drawing.Color.Red);
                  }
            }

            // Send the XML file to the web browser for open.
            Response.Clear();
            Response.AppendHeader("Content-Disposition", String.Format("filename={0}.xml", id));
            Response.AppendHeader("Content-Length", Encoding.ASCII.GetBytes(xml).Length.ToString());
            Response.ContentType = "application/octet-stream";
            Response.BinaryWrite(Encoding.ASCII.GetBytes(xml));
         }
      }
      catch (Exception ex)
      {
         ShowMessage(LabelMessage, ex.Message, System.Drawing.Color.Red);
      }
   }
}
