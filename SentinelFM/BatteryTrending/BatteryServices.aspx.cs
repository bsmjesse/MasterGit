using System;
using System.Data;
using System.Globalization;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;
using System.Web.UI;
using System.Xml;
using System.Collections.Generic;
using System.Linq;
using System.Configuration;
using System.Collections;
using ClosedXML.Excel;
using NPOI.HSSF.UserModel;
using NPOI.HPSF;
using NPOI.POIFS.FileSystem;
using NPOI.SS.UserModel;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace SentinelFM
{
    public partial class BatteryTrending_BatteryServices : System.Web.UI.Page
    {
        protected SentinelFMSession sn = null;
        private string sConnectionString;

        protected void Page_Load(object sender, EventArgs e)
        {
            try
            {
                sn = (SentinelFMSession)Session["SentinelFMSession"];

                sConnectionString = ConfigurationManager.ConnectionStrings["DWH"].ConnectionString;
                string request = Request.QueryString["QueryType"];
                if (string.IsNullOrEmpty(request))
                {
                    request = "GetLastKnownBatteryByFleet_NewTZ";                    
                }

                if (request.Equals("GetLastKnownBatteryByFleet_NewTZ", StringComparison.CurrentCultureIgnoreCase))
                {
                    GetLastKnownBatteryByFleet_NewTZ();
                }
                else if (request.Equals("GetBatteryTrendingByVehicleId_NewTZ", StringComparison.CurrentCultureIgnoreCase))
                {
                    GetBatteryTrendingByVehicleId_NewTZ();
                }
                else if (request.Equals("GetBatterySummaryByFleetId", StringComparison.CurrentCultureIgnoreCase))
                {
                    GetBatterySummaryByFleetId();
                }

            }
            catch (NullReferenceException Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.StackTrace.ToString()));

            }
            catch (Exception Ex)
            {
                System.Diagnostics.Trace.WriteLineIf(AppConfig.tsMain.TraceError, VLF.CLS.Util.TraceFormat(VLF.CLS.Def.Enums.TraceSeverity.Error, Ex.Message.ToString() + " " + Ex.StackTrace.ToString() + " User:" + sn.UserID.ToString() + " Form:" + Page.GetType().Name));
                Response.Write(Ex.Message.ToString() + " " + Ex.StackTrace.ToString());
            }
        }

        // Changes for TimeZone Feature start
        private void GetLastKnownBatteryByFleet_NewTZ()
        {
            int FleetId = 0;
            string VoltageThreshold = "all";
            int start = 0;
            int limit = 100;

            if (!string.IsNullOrEmpty(Request["fleetId"]))
            {
                int.TryParse(Request["fleetId"], out FleetId);
            }

            if (!string.IsNullOrEmpty(Request["t"]))
            {
                VoltageThreshold = Request["t"].Trim().ToLower();
            }

            if (!string.IsNullOrEmpty(Request["start"]))
            {
                int.TryParse(Request["start"], out start);
            }

            if (!string.IsNullOrEmpty(Request["limit"]))
            {
                int.TryParse(Request["limit"], out limit);
            }

            /*ArrayList filterarray = new ArrayList();
            for (int i = 0; i < 100; i++)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["filter[" + i.ToString() + "][field]"]))
                {
                    customFilter f = new customFilter();
                    f.field = Request.QueryString["filter[" + i.ToString() + "][field]"];
                    f.type = Request.QueryString["filter[" + i.ToString() + "][data][type]"];
                    f.value = Request.QueryString["filter[" + i.ToString() + "][data][value]"];
                    if (!string.IsNullOrEmpty(Request.QueryString["filter[" + i.ToString() + "][data][comparison]"]))
                    {
                        f.comparison = Request.QueryString["filter[" + i.ToString() + "][data][comparison]"];
                    }
                    filterarray.Add(f);
                }
                else
                {
                    break;
                }
            }*/

            VLF.DAS.Logic.BatteryTrending dbBatteryTrending = new VLF.DAS.Logic.BatteryTrending(sConnectionString);
            DataSet ds = dbBatteryTrending.GetLastKnownBatteryByFleet_NewTZ(FleetId, VoltageThreshold, sn.UserID);
            //VLF.PATCH.Logic.BatteryTrending dbBatteryTrending = new VLF.PATCH.Logic.BatteryTrending(sConnectionString);
            //DataSet ds = dbBatteryTrending.GetLastKnownBatteryByFleet(FleetId, VoltageThreshold, sn.UserID);

            dbBatteryTrending.Dispose();

            DataTable filteredTable = ds.Tables[0];

            List<customFilter> filterarray = new List<customFilter>();

            if (!string.IsNullOrEmpty(Request.QueryString["filter"]) && filteredTable.Rows.Count > 0)
            {
                filterarray = JsonConvert.DeserializeObject<List<customFilter>>(Request.QueryString["filter"]);
            }


            foreach (customFilter f in filterarray)
            {
                if (filteredTable.Rows.Count == 0)
                    break;

                if (f.type.Equals("int", StringComparison.CurrentCultureIgnoreCase) || f.type.Equals("numeric", StringComparison.CurrentCultureIgnoreCase))
                {
                    string op = "";
                    if (f.comparison.Equals("lt", StringComparison.CurrentCultureIgnoreCase))
                    {
                        op = " < ";
                    }
                    else if (f.comparison.Equals("gt", StringComparison.CurrentCultureIgnoreCase))
                    {
                        op = " > ";
                    }
                    else if (f.comparison.Equals("eq", StringComparison.CurrentCultureIgnoreCase))
                    {
                        op = " = ";
                    }

                    DataRow[] rows = filteredTable.Select(f.field + op + f.value);
                    filteredTable = rows.Any() ? rows.CopyToDataTable() : new DataTable();

                }
                else if (f.type.Equals("date", StringComparison.CurrentCultureIgnoreCase))
                {
                    if (f.comparison.Equals("before", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string col = f.field + " < #" + f.value + "#";
                        DataRow[] rows = filteredTable.Select(col);
                        filteredTable = rows.Any() ? rows.CopyToDataTable() : new DataTable();
                    }
                    else if (f.comparison.Equals("after", StringComparison.CurrentCultureIgnoreCase))
                    {

                        string col = f.field + " > #" + f.value + "#";
                        DataRow[] rows = filteredTable.Select(col);
                        filteredTable = rows.Any() ? rows.CopyToDataTable() : new DataTable();
                    }
                    else if (f.comparison.Equals("eq", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string col = f.field + " < #" + f.value + " 23:59:59" + "#";
                        col += " AND " + f.field + " > #" + f.value + " 00:00:00" + "#";
                        var rows = filteredTable.Select(col);
                        filteredTable = rows.Any() ? rows.CopyToDataTable() : new DataTable();
                    }

                }
                else
                {
                    var rows = filteredTable.Select(string.Format("{0} LIKE '%{1}%'", f.field, f.value));
                    filteredTable = rows.Any() ? rows.CopyToDataTable() : new DataTable();
                }
            }


            if (!string.IsNullOrEmpty(Request.QueryString["sort"]) && filteredTable.Rows.Count > 0)
            {
                List<Sorting> desSort = JsonConvert.DeserializeObject<List<Sorting>>(Request.QueryString["sort"]);

                if (desSort.Count > 0)
                {
                    string property = desSort[0].property;
                    string direction = desSort[0].direction;
                    filteredTable.DefaultView.Sort = property + " " + direction;
                    filteredTable = filteredTable.DefaultView.ToTable();
                }
            }

            /*string request = Request.QueryString["sorting"];
            if (!string.IsNullOrEmpty(request) && filteredTable.Rows.Count > 0)
            {
                filteredTable.DefaultView.Sort = request.Split(',')[0] + " " + request.Split(',')[1];
                filteredTable = filteredTable.DefaultView.ToTable();
            }*/

            string request = Request.QueryString["operation"];
            string operation = "";
            if (!string.IsNullOrEmpty(request))
            {
                operation = request;
            }

            request = Request.QueryString["formattype"];
            string formattype = "";
            if (!string.IsNullOrEmpty(request))
            {
                formattype = request;
            }

            if (operation == "Export" && !String.IsNullOrEmpty(formattype))
            {
                request = Request.QueryString["columns"];
                exportDatatable(filteredTable, formattype, request);
                return;
            }

            if (ds.Tables.CanRemove(ds.Tables["VehiclesBatteryInfo"]))
            {
                ds.Tables.Remove(ds.Tables["VehiclesBatteryInfo"]);
            }
            filteredTable.TableName = "VehiclesBatteryInfo";
            ds.Tables.Add(filteredTable);

            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.UTF8;

            if (ds.Tables["VehiclesBatteryInfo"].Rows.Count == 0)
            {
                Response.Write("<" + ds.DataSetName + "><totalCount>0</totalCount>");
                Response.Write("</" + ds.DataSetName + ">");
            }
            else
            {

                DataSet dstemp = new DataSet();
                DataView dv = ds.Tables["VehiclesBatteryInfo"].DefaultView;
                //dv.Sort = "OriginDateTime DESC";
                DataTable sortedTable = dv.ToTable();
                DataTable dttemp = sortedTable.AsEnumerable().Skip(start).Take(limit).CopyToDataTable();
                dttemp.TableName = "VehiclesBatteryInfo";
                dstemp.Tables.Add(dttemp);
                dstemp.DataSetName = "Fleet";

                getXmlFromDs(dstemp, ds.Tables["VehiclesBatteryInfo"].Rows.Count);
            }
        }


        // Changes for TimeZone Feature end

        private void GetLastKnownBatteryByFleet()
        {
            int FleetId = 0;
            string VoltageThreshold = "all";
            int start = 0;
            int limit = 100;

            if (!string.IsNullOrEmpty(Request["fleetId"]))
            {
                int.TryParse(Request["fleetId"], out FleetId);
            }

            if (!string.IsNullOrEmpty(Request["t"]))            
            {
                VoltageThreshold = Request["t"].Trim().ToLower();
            }

            if (!string.IsNullOrEmpty(Request["start"]))
            {
                int.TryParse(Request["start"], out start);
            }

            if (!string.IsNullOrEmpty(Request["limit"]))
            {
                int.TryParse(Request["limit"], out limit);
            }

            /*ArrayList filterarray = new ArrayList();
            for (int i = 0; i < 100; i++)
            {
                if (!string.IsNullOrEmpty(Request.QueryString["filter[" + i.ToString() + "][field]"]))
                {
                    customFilter f = new customFilter();
                    f.field = Request.QueryString["filter[" + i.ToString() + "][field]"];
                    f.type = Request.QueryString["filter[" + i.ToString() + "][data][type]"];
                    f.value = Request.QueryString["filter[" + i.ToString() + "][data][value]"];
                    if (!string.IsNullOrEmpty(Request.QueryString["filter[" + i.ToString() + "][data][comparison]"]))
                    {
                        f.comparison = Request.QueryString["filter[" + i.ToString() + "][data][comparison]"];
                    }
                    filterarray.Add(f);
                }
                else
                {
                    break;
                }
            }*/

            VLF.DAS.Logic.BatteryTrending dbBatteryTrending = new VLF.DAS.Logic.BatteryTrending(sConnectionString);
            DataSet ds = dbBatteryTrending.GetLastKnownBatteryByFleet(FleetId, VoltageThreshold, sn.UserID);
            //VLF.PATCH.Logic.BatteryTrending dbBatteryTrending = new VLF.PATCH.Logic.BatteryTrending(sConnectionString);
            //DataSet ds = dbBatteryTrending.GetLastKnownBatteryByFleet(FleetId, VoltageThreshold, sn.UserID);

            dbBatteryTrending.Dispose();

            DataTable filteredTable = ds.Tables[0];

            List<customFilter> filterarray = new List<customFilter>();

            if (!string.IsNullOrEmpty(Request.QueryString["filter"]) && filteredTable.Rows.Count > 0)
            {
                filterarray = JsonConvert.DeserializeObject<List<customFilter>>(Request.QueryString["filter"]);                
            }

            
            foreach (customFilter f in filterarray)
            {
                if (filteredTable.Rows.Count == 0)
                    break;

                if (f.type.Equals("int", StringComparison.CurrentCultureIgnoreCase) || f.type.Equals("numeric", StringComparison.CurrentCultureIgnoreCase))
                {
                    string op = "";
                    if (f.comparison.Equals("lt", StringComparison.CurrentCultureIgnoreCase))
                    {
                        op = " < ";
                    }
                    else if (f.comparison.Equals("gt", StringComparison.CurrentCultureIgnoreCase))
                    {
                        op = " > ";                            
                    }
                    else if (f.comparison.Equals("eq", StringComparison.CurrentCultureIgnoreCase))
                    {
                        op = " = ";                            
                    }

                    DataRow[] rows = filteredTable.Select(f.field + op + f.value);
                    filteredTable = rows.Any() ? rows.CopyToDataTable() : new DataTable();

                }
                else if (f.type.Equals("date",StringComparison.CurrentCultureIgnoreCase))
                {
                    if (f.comparison.Equals("before", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string col = f.field + " < #" + f.value + "#";
                        DataRow[] rows = filteredTable.Select(col);
                        filteredTable = rows.Any() ? rows.CopyToDataTable() : new DataTable();
                    }
                    else if (f.comparison.Equals("after", StringComparison.CurrentCultureIgnoreCase))
                    {

                        string col = f.field + " > #" + f.value + "#";
                        DataRow[] rows = filteredTable.Select(col);
                        filteredTable = rows.Any() ? rows.CopyToDataTable() : new DataTable();
                    }
                    else if (f.comparison.Equals("eq", StringComparison.CurrentCultureIgnoreCase))
                    {
                        string col = f.field + " < #" + f.value + " 23:59:59" + "#";
                        col += " AND " + f.field + " > #" + f.value + " 00:00:00" + "#";
                        var rows = filteredTable.Select(col);                            
                        filteredTable = rows.Any() ? rows.CopyToDataTable() : new DataTable();
                    }

                }
                else
                {
                    var rows = filteredTable.Select(string.Format("{0} LIKE '%{1}%'", f.field, f.value));
                    filteredTable = rows.Any() ? rows.CopyToDataTable() : new DataTable();
                }
            }                
            

            if (!string.IsNullOrEmpty(Request.QueryString["sort"]) && filteredTable.Rows.Count > 0)
            {
                List<Sorting> desSort = JsonConvert.DeserializeObject<List<Sorting>>(Request.QueryString["sort"]);
                
                if(desSort.Count>0)
                {
                    string property = desSort[0].property;
                    string direction = desSort[0].direction;
                    filteredTable.DefaultView.Sort = property + " " + direction;
                    filteredTable = filteredTable.DefaultView.ToTable();
                }
            }

            /*string request = Request.QueryString["sorting"];
            if (!string.IsNullOrEmpty(request) && filteredTable.Rows.Count > 0)
            {
                filteredTable.DefaultView.Sort = request.Split(',')[0] + " " + request.Split(',')[1];
                filteredTable = filteredTable.DefaultView.ToTable();
            }*/

            string request = Request.QueryString["operation"];
            string operation = "";
            if (!string.IsNullOrEmpty(request))
            {
                operation = request;
            }
            
            request = Request.QueryString["formattype"];
            string formattype = "";
            if (!string.IsNullOrEmpty(request))
            {
                formattype = request;
            }
            
            if (operation == "Export" && !String.IsNullOrEmpty(formattype))
            {
                request = Request.QueryString["columns"];
                exportDatatable(filteredTable, formattype, request);
                return;
            }

            if (ds.Tables.CanRemove(ds.Tables["VehiclesBatteryInfo"]))
            {
                ds.Tables.Remove(ds.Tables["VehiclesBatteryInfo"]);
            }
            filteredTable.TableName = "VehiclesBatteryInfo";
            ds.Tables.Add(filteredTable);

            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.UTF8;

            if (ds.Tables["VehiclesBatteryInfo"].Rows.Count == 0)
            {
                Response.Write("<" + ds.DataSetName + "><totalCount>0</totalCount>");
                Response.Write("</" + ds.DataSetName + ">");
            }
            else
            {

                DataSet dstemp = new DataSet();
                DataView dv = ds.Tables["VehiclesBatteryInfo"].DefaultView;
                //dv.Sort = "OriginDateTime DESC";
                DataTable sortedTable = dv.ToTable();
                DataTable dttemp = sortedTable.AsEnumerable().Skip(start).Take(limit).CopyToDataTable();
                dttemp.TableName = "VehiclesBatteryInfo";
                dstemp.Tables.Add(dttemp);
                dstemp.DataSetName = "Fleet";

                getXmlFromDs(dstemp, ds.Tables["VehiclesBatteryInfo"].Rows.Count);
            }
        }

        // Changes for TimeZone Feature start

        private void GetBatteryTrendingByVehicleId_NewTZ()
        {
            int FleetId = 0;
            int vehicleId = 0;
            int start = 0;
            int limit = 100;

            if (!string.IsNullOrEmpty(Request["fleetId"]))
            {
                int.TryParse(Request["fleetId"], out FleetId);
            }

            if (!string.IsNullOrEmpty(Request["vehicleId"]))
            {
                int.TryParse(Request["vehicleId"], out vehicleId);
            }

            if (!string.IsNullOrEmpty(Request["start"]))
            {
                int.TryParse(Request["start"], out start);
            }

            if (!string.IsNullOrEmpty(Request["limit"]))
            {
                int.TryParse(Request["limit"], out limit);
            }

            string strFromDate = "";
            string strToDate = "";

            strFromDate = Request["dateTimeFrom"].ToString();
            strToDate = Request["dateTimeTo"].ToString();

            DateTime dtsd = DateTime.ParseExact(strFromDate, sn.User.DateFormat, CultureInfo.InvariantCulture);
            DateTime dttd = DateTime.ParseExact(strToDate, sn.User.DateFormat, CultureInfo.InvariantCulture);
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");

            strFromDate = Convert.ToDateTime(dtsd, ci).ToString();
            strToDate = Convert.ToDateTime(dttd, ci).ToString();

            //VLF.PATCH.Logic.BatteryTrending dbBatteryTrending = new VLF.PATCH.Logic.BatteryTrending(sConnectionString);
            VLF.DAS.Logic.BatteryTrending dbBatteryTrending = new VLF.DAS.Logic.BatteryTrending(sConnectionString);
            DataSet ds = dbBatteryTrending.GetBatteryTrendingByVehicleId_NewTZ(vehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("yyyy-MM-dd"), Convert.ToDateTime(strToDate).AddHours(-sn.User.NewFloatTimeZone - sn.User.DayLightSaving).ToString("yyyy-MM-dd"), sn.UserID);
            dbBatteryTrending.Dispose();

            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.UTF8;

            getXmlFromDs(ds, ds.Tables["BatteryTrendingInfo"].Rows.Count);
        }

        // Changes for TimeZone Feature end

        private void GetBatteryTrendingByVehicleId()
        {
            int FleetId = 0;
            int vehicleId = 0;
            int start = 0;
            int limit = 100;

            if (!string.IsNullOrEmpty(Request["fleetId"]))
            {
                int.TryParse(Request["fleetId"], out FleetId);
            }

            if (!string.IsNullOrEmpty(Request["vehicleId"]))
            {
                int.TryParse(Request["vehicleId"], out vehicleId);
            }

            if (!string.IsNullOrEmpty(Request["start"]))
            {
                int.TryParse(Request["start"], out start);
            }

            if (!string.IsNullOrEmpty(Request["limit"]))
            {
                int.TryParse(Request["limit"], out limit);
            }

            string strFromDate = "";
            string strToDate = "";

            strFromDate = Request["dateTimeFrom"].ToString();
            strToDate = Request["dateTimeTo"].ToString();

            DateTime dtsd = DateTime.ParseExact(strFromDate, sn.User.DateFormat, CultureInfo.InvariantCulture);
            DateTime dttd = DateTime.ParseExact(strToDate, sn.User.DateFormat, CultureInfo.InvariantCulture);
            System.Globalization.CultureInfo ci = new System.Globalization.CultureInfo("en-US");

            strFromDate = Convert.ToDateTime(dtsd, ci).ToString();
            strToDate = Convert.ToDateTime(dttd, ci).ToString();

            //VLF.PATCH.Logic.BatteryTrending dbBatteryTrending = new VLF.PATCH.Logic.BatteryTrending(sConnectionString);
            VLF.DAS.Logic.BatteryTrending dbBatteryTrending = new VLF.DAS.Logic.BatteryTrending(sConnectionString);
            DataSet ds = dbBatteryTrending.GetBatteryTrendingByVehicleId(vehicleId, Convert.ToDateTime(strFromDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("yyyy-MM-dd"), Convert.ToDateTime(strToDate).AddHours(-sn.User.TimeZone - sn.User.DayLightSaving).ToString("yyyy-MM-dd"), sn.UserID);
            dbBatteryTrending.Dispose();
            
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.UTF8;

            getXmlFromDs(ds, ds.Tables["BatteryTrendingInfo"].Rows.Count);
        }

        private void GetBatterySummaryByFleetId()
        {
            //sConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
            int FleetId = 0;
            
            if (!string.IsNullOrEmpty(Request["fleetId"]))
            {
                int.TryParse(Request["fleetId"], out FleetId);
            }

            //VLF.PATCH.Logic.BatteryTrending dbBatteryTrending = new VLF.PATCH.Logic.BatteryTrending(sConnectionString);
            VLF.DAS.Logic.BatteryTrending dbBatteryTrending = new VLF.DAS.Logic.BatteryTrending(sConnectionString);

            DataSet ds = dbBatteryTrending.GetBatterySummaryByFleetId(FleetId);
            dbBatteryTrending.Dispose();
            
            Response.ContentType = "text/xml";
            Response.ContentEncoding = Encoding.UTF8;

            getXmlFromDs(ds, ds.Tables["BatterySummaryInfo"].Rows.Count);
        }

        private void getXmlFromDs(DataSet ds, int c)
        {
            Response.Write("<" + ds.DataSetName + "><totalCount>" + c.ToString() + "</totalCount>");
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                Response.Write("<" + ds.Tables[0].TableName + ">");
                foreach (DataColumn column in ds.Tables[0].Columns)
                {
                    Response.Write("<" + column.ColumnName + ">");
                    string v = "";
                    if (column.DataType.Name == "DateTime")
                        v = String.Format("{0:yyyy-MM-ddTHH:mm:ss.ff}", dr[column]);
                    else
                    {
                        v = dr[column].ToString();                        
                        v = v.Replace("&#x0", "").Replace("&", "&amp;").Replace("\0", string.Empty);
                    }
                    //byte[] data = Encoding.Default.GetBytes(v);
                    //v = Encoding.UTF8.GetString(data);
                    Response.Write(v);
                    Response.Write("</" + column.ColumnName + ">");
                }
                Response.Write("</" + ds.Tables[0].TableName + ">");
            }
            Response.Write("</" + ds.DataSetName + ">");
        }

        private void exportDatatable(DataTable dt, string formatter, string columns)
        {
            try
            {

                if (formatter == "csv")
                {

                    StringBuilder sresult = new StringBuilder();
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
                            if (column.Split(':')[1] == "Datetime")
                                s = Convert.ToDateTime(s).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                            data += "\"" + s.Replace("[br]", Environment.NewLine).Replace("\"", "\"\"") + "\",";                            
                        }
                        data = data.Substring(0, data.Length - 1);
                        sresult.Append(data);
                        sresult.Append(Environment.NewLine);
                    }

                    Response.Clear();
                    Response.AddHeader("content-disposition", string.Format("attachment;filename={0}.csv", "vehicles"));
                    Response.Charset = Encoding.GetEncoding("iso-8859-1").BodyName;
                    Response.ContentType = "application/csv";
                    Response.ContentEncoding = Encoding.GetEncoding("iso-8859-1");
                    Response.Write(sresult.ToString());
                    Response.Flush();
                    //Response.Close();
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
                            if (column.Split(':')[1] == "Datetime")
                            {
                                string datadate = Convert.ToDateTime(dt.Rows[i][column.Split(':')[1]].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                                rowData.CreateCell(rowData.Cells.Count).SetCellValue(datadate.Replace("[br]", Environment.NewLine));
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
                    httpResponse.AddHeader("Content-Type", "application/Excel");
                    httpResponse.ContentType = "application/vnd.xls";

                    httpResponse.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.xls", "Vehicle"));
                    // Flush the workbook to the Response.OutputStream
                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        wb.Write(memoryStream);
                        memoryStream.WriteTo(httpResponse.OutputStream);
                        memoryStream.Close();
                    }

                    //HttpContext.Current.Response.End();
                }
                else if (formatter == "excel2007")
                {
                    try
                    {
                        var wb = new XLWorkbook();
                        var ws = wb.Worksheets.Add("Sheet1");

                        for (int i = 0; i < columns.Split(',').Length; i++)
                        {
                            string s = columns.Split(',')[i].Split(':')[0];
                            ws.Cell(1, i + 1).Value = s;
                        }

                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            string data = string.Empty;
                            int iColumn = 1;
                            foreach (string column in columns.Split(','))
                            {
                                ws.Cell(i + 2, iColumn).DataType = XLCellValues.Text;

                                if (column.Split(':')[1] == "Datetime")
                                {
                                    string datadate = Convert.ToDateTime(dt.Rows[i][column.Split(':')[1]].ToString()).ToString(sn.User.DateFormat + " " + sn.User.TimeFormat);
                                    ws.Cell(i + 2, iColumn).Value = "'" + datadate.Replace("[br]", Environment.NewLine);
                                }
                                else
                                    ws.Cell(i + 2, iColumn).Value = "'" + dt.Rows[i][column.Split(':')[1]].ToString().Replace("[br]", Environment.NewLine);

                                iColumn++;
                            }
                        }
                        //Peter Editted
                        //ws.Rows().Style.Alignment.SetWrapText();
                        //ws.Rows().Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
                        //ws.Columns().AdjustToContents();

                        //Peter Editted
                        try
                        {
                            var files = new DirectoryInfo(Server.MapPath("../TempReports/")).GetFiles("*.xlsx");
                            foreach (var file in files)
                            {
                                if (DateTime.UtcNow - file.LastWriteTimeUtc > TimeSpan.FromDays(30))
                                {
                                    File.Delete(file.FullName);
                                }
                            }
                        }
                        catch { }

                        Response.Clear();
                        Response.AddHeader("Content-Type", "application/Excel");
                        Response.ContentType = "application/force-download";
                        Response.AddHeader("content-disposition", String.Format(@"attachment;filename={0}.xlsx", "vehicles"));

                        string filemame = string.Format(@"{0}.xlsx", Guid.NewGuid());
                        wb.SaveAs(Server.MapPath("../TempReports/") + filemame);
                        Response.TransmitFile("../TempReports/" + filemame);

                        //Response.Flush();
                        //HttpContext.Current.Response.End();

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

    class customFilter
    {
        public string field {get; set;}
        public string type { get; set; }
        public string value { get; set; }
        public string comparison { get; set; }
    }

    class Sorting
    {
        public string property { get; set; }
        public string direction { get; set; }        
    }
}