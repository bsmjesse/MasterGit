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

public partial class Map_frmProgress : SentinelFMBasePage
{
   protected void Page_Load(object sender, EventArgs e)
   {
      if (sn != null)
      {
         VLF.CLS.Interfaces.CommandStatus cmdStatus = (VLF.CLS.Interfaces.CommandStatus)Session["ReeferCommandStatus"];
         if (cmdStatus == VLF.CLS.Interfaces.CommandStatus.Ack ||
            cmdStatus == VLF.CLS.Interfaces.CommandStatus.CommTimeout ||
            cmdStatus == VLF.CLS.Interfaces.CommandStatus.Pending)
            this.ClientScript.RegisterStartupScript(this.GetType(), "", "window.close();", true);
      }
   }
}
