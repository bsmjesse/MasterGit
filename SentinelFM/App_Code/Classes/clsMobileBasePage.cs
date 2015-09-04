using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

/// <summary>
/// Summary description for clsMobileBasePage
/// </summary>
namespace SentinelFM
{
    public class clsMobileBasePage : System.Web.UI.Page
    {
        protected SentinelFMSession sn = null;
        protected clsUtility objUtil;
        protected override void OnLoad(EventArgs e)
        {
            try
            {
                //Clear IIS cache
                Response.Cache.SetCacheability(HttpCacheability.NoCache);
                Response.Cache.SetExpires(DateTime.Now);
                sn = (SentinelFMSession)Session["SentinelFMSession"];
                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                {
                    RedirectToLogin();
                    return;
                }
                objUtil = new clsUtility(sn);
                

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form: " + Page.GetType().Name));

            }

            base.OnLoad(e);
        }


        protected void Page_Init(object sender, System.EventArgs e)
        {
            AddIPhoneHeaderTags();
        }


        public clsMobileBasePage()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private void AddIPhoneHeaderTags()
        {

            string ua = Request.UserAgent;

            if (ua != null && (ua.Contains("iPhone") || ua.Contains("iPod")))
            {

                HtmlMeta meta = new HtmlMeta();

                meta.Name = "viewport";

                meta.Content = "width=330, user-scalable=yes";

                Page.Header.Controls.Add(meta);



                //HtmlLink link = new HtmlLink();

                //link.Attributes["rel"] = "apple-touch-icon";

                //link.Href = "favicon.ico";

                //Page.Header.Controls.Add(link);

            }

        }


        private void RedirectToLogin()
        {
            Session.Abandon();
            Response.Redirect("MobileLogin.aspx");
            return;
        }


    }

}