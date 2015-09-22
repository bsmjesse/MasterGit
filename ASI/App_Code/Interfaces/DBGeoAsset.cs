using System;
using System.Collections.Generic;
using System.Web;
using System.Web.Services;
using System.Data;
using System.Diagnostics;

using VLF.ERRSecurity;
using System.Xml;
using VLF.DAS.Logic;
using VLF.ASI;
using VLF.CLS;
using System.Text; 

namespace VLF.ASI.Interfaces
{
    [WebService(Namespace = "http://www.sentinelfm.com")]

    public class DBGeoAsset : System.Web.Services.WebService
    {

        public DBGeoAsset()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }


        [WebMethod(Description = "Get Geo Assets With Boundaries")]
        public string  GetGeoAssetsWithBoundaries(int userId, string SID, int orgId,
                   double topleftlat, double topleftlong, double bottomrightlat, double bottomrightlong,
                   double topleftlatnor, double topleftlongnor, double bottomrightlatnor, double bottomrightlongnor)
        {
            try
            {

                
               Log(">> GetGeoAssetsWithBoundaries(uId={0})", userId);

                // Authenticate & Authorize
               // LoginManager.GetInstance().SecurityCheck(userId, SID);

               string geoAssets = "";
                DataSet dsAsset = null;
                using (GeoAssetManager _geoAsset = new GeoAssetManager(LoginManager.GetConnnectionString(userId)))
                {
                   dsAsset= _geoAsset.GetGeoAssetsWithBoundaries(orgId,
                     topleftlat, topleftlong, bottomrightlat, bottomrightlong,
                    topleftlatnor, topleftlongnor, bottomrightlatnor, bottomrightlongnor);

                   
                }
                
                StringBuilder strData = new StringBuilder();
                string strResult = "";
                if (Util.IsDataSetValid(dsAsset))
                {
                    foreach (DataRow dr in dsAsset.Tables[0].Rows)
                        strData.Append(dr[0].ToString()+",");

                }

                if (strData.ToString().Length>0)
                    strResult = strData.ToString().Substring(0, strData.ToString().Length - 1);

                geoAssets = "var geoassets = [" + strResult + "]";
                

                LogFinal("<< GetGeoAssetsWithBoundaries(uId={0})",
                              userId);

                return geoAssets;


            }
            catch (Exception Ex)
            {
                LogException("<< GetAlarmsShortInfoXMLByLang : uId={0}, EXC={1})", userId, Ex.Message);
                return "";
            }
        }


        private void Log(string strFormat, params object[] objects)
        {
            try
            {
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                                       string.Format(strFormat, objects)));
            }
            catch (Exception exc)
            {

            }
        }

        /// <summary>
        ///   there are two important keywords 
        ///      -  uid         - user id
        ///      -  tSpan       - how fast was the operation
        ///   most of the time, the string format WILL CONTAIN KEYWORD, tSpan=... which is the time to execute 
        ///   the method name between << and (
        ///   in the same time, this function can send the information in real-time to a server
        /// </summary>
        /// <param name="strFormat"></param>
        /// <param name="objects"></param>
        private void LogFinal(string strFormat, params object[] objects)
        {
            try
            {
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                                       string.Format(strFormat, objects)));

            }
            catch (Exception exc)
            {

            }
        }

        /// <summary>
        ///      the exception should be saved in a separate file or in the Event log of the computer
        /// </summary>
        /// <param name="strFormat"></param>
        /// <param name="objects"></param>
        private void LogException(string strFormat, params object[] objects)
        {
            try
            {
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceError,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Error,
                                       string.Format(strFormat, objects)));
            }
            catch (Exception exc)
            {

            }
        }
    }
}

