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
   private string ResultTableName = "InvoiceTable";
   protected SentinelFMSession sn = null;
   protected clsUtility objUtil;
   DataTable dtResult = null;
   DataTable dtError = null;
   
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

         this.HyperLink1.NavigateUrl = "";  
         this.HyperLink1.Visible = false;

      }
      catch
      {
         Label1.Text = "Error loading invoice page";
         return;
      }
   }

   /// <summary>
   /// Generate a datatable which will be passed into AddOrganizationLandmarks method
   /// </summary>
   /// <returns></returns>
   private DataTable GenerateResultTable()
   {
       dtResult = new DataTable("InvoiceTable");
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
   private Boolean  ParseInvoiceXLS(string path, bool hdrYes, int GeoCodeEngine)
   {
      DataSet dsData = new DataSet("Data");
      string hdr = "";
      
      // first line contains headers or data
      if (hdrYes) hdr = "HDR=Yes";
      else hdr = "HDR=No";

      // connection string
      string connString = String.Format(
         "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties=\"Excel 8.0;{1};IMEX=1\";", 
         path, hdr);

      // get data into dataset
      using (OleDbConnection conn = new OleDbConnection(connString))
      {
         OleDbDataAdapter da = new OleDbDataAdapter("SELECT * FROM [Sheet1$]", conn);
         da.Fill(dsData, "tblMaster");
      }

      WriteNew(dsData.Tables[0], Server.MapPath("data.txt"));

       this.HyperLink1.Visible = true;
  
      return true ;
   }

   
   protected void cmdSubmit_Click(object sender, EventArgs e)
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
                       Label1.Text = "Error saving uploading file" +exc.Message;
                       return;
                   }
                   if (!ParseInvoiceXLS(fileName, CheckBox2.Checked, 1)) return;
                  
               }
               catch (Exception exc)
               {
                   Label1.Text = "Error uploading file. Please check the xls file format. " + exc.Message;
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



   static void Write(DataTable dt, string outputFilePath)
   {
       int[] maxLengths = new int[dt.Columns.Count];

       for (int i = 0; i < dt.Columns.Count; i++)
       {
           maxLengths[i] = dt.Columns[i].ColumnName.Length;

           foreach (DataRow row in dt.Rows)
           {
               if (!row.IsNull(i))
               {
                   int length = row[i].ToString().Length;

                   if (length > maxLengths[i])
                   {
                       maxLengths[i] = length;
                   }
               }
           }
       }

       using (StreamWriter sw = new StreamWriter(outputFilePath, false))
       {
           for (int i = 0; i < dt.Columns.Count; i++)
           {
               sw.Write(dt.Columns[i].ColumnName.PadRight(maxLengths[i] + 2));
           }

           sw.WriteLine();

           foreach (DataRow row in dt.Rows)
           {
               for (int i = 0; i < dt.Columns.Count; i++)
               {
                   if (!row.IsNull(i))
                   {
                       sw.Write(row[i].ToString().PadRight(maxLengths[i] + 2));
                   }
                   else
                   {
                       sw.Write(new string(' ', maxLengths[i] + 2));
                   }
               }

               sw.WriteLine();
           }

           sw.Close();
       }
   }



   static void WriteNew(DataTable dt, string outputFilePath)
   {



       string vehicle = "";
       string boxid = "";
       string cell = "";

           using (StreamWriter sw = new StreamWriter(outputFilePath, false))
           {
            

               foreach (DataRow row in dt.Rows)
               {
                   
                       sw.Write(row[0].ToString().Trim().PadRight(2));
                       sw.Write(row[1].ToString().Trim().PadRight(12));
                       sw.Write(Convert.ToString(row[2].ToString() + row[3].ToString().PadLeft(2, '0') + row[4].ToString().PadLeft(2, '0') + "120000"));//DateTime
                       vehicle = row[6].ToString().Trim();
                       if (vehicle.Length>10)
                            vehicle=row[6].ToString().Trim().Substring(0,10);

                       cell = row[8].ToString().Trim();

                       if (cell.Length > 10)
                           cell = row[8].ToString().Trim().Substring(0, 10);

                       sw.Write(vehicle.PadRight(10));//Vehicle
                       sw.Write(row[7].ToString().Trim().PadRight(6));//BoxId
                       sw.Write(cell.PadRight(10));//Cell
                       sw.Write(row[9].ToString().Trim().PadLeft(7, '0'));//Odometer
                       sw.Write(Convert.ToDecimal(row[10]).ToString("n2").Replace(".", "").PadLeft(7, '0') + "{");//Net
                       sw.Write(Convert.ToDecimal(row[11]).ToString("n2").Replace(".", "").PadLeft(6, '0') + "{");//GST
                       sw.Write(Convert.ToDecimal(row[12]).ToString("n2").Replace(".", "").PadLeft(6, '0') + "{");//PST
                       sw.Write(Convert.ToDecimal(row[13]).ToString("n2").Replace(".", "").PadLeft(10, '0') + "{");//Total Net
                       sw.Write(Convert.ToDecimal(row[14]).ToString("n2").Replace(".", "").PadLeft(6, '0') + "{");//Total-GST
                       sw.Write(Convert.ToDecimal(row[15]).ToString("n2").Replace(".", "").PadLeft(6, '0') + "{");//T-PST
                       sw.Write(row[16].ToString().PadLeft(5, '0') );//Vendor
                       sw.WriteLine();
               }

               sw.Close();

              
           }
   }
       
   //   void  SendEmail(string from, string to, string subject, string body)
   //     {
   //         if (string.IsNullOrEmpty(to) || string.IsNullOrEmpty(body))
   //             return;

   //       VLF.MailLib.EMailMessageAuth eMail = null;
   //        eMail = new VLF.MailLib.EMailMessageAuth(
   //                          System.Configuration.ConfigurationManager.AppSettings["UserName"],
   //                          System.Configuration.ConfigurationManager.AppSettings["Password"],
   //                          System.Configuration.ConfigurationManager.AppSettings["SMTPServer"]);
   //         try
   //         {
   //             eMail.SendMail(to, subject, body, "http://localhost/SentinelAdmin/ActivitySummaryReportPerVehicle_Old");
                
   //         }
   //         catch 
   //         {
                
   //         }

            
    

   //}
       


}