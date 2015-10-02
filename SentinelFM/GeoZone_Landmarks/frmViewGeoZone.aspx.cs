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
using VLF.MAP;
using VLF.CLS.Interfaces;
using System.IO ;

namespace SentinelFM.GeoZone_Landmarks
{
	/// <summary>
	/// Summary description for frmViewGeoZone.
	/// </summary>
	public partial class frmViewGeoZone : SentinelFMBasePage
	{
		
		public ClientMapProxy  map;
		public int imageW=655;
		public int imageH=361;
		


		protected void Page_Load(object sender, System.EventArgs e)
		{
            string GeoZoneId1 = Request.QueryString["GeoZoneId"];
            if (string.IsNullOrEmpty(GeoZoneId1)) Response.Redirect("~/MapNew/frmViewGeoZone.aspx");
            else Response.Redirect("~/MapNew/frmViewGeoZone.aspx?GeoZoneId=" + GeoZoneId1);
			

            // create ClientMapProxy only for mapping
            map = new VLF.MAP.ClientMapProxy(sn.User.MapEngine);
            if (map == null)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Can't create map " + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                return;
            }
            map.MapWidth = imageW;
            map.MapHeight = imageH;
					 
			string GeoZoneId=Request.QueryString["GeoZoneId"];
			DrawGeoZones(Convert.ToInt16(GeoZoneId) );
		}



		private void DrawGeoZones(Int16 GeoZoneId)
		{
			try
			{

				

					StringReader strrXML = null;
					DataSet dsGeoZoneDetails=new DataSet(); 
					
					string xml = "" ;
					ServerDBOrganization.DBOrganization dbo = new ServerDBOrganization.DBOrganization() ;
					
					if( objUtil.ErrCheck( dbo.GetOrganizationGeozoneInfo ( sn.UserID , sn.SecId ,sn.User.OrganizationId,GeoZoneId , ref xml ),false ) )
						if( objUtil.ErrCheck( dbo.GetOrganizationGeozoneInfo ( sn.UserID , sn.SecId ,sn.User.OrganizationId,GeoZoneId , ref xml ),true ) )
						{
							return;
						}

					if (xml == "")
						return;




				strrXML = new StringReader( xml ) ;
				dsGeoZoneDetails.ReadXml (strrXML) ;


				Int16 SeverityId=0;
				Int16 Type=0;
				Int16 GeozoneType=0;
				string GeoZoneName="";

				foreach(DataRow dr in sn.GeoZone.DsGeoZone.Tables[0].Rows)
				{		
					if (Convert.ToInt16(dr["GeoZoneId"].ToString().TrimEnd())==GeoZoneId  )
					{
						 SeverityId=Convert.ToInt16(dr["SeverityId"]);
						 Type=Convert.ToInt16(dr["Type"]);
						 GeozoneType=Convert.ToInt16(dr["GeozoneType"]);
						 GeoZoneName=dr["GeoZoneName"].ToString().TrimEnd()  ;
						 break;
					}
				}

					
					sn.GeoZone.GeozoneTypeId=(VLF.CLS.Def.Enums.GeozoneType)GeozoneType ;  
					if (dsGeoZoneDetails.Tables[0].Rows.Count>0)
					{

						if (sn.GeoZone.GeozoneTypeId==VLF.CLS.Def.Enums.GeozoneType.Rectangle) 
						{

							DataRow rowItem1=dsGeoZoneDetails.Tables[0].Rows[0];
							DataRow rowItem2=dsGeoZoneDetails.Tables[0].Rows[1];

							VLF.MAP.GeoPoint  geopoint1=new GeoPoint(Convert.ToDouble(rowItem1["Latitude"]),Convert.ToDouble(rowItem1["Longitude"]));       
							VLF.MAP.GeoPoint  geopoint2=new GeoPoint(Convert.ToDouble(rowItem2["Latitude"]),Convert.ToDouble(rowItem2["Longitude"]));       
							VLF.MAP.GeoPoint  center=VLF.MAP.MapUtilities.GetGeoCenter(geopoint1,geopoint2);
							map.GeoZones.Add (new GeoZoneIcon( geopoint1 , geopoint2,(VLF.CLS.Def.Enums.AlarmSeverity)SeverityId,(VLF.CLS.Def.Enums.GeoZoneDirection) Type,GeoZoneName  ) ); 

						}
						else //Polygon
						{

							GeoPoint[] points = new GeoPoint[dsGeoZoneDetails.Tables[0].Rows.Count];
							int i=0;
							foreach(DataRow rowItem in dsGeoZoneDetails.Tables[0].Rows)
							{
								points[i] = new GeoPoint( Convert.ToDouble(rowItem["Latitude"]),
									Convert.ToDouble(rowItem["Longitude"]) );
								i++;  
							}
							// TODO: put proper severity
							map.Polygons.Add(new PoligonIcon(points,(VLF.CLS.Def.Enums.AlarmSeverity)SeverityId,(VLF.CLS.Def.Enums.GeoZoneDirection)Type,GeoZoneName  ,true));
						

						}
					}

                map.DrawGeoZones = true;
                map.DrawPolygons = true;
                map.DrawLandmarks = false;

                string url = map.CreateMap();
                clsMap.AdjustMapURL(Request.IsSecureConnection, ref url);

                if (map.LastUsedMapID == VLF.MAP.MapType.NotSet)
                {
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "CreateMap returned: " + map.GetLastErrorDescription()));
                }
                else
                {
                    
                    ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();
                    if (objUtil.ErrCheck(dbs.AddMapUserUsage(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.MapTypes.Map), (short)map.LastUsedMapID), false))
                        if (objUtil.ErrCheck(dbs.AddMapUserUsage(sn.UserID, sn.SecId, Convert.ToInt16(VLF.CLS.Def.Enums.MapTypes.Map), (short)map.LastUsedMapID), true))
                        {
                            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceWarning, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Warning, "Update user map usage info failed."));
                        }
                }
							

				
				if (url!="")
					sn.GeoZone.ImgPath = url;
				
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
