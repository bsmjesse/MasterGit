using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Data.OleDb;
using VLF.MAP;
using VLF.DAS.Logic;
using System.IO;
using SentinelFM;

public partial class _Default : System.Web.UI.Page 
{
   private string ResultTableName = "LandmarksTable";
   protected SentinelFMSession sn = null;
   protected clsUtility objUtil;
   DataTable dtResult = null;
   DataTable dtError = null;
   string noteMessage = "The XLS file should have columns in the following order: Landmark Name, Description, Radius, Street, City, Province, Latitude, Longitude, Contact Name, Contact Phone, Email, TimeZone.";
   protected void Page_Load(object sender, EventArgs e)
   {
      try
      {
         sn = (SentinelFMSession)Session["SentinelFMSession"];
         if ((sn == null) || (sn.UserName == ""))
         {
            Response.Write("<SCRIPT Language='javascript'>window.open('frmAdminLogin.aspx','_top') </SCRIPT>");
            return;
         }
          if (!IsPostBack) lblMessage.Text = noteMessage;
      }
      catch
      {
         Label1.Text = "Error loading landmarks page";
         return;
      }
   }

   /// <summary>
   /// Generate a datatable which will be passed into AddOrganizationLandmarks method
   /// </summary>
   /// <returns></returns>
   private DataTable GenerateResultTable()
   {
       dtResult = new DataTable("LandmarksTable");
       dtResult.Columns.Add("Name", typeof(string));
       dtResult.Columns.Add("Latitude", typeof(double));
       dtResult.Columns.Add("Longitude", typeof(double));
       dtResult.Columns.Add("Description", typeof(string));
       dtResult.Columns.Add("Contact", typeof(string));
       dtResult.Columns.Add("PhoneNumber", typeof(string));
       dtResult.Columns.Add("Radius", typeof(int));
       dtResult.Columns.Add("Email", typeof(string));
       dtResult.Columns.Add("TimeZone", typeof(int));
       dtResult.Columns.Add("DayLightSaving", typeof(bool));
       dtResult.Columns.Add("AutoDayLightSaving", typeof(bool));
       dtResult.Columns.Add("Address", typeof(string));
       return dtResult;
   }

   /// <summary>
   /// Builds Master - Details Landmarks dataset
   /// </summary>
   /// <param name="path">XLS File path</param>
   /// <param name="hdrYes">Is the first line a header line</param>
   /// <param name="GeoCodeEngine">GeoCodeEngine code</param>
   /// <returns>Is error</returns>
   private Boolean  ParseLandmarksXLS(string path, bool hdrYes, int GeoCodeEngine)
   {
      DataSet dsLandmarks = new DataSet("Landmarks");
      string hdr = "", xmlDetails = "";
      
      // first line contains headers or data
      if (hdrYes) hdr = "HDR=Yes";
      else hdr = "HDR=No";

      // connection string
      string connString = String.Format(
	 "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties=\"Excel 12.0 Xml;HDR=Yes;IMEX=1\";",
         path, hdr);

      // get data into dataset
      using (OleDbConnection conn = new OleDbConnection(connString))
      {
         OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", conn);
         da.Fill(dsLandmarks, "tblMaster");
      }

      //Check columns number
      if (dsLandmarks.Tables[0].Columns.Count < 12)
      {
          Label1.Text = "Error: " + noteMessage;
          return false;
      }


      ServerDBSystem.DBSystem dbs = new ServerDBSystem.DBSystem();
      DataSet dsGeoCodeEngines = new DataSet();
      DataSet dsMapEngines = new DataSet();
      string xml = "";
      clsUtility objUtil = new clsUtility(sn);

      if (objUtil.ErrCheck(dbs.GetUserGeoCodeEngineInfoXML(sn.UserID, sn.SecId, ref xml), false))
          if (objUtil.ErrCheck(dbs.GetUserGeoCodeEngineInfoXML(sn.UserID, sn.SecId, ref xml), true))
          {
              System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, "GetGeoCodeEnginesInfo. User:" + sn.UserID.ToString() + " Form:clsUser "));
          }
      if (xml != "")
      {
          StringReader strrXML = new StringReader(xml);
          dsGeoCodeEngines.ReadXml(strrXML);
      }

      VLF.MAP.ClientMapProxy geoMap = new VLF.MAP.ClientMapProxy(VLF.MAP.MapUtilities.ConvertGeoCodersToMapEngine(dsGeoCodeEngines));
      //Generate result table
      GenerateResultTable();
      dtError = dtResult.Clone();
       DataSet ds;
      // parse data
      foreach (DataRow masterRow in dsLandmarks.Tables[0].Rows)
      {
          try
          {

              //Add row to result table 
              DataRow dr = dtResult.NewRow();

              // build address for geoMap method

             //---GeoMicro
             // string strAddress = String.Format("{0}|{1}, {2}",
             //    masterRow[3], masterRow[4], masterRow[5]);



              //Telogis
              string strAddress = String.Format("{0},{1},{2},,Canada",
                  masterRow[3], masterRow[4], masterRow[5]);



              



            
              try
              {
                


                  //--Telogis
                  //double X = 0; double Y = 0;
                  Resolver.Resolver res = new Resolver.Resolver();
                  //string resolvedAddress = "";
                  //res.Location(strAddress, ref Y, ref X, ref resolvedAddress);
                  //dr["Latitude"] = Y;
                  //dr["Longitude"] = X;

                  //

                  double longitude_1 = 0;
                  double latitude_1 = 0;
                  if (masterRow[7] != null && masterRow[7] !="0")
                      double.TryParse(masterRow[7].ToString(), out longitude_1);
                  if (masterRow[6] != null && masterRow[6] != "0")
                      double.TryParse(masterRow[6].ToString(), out latitude_1);

                  if (longitude_1 != 0)
                  {
                      res.StreetAddress(latitude_1, longitude_1, ref strAddress);
                      dr["Latitude"] = latitude_1;
                      dr["Longitude"] = longitude_1;
                  }
                  else
                      continue;
              

                  //--Telogis
              }
              catch(Exception ex)
              {
                  //Label1.Text = "Error in resolving address: " + strAddress;// +exc.Message;
                  //return false;
              }
              int radius = 0;
              int tz = 0;
              if (masterRow[2] != null)
                  int.TryParse(masterRow[2].ToString().Trim(), out radius);

              if (masterRow[11] != null)
                  int.TryParse(masterRow[11].ToString().Trim(), out tz);


              dr["Name"] = masterRow[0];
              dr["Description"] = masterRow[1];
              dr["Contact"] = masterRow[8];
              dr["PhoneNumber"] = masterRow[9];
              dr["Radius"] = radius;
              dr["Email"] = masterRow[10];
              dr["TimeZone"] = tz;
              dr["DayLightSaving"] = false;
              dr["AutoDayLightSaving"] = false;
           
              //dr["Address"] = String.Format("{0},{1},{2}",
              //       masterRow[3], masterRow[4], masterRow[5]);


              dr["Address"] = strAddress;

              if (dr["Latitude"].ToString() == "0" && dr["Longitude"].ToString() == "0")
              {
                  DataRow errorDr = dtError.NewRow();
                  errorDr["Name"] = dr["Name"];
                  errorDr["Description"] = dr["Description"];
                  errorDr["Contact"] = dr["Contact"];
                  errorDr["PhoneNumber"] = dr["PhoneNumber"];
                  errorDr["Radius"] = dr["Radius"];
                  errorDr["Email"] = dr["Email"];
                  errorDr["TimeZone"] = dr["TimeZone"];
                  errorDr["DayLightSaving"] = dr["DayLightSaving"];
                  errorDr["AutoDayLightSaving"] = dr["AutoDayLightSaving"];
                  errorDr["Address"] = dr["Address"];
                  dtError.Rows.Add(errorDr);
              }
              else dtResult.Rows.Add(dr);
          }
          catch(Exception ex)
          {
              Label1.Text = "Error in uploading data. " + noteMessage;// +exc.Message;
              return false;
          }
      }
        
      gvErrors.DataSource = dtError;
      gvErrors.DataBind();
      return true ;
   }

   protected void Submit2_Click(object sender, EventArgs e)
   {
      if (!Page.IsValid)
      {
           return;
      }
      try
      {
         bool fileOK = false;
         string ext = "";
         if (FileUpload1.HasFile)
         {
            ext = System.IO.Path.GetExtension(FileUpload1.FileName).ToLower();
            if (ext == ".xls") fileOK = true;
         }
         if (fileOK)
         {
            // parse xls file
            try
            {
               string serverPath = Server.MapPath("~/App_Data/");
               string fileName = serverPath + FileUpload1.FileName;
               try
               {
                   FileUpload1.PostedFile.SaveAs(fileName);
               }
               catch (Exception exc)
               {
                   Label1.Text = "Error saving uploading file";// +exc.Message;
                   return;
               }
               if  (!ParseLandmarksXLS(fileName, CheckBox2.Checked, 1) ) return;
               if (dtResult != null && dtResult.Rows.Count > 0)
               {
                   string landmarksResult = "";
                   using (ServerDBOrganization.DBOrganization orgProxy = new ServerDBOrganization.DBOrganization())
                   {
                       DataSet dsResult = new DataSet("LandmarksSet");
                       dsResult.Tables.Add(dtResult);
                       // Send all landmarks 
                       //Comment by devin
                       try
                       {
                           foreach (DataRow dr in dtResult.Rows)
                           {
                               int result = orgProxy.AddOrganizationLandmark(sn.UserID, sn.SecId, sn.User.OrganizationId,
                                    double.Parse(dr["Latitude"].ToString()),
                                    double.Parse(dr["Longitude"].ToString()),
                                    dr["Name"].ToString(),
                                    dr["Description"].ToString(),
                                    dr["Contact"].ToString(),
                                    dr["PhoneNumber"].ToString(),
                                    int.Parse(dr["Radius"].ToString()),
                                    dr["Email"].ToString(),
                                    short.Parse(dr["TimeZone"].ToString()),
                                    false,
                                    false,
                                    dr["Address"].ToString());
                           }
                       }
                       catch
                       {
                           Label1.Text = "Error saving to database. Landmark Name: "  ;
                           return;
                       }
                       //int result = orgProxy.AddOrganizationLandmarks(sn.UserID, sn.SecId, sn.User.OrganizationId, dsResult, ref landmarksResult);
                       //double l1 = 0,l2 = 0 ;
                       //orgProxy.GetLandmarkLocation(sn.UserID, sn.SecId, sn.User.OrganizationId, "Six Within", ref l1, ref l2);
                   }

                   //if (String.IsNullOrEmpty(landmarksResult))
                   Label1.Text = "The file was uploaded successfully";
                   //else Label1.Text = landmarksResult;
               }
               else
               {
                   if (dtError.Rows.Count  == 0)
                      Label1.Text = "The file does not contain any data";
                   else 
                   {
                       Label1.Text = "Please check the xls file format. " + noteMessage ;
                   }
               }
            }
            catch (Exception exc)
            {
                Label1.Text = "Error uploading file. Please check the xls file format. " + noteMessage;// +exc.Message;
            }
         }
         else Label1.Text = "Files of this type are not supported: *." + ext;
      }
      catch
      {
         Label1.Text = "Error uploading file";
         return;
      }
   }
}
