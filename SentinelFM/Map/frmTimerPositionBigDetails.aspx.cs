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
using VLF.CLS.Interfaces;  

namespace SentinelFM.Map
{
	/// <summary>
	/// Summary description for frmTimerPositionBigDetails.
	/// </summary>
	public partial class frmTimerPositionBigDetails : SentinelFMBasePage
	{
		
		protected bool reloadMap ;
		

		

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				

				if (sn.Map.TimerStatus==false) 
					return;

				int cmdStatus = 0;
				if(sn==null || sn.Map.DsFleetInfo==null)
				{
					string str="";
					str="top.document.all('TopFrame').cols='0,*';";
					Response.Write("<SCRIPT Language='javascript'>" + str + "window.open('../Login.aspx','_top') </SCRIPT>");
					return;
				}

		

				LocationMgr.Location dbl=new LocationMgr.Location();
				

				bool blnReload=false;

				foreach(DataRow rowItem in sn.Map.DsFleetInfo.Tables[0].Rows)
				{
					if (rowItem["chkBoxShow"].ToString().ToLower()=="true")  
					{
						if(	(Convert.ToInt16(rowItem["Updated"]) != (short)CommandStatus.Ack)&&
							(Convert.ToInt16(rowItem["Updated"]) != (short)CommandStatus.Pending )&&
							(Convert.ToInt16(rowItem["Updated"]) != (short)CommandStatus.CommTimeout ))
						{
							if( objUtil.ErrCheck( dbl.GetCommandStatus(  sn.UserID ,sn.SecId  ,Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(rowItem["ProtocolId"]), ref cmdStatus ),false)) 
								if( objUtil.ErrCheck( dbl.GetCommandStatus(  sn.UserID ,sn.SecId  ,Convert.ToInt32(rowItem["BoxId"]),Convert.ToInt16(rowItem["ProtocolId"]),  ref cmdStatus ),true)) 
								{
									cmdStatus = (int)CommandStatus.CommTimeout ;
									System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,"Session for Box: " + rowItem["BoxId"].ToString() + " and User: "+sn.UserID.ToString()+ " does not exist. Form:frmTimerPosition.aspx"));    
								}
					  
							sn.Cmd.Status=(CommandStatus) cmdStatus;  

							if( (cmdStatus == (int)CommandStatus.Ack) || (cmdStatus == (int)CommandStatus.CommTimeout ) || (cmdStatus == (int)CommandStatus.Pending))
							{

								//Queued Messages 
								if ((cmdStatus == (int)CommandStatus.Pending ) || (cmdStatus == (int)CommandStatus.CommTimeout ))
								{
									DataRow dr;
									dr=sn.Cmd.DtUpdatePositionFails.NewRow();
									dr["VehicleDesc"]=rowItem["Description"];
									dr["Status"]=Convert.ToString(cmdStatus);
									sn.Cmd.DtUpdatePositionFails.Rows.Add(dr);  

									// Clear flag GPS not valid for time-out vechicles.
									rowItem["LastCommunicatedDateTime"]=rowItem["OriginDateTime"];
								}

								rowItem["Updated"] =(int) cmdStatus;
								
							}	
							else
							{
								blnReload=true;
							}
						}
					}
				}



			

			
				if (blnReload)
				{
					sn.Map.TimerStatus=true;
					Response.Write("<script language='javascript'>window.setTimeout('location.reload(true)',1000)</script>") ;
				}
				else
				{
				
					sn.Map.TimerStatus=false;
               //Response.Write("<script language='javascript'> clearTimeout();  parent.main.location.href='frmFullScreenGrid.aspx'; </script>"); 

                    //Devin
                    Response.Write("<script language='javascript'> clearTimeout();   var loc=parent.main.location.toString().toLowerCase() ;  if (loc.indexOf('/new/frmfullscreengrid.aspx')>0) parent.main.UpdatePositionResult(); else parent.main.location.href='frmFullScreenGrid.aspx';   </script>");


				}
			}

			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
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
