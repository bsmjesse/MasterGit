using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using System.Data.SqlClient;
using System.Collections;
using System.Configuration;
using System.Web;
using System.Web.Services;
using System.Web.Services.Protocols;
//using System.Web.Script.Services;

namespace VLF.ASI.Interfaces
{
    /// <summary>
    /// Summary description for mapAjax
    /// </summary>
    [WebService(Namespace = "http://bsmwireless.com/")]
    [WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
    public class mapAjax : System.Web.Services.WebService
    {

        public mapAjax()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        [WebMethod]
        public string HelloWorld()
        {
            return "Hello World";
        }

        [WebMethod]
        public int GetLandmarksWithBoundaries(int uid, string sid, int organizationIdentifier, float topleftlat, float topleftlon, float bottomrightlat, float bottomrightlon, ref string csv)
        {
            //string csv = string.Empty;
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ToString();
                string sql = "GetOrganizationLandmarksWithBoundary";
                SqlConnection con = new SqlConnection(constr);
                SqlCommand com = new SqlCommand(sql, con);
                com.Parameters.Clear();
                com.Parameters.AddWithValue("@orgId", organizationIdentifier);
                com.Parameters.AddWithValue("@topleftlat", topleftlat);
                com.Parameters.AddWithValue("@topleftlong", topleftlon);
                com.Parameters.AddWithValue("@bottomrightlat", bottomrightlat);
                com.Parameters.AddWithValue("@bottomrightlong", bottomrightlon);
                com.CommandType = CommandType.StoredProcedure;
                DataSet ds = new DataSet();
                using (SqlDataAdapter da = new SqlDataAdapter(com))
                {
                    da.Fill(ds);
                }
                if (ds.Tables.Count > 0 && ds.Tables[0].Rows.Count > 0)
                {
                    DataTable dt = ds.Tables[0];
                    string[] values = new string[dt.Rows.Count];
                    for (int a = 0; a < dt.Rows.Count; a++)
                    {
                        //(int)model, description, latitude, longitude,radius,iconName
                        DataRow dr = dt.Rows[a];
                        string[] value = new string[6];
                        int c = 0;
                        value[c++] = "1";
                        value[c++] = Convert.ToString(dr["LandmarkName"]);
                        value[c++] = Convert.ToString(dr["Latitude"]);
                        value[c++] = Convert.ToString(dr["Longitude"]);
                        value[c++] = Convert.ToString(dr["Radius"]);
                        value[c++] = "landmark.ico"; //Convert.ToString(dr["description"]);
                        values[a] = string.Join("^", value);
                    }
                    csv = string.Join("|", values);
                }
            }
            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return 1;
            }
            return 0;
        }


        [WebMethod]
        public int GetGeozonesWithBoundaries(int uid, string sid, int organizationIdentifier, float topleftlat, float topleftlon, float bottomrightlat, float bottomrightlon, ref string csv)
        {
            try
            {
                string constr = ConfigurationManager.ConnectionStrings["DBConnectionString"].ToString();

                //if any one point of a geozone exist within boundary retrieve it...

                string sqlb = "GetOrganizationGeozonesWithBoundary";
                SqlConnection con = new SqlConnection(constr);
                SqlCommand com = new SqlCommand(sqlb, con);
                com.Parameters.Clear();
                com.Parameters.AddWithValue("@orgId", organizationIdentifier);
                com.Parameters.AddWithValue("@topleftlat", topleftlat);
                com.Parameters.AddWithValue("@topleftlong", topleftlon);
                com.Parameters.AddWithValue("@bottomrightlat", bottomrightlat);
                com.Parameters.AddWithValue("@bottomrightlong", bottomrightlon);
                com.CommandType = CommandType.StoredProcedure;
                DataTable dtZonesets = new DataTable();
                using (SqlDataAdapter da = new SqlDataAdapter(com))
                {
                    da.Fill(dtZonesets);
                }

                int currentGeoId = 0;

                //pull the parent record of each geozone with at least one point within our boundary and...
                //build new query string
                if (dtZonesets.Rows.Count > 0)
                {
                    StringBuilder sqla = new StringBuilder(string.Format("SELECT GeozoneNo, GeozoneId, GeozoneName, Type, SeverityId FROM vlfOrganizationGeozone WHERE ((OrganizationId={0}) AND (", organizationIdentifier));
                    List<string> wheres = new List<string>();
                    for (int a = 0; a < dtZonesets.Rows.Count; a++)
                    {
                        DataRow dr = dtZonesets.Rows[a];
                        int geoid = Convert.ToInt32(dr["GeozoneNo"]);
                        if (geoid != currentGeoId)
                        {
                            wheres.Add(string.Format("(GeozoneNo={0})", geoid));
                            currentGeoId = geoid;
                        }
                    }
                    sqla.Append(string.Join(" OR ", wheres.ToArray()));
                    sqla.Append("));");

                    com = new SqlCommand(sqla.ToString(), con);
                    com.CommandType = CommandType.Text;
                    DataTable dtGeozones = new DataTable();
                    using (SqlDataAdapter da = new SqlDataAdapter(com))
                    {
                        da.Fill(dtGeozones);
                    }

                    //prepare our container...
                    string[] values = new string[dtGeozones.Rows.Count];


                    //loop through each parent geozone and retrieve its full point set
                    for (int a = 0; a < dtGeozones.Rows.Count; a++)
                    {
                        DataRow dr = dtGeozones.Rows[a];
                        currentGeoId = Convert.ToInt32(dr["GeozoneNo"]);

                        //sqlb = string.Format("SELECT vlfGeozoneSet.GeozoneNo, vlfGeozoneSet.SequenceNum, vlfGeozoneSet.Latitude, vlfGeozoneSet.Longitude, vlfOrganizationGeozone.OrganizationId FROM vlfOrganizationGeozone INNER JOIN vlfGeozoneSet ON vlfOrganizationGeozone.GeozoneNo = vlfGeozoneSet.GeozoneNo WHERE ([OrganizationId] = {0}", organizationIdentifier);
                        sqlb = string.Format("SELECT GeozoneNo, SequenceNum, Latitude , Longitude FROM vlfGeozoneSet WHERE GeozoneNo = {0}", currentGeoId);
                        com = new SqlCommand(sqlb, con);
                        dtZonesets = new DataTable();
                        using (SqlDataAdapter da = new SqlDataAdapter(com))
                        {
                            da.Fill(dtZonesets);
                        }

                        string where = string.Format("GeozoneNo = {0}", currentGeoId);
                        string order = "SequenceNum ASC";

                        DataRow[] points = dtZonesets.Select(where, order);

                        //(int)model, description, numPoints, pointList, type, severity
                        int numpoints = points.Length;
                        string[] value = new string[5 + (numpoints * 2)];
                        int c = 0;
                        value[c++] = "2";
                        value[c++] = Convert.ToString(dr["GeozoneName"]);
                        value[c++] = Convert.ToString(numpoints);
                        for (int b = 0; b < numpoints; b++)
                        {

                            value[c++] = Convert.ToString(points[b]["Latitude"]);
                            value[c++] = Convert.ToString(points[b]["Longitude"]);
                        }

                        value[c++] = Convert.ToString(dr["Type"]);
                        value[c++] = Convert.ToString(dr["SeverityId"]);
                        values[a] = string.Join("^", value);

                    }
                    csv = string.Join("|", values);
                }
            }

            catch (Exception exc)
            {
                Console.WriteLine(exc.Message);
                return 1;
            }
            return 0;
        }


        [WebMethod]
        public int GetPoisWithBoundaries(int uid, string sid, int organizationIdentifier, float topleftlat, float topleftlon, float bottomrightlat, float bottomrightlon, ref string csv)
        {
            return 0;
        }
    }
}
