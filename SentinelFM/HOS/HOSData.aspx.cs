using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SentinelFM;
using HOS_WS;
using System.Data;

using ClosedXML.Excel;
using System.IO;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using Newtonsoft.Json;
using VLF.CLS.Def;

public partial class HOS_HOSData : System.Web.UI.Page
{
    SentinelFMSession sn = null;

    private int vlStart = 0;
    private int vlLimit = 20;
    string driverName = string.Empty;
    string fleetId = string.Empty;
    
    private string operation;
    private string formattype;

    protected void Page_Load(object sender, EventArgs e)
    {
        try
        {
            //sn = (SentinelFMSession)Session["SentinelFMSession"];
            if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
            else return;

            if (!Page.IsPostBack)
            {
                string request = Request.QueryString["QueryType"];

                if (string.IsNullOrEmpty(request))
                {
                    return;
                }

                if (request.Equals("GetDriverStatus", StringComparison.CurrentCultureIgnoreCase))
                {
                    request = Request.QueryString["driverName"];
                    if (!string.IsNullOrEmpty(request))
                    {
                        driverName = request;
                    }

                    request = Request.QueryString["start"];
                    if (!string.IsNullOrEmpty(request))
                    {
                        Int32.TryParse(request, out vlStart);
                        //if (vlStart < 0) vlStart = 0;
                    }

                    request = Request.QueryString["limit"];
                    if (!string.IsNullOrEmpty(request))
                    {
                        Int32.TryParse(request, out vlLimit);
                        //if (vlLimit <= 0) vlStart = vlLimit;
                    }

                    request = Request.QueryString["FleetId"];
                    if (!string.IsNullOrEmpty(request))
                    {
                        fleetId = request;
                    }

                    GetDriverStatus();
                }
                else if (request.Equals("ExportDriverStatus", StringComparison.CurrentCultureIgnoreCase))
                {
                    request = Request.QueryString["operation"];
                    if (!string.IsNullOrEmpty(request))
                        operation = request;
                    else
                        operation = string.Empty;

                    request = Request.QueryString["formattype"];
                    if (!string.IsNullOrEmpty(request))
                        formattype = request;
                    else
                        formattype = string.Empty;

                    //exportDatatable(dtDS, formattype,

                    if (operation == "Export" && !string.IsNullOrEmpty(formattype))
                    {
                        request = Request.QueryString["columns"];
                        if (!string.IsNullOrEmpty(request))
                        {
                            DataTable dtDS = GetAllDriverStatus();
                            if(dtDS != null) exportDatatable(dtDS, formattype, request, "DriverStatus");
                            return;
                        }
                    }
                }
            }
        }
        catch (NullReferenceException Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));
        }
        catch (Exception Ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
        }
    }

    private void GetDriverStatus()
    {
        //SentinelFMSession sn;
        System.IO.StringReader strXML;
        //if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        //else return;
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return;

        if (string.IsNullOrEmpty(fleetId)) fleetId = sn.User.DefaultFleet.ToString();

        try
        {
            HOSFileReteriver hfs = new HOSFileReteriver();
            string result = hfs.GetAllDriverStatus(sn.User.OrganizationId);
            //string result = hfs.GetAllDriverStatusNew(sn.User.OrganizationId, sn.UserID);
            //string header="xmlns=\"http://tempuri.org/HOS_DriverDashBoard.xsd\"";
            //result=result.Replace(header,string.Empty);
            //result = result.Replace("\n", string.Empty);
            Response.ContentType = "text/xml";
            //if (vlStart == 0 && vlLimit == 20)
            //{
            //    Response.Write(result);
            //}
            //else
            //{
            strXML = new System.IO.StringReader(result);
            DataSet iDataSet = new DataSet();
            iDataSet.ReadXml(strXML);

            DataSet dstemp = new DataSet();
            //DataView dv = iDataSet.Tables[0].DefaultView;
            //DataView dv = iDataSet.Tables[0].DefaultView;

            DataRow[] drDriver_Fleet = iDataSet.Tables[0].Select(string.Format("FleetIds LIKE '%,{0},%'", fleetId));
            DataView dv = drDriver_Fleet.CopyToDataTable().DefaultView;
            if (!string.IsNullOrEmpty(driverName))
            {
                //DataRow[] drDriver = iDataSet.Tables[0].Select(string.Format("DriverName LIKE '%{0}%'",driverName));
                DataRow[] drDriver = iDataSet.Tables[0].Select(string.Format("DriverName LIKE '%{0}%' AND FleetIds LIKE '%,{1},%'", driverName, fleetId));
                dv = drDriver.CopyToDataTable().DefaultView;
            }
            dv.Sort = "DriverID ASC";
            DataTable sortedTable = dv.ToTable();
            DataTable dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
            dt.TableName = "DriverDashBoard";
            dstemp.Tables.Add(dt);
            dstemp.DataSetName = "HOS_DriverDashBoard";
            result = dstemp.GetXml();
            result = result.Replace("<HOS_DriverDashBoard>", "<HOS_DriverDashBoard><totalCount>" + dv.ToTable().Rows.Count.ToString() + "</totalCount>");//iDataSet.Tables[0].Rows.Count
            Response.Write(result);
            //}
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + @"Web method: GetDriverStatus() Page:HOS\HOSData"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            return;
        }
    }

    // eXport functionalities
    private DataTable GetAllDriverStatus()
    {
        DataTable dt = new DataTable();
        //SentinelFMSession sn;
        System.IO.StringReader strXML;
        //if (HttpContext.Current.Session["SentinelFMSession"] != null) sn = (SentinelFMSession)HttpContext.Current.Session["SentinelFMSession"];
        //else return;
        if (sn.User == null || String.IsNullOrEmpty(sn.UserName)) return null;

        if (string.IsNullOrEmpty(fleetId)) fleetId = sn.User.DefaultFleet.ToString();

        try
        {
            HOSFileReteriver hfs = new HOSFileReteriver();
            string result = hfs.GetAllDriverStatus(sn.User.OrganizationId);
           
            strXML = new System.IO.StringReader(result);
            
            DataSet iDataSet = new DataSet();
            iDataSet.ReadXml(strXML);

            DataRow[] drDriver_Fleet = iDataSet.Tables[0].Select(string.Format("FleetIds LIKE '%,{0},%'", fleetId));
            DataView dv = drDriver_Fleet.CopyToDataTable().DefaultView;
            //if (!string.IsNullOrEmpty(driverName))
            //{
            //    DataRow[] drDriver = iDataSet.Tables[0].Select(string.Format("DriverName LIKE '%{0}%' AND FleetIds LIKE '%,{1},%'", driverName, fleetId));
            //    dv = drDriver.CopyToDataTable().DefaultView;
            //}
            dv.Sort = "DriverID ASC";
            dt = dv.ToTable();
            //DataTable sortedTable = dv.ToTable();
            //DataTable dt = sortedTable.AsEnumerable().Skip(vlStart).Take(vlLimit).CopyToDataTable();
            dt.TableName = "GetAllDriverStatus";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, ex.Message.ToString() + " User:" + sn.UserID.ToString() + @"Web method: GetAllDriverStatus() Page:\HOS\HOSData"));
            System.Diagnostics.StackTrace trace = new System.Diagnostics.StackTrace(ex, true);
            return null;
        }

        return dt;
    }

    private void exportDatatable(DataTable dt, string formatter, string columns, string fname)
    {
        try
        {
            if (formatter == "csv")
            {
                System.Text.StringBuilder sresult = new System.Text.StringBuilder();
                sresult.Append("sep=,");
                sresult.Append(Environment.NewLine);
                string header = string.Empty;
                foreach (string column in columns.Split(','))
                {
                    string s = column.Split(':')[0];
                    header += "\"" + s + "\",";
                }
                header = header.Substring(0, header.Length - 1);
                sresult.Append(header);
                sresult.Append(Environment.NewLine);

                foreach (DataRow row in dt.Rows)
                {
                    string data = string.Empty;
                    foreach (string column in columns.Split(','))
                    {
                        string s = row[column.Split(':')[1]].ToString();
                        if (column.Split(':')[1] == "LastUpdate")
                            s = Convert.ToDateTime(s).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                        data += "\"" + s.Replace("[br]", Environment.NewLine).Replace("\"", "\"\"") + "\",";
                    }
                    data = data.Substring(0, data.Length - 1);
                    sresult.Append(data);
                    sresult.Append(Environment.NewLine);
                }

                Response.Clear();
                Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.csv", fname));
                Response.Charset = System.Text.Encoding.GetEncoding("iso-8859-1").BodyName;
                Response.ContentType = "application/csv";
                Response.ContentEncoding = System.Text.Encoding.GetEncoding("iso-8859-1");
                Response.Write(sresult.ToString());
                Response.Flush();
            }
            else if (formatter == "excel2003")
            {
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
                foreach (string column in columns.Split(','))
                {
                    string s = column.Split(':')[0];
                    row.CreateCell(row.Cells.Count).SetCellValue(s);
                }
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string data = string.Empty;
                    IRow rowData = ws.CreateRow(i + 1);
                    foreach (string column in columns.Split(','))
                    {
                        if (column.Split(':')[1] == "LastUpdate")
                        {
                            DateTime currentDate = DateTime.Now.ToUniversalTime();
                            //DateTime recordDate;

                            string datadate = Convert.ToDateTime(dt.Rows[i][column.Split(':')[1]].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                            rowData.CreateCell(rowData.Cells.Count).SetCellValue(datadate.Replace("[br]", Environment.NewLine));
                            //recordDate = DateTime.ParseExact(datadate, sn.User.DateFormat + " " + sn.User.TimeFormat, System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();

                            //TimeSpan diffDate = currentDate.Subtract(recordDate);

                            //if (diffDate.TotalHours < 24)
                            //{
                            //    cellstyle1.WrapText = true;
                            //    cellstyle1.VerticalAlignment = VerticalAlignment.TOP;
                            //    rowData.Cells[rowData.Cells.Count - 1].CellStyle = cellstyle1;
                            //}
                            //else if (diffDate.TotalHours < 48)
                            //{
                            //    cellstyle2.WrapText = true;
                            //    cellstyle2.VerticalAlignment = VerticalAlignment.TOP;
                            //    rowData.Cells[rowData.Cells.Count - 1].CellStyle = cellstyle2;
                            //}
                            //else if (diffDate.TotalHours < 72)
                            //{
                            //    cellstyle3.WrapText = true;
                            //    cellstyle3.VerticalAlignment = VerticalAlignment.TOP;
                            //    rowData.Cells[rowData.Cells.Count - 1].CellStyle = cellstyle3;
                            //}
                            //else if (diffDate.TotalHours < 168)
                            //{
                            //    cellstyle4.WrapText = true;
                            //    cellstyle4.VerticalAlignment = VerticalAlignment.TOP;
                            //    rowData.Cells[rowData.Cells.Count - 1].CellStyle = cellstyle4;
                            //}
                            //else if (diffDate.TotalHours > 168)
                            //{
                            //    cellstyle5.WrapText = true;
                            //    cellstyle5.VerticalAlignment = VerticalAlignment.TOP;
                            //    rowData.Cells[rowData.Cells.Count - 1].CellStyle = cellstyle5;
                            //}

                        }
                        else
                            rowData.CreateCell(rowData.Cells.Count).SetCellValue(dt.Rows[i][column.Split(':')[1]].ToString().Replace("[br]", Environment.NewLine));

                    }
                }

                for (int i = 0; i < columns.Split(',').Length; i++)
                {
                    try
                    {
                        ws.AutoSizeColumn(i);
                    }
                    catch { }
                }

                HttpResponse httpResponse = Response;
                httpResponse.Clear();
                //httpResponse.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                //httpResponse.AddHeader("content-disposition", string.Format(@"attachment;filename={0}.xls", fname));

                Response.AddHeader("Content-Type", "application/Excel");
                Response.ContentType = "application/vnd.xls";
                HttpContext.Current.Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.xls", fname));

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    wb.Write(memoryStream);
                    memoryStream.WriteTo(httpResponse.OutputStream);
                    memoryStream.Close();
                }

                HttpContext.Current.Response.End();
            }
            else if (formatter == "excel2007")
            {
                try
                {
                    var wb = new XLWorkbook();
                    var ws = wb.Worksheets.Add("Sheet1");
                    foreach (string column in columns.Split(','))
                    {
                        string s = column.Split(':')[0];
                        ws.Cell(1, ws.Row(1).CellsUsed().Count() + 1).Value = s;
                    }

                    for (int i = 0; i < dt.Rows.Count; i++)
                    {
                        string data = string.Empty;
                        int iColumn = 1;
                        foreach (string column in columns.Split(','))
                        {
                            ws.Cell(i + 2, iColumn).DataType = XLCellValues.Text;

                            if (column.Split(':')[1] == "LastUpdate")
                            {
                                DateTime currentDate = DateTime.Now.ToUniversalTime();
                                //DateTime recordDate;


                                string datadate = Convert.ToDateTime(dt.Rows[i][column.Split(':')[1]].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                                ws.Cell(i + 2, iColumn).Value = "'" + datadate.Replace("[br]", Environment.NewLine);
                                //recordDate = DateTime.ParseExact(datadate, sn.User.DateFormat + " " + sn.User.TimeFormat, System.Globalization.CultureInfo.InvariantCulture).ToUniversalTime();

                                //TimeSpan diffDate = currentDate.Subtract(recordDate);

                                //if (diffDate.TotalHours < 24)
                                //{
                                //    ws.Cell(i + 2, iColumn).Style.Fill.BackgroundColor = XLColor.FromHtml("#7BB273");
                                //}
                                //else if (diffDate.TotalHours < 48)
                                //{
                                //    ws.Cell(i + 2, iColumn).Style.Fill.BackgroundColor = XLColor.FromHtml("#EFD700");
                                //}
                                //else if (diffDate.TotalHours < 72)
                                //{
                                //    ws.Cell(i + 2, iColumn).Style.Fill.BackgroundColor = XLColor.FromHtml("#FFA64A");
                                //}
                                //else if (diffDate.TotalHours < 168)
                                //{
                                //    ws.Cell(i + 2, iColumn).Style.Fill.BackgroundColor = XLColor.FromHtml("#DE7973");
                                //}
                                //else if (diffDate.TotalHours > 168)
                                //{
                                //    ws.Cell(i + 2, iColumn).Style.Fill.BackgroundColor = XLColor.FromHtml("#637DA5");
                                //}

                            }
                            else
                                ws.Cell(i + 2, iColumn).Value = "'" + dt.Rows[i][column.Split(':')[1]].ToString().Replace("[br]", Environment.NewLine);

                            iColumn++;
                        }
                    }

                    //ws.Rows().Style.Alignment.SetWrapText();
                    //ws.Rows().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                    //ws.Columns().AdjustToContents();

                    try
                    {
                        var files = new System.IO.DirectoryInfo(Server.MapPath("TempReports/")).GetFiles("*.xlsx");
                        foreach (var file in files)
                        {
                            if (DateTime.UtcNow - file.LastWriteTimeUtc > TimeSpan.FromDays(30))
                            {
                                System.IO.File.Delete(file.FullName);
                            }
                        }
                    }
                    catch { }

                    Response.Clear();
                    //Response.AddHeader("Content-Type", "application/Excel");
                    //Response.ContentType = "application/force-download";
                    //Response.AddHeader("content-disposition", string.Format(@"attachment;filename={0}.xlsx", fname));
                    HttpContext.Current.Response.Clear();
                    HttpContext.Current.Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                    HttpContext.Current.Response.AddHeader("content-disposition", string.Format(@"attachment;filename={0}.xlsx", fname));
                    
                    //string filemame = string.Format(@"{0}.xlsx", Guid.NewGuid());
                    //wb.SaveAs(Server.MapPath("TempReports/") + filemame);
                    //Response.TransmitFile("TempReports/" + filemame);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        wb.SaveAs(memoryStream);
                        memoryStream.WriteTo(HttpContext.Current.Response.OutputStream);
                        memoryStream.Close();
                    }

                    HttpContext.Current.Response.End();
                }
                //Peter Editted
                catch (Exception Ex)
                {
                    Response.Write("<script type='text/javascript'>alert('Failed to generate the file, please try it again or choose another Excel format to export.');</script>");
                    System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                }

            }
        }
        //Peter Editted
        catch (Exception Ex)
        {
            Response.Write("<script type='text/javascript'>alert('Failed to generate the file, please try it again or choose another Excel format to export.');</script>");
            System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
        }
    }
}