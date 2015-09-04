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
   public partial class frmHome : SentinelFMBasePage
   {
      protected void Page_Load(object sender, EventArgs e)
      {

          this.imgHomePage.ImageUrl = "../SentinelFM_Themes/" + Session["Host"].ToString() + "/images/home_page_logo.png";
      }
   }
}
