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
using SentinelFM;

public partial class frmBoxFindMsgs : System.Web.UI.Page
{
   protected SentinelFMSession sn = null;
   protected clsUtility objUtil;

   protected void Page_Load(object sender, EventArgs e)
   {
      sn = (SentinelFMSession)Session["SentinelFMSession"];
      if ((sn == null) || (sn.UserName == ""))
      {
         this.LabelMessage.Text = "Error loading session";
         return;
      }
   }

   protected void Load_Click(object sender, EventArgs e)
   {
      try
      {
         string result = "";
         ServerDBHistory.DBHistory dbh = new ServerDBHistory.DBHistory();
         objUtil = new clsUtility(sn);
         if (objUtil.ErrCheck(dbh.GetBoxMissingMessages(sn.UserID, sn.SecId, Convert.ToInt32(this.BoxId.Text), Convert.ToDateTime(this.DateFrom.Text), Convert.ToDateTime(this.DateTo.Text), ref result), false))
            if (objUtil.ErrCheck(dbh.GetBoxMissingMessages(sn.UserID, sn.SecId, Convert.ToInt32(this.BoxId.Text), Convert.ToDateTime(this.DateFrom.Text), Convert.ToDateTime(this.DateTo.Text), ref result), true))
            {
               return;
            }

         this.Messages.InnerHtml = result;
      }
      catch (Exception ex)
      {
         this.LabelMessage.Text = ex.Message;
      }
   }
}
