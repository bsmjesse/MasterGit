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
using System.Configuration;
using System.IO;  

namespace SentinelFM.AutomaticTask
{
	/// <summary>
	/// Summary description for frmAutomaticTasks.
	/// </summary>
	public partial class frmAutomaticTasks : SentinelFMBasePage
	{
		
		
		protected DataSet dsSchPeriod=new DataSet();
		protected DataSet dsSchInterval=new DataSet();

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				

				if (!Page.IsPostBack)
				{
					this.tblNoData.Visible=false;
					dsSchPeriod_Fill();
					dsSchInterval_Fill(); 
					DgAutomaticTasks_Fill();
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

		

		
		private void DgAutomaticTasks_Fill()
		{
			try
			{
				StringReader strrXML = null;
				DataSet ds=new DataSet(); 
				
				string xml = "" ;
				LocationMgr.Location     dbl = new LocationMgr.Location () ;

				if( objUtil.ErrCheck( dbl.GetUserTasks    ( sn.UserID, sn.SecId ,ref xml ),false ) )
					if( objUtil.ErrCheck( dbl.GetUserTasks    ( sn.UserID, sn.SecId ,ref xml ),true ) )
					{
						this.tblNoData.Visible=true;
						return;
					}

				if (xml == "")
				{
					this.tblNoData.Visible=true;
					this.dgAutomaticTasks.DataSource=null;
					this.dgAutomaticTasks.DataBind(); 
					return;
				}

				this.tblNoData.Visible=false;
				strrXML = new StringReader( xml ) ;
				ds.ReadXml (strrXML) ;

				DataColumn PeriodText = new DataColumn("PeriodText",Type.GetType("System.String"));
				ds.Tables[0].Columns.Add(PeriodText);   
				
				DataColumn IntervalText = new DataColumn("IntervalText",Type.GetType("System.String"));
				ds.Tables[0].Columns.Add(IntervalText);   

				foreach(DataRow rowItem in ds.Tables[0].Rows)
				{
					if (Convert.ToInt64(rowItem["TransmissionPeriod"])<3600)
					{
						rowItem["PeriodText"]=Convert.ToString(Convert.ToInt32(rowItem["TransmissionPeriod"])/60).ToString()+ " min" ; 
					}
					else if (Convert.ToInt64(rowItem["TransmissionPeriod"])>3600)
					{
						rowItem["PeriodText"]=Convert.ToString(Convert.ToInt32(rowItem["TransmissionPeriod"])/3600)+" hours" ; 
					}
					else if (Convert.ToInt64(rowItem["TransmissionPeriod"])==3600)
					{
						rowItem["PeriodText"]="1 hour" ; 
					}


					if (Convert.ToInt64(rowItem["TransmissionInterval"])<3600)
					{
						rowItem["IntervalText"]=Convert.ToString(Convert.ToInt32(rowItem["TransmissionInterval"])/60).ToString()+ " min" ; 
					}
					else if (Convert.ToInt64(rowItem["TransmissionInterval"])==3600)
					{
						rowItem["IntervalText"]="1 hour" ; 
					}
				}

				this.dgAutomaticTasks.DataSource=ds;
				this.dgAutomaticTasks.DataBind(); 
				
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

		private void dsSchPeriod_Fill()
		{
			if (dsSchPeriod.Tables.Count==0) 
				dsSchPeriod.Tables.Add();
 
			if (dsSchPeriod.Tables[0].Columns.Count==0) 
			{	
				DataColumn PeriodValue = new DataColumn("PeriodValue",Type.GetType("System.String"));
				DataColumn PeriodText = new DataColumn("PeriodText",Type.GetType("System.String"));
				dsSchPeriod.Tables[0].Columns.Add(PeriodValue);
				dsSchPeriod.Tables[0].Columns.Add(PeriodText);

				DataRow dr=dsSchPeriod.Tables[0].NewRow();
				dr["PeriodValue"]="600";
				dr["PeriodText"]="10 Min";
				dsSchPeriod.Tables[0].Rows.Add(dr);  

				dr=dsSchPeriod.Tables[0].NewRow();
				dr["PeriodValue"]="1800";
				dr["PeriodText"]="30 Min";
				dsSchPeriod.Tables[0].Rows.Add(dr);  

				dr=dsSchPeriod.Tables[0].NewRow();
				dr["PeriodValue"]="3600";
				dr["PeriodText"]="1 Hour";
				dsSchPeriod.Tables[0].Rows.Add(dr);  


				dr=dsSchPeriod.Tables[0].NewRow();
				dr["PeriodValue"]="3600";
				dr["PeriodText"]="1 Hour";
				dsSchPeriod.Tables[0].Rows.Add(dr);  


				dr=dsSchPeriod.Tables[0].NewRow();
				dr["PeriodValue"]="7200";
				dr["PeriodText"]="2 Hours";
				dsSchPeriod.Tables[0].Rows.Add(dr);  


				dr=dsSchPeriod.Tables[0].NewRow();
				dr["PeriodValue"]="21600";
				dr["PeriodText"]="6 Hours";
				dsSchPeriod.Tables[0].Rows.Add(dr);  


				dr=dsSchPeriod.Tables[0].NewRow();
				dr["PeriodValue"]="43200";
				dr["PeriodText"]="12 Hours";
				dsSchPeriod.Tables[0].Rows.Add(dr);  


				dr=dsSchPeriod.Tables[0].NewRow();
				dr["PeriodValue"]="86400";
				dr["PeriodText"]="24 Hours";
				dsSchPeriod.Tables[0].Rows.Add(dr);  


				
			}

		}



		private void dsSchInterval_Fill()
		{
			if (dsSchInterval.Tables.Count==0) 
				dsSchInterval.Tables.Add();
 
			if (dsSchInterval.Tables[0].Columns.Count==0) 
			{	
				DataColumn IntervalValue = new DataColumn("IntervalValue",Type.GetType("System.String"));
				DataColumn IntervalText = new DataColumn("IntervalText",Type.GetType("System.String"));
				dsSchInterval.Tables[0].Columns.Add(IntervalValue);
				dsSchInterval.Tables[0].Columns.Add(IntervalText);

				DataRow dr=dsSchInterval.Tables[0].NewRow();
				dr["IntervalValue"]="60";
				dr["IntervalText"]="1 Min";
				dsSchInterval.Tables[0].Rows.Add(dr);  

				dr=dsSchInterval.Tables[0].NewRow();
				dr["IntervalValue"]="120";
				dr["IntervalText"]="2 Min";
				dsSchInterval.Tables[0].Rows.Add(dr);  

				dr=dsSchInterval.Tables[0].NewRow();
				dr["IntervalValue"]="300";
				dr["IntervalText"]="5 Min";
				dsSchInterval.Tables[0].Rows.Add(dr);  


				dr=dsSchInterval.Tables[0].NewRow();
				dr["IntervalValue"]="1800";
				dr["IntervalText"]="30 Min";
				dsSchInterval.Tables[0].Rows.Add(dr);  


				dr=dsSchInterval.Tables[0].NewRow();
				dr["IntervalValue"]="3600";
				dr["IntervalText"]="1 Hour";
				dsSchInterval.Tables[0].Rows.Add(dr);  


			

				
			}

		}


		public int GetTransmissionPeriod(string Period) 
		{
			try
			{
				DropDownList cboPeriod=new DropDownList() ;
				cboPeriod.DataValueField = "PeriodValue";
				cboPeriod.DataTextField = "PeriodText";
				dsSchPeriod_Fill() ;
				cboPeriod.DataSource = dsSchPeriod;
				cboPeriod.DataBind();

				cboPeriod.SelectedIndex = -1;
				for (int i=0;i<cboPeriod.Items.Count;i++)
				{
					if (cboPeriod.Items[i].Value.TrimEnd()==Period.ToString())  
					{
						cboPeriod.SelectedIndex=i;
						break;
					}
				}

				return cboPeriod.SelectedIndex;
			}
			catch(NullReferenceException)
			{
				RedirectToLogin();
				return 0;
			}

		}


		public int GetTransmissionInterval(string Interval) 
		{
			try
			{
				DropDownList cboInterval=new DropDownList() ;
				cboInterval.DataValueField = "IntervalValue";
				cboInterval.DataTextField = "IntervalText";
				dsSchInterval_Fill() ;
				cboInterval.DataSource = dsSchInterval;
				cboInterval.DataBind();

				cboInterval.SelectedIndex = -1;
				for (int i=0;i<cboInterval.Items.Count;i++)
				{
					if (cboInterval.Items[i].Value.TrimEnd()==Interval.ToString())  
					{
						cboInterval.SelectedIndex=i;
						break;
					}
				}

				return cboInterval.SelectedIndex;
			}
			catch(NullReferenceException)
			{
				RedirectToLogin();
				return 0;
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
			this.dgAutomaticTasks.CancelCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgAutomaticTasks_CancelCommand);
			this.dgAutomaticTasks.EditCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgAutomaticTasks_EditCommand);
			this.dgAutomaticTasks.UpdateCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgAutomaticTasks_UpdateCommand);
			this.dgAutomaticTasks.DeleteCommand += new System.Web.UI.WebControls.DataGridCommandEventHandler(this.dgAutomaticTasks_DeleteCommand);

		}
		#endregion

		private void dgAutomaticTasks_CancelCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgAutomaticTasks.EditItemIndex = -1;
			DgAutomaticTasks_Fill();
		}

		private void dgAutomaticTasks_EditCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			dgAutomaticTasks.EditItemIndex = Convert.ToInt32(e.Item.ItemIndex);
			DgAutomaticTasks_Fill();
			dgAutomaticTasks.SelectedIndex = -1;
		}

		private void dgAutomaticTasks_UpdateCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			try
			{
				DropDownList cboPeriod;
				DropDownList cboInterval;
			
				string TaskId = dgAutomaticTasks.DataKeys[e.Item.ItemIndex].ToString() ;

				cboPeriod=(DropDownList) e.Item.FindControl("cboPeriod") ;
				cboInterval=(DropDownList) e.Item.FindControl("cboInterval") ;

				
				LocationMgr.Location    dbl = new LocationMgr.Location() ;
				
				bool RescheduleTask=false;
                //if( objUtil.ErrCheck( dbl.ReScheduledTask    ( sn.UserID , sn.SecId ,Convert.ToInt64(TaskId)  ,Convert.ToInt64(cboPeriod.SelectedItem.Value),Convert.ToInt32(cboInterval.SelectedItem.Value),false,ref RescheduleTask ) ,false ) )
                //    if( objUtil.ErrCheck( dbl.ReScheduledTask    ( sn.UserID , sn.SecId ,Convert.ToInt64(TaskId)  ,Convert.ToInt64(cboPeriod.SelectedItem.Value),Convert.ToInt32(cboInterval.SelectedItem.Value),false,ref RescheduleTask ) ,true ) )
                //    {
                //        this.lblMessage.Text="Reschedule task failed.";  
                //        return;
                //    }


				
				if (!RescheduleTask)
				{
					this.lblMessage.Text="Reschedule task failed.";  
				}

				
				lblMessage.Visible = true;
				dgAutomaticTasks.EditItemIndex = -1;
				DgAutomaticTasks_Fill();
				this.lblMessage.Text="Task updated.";  
			}
			catch(NullReferenceException)
			{
				RedirectToLogin();
			}
			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
			}
		}

		private void dgAutomaticTasks_DeleteCommand(object source, System.Web.UI.WebControls.DataGridCommandEventArgs e)
		{
			try
			{
				if (e.Item.ItemIndex != dgAutomaticTasks.SelectedIndex)
				{
					ViewState["ConfirmDelete"] = "1";
					dgAutomaticTasks.EditItemIndex = -1;
					dgAutomaticTasks.SelectedIndex = e.Item.ItemIndex;
					DgAutomaticTasks_Fill();
					lblMessage.Visible = true;
					lblMessage.Text = "To confirm, click the delete icon again.";
					return;
				}
				else
				{

					if (ViewState["ConfirmDelete"].ToString()  == "0")
					{
						lblMessage.Visible = true;
						lblMessage.Text = "To confirm, click the delete icon again";
						ViewState["ConfirmDelete"] = "1";
						return;
					}
				}
	            
			
		

			LocationMgr.Location dbl=new LocationMgr.Location();    
			

			bool TaskDeleted=false;

            //if( objUtil.ErrCheck( dbl.DeleteTask ( sn.UserID , sn.SecId   ,Convert.ToInt64(dgAutomaticTasks.DataKeys[e.Item.ItemIndex]),ref TaskDeleted ) ,false ) )
            //    if( objUtil.ErrCheck( dbl.DeleteTask ( sn.UserID , sn.SecId   ,Convert.ToInt64(dgAutomaticTasks.DataKeys[e.Item.ItemIndex]),ref TaskDeleted ) ,true ) )
            //    {
            //        return;
            //    }

				lblMessage.Visible = true;

				if (TaskDeleted)
				{
					lblMessage.Text = "Task deleted!";
				}
				else
				{
					lblMessage.Text = "Task delete failed!";
				}

				dgAutomaticTasks.SelectedIndex = -1;
				dgAutomaticTasks.CurrentPageIndex=0; 
				DgAutomaticTasks_Fill();
				ViewState["ConfirmDelete"] = "0";
				
			
		}
		catch(NullReferenceException)
			{
				RedirectToLogin();
			}
		catch(Exception Ex)
		{
			System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
		}
	 }
	}
}
