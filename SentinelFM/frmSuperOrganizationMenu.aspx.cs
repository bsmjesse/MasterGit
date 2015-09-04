
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
using System.IO;
using System.Data.SqlClient;
namespace SentinelFM
{
    public partial class frmSuperOrganizationMenu : System.Web.UI.Page
    {
        protected SentinelFMSession sn = null;
        protected clsUtility objUtil;
        protected void Page_Load(object sender, EventArgs e)
        {

            sn = (SentinelFMSession)Session["SentinelFMSession"];
            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                RedirectToLogin();
                return;
            }
            objUtil = new clsUtility(sn);

            if (!Page.IsPostBack)
            {
                if (Session["superOrganizationId"] == null)
                    StoreSuperOrganizationData();
                else
                    RetrieveSuperOrganizationData();

                DgGeoZone_Fill();
            }
        }


        private void DgGeoZone_Fill()
        {
            try
            {
                StringReader strrXML = null;
                DataSet dsOrganizations = new DataSet();

                string xml = "";
                //ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

                //if (objUtil.ErrCheck(dbo.GetOrganizationsMonitoredBy(Convert.ToInt32(Session["userID"]),Convert.ToString( Session["secId"]),Convert.ToInt32(Session["superOrganizationId"]), ref xml), false))
                //    if (objUtil.ErrCheck(dbo.GetOrganizationsMonitoredBy(Convert.ToInt32(Session["userID"]), Convert.ToString(Session["secId"]), Convert.ToInt32(Session["superOrganizationId"]), ref xml), true ))
                //    {
                //        this.dgOrganizations.DataSource = null;
                //        this.dgOrganizations.DataBind();
                //        return;
                //    }

                //if (xml == "")
                //{
                //    this.dgOrganizations.DataSource = null;
                //    this.dgOrganizations.DataBind();
                //    return;
                //}

                //strrXML = new StringReader(xml);
                //dsOrganizations.ReadXml(strrXML);

                //this.dgOrganizations.DataSource = dsOrganizations;
                ///this.dgOrganizations.DataBind();





                string constr = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ToString();
                string sql = "";
                if (Convert.ToInt32(Session["superOrganizationId"]) == 410)
                    sql = "SELECT * FROM vlfOrganization WITH(NOLOCK) ORDER BY OrganizationName";
                else
                    sql = string.Format("SELECT * FROM vlfOrganization WITH(NOLOCK) WHERE SuperOrganizationId ={0} ORDER BY OrganizationName", Convert.ToInt32(Session["superOrganizationId"]));

                dsOrganizations = new DataSet();
                SqlDataAdapter adapter = new SqlDataAdapter();


                using (SqlConnection con = new SqlConnection(constr))
                {
                    con.Open();
                    SqlCommand com = new SqlCommand(sql, con);
                    adapter.SelectCommand = com;
                    adapter.Fill(dsOrganizations);
                    con.Close();

                }

                this.dgOrganizations.DataSource = dsOrganizations;
                this.dgOrganizations.DataBind();





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

        private void StoreSuperOrganizationData()
        {
            Session["superOrganizationId"] = sn.SuperOrganizationId;
            Session["userID"] = sn.UserID;
            Session["loginUserID"] = sn.LoginUserID;
            Session["secId"] = sn.SecId;
            Session["userName"] = sn.UserName;
            Session["hashPassword"] = sn.Password;
            Session["key"] = sn.Key;
            Session["screenHeight"] = sn.User.ScreenHeight;
            Session["screenWidth"] = sn.User.ScreenWidth;
        }

        private void RetrieveSuperOrganizationData()
        {
            sn.UserID = Convert.ToInt32(Session["userID"]);
            sn.LoginUserID = Convert.ToInt32(Session["loginUserID"]);
            sn.SecId = Session["secId"].ToString();
            sn.UserName = Session["userName"].ToString();
            sn.Password = Session["hashPassword"].ToString();
            sn.Key = Session["key"].ToString();
            sn.User.ScreenHeight = Convert.ToInt32(Session["screenHeight"]);
            sn.User.ScreenWidth = Convert.ToInt32(Session["screenWidth"]);
        }

        protected void dgOrganizations_SelectedIndexChanged(object sender, EventArgs e)
        {
            int LoginUserID = 0;

            try
            {
                StringReader strrXML = null;
                DataSet dsOrganizations = new DataSet();

                string xml = "";
                string userName = "";
                string hashPassword = "";
                int uid = -1;
                string secId = "";
                int superOrganizationId = 1;
                string Email = string.Empty;
                bool isDisclaimer = false;


                Int32 organizationId = Convert.ToInt32(dgOrganizations.DataKeys[dgOrganizations.SelectedIndex]);
                ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

                if (objUtil.ErrCheck(dbo.GetLoginCredentialsWithinSameGroup(Convert.ToInt32(Session["userID"]), Convert.ToString(Session["secId"]), organizationId, ref userName, ref hashPassword), false))
                    if (objUtil.ErrCheck(dbo.GetLoginCredentialsWithinSameGroup(Convert.ToInt32(Session["userID"]), Convert.ToString(Session["secId"]), organizationId, ref userName, ref hashPassword), true))
                    {
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "No user with same security group. User Id :" + Session["userID"] + ", Organization Id : " + organizationId + ", Form:" + Page.GetType().Name));
                        return;
                    }

                SecurityManager.SecurityManager sec = new SecurityManager.SecurityManager();
                int errCode = sec.LoginMD5Extended("", userName, hashPassword, "", ref uid, ref secId, ref superOrganizationId, ref Email, ref isDisclaimer);
                LoginUserID = Convert.ToInt32(Session["loginUserID"]);
                SentinelFMSession sn = new SentinelFMSession();
                Session.Remove("SentinelFMSession");

                sn.UserID = uid;
                sn.LoginUserID = LoginUserID;
                sn.SecId = secId;
                sn.SuperOrganizationId = superOrganizationId;
                sn.UserName = userName;
                sn.Password = hashPassword;
                sn.Key = "";

                Session.Add("SentinelFMSession", sn);
                sn.User.ScreenHeight = Convert.ToInt32(Session["screenHeight"]);
                sn.User.ScreenWidth = Convert.ToInt32(Session["screenWidth"]);
                sn.User.ExistingPreference(sn);
                sn.User.GetGuiControlsInfo(sn);

                if (Session["PreferredCulture"] != null)
                    sn.SelectedLanguage = Session["PreferredCulture"].ToString();

                if (isDisclaimer == true)
                {
                    if (Email == string.Empty || Email == null || Email == "")
                    {
                        if (Session["flag"] != null)
                        {
                            if (Session["flag"].ToString() == "1")
                            { Session["PreferredCulture"] = "fr-CA"; }

                        }
                        //Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "popup('popUpDiv');", true);
                        Session["SNuserid"] = sn.UserID;
                        Session["SNsecId"] = sn.SecId;
                        Session["Destination"] = "frmMain_Top.aspx";

                        Response.Redirect("Disclaimer2.aspx");

                    }
                    else { Response.Redirect("frmMain_Top.aspx"); }
                }
                else { Response.Redirect("frmMain_Top.aspx"); }

                



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
        protected void cmdLogout_Click(object sender, EventArgs e)
        {

            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "SuperOrganization Menu - Logout Session:" + Session.SessionID.ToString()));
            FormsAuthentication.SignOut();
            Session["SentinelFMSession"] = null;
            Session.Abandon();
            //Response.Write("<SCRIPT Language='javascript'>window.open('Responsive/ResponsiveLogin.aspx','_top') </SCRIPT>");
            Response.Write("<SCRIPT Language='javascript'>window.open('Login.aspx','_top') </SCRIPT>");
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
            catch
            {
            }

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
            //Response.Write("<SCRIPT Language='javascript'>window.open('Responsive/ResponsiveLogin.aspx','_top') </SCRIPT>");
            Response.Write("<SCRIPT Language='javascript'>window.open('Login.aspx','_top') </SCRIPT>");
            return;


        }
    }
}
