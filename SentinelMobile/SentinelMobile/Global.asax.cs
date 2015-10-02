using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace SentinelMobile
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode, 
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();

            WebApiConfig.Register(GlobalConfiguration.Configuration);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e)
        {
            //Only access session state if it is available
            if (Context.Handler is IRequiresSessionState || Context.Handler is IReadOnlySessionState)
            {
                //If we are authenticated AND we dont have a session here.. redirect to login page.
                HttpCookie authenticationCookie = Request.Cookies[FormsAuthentication.FormsCookieName];
                if (authenticationCookie != null)
                {
                    FormsAuthenticationTicket authenticationTicket = FormsAuthentication.Decrypt(authenticationCookie.Value);
                    if (!authenticationTicket.Expired)
                    {
                        //of course.. replace ANYKNOWNVALUEHERETOCHECK with "UserId" or something you set on the login that you can check here to see if its empty.
                        if (Session["SentinelUser"] == null)
                        {
                            //This means for some reason the session expired before the authentication ticket. Force a login.
                            FormsAuthentication.SignOut();
                            Response.Redirect(FormsAuthentication.LoginUrl, true);
                            return;
                        }
                    }
                }
            }
        }

    }
}