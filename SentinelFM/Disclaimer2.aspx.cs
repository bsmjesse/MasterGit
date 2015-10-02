using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.IO;
using VLF.CLS.Def;
using System.Web.Script.Serialization;
using VLF.DAS.Logic;

namespace SentinelFM
{
    public partial class Disclaimer2 : SentinelFMBasePage
    {

        protected void Page_Load(object sender, EventArgs e)
        {
            sn = (SentinelFMSession)Session["SentinelFMSession"];
            if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
            {
                RedirectToLogin();
                return;
            }
            Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "popup('popUpDiv');", true);
        }

        protected override void InitializeCulture()
        {
            string culture = "en-US";
            if (Session["flag"] != null)
            {
                if (Convert.ToString(Session["flag"]) == "0")
                    culture = "en-US";
                else
                {
                    culture = "fr-CA";
                }
            }


            string UserCulture = culture;
            if (UserCulture != "")
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(UserCulture);
            }


        }

        protected void btnDisagree_Click(object sender, EventArgs e)
        {
            txtEmail.Text = "";
            lblRequireEmail.Text = "";
            lblRequireEmail.Visible= false;
            //Response.Redirect("Responsive/ResponsiveLogin.aspx");
            Response.Redirect("Login.aspx");
        }

        protected void btnSaveEmail_Click(object sender, EventArgs e)
        {
            string email = txtEmail.Text;
            var x = Convert.ToString(Session["SNuserid"]);
            string secId = Convert.ToString(Session["SNsecId"]);
            string destination = Convert.ToString(Session["Destination"]);
            //get all emailIds
            string xml = "";
            DataSet dsEmail = new DataSet();
            ServerDBUser.DBUser dbUser = new ServerDBUser.DBUser();

            try
            {
                int errCode = dbUser.GetAllEmailXML(Convert.ToInt32(x), secId, ref xml);
                if (errCode == (int)VLF.ERRSecurity.InterfaceError.NoError)
                {
                    if (xml == "")
                    {
                        // this.lblMessage.Text = "No Record Found";

                        return;
                    }
                    StringReader strrXML = new StringReader(xml);
                    dsEmail.ReadXml(strrXML);

                    DataColumnCollection columns = (dsEmail.Tables[0]).Columns;
                    if (columns.Contains("Email"))
                    {
                        //check for duplicate email
                        if ((dsEmail.Tables[0]).Select(string.Format("Email LIKE '%{0}%'", txtEmail.Text)).Length > 0)
                        {
                            this.lblRequireEmail.Visible = true;
                            this.lblRequireEmail.Text = GetLocalResourceObject("lblMessage_DuplicateEmail").ToString();
                            //this.lblRequireEmail.Text = "Email ID already exists";
                            Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "popup('popUpDiv');", true);
                            return;
                        }
                    }

                    //insert Email
                    int j = dbUser.AddEmail(Convert.ToInt32(x), secId, email);
                    if (j != (int)VLF.ERRSecurity.InterfaceError.NoError)
                    {
                        this.lblRequireEmail.Visible = true;
                        this.lblRequireEmail.Text = GetLocalResourceObject("lblMessage_NotSavedEmail").ToString();
                        //this.lblRequireEmail.Text = "Error : Email Address was not saved";                    
                        Page.ClientScript.RegisterStartupScript(this.GetType(), "myScript", "popup('popUpDiv');", true);
                        return;
                    }
                    else
                    {

                        txtEmail.Text = "";
                        lblRequireEmail.Text = "";
                        lblRequireEmail.Visible = false;
                        if (!string.IsNullOrEmpty(Convert.ToString(destination)))
                            Response.Redirect(Convert.ToString(destination));

                    }

                }
            }
            catch
            {
                if (Session["Destination"] == null)
                {
                    //Response.Redirect("Response/ResponsiveLogin.aspx");
                    Response.Redirect("Login.aspx");
                }
            }

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