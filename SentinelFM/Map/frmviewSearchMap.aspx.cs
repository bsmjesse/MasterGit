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
using VLF.MAP;
using VLF.CLS.Interfaces;
using System.IO ;
namespace SentinelFM
{
	/// <summary>
	/// Summary description for frmViewLandMark.
	/// </summary>
    public partial class frmviewSearchMap : SentinelFMBasePage
	{

		
		public ClientMapProxy map;
		public int imageW=655;
		public int imageH=361;
		

		protected void Page_Load(object sender, System.EventArgs e)
		{
			

            // create ClientMapProxy only for mapping
            map = new VLF.MAP.ClientMapProxy(sn.User.MapEngine);
            if (map == null)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "Can't create map " + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
                return;
            }
            map.MapWidth = imageW;
            map.MapHeight = imageH;

			string x=Request.QueryString["x"];
            string y = Request.QueryString["y"];
            //map.MapCenterLatitude = Convert.ToDouble(y);
            //map.MapCenterLongitude =Convert.ToDouble(x);

            map.Landmarks.Add(new MapIcon(Convert.ToDouble(y), Convert.ToDouble(x), "Landmark.ico", ""));

            DrawMap();
		}

		private void DrawMap()
		{

            map.DrawGeoZones = false;
            map.DrawPolygons = false;
            map.DrawLandmarks = true ;
            //MapZoomLevel mapZoomLevel = MapZoomLevel.StreetLevelNorm5;
            //map.Zoom(mapZoomLevel);
            
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
				sn.Map.ImgPath = url;
			
			 
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
