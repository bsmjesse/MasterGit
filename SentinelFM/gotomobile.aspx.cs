using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SentinelFM
{
    public partial class gotomobile : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            SentinelFMSession sn = null;
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                Response.Redirect("/mobile");
            }
            else
            {
                string username = string.Empty;
                string password = string.Empty;
                //string dt = Request["dt"].ToString();
                ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();
                dbo.GetLoginCredentialsWithinSameGroup(sn.UserID, sn.SecId, sn.User.OrganizationId, ref username, ref password);  
                
                HttpCookie userCookie = new HttpCookie("xxxxxuxxxxx");
                userCookie.HttpOnly = true;
                userCookie["u"] = username;
                userCookie["p"] = password + "dakuadhhkkll3w9299766lknlo";
                userCookie.Expires = DateTime.Now.AddMinutes(1);
                //userCookie.Expires = DateTime.Parse(dt).AddMinutes(1);
                userCookie.Path = "/mobile";
                Response.Cookies.Add(userCookie);

                Response.Redirect("mobile/Account/Login?s=web");
            }           
              
        }
    }
}