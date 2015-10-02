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

namespace SentinelFM.Admin
{
	/// <summary>
	/// Summary description for frmReports.
	/// </summary>
	public partial class frmReports : System.Web.UI.Page
	{
		protected SentinelFMSession sn = null;
		protected clsUtility objUtil;

		protected void Page_Load(object sender, System.EventArgs e)
		{


			sn = (SentinelFMSession) Session["SentinelFMSession"] ;
			if ((sn==null) || (sn.UserName=="") )
			{
				Response.Write("<SCRIPT Language='javascript'>window.open('frmAdminLogin.aspx','_top') </SCRIPT>");
				return;
			}

			int i;

			if (!Page.IsPostBack)
			{
				ListItem ls;
				ListItem lsTo;

				//12 AM
				ls =new ListItem() ;
				lsTo =new ListItem() ;
				ls.Text="12"+" AM"  ;
				ls.Value="0" ;
				lsTo.Text="12"+" AM"  ;
				lsTo.Value="0" ;
				this.cboHoursFrom.Items.Add(ls);  
				this.cboHoursTo.Items.Add(lsTo);  
				//---------

				for (i=1;i<12;i++)
				{
					ls =new ListItem() ;
					lsTo =new ListItem() ;
					if (i<10)
					{
						ls.Text="0"+i.ToString()+" AM"  ;
						ls.Value="0"+i.ToString() ;

						lsTo.Text="0"+i.ToString()+" AM"  ;
						lsTo.Value="0"+i.ToString() ;
					}
					else
					{
						ls.Text=i.ToString()+" AM"  ;
						ls.Value=i.ToString() ;

						lsTo.Text=i.ToString()+" AM"  ;
						lsTo.Value=i.ToString() ;
					}
					this.cboHoursFrom.Items.Add(ls);   
					this.cboHoursTo.Items.Add(lsTo);   
				}


				//12 PM
				ls =new ListItem() ;
				lsTo =new ListItem() ;
				ls.Text="12"+" PM"  ;
				ls.Value="12" ;
				lsTo.Text="12"+" PM"  ;
				lsTo.Value="12" ;
				this.cboHoursFrom.Items.Add(ls);  
				this.cboHoursTo.Items.Add(lsTo);  
				//---------

				int nextValue=0;

				for (i=1;i<12;i++)
				{
					ls =new ListItem() ;
					lsTo =new ListItem() ;

					ls.Text=i.ToString()+" PM"  ;
					lsTo.Text=i.ToString()+" PM"  ;
					nextValue=i+12;
					ls.Value=nextValue.ToString()  ;
					lsTo.Value=nextValue.ToString()  ;
					this.cboHoursFrom.Items.Add(ls);   
					this.cboHoursTo.Items.Add(lsTo);   
				}

				this.txtFrom.Text=DateTime.Now.AddDays(-1).ToShortDateString() ;
				this.cldFrom.SelectedDate=DateTime.Now.AddDays(-1)  ;
					
				this.txtTo.Text=DateTime.Now.ToShortDateString();
				this.cldTo.SelectedDate=DateTime.Now;

				


				this.cboHoursFrom.SelectedIndex = -1;
				for (i=0;i<=cboHoursFrom.Items.Count-1;i++)
				{
					if (Convert.ToInt32(cboHoursFrom.Items[i].Value) ==Convert.ToInt32(DateTime.Now.AddHours(-1).Hour.ToString()))
					{
						cboHoursFrom.Items[i].Selected = true;
						break;
					}
				}

				this.cboHoursTo.SelectedIndex = -1;
				for (i=0;i<=cboHoursTo.Items.Count-1;i++)
				{
					if (cboHoursTo.Items[i].Value == DateTime.Now.AddHours(1).Hour.ToString())
					{
						cboHoursTo.Items[i].Selected = true;
						break;
					}
				}

				this.tblMonth.Visible=false;  
				this.tblBoxes.Visible=false;
               
				cboOrganization_Fill();
                CboFleet_Fill();
                CboVehicle_Fill(Convert.ToInt32(this.cboFleet.SelectedItem.Value));     
                this.tblFleets.Visible = true;
  
				cboYear.SelectedIndex =cboYear.Items.IndexOf (cboYear.Items.FindByValue(DateTime.Now.Year.ToString()));

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
            cboOrganization.SelectedIndex = cboOrganization.Items.IndexOf(cboOrganization.Items.FindByValue(sn.User.OrganizationId.ToString()));
		}

		protected void cmdPreview_Click(object sender, System.EventArgs e)
		{
			string strFromDate="";
			string strToDate="";

			if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value)<12)
				strFromDate=this.txtFrom.Text+" " + this.cboHoursFrom.SelectedItem.Value+ ":00 AM"  ;

			if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value)==12)
				strFromDate=this.txtFrom.Text+" " + this.cboHoursFrom.SelectedItem.Value+ ":00 PM"  ;
	
			if (Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value)>12)
				strFromDate=this.txtFrom.Text+" " + Convert.ToString(Convert.ToInt32(this.cboHoursFrom.SelectedItem.Value)-12)+ ":00 PM"  ;

			if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value)<12)
				strToDate=this.txtTo.Text+" " + this.cboHoursTo.SelectedItem.Value+ ":00 AM";

			if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value)==12)
				strToDate=this.txtTo.Text+" " + this.cboHoursTo.SelectedItem.Value+ ":00 PM"  ;

			if (Convert.ToInt32(this.cboHoursTo.SelectedItem.Value)>12)
				strToDate=this.txtTo.Text+" " + Convert.ToString(Convert.ToInt32(this.cboHoursTo.SelectedItem.Value)-12)+ ":00 PM"  ;


			if (Convert.ToDateTime(strFromDate)>Convert.ToDateTime(strToDate))
			{
				this.lblMessage.Visible=true;
				this.lblMessage.Text="The From Date should be earlier than the To Date!"  ;
				return;
			}
			else
			{
				this.lblMessage.Visible=false;
				this.lblMessage.Text=""  ;
			}


			
		

			
			if ((this.cboReports.SelectedItem.Value=="2") && (this.optSystemUsage.SelectedItem.Value=="2") &&  (this.txtBoxId.Text==""))  
			{
				GetBoxByPhone(this.txtPhone.Text);
			}


            //if ((this.cboReports.SelectedItem.Value=="2") && (this.txtBoxId.Text==""))  
            //{
            //    this.lblMessage.Visible=true;
            //    this.lblMessage.Text="Please enter Box number!"  ;
            //    return;
            //}
            //else
            //{
            //    this.lblMessage.Visible=false;
            //    this.lblMessage.Text=""  ;
            //}


			
			string strUrl="<script language='javascript'> function NewWindow(mypage) { ";
			strUrl=strUrl+"	var myname='Report';"; 
			strUrl=strUrl+" var w=800;";
			strUrl=strUrl+" var h=480;" ;
			strUrl=strUrl+" var winl = (screen.width - w) / 2; ";
			strUrl=strUrl+" var wint = (screen.height - h) / 2; ";
			strUrl=strUrl+" winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,scrollbars=1,menubar=1,'; ";
			strUrl=strUrl+" win = window.open(mypage, myname, winprops); }" ;
			
			if (this.cboReports.SelectedItem.Value=="1"   )
			{
				string WindowUrl="";

				if (this.optRange.SelectedItem.Value=="1")
					WindowUrl="frmReport_SystemUsage.aspx?OrgId="+this.cboOrganization.SelectedItem.Value+"&FromDT="+strFromDate+"&ToDT=" + strToDate +"&OrgName="+this.cboOrganization.SelectedItem.Text.TrimEnd()   ;
				if (this.optRange.SelectedItem.Value=="2")
					WindowUrl="frmReport_SystemUsageMonthly.aspx?OrgId="+this.cboOrganization.SelectedItem.Value+"&Month="+this.cboMonth.SelectedItem.Text+"&Year="+this.cboYear.SelectedItem.Value+"&OrgName="+this.cboOrganization.SelectedItem.Text.TrimEnd()   ;

				strUrl=strUrl+" NewWindow('"+WindowUrl+"');</script>";
			}

			if (this.cboReports.SelectedItem.Value=="2")
			{
                int BoxId =0;
                DataSet ds=(DataSet)ViewState["dsVehicle"];
                DataRow[] drArr = ds.Tables[0].Select("VehicleId='" + this.cboVehicle.SelectedItem.Value+"'");
                if (drArr != null || drArr.Length > 0)
                    BoxId = Convert.ToInt32(drArr[0]["BoxId"]);
                else
                    return;

                string WindowUrl = "frmReport_SystemUsage_Box.aspx?OrgId=" + this.cboOrganization.SelectedItem.Value + "&FromDT=" + strFromDate + "&ToDT=" + strToDate + "&OrgName=" + this.cboOrganization.SelectedItem.Text.TrimEnd() + "&BoxId=" + BoxId;
				strUrl=strUrl+" NewWindow('"+WindowUrl+"');</script>";
			}

			if (this.cboReports.SelectedItem.Value=="3"   )
			{
				string WindowUrl="frmReport_Login.aspx?OrgId="+this.cboOrganization.SelectedItem.Value+"&FromDT="+strFromDate+"&ToDT=" + strToDate +"&OrgName="+this.cboOrganization.SelectedItem.Text.TrimEnd()   ;
				strUrl=strUrl+" NewWindow('"+WindowUrl+"');</script>";
			}


			if (this.cboReports.SelectedItem.Value=="4"   )
			{
				string WindowUrl="frmReport_ExceptionUsage.aspx?Month="+this.cboMonth.SelectedItem.Text+"&Year="+this.cboYear.SelectedItem.Value;
				strUrl=strUrl+" NewWindow('"+WindowUrl+"');</script>";
			}


			if (this.cboReports.SelectedItem.Value=="5"   )
			{
				string WindowUrl="frmReport_MapUsage.aspx?Month="+this.cboMonth.SelectedItem.Value+"&Year="+this.cboYear.SelectedItem.Value+"&OrgId="+this.cboOrganization.SelectedItem.Value.TrimEnd()+"&OrgName="+this.cboOrganization.SelectedItem.Text.TrimEnd()   ;
				strUrl=strUrl+" NewWindow('"+WindowUrl+"');</script>";
			}


			if (this.cboReports.SelectedItem.Value=="6"   )
			{

				string CommModes="";

				foreach(ListItem li in this.lstCommMode.Items)
				{
					if (li.Selected)
					{
						CommModes+=li.Value+";"; 
					}
				}

				if (CommModes=="")
				{
					this.lblMessage.Visible=true;
					this.lblMessage.Text="Please select a Communication Mode!"  ;
					return;
				}

				CommModes=CommModes.Substring(0,CommModes.Length-1);

				string WindowUrl="frmReport_CommDiagnostic.aspx?OrgId="+this.cboOrganization.SelectedItem.Value+"&FromDT="+strFromDate+"&ToDT=" + strToDate +"&OrgName="+this.cboOrganization.SelectedItem.Text.TrimEnd() +"&FleetId="+this.cboFleet.SelectedItem.Value+ "&VehicleId="+this.cboVehicle.SelectedItem.Value.ToString()  + "&CommModes="+CommModes;
				strUrl=strUrl+" NewWindow('"+WindowUrl+"');</script>";
			}

			Response.Write(strUrl) ;
		}

		protected void cldFrom_SelectionChanged(object sender, System.EventArgs e)
		{
			this.txtFrom.Text=cldFrom.SelectedDate.ToShortDateString() ;
		}

		protected void cldTo_SelectionChanged(object sender, System.EventArgs e)
		{
			this.txtTo.Text=cldTo.SelectedDate.ToShortDateString(); 
		}

		protected void cboReports_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.tblBoxes.Visible=false;
			this.optRange.Visible=false;  
			this.tblMonth.Visible=false;
			this.cboOrganization.Visible=true;
			this.lblOrganization.Visible=true;  
			this.tblFleets.Visible=false;  
		//	this.optRange.Items[0].Selected=true ;   
			ShowDateCriteria(true);

            //if (this.cboReports.SelectedItem.Value=="2")
            //    this.tblBoxes.Visible=true;  

			if (this.cboReports.SelectedItem.Value=="1")
			{
				this.optRange.Visible=true;  
				if (this.optRange.SelectedItem.Value=="1")
				{
					this.tblMonth.Visible=false;  
					ShowDateCriteria(true); 
				}

				if (this.optRange.SelectedItem.Value=="2")
				{
					this.tblMonth.Visible=true;
					ShowDateCriteria(false);
				}
			}

			if (this.cboReports.SelectedItem.Value=="4")
			{
				this.tblMonth.Visible=true;
				ShowDateCriteria(false);
				this.cboOrganization.Visible=false;
				this.lblOrganization.Visible=false;  
			}


			if (this.cboReports.SelectedItem.Value=="5")
			{
				this.tblMonth.Visible=true;
				ShowDateCriteria(false);
				this.cboOrganization.Visible=true;
				this.lblOrganization.Visible=true;  
			}

			if (this.cboReports.SelectedItem.Value=="6")
			{

				
				ShowDateCriteria(true);
				this.cboOrganization.Visible=true;
				this.lblOrganization.Visible=true;  
				cboOrganization.Items.Insert(0, new ListItem("All",  "-1"));
				cboOrganization.SelectedIndex=0; 
				cboFleet.Items.Insert(0, new ListItem("All",  "-1"));
				cboFleet.SelectedIndex=0; 
				cboVehicle.Items.Insert(0, new ListItem("All",  "-1")); 
				cboVehicle.SelectedIndex=0; 
				lstCommMode_Fill();
				tblFleets.Visible=true; 

			}

		}


		private void cmdFind_Click(object sender, System.EventArgs e)
		{
			 Search_Phone_Box();
		}

		private void lstBoxes_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.txtBoxId.Text=this.lstBoxes.SelectedItem.Value;     
			string[] values = this.lstBoxes.SelectedItem.Text.Split(',');
			this.txtPhone.Text=values[0]; 
		}

		protected void optSystemUsage_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			this.txtBoxId.Text="";
			this.txtPhone.Text="";  
			this.lstBoxes.DataSource=null;
			this.lstBoxes.DataBind();
			this.lstBoxes.Visible=false;  

			if (this.optSystemUsage.SelectedItem.Value=="2")
			{
				this.txtPhone.Visible=true ;
				this.lblPhone.Visible=true;  
				this.txtBoxId.Enabled=false;  
			}
			else
			{
				this.txtPhone.Visible=false ;
				this.lblPhone.Visible=false; 
				this.txtBoxId.Enabled=true;  
			}
		}

		private void Search_Phone_Box()
		{
			StringReader strrXML = null;
			DataSet ds=new DataSet(); 
			objUtil=new clsUtility(sn) ;
			string xml = "" ;
			
			ServerDBOrganization.DBOrganization	dbo = new ServerDBOrganization.DBOrganization() ;

			if( objUtil.ErrCheck( dbo.GetOrganizationCommPhonesInfo    ( sn.UserID , sn.SecId ,Convert.ToInt32(this.cboOrganization.SelectedItem.Value),     ref xml ),false ) )
				if( objUtil.ErrCheck( dbo.GetOrganizationCommPhonesInfo    ( sn.UserID , sn.SecId ,Convert.ToInt32(this.cboOrganization.SelectedItem.Value),     ref xml ),true ) )
				{
					this.txtBoxId.Text=""; 
					this.txtPhone.Text="";
					return;
				}

			if (xml == "")
			{
				this.txtBoxId.Text="";
				this.txtPhone.Text="";
				return;
			}

			strrXML = new StringReader( xml ) ;
			ds.ReadXml (strrXML) ;



			DataRow[] drCollections=null;


			DataTable dt=new DataTable(); 
			DataColumn colBoxId = new DataColumn("BoxId",Type.GetType("System.String"));
			dt.Columns.Add(colBoxId);
			DataColumn colPhone = new DataColumn("Phone",Type.GetType("System.String"));
			dt.Columns.Add(colPhone);
			DataColumn colDescription = new DataColumn("Description",Type.GetType("System.String"));
			dt.Columns.Add(colDescription);


			if (this.optSystemUsage.SelectedItem.Value=="1") 
				drCollections=ds.Tables[0].Select("BoxId like '" + this.txtBoxId.Text   +"%'","BoxId",DataViewRowState.CurrentRows);

			if (this.optSystemUsage.SelectedItem.Value=="2") 
				drCollections=ds.Tables[0].Select("CommAddressValue like '" + this.txtPhone.Text +"%'","CommAddressValue",DataViewRowState.CurrentRows);

		
			if (drCollections!=null)
			{
				foreach(DataRow rowItem in drCollections)
				{
					DataRow dr=dt.NewRow() ;
					dr["BoxId"]=rowItem["BoxId"].ToString().TrimEnd()  ;
					dr["Phone"]=rowItem["CommAddressValue"].ToString().TrimEnd() ;
					if (this.optSystemUsage.SelectedItem.Value=="1")
						dr["Description"]=rowItem["BoxId"].ToString().TrimEnd() ;
					if (this.optSystemUsage.SelectedItem.Value=="2")
						dr["Description"]=rowItem["CommAddressValue"].ToString().TrimEnd()+" ,"+rowItem["BoxId"].ToString().TrimEnd() ;

					
					dt.Rows.Add(dr);  
				}


				if (drCollections.Length==1)
				{
					this.txtBoxId.Text=drCollections[0]["BoxId"].ToString().TrimEnd();
					this.txtPhone.Text=drCollections[0]["CommAddressValue"].ToString().TrimEnd();
				}
				else
				{
					this.txtPhone.Text="";  
					this.txtBoxId.Text="";  
				}
			}

			
			this.lstBoxes.Visible=true;
			this.lstBoxes.DataSource=dt;
			this.lstBoxes.DataBind();  
		
		}

		private void GetBoxByPhone(string Phone)
		{
			StringReader strrXML = null;
			DataSet ds=new DataSet(); 
			objUtil=new clsUtility(sn) ;
			string xml = "" ;
			
			ServerDBOrganization.DBOrganization	dbo = new ServerDBOrganization.DBOrganization() ;

			if( objUtil.ErrCheck( dbo.GetOrganizationCommPhonesInfo    ( sn.UserID , sn.SecId ,Convert.ToInt32(this.cboOrganization.SelectedItem.Value),     ref xml ),false ) )
				if( objUtil.ErrCheck( dbo.GetOrganizationCommPhonesInfo    ( sn.UserID , sn.SecId ,Convert.ToInt32(this.cboOrganization.SelectedItem.Value),     ref xml ),true ) )
				{
					this.txtBoxId.Text="";  
					this.txtPhone.Text="";
					return;
				}

			if (xml == "")
			{
				this.txtBoxId.Text="";
				this.txtPhone.Text="";
				return;
			}

			strrXML = new StringReader( xml ) ;
			ds.ReadXml (strrXML) ;



			DataRow[] drCollections=null;

			if (this.optSystemUsage.SelectedItem.Value=="2") 
				drCollections=ds.Tables[0].Select("CommAddressValue like '" + this.txtPhone.Text +"%'","CommAddressValue",DataViewRowState.CurrentRows);

			if ((drCollections==null) || (drCollections.Length>1) || (drCollections.Length==0))
			{
			
				this.txtBoxId.Text="";
				this.txtPhone.Text="";
				Search_Phone_Box();
			}
			else
			{
				if (drCollections.Length==1)
				{
					this.txtBoxId.Text=drCollections[0]["BoxId"].ToString().TrimEnd();
					this.txtPhone.Text=drCollections[0]["CommAddressValue"].ToString().TrimEnd();
				}
			}
		}

		protected void optRange_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (this.optRange.SelectedItem.Value=="1")
			{
				this.tblMonth.Visible=false;  
				ShowDateCriteria(true); 
			}

			if (this.optRange.SelectedItem.Value=="2")
			{
				this.tblMonth.Visible=true;  
				ShowDateCriteria(false);
			}
		}

		private void ShowDateCriteria(bool VisibleDate)
		{
			this.lblFrom.Visible=VisibleDate;
			this.lblTo.Visible=VisibleDate;
			this.cboHoursFrom.Visible=VisibleDate;
			this.cboHoursTo.Visible=VisibleDate;
			this.txtFrom.Visible=VisibleDate;   
			this.txtTo.Visible=VisibleDate;  
			this.cldFrom.Visible=VisibleDate;
			this.cldTo.Visible=VisibleDate;  
		}

		protected void cboOrganization_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (Convert.ToInt32(cboOrganization.SelectedItem.Value)!=0)
			{
				this.tblFleets.Visible=true;
				CboFleet_Fill(); 
				this.lblComFleets.Visible=true;  
				this.lblComVehicles.Visible=false;
				this.cboVehicle.Visible=false;  
  			}
		}

		protected void cboFleet_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (Convert.ToInt32(cboFleet.SelectedItem.Value)!=0)
			{
				this.tblFleets.Visible=true;
				CboVehicle_Fill(Convert.ToInt32(cboFleet.SelectedItem.Value));
				cboVehicle.Items.Insert(0, new ListItem("All",  "-1"));
				cboVehicle.SelectedIndex=0; 
				this.lblComFleets.Visible=true;  
				this.lblComVehicles.Visible=true;
				this.cboVehicle.Visible=true;  
			}
		
		}

		private void lstCommMode_Fill()
		{
			StringReader strrXML = null;
			DataSet ds=new DataSet(); 
			objUtil=new clsUtility(sn) ;
			string xml = "" ;
			
			ServerDBSystem.DBSystem  dbs = new ServerDBSystem.DBSystem() ;

			if( objUtil.ErrCheck( dbs.GetCommModesInfo      ( sn.UserID , sn.SecId ,   ref xml ),false ) )
				if( objUtil.ErrCheck( dbs.GetCommModesInfo      ( sn.UserID , sn.SecId ,   ref xml ),true ) )
				{
					
					return;
				}

			if (xml == "")
			{
				return;
			}

			strrXML = new StringReader( xml ) ;
			ds.ReadXml (strrXML) ;
			this.lstCommMode.DataSource=ds;
			this.lstCommMode.DataBind(); 
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
                        this.cboFleet.Items.Clear();  
						return;
					}


				if (xml == "")
				{
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
                ViewState["dsVehicle"] = dsVehicle;
		}


	}
}
