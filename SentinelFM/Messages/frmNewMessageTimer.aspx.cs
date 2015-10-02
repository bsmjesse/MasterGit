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

namespace SentinelFM.Messages
{
	/// <summary>
	/// Summary description for frmNewMessageTimer.
	/// </summary>
	/// 


	public partial class frmNewMessageTimer : SentinelFMBasePage
	{
		
		protected bool reloadMap ;
		


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

		protected void Page_Load(object sender, System.EventArgs e)
		{
			try
			{
				//Clear IIS cache
				Response.Cache.SetCacheability(HttpCacheability.NoCache);
				Response.Cache.SetExpires(DateTime.Now);

				sn = (SentinelFMSession) Session["SentinelFMSession"] ;
				if (sn.Message.TimerStatus==false) 
					return;

				int cmdStatus = 0;
				
		
				LocationMgr.Location dbl=new LocationMgr.Location();
				

				bool blnReload=false;
				

				foreach(DataRow rowItem in sn.Message.DtSendMessageBoxes.Rows   )
				{
					
					  if(	(Convert.ToInt16(rowItem["Updated"]) != (short)CommandStatus.Ack)&&
							(Convert.ToInt16(rowItem["Updated"]) != (short)CommandStatus.CommTimeout )&&
							(Convert.ToInt16(rowItem["Updated"]) != (short)CommandStatus.Pending))
						{
								if( objUtil.ErrCheck( dbl.GetCommandStatus(  sn.UserID ,sn.SecId  ,Convert.ToInt32(rowItem["BoxId"]),Convert.ToInt16(rowItem["ProtocolId"]),  ref cmdStatus ),false)) 
									if( objUtil.ErrCheck( dbl.GetCommandStatus(  sn.UserID ,sn.SecId  ,Convert.ToInt32(rowItem["BoxId"]),Convert.ToInt16(rowItem["ProtocolId"]), ref cmdStatus ),true)) 
									{
										cmdStatus = (int)CommandStatus.CommTimeout ;
										System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,"Session for Box: " + rowItem["BoxId"].ToString() + " and User: "+sn.UserID.ToString()+ " does not exist. Form:frmNewMessageTimer.aspx"));    
									}
					  
									sn.Cmd.Status=(CommandStatus) cmdStatus;  

									if( (cmdStatus == (int)CommandStatus.Ack) || (cmdStatus == (int)CommandStatus.CommTimeout ) || (cmdStatus == (int)CommandStatus.Pending))
									{

										//Dual Communication mode
										if ((cmdStatus == (int)CommandStatus.CommTimeout ) && (sn.Cmd.DualComm==true))  
										{
										
										
											sn.Cmd.CommModeId=Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[1]["CommModeId"]);
											sn.Cmd.ProtocolTypeId=Convert.ToInt16(sn.Cmd.DsProtocolTypes.Tables[0].Rows[1]["BoxProtocolTypeId"]);

											
											bool cmdSent = false;
											
	
											short CommModeId=sn.Cmd.CommModeId;
											short ProtocolTypeId=sn.Cmd.ProtocolTypeId;
											Int64 sessionTimeOut=0;
											if (objUtil.ErrCheck( dbl.SendCommand     (    sn.UserID , sn.SecId ,DateTime.Now  ,Convert.ToInt32(rowItem["BoxId"]) ,sn.Cmd.CommandId  ,sn.Cmd.CommandParams,  ref ProtocolTypeId,ref CommModeId,ref cmdSent,ref sessionTimeOut),false ) )
												if (objUtil.ErrCheck( dbl.SendCommand     (    sn.UserID , sn.SecId ,DateTime.Now  ,Convert.ToInt32(rowItem["BoxId"]) ,sn.Cmd.CommandId  ,sn.Cmd.CommandParams,  ref ProtocolTypeId,ref CommModeId,ref cmdSent,ref sessionTimeOut),true ) )
												{
													System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,"Sent command failed "+ Convert.ToString((VLF.CLS.Def.Enums.CommMode) CommModeId) + ". User:"+sn.UserID.ToString()+" Form:frmNewMessageTimer.aspx"));    			
												}


											

											if (cmdSent)
											{
												rowItem["ProtocolId"]=ProtocolTypeId;
												sn.Cmd.DualComm=false;
												sn.Message.TimerStatus=true;
												Response.Write("<script language='javascript'>window.setTimeout('location.reload(true)',1000)</script>") ;
												return;
											}
										}

                                        rowItem["Updated"] = cmdStatus;

										//Time Out Send Message
										if (cmdStatus == (int)CommandStatus.CommTimeout ) 
										{

                                            if (sn.Cmd.SchCommand)
                                            {
                                                sn.Message.MessageQueued = false;

                                                Int64 TaskId = 0;
                                                Int16 CommModeId = Convert.ToInt16(rowItem["ComModeId"]);
                                                Int16 ProtocolTypeId = Convert.ToInt16(rowItem["ProtocolId"]);

                                                if (objUtil.ErrCheck(dbl.SendAutomaticCommand(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]),   Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.MDTTextMessage),DateTime.Now,   sn.Cmd.CommandParams, ref ProtocolTypeId, ref   CommModeId, sn.Cmd.SchPeriod, sn.Cmd.SchInterval, false, ref TaskId), false))
                                                    if (objUtil.ErrCheck(dbl.SendAutomaticCommand(sn.UserID, sn.SecId, Convert.ToInt32(rowItem["BoxId"]), Convert.ToInt16(VLF.CLS.Def.Enums.CommandType.MDTTextMessage), DateTime.Now, sn.Cmd.CommandParams, ref ProtocolTypeId, ref   CommModeId, sn.Cmd.SchPeriod, sn.Cmd.SchInterval, false, ref TaskId), true))
                                                    {
                                                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Sent Automatic command failed " + Convert.ToString((VLF.CLS.Def.Enums.CommMode)CommModeId) + ". User:" + sn.UserID.ToString() + " Form:frmNewMessageTimer.aspx"));
                                                    }

                                                if (TaskId > 0)
                                                {
                                                    sn.Message.MessageQueued = true;
                                                    rowItem["Updated"] = (short)CommandStatus.Ack;
                                                    sn.Message.TimerStatus = true;
                                                    Response.Write("<script language='javascript'>window.setTimeout('location.reload(true)',1000)</script>");
                                                    return;
                                                }
                                            }

											DataRow dr;
											dr=sn.Message.DtSendMessageFails.NewRow();
											dr["VehicleDesc"]=rowItem["VehicleDesc"].ToString().Replace("'","''")  ;
											sn.Message.DtSendMessageFails.Rows.Add(dr);  
										}

										

									}	
									else
									{
										blnReload=true;
									}
						}
				}


				if (blnReload)
				{
					sn.Message.TimerStatus=true;
					Response.Write("<script language='javascript'>window.setTimeout('location.reload(true)',1000)</script>") ;
				}
				else
				{
				
					sn.Message.TimerStatus=false;
					Response.Write("<script language='javascript'> clearTimeout();  parent.frmNewMessage.location.href='frmNewMessage.aspx'; </script>") ; 
				}

			}
			catch(Exception Ex)
			{
				System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,Ex.Message.ToString()+" User:"+sn.UserID.ToString()+" Form:"+Page.GetType().Name));    
			}
		}
	}
}


