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
using System.Data.SqlClient;
using System.Configuration;
namespace SentinelFM.Admin
{
	/// <summary>
	/// Summary description for frmFWResults.
	/// </summary>
	public partial class frmFWResults : System.Web.UI.Page
	{
		protected clsUtility objUtil;
		protected SentinelFMSession sn = null;
	
		protected void Page_Load(object sender, System.EventArgs e)
		{
			sn = (SentinelFMSession) Session["SentinelFMSession"] ;
			if ((sn==null) || (sn.UserName=="") )
			{
				
				Response.Write("<SCRIPT Language='javascript'>window.open('frmAdminLogin.aspx','_top') </SCRIPT>");
				return;
			}

			if (!Page.IsPostBack)
			{
				string strOrgId=Request.QueryString["OrgId"];
				string strFleetId=Request.QueryString["FleetId"];
				string strVehicleId=Request.QueryString["VehicleId"];

				this.tblFW.Visible=false;
  
				cboOrganization_Fill();
//				if (strOrgId!=null)
//				{
					cboOrganization.SelectedIndex = -1;
					cboOrganization.Items.FindByValue(sn.User.OrganizationId.ToString()).Selected = true;
//				}
//				else
//				{
//					cboOrganization.Items.Insert(0, new ListItem("Please select an Organization",  "-1"));
//				}

				CboFleet_Fill();
				if ((strFleetId!=null) && (strFleetId!="-1"))
				{
					cboFleet.SelectedIndex = -1;
					cboFleet.Items.FindByValue(strFleetId.ToString()).Selected = true;

					CboVehicle_Fill(Convert.ToInt32(strFleetId)); 
				}
				else
				{
					//cboFleet.Items.Clear(); 
					cboFleet.Items.Insert(0, new ListItem("Please Select a Fleet",  "-1"));
				}


				if ((strVehicleId!=null) && (strVehicleId!="-1"))
				{
					cboVehicle.SelectedIndex = -1;
					cboVehicle.Items.FindByValue(strVehicleId.ToString()).Selected = true;
				}
				else
				{
					cboVehicle.Visible=false; 
					lblVehicles.Visible=false; 
					cboVehicle.Items.Clear(); 
					cboVehicle.Items.Insert(0, new ListItem("Please Select a Vechicle",  "-1"));
				}


				if(strOrgId!=null)
				{
					ShowResults();
				}

			}
		}

		private void cboOrganization_Fill()
		{

			StringReader strrXML = null;
			DataSet ds=new DataSet(); 
			objUtil=new clsUtility(sn) ;
			string xml = "" ;
			
			ServerDBOrganization.DBOrganization	dbo = new ServerDBOrganization.DBOrganization() ;

			if( objUtil.ErrCheck( dbo.GetAllOrganizationsInfoXML    ( sn.UserID , sn.SecId , ref xml ),false ) )
				if( objUtil.ErrCheck( dbo.GetAllOrganizationsInfoXML    ( sn.UserID , sn.SecId , ref xml ),true ) )
				{
					return;
				}

			if (xml == "")
			{
				return;
			}

			strrXML = new StringReader( xml ) ;
			ds.ReadXml (strrXML) ;
			this.cboOrganization.DataSource=ds;
			this.cboOrganization.DataBind();  

		
		
		}


		protected void cboOrganization_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (Convert.ToInt32(cboOrganization.SelectedItem.Value)!=0)
			{
				CboFleet_Fill(); 
				this.cboVehicle.Visible=false;  
				cboVehicle.Items.Clear(); 
				cboVehicle.Items.Insert(0, new ListItem("Please select a Vehicle",  "-1"));
				this.lblFleets.Visible=true;  
				this.lblVehicles.Visible=false;
				this.cboVehicle.Visible=false;  
				this.tblFW.Visible=false;
				
			}
		}

			
		private void CboFleet_Fill()
		{
		
			objUtil=new clsUtility(sn) ;
			DataSet dsFleets=new DataSet() ;
			StringReader strrXML = null;

				

			
			string xml = "" ;
			ServerDBOrganization.DBOrganization dbo   = new ServerDBOrganization.DBOrganization() ;

			if( objUtil.ErrCheck( dbo.GetFleetsInfoByOrganizationId( sn.UserID , sn.SecId ,Convert.ToInt32(this.cboOrganization.SelectedItem.Value),   ref xml ),false ) )
				if( objUtil.ErrCheck( dbo.GetFleetsInfoByOrganizationId( sn.UserID , sn.SecId ,Convert.ToInt32(this.cboOrganization.SelectedItem.Value),   ref xml ),true ) )
				{
					System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error," No Fleets for User:"+sn.UserID.ToString()+" Form:frmHistoryCrt.aspx"));    
					cboFleet.Items.Clear();  
					cboFleet.Items.Insert(0, new ListItem("Please Select a Fleet",  "-1"));
					return;
				}


			if (xml == "")
			{
				
				cboFleet.Items.Clear();  
				cboFleet.Items.Insert(0, new ListItem("Please Select a Fleet",  "-1"));
				return;
			}

			strrXML = new StringReader( xml ) ;
			dsFleets.ReadXml (strrXML) ;

			cboFleet.DataSource=dsFleets  ;
			cboFleet.DataBind();
		}
		

		private void CboVehicle_Fill(int fleetId)
		{
			DataSet dsVehicle;
			dsVehicle = new DataSet();
			objUtil=new clsUtility(sn) ;
			StringReader strrXML = null;

							
			string xml = "" ;
			ServerDBFleet.DBFleet dbf = new ServerDBFleet.DBFleet() ;

			if( objUtil.ErrCheck( dbf.GetVehiclesInfoXMLByFleetId( sn.UserID , sn.SecId ,fleetId, ref xml ),false ) )
				if( objUtil.ErrCheck( dbf.GetVehiclesInfoXMLByFleetId( sn.UserID , sn.SecId,fleetId, ref xml ), true ) )
				{
					return;
				}
			if (xml == "")
			{
				return;
			}

			strrXML = new StringReader( xml ) ;
			dsVehicle.ReadXml(strrXML);
			
			cboVehicle.DataSource =dsVehicle;
			cboVehicle.DataBind();

		}

		protected void cboFleet_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (Convert.ToInt32(cboFleet.SelectedItem.Value)!=0)
			{
				cboVehicle.Items.Clear(); 
				CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
				cboVehicle.Items.Insert(0, new ListItem("Please select a Vehicle",  "-1"));
				cboVehicle.SelectedIndex=0; 
				this.lblFleets.Visible=true;  
				this.lblVehicles.Visible=true;
				this.cboVehicle.Visible=true;  
				this.tblFW.Visible=false;  
				
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

		protected void cmdView_Click(object sender, System.EventArgs e)
		{
			ShowResults();
		}

		protected void cboVehicle_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}

		protected void cmdRefresh_Click(object sender, System.EventArgs e)
		{
         ShowResults();
		}


		private void ShowResults()
		{
			DataSet ds;
			ds = new DataSet();
			objUtil=new clsUtility(sn) ;
			StringReader strrXML = null;

							
			string xml = "" ;
			ServerDBVehicle.DBVehicle   dbv = new ServerDBVehicle.DBVehicle() ;


			if (cboVehicle.Items.Count>0 && Convert.ToInt32(cboVehicle.SelectedItem.Value)!=-1)
			{
				if( objUtil.ErrCheck( dbv.GetActiveVehicleCfgInfo( sn.UserID , sn.SecId ,Convert.ToInt64(this.cboVehicle.SelectedItem.Value)  ,  ref xml ),false ) )
					if( objUtil.ErrCheck( dbv.GetActiveVehicleCfgInfo( sn.UserID , sn.SecId ,Convert.ToInt64(this.cboVehicle.SelectedItem.Value)  ,  ref xml ),true ) )
					{
						return;
					}
			}
			else if (Convert.ToInt32(cboFleet.SelectedItem.Value)!=-1)
			{
				ServerDBFleet.DBFleet  dbf = new ServerDBFleet.DBFleet() ;

				if( objUtil.ErrCheck( dbf.GetFleetAllActiveVehiclesCfgInfo( sn.UserID , sn.SecId ,Convert.ToInt32(this.cboFleet.SelectedItem.Value)  ,  ref xml ),false ) )
					if( objUtil.ErrCheck( dbf.GetFleetAllActiveVehiclesCfgInfo( sn.UserID , sn.SecId ,Convert.ToInt32(this.cboFleet.SelectedItem.Value)  ,  ref xml ),false ) )
					{
						return;
					}
			}
			


			if (xml == "")
			{
				this.tblFW.Visible=false;  
				return;
			}

			strrXML = new StringReader( xml ) ;
			ds.ReadXml(strrXML);




			string strXML="<ROOT>";
			foreach (DataRow dr in ds.Tables[0].Rows)
			{
				strXML+="<Box>";
				strXML+="<BoxId>"+dr["BoxId"].ToString()+"</BoxId>";
				strXML+="</Box>";
			}
			strXML+="</ROOT>";


			ViewState["XML"]=strXML;

			dbv = new ServerDBVehicle.DBVehicle() ;
			xml = "";

			DateTime DDate=DateTime.Now.AddHours(-Convert.ToDouble(this.cboDays.SelectedItem.Value)-sn.User.TimeZone-sn.User.DayLightSaving);
            dbv.Timeout = 360000; 
			if( objUtil.ErrCheck( dbv.GetLastUploadFirmwareMessageFromHistory  ( sn.UserID , sn.SecId ,DDate,strXML ,  ref xml ),false ) )
            if (objUtil.ErrCheck(dbv.GetLastUploadFirmwareMessageFromHistory(sn.UserID, sn.SecId, DDate, strXML, ref xml), true))
				{
					return;
				}




			if (xml == "")
			{
				this.tblFW.Visible=false;  
				return;
			}

			strrXML = new StringReader( xml ) ;
			DataSet dsResults=new DataSet(); 
			dsResults.ReadXml(strrXML);

         
         string UploadFirmwareUpdate = "";

         foreach (DataRow dr in dsResults.Tables[0].Rows)
         {
            dr["DateTime"] = Convert.ToDateTime(dr["DateTime"].ToString());
            UploadFirmwareUpdate =  VLF.CLS.Util.PairFindValue(VLF.CLS.Def.Const.keyFirmwareUpdateStatus, dr["CustomProp"].ToString().TrimEnd());
            if (dr["MsgDirection"].ToString().TrimEnd()   == "In")
               dr["Acknowledged"] = ((VLF.CLS.Def.Enums.OTAReturnCode)Convert.ToInt16(UploadFirmwareUpdate)).ToString();
         }

             // Show Combobox
             DataColumn dc = new DataColumn("chkBox", Type.GetType("System.Boolean"));
             dc.DefaultValue = false;
             dsResults.Tables[0].Columns.Add(dc);

              sn.Admin.DsConfigFirmaware = dsResults;

			
			this.dgData.DataSource=dsResults;
			this.dgData.DataBind(); 
 
			this.tblFW.Visible=true;

           
		}

		protected void cmdFWUpdate_Click(object sender, System.EventArgs e)
		{
			Response.Redirect("frmFWUpdate.aspx?OrgId="+this.cboOrganization.SelectedItem.Value+"&FleetId="+this.cboFleet.SelectedItem.Value+"&VehicleId="+this.cboVehicle.SelectedItem.Value); 
		}
      protected void dgData_SelectedIndexChanged(object sender, EventArgs e)
      {

          Int32 BoxId=Convert.ToInt32(dgData.SelectedItem.Cells[1].Text.Substring(0,dgData.SelectedItem.Cells[1].Text.Length-1));


          string statusOTA = dgData.SelectedItem.Cells[5].Text.Substring(0, dgData.SelectedItem.Cells[5].Text.Length - 1);


         DataRow[] drColl = sn.Admin.DsConfigFirmaware.Tables[0].Select("BoxId=" + BoxId.ToString());

         Int16 protocolId = Convert.ToInt16(drColl[0]["BoxProtocolTypeId"]);

         if ((statusOTA == "FWUpdateFailed" || statusOTA == "NoCommunication" || statusOTA == "DBUpdateFailed" || statusOTA == "FirmwareProblem" || statusOTA == "SetupFailed"))
         {
         
            LocationMgr.Location dbl = new LocationMgr.Location();
            objUtil = new clsUtility(sn);

            if (objUtil.ErrCheck(dbl.DeleteSession(sn.UserID, sn.SecId, BoxId, protocolId), false))
               if (objUtil.ErrCheck(dbl.DeleteSession(sn.UserID, sn.SecId, BoxId, protocolId), true))
               {
                  dgData.SelectedItem.Cells[5].Text = "Delete Session Failed";
                  return; 
               }

            dgData.SelectedItem.Cells[5].Text = "Session Deleted";
         }
      }
        protected void cmdSubmitToCS_Click(object sender, EventArgs e)
        {
            try
            {
               
                CheckBox ch = null;
                DataRow[] drColl = null;
                Int32 BoxId = 0;
                string status = "";

                SqlCommand sqlCustomerCare=new SqlCommand(); 
                sqlCustomerCare.Connection=new SqlConnection(ConfigurationManager.ConnectionStrings["CustomerSupport"].ConnectionString);   
                sqlCustomerCare.Connection.Open();
            
                for (int i = 0; i < dgData.Items.Count; i++)
                {
                    BoxId = Convert.ToInt32(dgData.Items[i].Cells[1].Text.ToString());
                    drColl = sn.Admin.DsConfigFirmaware.Tables[0].Select("BoxId=" + BoxId.ToString());
                    ch = (CheckBox)(dgData.Items[i].Cells[0].Controls[1]);
                    status = dgData.Items[i].Cells[5].Text.ToString();

                    if (ch.Checked)
                    {
                        try
                        {
                            sqlCustomerCare.CommandText = "INSERT INTO Firmware (BoxID,Name,Status,ValidDate,LoadMethodID,LoadedOn,LoadedBy) values (" + BoxId + ",'" + drColl[0]["FwName"].ToString() + "','" + status + "',1,1,'" + System.DateTime.Now + "')";
                            sqlCustomerCare.ExecuteNonQuery();
                        }
                        catch
                        {
                        }
                    }
                }

                
                sqlCustomerCare.Connection.Close();
            }
            catch (Exception ex)
            {
                this.lblMessage.Text = ex.Message.ToString();   
            }
        }
        protected void cmdSetAllSensors_Click(object sender, EventArgs e)
        {
            foreach (DataRow rowItem in sn.Admin.DsConfigFirmaware.Tables[0].Rows)
                rowItem["chkBox"] = true;


            this.dgData.DataSource = sn.Admin.DsConfigFirmaware;
            this.dgData.DataBind();
        }
        protected void cmdUnselectAllSensors_Click(object sender, EventArgs e)
        {
            foreach (DataRow rowItem in sn.Admin.DsConfigFirmaware.Tables[0].Rows)
                rowItem["chkBox"] = false;


            this.dgData.DataSource = sn.Admin.DsConfigFirmaware;
            this.dgData.DataBind();
        }
        protected void dgData_ItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if ((e.Item.ItemType == ListItemType.AlternatingItem) || (e.Item.ItemType == ListItemType.Item))
            {
                e.Item.Cells[1].Text += "`";
                e.Item.Cells[2].Text += "`";
                e.Item.Cells[3].Text += "`";
                e.Item.Cells[4].Text += "`";
                e.Item.Cells[5].Text += "`";
            }
           
        }
}
}
