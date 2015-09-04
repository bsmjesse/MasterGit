using System;
using System.Data;
using System.Configuration;
using System.IO; 


namespace SentinelFM
{
	/// <summary>
	/// Summary description for clsUser.
	/// </summary>
	public class clsUser
	{

		
		ServerDBUser.DBUser dbu  ;
		protected clsUtility objUtil;


	
		private string outMapURL="";
		public string OutMapURL
		{
			get{return outMapURL;}
			set{outMapURL = value;}
		}

		
		
		private int defaultFleet=-1;
		public int DefaultFleet
		{
			get{return defaultFleet;}
			set{defaultFleet = value;}
		}

		private double unitOfMes=0.6214;
		public double UnitOfMes
		{
			get{return unitOfMes;}
			set{unitOfMes = value;}
		}


		private Int16 timeZone=0;
		public Int16 TimeZone
		{
			get{return timeZone;}
			set{timeZone = value;}
		}


		private Int16 dayLightSaving=0;
		public Int16 DayLightSaving
		{
			get{return dayLightSaving;}
			set{dayLightSaving = value;}
		}

		private Int16 showReadMess=0;
		public Int16 ShowReadMess
		{
			get{return showReadMess;}
			set{showReadMess = value;}
		}

		private Int32 alarmRefreshFrequency=60000;
		public Int32 AlarmRefreshFrequency
		{
			get{return alarmRefreshFrequency;}
			set{alarmRefreshFrequency = value;}
		}

		private Int32 generalRefreshFrequency=60000;
		public Int32 GeneralRefreshFrequency
		{
			get{return generalRefreshFrequency;}
			set{generalRefreshFrequency = value;}
		}

		private Int64 positionExpiredTime=VLF.CLS.Def.Const.PositionExpiredTime;   
		public Int64 PositionExpiredTime
		{
			get{return positionExpiredTime;}
			set{positionExpiredTime = value;}
		}

		private string firstName="";
		public string FirstName
		{
			get{return firstName;}
			set{firstName = value;}
		}


		private string lastName="";
		public string LastName
		{
			get{return lastName;}
			set{lastName = value;}
		}


		private string ipAddr="";
		public string IPAddr
		{
			get{return ipAddr;}
			set{ipAddr = value;}
		}


		private string organizationName="";
		public string OrganizationName
		{
			get{return organizationName;}
			set{organizationName = value;}
		}



		private string companyLogo="";
		public string CompanyLogo
		{
			get{return companyLogo;}
			set{companyLogo = value;}
		}


		private string companyURL="";
		public string CompanyURL
		{
			get{return companyURL;}
			set{companyURL = value;}
		}

		private Int32 organizationId=0;
		public Int32 OrganizationId
		{
			get{return organizationId;}
			set{organizationId = value;}
		}

		

		private DataSet dsGUIControls;
		public DataSet DsGUIControls
		{
			get{return dsGUIControls;}
			set{dsGUIControls = value;}
		}


		private string mapPath="";
		public string MapPath
		{
			get{return mapPath;}
			set{mapPath = value;}
		}




		private string geoCodePath="";
		public string GeoCodePath
		{
			get{return geoCodePath;}
			set{geoCodePath = value;}
		}

		public void ExistingPreference(SentinelFMSession sn)
		{

			try
			{
				
				if ((sn==null) || (sn.UserName=="") )
				{
					return ;
				}


				dbu = new ServerDBUser.DBUser()  ;
				objUtil=new clsUtility(sn) ;

				string xml = "" ;
				StringReader strrXML = null;
				DataSet dsUser=new DataSet();
				ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization() ;

				if( objUtil.ErrCheck( dbu.GetUserInfoByUserId( sn.UserID , sn.SecId , ref xml ),false ) )
					if( objUtil.ErrCheck( dbu.GetUserInfoByUserId( sn.UserID , sn.SecId, ref xml ), true ) )
					{
						
					}
				if (xml != "")
				{
					strrXML = new StringReader( xml ) ;
					dsUser.ReadXml (strrXML) ;

					sn.User.LastName=dsUser.Tables[0].Rows[0]["LastName"].ToString();  
					sn.User.FirstName =dsUser.Tables[0].Rows[0]["FirstName"].ToString();  
					sn.User.OrganizationName =dsUser.Tables[0].Rows[0]["OrganizationName"].ToString();
					sn.User.OrganizationId=Convert.ToInt32(dsUser.Tables[0].Rows[0]["OrganizationId"]);
				
				}


				
				DataSet dsPref=new DataSet();
				if( objUtil.ErrCheck( dbu.GetUserPreferencesXML  ( sn.UserID , sn.SecId , ref xml ),false ) )
					if( objUtil.ErrCheck( dbu.GetUserPreferencesXML( sn.UserID , sn.SecId, ref xml ), true ) )
					{
						
					}
				if (xml != "")
				{
					strrXML = new StringReader( xml ) ;
					dsPref.ReadXml (strrXML) ;


					
				}


				
				
				
			

			

			}
		
			catch(System.Threading.ThreadAbortException)
			{
				return;
			}
			
		}




		

		public clsUser()
		{
			//
			// TODO: Add constructor logic here
			//
		}
	}
}
