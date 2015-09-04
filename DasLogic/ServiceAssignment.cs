using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using VLF.DAS.DB;

namespace VLF.DAS.Logic
{
    public class ServiceAssignment
    {
        public static void CreateServiceConfig(int organizationId, int userId, int serviceId, string landmarkName, string rulesApplied, DateTime expireDate)
        {
            
        }

        public static Dictionary<string, string> GetServices(int organizationId)
        {            
            try
            {
                DataTable dataTable = ServiceAssignmentDB.GetServices(organizationId);
                Dictionary<string, string> result = new Dictionary<string, string>();
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    if (!result.ContainsKey(Convert.ToString(dataRow["ServiceID"])))
                    {
                        result.Add(Convert.ToString(dataRow["ServiceID"]), Convert.ToString(dataRow["ServiceName"]));    
                    }                    
                }
                return result;
            }
            catch (Exception exception)
            {
                
                throw new Exception(exception.StackTrace);
            }
        }

        public static Dictionary<string, string> GetServiceInfo(int serviceId)
        {
            try
            {
                DataTable dataTable = ServiceAssignmentDB.GetServices(serviceId);
                Dictionary<string, string> result = new Dictionary<string, string>();
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    if (!result.ContainsKey(Convert.ToString(dataRow["ServiceID"])))
                    {
                        result.Add(Convert.ToString(dataRow["ServiceID"]), Convert.ToString(dataRow["ServiceName"]));
                    }
                }
                return result;
            }
            catch (Exception exception)
            {

                throw new Exception(exception.StackTrace);
            }
        }

        public static Dictionary<string, List<Dictionary<string, string>>>
            GetServiceAndConfiguredRules(int organizationId, int userId)
        {
            try
            {
                DataTable dataTable = ServiceAssignmentDB.GetServicesAndConfiguredRules(organizationId, userId);
                Dictionary<string, List<Dictionary<string, string>>> result = new Dictionary<string, List<Dictionary<string, string>>>();                
                foreach (DataRow dataRow in dataTable.Rows)
                {
                    string key = Convert.ToString(dataRow["ServiceID"]) + "|" + Convert.ToString(dataRow["ServiceName"]);
                    Dictionary<string, string> valDictionary = new Dictionary<string, string>();
                    valDictionary.Add(Convert.ToString(dataRow["ServiceConfigID"]), Convert.ToString(dataRow["ServiceConfigName"]));
                    if (!result.ContainsKey(key))
                    {    
                        List<Dictionary<string, string>> vaList = new List<Dictionary<string, string>>();
                        vaList.Add(valDictionary);
                        result.Add(key, vaList);
                    }
                    else
                    {
                        result[key].Add(valDictionary);
                    }                    
                }
                return result;
            }
            catch (Exception exception)
            {

                throw new Exception(exception.StackTrace);
            }
            return null;
        }

        public static IList<Dictionary<string, string>> GetRules(int serviceId)
        {
            try
            {
                DataTable rules = ServiceAssignmentDB.GetRules(serviceId);
                IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
                foreach (DataRow rule in rules.Rows)
                {
                    Dictionary<string, string> rowValue = new Dictionary<string, string>();
                    rowValue.Add("RuleName", Convert.ToString(rule["RuleName"]));
                    rowValue.Add("Operators", Convert.ToString(rule["Operators"]));
                    rowValue.Add("Results", Convert.ToString(rule["Results"]));
                    rowValue.Add("ToolTip", Convert.ToString(rule["ToolTip"]));
                    rowValue.Add("MustInclude", Convert.ToString(rule["MustInclude"]));
                    rowValue.Add("MustExclude", Convert.ToString(rule["MustExclude"]));
                    rowValue.Add("Sample", Convert.ToString(rule["Sample"]));
                    results.Add(rowValue);
                }
                return results;
            }
            catch (Exception exception)
            {
                
                throw new Exception(exception.StackTrace);
            }
        }

        public static int SaveServiceConfig(string expression, string serviceConfigName, int serviceId, int organizationId, int userId, DateTime? expiredDate = null, int serviceConfigId = 0, bool deleteService = false, int isActive = 1, int isReportable = 1)
        {
            try
            {
                return ServiceAssignmentDB.SaveServiceConfig(expression, serviceConfigName, serviceId, organizationId, userId, expiredDate, serviceConfigId, deleteService, isActive, isReportable);
            }
            catch (Exception exception)
            {
                
                throw new Exception(exception.StackTrace);
            }
            
        }

        public static int SaveNotificationConfig(int serviceConfigId, string recipiensList, string emailLevel, string subject, string messageBody)
        {
            return ServiceAssignmentDB.SaveNotification(serviceConfigId, recipiensList, emailLevel, subject, messageBody);
        }

        public static IList<Dictionary<string, string>> GetConfiguredServices(int serviceId, int organizationId, int userId, string serviceName = null)
        {
            try
            {
                DataTable configuredServices = ServiceAssignmentDB.GetConfiguredServices(serviceId, organizationId, userId, serviceName);
                IList<Dictionary<string, string>> result = new List<Dictionary<string, string>>();
                foreach (DataRow dataRow in configuredServices.Rows)
                {
                    Dictionary<string, string> tempResult = new Dictionary<string, string>();
                    tempResult.Add("ServiceConfigID", Convert.ToString(dataRow["ServiceConfigID"]));
                    tempResult.Add("ServiceConfigName", Convert.ToString(dataRow["ServiceConfigName"]));
                    tempResult.Add("IsActive", Convert.ToString(dataRow["IsActive"]));
                    tempResult.Add("RulesApplied", Convert.ToString(dataRow["RulesApplied"]));
                    tempResult.Add("CreatedDate", Convert.ToString(dataRow["CreatedDate"]));
                    tempResult.Add("ExpiredDate", Convert.ToString(dataRow["ExpiredDate"]));
                    tempResult.Add("RecipientsList", Convert.ToString(dataRow["RecipientsList"]));
                    tempResult.Add("EmailLevel", Convert.ToString(dataRow["EmailLevel"]));
                    tempResult.Add("Subject", Convert.ToString(dataRow["Subject"]));
                    tempResult.Add("Message", Convert.ToString(dataRow["Message"]));
                    tempResult.Add("OrganizationID", Convert.ToString(dataRow["OrganizationID"]));
                    tempResult.Add("IsReportable", Convert.ToString(dataRow["IsReportable"])); 
                    result.Add(tempResult);
                }
                return result;
            }
            catch (Exception exception)
            {

                throw new Exception(exception.StackTrace);
            }
        }
 
        public static Dictionary<string, string> GetConfiguredService(int configuredServiceId)
        {
            try
            {
                DataTable configuredServices = ServiceAssignmentDB.GetConfiguredService(configuredServiceId);
                Dictionary<string, string> result = new Dictionary<string, string>();
                foreach (DataRow dataRow in configuredServices.Rows)
                {
                    result.Add("ServiceConfigID", Convert.ToString(dataRow["ServiceConfigID"]));
                    result.Add("ServiceConfigName", Convert.ToString(dataRow["ServiceConfigName"]));
                    result.Add("RulesApplied", Convert.ToString(dataRow["RulesApplied"]));
                    result.Add("CreatedDate", Convert.ToString(dataRow["CreatedDate"]));
                    result.Add("ExpiredDate", Convert.ToString(dataRow["ExpiredDate"]));
                    result.Add("RecipientsList", Convert.ToString(dataRow["RecipientsList"]));
                    result.Add("EmailLevel", Convert.ToString(dataRow["EmailLevel"]));
                    result.Add("Subject", Convert.ToString(dataRow["Subject"]));
                    result.Add("Message", Convert.ToString(dataRow["Message"]));
                    break;
                }
                return result;
            }
            catch (Exception exception)
            {

                throw new Exception(exception.StackTrace);
            }
        }

        public static Dictionary<string, string> GetFilteredData(int organizationId, int userId, string dataName, string table)
        {
            try
            {            
                DataTable configuredServices = ServiceAssignmentDB.GetFilteredDataDB( organizationId, userId, dataName, table);
                Dictionary<string, string> result = new Dictionary<string, string>();
                foreach (DataRow dataRow in configuredServices.Rows)
                {

                    if (!result.ContainsKey(Convert.ToString(dataRow["id"])))
                    {
                        result.Add(Convert.ToString(dataRow["id"]), Convert.ToString(dataRow["name"]));
                    }                                                                  
                }
                return result;
            }
            catch (Exception exception)
            {

                throw new Exception(exception.StackTrace);
            }
        }

        public static void SaveObjects(int serviceConfigId, string target, IList<int> includeList, IList<int> excludeList, DateTime? createdDate = null, bool delete = false )
        {
            try
            {
                if (serviceConfigId.Equals(0))
                {
                    throw new Exception("Service config id cannot be empty");
                }
                ServiceAssignmentDB.SaveObjects(serviceConfigId, target, includeList, excludeList, createdDate);                
            }
            catch (Exception exception)
            {
                throw new Exception(exception.StackTrace);
            }
        }

        public static void SaveSingleAssignment(int serviceConfigId, int oid, string target, DateTime createdTime)
        {
            ServiceAssignmentDB.SaveSingleAssignment(serviceConfigId, oid, target, createdTime);
        }

        public static void DeleteObject(int serviceConfigId, int objectId, string target)
        {
            ServiceAssignmentDB.DeleteServiceAssignment(serviceConfigId, objectId, target);
        }

        public static Dictionary<string, IList<int>> GetSelectedFleetsAndVehicles(int selectedConfiguredId, string target)
        {
            try
            {
                DataTable results = ServiceAssignmentDB.GetAssignedFleetsAndExcludedVehicles(selectedConfiguredId, target);
                Dictionary<string, IList<int>> savedValues = new Dictionary<string, IList<int>>();
                foreach (DataRow dataRow in results.Rows)
                {
                    if (Convert.ToString(dataRow["Objects"]).Equals(target) && Convert.ToInt16(dataRow["Inclusive"]).Equals(1))
                    {
                        if (!savedValues.ContainsKey("Include"))
                        {
                            IList<int> includeList = new List<int>();
                            includeList.Add(Convert.ToInt32(dataRow["ObjectID"]));
                            savedValues.Add("Include", includeList);
                        }
                        else
                        {
                            savedValues["Include"].Add(Convert.ToInt32(dataRow["ObjectID"]));
                        }
                    }

                    if (Convert.ToString(dataRow["Objects"]).Equals("Vehicle") && Convert.ToInt16(dataRow["Inclusive"]).Equals(0))
                    {
                        if (!savedValues.ContainsKey("Exclude"))
                        {
                            IList<int> excludeList = new List<int>();
                            excludeList.Add(Convert.ToInt32(dataRow["ObjectID"]));
                            savedValues.Add("Exclude", excludeList);
                        }
                        else
                        {
                            savedValues["Exclude"].Add(Convert.ToInt32(dataRow["ObjectID"]));
                        }
                    }
                }
                return savedValues;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.StackTrace);
            }
        }

        public static IList<Dictionary<string, string>> GetSelectedFleetAndVehicleExceptions(Dictionary<string, string> conditions, int organizationId, int userId, ref int totalCount)
        {
            IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
            DataTable dataTable = ServiceAssignmentDB.GetFilteredFleetAndVehicleExceptions(conditions, organizationId, userId, ref totalCount);
            if (dataTable != null)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    int timeZone = 0;
                    int daylightSaving = 0;
                    if (dataTable.Rows[i]["TimeZone"] != DBNull.Value)
                    {
                        timeZone = Convert.ToInt32(dataTable.Rows[i]["TimeZone"]);
                        daylightSaving = Convert.ToInt32(dataTable.Rows[i]["DayLightSaving"]);
                    }                    
                    DateTime vehicleLocalStartDateTime =
                        Convert.ToDateTime(dataTable.Rows[i]["StDate"]).AddHours(timeZone + daylightSaving);
                    DateTime vehicleLocalEndDateTime = Convert.ToDateTime(dataTable.Rows[i]["EndDate"]).AddHours(timeZone + daylightSaving);

                    Dictionary<string, string> tmpRowResults = new Dictionary<string, string>();
                    tmpRowResults.Add("0", Convert.ToString(dataTable.Rows[i]["objectname"]));
                    
                    if(!Convert.ToInt32(dataTable.Rows[i]["ServiceID"]).Equals(2))
                    {
                        tmpRowResults.Add("1", string.Format("{0} (UTC{1})", vehicleLocalStartDateTime, timeZone + daylightSaving));
                        tmpRowResults.Add("2", string.Format("{0} (UTC{1})", vehicleLocalEndDateTime, timeZone + daylightSaving));
                        tmpRowResults.Add("3", (Convert.ToInt32(dataTable.Rows[i]["Duration"]) < 1 ? "1" : Convert.ToString(dataTable.Rows[i]["Duration"])));    
                    }
                    else
                    {
                        tmpRowResults.Add("1", string.Format("{0} (UTC{1})", vehicleLocalEndDateTime, timeZone + daylightSaving));
                        tmpRowResults.Add("2", string.Format("{0} (UTC{1})", vehicleLocalEndDateTime.AddSeconds(1), timeZone + daylightSaving));
                        tmpRowResults.Add("3", "1");
                    }                    
                    tmpRowResults.Add("4", Convert.ToString(dataTable.Rows[i]["Fuel"]));
                    tmpRowResults.Add("5", Convert.ToString(dataTable.Rows[i]["Distance"]));
                    if (Convert.ToString(dataTable.Rows[i]["RulesApplied"]).Contains("Metric = 2;"))
                    {
                        int speed = Convert.ToInt32(Math.Round(Convert.ToInt32(dataTable.Rows[i]["Speed"]) / 1.609344));
                        tmpRowResults.Add("6", Convert.ToString(speed) + "mph");    
                    }
                    else
                    {
                        tmpRowResults.Add("6", Convert.ToString(dataTable.Rows[i]["Speed"]) + "km/h");
                    }
                    
                    tmpRowResults.Add("7", string.Format("{0}[{1}]", dataTable.Rows[i]["LandmarkName"] == DBNull.Value ? "On road" : Convert.ToString(dataTable.Rows[i]["LandmarkName"]), Convert.ToString(dataTable.Rows[i]["LandmarkId"])));
                    tmpRowResults.Add("8", Convert.ToString(dataTable.Rows[i]["StreetAddress"])); 
                    results.Add(tmpRowResults);
                }
            }
            return results;
        }

        public static string GetLandmarkNameById(int landmarkId)
        {
            try
            {
                DataTable dataTable = ServiceAssignmentDB.GetLandmarkNameByLandmarkId(landmarkId);
                if (dataTable.Rows.Count > 0)
                {
                    return Convert.ToString(dataTable.Rows[0]["LandmarkName"]);
                }
                return null;
            }
            catch (Exception exception)
            {
                throw new Exception(exception.StackTrace);
            }
        }

        public static IList<Dictionary<string, string>> GetFilteredAssignments(Dictionary<string, string> conditions, int organizationid, int userId, ref int totalCount)
        {
            IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
            DataTable dataTable = ServiceAssignmentDB.GetFilteredResults(conditions, organizationid, userId, ref totalCount);
            if (dataTable != null)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Dictionary<string, string> tmpRowResults = new Dictionary<string, string>();
                    tmpRowResults.Add("0", Convert.ToString(dataTable.Rows[i]["objectname"]));
                    tmpRowResults.Add("1", Convert.ToString(dataTable.Rows[i]["ServiceConfigName"]));
                    tmpRowResults.Add("2", Convert.ToString(dataTable.Rows[i]["Objects"]));
                    tmpRowResults.Add("3", Convert.ToString(dataTable.Rows[i]["RulesApplied"]));
                    tmpRowResults.Add("4", Convert.ToString(dataTable.Rows[i]["CreatedDate"]));
                    tmpRowResults.Add("5", Convert.ToString(dataTable.Rows[i]["ExpiredDate"]));
                    tmpRowResults.Add("6", Convert.ToString(dataTable.Rows[i]["UserName"]));
                    tmpRowResults.Add("7", string.Format("<a href=\"#\" onclick=\"InitializeReport('{0}', {1}, {2}, '{3}')\">View</a>", conditions["searchCriteria"], Convert.ToString(dataTable.Rows[i]["myobjectid"]), Convert.ToString(dataTable.Rows[i]["ServiceConfigId"]), Convert.ToString(dataTable.Rows[i]["ServiceConfigName"])));
                    
                    results.Add(tmpRowResults);
                }
            }            
            return results;
        }

        public static IList<Dictionary<string, string>> GetFilteredAssignmentHistory(Dictionary<string, string> conditions, int organizationid, int serviceId, int serviceConfigId, int userId, ref int totalCount)
        {
            IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
            DataTable dataTable = ServiceAssignmentDB.GetFilteredResults(conditions, organizationid, userId, ref totalCount, serviceId, serviceConfigId, true);
            if (dataTable != null)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Dictionary<string, string> tmpRowResults = new Dictionary<string, string>();
                    tmpRowResults.Add("0", Convert.ToString(dataTable.Rows[i]["objectname"]));                                        
                    tmpRowResults.Add("1", Convert.ToString(dataTable.Rows[i]["UserName"]));
                    tmpRowResults.Add("2", Convert.ToString(dataTable.Rows[i]["CreatedDate"]));
                    tmpRowResults.Add("3", Convert.ToString(dataTable.Rows[i]["ExpiredDate"]));
                    tmpRowResults.Add("4", Convert.ToString(dataTable.Rows[i]["Deleted"]).Equals("1") ? "Deleted" : "Active");

                    results.Add(tmpRowResults);
                }
            }
            return results;
        }

        public static IList<Dictionary<string, string>> GetFilteredUnAssignedService(Dictionary<string, string> conditions, int organizationid, int userId, ref int totalCount, int serviceId)
        {
            IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
            DataTable dataTable = ServiceAssignmentDB.GetFilteredUnAssignedServices(conditions, organizationid, serviceId, userId, ref totalCount);
            if (dataTable != null)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Dictionary<string, string> tmpRowResults = new Dictionary<string, string>();
                    tmpRowResults.Add("0", Convert.ToString(dataTable.Rows[i]["ServiceConfigName"]));
                    tmpRowResults.Add("1", String.Empty);
                    tmpRowResults.Add("2", String.Empty);
                    tmpRowResults.Add("3", String.Empty);
                    tmpRowResults.Add("4", Convert.ToString(dataTable.Rows[i]["CreatedDate"]));                    
                    tmpRowResults.Add("5", Convert.ToString(dataTable.Rows[i]["UserName"]));
                    tmpRowResults.Add("6", string.Format("<a href=\"#\" onclick=\"AddService('{0}')\">Create Assignment</a>", Convert.ToString(dataTable.Rows[i]["ServiceConfigName"])));

                    results.Add(tmpRowResults);
                }
            }
            return results;
        }

        public static IList<Dictionary<string, string>> GetFilteredDedicatedPageAssignments(Dictionary<string, string> conditions, int organizationid, int userId, ref int totalCount, int serviceId = 0)
        {
            IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
            DataTable dataTable = ServiceAssignmentDB.GetFilteredResults(conditions, organizationid, userId, ref totalCount, serviceId);
            if (dataTable != null)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Dictionary<string, string> tmpRowResults = new Dictionary<string, string>();
                    tmpRowResults.Add("0", Convert.ToString(dataTable.Rows[i]["ServiceConfigName"]));
                    tmpRowResults.Add("1", Convert.ToString(dataTable.Rows[i]["objectname"]));                    
                    tmpRowResults.Add("2", Convert.ToString(dataTable.Rows[i]["Objects"]));
                    tmpRowResults.Add("3", Convert.ToString(dataTable.Rows[i]["RulesApplied"]));
                    tmpRowResults.Add("4", Convert.ToString(dataTable.Rows[i]["CreatedDate"]));                    
                    tmpRowResults.Add("5", Convert.ToString(dataTable.Rows[i]["UserName"]));
                    tmpRowResults.Add("6", string.Format("<a href=\"#\" onclick=\"DeleteAssignment({0}, {1})\">Delete</a>&nbsp;&nbsp;|&nbsp;&nbsp;<a href=\"#\" onclick=\"InitializeReport('{2}', {3}, {4}, '{5}')\">Report</a>", dataTable.Rows[i]["ServiceConfigId"], dataTable.Rows[i]["myobjectid"], conditions["searchCriteria"], Convert.ToString(dataTable.Rows[i]["myobjectid"]), Convert.ToString(dataTable.Rows[i]["ServiceConfigId"]), Convert.ToString(dataTable.Rows[i]["ServiceConfigName"])));

                    results.Add(tmpRowResults);
                }
            }
            return results;
        }

        public static IList<Dictionary<string, string>> GetFilteredLandmarks(Dictionary<string, string> conditions, int organizationid, int userId, ref int totalCount)
        {
            IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
            DataTable dataTable = ServiceAssignmentDB.GetFilteredLandmarks(conditions, organizationid, userId, ref totalCount);
            if (dataTable != null)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Dictionary<string, string> tmpRowResults = new Dictionary<string, string>();
                    tmpRowResults.Add("0", Convert.ToString(dataTable.Rows[i]["LandmarkId"]));
                    tmpRowResults.Add("1", Convert.ToString(dataTable.Rows[i]["LandmarkName"]));
                    tmpRowResults.Add("2", string.Format("<input type=\"button\" onclick=\"SelectLandmark('{0}', '{1}')\" value=\"Select\" id=\"btnSelect\" />", Convert.ToString(dataTable.Rows[i]["LandmarkId"]), Convert.ToString(dataTable.Rows[i]["LandmarkName"]).Replace("'", @"\'")));
                    
                    results.Add(tmpRowResults);
                }
            }
            return results;
        }

        public static IList<Dictionary<string, string>> GetFilteredDtcValues(Dictionary<string, string> conditions, ref int totalCount)
        {
            IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
            DataTable dataTable = ServiceAssignmentDB.GetFilteredDtcValues(conditions, ref totalCount);
            if (dataTable != null)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    Dictionary<string, string> tmpRowResults = new Dictionary<string, string>();
                    tmpRowResults.Add("0", Convert.ToString(dataTable.Rows[i]["DTCCode"]));
                    tmpRowResults.Add("1", Convert.ToString(dataTable.Rows[i]["text"]));
                    tmpRowResults.Add("2", string.Format("<input type=\"button\" onclick=\"SelectDtcValue('{0}', '{1}')\" value=\"Select\" id=\"btnSelect\" />", Convert.ToString(dataTable.Rows[i]["DTCCode"]), Convert.ToString(dataTable.Rows[i]["text"]).Replace("'", @"\'")));

                    results.Add(tmpRowResults);
                }
            }
            return results;
        }
        public static IList<Dictionary<string, string>> GetFilteredConfiguredRoutes(Dictionary<string, string> conditions, int organizationid, int userId, ref int totalCount)
        {
            IList<Dictionary<string, string>> results = new List<Dictionary<string, string>>();
            if (!conditions.ContainsKey("sVehicleId"))
            {
                throw new Exception("Vehicle id cannotbe empty");
            }
            int vehicleId = Convert.ToInt32(conditions["sVehicleId"]);
            DataTable dataTable = ServiceAssignmentDB.GetFilteredConfiguredRoutesFromDb(conditions, organizationid, userId, ref totalCount);
            if (dataTable != null)
            {
                for (int i = 0; i < dataTable.Rows.Count; i++)
                {
                    string assignToVehicle = "<a href=\"#\" onclick=\"AssignToRoute(" + Convert.ToString(dataTable.Rows[i]["ServiceConfigID"]) + ")\">Assign</a>";
                    if (dataTable.Rows[i]["ObjectID"] != DBNull.Value)
                    {
                        if (Convert.ToInt32(dataTable.Rows[i]["ObjectID"]).Equals(vehicleId))
                        {
                            assignToVehicle = "<a href=\"#\" onclick=\"DeleteAssignment(" + Convert.ToString(dataTable.Rows[i]["ServiceConfigID"]) + ")\">Delete</a>";
                        }
                    }
                    Dictionary<string, string> tmpRowResults = new Dictionary<string, string>();
                    tmpRowResults.Add("0", Convert.ToString(dataTable.Rows[i]["ServiceConfigName"]));
                    tmpRowResults.Add("1", Convert.ToString(dataTable.Rows[i]["RulesApplied"]));
                    tmpRowResults.Add("2", assignToVehicle);
                    results.Add(tmpRowResults);
                }
            }
            return results;
        }
        
        public static bool SaveServiceRouteMapping(int organizationId, int serviceConfigId, int routeId, bool delete = false)
        {
            if (!delete)
            {
                return ServiceAssignmentDB.SaveToServiceRouteMapping(serviceConfigId, routeId);    
            }
            else
            {                
                return ServiceAssignmentDB.DeleteServiceConfigMapping(serviceConfigId, routeId);
            }
            
        }

        public static int RouteServiceConfigId(int routeId)
        {
            return ServiceAssignmentDB.MappingExist(routeId);
        }

        public static DataSet GetColorRuleByVehicle(string vehicle)
        {
            return ServiceAssignmentDB.GetColorRuleByVehicle(vehicle);    
        }

    }    
}
