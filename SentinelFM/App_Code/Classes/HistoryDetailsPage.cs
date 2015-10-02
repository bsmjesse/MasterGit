using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Globalization;
using SentinelFM;
using System.IO;

/// <summary>
/// Summary description for HistoryDetailsPage
/// </summary>
public class HistoryDetailsPage : SentinelFMBasePage 
{
   protected SentinelFM.ServerDBVehicle.DBVehicle dbv = new SentinelFM.ServerDBVehicle.DBVehicle();

   public HistoryDetailsPage()
   {
      //
      // TODO: Add constructor logic here
      //
   }

   /// <summary>
   /// Retrieves sensors DataSet from the Session or if null - via the Web Method Call
   /// </summary>
   /// <param name="licensePlate">Vehicle License Plate</param>
   /// <param name="errorMessage">Error message if web call fails</param>
   /// <returns>DataSet Sensors for the vehicle</returns>
   protected DataSet GetAllSensorsForBox(int boxId)
   {
      DataSet dsSensors = new DataSet("Sensors");

      string xml = "";
      if (Session["Sensors"] == null)
      {
         //SentinelFM.ServerDBVehicle.DBVehicle dbv = new SentinelFM.ServerDBVehicle.DBVehicle();
        

         if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByBoxId(sn.UserID, sn.SecId, boxId, ref xml), false))
            if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByBoxId(sn.UserID, sn.SecId, boxId, ref xml), true))
            {
               System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                  VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                  "No Sensors for box: " + boxId + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

         if (String.IsNullOrEmpty(xml))
         {
            return null;
         }

         dsSensors.ReadXml(new StringReader(xml));

         Session["Sensors"] = dsSensors;
      }
      else
         dsSensors = (DataSet)Session["Sensors"];

      return dsSensors;
   }

   protected override void OnUnload(EventArgs e)
   {
      if (Session["Sensors"] != null)
         Session.Remove("Sensors");
      base.OnUnload(e);
   }

}
