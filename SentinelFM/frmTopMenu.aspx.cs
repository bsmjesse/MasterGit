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
using System.Globalization;
using System.IO;

namespace SentinelFM
{
    public partial class frmTopMenu : System.Web.UI.Page
    {
        protected string ReportServerURL;
        protected SentinelFMSession sn = null;
        protected clsUtility objUtil;

        public bool IsHydroQuebec = false;
        public string strContactUs = "";
      
        public bool HGI = false;
        protected  void Page_Init(object sender, EventArgs e)
        {
            try
                {
                  
                
                imgProdLogo.ImageUrl = "SentinelFM_Themes/" + Session["Host"].ToString() + "/images/top_menu_logo.png";
                strContactUs = "SentinelFM_Themes/" + Session["Host"].ToString() + "/forms/frmContactUs.aspx";

                sn = (SentinelFMSession)Session["SentinelFMSession"];
               
                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                {
                    if (Page.Request.QueryString["sn"] != null)
                    {
                        SentinelFMSessionBase snb = SentinelFMSession.GetSessionBaseByKey((string)Page.Request.QueryString["sn"]);
                        sn = new SentinelFMSession(snb);
                        if (sn == null)
                            return;
                        Session["SentinelFMSession"] = sn;
                    }
                    else if (Page.Request.UrlReferrer != null && Page.Request.UrlReferrer.Query != null && Page.Request.UrlReferrer.Query.StartsWith("?sn="))
                    {
                        string qs = Page.Request.UrlReferrer.Query.Substring(4);
                        SentinelFMSessionBase snb = SentinelFMSession.GetSessionBaseByKey(qs);
                        sn = new SentinelFMSession(snb);
                        if (sn == null)
                            return;
                        Session["SentinelFMSession"] = sn;
                    }
                    else
                    {
                        RedirectToLogin();
                        return;
                    }

                    sn.User.ExistingPreference(sn);
                    sn.User.GetGuiControlsInfo(sn);
                }
                //if (Session["CompanyLogo"] != null)
                //  this.imgProdLogo.ImageUrl = "images/" + Session["CompanyLogo"].ToString().TrimEnd();



   
                string cScript = "window.setInterval('keepMeAlive()', 100000)";

	        if (!(sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957))
                    ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "KeepAlive", cScript, true);
                else
                    Response.AddHeader("Refresh", Convert.ToString((Session.Timeout * 60) + 10));


            if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 1000151)
                    IsHydroQuebec = true;
                else
                    IsHydroQuebec = false;

                if (!Page.IsPostBack)
                {
                    if (sn.User.OrganizationName == "" || sn.User.OrganizationName == null)
                    {
                        sn.User.ExistingPreference(sn);
                        sn.User.GetGuiControlsInfo(sn);
                    }
                }

                HtmlGenericControl hgc = new HtmlGenericControl("style");
                hgc.Attributes.Add("type", "text/css");

                String filePath = HttpContext.Current.Server.MapPath("GlobalStyle.css");

                StreamReader reader = new StreamReader(filePath);
                string content = reader.ReadToEnd();
                reader.Close();
                if (sn.User.MenuColor != "")
                    hgc.InnerText = content.Replace("#009933", sn.User.MenuColor);
                else
                    hgc.InnerText = content;

                if (sn.InterfacePrefrence == (Int16)VLF.CLS.Def.Enums.InterfacePrefrence.Lite || sn.InterfacePrefrence == (Int16)VLF.CLS.Def.Enums.InterfacePrefrence.Both)
                    this.lnkLite.Enabled = true;
                else
                    this.lnkLite.Enabled = false;

                this.lnkLite.Enabled = true;

                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                StringWriter tw = new StringWriter(sb);
                HtmlTextWriter hw = new HtmlTextWriter(tw);
                hgc.RenderControl(hw);
                Response.Write(sb.ToString());
            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
                RedirectToLogin();
            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {

                if (sn.SelectedLanguage == "fr-CA")
                {
                    lnkGuide.Visible = true;
                }
                else
                {
                    lnkGuide.Visible = false;
                }

                if (sn.User.OrganizationId == 952 || sn.User.OrganizationId == 999988 || sn.User.OrganizationId == 952) // No lite link 
                {
                    lnkLite.Visible = false;
                    lnkLiteSepertor.Visible = false;
                }

                if (sn.User.OrganizationId == 999728 || sn.User.OrganizationId == 480) // Only Burnbrae has the mobile link
                {
                    lnkMobile.Visible = true;
                    lnkMobileSepertor.Visible = true;
                }

                bool IsReset = false;
                if (Session["IsReset"] != null)
                {
                    IsReset = (bool)(Session["IsReset"]);
                    if (IsReset) sn.UserID = 0;
                }
                objUtil = new clsUtility(sn);
                ServerDBUser.DBUser vdbu = new ServerDBUser.DBUser();
                if (vdbu.ValidateLockedUser(sn.UserID) == 1)
                {
                    //Response.Write("<script>alert('Your account is locked, please reset your password'); __doPostBack('mnuMenu','Configuration/frmPreference.aspx')</script>");
                    Response.Redirect("Configuration/frmPreference.aspx?errormsg=1");
                    return;
                }
                
                Session["CompanyLogo"] = sn.User.CompanyLogo;
                Session["CompanyURL"] = sn.CompanyURL;

                string sURL = @ConfigurationSettings.AppSettings["ReportServerURL"];

                ReportServerURL = sURL + "Login.aspx";
                

                if (sn.User.OrganizationName.TrimEnd().Length > 25)
                    lnkUser.Text = sn.UserName + ", " + sn.User.OrganizationName.TrimEnd().Substring(0, 25) + "...";

                else
                    lnkUser.Text = sn.UserName + ", " + sn.User.OrganizationName.TrimEnd();

                lnkUser.ToolTip = sn.UserName + ", " + sn.User.OrganizationName.TrimEnd();

                if (!Page.IsPostBack)
                {


                    if (Session["PreferredCulture"] != null)
                        sn.SelectedLanguage = Session["PreferredCulture"].ToString();

                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmMenu, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);




                    int itemsCount = mnuMenu.Items.Count;

                    for (int i = 0; i < itemsCount; i++)
                    {

                        if (
                              (mnuMenu.Items[i].Value == "Map/frmMain.aspx" && !sn.User.ControlEnable(sn, 66))
                              || (mnuMenu.Items[i].Value == "NewMap.aspx" && !sn.User.ControlEnable(sn, 62))
                              || (mnuMenu.Items[i].Value == "History/frmhistmain_new.aspx" && !sn.User.ControlEnable(sn, 41))
                              || (mnuMenu.Items[i].Value == "Maintenance/frmMaintenanceGrid.aspx" && !sn.User.ControlEnable(sn, 46))
                              || (mnuMenu.Items[i].Value == "Messages/frmAlarms.aspx" && !sn.User.ControlEnable(sn, 63))
                              || (mnuMenu.Items[i].Value == "ant/ant.html" && !sn.User.ControlEnable(sn, 67))
                              || (mnuMenu.Items[i].Value == "Messages/frmMesssagesExtendedNew.aspx" && !sn.User.ControlEnable(sn, 24))
                              || (mnuMenu.Items[i].Value == "GeoZone_Landmarks/frmLandmark.aspx" && !sn.User.ControlEnable(sn, 42))
                              || (mnuMenu.Items[i].Value.Contains("Reports") && !sn.User.ControlEnable(sn, 44))
                              || (mnuMenu.Items[i].Value.Contains("HOS") && !sn.User.ControlEnable(sn, 47))
                              || ((mnuMenu.Items[i].Value == "Configuration/frmEmails.aspx") && !sn.User.ControlEnable(sn, 16))
                              || (mnuMenu.Items[i].Value.Contains("DashBoard") && !sn.User.ControlEnable(sn, 64))
                              || (mnuMenu.Items[i].Value.Contains("Configuration/frmpreference.aspx") && !sn.User.ControlEnable(sn, 65))
			      || (mnuMenu.Items[i].Value.Contains("routing/Main.aspx") && !sn.User.ControlEnable(sn, 68))
                               || (mnuMenu.Items[i].Value.Contains("ScheduleAdherence/frmReport.aspx") && !sn.User.ControlEnable(sn, 73))
                             || (mnuMenu.Items[i].Value == "EventViewer.aspx" && !sn.User.ControlEnable(sn, 108))
                          )
                        {
                            mnuMenu.Items.RemoveAt(i);
                            itemsCount--;
                            i--;
                            continue;
                        }

                        if (sn.SuperOrganizationId == 754 && mnuMenu.Items[i].Value == "Messages/frmMesssagesExtendedNew.aspx" )
                            mnuMenu.Items[i].Value = "EmailSystem/EmailSystem.aspx";

			if  (mnuMenu.Items[i].Value == "NewMap.aspx" && sn.User.ControlEnable(sn, 69))
			 {
				mnuMenu.Items[i].Value="ReeferMap.aspx";
				mnuMenu.Items[i].Text="Reefer Map";
			  }	

			
                        if (mnuMenu.Items[i].Value.Contains("Reports"))
                        {
                              if (sn.User.OrganizationId == 999991 || sn.User.OrganizationId == 480
                                || sn.User.OrganizationId == 999650 || sn.User.OrganizationId == 1000041 || sn.User.OrganizationId == 1000056 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 1000041
                                || sn.User.OrganizationId == 1000088 || sn.User.OrganizationId == 1000096 || sn.User.OrganizationId == 1000151 || sn.User.OrganizationId == 1000148
                                || sn.User.OrganizationId == 1000144 || sn.User.OrganizationId == 999646 || sn.User.OrganizationId == 1000164 || sn.User.OrganizationId == 1000141)
		                mnuMenu.Items[i].Value = "Reports/frmReportsNav.aspx";
                            else
		                mnuMenu.Items[i].Value = "Reports/frmReports_new.aspx";
                
                
                        }

                        if (IsHydroQuebec)
                        {
                            if (((sn.User.OrganizationId != 999763 && sn.User.OrganizationId != 999956 && mnuMenu.Items[i].Value == "NewMap.aspx") && sn.User.ControlEnable(sn, 62))
                                || (mnuMenu.Items[i].Value.Contains("HOS") && sn.User.ControlEnable(sn, 47))
                                || ((mnuMenu.Items[i].Value.Contains("DashBoard") && sn.User.ControlEnable(sn, 64))))
                            {
                                mnuMenu.Items.RemoveAt(i);
                                itemsCount--;
                                i--;

                            }

                        }


                    }



                    if (Convert.ToInt32(Session["superOrganizationId"]) == sn.SuperOrganizationId)
                    {
                        MenuItem mnu = new MenuItem();
                        mnu.Text = "Organizations Menu";
                        if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower() == "fr-ca") mnu.Text = "Menu organisationnel";
                        mnu.Value = "frmSuperOrganizationMenu.aspx";
                        mnuMenu.Items.Add(mnu);
                    }


                    string cScript = "window.setInterval('keepMeAlive()', 100000); ";

                    if (!(sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999955 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957))
                        ScriptManager.RegisterClientScriptBlock(this, this.GetType(), "KeepAlive", cScript, true);


                    //if (sn.User.OrganizationId == 480 || sn.User.OrganizationId == 18)
                    //{
                    //    MenuItem menuItem = new MenuItem("Dispatch");
                    //    menuItem.Value = "ant/ant.html";
                    //    mnuMenu.Items.AddAt(6, menuItem);
                    //}


                  



                    ////History View security check
                    //mnuMenu.Items[3].Enabled = !sn.User.ControlEnable(sn, 41) ? false : true;

                    ////Maintenance security check
                    //mnuMenu.Items[4].Enabled = !sn.User.ControlEnable(sn, 46) ? false : true;

                    ////Messages View security check
                    //mnuMenu.Items[6].Enabled = !sn.User.ControlEnable(sn, 24) ? false : true;

                    //string OrganizationsWithNewMessagesScreen=ConfigurationManager.AppSettings["OrganizationsWithNewMessagesScreen"].ToString();
                    //string[] tmp = OrganizationsWithNewMessagesScreen.Split(';');
                    //for (int i=0;i < tmp.Length;i++)
                    //{
                    //    if (clsUtility.IsNumeric(tmp[i]) && Convert.ToUInt32(tmp[i])==sn.User.OrganizationId)
                    //    {
                    //        mnuMenu.Items[5].Value = "EmailSystem/EmailSystem.aspx";
                    //        break;
                    //    }
                    //}

                    //if (sn.SuperOrganizationId == 754)
                    //    mnuMenu.Items[6].Value = "EmailSystem/EmailSystem.aspx";


                    ////Landmark and Geozones View security check
                    //mnuMenu.Items[7].Enabled = !sn.User.ControlEnable(sn, 42) ? false : true;

                    ////Reports security check
                    //mnuMenu.Items[8].Enabled = !sn.User.ControlEnable(sn, 44) ? false : true;



           //if (sn.User.OrganizationId == 999630 || sn.User.OrganizationId == 951 || sn.User.OrganizationId == 999700 || sn.User.OrganizationId == 999746 || sn.User.OrganizationId ==999994)
           //     mnuMenu.Items[8].Value = "Reports/frmReports_new.aspx";
           //  else
           //             mnuMenu.Items[8].Value = "Reports/frmReportsNav.aspx";	


                    
           //         //HOS View security check

           //         if (sn.User.HosEnabled)
           // {
           //                 mnuMenu.Items[9].Enabled = !sn.User.ControlEnable(sn, 47) ? false : true;
           //         mnuMenu.Items[12].Enabled = !sn.User.ControlEnable(sn, 47) ? false : true;
           // }
           //         else
           // {
           //                 mnuMenu.Items[9].Enabled = false;
           //         mnuMenu.Items[12].Enabled = false;
           // }

                    ////Configuration View security check
                    //mnuMenu.Items[10].Enabled = !sn.User.ControlEnable(sn, 16) ? false : true;



            //        if (IsHydroQuebec)
            //        {
            //            this.lnkLite.Enabled = false;   
            //            //mnuMenu.Items[1].Value = "NewMap.aspx";
            //            MenuItemCollection menuItems = mnuMenu.Items;
            //            menuItems.Remove(mnuMenu.Items[13]);
            //            menuItems.Remove(mnuMenu.Items[12]);
            //            menuItems.Remove(mnuMenu.Items[9]);
            //            if(sn.User.OrganizationId != 999763) menuItems.Remove(mnuMenu.Items[2]);
                        
            //        }

            //if (sn.User.UserGroupId == 27 || sn.User.UserGroupId == 28)
            //        {
            //            mnuMenu.Items[2].Enabled = false;
            //            //mnuMenu.Items[11].Enabled = false;
            //            if (sn.User.UserGroupId == 28)
            //                mnuMenu.Items[5].Enabled = false;
            // if (sn.User.UserGroupId == 27)
            //                mnuMenu.Items[7].Enabled = false;

            //        }



                }
                if (sn.User.RemberLastPage == 1 && !Page.IsPostBack && Request.Cookies[sn.User.OrganizationId.ToString() + "lastMenuVisit"] != null)
                {
                    foreach (MenuItem mi in mnuMenu.Items)
                    {
                        if (mi.Value == Request.Cookies[sn.User.OrganizationId.ToString() + "lastMenuVisit"].Value)
                        {
                            mi.Selected = true;
                        }
                        else
                        {
                            mi.Selected = false;
                        }
                    }
                    Response.Write("<SCRIPT Language='javascript'>parent.main.window.location='" + Request.Cookies[sn.User.OrganizationId.ToString() + "lastMenuVisit"].Value + "' </SCRIPT>");
                }
            }
            catch(Exception ex)
            {
                RedirectToLogin();
            }
        }

        protected void mnuMenu_MenuItemClick(object sender, MenuEventArgs e)
        {
            //if  ( (e.Item.Value == "Map/frmMain.aspx"
            //           || e.Item.Value == "History/frmhistmain.aspx"
            //           || e.Item.Value == "Configuration/frmEmails.aspx" 
            //           || e.Item.Value == "GeoZone_Landmarks/frmLandmark.aspx")
            //           || e.Item.Value == "Logout")
            //{

            //    if (sn.User.MapType == VLF.MAP.MapType.MapsoluteWeb)
            //      sn.MapSolute.LoadDefaultMap(sn);
            //}

            //clsUtility.LogUserAction(sn.UserID, e.Item.Value); 
            Session["LastMenu"] = "EventViewer.aspx";
            Session["CurrentMenu"] = e.Item.Value;
            if ((string)Session["LastMenu"] != (string)Session["CurrentMenu"])
            {
                Session["EVflag"] = 0;
            }
            if (e.Item.Value == "frmSuperOrganizationMenu.aspx")
            {
                Response.Write("<SCRIPT Language='javascript'>top.document.all('TopFrame').cols='0,*';window.open('frmSuperOrganizationMenu.aspx','_top')</SCRIPT>");
            }
            else if (e.Item.Value == "Logout")
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Logout Session:" + Session.SessionID.ToString()));
                FormsAuthentication.SignOut();
                Session["SentinelFMSession"] = null;
                Session.Abandon();
                Response.Write("<SCRIPT Language='javascript'>window.open('Login.aspx','_top') </SCRIPT>");
            }
            else
            {
                if (sn.User.RemberLastPage == 1)
                {
                    HttpCookie aCookie = new HttpCookie(sn.User.OrganizationId.ToString() + "lastMenuVisit");
                    aCookie.Value = e.Item.Value;
                    aCookie.Expires = DateTime.Now.AddYears(1);
                    Response.Cookies.Add(aCookie);
                }

                Response.Write("<SCRIPT Language='javascript'>parent.main.window.location='" + e.Item.Value + "' </SCRIPT>");
            }
        }

        protected void lnkUser_Click(object sender, EventArgs e)
        {
            foreach (MenuItem mi in mnuMenu.Items)
                mi.Selected = false;

            Response.Write("<SCRIPT Language='javascript'>parent.main.window.location='Configuration/frmPreference.aspx' </SCRIPT>");
        }

        private void RedirectToLogin()
        {
            int UserId = 0;
            string frmName = "";

            //Session Check
            try
            {
                SentinelFMSession snMain = (SentinelFMSession)Session["SentinelFMSession"];
                if (snMain != null && snMain.UserID != null)
                    UserId = snMain.UserID;
                else
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin -  Session null , Form:" + frmName));
            }
            catch
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin -  Session null , Form:" + frmName));
            }

            //Form Name
            try
            {
                frmName = Page.GetType().Name;
            }
            catch {}

            //Get Last Error if exists
            try
            {
                if (Server.GetLastError() != null)
                {
                    Exception ex = Server.GetLastError().GetBaseException();

                    string Excp = "SentinelFM " +
                        "MESSAGE: " + ex.Message +
                        "\nSOURCE: " + ex.Source +
                        "\nFORM: " + frmName +
                        "\nQUERYSTRING: " + Request.QueryString.ToString() +
                        "\nTARGETSITE: " + ex.TargetSite +
                        "\nSTACKTRACE: " + ex.StackTrace;

                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin - UserId:" + UserId.ToString() + " , Form:" + frmName + " , Error :" + Excp));
                }
                else
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin -  UserId:" + UserId.ToString() + " , Form:" + frmName));
            }
            catch
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "RedirectToLogin - not exception info,  Form:" + frmName));
            }

            Session.Abandon();
            Response.Write("<SCRIPT Language='javascript'>window.open('Login.aspx','_top') </SCRIPT>");
            return;
        }

        protected void lnkHelp_Click(object sender, EventArgs e)
        {
            Response.Write("<SCRIPT Language='javascript'>parent.main.window.location='Help/frmHelp.aspx' </SCRIPT>");
        }

        protected void lnkLogout_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "Logout Session:" + Session.SessionID));

                //The Logout process is initiated in the User Services Layer (USL). A user selects the Logout option from the right side of the user interface, which calls the SignOut method of FormsAuthentication.
                FormsAuthentication.SignOut(); // The SignOut method invalidates the authentication cookie.
                Session["SentinelFMSession"] = null;
                Session.Abandon();

                //Response.Redirect("Login.aspx");
                //FormsAuthentication.RedirectToLoginPage();

                //Session.Abandon();
                //Session.Clear();
                //Response.Redirect("Login.aspx");//("~/Default.aspx");


                string destination = "Login.aspx";
                if (sn.User.OrganizationId == 1000065) // Only Burnbrae has the mobile link
                {
                    destination = "Loginameco.aspx";
                }
               
                //if (sn.InterfacePrefrence==(Int16)VLF.CLS.Def.Enums.InterfacePrefrence.Lite ||  sn.InterfacePrefrence==(Int16)VLF.CLS.Def.Enums.InterfacePrefrence.Both)
                    //destination = (string)ConfigurationManager.AppSettings["SentinelLite_URL"];
            
                Response.Write("<SCRIPT Language='javascript'>window.open('" + destination + "','_top') </SCRIPT>");
                
            }
            catch {}
        }

        protected override void InitializeCulture()
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(sn.SelectedLanguage);
            base.InitializeCulture();
        }

        protected void lnkLite_Click(object sender, EventArgs e)
        {
            string qs = sn.GetSerializedBase();
            //string key = clsUtility.SetSessionBaseToMemcahed(qs);
            string key = clsUtility.SetSessionBaseToTxtFile(qs);
            string destination = string.Format((string)ConfigurationManager.AppSettings["SentinelLite_URL"], key);
            Session["SentinelFMSession"] = null;
            Session.Abandon();
            Response.Write("<SCRIPT Language='javascript'>window.open('"+destination+"','_top') </SCRIPT>");
        }
    }
}
