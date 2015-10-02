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
using System.IO;
using System.Configuration;
using System.Globalization;
using System.Text;

namespace SentinelFM
{
	/// <summary>
	/// Summary description for frmUserGroups.
	/// </summary>
	public partial class frmUserGroups : SentinelFMBasePage
	{
        public string ExportToExcel = String.Empty;
		
		protected void Page_Load(object sender, System.EventArgs e)
		{
            ExportToExcel = (string)base.GetLocalResourceObject("ExportToExcel");

			try
			{
				if (!Page.IsPostBack)
				{
                    LocalizationLayer.GUILocalizationLayer.GUILocalizeForm(ref frmVehicleInfo, System.Globalization.CultureInfo.CurrentUICulture.TwoLetterISOLanguageName);
					GuiSecurity(this); 
					this.tblUsers.Visible=false;
					CboGroups_Fill();
                    if (!sn.User.ControlEnable(sn, 71))
                        cmdGroups.Visible = false;
                    if (!sn.User.ControlEnable(sn, 21))
                        cmdUserGroups.Visible = false;
                    if (!sn.User.ControlEnable(sn, 17))
                        cmdUserInfo.Visible = false;
                    if (!sn.User.ControlEnable(sn, 79))
                        cmdGroupConfiguration.Visible = false;
                    if (!sn.User.ControlEnable(sn, 70))
                        cmdControls.Visible = false;
                    if (!sn.User.ControlEnable(sn, 90))
                        cmdServices.Visible = false;
				}
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
			}
		}

		private void CboGroups_Fill()
		{
            bool GetUserGroupsbyUser = true;

			try
			{
				DataSet dsGroups=new DataSet() ;
				StringReader strrXML = null;
	
				string xml = "" ;
				ServerDBUser.DBUser  dbu = new ServerDBUser.DBUser() ;

                //if (objUtil.ErrCheck(dbu.GetAllUserGroupsByLang(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
                //    if (objUtil.ErrCheck(dbu.GetAllUserGroupsByLang(sn.UserID, sn.SecId, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
                //    {
                //        cboGroups.DataSource = null;
                //        return;
                //    }

                if (objUtil.ErrCheck(dbu.GetUserGroupsbyUser(sn.UserID, GetUserGroupsbyUser, sn.SecId, ref xml), false))
                    if (objUtil.ErrCheck(dbu.GetUserGroupsbyUser(sn.UserID, GetUserGroupsbyUser, sn.SecId, ref xml), true))
                    {
                        cboGroups.DataSource = null;
                        cboGroups.DataBind();
                        return;
                    }

				if (xml == "")
					return;

				strrXML = new StringReader( xml ) ;
				dsGroups.ReadXml (strrXML) ;

				//Check if User Assigned to HGIAdmin

				if( objUtil.ErrCheck( dbu.GetAssignedGroupsByUser ( sn.UserID , sn.SecId , ref xml ),false ) )
					if( objUtil.ErrCheck( dbu.GetAssignedGroupsByUser( sn.UserID , sn.SecId, ref xml ), true ) )
					{
						return;
					}

				if (xml == "")
					return;
				
				DataSet ds=new DataSet() ;
				strrXML = new StringReader( xml ) ;
				ds.ReadXml (strrXML) ;

				bool HgiAdminGroup=false;
            bool MaintenanceGroup = false;
				foreach(DataRow rowItem in ds.Tables[0].Rows)
				{
					if (rowItem["UserGroupId"].ToString().TrimEnd()=="1")
						HgiAdminGroup=true;
               else if (rowItem["UserGroupId"].ToString().TrimEnd()=="17")
                  MaintenanceGroup=true; 
				}

				if ((!HgiAdminGroup) || (!MaintenanceGroup)) 
				{
					int RowsCount=dsGroups.Tables[0].Rows.Count-1;
					for (int index=RowsCount;index>=0;--index)
					{
						DataRow rowItem=dsGroups.Tables[0].Rows[index];
						if ((rowItem["UserGroupId"].ToString().TrimEnd()=="1") && (!HgiAdminGroup))
							dsGroups.Tables[0].Rows[index].Delete();
                  else if ((rowItem["UserGroupId"].ToString().TrimEnd() == "17") && (!MaintenanceGroup))
                     dsGroups.Tables[0].Rows[index].Delete();
					}
				}

				cboGroups.DataSource=dsGroups  ;
				cboGroups.DataBind();
                cboGroups.Items.Insert(0, new ListItem((string)base.GetLocalResourceObject("SelectGroup"), "-1"));
			
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
			}
			
		}

		private void lstUnAssUsers_Fill()
		{

			try
			{
				
				StringReader strrXML = null;
				DataSet ds=new DataSet(); 
							
				string xml = "" ;
				ServerDBUser.DBUser   dbu = new ServerDBUser.DBUser() ;
			 
				if( objUtil.ErrCheck( dbu.GetAllUnassignedUsersToUserGroup  ( sn.UserID , sn.SecId ,  Convert.ToInt16(cboGroups.SelectedItem.Value), ref xml ),false ) )
					if( objUtil.ErrCheck( dbu.GetAllUnassignedUsersToUserGroup  ( sn.UserID , sn.SecId ,  Convert.ToInt16(cboGroups.SelectedItem.Value), ref xml ),true ) )
					{
						return;
					}
				if (xml == "")
				{
					this.lstUnAss.Items.Clear();  
					return;
				}

				strrXML = new StringReader( xml ) ;
				ds.ReadXml(strrXML);
				DataColumn UserFullName = new DataColumn("UserFullName",Type.GetType("System.String"));
				ds.Tables[0].Columns.Add(UserFullName);


				foreach(DataRow rw in ds.Tables[0].Rows)
					rw["UserFullName"]=rw["LastName"].ToString()+" "+rw["FirstName"].ToString() ;
				

				ViewState["dsUnAssUsers"]=ds;
				this.lstUnAss.DataSource=ds;
				this.lstUnAss.DataBind();  
			}

			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
			}
  
		}

		private void lstAssUsers_Fill()
		{
			try
			{
				
				StringReader strrXML = null;
				DataSet ds=new DataSet(); 
							
				string xml = "" ;
				ServerDBUser.DBUser   dbu = new ServerDBUser.DBUser() ;
			 
				if( objUtil.ErrCheck( dbu.GetUsersByUserGroup    ( sn.UserID , sn.SecId ,Convert.ToInt16(cboGroups.SelectedItem.Value), ref xml ),false ) )
					if( objUtil.ErrCheck( dbu.GetUsersByUserGroup    ( sn.UserID , sn.SecId ,Convert.ToInt16(cboGroups.SelectedItem.Value), ref xml ),true ) )
					{
						return;
					}
				if (xml == "")
				{
					this.lstAss.Items.Clear();  
					return;
				}

				strrXML = new StringReader( xml ) ;
				ds.ReadXml(strrXML);
				DataColumn UserFullName = new DataColumn("UserFullName",Type.GetType("System.String"));
				ds.Tables[0].Columns.Add(UserFullName);

				foreach(DataRow rw in ds.Tables[0].Rows)
					rw["UserFullName"]=rw["LastName"].ToString()+" "+rw["FirstName"].ToString() ;
				

				ViewState["dsAssUsers"]=ds;
				this.lstAss.DataSource=ds;
				this.lstAss.DataBind();  
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
			}
  
		}

        private void PrepareExcel()
        {
            DataSet dsUnassignedUsers = new DataSet();
            DataSet dsAssignedUsers = new DataSet();
            DataTable dtUnassignedUsers = new DataTable();
            DataTable dtAssignedUsers = new DataTable();
            int iUnassignedUsersCtr = 0;
            int iAssignedUsersCtr = 0;
            StringBuilder _sUsers = new StringBuilder();
            string sUsers = String.Empty;
            string sUnassignedUser = String.Empty;
            string sAssignedUser = String.Empty;

            try
            {
                if (ViewState["dsUnAssUsers"] != null)
                {
                    dsUnassignedUsers = (DataSet)ViewState["dsUnAssUsers"];
                    dtUnassignedUsers = dsUnassignedUsers.Tables[0];
                    iUnassignedUsersCtr = dsUnassignedUsers.Tables[0].Rows.Count;
                }

                if (ViewState["dsAssUsers"] != null)
                {
                    dsAssignedUsers = (DataSet)ViewState["dsAssUsers"];
                    dtAssignedUsers = dsAssignedUsers.Tables[0];
                    iAssignedUsersCtr = dsAssignedUsers.Tables[0].Rows.Count;
                }

                if (iUnassignedUsersCtr > iAssignedUsersCtr)
                {
                    for (int intCtrC = 0; intCtrC < iUnassignedUsersCtr; intCtrC++)
                    {
                        if (dtUnassignedUsers.Rows[intCtrC]["UserFullName"] != null)
                            sUnassignedUser = dtUnassignedUsers.Rows[intCtrC]["UserFullName"].ToString();
                        else
                            sUnassignedUser = string.Empty;
                        if (iAssignedUsersCtr > intCtrC)
                        {
                            if (dtAssignedUsers.Rows[intCtrC]["UserFullName"] != null)
                                sAssignedUser = dtAssignedUsers.Rows[intCtrC]["UserFullName"].ToString();
                            else
                                sAssignedUser = string.Empty;
                        }
                        else
                            sAssignedUser = string.Empty;

                        _sUsers.Append(String.Format("[\"{0}\",\"{1}\"],", sUnassignedUser, sAssignedUser));
                    }

                    if (_sUsers.Length > 0)
                    {
                        sUsers = _sUsers.ToString().Substring(0, _sUsers.Length - 1);
                        expdata.Value = "{\"Header\":[\"" + "Unassigned Users" + "\",\"" + "Assigned Users" + "\"],\"Data\":[" + sUsers + "]}";
                        imgExcel.Visible = true;
                    }
                }
                else
                {
                    for (int intCtrC = 0; intCtrC < iAssignedUsersCtr; intCtrC++)
                    {
                        if (dtAssignedUsers.Rows[intCtrC]["UserFullName"] != null)
                            sAssignedUser = dtAssignedUsers.Rows[intCtrC]["UserFullName"].ToString();
                        else
                            sAssignedUser = string.Empty;
                        if (iUnassignedUsersCtr > intCtrC)
                        {
                            if (dtUnassignedUsers.Rows[intCtrC]["UserFullName"] != null)
                                sUnassignedUser = dtUnassignedUsers.Rows[intCtrC]["UserFullName"].ToString();
                            else
                                sUnassignedUser = string.Empty;
                        }
                        else
                            sUnassignedUser = string.Empty;

                        _sUsers.Append(String.Format("[\"{0}\",\"{1}\"],", sUnassignedUser, sAssignedUser));
                    }

                    if (_sUsers.Length > 0)
                    {
                        sUsers = _sUsers.ToString().Substring(0, _sUsers.Length - 1);
                        expdata.Value = "{\"Header\":[\"" + "Unassigned Users" + "\",\"" + "Assigned Users" + "\"],\"Data\":[" + sUsers + "]}";
                        imgExcel.Visible = true;
                    }
                }
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

		protected void cboGroups_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (Convert.ToInt32(cboGroups.SelectedItem.Value)!=-1)
			{
				this.tblUsers.Visible=true;  
				lstUnAssUsers_Fill();
				lstAssUsers_Fill();
                PrepareExcel();
                btnPreview.Visible = true;
                imgExcel.Visible = true;
			}
			else
			{
				this.lstAss.Items.Clear();   
				this.lstUnAss.Items.Clear();
                btnPreview.Visible = false;
                imgExcel.Visible = false;
			}
		}

        protected void cmdFleets_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmEmails.aspx");
        }

        protected void cmdVehicles_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmVehicleInfo.aspx");
        }

        protected void cmdUserInfo_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmUsers.aspx");
        }

        protected void cmdGroups_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmgroups.aspx");
        }

		protected void cmdAdd_Click(object sender, System.EventArgs e)
		{
			try
			{
				ServerDBUser.DBUser   dbu = new ServerDBUser.DBUser() ;

				this.lblMessage.Text="";

				if (this.cboGroups.SelectedIndex==-1)
				{
					this.lblMessage.Visible=true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectGroup");
					return;
				}

			
				if (this.lstUnAss.SelectedIndex==-1)
				{
					this.lblMessage.Visible=true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectUser");
					return;
				}
			
				foreach(ListItem li in lstUnAss.Items)
				{
					if (li.Selected)
					{
						if( objUtil.ErrCheck( dbu.AssignUserToGroup  ( sn.UserID , sn.SecId ,Convert.ToInt32(li.Value ),Convert.ToInt16(cboGroups.SelectedItem.Value) ),false ) )
							if( objUtil.ErrCheck( dbu.AssignUserToGroup  ( sn.UserID , sn.SecId ,Convert.ToInt32(li.Value ),Convert.ToInt16(cboGroups.SelectedItem.Value) ),true ) )
							{
								return;
							}
					}
 
				}


				lstUnAssUsers_Fill();
				lstAssUsers_Fill();
                PrepareExcel();
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}


			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
			}
		}

		protected void cmdAddAll_Click(object sender, System.EventArgs e)
		{
			try
			{

				if (this.cboGroups.SelectedIndex==-1)
				{
					this.lblMessage.Visible=true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectGroup");
					return;
				}

				this.lblMessage.Text="";

				
				
				ServerDBUser.DBUser   dbu = new ServerDBUser.DBUser() ;


				DataSet dsUnAssUsers=new DataSet();
				dsUnAssUsers=(DataSet)ViewState["dsUnAssUsers"];

				foreach(DataRow rowItem in dsUnAssUsers.Tables[0].Rows)
				{
					if( objUtil.ErrCheck( dbu.AssignUserToGroup  ( sn.UserID , sn.SecId ,Convert.ToInt32(rowItem["UserId"]),Convert.ToInt16(cboGroups.SelectedItem.Value) ),false ) )
						if( objUtil.ErrCheck( dbu.AssignUserToGroup  ( sn.UserID , sn.SecId ,Convert.ToInt32(rowItem["UserId"]),Convert.ToInt16(cboGroups.SelectedItem.Value) ),true ) )
						{
							return;
						}
				}

				lstUnAssUsers_Fill();
				lstAssUsers_Fill();
                PrepareExcel();
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
			}
		}

		protected void cmdRemove_Click(object sender, System.EventArgs e)
		{
			try
			{

				this.lblMessage.Text="";

				if (this.cboGroups.SelectedIndex==-1)
				{
					this.lblMessage.Visible=true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectGroup");
					return;
				}


				if (this.lstAss.SelectedIndex==-1)
				{
					this.lblMessage.Visible=true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectUser");
					return;
				}

				
				ServerDBUser.DBUser   dbu = new ServerDBUser.DBUser() ;

				foreach(ListItem li in lstAss.Items)
				{
					if (li.Selected)
					{
						if( objUtil.ErrCheck( dbu.DeleteUserAssignment   ( sn.UserID , sn.SecId ,Convert.ToInt32(li.Value ),Convert.ToInt16(cboGroups.SelectedItem.Value) ),false ) )
							if( objUtil.ErrCheck( dbu.DeleteUserAssignment   ( sn.UserID , sn.SecId ,Convert.ToInt32(li.Value ),Convert.ToInt16(cboGroups.SelectedItem.Value) ),true ) )
							{
								return;
							}
					}
 
				}


				lstUnAssUsers_Fill();
				lstAssUsers_Fill();
                PrepareExcel();
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
			}
		}

		protected void cmdRemoveAll_Click(object sender, System.EventArgs e)
		{
			try
			{

				if (this.cboGroups.SelectedIndex==-1)
				{
					this.lblMessage.Visible=true;
                    this.lblMessage.Text = (string)base.GetLocalResourceObject("SelectGroup");
					return;
				}

				this.lblMessage.Text="";

				
				ServerDBUser.DBUser   dbu = new ServerDBUser.DBUser() ;

				DataSet dsAssVehicle=new DataSet();
				dsAssVehicle=(DataSet)ViewState["dsAssUsers"];

				foreach(DataRow rowItem in dsAssVehicle.Tables[0].Rows)
				{
					if(objUtil.ErrCheck( dbu.DeleteUserAssignment(sn.UserID , sn.SecId ,Convert.ToInt32(rowItem["UserId"] ),Convert.ToInt16(cboGroups.SelectedItem.Value)),false ) )
						if(objUtil.ErrCheck( dbu.DeleteUserAssignment(sn.UserID , sn.SecId ,Convert.ToInt32(rowItem["UserId"]),Convert.ToInt16(cboGroups.SelectedItem.Value)),true ) )
						{
							return;
						}
				}

				lstUnAssUsers_Fill();
				lstAssUsers_Fill();
                PrepareExcel();
			}
			catch(NullReferenceException Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.StackTrace.ToString()));
				RedirectToLogin();
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
			}
		}

        protected void cmdDriver_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmdrivers.aspx");
        }

        protected void cmdUserDashBoards_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmuserdashboards.aspx");
        }

        protected void cmdScheduledTasks_Click(object sender, EventArgs e)
        {
            Response.Redirect("frmTaskScheduler.aspx");
        }

        protected void cmdGroupConfiguration_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmOperationGroups.aspx");
        }

        protected void cmdControls_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmControls.aspx");
        }

        protected void cmdServices_Click(object sender, System.EventArgs e)
        {
            Response.Redirect("frmServices.aspx");
        }
}
}
