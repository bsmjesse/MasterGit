using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using SentinelFM;
using System.Globalization;
using System.IO;
using VLF.CLS;

/// <summary>
/// Class for frmSensorsInfo.aspx, frmReefer.aspx
/// </summary>
public class SensorInfoPage : SentinelFMBasePage
{
   private static HtmlInputHidden hdnLicensePlate = new HtmlInputHidden();

   public string LicensePlate
   {
      get { return hdnLicensePlate.Value; }
      set { hdnLicensePlate.Value = value; }
   }

   public SensorInfoPage()
   {
      //
      // TODO: Add constructor logic here
      //
   }

   /// <summary>
   /// Retrieves sensors DataSet from the ViewState or if null - uses Web Method Call
   /// </summary>
   /// <param name="licensePlate">Vehicle License Plate</param>
   /// <param name="getAllSensors">True to get all box sensors, false to get box sensors only</param>
   /// <returns>Sensors for the vehicle</returns>
   protected DataTable GetAllSensorsForVehicle(string licensePlate, bool getAllSensors)
   {
      DataSet dsSensors = new DataSet("Sensors");
      DataTable dtSensors = new DataTable("BoxSensors");

      string xml = "";
     
         SentinelFM.ServerDBVehicle.DBVehicle dbv = new SentinelFM.ServerDBVehicle.DBVehicle();

         if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByLang(sn.UserID, sn.SecId, licensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), false))
            if (objUtil.ErrCheck(dbv.GetVehicleSensorsXMLByLang(sn.UserID, sn.SecId, licensePlate, CultureInfo.CurrentUICulture.TwoLetterISOLanguageName, ref xml), true))
            {
               System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                  VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                  "No Sensors for vehicle: " + licensePlate + " User:" + sn.UserID.ToString() + " Form:"+Page.GetType().Name));
            }

         if (String.IsNullOrEmpty(xml))
         {
            return null;
         }

         dsSensors.ReadXml(new StringReader(xml));
        
     

      dtSensors.Columns.Add("SensorId", typeof(short));
      dtSensors.Columns.Add("SensorName", typeof(string));
      dtSensors.Columns.Add("SensorAction", typeof(string));
      dtSensors.Columns.Add("AlarmLevelOn", typeof(short));
      dtSensors.Columns.Add("AlarmLevelOff", typeof(short));

      if (Util.IsDataSetValid(dsSensors))
      {
         foreach (DataRow rowSensor in dsSensors.Tables[0].Rows)
         {
            if (!getAllSensors)
               if (Convert.ToInt16(rowSensor["SensorId"]) > VLF.CLS.Def.Enums.ReeferBase)
                  continue;

            dtSensors.ImportRow(rowSensor);
         }
      }

      return dtSensors;
   }

   /// <summary>
   /// Check valid range
   /// </summary>
   /// <param name="lower"></param>
   /// <param name="upper"></param>
   /// <returns></returns>
   protected bool IsRangeValid(string lower, string upper, short min, short max)
   {
      short lowerLimit = 0, upperLimit = 0;
      if (!Int16.TryParse(lower, out lowerLimit))
         return false;
      if (lowerLimit < min || lowerLimit > max)
         return false;
      if (!Int16.TryParse(upper, out upperLimit))
         return false;
      if (upperLimit < min || upperLimit > max)
         return false;
      if (lowerLimit > upperLimit)
         return false;
      return true;
   }

   /// <summary>
   /// Check value
   /// </summary>
   /// <param name="textValue">Text</param>
   /// <param name="min">Min value</param>
   /// <param name="max">Max value</param>
   /// <returns>True if valid or false if not</returns>
   protected bool IsInRange(string textValue, short min, short max)
   {
      short limit = 0;
      if (!Int16.TryParse(textValue, out limit))
         return false;
      if (limit < min || limit > max)
         return false;
      return true;
   }

   /// <summary>
   /// Violation table
   /// </summary>
   /// <param name="text"></param>
   /// <returns></returns>
   protected string GetViolationValue(string text)
   {
      if (!IsInRange(text, 0, 100))
         return "0";
      else
         return text;
   }

  
}
