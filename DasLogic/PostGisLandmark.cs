using System;
using System.Collections.Generic;
using System.Data;
using System.Runtime.Serialization;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using VLF.DAS.DB;
namespace VLF.DAS.Logic
{
    public class PostGisLandmark
    {

        public static bool SaveRoute(string connStr, string routePoints, int organizationId, string landmarkName, string contactPerson,
                                       int buffer, string email, string contactNo, int timeZone, int daylightSaving, string routerLink, string wayPoints,
                                       string description, double latitude, double longitude, ref int landmarkId, int createUserId)
        {            
            if (landmarkId > 0)
            {
                if (PostGISLandmarkDB.ThereIsThingAssigned(organizationId, landmarkId))
                {
                    return false;
                }
            }
            return PostGISLandmarkDB.SaveRoute(connStr, routePoints, organizationId, landmarkName, contactPerson,
                                       buffer, email, contactNo, timeZone, daylightSaving, routerLink, wayPoints,
                                       description, latitude, longitude, ref landmarkId, createUserId);

        }

        public static bool DeleteRoute(string connStr, int organizationId, int landmarkId)
        {
            if (landmarkId > 0)
            {
                if (PostGISLandmarkDB.ThereIsThingAssigned(organizationId, landmarkId))
                {
                    return false;
                }
            }            
            return PostGISLandmarkDB.DeleteRoute(connStr, landmarkId);
        }

        public static IList<Dictionary<string, string>> GetFilteredRoutes(Dictionary<string, string> conditions, int organizationid, int userid, ref int totalCount )
        {
            IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
            DataTable dataTable = PostGISLandmarkDB.RetrieveFromDB(conditions, organizationid, userid, ref totalCount);            
            if (dataTable != null)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Dictionary<string, string> tmpRowResults = new Dictionary<string, string>();
                    tmpRowResults.Add("0", Convert.ToString(dataTable.Rows[i]["landmarkname"]));
                    tmpRowResults.Add("1", Convert.ToString(dataTable.Rows[i]["contactpersonname"]));
                    tmpRowResults.Add("2", Convert.ToString(dataTable.Rows[i]["createddatetime"]));
                    tmpRowResults.Add("3", string.Format("<a href=\"{0}\">Map It</a>&nbsp;&nbsp;|&nbsp;&nbsp;<a href=\"#\" onclick=\"DeleteRouter({1})\">Delete</a>", Convert.ToString(dataTable.Rows[i]["routerlink"]), Convert.ToInt32(dataTable.Rows[i]["originallandmarkid"])));
                    results.Add(tmpRowResults);
                }
            }            
            return results;
        }

        public static IList<Dictionary<string, string>> GetFilteredRoutesWithService(Dictionary<string, string> conditions, int organizationid, int userid, ref int totalCount)
        {
            IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
            DataTable dataTable = PostGISLandmarkDB.RetrieveRoutesFromServiceDB(conditions, organizationid, userid, ref totalCount);
            if (dataTable != null)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Dictionary<string, string> tmpRowResults = new Dictionary<string, string>();                    
                    tmpRowResults.Add("0", Convert.ToString(dataTable.Rows[i]["LandmarkName"]));
                    tmpRowResults.Add("1", Convert.ToString(dataTable.Rows[i]["UserName"]));
                    tmpRowResults.Add("2", Convert.ToString(dataTable.Rows[i]["CreatedDate"] == DBNull.Value ? "Not created" : dataTable.Rows[i]["CreatedDate"]));
                    tmpRowResults.Add("3", Convert.ToString(dataTable.Rows[i]["IsExpired"]).Equals("0") && dataTable.Rows[i]["ServiceConfigName"] != DBNull.Value ? "Active" : "Inactive");
                    tmpRowResults.Add("4", Convert.ToString(dataTable.Rows[i]["IsAssigned"]).Equals("1") && dataTable.Rows[i]["ServiceConfigID"] != DBNull.Value ? "Yes" : "No");
                    if (Convert.ToString(dataTable.Rows[i]["IsExpired"]).Equals("0") &&
                        dataTable.Rows[i]["ServiceConfigName"] != DBNull.Value)
                    {
                        tmpRowResults.Add("5", string.Format("<a href=\"AssignmentForm.aspx?servicename={2}&service=route\">Create Assignment</a>&nbsp;&nbsp;|&nbsp;&nbsp; <a href=\"javascript:ShowAssignmentHistory({0}, '{1}')\">Show assignments</a> &nbsp;&nbsp;|&nbsp;&nbsp;<a href=\"javascript:opener.MapRoute({3}, {0})\">Map it</a>", Convert.ToInt32(dataTable.Rows[i]["ServiceConfigID"] == DBNull.Value ? "0" : dataTable.Rows[i]["ServiceConfigID"]), Convert.ToString(dataTable.Rows[i]["LandmarkName"]), Convert.ToString(dataTable.Rows[i]["ServiceConfigName"]), Convert.ToString(dataTable.Rows[i]["LandmarkId"])));
                    }
                    else
                    {
                        tmpRowResults.Add("5", string.Format("<a href=\"javascript:ShowAssignmentHistory({0}, '{1}')\">Show assignments</a>&nbsp;&nbsp;|&nbsp;&nbsp;<a href=\"javascript:opener.MapRoute({2}, {0})\">Map it</a>", Convert.ToInt32(dataTable.Rows[i]["ServiceConfigID"] == DBNull.Value ? "0" : dataTable.Rows[i]["ServiceConfigID"]), Convert.ToString(dataTable.Rows[i]["LandmarkName"]), Convert.ToString(dataTable.Rows[i]["LandmarkId"])));
                    }
                    results.Add(tmpRowResults);
                }
            }
            return results;
        }

        public static Dictionary<string, string> GetLandmark(string connStr, int landmarkId)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            DataTable dataTable = PostGISLandmarkDB.GetLandmarkInfoById(connStr, landmarkId);
            if (dataTable.Rows.Count > 0)
            {
                foreach (DataColumn columnName in dataTable.Columns)
                {
                    result.Add(Convert.ToString(columnName), Convert.ToString(dataTable.Rows[0][Convert.ToString(columnName)]));
                }
            }
            return result;
        }

        public static IList<Dictionary<string, string>> GetBsmLandmarksAndGeozones(int organiztionId, string input)
        {
            IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
            DataTable dataTable = PostGISLandmarkDB.GetBsmLandmarksAndGeozonesFromDB(organiztionId, input);
            if (dataTable != null)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Dictionary<string, string> tmpRowResults = new Dictionary<string, string>();
                    tmpRowResults.Add("description", Convert.ToString(dataTable.Rows[i]["name"]));
                    tmpRowResults.Add("reference", Convert.ToString(dataTable.Rows[i]["latlon"]));                    
                    results.Add(tmpRowResults);
                }
            }
            return results;
        }

        public static IList<Dictionary<string, string>> GetDisputePoints(int organizationId = 0, string filter = null, int pid = 0)
        {
            IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
            DataTable dataTable = PostGISLandmarkDB.GetDisputePointsDb(organizationId, filter, pid);
            if (dataTable != null)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    int speed = Convert.ToInt32(dataTable.Rows[i]["speed"]);
                    //int correctionid = Convert.ToInt32(dataTable.Rows[i]["correctionid"]);
                    //int deleted = Convert.ToInt32(dataTable.Rows[i]["deleted"]);
                    //if (Convert.ToInt32(dataTable.Rows[i]["metric"]) > 1 && correctionid > 0 && deleted.Equals(0))
                    //{
                    //    speed = Convert.ToInt32(Math.Round(Convert.ToInt32(dataTable.Rows[i]["speed"]) / 1.609344));
                    //}

                    Dictionary<string, string> tmpRowResults = new Dictionary<string, string>();
                    tmpRowResults.Add("id", Convert.ToString(dataTable.Rows[i]["id"]));
                    tmpRowResults.Add("latitude", Convert.ToString(dataTable.Rows[i]["latitude"]));
                    tmpRowResults.Add("longitude", Convert.ToString(dataTable.Rows[i]["longitude"]));
                    tmpRowResults.Add("speed", string.Format("{0}{1}", speed, (Convert.ToInt32(dataTable.Rows[i]["metric"]).Equals(1) ? "km/h" : "mph")));
                    tmpRowResults.Add("metric", Convert.ToString(dataTable.Rows[i]["metric"]));
                    tmpRowResults.Add("notes", Convert.ToString(dataTable.Rows[i]["notes"]));
                    tmpRowResults.Add("correctionid", Convert.ToString(dataTable.Rows[i]["correctionid"]));
                    tmpRowResults.Add("notificationid", Convert.ToString(dataTable.Rows[i]["notificationid"]));
                    tmpRowResults.Add("deleted", Convert.ToString(dataTable.Rows[i]["deleted"]));
                    tmpRowResults.Add("comments", Convert.ToString(dataTable.Rows[i]["comments"]));
                    results.Add(tmpRowResults);
                }
            }
            return results;
        }

        public static IList<Dictionary<string, string>> GetDisputePointsForGrid(Dictionary<string, string> conditions, int organizationId, ref int totalCount)
        {
            IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
            DataTable dataTable = PostGISLandmarkDB.RetrieveDisputesFromDB(conditions, organizationId, ref totalCount);
            if (dataTable != null)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Dictionary<string, string> tmpRowResults = new Dictionary<string, string>();
                    string status = "<span class=\"correct\">Disputed</span>";
                    string correctionLink = string.Format("<a href=\"javascript:FixPoint({0}, {1}, {2})\">Map it</a>", Convert.ToString(dataTable.Rows[i]["latitude"]), Convert.ToString(dataTable.Rows[i]["longitude"]), Convert.ToString(dataTable.Rows[i]["id"]));
                    if (Convert.ToInt32(dataTable.Rows[i]["correctionid"]) > 0)
                    {
                        correctionLink = string.Format("<a href=\"javascript:DisplayCorrectedArea({0}, {1})\">Map it</a>", Convert.ToString(dataTable.Rows[i]["correctionid"]), Convert.ToString(dataTable.Rows[i]["id"]));
                        //tmpRowResults.Add("DT_RowClass", "corrected");
                        status = "<span class=\"corrected\">Accepted</span>";
                    }
                    
                    if (Convert.ToInt32(dataTable.Rows[i]["deleted"]).Equals(1))
                    {
                        status = "<span class=\"dismissed\">Dismissed</font>";
                    }

                    int speed = Convert.ToInt32(dataTable.Rows[i]["speed"]);
                    //int correctionid = Convert.ToInt32(dataTable.Rows[i]["correctionid"]);
                    //int deleted = Convert.ToInt32(dataTable.Rows[i]["deleted"]);
                    //if ((Convert.ToInt32(dataTable.Rows[i]["metric"]) > 1 && correctionid > 0 && deleted.Equals(0)) || (Convert.ToInt32(dataTable.Rows[i]["metric"]) > 1 && correctionid < 0))
                    //{
                    //    speed = Convert.ToInt32(Math.Round(Convert.ToInt32(dataTable.Rows[i]["speed"])/1.609344));
                    //}
                    string extraInfo = Convert.ToString(dataTable.Rows[i]["notes"]);
                    string[] extraInfos = extraInfo.Split(new string[] {"||"}, StringSplitOptions.None);
                    tmpRowResults.Add("0", Convert.ToString(dataTable.Rows[i]["notificationid"]));
                    tmpRowResults.Add("1", Convert.ToString(dataTable.Rows[i]["streetaddress"]));
                    tmpRowResults.Add("2", extraInfos[1]);
                    tmpRowResults.Add("3", (extraInfos.Length > 3 ? extraInfos[3] : string.Empty));
                    tmpRowResults.Add("4", extraInfos[0]);
                    tmpRowResults.Add("5", string.Format("{0}{1}", speed, (Convert.ToInt32(dataTable.Rows[i]["metric"]).Equals(1) ? "km/h" : "mph")));    
                    tmpRowResults.Add("6", extraInfos[2]);
                    tmpRowResults.Add("7", Convert.ToString(dataTable.Rows[i]["Created"]));
                    tmpRowResults.Add("8", Convert.ToString(dataTable.Rows[i]["modified"]));
                    tmpRowResults.Add("9", status);
                    tmpRowResults.Add("10", correctionLink);                    
                    results.Add(tmpRowResults);
                }
            }
            return results;
        }

        public static IList<Dictionary<string, string>> GetSegmentsForGrid(Dictionary<string, string> conditions, int organizationId, ref int totalCount)
        {
            IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
            try
            {
                
                DataTable dataTable = PostGISLandmarkDB.RetrieveSegmentsFromDB(conditions, organizationId, ref totalCount);                
                if (dataTable != null)
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {                        
                        Dictionary<string, string> tmpRowResults = new Dictionary<string, string>();
                        tmpRowResults.Add("0", Convert.ToString(dataTable.Rows[i]["routename"]));
                        tmpRowResults.Add("1", Convert.ToString(dataTable.Rows[i]["contactperson"]));
                        tmpRowResults.Add("2", Convert.ToString(dataTable.Rows[i]["created"]));
                        tmpRowResults.Add("3", Convert.ToString(dataTable.Rows[i]["expired"]));
                        tmpRowResults.Add("4", Convert.ToString(dataTable.Rows[i]["speed"]));
                        tmpRowResults.Add("5", Convert.ToString(dataTable.Rows[i]["description"]));
                        tmpRowResults.Add("6", Convert.ToString(dataTable.Rows[i]["AssignedNum"]));
                        tmpRowResults.Add("7", string.Format("<a href=\"javascript:DisplayCorrectedArea({0}, 0)\">Map it</a>", dataTable.Rows[i]["id"]));
                        results.Add(tmpRowResults);
                    }                    
                }
            }
            catch (Exception exception)
            {                
                throw new Exception(exception.StackTrace);
            }
            return results;
        }
        public static bool DeleteDisputePoint(int pointId, string comment = null)
        {           
            try
            {
                int organizationId = 0;
                int landmarkId = 0, vehicleId = 0, boxId = 0;
                string streedAddress = null;
                DataTable dataTable = PostGISLandmarkDB.GetDisputePointsDb(0, null, pointId);
                if (dataTable != null && PostGISLandmarkDB.DeleteDisputePointDb(pointId, comment))
                {
                    for (int i = 0; i < dataTable.Rows.Count; i++)
                    {                                               
                        DataTable dataViolationTable = PostGISLandmarkDB.GetViolation(Convert.ToInt32(dataTable.Rows[i]["notificationid"]));                       
                        if (dataViolationTable != null)
                        {
                            for (int a = 0; a < dataViolationTable.Rows.Count; a++)
                            {

                                organizationId = Convert.ToInt32(dataViolationTable.Rows[a]["OrganizationID"]);
                                landmarkId = Convert.ToInt32(dataViolationTable.Rows[a]["LandmarkID"]);
                                vehicleId = Convert.ToInt32(dataViolationTable.Rows[a]["VehicleID"]);
                                streedAddress = Convert.ToString(dataViolationTable.Rows[a]["StreetAddress"]);
                                boxId = Convert.ToInt32(dataViolationTable.Rows[a]["BoxID"]);
                            }
                        }

                        string notes = Convert.ToString(dataTable.Rows[i]["notes"]);
                        string[] extraInfos = notes.Split(new string[] { "||" }, StringSplitOptions.None);
                        int speed = Convert.ToInt32(dataTable.Rows[i]["speed"]);
                        int correctionid = Convert.ToInt32(dataTable.Rows[i]["correctionid"]);
                        int deleted = Convert.ToInt32(dataTable.Rows[i]["deleted"]);
                        int nid = Convert.ToInt32(dataTable.Rows[i]["nid"]);
                        //if (Convert.ToInt32(dataTable.Rows[i]["metric"]) > 1 && correctionid > 0 && deleted.Equals(0))
                        //{
                        //    speed = Convert.ToInt32(Math.Round(Convert.ToInt32(dataTable.Rows[i]["speed"]) / 1.609344));
                        //}
                        string emailslist = Convert.ToString(dataTable.Rows[i]["emailslist"]);
                        string body = EmailBody(Convert.ToInt32(dataTable.Rows[i]["notificationid"]), extraInfos[1], streedAddress,
                            (extraInfos.Length > 3 ? extraInfos[3] : string.Empty), extraInfos[0],
                            string.Format("{0}{1}", speed,
                                (Convert.ToInt32(dataTable.Rows[i]["metric"]).Equals(1) ? "km/h" : "mph")),
                            (extraInfos.Length > 1 ? extraInfos[2] : string.Empty), "Declined", nid, comment);
                        GenerateNotification(organizationId, boxId, vehicleId, landmarkId, "Vehicle", vehicleId, emailslist, string.Format("Speed dispute ticket: {0} has been declined", Convert.ToInt32(dataTable.Rows[i]["notificationid"])), body);
                        return true;
                    }
                }
                return false;
            }
            catch (Exception exception)
            {                
                throw new Exception(exception.Message);
            }           
        }

        public static bool UpdateViolationDispute( int violationId, int speed, double lat, double lon, int metric, string notes, string drivername, string errorspeed, string objects, int objectId, string yourSpeed, int nid, string email)
        {
            string vNotes = string.Format("{0}||{1}||{2}||{3}", errorspeed, drivername, notes, yourSpeed);
            DataTable dataTable = PostGISLandmarkDB.GetViolation(violationId);
            int organizationId = 0, serviceConfigId = 0;
            int landmarkId = 0, vehicleId = 0, boxId = 0;
            string streedAddress = null; 
            

            if (dataTable != null)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {

                    organizationId = Convert.ToInt32(dataTable.Rows[i]["OrganizationID"]);
                    landmarkId = Convert.ToInt32(dataTable.Rows[i]["LandmarkID"]);
                    vehicleId = Convert.ToInt32(dataTable.Rows[i]["VehicleID"]);
                    serviceConfigId = Convert.ToInt32(dataTable.Rows[i]["ServiceConfigId"]);
                    streedAddress = Convert.ToString(dataTable.Rows[i]["StreetAddress"]);
                    boxId = Convert.ToInt32(dataTable.Rows[i]["BoxID"]);
                }
            }
            else
            {
                return false;
            }
            string emailsList =
                PostGISLandmarkDB.GetEmailsList(organizationId, serviceConfigId, boxId, landmarkId, objects, objectId) +
                ';' + email;
            PostGISLandmarkDB.UpdateViolationDispute(violationId, vNotes);
            Dictionary<string, string> result = PostGISLandmarkDB.AlreadyCreatedForDispute(organizationId, lat, lon);
            int iErrorSpeed = Convert.ToInt32(errorspeed.Replace("km/h", string.Empty).Replace("mph", string.Empty));
            int iYourSpeed = Convert.ToInt32(yourSpeed.Replace("km/h", string.Empty).Replace("mph", string.Empty));
            bool createdNewDispute = PostGISLandmarkDB.CreateNewDisputePoints(violationId, lat, lon, speed, iErrorSpeed, iYourSpeed, metric, vNotes, organizationId,
                streedAddress, emailsList, Convert.ToInt32(result.ContainsKey("id") ? result["id"] : "0"), nid);
            if (!result.ContainsKey("id"))
            {
                if (createdNewDispute)
                {

                    string body = EmailBody(violationId, drivername, streedAddress, yourSpeed, errorspeed, string.Format("{0}{1}", speed, (metric.Equals(1) ? "km/h" : "mph")), notes, "Submitted", nid);
                    return GenerateNotification(organizationId, boxId, vehicleId, landmarkId, objects, objectId, emailsList, string.Format("Speed dispute ticket: {0} has been submitted", violationId), body);
                }
                return false;
            }
            else
            {
                string speedWithMetric = string.Format("{0}km/hr", Convert.ToString(result["speed"]));
                if (metric > 1)
                {
                    speedWithMetric = string.Format("{0}mph", Convert.ToString(result["speed"]));
                }
                GenerateCorrectedConfirmation(violationId,
                    true,
                    speedWithMetric, emailsList, vNotes, result["description"], nid);
            }

            return true;
        }

        private static bool GenerateNotification(int organizationId, int boxId, int vehicleId, int landmarkId, string objects, int objectId, string emailslist, string subject, string body)
        {

            return PostGISLandmarkDB.SendNotification(organizationId, boxId, vehicleId, landmarkId, objects, objectId, emailslist, subject, body);
        }


        private static bool GenerateCorrectedConfirmation(int vid, bool corrected, string speed, string emailsList, string notes, string comments, int nid)
        {
            DataTable dataTable = PostGISLandmarkDB.GetViolation(vid);
            int organizationId = 0;
            int landmarkId = 0, vehicleId = 0, boxId = 0;
            string streedAddress = null;
            string subject = string.Format("Your speed dispute {0} has been corrected", vid);
            string[] extraInfos = notes.Split(new string[] { "||" }, StringSplitOptions.None);
           
           
            if (dataTable != null)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {

                    organizationId = Convert.ToInt32(dataTable.Rows[i]["OrganizationID"]);
                    landmarkId = Convert.ToInt32(dataTable.Rows[i]["LandmarkID"]);
                    vehicleId = Convert.ToInt32(dataTable.Rows[i]["VehicleID"]);                                     
                    boxId = Convert.ToInt32(dataTable.Rows[i]["BoxID"]);
                    streedAddress = Convert.ToString(dataTable.Rows[i]["StreetAddress"]);
                }
            }
            else
            {
                return false;
            }
            string body =
                EmailBody(vid, extraInfos[1], streedAddress, (extraInfos.Length > 3 ? extraInfos[3] : string.Empty), extraInfos[0], speed, (extraInfos.Length > 1 ? extraInfos[2] : string.Empty), "Accepted", nid, comments);
            if (!corrected)
            {
                subject = string.Format("Your speed dispute {0} cannot be corrected", vid);
                body = EmailBody(vid, extraInfos[1], streedAddress, (extraInfos.Length > 3 ? extraInfos[3] : string.Empty), extraInfos[0], speed, (extraInfos.Length > 1 ? extraInfos[2] : string.Empty), "Decliend", nid, comments);                
            }
            
            return GenerateNotification(organizationId, boxId, vehicleId, landmarkId, "Vehicle", vehicleId, emailsList, subject, body); ;
        }

        public static IList<Dictionary<string, string>> SaveSpeedCorrection(string connStr, string routePoints, int organizationId, string landmarkName, string contactPerson,
                                       int buffer, string email, string contactNo, int timeZone, int daylightSaving, string routerLink, string wayPoints,
                                       string description, double latitude, double longitude, ref int correctionid, int createUserId, int speed, int disputeId = 0, string comment = null)
        {
            if (!string.IsNullOrEmpty(comment) && disputeId > 0)
            {
                PostGISLandmarkDB.UpdateComment(disputeId, comment);
            }

            DataTable dataTable = PostGISLandmarkDB.SaveSpeedCorrection(connStr, routePoints, organizationId, landmarkName, contactPerson,
                                       buffer, email, contactNo, timeZone, daylightSaving, routerLink, wayPoints,
                                       description, latitude, longitude, ref correctionid, createUserId, speed, disputeId, comment);
            IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
            if (dataTable != null)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Dictionary<string, string> tmpRowResults = new Dictionary<string, string>();
                    tmpRowResults.Add("id", Convert.ToString(dataTable.Rows[i]["id"]));
                    tmpRowResults.Add("latitude", Convert.ToString(dataTable.Rows[i]["latitude"]));
                    tmpRowResults.Add("longitude", Convert.ToString(dataTable.Rows[i]["longitude"]));
                    tmpRowResults.Add("speed", Convert.ToString(dataTable.Rows[i]["speed"]));
                    tmpRowResults.Add("metric", Convert.ToString(dataTable.Rows[i]["metric"]));
                    tmpRowResults.Add("notes", Convert.ToString(dataTable.Rows[i]["notes"]));
                    tmpRowResults.Add("correctionid", Convert.ToString(dataTable.Rows[i]["correctionid"]));
                    tmpRowResults.Add("deleted", Convert.ToString(dataTable.Rows[i]["deleted"]));
                    results.Add(tmpRowResults);
                    string speedWithMetric = string.Format("{0}km/hr", Convert.ToString(dataTable.Rows[i]["speed"]));
                    if (Convert.ToInt32(tmpRowResults["metric"]) > 1)
                    {
                        speedWithMetric = string.Format("{0}mph", Convert.ToString(dataTable.Rows[i]["speed"]));
                    }
                    GenerateCorrectedConfirmation(Convert.ToInt32(dataTable.Rows[i]["notificationid"]),
                        Convert.ToInt32(dataTable.Rows[i]["correctionid"]) > 0,
                        speedWithMetric, Convert.ToString(dataTable.Rows[i]["emailslist"]), Convert.ToString(dataTable.Rows[i]["notes"]), description, Convert.ToInt32(dataTable.Rows[i]["nid"]));                    

                }
            }
            return results;
        }

        public static Dictionary<string, string> GetCorrectionRoute(string connStr, int correctionid)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            DataTable dataTable = PostGISLandmarkDB.GetCorrectionRoute(connStr, correctionid);
            if (dataTable != null)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {                    
                    result.Add("id", Convert.ToString(dataTable.Rows[i]["id"]));
                    result.Add("organizationid", Convert.ToString(dataTable.Rows[i]["organizationid"]));
                    result.Add("routelink", Convert.ToString(dataTable.Rows[i]["routelink"]));
                    result.Add("waypoints", Convert.ToString(dataTable.Rows[i]["waypoints"]));
                    result.Add("speed", Convert.ToString(dataTable.Rows[i]["speed"]));
                    result.Add("routename", Convert.ToString(dataTable.Rows[i]["routename"]));
                    result.Add("contactperson", Convert.ToString(dataTable.Rows[i]["contactperson"]));
                    result.Add("email", Convert.ToString(dataTable.Rows[i]["email"]));
                    result.Add("phone", Convert.ToString(dataTable.Rows[i]["phone"]));
                    result.Add("buffer", Convert.ToString(dataTable.Rows[i]["buffer"]));
                    result.Add("description", Convert.ToString(dataTable.Rows[i]["description"]));                        
                }
            }
            return result;
        }

        public static IList<Dictionary<string, string>> DeleteCorrectedSegment(int correctionId)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            DataTable dataTable = PostGISLandmarkDB.DeleteCorrectedSegment(correctionId);
            IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
            if (dataTable != null)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Dictionary<string, string> tmpRowResults = new Dictionary<string, string>();
                    tmpRowResults.Add("id", Convert.ToString(dataTable.Rows[i]["id"]));
                    tmpRowResults.Add("latitude", Convert.ToString(dataTable.Rows[i]["latitude"]));
                    tmpRowResults.Add("longitude", Convert.ToString(dataTable.Rows[i]["longitude"]));
                    tmpRowResults.Add("speed", Convert.ToString(dataTable.Rows[i]["speed"]));
                    tmpRowResults.Add("metric", Convert.ToString(dataTable.Rows[i]["metric"]));
                    tmpRowResults.Add("notes", Convert.ToString(dataTable.Rows[i]["notes"]));
                    tmpRowResults.Add("correctionid", Convert.ToString(dataTable.Rows[i]["correctionid"]));
                    tmpRowResults.Add("deleted", Convert.ToString(dataTable.Rows[i]["deleted"]));
                    results.Add(tmpRowResults);
                }
            }
            return results;
        }

        private static string EmailBody(int ticketId, string reportedBy, string address, string yourSpeed, string postedSpeed, string claimedSpeed, string notes, string status, int nid, string comments = null)
        {           
            string disputeComment = string.Empty;
            if (!string.IsNullOrEmpty(comments))
            {
                disputeComment = string.Format("<tr>" +
                                "<td>Comment:</td><td>{0}</td>" +
                                "</tr>" +
                                "<tr>", comments);
            }
           
            string originalMessage = PostGISLandmarkDB.GetMessage(nid);
            string body = string.Format("<table>" +
                                        "<tr>" +
                                        "<td>Ticket ID:</td><td><strong>{0}</strong></td>" +
                                        "</tr>" +
                                        "<tr>" +
                                        "<td>Reported by:</td><td>{1}</td>" +
                                        "</tr>" +                                                                               
                                        "<tr>" +
                                        "<td>Report speed:</td><td>{2}</td>" +
                                        "</tr>" +
                                        "<tr>" +
                                        "<td>Note:</td><td>{3}</td>" +
                                        "</tr>{4}<tr>" +
                                        "<td>Status:</td><td>{5}</td>" +
                                        "</tr>" +
                                        "<tr>" +
                                        "<td colspan=\"2\"><hr /><br /><br /><strong>Infraction Details</strong>{6}" +                                        
                                        "</td>" +
                                        "</tr>" +
                                        "</table>", ticketId, reportedBy, claimedSpeed, notes, disputeComment, status, originalMessage);
            return body;
        }
    }
}
