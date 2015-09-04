/** \file      DBDriver.cs
 *  \comment   keywords for logs
 *             uid            ->    userid
 *             vehicleId      ->    vehicleId
 *             driverId       ->    driverId
 *             orgId          ->    organization Id
 *             dtFrom         ->    date time from
 *             dtTo           ->    date time to
 */
using System;
using System.Web;
using System.Collections;
using System.Web.Services;
using System.Web.Services.Protocols;
using System.Diagnostics;
using VLF.ERRSecurity;
using VLF.DAS.Logic;
using VLF.CLS;
using VLF.CLS.Def;
using System.Data;
using System.Xml;
using System.IO;
using VLF.CLS.Def.Structures;

namespace VLF.ASI.Interfaces
{
    /// <summary>
    /// DBDriver data, assignment and history
    /// </summary>
    [WebService(Namespace = "http://www.sentinelfm.com")]

    public class DBDriver : System.Web.Services.WebService
    {
        public DBDriver()
        {
            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }

        #region refactored functions
        /// <summary>
        ///      by replacing the log calls we can add a UDP sender for logs
        ///      or dynamic filtering 
        /// </summary>
        /// <param name="strFormat"></param>
        /// <param name="objects"></param>
        private void Log(string strFormat, params object[] objects)
        {
            try
            {
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceVerbose,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.WebInterfaces,
                                       string.Format(strFormat, objects)));
            }
            catch (Exception exc)
            {

            }
        }


        /// <summary>
        ///      the exception should be saved in a separate file or in the Event log of the computer
        /// </summary>
        /// <param name="strFormat"></param>
        /// <param name="objects"></param>
        private void LogException(string strFormat, params object[] objects)
        {
            try
            {
                Trace.WriteLineIf(AppConfig.tsWeb.Enabled | AppConfig.tsMain.TraceError,
                     CLS.Util.TraceFormat(CLS.Def.Enums.TraceSeverity.Error,
                                       string.Format(strFormat, objects)));
            }
            catch (Exception exc)
            {

            }
        }

        private bool ValidateUserDriver(int userId, string SID, int driverId)
        {
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
                return dbUser.ValidateUserDriver(userId, driverId);
            }
        }

        private bool ValidateUserVehicle(int userId, string SID, Int64 vehicleId)
        {
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
                return dbUser.ValidateUserVehicle(userId, vehicleId);
            }
        }

        private bool ValidateSuperUser(int userId, string SID)
        {
            // Authenticate
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
                return dbUser.ValidateSuperUser(userId);
            }
        }
        #endregion      refactored functions

        # region Driver general

        /// <summary>
        ///      Add a new driver
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="xmlDriver">  Expects an XML string with format 
        ///         [FirstName],[ LastName],[ License],[ Class],[LicenseIssued], 
        ///         [LicenseExpired], [OrganizationId], [Gender], [Height],[HomePhone], 
        ///         [CellPhone], [AdditionalPhone], [SMSId], [Email], [Address], [City], 
        ///         [Zipcode], [State], [Country], [Description]
        /// </param>
        /// <returns></returns>
        [WebMethod(Description = "Add a new driver")]
        public int AddDriver(int userId, string SID, string xmlDriver)
        {
            try
            {
                Log(">> AddDriver(uId={0}, data={1})", userId, xmlDriver);

                if (!ValidateSuperUser(userId, SID))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    // xml parsing
                    XmlUtil xDoc = new XmlUtil(xmlDriver);

                    driver.AddDriver(
                       xDoc.GetNodeValue("FirstName"), xDoc.GetNodeValue("LastName"),
                       xDoc.GetNodeValue("License"), xDoc.GetNodeValue("Class"),
                       Convert.ToDateTime(xDoc.GetNodeValue("LicenseIssued")),
                       Convert.ToDateTime(xDoc.GetNodeValue("LicenseExpired")),
                       Convert.ToInt32(xDoc.GetNodeValue("OrganizationId")),
                       xDoc.GetNodeValue("Gender")[0],
                       Convert.ToInt16(xDoc.GetNodeValue("Height")),
                       xDoc.GetNodeValue("HomePhone"),
                       xDoc.GetNodeValue("CellPhone"), xDoc.GetNodeValue("AdditionalPhone"),
                       xDoc.GetNodeValue("SmsPWD"), xDoc.GetNodeValue("Smsid"), xDoc.GetNodeValue("Email"),
                       xDoc.GetNodeValue("Address"), xDoc.GetNodeValue("City"),
                       xDoc.GetNodeValue("Zipcode"), xDoc.GetNodeValue("State"),
                       xDoc.GetNodeValue("Country"), xDoc.GetNodeValue("Description"));


                    LoggerManager.RecordUserAction("Driver", userId, 0, "vlfDriver",
                                                null,
                                                "Add", this.Context.Request.UserHostAddress,
                                                this.Context.Request.RawUrl,
                                                string.Format("FirstName='{0}' AND LastName='{1}'", xDoc.GetNodeValue("FirstName"), xDoc.GetNodeValue("LastName")));


                }
                //////////////////////////////////////////////////////////////////////////////////////////////

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< AddDriver : uId={0},EXC={1})", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Add collection of drivers")]
        public int AddDrivers(int userId, string SID, string xmlDrivers)
        {
            try
            {
                Log(">> AddDrivers(uId={0}, data={1})", userId, xmlDrivers);

                // Authentication
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    //if (!dbUser.ValidateSuperUser(userId))
                    //   return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                    DataSet dsDrivers = new DataSet("Drivers");
                    dsDrivers.ReadXmlSchema(Server.MapPath("~/Datasets/DS_Drivers.xsd"));

                    DataSet dsData = new DataSet();
                    dsData.ReadXml(new StringReader(xmlDrivers));

                    foreach (DataRow row in dsData.Tables[0].Rows)
                    {
                        dsDrivers.Tables[0].ImportRow(row);
                    }

                    using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                    {
                        driver.AddDrivers(dsDrivers.Tables[0]);

                        LoggerManager.RecordUserAction("Driver", userId, 0, "vlfDriver",
                                                   null,
                                                   "Add", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl,
                                                   string.Format("Import drivers file"));
                    }
                    //////////////////////////////////////////////////////////////////////////////////////////////
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< AddDrivers : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Delete a driver")]
        public int DeleteDriver(int userId, string SID, int driverId)
        {
            try
            {
                Log(">> DeleteDriver(uId={0}, DriverId={1})", userId, driverId);

                if (!ValidateUserDriver(userId, SID, driverId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Driver", userId, 0, "vlfDriver",
                                                 string.Format("DriverId={0}", driverId.ToString()),
                                                 "Update", this.Context.Request.UserHostAddress,
                                                 this.Context.Request.RawUrl,
                                                 string.Format("Update driver({0}) - Initial values", driverId));

                    driver.DeleteDriver(driverId);

                    LoggerManager.RecordUserAction("Driver", userId, 0, "vlfDriver",
                                                  string.Format("DriverId={0}", driverId.ToString()),
                                                  "Delete", this.Context.Request.UserHostAddress,
                                                  this.Context.Request.RawUrl,
                                                  string.Format("Delete driver({0})", driverId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< DeleteDriver : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Update a driver")]
        public int UpdateDriver(int userId, string SID, string xmlDriver)
        {
            int driverId = 0;
            try
            {
                Log(">> UpdateDriver(uId={0}, data={1})", userId, xmlDriver);

                // xml parsing
                XmlUtil xDoc = new XmlUtil(xmlDriver);
                driverId = Convert.ToInt32(xDoc.GetNodeValue("DriverId"));

               // if (!ValidateUserDriver(userId, SID, driverId))
                 //   return Convert.ToInt32(InterfaceError.AuthorizationFailed);

		 // Authentication
	            LoginManager.GetInstance().SecurityCheck(userId, SID);





                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Driver", userId, 0, "vlfDriver",
                                                string.Format("DriverId={0}", driverId),
                                                "Update", this.Context.Request.UserHostAddress,
                                                this.Context.Request.RawUrl,
                                                string.Format("Update driver({0}) - Initial values", driverId));

                    driver.UpdateDriver(driverId,
                                         xDoc.GetNodeValue("FirstName"), xDoc.GetNodeValue("LastName"),
                                         xDoc.GetNodeValue("License"), xDoc.GetNodeValue("Class"),
                                         Convert.ToDateTime(xDoc.GetNodeValue("LicenseIssued")),
                                         Convert.ToDateTime(xDoc.GetNodeValue("LicenseExpired")),
                                         Convert.ToInt32(xDoc.GetNodeValue("OrganizationId")),
                                         xDoc.GetNodeValue("Gender")[0],
                                         Convert.ToInt16(xDoc.GetNodeValue("Height")),
                                         xDoc.GetNodeValue("HomePhone"),
                                         xDoc.GetNodeValue("CellPhone"), xDoc.GetNodeValue("AdditionalPhone"),
                                         xDoc.GetNodeValue("SmsPWD"), xDoc.GetNodeValue("Smsid"), xDoc.GetNodeValue("Email"),
                                         xDoc.GetNodeValue("Address"), xDoc.GetNodeValue("City"),
                                         xDoc.GetNodeValue("Zipcode"), xDoc.GetNodeValue("State"),
                                         xDoc.GetNodeValue("Country"), xDoc.GetNodeValue("Description"));

                    LoggerManager.RecordUserAction("Driver", userId, 0, "vlfDriver",
                                                 string.Format("DriverId={0}", driverId),
                                                 "Update", this.Context.Request.UserHostAddress,
                                                 this.Context.Request.RawUrl,
                                                 string.Format("Update driver({0})", driverId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< UpdateDriver : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Get a driver")]
        public int GetDriver(int userId, string SID, int driverId, ref string xmlResult)
        {
            try
            {
                Log(">> GetDriver(uId={0}, DriverId={1})", userId, driverId);

                if (!ValidateUserDriver(userId, SID, driverId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = driver.GetDriver(driverId);

                    if (Util.IsDataSetValid(dsDriver))
                        xmlResult = dsDriver.GetXml();
                    else
                        xmlResult = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetDriver : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Get all drivers for organization")]
        public int GetAllDrivers(int userId, string SID, int organizationId, ref string xmlResult)
        {
            try
            {
                Log(">> GetAllDrivers(uId={0}, OrganizationId={1})", userId, organizationId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = new DataSet();

                    dsDriver = driver.GetDriversForOrganization(organizationId);

                    if (Util.IsDataSetValid(dsDriver))
                        xmlResult = dsDriver.GetXml();
                    else
                        xmlResult = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetAllDrivers : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }


        # endregion

        # region Driver assignment

        [WebMethod(Description = "Add a new driver assignment")]
        public int AssignDriver(int userId, string SID, long vehicleId, int driverId, string description)
        {
            try
            {
                Log(">> AssignDriver(uId={0}, vehicleId={1}, driverId={2})", userId, vehicleId, driverId);

               // if (!ValidateUserDriver(userId, SID, driverId))
                   // return Convert.ToInt32(InterfaceError.AuthorizationFailed);
		
		LoginManager.GetInstance().SecurityCheck(userId, SID);


                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    driver.AddDriverAssignment(userId, vehicleId, driverId, description);

                    LoggerManager.RecordUserAction("Driver", userId, 0, "vlfDriverAssignment",
                                                   string.Format("VehicleId={0}", vehicleId),
                                                   "Add", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl,
                                                   string.Format("Add driver assignment for vehicle({0}) and driver({1})", vehicleId, driverId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< AssignDriver : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Add a new driver assignment")]
        public int AssignDriverByDate(int userId, string SID, long vehicleId, int driverId, DateTime assgnDate, string description)
        {
            try
            {
                Log(">> AssignDriverByDate(uId={0}, vehicleId={1}, driverId={2})", userId, vehicleId, driverId);

                if (!ValidateUserDriver(userId, SID, driverId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    driver.AddDriverAssignment(userId, vehicleId, driverId, assgnDate, description);

                    LoggerManager.RecordUserAction("Driver", userId, 0, "vlfDriverAssignment",
                                                 string.Format("VehicleId={0}", vehicleId),
                                                 "Add", this.Context.Request.UserHostAddress,
                                                 this.Context.Request.RawUrl,
                                                 string.Format("Add driver assignment for vehicle({0}) and driver({1})", vehicleId, driverId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< AssignDriverByDate : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Add collection of drivers assignments")]
        public int AssignDrivers(int userId, string SID, string xmlData)
        {
            try
            {
                Log(">> AssignDrivers(uId={0})", userId);

                // Authentication
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    //if (!dbUser.ValidateSuperUser(userId))
                    //   return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                    //DataSet dsDrivers = new DataSet("Drivers");
                    //dsDrivers.ReadXmlSchema(Server.MapPath("~/App_Code/DS_Drivers.xsd"));

                    //DataSet dsData = new DataSet();
                    //dsData.ReadXml(new StringReader(xmlData));

                    //foreach (DataRow row in dsData.Tables[0].Rows)
                    //{
                    //   dsDrivers.Tables[0].ImportRow(row);
                    //}

                    using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                    {
                        //driver.AssignDrivers(userId, dsData.Tables[0]);
                        driver.AssignDriversInBulk(userId, xmlData);

                        LoggerManager.RecordUserAction("Driver", userId, 0, "vlfDriverAssignment",
                                                   null,
                                                   "Add", this.Context.Request.UserHostAddress,
                                                   this.Context.Request.RawUrl,
                                                   string.Format("Add bulk driver assignment"));
                    }
                    //////////////////////////////////////////////////////////////////////////////////////////////
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< AssignDrivers : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Delete a driver assignment")]
        public int DeleteDriverAssignment(int userId, string SID, long vehicleId, int driverId, string description)
        {
            try
            {
                Log(">> DeleteDriverAssignment(uId={0}, vehicleId={1}, driverId={2})", userId, vehicleId, driverId);

                //if (!ValidateUserDriver(userId, SID, driverId))
                  //  return Convert.ToInt32(InterfaceError.AuthorizationFailed);

		 // Authenticate
               LoginManager.GetInstance().SecurityCheck(userId, SID);


                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Driver", userId, 0, "vlfDriverAssignment",
                                             string.Format("VehicleId={0} AND DriverId={1}", vehicleId, driverId),
                                             "Update", this.Context.Request.UserHostAddress,
                                             this.Context.Request.RawUrl,
                                             string.Format("Update driver assignment for vehicle({0}) and driver({1}) - Initial values", vehicleId, driverId));

                    driver.DeleteActiveDriverAssignment(userId, vehicleId, driverId, description);

                    LoggerManager.RecordUserAction("Driver", userId, 0, "vlfDriverAssignment",
                                              string.Format("VehicleId={0} AND DriverId={1}", vehicleId, driverId),
                                              "Delete", this.Context.Request.UserHostAddress,
                                              this.Context.Request.RawUrl,
                                              string.Format("Delete driver assignment for vehicle({0}) and driver({1})", vehicleId, driverId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< DeleteDriverAssignment : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Delete all driver assignments")]
        public int DeleteDriverAssignments(int userId, string SID, int driverId, string description)
        {
            try
            {
                Log(">> DeleteDriverAssignments(uId={0}, driverId={1})", userId, driverId);

                if (!ValidateUserDriver(userId, SID, driverId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    LoggerManager.RecordInitialValues("Driver", userId, 0, "vlfDriverAssignment",
                                              string.Format("DriverId={0}", driverId.ToString()),
                                              "Update", this.Context.Request.UserHostAddress,
                                              this.Context.Request.RawUrl,
                                              string.Format("Update all driver assignments for driver({0}) - Initial values", driverId));

                    driver.DeleteActiveDriverAssignments(userId, driverId, description);

                    LoggerManager.RecordUserAction("Driver", userId, 0, "vlfDriverAssignment",
                                               string.Format("DriverId={0}", driverId.ToString()),
                                               "Delete", this.Context.Request.UserHostAddress,
                                               this.Context.Request.RawUrl,
                                               string.Format("Delete all driver assignments for driver({0})", driverId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< DeleteDriverAssignments : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Get driver assignment")]
        public int GetDriverAssignment(int userId, string SID, long vehicleId, ref string xmlResult)
        {
            try
            {
                Log(">> GetDriverAssignment(uId={0}, vehicleId = {1})", userId, vehicleId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = new DataSet();

                    dsDriver = driver.GetDriverActiveAssignment(vehicleId);

                    if (Util.IsDataSetValid(dsDriver))
                        xmlResult = dsDriver.GetXml();
                    else
                        xmlResult = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetDriverAssignment : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Get all driver assignments")]
        public int GetDriverAssignments(int userId, string SID, int driverId, ref string xmlResult)
        {
            try
            {
                Log(">> GetDriverAssignments(uId={0}, driverId={1})", userId, driverId);

                if (!ValidateUserDriver(userId, SID, driverId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = new DataSet();

                    dsDriver = driver.GetDriverActiveAssignments(driverId);

                    if (Util.IsDataSetValid(dsDriver))
                        xmlResult = dsDriver.GetXml();
                    else
                        xmlResult = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetDriverAssignments : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Get unassigned drivers for organization")]
        public int GetUnassignedDrivers(int userId, string SID, int organizationId, ref string xmlResult)
        {
            try
            {
                Log(">> GetUnassignedDrivers(uId={0}, orgId={1})", userId, organizationId);


                //if (!ValidateSuperUser(userId, SID))
                  //  return Convert.ToInt32(InterfaceError.AuthorizationFailed);

		 LoginManager.GetInstance().SecurityCheck(userId, SID);


                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = new DataSet();

                    dsDriver = driver.GetUnassignedDriversForOrganization(organizationId);

                    if (Util.IsDataSetValid(dsDriver))
                        xmlResult = dsDriver.GetXml();
                    else
                        xmlResult = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetUnassignedDrivers : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Get assigned drivers for organization")]
        public int GetAssignedDrivers(int userId, string SID, int organizationId, ref string xmlResult)
        {
            try
            {
                Log(">> GetAssignedDrivers(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateSuperUser(userId, SID))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = new DataSet();

                    dsDriver = driver.GetAssignedDriversForOrganization(organizationId);

                    if (Util.IsDataSetValid(dsDriver))
                        xmlResult = dsDriver.GetXml();
                    else
                        xmlResult = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetAssignedDrivers : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }


        [WebMethod(Description = "Get hours of service state types")]
        public int GetHoursOfServiceStateTypes(int userId, string SID, ref string xmlResult)
        {
            try
            {
                Log(">> GetHoursOfServiceStateTypes(uId={0})", userId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);


                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = new DataSet();

                    dsDriver = driver.GetHoursOfServiceStateTypes();

                    if (Util.IsDataSetValid(dsDriver))
                        xmlResult = dsDriver.GetXml();
                    else
                        xmlResult = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetHoursOfServiceStateTypes : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }



        [WebMethod(Description = "Get hours of service rules")]
        public int GetHoursOfServiceRules(int userId, string SID, ref string xmlResult)
        {
            try
            {
                Log(">> GetHoursOfServiceRules(uId={0})", userId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);


                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = new DataSet();

                    dsDriver = driver.GetHoursOfServiceRules();

                    if (Util.IsDataSetValid(dsDriver))
                        xmlResult = dsDriver.GetXml();
                    else
                        xmlResult = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetHoursOfServiceRules : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }


        # endregion

        # region Assignment history

        [WebMethod(Description = "Add driver - vehicle assignment history")]
        public int AddAssignmentHistory(int userId, string SID, int driverId, long vehicleId, string description, DateTime startDt, DateTime endDt)
        {
            int rows = 0;
            try
            {
                Log(">> AddAssignmentHistory(uId={0}, driverId={1})", userId, driverId);

                if (!ValidateUserDriver(userId, SID, driverId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    rows = driver.AddAssignmentHistory(driverId, vehicleId, userId, description, startDt, endDt);
                }

                return (rows > 0) ? (int)InterfaceError.NoError : (int)InterfaceError.InvalidParameter;
            }
            catch (Exception ex)
            {
                LogException("<< AddAssignmentHistory : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Get driver assignment history")]
        public int GetDriverAssignmentHistory(int userId, string SID, int driverId, ref string xmlResult)
        {
            try
            {
                Log(">> GetDriverAssignmentHistory(uId={0}, driverId={1})", userId, driverId);

               // if (!ValidateUserDriver(userId, SID, driverId))
                 //   return Convert.ToInt32(InterfaceError.AuthorizationFailed);

		 LoginManager.GetInstance().SecurityCheck(userId, SID);


                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = new DataSet();

                    dsDriver = driver.GetDriverAssignmentHistory(driverId);

                    if (Util.IsDataSetValid(dsDriver))
                        xmlResult = dsDriver.GetXml();
                    else
                        xmlResult = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetDriverAssignmentHistory : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Get driver assignment history with dates")]
        public int GetDriverAssignmentHistoryByDates(int userId, string SID, int driverId, DateTime from, DateTime to, ref string xmlResult)
        {
            try
            {
                Log(">> GetDriverAssignmentHistoryByDates(uId={0}, driverId={1}, dtFrom={2}, dtTo={3})", userId, driverId, from, to);

                if (!ValidateUserDriver(userId, SID, driverId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = new DataSet();

                    dsDriver = driver.GetDriverAssignmentHistory(driverId, from, to);

                    if (Util.IsDataSetValid(dsDriver))
                        xmlResult = dsDriver.GetXml();
                    else
                        xmlResult = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetDriverAssignmentHistoryByDates : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Get vehicle assignment history")]
        public int GetVehicleAssignmentHistory(int userId, string SID, long vehicleId, ref string xmlResult)
        {
            try
            {
                Log(">> GetVehicleAssignmentHistory(uId={0}, vehicleId={1})", userId, vehicleId);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsVehicle = new DataSet();

                    dsVehicle = driver.GetVehicleAssignmentHistory(vehicleId);

                    if (Util.IsDataSetValid(dsVehicle))
                        xmlResult = dsVehicle.GetXml();
                    else
                        xmlResult = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetVehicleAssignmentHistory : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Get vehicle assignment history")]
        public int GetVehicleAssignmentHistoryByDates(int userId, string SID, long vehicleId,
                                                      DateTime from, DateTime to, ref string xmlResult)
        {
            try
            {
                Log(">> GetVehicleAssignmentHistoryByDates(uId={0}, vehicleId={1}, dtFrom={2}, dtTo={3})",
                                  userId, vehicleId, from, to);

                if (!ValidateUserVehicle(userId, SID, vehicleId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsVehicle = new DataSet();

                    dsVehicle = driver.GetVehicleAssignmentHistory(vehicleId, from, to);

                    if (Util.IsDataSetValid(dsVehicle))
                    {
                        xmlResult = dsVehicle.GetXml();
                    }
                    else
                        xmlResult = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetVehicleAssignmentHistoryByDates : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Get driver - vehicle assignment history")]
        public int GetDriverVehicleAssignmentHistory(int userId, string SID, int driverId,
                                                     long vehicleId, ref string xmlResult)
        {
            try
            {
                Log(">> GetDriverVehicleAssignmentHistory(uId={0}, vehicleId={1} driverId={2})",
                                            userId, vehicleId, driverId);

                if (!ValidateUserDriver(userId, SID, driverId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsVehicle = new DataSet();

                    dsVehicle = driver.GetDriverVehicleAssignmentHistory(driverId, vehicleId);

                    if (Util.IsDataSetValid(dsVehicle))
                        xmlResult = dsVehicle.GetXml();
                    else
                        xmlResult = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetDriverVehicleAssignmentHistory : uid={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Get driver - vehicle assignment history")]
        public int GetDriverVehicleAssignmentHistoryByDates(int userId, string SID, int driverId,
                                               long vehicleId, DateTime from, DateTime to, ref string xmlResult)
        {
            try
            {
                Log(">> GetDriverVehicleAssignmentHistoryByDates (uid={0}, vehicleId={1} driverId={2})",
                               userId, vehicleId, driverId);

                // Authenticate
                if (!ValidateUserDriver(userId, SID, driverId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsVehicle = new DataSet();

                    dsVehicle = driver.GetDriverVehicleAssignmentHistory(driverId, vehicleId, from, to);

                    if (Util.IsDataSetValid(dsVehicle))
                        xmlResult = dsVehicle.GetXml();
                    else
                        xmlResult = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetDriverVehicleAssignmentHistoryByDates : uid={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        # endregion

        #region HOS

        [WebMethod(Description = "Add a new Hours of Service")]
        public int AddHoursOfService(int userId, string SID, int driverId, DateTime timestamp,
                                     Int16 state, Int16 cycle, bool IsSecondDay, bool IsCalculated,
                                     bool IsSignedOff, string description, Int16 ruleId, ref int newServiceId)
        {
            try
            {

                Log(">> AddHoursOfService(uid={7}, driverId={0}, timestamp={1}, state={2},cycle={3}, IsSecondDay={4}, IsCalculated={5}, IsSignedOff={6} )",
                             driverId, timestamp, state, cycle, IsSecondDay, IsCalculated, IsSignedOff, userId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = new DataSet();

                    newServiceId = driver.AddHoursOfService(driverId, timestamp, state, cycle, IsSecondDay,
                                             IsCalculated, IsSignedOff, userId, description, ruleId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< AddHoursOfService : uid={0}, driverId={1} timestamp={2}, EXC={3}",
                             userId, driverId, timestamp, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }



        [WebMethod(Description = "Disable HOS record")]
        public int DisableHOS(int userId, string SID, int hoursOfServiceId, int changedByServiceId)
        {
            try
            {
                Log(">> DisableHOS(uid = {2}, hoursOfServiceId={0}, changedByServiceId={1})",
                      hoursOfServiceId, changedByServiceId, userId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);


                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = new DataSet();

                    driver.DisableHOS(hoursOfServiceId, changedByServiceId, userId);

                }
                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< DisableHOS : uid={0}, hoursOfServiceId={1}, EXC={2}",
                           userId, hoursOfServiceId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        [WebMethod(Description = "Validate HOS entry. Check if time/state exists or overlap")]
        public int ValidateHOSentry(int userId, string SID, int driverId, DateTime timestamp,
                                    int stateTypeId, ref int existingHoursOfServiceId, ref Int16 flagOverride)
        {
            try
            {
                Log(">> ValidateHOSentry(uId={2}, driverId={0}, timestamp={1}, stateTypeId={3})",
                         driverId, timestamp, userId, stateTypeId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);


                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = new DataSet();

                    driver.ValidateHOSentry(driverId, timestamp, stateTypeId, ref existingHoursOfServiceId, ref  flagOverride);

                }
                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< ValidateHOSentry : uid={0}, driverId={1}, EXC={2}",
                         userId, driverId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        // Changes for TimeZone Feature start

        [WebMethod(Description = "Get a Hours of Service for driver based on DateTime")]
        public int GetHOSbyDateTime_NewTZ(int userId, string SID, int driverId, DateTime from, DateTime to,
                                    ref string xmlResult)
        {
            try
            {
                Log(">> GetHOSbyDateTime(uId={0}, driverId={1}, dtFrom={2}, dtTo={3})", userId, driverId, from, to);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                //Authorization
                //using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                //{
                //if (!dbUser.ValidateUserDriver(userId, SID, driverId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = driver.GetHOSbyDateTime_NewTZ(driverId, userId, from, to);

                    if (Util.IsDataSetValid(dsDriver))
                        xmlResult = dsDriver.GetXml();
                    else
                        xmlResult = "";
                }
                // }
                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetHOSbyDateTime : uid={0}, driverId={1}, EXC={2}", userId, driverId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        // Changes for TimeZone Feature end

        [WebMethod(Description = "Get a Hours of Service for driver based on DateTime")]
        public int GetHOSbyDateTime(int userId, string SID, int driverId, DateTime from, DateTime to,
                                    ref string xmlResult)
        {
            try
            {
                Log(">> GetHOSbyDateTime(uId={0}, driverId={1}, dtFrom={2}, dtTo={3})", userId, driverId, from, to);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                //Authorization
                //using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                //{
                //if (!dbUser.ValidateUserDriver(userId, SID, driverId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = driver.GetHOSbyDateTime(driverId, userId, from, to);

                    if (Util.IsDataSetValid(dsDriver))
                        xmlResult = dsDriver.GetXml();
                    else
                        xmlResult = "";
                }
                // }
                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetHOSbyDateTime : uid={0}, driverId={1}, EXC={2}", userId, driverId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }


        [WebMethod(Description = "Get a Hours of Service notifications for driver based on DateTime")]
        public int GetHOSnotificationsByDateTime(int userId, string SID, int driverId,
                                                 DateTime from, DateTime to, ref string xmlResult)
        {
            try
            {
                Log(">> GetHOSnotificationsByDateTime(uId={0}, driverId={1}, dtFrom={2}, dtTo={3})",
                                userId, driverId, from, to);


                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                //Authorization
                //using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                //{
                //if (!dbUser.ValidateUserDriver(userId, SID, driverId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = driver.GetHOSnotificationsByDateTime(driverId, userId, from, to);

                    if (Util.IsDataSetValid(dsDriver))
                        xmlResult = dsDriver.GetXml();
                    else
                        xmlResult = "";
                }
                // }
                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetHOSnotificationsByDateTime : uid={0}, driverId={1}, EXC={2}",
                                userId, driverId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }


        [WebMethod(Description = "Get a drivers list by user-fleet assigmnent")]
        public int GetDriversListByUserFleetAssigment(int userId, string SID, ref string xmlResult)
        {
            try
            {
                Log(">>GetDriversListByUserFleetAssigment(uId={0})", userId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                //Authorization
                //using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                //{
                //if (!dbUser.ValidateUserDriver(userId, SID, driverId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = driver.GetDriversListByUserFleetAssigment(userId);

                    if (Util.IsDataSetValid(dsDriver))
                        xmlResult = dsDriver.GetXml();
                    else
                        xmlResult = "";
                }
                //}
                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetDriversListByUserFleetAssigment : uid={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }


        [WebMethod(Description = "Add a new driver assignment to HOS and Sentinel")]
        public int AssignHOSDriver(int userId, string SID, long vehicleId, int driverId, string description)
        {
            try
            {
                Log(">> AssignDriver(uId={0}, vehicleId={1}, driverId={2})", userId, vehicleId, driverId);

                if (!ValidateUserDriver(userId, SID, driverId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    driver.AddHOSDriverAssignment(userId, vehicleId, driverId, description);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< AssignDriver : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }


        [WebMethod(Description = "Delete a driver assignment from HOS as well as Sentinel")]
        public int DeleteHOSDriverAssignment(int userId, string SID, long vehicleId, int driverId, string description)
        {
            try
            {
                Log(">> DeleteDriverAssignment(uId={0}, vehicleId={1}, driverId={2})", userId, vehicleId, driverId);

                if (!ValidateUserDriver(userId, SID, driverId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    driver.DeleteHOSActiveDriverAssignment(userId, vehicleId, driverId, description);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< DeleteDriverAssignment : uId={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }


        #endregion

        #region Driver text Messages
        /// <summary>
        /// Add a Driver text message
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="driverId"></param>
        /// <param name="email"></param>
        /// <param name="msgBody"></param>
        /// <param name="msgDateTime"></param>
        /// <returns></returns>
        [WebMethod(Description = "Add a Driver text message")]
        public int AddDriverMsg(int userId, string SID, int driverId, string email, string msgBody, DateTime msgDateTime)
        {
            try
            {
                Log(">> AddDriverMsg( uId={0}, driverId={1}, email={2}, msgBody={3}, msgDateTime={4})",
                  userId, driverId, email, msgBody, msgDateTime);

                if (!ValidateUserDriver(userId, SID, driverId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    driver.AddDriverMsg(driverId, userId, email, msgBody, msgDateTime);

                    LoggerManager.RecordUserAction("Driver", userId, 0, "vlfDriverMsgs",
                                              string.Format("DriverId={0} AND UserId={1} AND Email='{2}' AND MsgBody='{3}' AND MsgDateTime='{4}'", driverId, userId, email, msgBody, msgDateTime),
                                              "Add", this.Context.Request.UserHostAddress,
                                              this.Context.Request.RawUrl,
                                              string.Format("Add driver({0}) message", driverId));
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddEmail : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Get a drivers text messages")]
        public int GetDriverMsgs(int userId, string SID, int driverId, DateTime fromDate, DateTime toDate, ref string xmlResult)
        {
            try
            {
                Log(">>GetDriverMsgs(uId={0})", userId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                //Authorization
                //using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                //{
                //if (!dbUser.ValidateUserDriver(userId, SID, driverId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (DriverManager driver = new DriverManager(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsDriver = driver.GetDriverMsgs(driverId, fromDate, toDate);

                    if (Util.IsDataSetValid(dsDriver))
                        xmlResult = dsDriver.GetXml();
                    else
                        xmlResult = "";
                }
                //}
                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< GetDriverMsgs : uid={0}, EXC={1}", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        #endregion
    }

    /// <summary>
    /// Each method must be registered in a DB and method Name-Id added to this enum
    /// </summary>
    //public enum DBDriverMethod
    //{
    //   AddDriver = 187,
    //   DeleteDriver = 188,
    //   UpdateDriver = 189,
    //   GetDriver = 193,
    //   GetAllDrivers = 194,
    //   AssignDriver = 195,
    //   DeleteDriverAssignment = 196,
    //   DeleteDriverAssignments = 197,
    //   GetDriverAssignment = 198,
    //   GetDriverAssignments = 199,
    //   GetUnassignedDrivers = 200,
    //   GetAssignedDrivers = 201,
    //   GetDriverAssignmentHistory = 208,
    //   GetDriverAssignmentHistoryByDates = 209,
    //   GetVehicleAssignmentHistory = 210,
    //   GetVehicleAssignmentHistoryByDates = 211,
    //   GetDriverVehicleAssignmentHistory = 212,
    //   GetDriverVehicleAssignmentHistoryByDates = 213,
    //   AddAssignmentHistory = 214,
    //   AssignDriverByDate = 215
    //}
}
