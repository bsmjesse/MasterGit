using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;



namespace SentinelFM
{
    public partial class Messages_frmNewGarminMsgs : SentinelFMBasePage
    {
        public int boxId = 0;
        public bool sendToAll = false;  

        protected void Page_Load(object sender, EventArgs e)
        {
            boxId = Convert.ToInt32(Request.QueryString["boxId"]);
            sendToAll = Convert.ToBoolean(Request.QueryString["sendToAll"]);
        }
        protected void cmdSent_Click(object sender, EventArgs e)
        {

            string paramList = VLF.CLS.Util.MakePair("TXT", this.txtMessageBody.Text);
            paramList += VLF.CLS.Util.MakePair("YESNO", "false");
            

            using (DBGarmin.Garmin garmin = new DBGarmin.Garmin())
            {
            

                if (!sendToAll)
                {
                    if (objUtil.ErrCheck(garmin.SendTextMessage(sn.UserID, sn.SecId, Convert.ToInt32(boxId), Convert.ToInt32(this.cboMessageType.SelectedItem.Value), paramList), false))
                        if (objUtil.ErrCheck(garmin.SendTextMessage(sn.UserID, sn.SecId, Convert.ToInt32(boxId), Convert.ToInt32(this.cboMessageType.SelectedItem.Value), paramList), true))
                        {
                            this.lblMessage.Text = "Send message failed";
                            return;
                        }
                }
                else
                {
                    foreach (DataRow dr in sn.Message.DsGarminVehicles.Tables[0].Rows)
                    {

                        if (objUtil.ErrCheck(garmin.SendTextMessage(sn.UserID, sn.SecId, Convert.ToInt32(dr["boxId"]), Convert.ToInt32(this.cboMessageType.SelectedItem.Value), paramList), false))
                           if (objUtil.ErrCheck(garmin.SendTextMessage(sn.UserID, sn.SecId, Convert.ToInt32(dr["boxId"]), Convert.ToInt32(this.cboMessageType.SelectedItem.Value), paramList), true))
                            {
                                continue;
                            }
                    }

                }

            }

            Response.Write("<script language='javascript'>window.close()</script>");

            //this.lblMessage.Text = "Message queued successfully";
            //this.cmdSent.Enabled = false;  
        }
}
}
