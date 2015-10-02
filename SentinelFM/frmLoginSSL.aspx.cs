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

public partial class frmLoginSSL : System.Web.UI.Page
{
   protected void Page_Load(object sender, EventArgs e)
   {
      // Redirect for SSL Security
      string sDomainName = @ConfigurationSettings.AppSettings["DomainName"];
      string sPath = "https://" + sDomainName + "/login.aspx";
      Response.Redirect("https://sentinelfm.com/new/login.aspx");  

   }
}
