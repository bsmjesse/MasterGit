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
  
public partial class RedirectPage : SentinelFMBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        string sDomainName = @ConfigurationSettings.AppSettings["DomainName"];
        string sPath = "https://" + sDomainName+"/login.aspx";
        Response.Redirect(sPath);  
    }
}
