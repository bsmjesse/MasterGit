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

namespace SentinelFM
{
	/// <summary>
	/// Summary description for frmMessage.
	/// </summary>
	public partial class frmMessage : System.Web.UI.Page
    {
        protected SentinelFMSession sn = null; 
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			//Clear IIS cache
			Response.Cache.SetCacheability(HttpCacheability.NoCache);
			Response.Cache.SetExpires(DateTime.Now);

			    sn = (SentinelFMSession)Session["SentinelFMSession"];
                if (sn == null || sn.User == null || String.IsNullOrEmpty(sn.UserName))
                {
                    return;
                }
        
			if ((sn!=null) || (sn.UserName!="") )
			{
				this.txtMessage.Text=sn.MessageText; 
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
	}
}
