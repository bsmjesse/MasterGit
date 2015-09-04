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
using VLF.CLS.Def ;
using System.IO;
namespace SentinelFM
{
	/// <summary>
	/// Summary description for frmHistoryMsgs.
	/// </summary>
	public partial class frmHistoryMsgs : System.Web.UI.Page
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

         
			if (!Page.IsPostBack)
			{
             lstMessageType.Items.Clear();
             foreach (string item in Enum.GetNames(typeof(Enums.MessageType)))
             {
                Enums.MessageType value = (Enums.MessageType)Enum.Parse(typeof(Enums.MessageType), item);
                ListItem listItem = new ListItem(item, Convert.ToInt32(value).ToString());
                lstMessageType.Items.Add(listItem);
             }


             lstMessageType.Items.Insert(0, new ListItem("No Filter", "-1"));


				short msgShow = 10 ;
				int boxId = VLF.CLS.Def.Const.unassignedIntValue;
				if(txtBoxId.Text != "")
				{
					try
					{
						boxId = Convert.ToInt32(txtBoxId.Text);
					}
					catch
					{
						txtBoxId.Text = "";
					}
				}
			
				DateTime fromDT = VLF.CLS.Def.Const.unassignedDateTime;
				if(txtFormDT.Text != "mm/dd/yyyy 00:00:00 AM")
				{
					try
					{
						fromDT = Convert.ToDateTime(txtFormDT.Text);
					}
					catch
					{
						txtFormDT.Text = "mm/dd/yyyy 00:00:00 AM";
					}
				}
				else
				{
					fromDT=Convert.ToDateTime(DateTime.Now.AddDays(-1).ToShortDateString()+" "+DateTime.Now.ToShortTimeString ()).AddHours(5) ; 
					txtFormDT.Text=DateTime.Now.AddDays(-1).ToShortDateString()+" "+DateTime.Now.AddHours(5).ToShortTimeString() ;
				}

				DateTime toDT = VLF.CLS.Def.Const.unassignedDateTime;
				if(txtToDT.Text != "mm/dd/yyyy 00:00:00 AM")
				{
					try
					{
						toDT = Convert.ToDateTime(txtToDT.Text);
					}
					catch
					{
						txtToDT.Text = "mm/dd/yyyy 00:00:00 AM";
					}
				}
				else
				{
					toDT=Convert.ToDateTime(DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString()).AddHours(5)  ;
					txtToDT.Text=DateTime.Now.ToShortDateString()+" "+DateTime.Now.AddHours(5).ToShortTimeString();     
				}

			
				if (this.txtLastMessages.Text!="")
					try
					{
						msgShow = Convert.ToInt16 (txtLastMessages.Text); 
					}
					catch
					{
						msgShow =10;
					}

            cboOrganization_Fill(); 
            cboOrganization.SelectedIndex = -1;
            cboOrganization.Items.FindByValue(sn.User.OrganizationId.ToString()).Selected = true;
            CboFleet_Fill();
			}
		}

		private void DgHistory_Fill()
		{
			short msgShow = 10 ;
			int boxId = VLF.CLS.Def.Const.unassignedIntValue;
			if(txtBoxId.Text != "")
			{
				try
				{
					boxId = Convert.ToInt32(txtBoxId.Text);
				}
				catch
				{
					txtBoxId.Text = "";
				}
			}
			
			DateTime fromDT = VLF.CLS.Def.Const.unassignedDateTime;
			if((txtFormDT.Text != "mm/dd/yyyy 00:00:00 AM") && (txtFormDT.Text !=""))
			{
				try
				{
					fromDT = Convert.ToDateTime(txtFormDT.Text);
				}
				catch
				{
					txtFormDT.Text = "mm/dd/yyyy 00:00:00 AM";
				}
			}
			else
			{
				fromDT=Convert.ToDateTime(DateTime.Now.AddDays(-1).ToShortDateString()+" "+DateTime.Now.ToShortTimeString ()); 
				txtFormDT.Text=DateTime.Now.AddDays(-1).ToShortDateString()+" "+DateTime.Now.ToShortTimeString() ;
			}

			DateTime toDT = VLF.CLS.Def.Const.unassignedDateTime;
			if((txtToDT.Text != "mm/dd/yyyy 00:00:00 AM") && (txtToDT.Text !=""))
			{
				try
				{
					toDT = Convert.ToDateTime(txtToDT.Text);
				}
				catch
				{
					txtToDT.Text = "mm/dd/yyyy 00:00:00 AM";
				}
			}
			else
			{
				toDT=Convert.ToDateTime(DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString()) ;
				txtToDT.Text=DateTime.Now.ToShortDateString()+" "+DateTime.Now.ToShortTimeString();     
			}


			objUtil=new clsUtility(sn) ;
			DataSet dsHistory=new DataSet() ;
			StringReader strrXML = null;

			string xml = "" ;

			
			if (this.txtLastMessages.Text!="")
				try
				{
					msgShow = Convert.ToInt16 (txtLastMessages.Text); 
				}
				catch
				{
					msgShow =10;
				}

				
			ServerDBHistory.DBHistory   dbh = new ServerDBHistory.DBHistory () ;

			
			System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");
			System.Threading.Thread.CurrentThread.CurrentCulture = ci ;


            

			if (lstMessages.SelectedItem.Value=="0")
			{
				if( objUtil.ErrCheck( dbh.GetLastMessagesFromHistoryByOrganization   ( sn.UserID , sn.SecId ,msgShow,Convert.ToInt32(this.cboOrganization.SelectedItem.Value), Convert.ToInt32(cboFleet.SelectedItem.Value),	boxId,Convert.ToInt16(lstMessageType.SelectedItem.Value),fromDT,toDT, ref xml ),false ) )
               if (objUtil.ErrCheck(dbh.GetLastMessagesFromHistoryByOrganization(sn.UserID, sn.SecId, msgShow, Convert.ToInt32(this.cboOrganization.SelectedItem.Value), Convert.ToInt32(cboFleet.SelectedItem.Value), boxId, Convert.ToInt16(lstMessageType.SelectedItem.Value), fromDT, toDT, ref xml), true))
					{
						this.dgrHistoryIn.DataSource=null;
						this.dgrHistoryIn.DataBind();
						this.dgrHistoryOut.DataSource=null;
						this.dgrHistoryOut.DataBind();	
						return;
					}
			}
			else
			{
            if (objUtil.ErrCheck(dbh.GetLastMessagesOutFromHistoryByOrganization(sn.UserID, sn.SecId, msgShow,Convert.ToInt32(this.cboOrganization.SelectedItem.Value), Convert.ToInt32(cboFleet.SelectedItem.Value), boxId, Convert.ToInt16(lstMessageType.SelectedItem.Value), fromDT, toDT, ref xml), false))
					if( objUtil.ErrCheck( dbh.GetLastMessagesOutFromHistoryByOrganization ( sn.UserID , sn.SecId,msgShow,	boxId,Convert.ToInt32(this.cboOrganization.SelectedItem.Value), Convert.ToInt32(cboFleet.SelectedItem.Value),Convert.ToInt16(lstMessageType.SelectedItem.Value),fromDT,toDT, ref xml ), true ) )
					{
						this.dgrHistoryIn.DataSource=null;
						this.dgrHistoryIn.DataBind();
						this.dgrHistoryOut.DataSource=null;
						this.dgrHistoryOut.DataBind();		
						return;
					}
			}

			if (xml == "")
			{
				this.dgrHistoryIn.DataSource=null;
				this.dgrHistoryIn.DataBind();  
				this.dgrHistoryOut.DataSource=null;
				this.dgrHistoryOut.DataBind();  
				//this.tblNoData.Visible=true;  
				return;
			}

			//this.tblNoData.Visible=false;
			
			strrXML = new StringReader( xml ) ;
			dsHistory.ReadXml (strrXML) ;


         foreach (DataRow rowItem in dsHistory.Tables[0].Rows)
         {
            // Date
            if (lstMessages.SelectedItem.Value == "0")
            {
               rowItem["OriginDateTime"] = Convert.ToDateTime(rowItem["OriginDateTime"].ToString());
               rowItem["DateTimeReceived"] = Convert.ToDateTime(rowItem["DateTimeReceived"].ToString());
            }
            else
            {
               rowItem["DateTime"] = Convert.ToDateTime(rowItem["DateTime"].ToString());
            }
         }


         
         

			if (lstMessages.SelectedItem.Value=="0")
			{

            //dgrHistoryIn.DataSource = dsHistory ;
            //dgrHistoryIn.DataBind() ;

            //dgrHistoryOut.DataSource = null;
            //dgrHistoryOut.DataBind() ;


            dgHistIn.DataSource = dsHistory;
            dgHistIn.DataBind();
            dgHistIn.Visible = true;
            dgHistOut.Visible = false ;
            Session["HistIn"] = dsHistory;
			}
			else
			{
            //dgrHistoryIn.DataSource = null ;
            //dgrHistoryIn.DataBind() ;

            //dgrHistoryOut.DataSource = dsHistory ;
            //dgrHistoryOut.DataBind() ;

            dgHistOut.DataSource = dsHistory;
            dgHistOut.DataBind();
            dgHistIn.Visible = false;
            dgHistOut.Visible = true;
            Session["HistOut"] = dsHistory;
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

		protected void btnRetrieveLastMessages_Click(object sender, System.EventArgs e)
		{
			DgHistory_Fill();
		}


      protected void dgHistIn_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
      {
            if (Session["HistIn"] != null)
               e.DataSource = Session["HistIn"];
       
      }
      protected void dgHistOut_InitializeDataSource(object sender, ISNet.WebUI.WebGrid.DataSourceEventArgs e)
      {

         if (Session["HistOut"] != null)
            e.DataSource = Session["HistOut"];
     
      }
      protected void lstMessages_SelectedIndexChanged(object sender, EventArgs e)
      {
         Session["HistIn"] = null;
         Session["HistOut"] = null;
         lstMessageType.Items.Clear();
  
         if (lstMessages.SelectedItem.Value == "1")
         {

            foreach (string item in Enum.GetNames(typeof(Enums.CommandType )))
            {
               Enums.CommandType value = (Enums.CommandType)Enum.Parse(typeof(Enums.CommandType), item);
               ListItem listItem = new ListItem(item, Convert.ToInt32(value).ToString());
               lstMessageType.Items.Add(listItem);
            }


         }
         else
         {
            foreach (string item in Enum.GetNames(typeof(Enums.MessageType )))
            {
               Enums.MessageType value = (Enums.MessageType)Enum.Parse(typeof(Enums.MessageType), item);
               ListItem listItem = new ListItem(item, Convert.ToInt32(value).ToString());
               lstMessageType.Items.Add(listItem);
            }
         }

        
        lstMessageType.Items.Insert(0, new ListItem("No Filter", "-1"));
      }

      private void cboOrganization_Fill()
      {

         StringReader strrXML = null;
         DataSet ds = new DataSet();
         objUtil = new clsUtility(sn);
         string xml = "";

         ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

         if (objUtil.ErrCheck(dbo.GetAllOrganizationsInfoXML(sn.UserID, sn.SecId, ref xml), false))
            if (objUtil.ErrCheck(dbo.GetAllOrganizationsInfoXML(sn.UserID, sn.SecId, ref xml), true))
            {
               return;
            }

         if (xml == "")
         {
            return;
         }

         strrXML = new StringReader(xml);
         ds.ReadXml(strrXML);
         this.cboOrganization.DataSource = ds;
         this.cboOrganization.DataBind();



      }


      private void CboFleet_Fill()
      {

         objUtil = new clsUtility(sn);
         DataSet dsFleets = new DataSet();
         StringReader strrXML = null;




         string xml = "";
         ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization();

         if (objUtil.ErrCheck(dbo.GetFleetsInfoByOrganizationId(sn.UserID, sn.SecId, Convert.ToInt32(this.cboOrganization.SelectedItem.Value), ref xml), false))
            if (objUtil.ErrCheck(dbo.GetFleetsInfoByOrganizationId(sn.UserID, sn.SecId, Convert.ToInt32(this.cboOrganization.SelectedItem.Value), ref xml), true))
            {

               cboFleet.Items.Clear();
               cboFleet.Items.Insert(0, new ListItem("Please Select a Fleet", "-1"));
               return;
            }


         if (xml == "")
         {
            cboFleet.Items.Clear();
            cboFleet.Items.Insert(0, new ListItem("Please Select a Fleet", "-1"));

            return;
         }

         strrXML = new StringReader(xml);
         dsFleets.ReadXml(strrXML);

         cboFleet.DataSource = dsFleets;
         cboFleet.DataBind();
      }

      protected void cboOrganization_SelectedIndexChanged(object sender, System.EventArgs e)
      {
         if (Convert.ToInt32(cboOrganization.SelectedItem.Value) != 0)
            CboFleet_Fill();
      }

      protected void cboFleet_SelectedIndexChanged(object sender, EventArgs e)
      {

      }
}
}
