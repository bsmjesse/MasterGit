using System;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Web;
using System.Web.SessionState;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.IO ;
using System.Configuration; 

namespace SentinelFM
{
	/// <summary>
	/// Summary description for frmServicesUpdates.
	/// </summary>
   public partial class frmServicesUpdates : SentinelFMBasePage
	{
		
		protected System.Web.UI.WebControls.Label lblLoginName;
		protected System.Web.UI.WebControls.Label lblOrganization;
		protected System.Web.UI.WebControls.HyperLink linkPreference;

        System.Web.HttpBrowserCapabilities browser = HttpContext.Current.Request.Browser;

		protected void Page_Load(object sender, System.EventArgs e)
		{
            if (browser.Type.ToUpper().Contains("IE"))// && browser.MajorVersion != 10)
            {
                //var x = HttpContext.Current.Request.Browser.ScreenPixelsHeight;
                //var y = HttpContext.Current.Request.Browser.ScreenPixelsWidth;

                lblCustomMsg.Width = new Unit(60, UnitType.Percentage); // Shortcut: lblCustomMsg.Width = new Unit("70%");
            }

            /*Step 1 Add your Javascript code
            <script type="text/javascript" language="javascript">
                function Func() {
                    alert("hello!")
                }
            </script>

            Step 2 Add 1 Script Manager in your webForm and Add 1 button too

            Step 3 Add this code in your button click event 

            ScriptManager.RegisterStartupScript(this.Page, Page.GetType(), "text", "Func()", true);
             */

            if (!Page.IsPostBack)
			{
                // Mantis #0007045
                //this.lblLoginName.Text=sn.User.FirstName + " " +sn.User.LastName ; 
				//this.lblOrganization.Text=sn.User.OrganizationName;    

				dgSystem_Fill();
				dgFeatures_Fill();
				GetUserLastLogin();
                DataSet ds=new DataSet();


                if (sn.User.OrganizationId == 999763 || sn.User.OrganizationId == 999956 || sn.User.OrganizationId == 999957 || sn.User.OrganizationId == 999955)
                {
                    //this.lblNewFeatures.Visible = false;
                    //this.dgFeatures.Visible = false;
                    string xml = "";
                    using (ServerDBOrganization.DBOrganization dbOrganization = new ServerDBOrganization.DBOrganization())
                    {
                        if (objUtil.ErrCheck(dbOrganization.GetOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), false))
                            if (objUtil.ErrCheck(dbOrganization.GetOrganizationSettings(sn.UserID, sn.SecId, sn.User.OrganizationId, ref xml), true))
                            {
                                return;
                            }
                    }

                    StringReader strrXML = new StringReader(xml);
                    ds.ReadXml(strrXML);

                    foreach (DataRow dr in ds.Tables[0].Rows)
                    {
                        try
                        {


                            if (dr["OrgPreferenceId"].ToString() == "13")
                            {
                                this.lblCustomMsg.Text = dr["PreferenceValue"].ToString();
                                this.lblCustomMsg.Visible = true;
                            }

                        }
                        catch
                        {

                        }
                    }

                }


                try
                {

                    string strVersion="";
                    string strReleaseDate = "";

                    using (ServerDBSystem.DBSystem dbSystem = new ServerDBSystem.DBSystem())
                    {
                        if (objUtil.ErrCheck(dbSystem.GetConfigurationValue(sn.UserID, sn.SecId, 29, 1, "Version", ref strVersion), false))
                            if (objUtil.ErrCheck(dbSystem.GetConfigurationValue(sn.UserID, sn.SecId, 29, 1, "Version", ref strVersion), true))
                            {
                                return;
                            }


                        if (objUtil.ErrCheck(dbSystem.GetConfigurationValue(sn.UserID, sn.SecId, 29, 1, "ReleaseDate", ref strReleaseDate), false))
                            if (objUtil.ErrCheck(dbSystem.GetConfigurationValue(sn.UserID, sn.SecId, 29, 1, "ReleaseDate", ref strReleaseDate), true))
                            {
                                return;
                            }
                    }

                    this.lblVersion.Text = strVersion;
                    this.lblReleaseDate.Text = Convert.ToDateTime(strReleaseDate).ToString(sn.User.DateFormat); 

                }
                catch
                {
                }


                //if (sn.User.UserGroupId==1)
                //    this.lnkInstaller.Visible=true;
                //else 
                //    this.lnkInstaller.Visible=false ;
			}
		}

		private void dgFeatures_Fill()
		{
			string xml = "" ;	
			
			ServerDBSystem.DBSystem dbs=new ServerDBSystem.DBSystem();

            //if (objUtil.ErrCheck(dbs.GetSystemUpdates(sn.UserID, sn.SecId, "", "01/01/2222", Convert.ToInt16(VLF.CLS.Def.Enums.SystemUpdateType.NewFeature), ref xml), false))
            //    if (objUtil.ErrCheck(dbs.GetSystemUpdates(sn.UserID, sn.SecId, "", "01/01/2222", Convert.ToInt16(VLF.CLS.Def.Enums.SystemUpdateType.NewFeature), ref xml), false))
            if (objUtil.ErrCheck(dbs.GetSystemUpdatesByLang(sn.UserID, sn.SecId, "", "01/01/2222", Convert.ToInt16(VLF.CLS.Def.Enums.SystemUpdateType.NewFeature), CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                if (objUtil.ErrCheck(dbs.GetSystemUpdatesByLang(sn.UserID, sn.SecId, "", "01/01/2222", Convert.ToInt16(VLF.CLS.Def.Enums.SystemUpdateType.NewFeature), CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
				{
					 
					dgFeatures.DataSource = null;
					dgFeatures.DataBind(); 
					return;
				}

			if (xml == "")
			{
				return;
			}

			StringReader strrXML = new StringReader( xml ) ;
			DataSet ds=new DataSet();
			ds.ReadXml (strrXML) ;

          
			dgFeatures.DataSource = ds;
			dgFeatures.DataBind(); 

		}



		private void dgSystem_Fill()
		{
			string xml = "" ;
			
			ServerDBSystem.DBSystem dbs=new ServerDBSystem.DBSystem();

            //if (objUtil.ErrCheck(dbs.GetSystemUpdates(sn.UserID, sn.SecId, "", "01/01/2222", Convert.ToInt16(VLF.CLS.Def.Enums.SystemUpdateType.SystemStatus), ref xml), false))
            //    if (objUtil.ErrCheck(dbs.GetSystemUpdates(sn.UserID, sn.SecId, "", "01/01/2222", Convert.ToInt16(VLF.CLS.Def.Enums.SystemUpdateType.SystemStatus), ref xml), false))
            if (objUtil.ErrCheck(dbs.GetSystemUpdatesByLang(sn.UserID, sn.SecId, "", "01/01/2222", Convert.ToInt16(VLF.CLS.Def.Enums.SystemUpdateType.SystemStatus), CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                if (objUtil.ErrCheck(dbs.GetSystemUpdatesByLang(sn.UserID, sn.SecId, "", "01/01/2222", Convert.ToInt16(VLF.CLS.Def.Enums.SystemUpdateType.SystemStatus), CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
				{
					  
					dgSystem.DataSource = null;
					dgSystem.DataBind(); 
					return;
				}

			if (xml == "")
			{
				return;
			}


			StringReader strrXML = new StringReader( xml ) ;
			DataSet ds=new DataSet();
			ds.ReadXml (strrXML) ;


            string boldStart = "";
            string boldEnd = "";

            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                if (Convert.ToBoolean(dr["FontBold"]))
                {
                    boldStart = "<b>";
                    boldEnd = "</b>";
                }
                else
                {
                    boldStart = "";
                    boldEnd = "";
                }

                if (dr["FontColor"] == "")
                    dr["FontColor"] = "black";

                dr["Msg"] = boldStart + "<font color='" + dr["FontColor"] + "'>" + dr["Msg"] + "</font>" + boldEnd;
            }

			dgSystem.DataSource = ds;
			dgSystem.DataBind(); 

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

		private void dgSystem_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
		{
			if ((e.Item.ItemType == ListItemType.AlternatingItem ) || (e.Item.ItemType == ListItemType.Item))
			{
				switch((VLF.CLS.Def.Enums.AlarmSeverity ) Convert.ToInt16(e.Item.Cells[1].Text  ))
				{
					case VLF.CLS.Def.Enums.AlarmSeverity.Critical:
						e.Item.ForeColor=Color.Red;  
						break;
				
				}
			}
		}

		private void GetUserLastLogin()
		{

			try 
			{

				
				DataSet ds=new DataSet() ;
				StringReader strrXML = null;

				

			
				string xml = "" ;
				ServerDBUser.DBUser   dbu = new ServerDBUser.DBUser() ;

				if( objUtil.ErrCheck( dbu.GetUserLastLogin ( sn.UserID , sn.SecId ,sn.UserID, ref xml ),false ) )
					if( objUtil.ErrCheck( dbu.GetUserLastLogin ( sn.UserID , sn.SecId ,sn.UserID, ref xml ),true ) )
					{
						System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error," No Last Login info for User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
						return;
					}

				if (xml == "")
				{
					System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info, "No Last Login info for User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
					return;
				}


				strrXML = new StringReader( xml ) ;
				ds.ReadXml (strrXML) ;

               // if (sn.SelectedLanguage == "fr-CA")
                    this.lblLastLogin.Text = Convert.ToDateTime(ds.Tables[0].Rows[0]["LoginDateTime"]).ToString(sn.User.DateFormat+" "+sn.User.TimeFormat);
                //else
				   // this.lblLastLogin.Text=Convert.ToDateTime(ds.Tables[0].Rows[0]["LoginDateTime"]).ToString();     
			}
			
			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
			}
			

		}


		


     
	}
}
