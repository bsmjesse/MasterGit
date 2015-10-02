#define DEV_OFF
using System;
using System.Text;
using System.Reflection;
using System.Diagnostics;
using System.IO;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using System.Web;
using System.Xml;

/// <summary>
/// Summary description for DBAcess
/// </summary>
public class DBAccess
{

    static DBAccess _Instance;
    const int peripheralMessageTypeIdDest = 257; //stop data
    const int peripheralMessageTypeIdTxt = 42; //txt msg
    static object _SYNC = new object();

    const int _DefaultRadius = 100;

    public static DBAccess Get
    {
        get
        {
            if (_Instance == null)
            {
                lock (_SYNC)
                {
                    _Instance = new DBAccess();
                }
            }
            return _Instance;
        }
    }

    string _ConnectionString;


    public delegate void ExceptionHandler(Exception exception);
    public static event ExceptionHandler ExceptionEvent;

    private DBAccess()
    {
        //_ConnectionString = ConfigurationManager.ConnectionStrings["SentinelFMConnection"].ConnectionString;
        _ConnectionString = ConfigurationManager.ConnectionStrings["DBConnectionString"].ConnectionString;


    }


    byte[] Iso8859(string src)
    {
        Encoding iso = Encoding.GetEncoding("iso8859-1");
        return iso.GetBytes(src);
    }

    public long Send(int userId, string unitName, string properties)
    {
        long messageID = -1;
        try
        {
#if DEV
            string sql = "SendPacket2Garmin2Test";
#else
            string sql = "SendPacket2Garmin2";
#endif
            using (SqlConnection scon = new SqlConnection(_ConnectionString))
            {
                using (SqlCommand scom = new SqlCommand(sql, scon))
                {
                    scom.CommandType = CommandType.StoredProcedure;
                    scom.Parameters.Clear();
                    scom.Parameters.AddWithValue("@UnitName", unitName);
                    scom.Parameters.AddWithValue("@GarminMessageTypeId", (properties.Contains("LAT=;") || properties.Contains("LON=;")) ? peripheralMessageTypeIdTxt : peripheralMessageTypeIdDest);
                    scom.Parameters.AddWithValue("@Properties", properties);
                    scom.Parameters.AddWithValue("@userId", userId);
                    if (scom.Connection.State == ConnectionState.Closed) { scom.Connection.Open(); }
                    try
                    {

                        messageID = (long)scom.ExecuteScalar();
                    }
                    catch (Exception exc)
                    {
                        if (ExceptionEvent != null)
                            ExceptionEvent(exc);
                    }
                    finally
                    {
                        if (scom.Connection.State == ConnectionState.Open) { scom.Connection.Close(); }
                    }

                }
            }
        }
        catch (Exception exc)
        {
            if (ExceptionEvent != null)
                ExceptionEvent(exc);
        }
        return messageID;
    }


    public long Send(string token, string xml)
    {
        long messageID = -1;
        try
        {

            DataSet ds = new DataSet();
            using (MemoryStream ms = new MemoryStream(Iso8859(xml)))
            {
                try { ds.ReadXml(ms, XmlReadMode.InferSchema); }
                catch { ms.Position = 0; ds.ReadXml(ms, XmlReadMode.ReadSchema); }
            }
            List<DestinationMessage> tickets = new List<DestinationMessage>();
            DataTable dtTickets = ds.Tables["Ticket"];
            PropertyInfo[] props = typeof(DestinationMessage).GetProperties();
            foreach (DataRow row in dtTickets.Rows)
            {
                DestinationMessage ticket = new DestinationMessage();
                foreach (PropertyInfo pi in props)
                {
                    if (row.Table.Columns.Contains(pi.Name))
                    {
                        object value = row[pi.Name].ToString();
                        if (pi.PropertyType.Name == "Double")
                        {
                            double dvalue = 0.0f;
                            double.TryParse((string)value, out dvalue);
                            value = dvalue;
                        }
                        pi.SetValue(ticket, value, null);
                    }
                }
                if (string.IsNullOrEmpty(ticket.Token))
                {
                    ticket.Token = token;
                }
                tickets.Add(ticket);
            }
            foreach (DestinationMessage ticket in tickets)
            {
                int userId = 0;
                int orgId = 0;
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0}={1};", "TXT", string.Format("{0}: {1}", ticket.TicketID, ticket.CustomerName).Replace("=", ":").Replace(";", ","));
                sb.AppendFormat("{0}={1};", "RETRYINTERVAL", 5);
                sb.AppendFormat("{0}={1};", "LIFETIME", 300);
                sb.AppendFormat("{0}={1};", "LAT", ticket.DeliveryLatitude);
                sb.AppendFormat("{0}={1};", "LON", ticket.DeliveryLongitude);
                try
                {
                    userId = SessionHandler.Get.User(ticket.Token);
                    orgId = SessionHandler.Get.Organization(ticket.Token);
#if DEV
                    string sql = "SendPacket2Garmin2Test";
#else
                    string sql = "SendPacket2Garmin2";
#endif
                    using (SqlConnection scon = new SqlConnection(_ConnectionString))
                    {
                        using (SqlCommand scom = new SqlCommand(sql, scon))
                        {
                            scom.CommandType = CommandType.StoredProcedure;
                            scom.Parameters.Clear();
                            scom.Parameters.AddWithValue("@UnitName", ticket.TruckNumber.ToString());
                            //scom.Parameters.AddWithValue("@GarminMessageTypeId", (properties.Contains("LAT=;") || properties.Contains("LON=;")) ? peripheralMessageTypeIdTxt : peripheralMessageTypeIdDest);
                            scom.Parameters.AddWithValue("@GarminMessageTypeId", peripheralMessageTypeIdDest);
                            scom.Parameters.AddWithValue("@Properties", sb.ToString());
                            scom.Parameters.AddWithValue("@userId", userId);
                            if (scom.Connection.State == ConnectionState.Closed) { scom.Connection.Open(); }
                            try
                            {
                                object o = scom.ExecuteScalar();
                                long mid = 0;
                                if (long.TryParse(o.ToString(), out mid))
                                {
                                    messageID = mid;
                                }
                            }
                            catch (Exception exc)
                            {
                                if (ExceptionEvent != null)
                                    ExceptionEvent(exc);
                            }
                            finally
                            {
                                if (scom.Connection.State == ConnectionState.Open) { scom.Connection.Close(); }
                            }
                        }
                    }
                }
                catch (Exception exc)
                {
                    if (ExceptionEvent != null)
                        ExceptionEvent(exc);
                }

                if (messageID > -1)
                {

                    //Create virtual landmarks....
                    using (SqlConnection scon = new SqlConnection(_ConnectionString))
                    {
                        if (scon.State != ConnectionState.Open)
                        {
                            scon.Open();
                        }

                        DataTable dt = new DataTable();
                        //check first...
                        using (SqlCommand scom = new SqlCommand("vlfLandmarkVirtual_Get", scon))
                        {
                            scom.CommandType = CommandType.StoredProcedure;
                            scom.Parameters.Clear();
                            scom.Parameters.AddWithValue("@OrganizationId", orgId);
                            scom.Parameters.AddWithValue("@Latitude", ticket.DeliveryLatitude);
                            scom.Parameters.AddWithValue("@Longitude", ticket.DeliveryLongitude);

                            using (SqlDataAdapter sda = new SqlDataAdapter(scom))
                            {
                                int rows = sda.Fill(dt);
                            }
                        }
                        if (dt.Rows.Count > 0)
                        {
                            //update existing new virtual landmark
                            long landmarkId = 0;
                            long.TryParse(dt.Rows[0]["LandmarkId"].ToString(), out landmarkId);
                            using (SqlCommand scom = new SqlCommand("vlfLandmarkVirtualPointSet_Activate", scon))
                            {
                                scom.CommandType = CommandType.StoredProcedure;
                                scom.Parameters.Clear();
                                scom.Parameters.AddWithValue("@LandmarkId", landmarkId);
                                int rows = scom.ExecuteNonQuery();
                                if (rows == 0)
                                {
                                    if (ExceptionEvent != null)
                                        ExceptionEvent(new Exception(string.Format("Could not activate landmark:[{0}]", landmarkId)));
                                }
                            }
                        }
                        else
                        {
                            //insert new virtual landmark
                            using (SqlCommand scom = new SqlCommand("vlfLandmarkVirtualPointSet_Add", scon))
                            {
                                scom.CommandType = CommandType.StoredProcedure;
                                scom.Parameters.Clear();

                                scom.Parameters.AddWithValue("@OrganizationId", orgId);
                                scom.Parameters.AddWithValue("@LandmarkName", ticket.CustomerName);
                                scom.Parameters.AddWithValue("@Description", "JOB");
                                scom.Parameters.AddWithValue("@Latitude", ticket.DeliveryLatitude);
                                scom.Parameters.AddWithValue("@Longitude", ticket.DeliveryLongitude);
                                scom.Parameters.AddWithValue("@Radius", _DefaultRadius);
                                scom.Parameters.AddWithValue("@StreetAddress", string.Empty);
                                scom.Parameters.AddWithValue("@PointSets", string.Empty);
                                scom.Parameters.AddWithValue("@IsPersistent", false);

                                int rows = scom.ExecuteNonQuery();
                                if (rows == 0)
                                {
                                    if (ExceptionEvent != null)
                                        ExceptionEvent(new Exception(string.Format("Could not insert virtual landmark:[{0}]", ticket.CustomerName)));
                                }
                            }
                        }
                        if (scon.State != ConnectionState.Closed)
                        {
                            scon.Close();
                        }
                    }
                }
            }
        }
        catch (Exception exc)
        {
            if (ExceptionEvent != null)
                ExceptionEvent(exc);
        }
        return messageID;
    }


    public long Send(string xml)
    {
        long messageID = -1;
        try
        {
            DataSet ds = new DataSet();
            //ISO-8859-1
            //            byte[] payload = System.Text.Encoding.UTF8.GetBytes(Iso8859_unicode(xml));

            using (MemoryStream ms = new MemoryStream(Iso8859(xml)))
            {
                try
                {
                    ds.ReadXmlSchema(ms);
                    ds.ReadXml(ms);
                    if (ds.Tables.Count == 0) throw new Exception("EMPTY");
                }
                catch (Exception exc)
                {
                    bool stop = true;
                }


                try
                {
                    ms.Position = 0;
                    ds.ReadXml(ms, XmlReadMode.IgnoreSchema);
                    if (ds.Tables.Count == 0) throw new Exception("EMPTY");
                }
                catch (Exception exc)
                {
                    bool stop = true;
                }

                try
                {
                    ms.Position = 0;
                    ds.ReadXml(ms, XmlReadMode.InferSchema);
                    if (ds.Tables.Count == 0) throw new Exception("EMPTY");
                }
                catch (Exception exc)
                {
                    bool stop = true;
                }

                try
                {
                    ms.Position = 0;
                    ds.ReadXml(ms, XmlReadMode.InferTypedSchema);
                    if (ds.Tables.Count == 0) throw new Exception("EMPTY");
                }
                catch (Exception exc)
                {
                    bool stop = true;
                }

                try
                {
                    ms.Position = 0;
                    ds.ReadXml(ms, XmlReadMode.ReadSchema);
                    if (ds.Tables.Count == 0) throw new Exception("EMPTY");
                }
                catch (Exception exc)
                {
                    bool stop = true;
                }

                try
                {
                    ms.Position = 0;
                    ds.ReadXml(ms, XmlReadMode.Fragment);
                    if (ds.Tables.Count == 0) throw new Exception("EMPTY");
                }
                catch (Exception exc)
                {
                    bool stop = true;
                }
                try
                {
                    ms.Position = 0;
                    ds.ReadXml(ms, XmlReadMode.DiffGram);
                    if (ds.Tables.Count == 0) throw new Exception("EMPTY");
                }
                catch (Exception exc)
                {
                    bool stop = true;
                }

                try
                {
                    ms.Position = 0;
                    ds.ReadXml(ms, XmlReadMode.Auto);
                    if (ds.Tables.Count == 0) throw new Exception("EMPTY");
                }
                catch (Exception exc)
                {
                    bool stop = true;
                }
            }


            XmlDataDocument xdoc = new XmlDataDocument();
            xdoc.LoadXml(xml);

            PropertyInfo[] props = typeof(DestinationMessage).GetProperties();
            List<DestinationMessage> tickets = new List<DestinationMessage>();

            foreach (object xo in xdoc.ChildNodes)
            {
                if (xo.GetType().BaseType == typeof(XmlElement))
                {
                    XmlElement xe = (XmlElement)xo;
                    if (xe.Name == "Tickets") //datatable marker
                    {
                        Trace.TraceInformation("found `Tickets` datatabe");

                        foreach (object xot in xe.ChildNodes)
                        {
                            if (xot.GetType().BaseType == typeof(XmlElement))
                            {
                                XmlElement xer = (XmlElement)xot;
                                if (xer.Name == "Ticket") //data record marker
                                {
                                    DestinationMessage ticket = new DestinationMessage();

                                    Trace.TraceInformation("found `Ticket` record");
                                    foreach (object xor1 in xer.ChildNodes)
                                    {
                                        if (xor1.GetType().BaseType == typeof(XmlElement))
                                        {
                                            XmlElement xer1 = (XmlElement)xor1;
                                            PropertyInfo pi = typeof(DestinationMessage).GetProperty(xer1.Name);
                                            if (pi != null)
                                            {

                                                if (pi.PropertyType.Name == "Double")
                                                {
                                                    double value = 0;
                                                    double.TryParse(xer1.InnerText, out value);
                                                    pi.SetValue(ticket, value, null);
                                                }
                                                else
                                                {
                                                    pi.SetValue(ticket, xer1.InnerText, null);
                                                }

                                            }

                                        }
                                    }
                                    tickets.Add(ticket);
                                }
                            }
                        }
                    }
                }
            }


            foreach (DestinationMessage ticket in tickets)
            {
                StringBuilder sb = new StringBuilder();
                sb.AppendFormat("{0}={1};", "TXT", string.Format("{0}: {1}", ticket.TicketID, ticket.CustomerName));
                sb.AppendFormat("{0}={1};", "RETRYINTERVAL", 5);
                sb.AppendFormat("{0}={1};", "LIFETIME", 300);
                sb.AppendFormat("{0}={1};", "LAT", ticket.DeliveryLatitude);
                sb.AppendFormat("{0}={1};", "LON", ticket.DeliveryLongitude);


                string sql = "SendPacket2Garmin2Test";
                using (SqlConnection scon = new SqlConnection(_ConnectionString))
                {
                    using (SqlCommand scom = new SqlCommand(sql, scon))
                    {
                        scom.CommandType = CommandType.StoredProcedure;
                        scom.Parameters.Clear();
                        scom.Parameters.AddWithValue("@UnitName", ticket.TruckNumber);
                        //scom.Parameters.AddWithValue("@GarminMessageTypeId", (properties.Contains("LAT=;") || properties.Contains("LON=;")) ? peripheralMessageTypeIdTxt : peripheralMessageTypeIdDest);
                        scom.Parameters.AddWithValue("@GarminMessageTypeId", peripheralMessageTypeIdDest);
                        scom.Parameters.AddWithValue("@Properties", sb.ToString());
                        //scom.Parameters.AddWithValue("@userId", userId);
                        if (scom.Connection.State == ConnectionState.Closed) { scom.Connection.Open(); }
                        try
                        {

                            messageID = (long)scom.ExecuteScalar();
                        }
                        catch (Exception exc)
                        {
                            if (ExceptionEvent != null)
                                ExceptionEvent(exc);
                        }
                        finally
                        {
                            if (scom.Connection.State == ConnectionState.Open) { scom.Connection.Close(); }
                        }

                    }
                }
            }
        }
        catch (Exception exc)
        {
            if (ExceptionEvent != null)
                ExceptionEvent(exc);
        }
        return messageID;
    }



    int GetTargetPeripheralID(string unitName)
    {
        int unitID = 0;
        try
        {
            string sql = "SELECT vlfPeripheralBoxAssigment.PeripheralId FROM vlfPeripheralBoxAssigment RIGHT OUTER JOIN vlfVehicleAssignment ON vlfPeripheralBoxAssigment.BoxId = vlfVehicleAssignment.BoxId RIGHT OUTER JOIN vlfVehicleInfo ON vlfVehicleAssignment.VehicleId = vlfVehicleInfo.VehicleId WHERE vlfVehicleInfo.Description = @UnitName AND PeripheralId is not null";
            using (SqlConnection scon = new SqlConnection(_ConnectionString))
            {
                using (SqlCommand scom = new SqlCommand(sql, scon))
                {
                    scom.CommandType = CommandType.Text;
                    scom.Parameters.Clear();
                    scom.Parameters.AddWithValue("@UnitName", unitName);
                    if (scom.Connection.State == ConnectionState.Closed) { scom.Connection.Open(); }
                    try
                    {
                        unitID = (int)scom.ExecuteScalar();
                    }
                    catch (Exception exc)
                    {
                        if (ExceptionEvent != null)
                            ExceptionEvent(exc);
                    }
                    finally
                    {
                        if (scom.Connection.State == ConnectionState.Open) { scom.Connection.Close(); }
                    }
                }
            }
        }
        catch (Exception exc)
        {
            if (ExceptionEvent != null)
                ExceptionEvent(exc);
        }
        return unitID;
    }


    public Landmark GetLandmarkInfo(string landmarkName, int organization)
    {
        Landmark landmark = null;
        try
        {
            string sql = "SELECT LandmarkId,Latitude,Longitude,Radius FROM vlfLandmark WHERE LandmarkName=@LandmarkName and OrganizationId=@OrganizationId";
            using (SqlConnection scon = new SqlConnection(_ConnectionString))
            {
                using (SqlCommand scom = new SqlCommand(sql, scon))
                {
                    scom.CommandType = CommandType.Text;
                    scom.Parameters.Clear();
                    scom.Parameters.AddWithValue("@LandmarkName", landmarkName);
                    scom.Parameters.AddWithValue("@OrganizationId", organization);
                    if (scom.Connection.State == ConnectionState.Closed) { scom.Connection.Open(); }
                    try
                    {
                        DataTable dt = new DataTable();
                        using (SqlDataAdapter sda = new SqlDataAdapter(scom))
                        {
                            sda.Fill(dt);
                        }
                        foreach (DataRow row in dt.Rows)
                        {
                            landmark = new Landmark(row);
                            return landmark;
                        }
                    }
                    catch (Exception exc)
                    {
                        if (ExceptionEvent != null)
                            ExceptionEvent(exc);
                    }
                    finally
                    {
                        if (scom.Connection.State == ConnectionState.Open) { scom.Connection.Close(); }
                    }
                }
            }

        }
        catch (Exception exc)
        {
            if (ExceptionEvent != null)
                ExceptionEvent(exc);
        }
        return landmark;
    }

    public bool CreateVirtualLandmark(int organization, string landmarkName, double latitude, double longitude, string streetAddress)
    {
        return CreateVirtualLandmark(organization, landmarkName, latitude, longitude, 100, streetAddress);
    }
    public bool CreateVirtualLandmark(int organization, string landmarkName, double latitude, double longitude, int radius, string streetAddress)
    {
        bool result = false;
        try
        {
            string sql = "INSERT INTO vlfLandmarkVirtual (OrganizationId,LandmarkName,Latitude,Longitude,Radius,StreetAddress) VALUES (@OrganizationId,@LandmarkName,@Latitude,@Longitude,@Radius,@StreetAddress)";
            using (SqlConnection scon = new SqlConnection(_ConnectionString))
            {
                using (SqlCommand scom = new SqlCommand(sql, scon))
                {
                    scom.CommandType = CommandType.Text;
                    scom.Parameters.Clear();
                    scom.Parameters.AddWithValue("@OrganizationId", organization);
                    scom.Parameters.AddWithValue("@LandmarkName", landmarkName);
                    scom.Parameters.AddWithValue("@Latitude", latitude);
                    scom.Parameters.AddWithValue("@Longitude", longitude);
                    scom.Parameters.AddWithValue("@Radius", radius);
                    scom.Parameters.AddWithValue("@StreetAddress", streetAddress);
                    if (scom.Connection.State == ConnectionState.Closed) { scom.Connection.Open(); }
                    try
                    {
                        int rows = scom.ExecuteNonQuery();
                        return rows > 0;
                    }
                    catch (Exception exc)
                    {
                        if (ExceptionEvent != null)
                            ExceptionEvent(exc);
                    }
                    finally
                    {
                        if (scom.Connection.State == ConnectionState.Open) { scom.Connection.Close(); }
                    }
                }
            }
        }
        catch (Exception exc)
        {
            if (ExceptionEvent != null)
                ExceptionEvent(exc);
        }
        return result;
    }


    public string GetSamplePost(string path)
    {
        string result = string.Empty;
        try
        {
            DataSet ds = new DataSet();

            string content = string.Empty;
            using (StreamReader sr = new StreamReader(path))
            {
                content = sr.ReadToEnd();
            }
            using (MemoryStream ms = new MemoryStream(System.Text.Encoding.UTF8.GetBytes(content)))
            {
                ds.ReadXml(ms, XmlReadMode.Auto);
            }
        }
        catch (Exception exc)
        {
            if (ExceptionEvent != null)
                ExceptionEvent(exc);
        }
        return result;
    }


    public class DestinationMessage
    {
        string _Token;
        string _TicketID;
        string _TruckNumber;
        string _CustomerName;
        double _DeliveryLatitude;
        double _DeliveryLongitude;

        public string Token { get { return _Token; } set { _Token = value; } }
        public string TicketID { get { return _TicketID; } set { _TicketID = value; } }
        public string TruckNumber { get { return _TruckNumber; } set { _TruckNumber = value; } } //.Replace("'", "''"); } }
        public string CustomerName { get { return _CustomerName; } set { _CustomerName = value; } }
        public double DeliveryLatitude { get { return _DeliveryLatitude; } set { _DeliveryLatitude = value; } }
        public double DeliveryLongitude { get { return _DeliveryLongitude; } set { _DeliveryLongitude = value; } }
    }

}