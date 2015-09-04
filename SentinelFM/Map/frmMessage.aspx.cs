using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;

namespace SentinelFM.Map
{
    /// <summary>
    /// Summary description for frmMessage.
    /// </summary>
    public partial class frmMessage : SentinelFMBasePage
    {
        

        protected void Page_Load(object sender, System.EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                GuiSecurity(this);
            }
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

        //protected void cmdNewMessage_Click(object sender, System.EventArgs e)
        //{

        //    string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
        //    strUrl = strUrl + "	var myname='Message';";
        //    strUrl = strUrl + " var w=560;";
        //    strUrl = strUrl + " var h=560;";
        //    strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
        //    strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
        //    strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,toolbar=0,scrollbars=0,menubar=0,'; ";
        //    strUrl = strUrl + " win = window.open(mypage, myname, winprops);} ";

        //    strUrl = strUrl + " NewWindow('../Messages/frmNewMessageMain.aspx');</script>";
        //    Response.Write(strUrl);


        //}

  

    }
}
