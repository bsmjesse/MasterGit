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

namespace SentinelFM
{
   public partial class frmResetPassword : SentinelFMBasePage
   {

      
      
      protected ServerDBUser.DBUser dbu;
      public int userIdUpdated = 0;
      public string msgPsw_Medium ="" ;
      public string msgPsw_Weak = "";
      public string msgPsw_MoreCharacters = "";
      public string msgPsw_Strong = "";
      public string msgPsw_TypePassword = "";
      public string PswMsgRule = "";




      protected void Page_Load(object sender, EventArgs e)
      {
         
         msgPsw_Medium = (string)base.GetLocalResourceObject("msgPsw_Medium");
         msgPsw_Weak = (string)base.GetLocalResourceObject("msgPsw_Weak");
         msgPsw_MoreCharacters = (string)base.GetLocalResourceObject("msgPsw_MoreCharacters");
         msgPsw_Strong = (string)base.GetLocalResourceObject("msgPsw_Strong");
         msgPsw_TypePassword = (string)base.GetLocalResourceObject("msgPsw_TypePassword");
         userIdUpdated = Convert.ToInt32(Request.QueryString["UserId"]);
         this.txtNewPassword.Attributes.Add("onkeyup", "return passwordChanged();");
         PswMsgRule = (string)base.GetLocalResourceObject("lblPswMsgRuleResource1.Text");
         this.lblPswMsgRule.Text = PswMsgRule;
         this.lblPswMsgRule.Visible = true;
        
      }
      protected void cmdSave_Click(object sender, EventArgs e)
      {

         
         dbu = new ServerDBUser.DBUser();

         string txtPasswordStatus = Request.Form["txtPasswordStatus"];
         if (txtPasswordStatus == "")
            return; 
         else if (txtPasswordStatus != "1")
         {
            this.lblPswMsg.Text = (string)base.GetLocalResourceObject("msgWeakPassword");
            this.lblPswMsg.Visible = true;
            return;
         }

          int resultChange = dbu.ResetPasswordBySuperUser(sn.UserID, userIdUpdated, sn.SecId, this.txtNewPassword.Text);
          if (objUtil.ErrCheck(resultChange, false))
              if (objUtil.ErrCheck(resultChange, true))
            {
                if (resultChange == 21)
                {
                    this.lblPswMsg.Text = (string)base.GetLocalResourceObject("AddUserPasswordFailed21");
                }
                else if (resultChange == 22)
                {
                    this.lblPswMsg.Text = (string)base.GetLocalResourceObject("AddUserPasswordFailed22");
                }
                else
                {
                    this.lblPswMsg.Text = (string)base.GetLocalResourceObject("MsgUpdateFailed");
                }
               this.lblPswMsg.Visible = true;
               return;
            }


         this.lblPswMsg.Visible = true;
         this.lblPswMsg.Text = (string)base.GetLocalResourceObject("cmdPswUpdated");
         


      }
}
}
