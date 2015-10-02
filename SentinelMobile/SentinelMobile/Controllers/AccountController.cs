using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;
using SentinelMobile.Models;
using System.Data;

namespace SentinelMobile.Controllers
{

    [Authorize]
    public class AccountController : Controller
    {
        //
        // GET: /Account/Index

        public ActionResult Index()
        {
            return View();
        }

        //
        // GET: /Account/Login

        [AllowAnonymous]
        public ActionResult Login()
        {
            if (HttpContext.Request["s"] != null && HttpContext.Request["s"] == "web")
            {

                HttpCookie userCookie = Request.Cookies["xxxxxuxxxxx"];
                if (userCookie != null)
                {
                    string uname = userCookie["u"];
                    string password = userCookie["p"];
                    password = password.Replace("dakuadhhkkll3w9299766lknlo", "");
                    userCookie = new HttpCookie("xxxxxuxxxxx");
                    userCookie.Expires = DateTime.Now.AddYears(-1);
                    userCookie.Path = "/mobile";
                    Response.Cookies.Add(userCookie);
                    if (uname != "" && password != "")
                    {
                        //SecurityManager.SecurityManager sec = new SecurityManager.SecurityManager();
                        //int errCode = sec.LoginMD5Extended("", uname, password, "", ref uid, ref secId, ref superOrganizationId);
                        //return RedirectPermanent("~/Account/Good?u=" + uname + "&p=" + password);

                        string IpAddr = "";
                        try
                        {

                            if (HttpContext.Request.UserHostAddress.Trim() != "")
                                IpAddr = HttpContext.Request.UserHostAddress.Trim();
                            else if (HttpContext.Request.ServerVariables["REMOTE_ADDR"].Trim() != "")
                                IpAddr = HttpContext.Request.ServerVariables["REMOTE_ADDR"].Trim();
                            else if (HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Trim() != "")
                                IpAddr = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Trim();

                        }
                        catch
                        {
                        }
                        if (SentinelMobile.Models.SentinelFM.ValidateUser(uname, password, IpAddr, ""))
                        {
                            FormsAuthentication.SetAuthCookie(uname, false);

                            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
                            if (user == null)
                            {
                                return null;
                            }

                            if (user.FleetType.ToLower() == "hierarchy")
                            {
                                string constr = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ToString();
                                OrganizationHierarchy.Hierarchy _hie = new OrganizationHierarchy.Hierarchy(constr);

                                user.DefaultFleet = _hie.GetFleetIdByNodeCode(user.DefaultNodeCode, user.OrganizationId);
                            }

                            if (user.SuperOrganizationId == user.OrganizationId)
                            {
                                user.IsSuperUser = true;
                                return RedirectPermanent("~/Account/SuperOrganizationMenu");
                            }
                            /*else if (Url.IsLocalUrl(returnUrl))
                            {
                                return RedirectPermanent(returnUrl);
                            }*/
                            else
                            {
                                //return RedirectPermanent("~/Home/FleetView#vehicleList");                        
                                return RedirectPermanent("~/Home/About");
                            }
                        }
                    }
                }
            }
            
            ViewBag.LastUpdatedCustomTheme = System.IO.File.GetLastWriteTime(Server.MapPath("~/Content/themes/custom/sfmjquerymobileCustom.css")).ToString("yyyyMMddHHmmss");

            string language = (Request.Cookies["language"]==null) ? "en" : Request.Cookies["language"].Value.ToString();
            ViewBag.authseed = Convert.ToString(new System.Random().NextDouble());
            if (!string.IsNullOrEmpty(language))
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language);
            }
            else
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            return View("");
        }

        //
        // POST: /Account/Login

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {
            ViewBag.LastUpdatedCustomTheme = System.IO.File.GetLastWriteTime(Server.MapPath("~/Content/themes/custom/sfmjquerymobileCustom.css")).ToString("yyyyMMddHHmmss");

            //if (model.UserName.ToLower() == "hosdriver")
                //return RedirectToAction("Index", "HOS");

            string language = (Request.Cookies["language"] == null) ? "en" : Request.Cookies["language"].Value.ToString();
            if (!string.IsNullOrEmpty(language))
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo(language);
            }
            else
                System.Threading.Thread.CurrentThread.CurrentUICulture = new System.Globalization.CultureInfo("en");
            
            string authseed = HttpContext.Request["txtRnd"].ToString();
            
            ViewBag.authseed = authseed;

            if (ModelState.IsValid)
            {
                string IpAddr = "";
                try
                {
                    
                    if (HttpContext.Request.UserHostAddress.Trim() != "")
                        IpAddr = HttpContext.Request.UserHostAddress.Trim();
                    else if (HttpContext.Request.ServerVariables["REMOTE_ADDR"].Trim() != "")
                        IpAddr = HttpContext.Request.ServerVariables["REMOTE_ADDR"].Trim();
                    else if (HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Trim() != "")
                        IpAddr = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"].Trim();

                }
                catch
                {
                }

                string hashPassword = string.Empty;

                if (SentinelMobile.Models.SentinelFM.ValidateUser(model.UserName, model.HashPassword, IpAddr, authseed))
                {                    
                    FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                    User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
                    if (user == null)
                    {
                        return null;
                    }

                    if (user.FleetType.ToLower() == "hierarchy")
                    {
                        string constr = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ToString();
                        OrganizationHierarchy.Hierarchy _hie = new OrganizationHierarchy.Hierarchy(constr);

                        user.DefaultFleet = _hie.GetFleetIdByNodeCode(user.DefaultNodeCode, user.OrganizationId);
                    }

                    if (user.SuperOrganizationId == user.OrganizationId)
                    {
                        user.IsSuperUser = true;
                        return RedirectPermanent("~/Account/SuperOrganizationMenu");
                    }
                    /*else if (Url.IsLocalUrl(returnUrl))
                    {
                        return RedirectPermanent(returnUrl);
                    }*/
                    else
                    {
                        //return RedirectPermanent("~/Home/FleetView#vehicleList");                        
                        return RedirectPermanent("~/Home/About");
                    }
                }
                else
                {
                    //Hos
                    HOSController hosControl = new HOSController();
                    User driver = hosControl.LoginDriver(model.UserName, model.Password, model.DomainName);
                    if (driver != null)
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);
                        HttpContext.Session.Add("SentinelUser", driver);
                        return RedirectPermanent("~/Home/About");
                    }

                    ModelState.AddModelError("", SentinelMobile.Resources.Resources.UserNamePasswordIncorrect);
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
            
        }

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            HttpContext.Session["SentinelUser"] = null;
            

            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        //
        // POST: /Account/Register

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(RegisterModel model)
        {
            if (ModelState.IsValid)
            {
                // Attempt to register the user
                MembershipCreateStatus createStatus;
                Membership.CreateUser(model.UserName, model.Password, model.Email, passwordQuestion: null, passwordAnswer: null, isApproved: true, providerUserKey: null, status: out createStatus);

                if (createStatus == MembershipCreateStatus.Success)
                {
                    FormsAuthentication.SetAuthCookie(model.UserName, createPersistentCookie: false);
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    ModelState.AddModelError("", ErrorCodeToString(createStatus));
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePassword

        public ActionResult ChangePassword()
        {
            return View();
        }

        //
        // POST: /Account/ChangePassword

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {

                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                try
                {
                    MembershipUser currentUser = Membership.GetUser(User.Identity.Name, userIsOnline: true);
                    changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                }
                catch (Exception)
                {
                    changePasswordSucceeded = false;
                }

                if (changePasswordSucceeded)
                {
                    return RedirectToAction("ChangePasswordSuccess");
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect or the new password is invalid.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //
        // GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        [Authorize]
        public ActionResult SuperOrganizationMenu()
        {
            ViewBag.LastUpdatedCustomTheme = System.IO.File.GetLastWriteTime(Server.MapPath("~/Content/themes/custom/sfmjquerymobileCustom.css")).ToString("yyyyMMddHHmmss");
            
            /*User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                return null;
            }

            string constr = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ToString();
            ViewBag.DsOrganizations = SentinelFM.GetSuperOrganizationList(constr, user.SuperOrganizationId);*/

            return View();
        }

        [Authorize]
        public ActionResult SuperOrganizationLogin(int organizationId)
        {
            string userName = "";
            string hashPassword = "";
            int uid = -1;
            string secId = "";
            int superOrganizationId = 1;

            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                return null;
            }
            
            ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();
            dbo.GetLoginCredentialsWithinSameGroup(user.UserId, user.SecId, organizationId, ref userName, ref hashPassword);

            SecurityManager.SecurityManager sec = new SecurityManager.SecurityManager();
            int errCode = sec.LoginMD5Extended("", userName, hashPassword, "", ref uid, ref secId, ref superOrganizationId);

            user.UserId = uid;
            user.SecId = secId;
            //user.SuperOrganizationId = superOrganizationId;
            user.OrganizationId = organizationId;
            user.UserName = userName;
            
            
            //sn.Password = hashPassword;
            //sn.Key = "";

            user.ExistingPreference();
            
            
            
            //return RedirectPermanent("~/Home/FleetView#vehicleList");
            return RedirectPermanent("~/Home/About");
        }

        [Authorize]
        [OutputCache(Duration = 0, VaryByParam = "None")]
        public ViewResult _organizatoinList(int? pageIndex, int? isSearch, string searchString)
        {


            User user = HttpContext.Session != null ? (User)HttpContext.Session["SentinelUser"] : null;
            if (user == null)
            {
                return null;
            }

            string constr = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ToString();

            DataSet dsOrg = SentinelMobile.Models.SentinelFM.GetSuperOrganizationList(constr, user.SuperOrganizationId);

            if (isSearch != null && isSearch == 1)
            {
                string filter = String.Format("OrganizationName like '%{0}%'", searchString.Replace("'", "''"));
                DataRow[] drCollections = null;
                drCollections = dsOrg.Tables[0].Select(filter, "OrganizationName");

                DataTable dt = dsOrg.Tables[0].Clone();

                if (drCollections != null && drCollections.Length > 0)
                {
                    foreach (DataRow dr in drCollections)
                    {
                        dt.ImportRow(dr);
                    }
                }

                DataSet ds = new DataSet();
                ds.Tables.Add(dt);

                ViewBag.DsOrganizations = ds;
            }
            else
            {
                ViewBag.DsOrganizations = dsOrg;
            }
            ViewBag.PageIndex = pageIndex ?? 1;
            ViewBag.PageSize = 30;

            return View();
        }

        #region Status Codes
        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }
        #endregion
    }
}
