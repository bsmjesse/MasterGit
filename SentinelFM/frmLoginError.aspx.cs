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
using System.Configuration;
using VLF.CLS;

namespace SentinelFM
{
    public partial class frmLoginError : SentinelFMBasePage
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            string IpAddr = Request.ServerVariables["remote_addr"];
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "A potentially dangerous Request - IP:" + IpAddr));

            //try
            //{
            //    MailLib.EMailMessageAuth eMail = new MailLib.EMailMessageAuth("SentinelFM","sentinel","192.168.8.21");

            //    //eMail.SendMail("prodsupport@bsmwireless.com", "SentinelFM locked for IP:", "");
            //    eMail.SendMail("mvaksman@bsmwireless.com", "A potentially dangerous Request - IP:" + IpAddr, "");
                
            //}
            //catch (Exception ex)
            //{
            //    Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Info, "Error in sending Email 'Account Locked'."));
            //}


            

        }
    }
}
