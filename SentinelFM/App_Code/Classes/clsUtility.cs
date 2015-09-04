using System;
using System.Configuration;
using System.Globalization;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.SessionState;
using System.Linq;

using Enyim.Caching;
using Enyim.Caching.Memcached;
using Enyim.Caching.Configuration;

using VLF.ERRSecurity;

using ClosedXML.Excel;

using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;

using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.text.html;
using iTextSharp.text.html.simpleparser;

namespace SentinelFM
{
	/// <summary>
	/// Summary description for clsUtility.
	/// </summary>
    public class clsUtility 
	{
        public const string DTFORMAT_DMY = "dd/MM/yyyy";
        public const string DTFORMAT_MDY = "MM/dd/yyyy";
        public const string DTFORMAT_YMD = "yyyy/MM/dd";

		protected SentinelFMSession sn=null;

		public clsUtility(SentinelFMSession sn)
		{
			this.sn=sn;
		}

        public bool ErrCheck(int res, bool retrying,string lang, ref string errMsg)
        {
          
            bool retResult = true;
            switch ((InterfaceError)res)
            {
                case InterfaceError.CommInfoError:
                    errMsg = InterfaceErrorDescription.GetDescription((InterfaceError)res,lang);
                    break;

                case InterfaceError.SessionIsBusyError :
                   errMsg = InterfaceErrorDescription.GetDescription((InterfaceError)res, lang);
                    break;
                case InterfaceError.CallFrequencyExceeded:
                   errMsg = InterfaceErrorDescription.GetDescription((InterfaceError)res, lang);
                    break;
                default:
                    retResult = ErrCheck(res, retrying);
                    break;
            }
            return retResult;

        }

        public bool ErrCheck( int res, bool retrying )
		{
			bool retResult = true;
			switch( (InterfaceError)res )
			{
				case InterfaceError.NoError:
					retResult = false ;
					break;

				case InterfaceError.PassKeyExpired:
					//System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceVerbose,VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Verbose,"ErrCheck::InterfaceError.PassKeyExpired"));    
					// if expired, we have to re-login and retry again...
					// if we call te method after relogin and still expired, handle an error!
					if(!retrying)
					{
						// relogin
						SecurityManager.SecurityManager sec = new SecurityManager.SecurityManager() ;
						string secId = "";
                        int result = sec.ReloginMD5ByDBName (sn.UserID, sn.Key, sn.UserName, sn.Password, sn.User.IPAddr, ref secId);
						if(result!=0)
							sn.SecId= secId;
					}
					break;

              
                case InterfaceError.CallFrequencyExceeded:
                    if (!retrying)
                    {
                        sn.MessageText = Resources.Const.CallFrequencyExceeded;
                        ShowErrorMessage();
                    }
                    break;


                case InterfaceError.AuthenticationFailed:
                    // verify the userId == 0
                    sn.MessageText = "Authentication Failed";
                    if (sn.UserID == 0)
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo,
                           VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info,
                           "ErrCheck:: AuthenticationFailed-->Session timeout - UserId=0"));
                    else
                        System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                     VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                             "ErrCheck:: AuthenticationFailed--> " + res.ToString() + " , UserId=" + sn.UserID + " " +
                             InterfaceErrorDescription.GetDescription((InterfaceError)res) +
                             "--- Stack:" + new System.Diagnostics.StackTrace().ToString()));

                    break;

                case InterfaceError.AuthorizationFailed:
                    // verify the userId == 0
                    sn.MessageText = "Authorization Failed";
                     if (sn.UserID==0)
                         System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo,
                            VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info,
                            "ErrCheck:: AuthorizationFailed-->Session timeout - UserId=0"));
                     else
                         System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                      VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                              "ErrCheck:: AuthorizationFailed--> " + res.ToString() + " , UserId=" + sn.UserID + " " +
                              InterfaceErrorDescription.GetDescription((InterfaceError)res) +
                              "--- Stack:" + new System.Diagnostics.StackTrace().ToString()));
 
                    break;


                case InterfaceError.SessionNotFound:

                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceInfo,
                             VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Info,
                             "ErrCheck:: SessionNotFound --> Session [SEND COMMAND] not found. UserId=" + sn.UserID.ToString()));

                    break;


                case InterfaceError.ServerError:
				default:
					// get error description
                    //System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, 
                    //    VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, 
                    //            "ErrCheck::" + res.ToString + 
                    //            InterfaceErrorDescription.GetDescription((InterfaceError)res) +
                    //            "--- Stack:" + new System.Diagnostics.StackTrace().ToString()));



                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError,
                        VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error,
                                "ErrCheck:: " + res.ToString() + " "+
                                InterfaceErrorDescription.GetDescription((InterfaceError)res) +
                                "--- Stack:" + new System.Diagnostics.StackTrace().ToString()));

					break;
					
			}
			return retResult;
		}

		public Int16 IsDayLightSaving(bool AutoAdjustDayLightSaving)
		{
			if	(AutoAdjustDayLightSaving)
			{
				TimeZone timeZone = System.TimeZone.CurrentTimeZone;
				return Convert.ToInt16(timeZone.IsDaylightSavingTime(DateTime.Now));
			}
			else
			{
				return 0;
			}

		}

        private void ShowErrorMessage()
        {
            //Create pop up message
            string strUrl = "<script language='javascript'> function NewWindow(mypage) { ";
            strUrl = strUrl + "	var myname='Message';";
            strUrl = strUrl + " var w=370;";
            strUrl = strUrl + " var h=50;";
            strUrl = strUrl + " var winl = (screen.width - w) / 2; ";
            strUrl = strUrl + " var wint = (screen.height - h) / 2; ";
            strUrl = strUrl + " winprops = 'height='+h+',width='+w+',top='+wint+',left='+winl+'location=0,resizable,toolbar=0,scrollbars=0,menubar=0,'; ";
            strUrl = strUrl + " win = window.open(mypage, '_blank', winprops);}";

            strUrl = strUrl + " NewWindow('../frmMessage.aspx');</script>";

           System.Web.HttpContext.Current.Response.Write(strUrl);
        }

        public static bool IsNumeric(String str)
        {
            try
            {
                Double.Parse(str, System.Globalization.NumberStyles.Any);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public static string ValidateVEToken()
        {
            string token = "";
            DateTime dtNow = DateTime.Now;
            try
            {
                if (HttpContext.Current.Application["VE_Time"] != null)
                {
                    DateTime dtThen = Convert.ToDateTime(HttpContext.Current.Application["VE_Time"]);
                    TimeSpan ts = dtNow - dtThen;
                    if (ts.TotalMinutes >= Convert.ToInt32(Convert.ToInt32(ConfigurationSettings.AppSettings["VE_TokenValidityDurationMinutes"])))
                        token = GetVEToken();
                    else
                        return HttpContext.Current.Application["VE_Token"].ToString() ;
                }
                else if (HttpContext.Current.Application["VE_Time"] == null)
                {
                    token = GetVEToken();
                }


                lock (HttpContext.Current.Application)
                {
                    HttpContext.Current.Application["VE_Token"] = token;
                    HttpContext.Current.Application["VE_Time"] = dtNow.ToString();
                }
            }
            catch
            {
            }

            return token;

        }

        private static  string GetVEToken()
        {
            string username = ConfigurationSettings.AppSettings["VE_UserName"];
            string password = ConfigurationSettings.AppSettings["VE_password"];
            string bsmurl = ConfigurationSettings.AppSettings["VE_bsmurl"];

            try
            {
                //net.mappoint.staging.CommonServiceSoap cs = new net.mappoint.staging.CommonServiceSoap();
                //net.mappoint.staging.TokenSpecification tokenSpec = new net.mappoint.staging.TokenSpecification();

                net.mappoint.service.CommonServiceSoap cs = new net.mappoint.service.CommonServiceSoap();
                net.mappoint.service.TokenSpecification tokenSpec = new net.mappoint.service.TokenSpecification();

                cs.Credentials = new NetworkCredential(username, password);
                tokenSpec.ClientIPAddress = bsmurl;
                tokenSpec.TokenValidityDurationMinutes =Convert.ToInt32(ConfigurationSettings.AppSettings["VE_TokenValidityDurationMinutes"]) ;
                string token = cs.GetClientToken(tokenSpec);

                return token;  
            }
            catch
            {
                return null;
            }

        }

        public static void LogUserAction(Int32 userId, string strPage)
        {
            try
            {
                DataTable dt = (DataTable)HttpContext.Current.Application["dtUserCounter"];
                DataRow[] drCollections=null;
                if (dt!=null && dt.Rows.Count>0) 
                    drCollections=dt.Select("UserId=" + userId);

                if (drCollections != null && drCollections.Length > 0)
                {
                    drCollections[0]["Page"] = drCollections[0]["Page"] + ";" + strPage;
                    drCollections[0]["ActionDateTime"] = DateTime.Now;
                }
                else
                {
                    DataRow dr = dt.NewRow();
                    dr["UserId"] = userId;
                    dr["Page"] = strPage;
                    dr["ActionDateTime"] = DateTime.Now;
                    dt.Rows.Add(dr);
                }
                HttpContext.Current.Application.Lock();
                HttpContext.Current.Application["dtUserCounter"] = dt;
                HttpContext.Current.Application.UnLock();
            }

            catch
            {

            }
        }

        #region SentinelFM/Lite Session
        public static string SetSessionBaseToMemcahed(string SessionBase)
        {
            string key = Guid.NewGuid().ToString();
            MemcachedClient mc = new MemcachedClient();
            if (mc.Get(key) == null)
                mc.Store(StoreMode.Set, key, SessionBase, DateTime.Now.AddHours(12));

            return key;
        }

        public static string GetSessionBaseFromMemcahed(string key)
        {
            string SessionBase = "";
            MemcachedClient mc = new MemcachedClient();
            object obj = mc.Get(key);
            if (null != obj)
                SessionBase = (string)obj;

            return SessionBase;
        }
        #endregion

        #region SentinelFM/Lite Session to Text File
        public static string SetSessionBaseToTxtFile(string SessionBase)
        {

            // create a writer and open the file
            string key = Guid.NewGuid().ToString();
            string path=ConfigurationSettings.AppSettings["SentinelLite_SharedFolder"];
            TextWriter tw = new StreamWriter(path+key + ".txt");
            tw.WriteLine(SessionBase);
            tw.Close(); 
            return key;

        }

        public static string GetSessionBaseFromTxtFile(string key)
        {

            string SessionBase = "";
            string path = ConfigurationSettings.AppSettings["SentinelLite_SharedFolder"];
            TextReader tr = new StreamReader(path + key + ".txt");

            // read a line of text
            SessionBase= tr.ReadLine() ;

            // close the stream
            tr.Close();



            return SessionBase;
        }
        #endregion

        public static DataTable ReadDataFromExcel2003(string filePath)
        {
            HSSFWorkbook hssfworkbook;

            using (FileStream file = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                hssfworkbook = new HSSFWorkbook(file);
            }

            HSSFSheet sheet = (HSSFSheet)hssfworkbook.GetSheetAt(0);
            System.Collections.IEnumerator rows = sheet.GetRowEnumerator();

            DataTable dt = new DataTable();

            if (rows.MoveNext())
            {
                HSSFRow row0 = (HSSFRow)rows.Current;
                for (int i = 0; i < row0.LastCellNum; i++)
                {
                    HSSFCell cell = (HSSFCell)row0.GetCell(i);

                    dt.Columns.Add(cell.ToString());
                    //dr[i] = cell.ToString();

                }
            }

            while (rows.MoveNext())
            {
                HSSFRow row = (HSSFRow)rows.Current;
                DataRow dr = dt.NewRow();

                for (int i = 0; i < row.LastCellNum; i++)
                {
                    HSSFCell cell = (HSSFCell)row.GetCell(i);

                    if (cell != null)
                        dr[i] = cell.ToString();
                    else
                        dr[i] = "";

                }
                dt.Rows.Add(dr);
            }
            return dt;
        }

        public static string CreateCSV(string filepath, string filename, string fileextension, DataTable dt, string columnsList, string dateTimeColumns, string dateTimeFormat, bool deleteOldFiles, string title)
        {
            if (deleteOldFiles)
            {
                clsUtility.DeleteOldFiles(filepath, fileextension);
            }

            using (System.IO.StreamWriter file = new System.IO.StreamWriter(filepath + filename + "." + fileextension))
            {

                file.WriteLine("sep=,");

                string header = string.Empty;
                foreach (string column in columnsList.Split(','))
                {
                    string s = column.Split(':')[0];
                    header += "\"" + s + "\",";
                }
                header = header.Substring(0, header.Length - 1);
                
                file.WriteLine(header);

                foreach (DataRow row in dt.Rows)
                {
                    string data = string.Empty;
                    foreach (string column in columnsList.Split(','))
                    {
                        string s = string.Empty;
                        s = row[column.Split(':')[1]].ToString();
                        if (dateTimeColumns.Split(',').Contains(column.Split(':')[1]))
                            s = Convert.ToDateTime(s).ToString(dateTimeFormat);
                        data += "\"" + s.Replace("\"", "\"\"").Replace("\r", " ").Replace("\n", " ") + "\",";

                    }
                    data = data.Substring(0, data.Length - 1);
                    file.WriteLine(data);                    
                }
            }

            return filepath + filename + "." + fileextension;
        }

        public static string CreateExcel2003(string filepath, string filename, string fileextension, DataTable dt, string columnsList, string dateTimeColumns, string dateTimeFormat, bool deleteOldFiles, string title)
        {
            if (deleteOldFiles)
            {
                clsUtility.DeleteOldFiles(filepath, fileextension);
            }

            HSSFWorkbook wb = new HSSFWorkbook();
            ISheet ws = wb.CreateSheet("Sheet1");
            ICellStyle cellstyle1 = wb.CreateCellStyle();
            ICellStyle cellstyle2 = wb.CreateCellStyle();
            ICellStyle cellstyle3 = wb.CreateCellStyle();
            ICellStyle cellstyle4 = wb.CreateCellStyle();
            ICellStyle cellstyle5 = wb.CreateCellStyle();
            cellstyle1.FillPattern = FillPatternType.SOLID_FOREGROUND;
            cellstyle2.FillPattern = FillPatternType.SOLID_FOREGROUND;
            cellstyle3.FillPattern = FillPatternType.SOLID_FOREGROUND;
            cellstyle4.FillPattern = FillPatternType.SOLID_FOREGROUND;
            cellstyle5.FillPattern = FillPatternType.SOLID_FOREGROUND;
            HSSFPalette palette = wb.GetCustomPalette();
            palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.SEA_GREEN.index, (byte)123, (byte)178, (byte)115);
            palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.YELLOW.index, (byte)239, (byte)215, (byte)0);
            palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.LIGHT_ORANGE.index, (byte)255, (byte)166, (byte)74);
            palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.ROSE.index, (byte)222, (byte)121, (byte)115);
            palette.SetColorAtIndex(NPOI.HSSF.Util.HSSFColor.INDIGO.index, (byte)99, (byte)125, (byte)165);
            cellstyle1.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.SEA_GREEN.index;
            cellstyle2.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.YELLOW.index;
            cellstyle3.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.LIGHT_ORANGE.index;
            cellstyle4.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.ROSE.index;
            cellstyle5.FillForegroundColor = NPOI.HSSF.Util.HSSFColor.INDIGO.index;
            IRow row = ws.CreateRow(0);
            foreach (string column in columnsList.Split(','))
            {
                string s = column.Split(':')[0];
                row.CreateCell(row.Cells.Count).SetCellValue(s);
            }
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string data = string.Empty;
                IRow rowData = ws.CreateRow(i + 1);
                foreach (string column in columnsList.Split(','))
                {
                    string s = string.Empty;
                    s = dt.Rows[i][column.Split(':')[1]].ToString();
                    //if (column.Split(':')[1] == "TimeCreated")
                    //    s = Convert.ToDateTime(s).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                    if (dateTimeColumns.Split(',').Contains(column.Split(':')[1]))
                        s = Convert.ToDateTime(s).ToString(dateTimeFormat);
                    else
                        s = s.Replace("[br]", Environment.NewLine);
                    rowData.CreateCell(rowData.Cells.Count).SetCellValue(s);                    

                }
            }

            for (int i = 0; i < columnsList.Split(',').Length; i++)
            {
                try
                {
                    ws.AutoSizeColumn(i);
                }
                catch { }
            }

            using (FileStream fs = new FileStream(filepath + filename + "." + fileextension, FileMode.Create, FileAccess.Write))
            {
                wb.Write(fs);
            }

            return filepath + filename + "." + fileextension;
        }

        public static string CreateExcel2007(string filepath, string filename, string fileextension, DataTable dt, string columnsList, string dateTimeColumns, string dateTimeFormat, bool deleteOldFiles, string title)
        {
            if (deleteOldFiles)
            {
                clsUtility.DeleteOldFiles(filepath, fileextension);
            }

            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("Sheet1");
            foreach (string column in columnsList.Split(','))
            {
                string s = column.Split(':')[0];
                ws.Cell(1, ws.Row(1).CellsUsed().Count() + 1).Value = s;
            }

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string data = string.Empty;
                int iColumn = 1;
                foreach (string column in columnsList.Split(','))
                {
                    ws.Cell(i + 2, iColumn).DataType = XLCellValues.Text;

                    string s = string.Empty;
                    s = dt.Rows[i][column.Split(':')[1]].ToString();
                    //if (column.Split(':')[1] == "TimeCreated")
                    //    s = Convert.ToDateTime(s).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                    if (dateTimeColumns.Split(',').Contains(column.Split(':')[1]))
                        s = Convert.ToDateTime(s).ToString(dateTimeFormat);
                    else
                        s = s.Replace("[br]", Environment.NewLine);

                    ws.Cell(i + 2, iColumn).Value = "'" + s;
                    //ws.Cell(i + 2, iColumn).Value = "'" + dt.Rows[i][column.Split(':')[1]].ToString().Replace("[br]", Environment.NewLine);

                    iColumn++;
                }
            }

            ws.Rows().Style.Alignment.SetWrapText();
            ws.Rows().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Columns().AdjustToContents();


            wb.SaveAs(filepath + filename + "." + fileextension);

            return filepath + filename + "." + fileextension;
        }

        public static string CreatePDFFile(string filepath, string filename, string fileextension, DataTable dt, string columnsList, string dateTimeColumns, string dateTimeFormat, bool deleteOldFiles, string title)
        {
            try
            {
                if (deleteOldFiles)
                {
                    clsUtility.DeleteOldFiles(filepath, fileextension);
                }

                Document pdfDoc = new Document(new Rectangle(288f, 144f), 10, 10, 10, 10);
                pdfDoc.SetPageSize(iTextSharp.text.PageSize.A4.Rotate());
                PdfWriter.GetInstance(pdfDoc, new FileStream(filepath + filename + "." + fileextension, FileMode.Create));
                pdfDoc.Open();
                Chunk c = new Chunk("", FontFactory.GetFont("Verdana", 11));
                Font font8 = FontFactory.GetFont("ARIAL", 7);
                Font fontBold = FontFactory.GetFont("ARIAL", 7, Font.BOLD);
                var titleFont = FontFactory.GetFont("Arial", 18, Font.BOLD);
                var boldTableFont = FontFactory.GetFont("Arial", 12, Font.BOLD);
                pdfDoc.Add(new Paragraph(title, titleFont));
                pdfDoc.Add(new Paragraph(" ", font8));

                var DetailTable = new PdfPTable(2);
                DetailTable.HorizontalAlignment = 0;
                DetailTable.SpacingBefore = 10;
                DetailTable.SpacingAfter = 10;
                DetailTable.DefaultCell.Border = 0;
                //DetailTable.AddCell(new Phrase("From:", boldTableFont));
                //DetailTable.AddCell(strFromDate);
                //DetailTable.AddCell(new Phrase("To:", boldTableFont));
                //DetailTable.AddCell(strToDate);
                //DetailTable.AddCell(new Phrase("Fleet:", boldTableFont));

                //DetailTable.AddCell(new Phrase("Operation:", boldTableFont));
                //DetailTable.AddCell(operation);
                pdfDoc.Add(DetailTable);

                if (dt != null)
                {
                    PdfPTable PdfTable = new PdfPTable(columnsList.Split(',').Length);
                    PdfTable.WidthPercentage = 100;
                    PdfTable.DefaultCell.Border = Rectangle.NO_BORDER;
                    PdfTable.SplitRows = true;
                    PdfPCell PdfPCell = null;
                    foreach (string column in columnsList.Split(','))
                    {
                        string s = column.Split(':')[0];
                        //var r = new Regex(@"(?<=[a-z])(?=[A-Z])", RegexOptions.IgnorePatternWhitespace);
                        //s = r.Replace(s, " ");
                        PdfPCell = new PdfPCell(new Phrase(new Chunk(s, fontBold)));
                        PdfPCell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#538DD5"));
                        PdfPCell.VerticalAlignment = Element.ALIGN_MIDDLE;
                        PdfPCell.HorizontalAlignment = Element.ALIGN_MIDDLE;
                        PdfTable.AddCell(PdfPCell);
                    }
                    for (int rows = 0; rows < dt.Rows.Count; rows++)
                    {
                        foreach (string column in columnsList.Split(','))
                        {
                            string s = string.Empty;
                            s = dt.Rows[rows][column.Split(':')[1]].ToString();
                            //if (column.Split(':')[1] == "TimeCreated")
                                //s = Convert.ToDateTime(s).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                            if (dateTimeColumns.Split(',').Contains(column.Split(':')[1]))
                                s = Convert.ToDateTime(s).ToString(dateTimeFormat);
                            PdfPCell = new PdfPCell(new Phrase(new Chunk(s, font8)));

                            PdfPCell.BorderColorBottom = new Color(System.Drawing.ColorTranslator.FromHtml("#538DD5"));
                            if (rows % 2 != 0)
                            {
                                PdfPCell.BackgroundColor = new Color(System.Drawing.ColorTranslator.FromHtml("#C5D9F1"));
                            }
                            PdfTable.AddCell(PdfPCell);
                        }
                    }
                    pdfDoc.Add(PdfTable);
                }
                pdfDoc.Close();
            }
            catch (DocumentException de)
            {
                System.Web.HttpContext.Current.Response.Write(de.Message);
            }
            catch (IOException ioEx)
            {
                System.Web.HttpContext.Current.Response.Write(ioEx.Message);
            }
            catch (Exception ex)
            {
                System.Web.HttpContext.Current.Response.Write(ex.Message);
            }
            bool fileCreated = false;
            int numTry = 0;
            while (!fileCreated)
            {
                numTry++;
                try
                {
                    using (FileStream inputStream = File.Open(filepath + filename + "." + fileextension, FileMode.Open, FileAccess.Read, FileShare.None))
                    {
                        fileCreated = true;
                    }
                }
                catch (Exception ex)
                {
                    fileCreated = false;
                    if (numTry > 5)
                    {
                        fileCreated = true;
                        throw;
                    }

                }

            }
            return filepath + filename + "." + fileextension;
        }

        public static void DeleteOldFiles(string filepath, string fileextension)
        {
            try
            {
                var files = new DirectoryInfo(filepath).GetFiles("*." + fileextension);
                foreach (var file in files)
                {
                    if (DateTime.UtcNow - file.LastWriteTimeUtc > TimeSpan.FromDays(30))
                    {
                        File.Delete(file.FullName);
                    }
                }
            }
            catch { }
        }

        public static string EscapeStringValue(string value)
        {
            const char BACK_SLASH = '\\';
            const char SLASH = '/';
            const char DBL_QUOTE = '"';

            var output = new StringBuilder(value.Length);
            foreach (char c in value)
            {
                switch (c)
                {
                    case SLASH:
                        output.AppendFormat("{0}{1}", BACK_SLASH, SLASH);
                        break;

                    case BACK_SLASH:
                        output.AppendFormat("{0}{0}", BACK_SLASH);
                        break;

                    case DBL_QUOTE:
                        output.AppendFormat("{0}{1}", BACK_SLASH, DBL_QUOTE);
                        break;

                    default:
                        output.Append(c);
                        break;
                }
            }

            return output.ToString();
        }

        #region Data Conversion Functions

        public static string DateTimeToString(DateTime val) {
            if(System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower() == "fr-ca")
                return DateTimeToString(val, DTFORMAT_DMY);
            else
                return DateTimeToString(val, DTFORMAT_MDY);
        }

        public static string DateTimeToString(DateTime val, string format)
        {
            return val.ToString(format);
        }

        public static string DateTimeToString(DateTime val, System.Globalization.DateTimeFormatInfo dtformat)
        {
            if (System.Globalization.CultureInfo.CurrentUICulture.ToString().ToLower() == "fr-ca")
                return DateTimeToString(val, DTFORMAT_DMY);
            else
                return DateTimeToString(val, DTFORMAT_MDY);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="val"></param>
        /// <param name="NumberStyle"></param>
        /// <returns></returns>
        public static bool isNumeric(string val, System.Globalization.NumberStyles NumberStyle)
        {
            double result;
            return Double.TryParse(val, NumberStyle, System.Globalization.CultureInfo.CurrentCulture, out result);
        }

        public static bool isNumeric(char val)
        {
            return char.IsNumber(val);
        }

        public static bool isNumericRegex(string val)
        {
            var regex = new Regex(@"^-*[0-9,\.]+$");
            return regex.IsMatch(val);
        }

        /// <summary>
        /// Overloading
        /// </summary>
        /// <param name="DateValue"></param>
        /// <param name="Dateformat"></param>
        /// <returns></returns>
        public static DateTime StringToDateTime(string DateValue, string Dateformat)
        {
            return StringToDateTime(DateValue, Dateformat, System.Globalization.CultureInfo.CurrentUICulture.ToString());
        }

        /// <summary>
        /// Format Date/Time accouding Current UI Culture.
        /// Support two format: MM/DD/YYYY hh:mm:ss AM|PM (12h, Default) for EN and DD/MM/YYYY HH:MM:SS (24h) for FR. 
        /// </summary>
        /// <param name="value"></param>
        /// <param name="format"></param>
        /// <param name="cultureinfo"></param>
        /// <returns></returns>
        public static DateTime StringToDateTime(string value, string format, string cultureinfo)
        {

            CultureInfo culture = new CultureInfo(cultureinfo);
            DateTime date = DateTime.Now;
            string err = "";

            try
            {
                if (format.ToLower().IndexOf("hh") >= 0)
                    value = Convert.ToDateTime(value).ToString(format);

                date = DateTime.ParseExact(value, format, null);
                err = "";
            }
            catch (FormatException fx)
            {
                err = fx.Message;
                date = DateTime.Now;
            }
            catch (Exception ex)
            {
                err = ex.Message;
                date = DateTime.Now;
            }
            finally
            {
            }

            return date;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int StringToInt(string value)
        {
            return StringToInt(value, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static int StringToInt(string value, int defaultValue)
        {
            int i = 0;
            if (int.TryParse(value, out i))
                return i;
            else
                return defaultValue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Int32 StringToInt32(string value)
        {
            return StringToInt32(value, 0) ;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Int32 StringToInt32(string value, Int32 defaultvalue)
        {
            Int32 i = 0;
            if (Int32.TryParse(value, out i))
                return i;
            else
                return defaultvalue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Int64 StringToInt64(string value)
        {
            return StringToInt64(value, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Int64 StringToInt64(string value, Int64 defaultvalue)
        {
            Int64 i = 0;
            if (Int64.TryParse(value, out i))
                return i;
            else
                return defaultvalue;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Int16 StringToInt16(string value)
        {
            return StringToInt16(value, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Int16 StringToInt16(string value, Int16 defaultvalue)
        {
            Int16 i = 0;
            if (Int16.TryParse(value, out i))
                return i;
            else
                return defaultvalue;
        }

        #endregion
    }
}
