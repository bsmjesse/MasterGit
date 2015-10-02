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
using System.Text;
using System.IO;
using System.Globalization; 
namespace SentinelFM
{
public partial class Map_frmMessageRotatingClient :  System.Web.UI.Page
{
        public string strMessage;
        protected SentinelFMSession sn = null;
        protected System.Web.UI.WebControls.Label lblTotalAlarms;
        protected clsUtility objUtil;
        public string _xml = "";
        public string _checksum = "";
        public string headerColor = "#009933";
        public string MDTMessagesScrollingHight = "105";

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (sn.User.ViewAlarmScrolling  == 0)
                MDTMessagesScrollingHight = "260";

        }







    protected override void InitializeCulture()
    {

        if (Session["PreferredCulture"] != null)
        {
            string UserCulture = Session["PreferredCulture"].ToString();
            if (UserCulture != "")
            {
                System.Threading.Thread.CurrentThread.CurrentUICulture = new CultureInfo(UserCulture);
            }
        }
    }


    protected void Page_Init(object sender, EventArgs e)
    {

        sn = (SentinelFMSession)Session["SentinelFMSession"];
        if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
        {
            RedirectToLogin();
            return;
        }

        if (sn.User.MenuColor != "")
            headerColor = sn.User.MenuColor;

      

    }


    public void RedirectToLogin()
    {

        Session.Abandon();
        Response.Write("<SCRIPT Language='javascript'>top.document.all('TopFrame').cols='0,*';window.open('../Login.aspx','_top')</SCRIPT>");
        return;
    }

        #region Web Form Designer generated code
        override protected void OnInit(EventArgs e)
        {
            //
            // CODEGEN: This call is required by the ASP.NET Web Form Designer.
            //
            InitializeComponent();
            
            base.OnInit(e);
        }

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {

        }
        #endregion


    }
}

