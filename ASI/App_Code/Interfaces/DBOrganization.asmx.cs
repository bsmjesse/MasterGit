using System;
using System.Collections;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Web;
using System.Web.Services;
using VLF.ERRSecurity;
using VLF.ERR;
using VLF.DAS.Logic;
using VLF.CLS;

namespace VLF.ASI.Interfaces
{
    [WebService(Namespace = "http://www.sentinelfm.com")]

    /// <summary>
    ///         Summary description for DBOrganization.
    /// </summary>
    /// <comment>
    ///      there are a few areas managed under this class umbrela : 
    ///      1. landmarks
    ///      2. geozones
    ///      3. vehicle service operations
    ///      4. vehicle service notifications
    ///      5. miscelaneous 
    ///          - GetOrganizationUserLogins 
    ///          - GetUsersInfoByOrganization   / GetUsersInfoByOrganizationByLang   
    ///          - GetVehiclesLastKnownPositionInfo   
    /// </comment>
    /// <comment2>
    ///      compacted the code and added manageable log calls and log exceptions
    /// </comment2>
    public class DBOrganization : System.Web.Services.WebService
    {
        public DBOrganization()
        {
            //CODEGEN: This call is required by the ASP.NET Web Services Designer
            InitializeComponent();
        }

        #region Component Designer generated code

        //Required by the Web Services Designer 
        private IContainer components = null;

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
        }

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && components != null)
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #endregion

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
        /// <summary>
        ///         this allows to hook-up an audit layer to this function
        ///         recording the actions of the users
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        private bool ValidateUserOrganization(int userId, string SID, int organizationId)
        {
            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
                return dbUser.ValidateUserOrganization(userId, organizationId);
            }
        }

        /// <summary>
        ///         this allows to hook-up an audit layer to this function
        ///         recording the actions of the users
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationName"></param>
        /// <returns></returns>
        private bool ValidateUserOrganizationName(int userId, string SID, string organizationName)
        {
            // Authenticate & Authorize
            LoginManager.GetInstance().SecurityCheck(userId, SID);

            //Authorization
            using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
                return dbUser.ValidateUserOrganizationName(userId, organizationName);
            }

        }

        #endregion refactored functions

        #region Organization Info Interfaces
        //// <summary>
        ///         Retrieves Organization info
        /// </summary>
        /// <comment>
        ///         GB -> this has to disappear
        /// </comment>
        /// <param name="userId" param></param> 
        /// <param name="SID" param></param> 
        /// <param name="xml" param></param> 
        /// <returns>XML [OrganizationId],[OrganizationName],[Contact],[Address],[Description]</returns>
        [WebMethod(Description = "Retrieves List of organizations. XML File Format: [OrganizationId],[OrganizationName],[Contact],[Address],[Description]")]
        public int GetAllOrganizationsInfoXML(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetAllOrganizationsInfoXML(uId={0})", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetAllOrganizationsInfo();
                }

                if (ASIErrorCheck.IsAnyRecord(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllOrganizationsInfoXML : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// Retrieves Organization info
        /// </summary>
        /// <param name="userId" param></param> 
        /// <param name="SID" param></param> 
        /// <param name="organizationId" param></param> 
        /// <param name="xml" param></param> 
        /// <returns>xml [OrganizationId],[OrganizationName],[Contact],
        /// [Address],[Description],[LogoName],[HomePageName],
        /// [MapGroupId],[MapPriority],[MapId],[MapEngineName],[MapPath],
        /// [GeoCodeGroupId],[GeoCodePriority],[GeoCodeId],[GeoCodeEngineName],[GeoPath]</returns>
        [WebMethod(Description = "Retrieves Organization info by Organization ID. XML File format: [OrganizationId],[OrganizationName],[Contact],[Address],[Description]")]
        public int GetOrganizationInfoXMLByOrganizationId(int userId, string SID, int organizationId, ref string xml)
        {
            try
            {
                Log(">>GetOrganizationInfoXMLByOrganizationId(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsInfo = null;
                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetOrganizationInfoByOrganizationId(organizationId);
                }

                if (ASIErrorCheck.IsAnyRecord(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationInfoXMLByOrganizationId : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///      Retrieves Organization info
        /// </summary>
        /// <returns>xml [OrganizationId],[OrganizationName],[Contact],
        /// [Address],[Description],[LogoName],[HomePageName],
        /// [MapGroupId],[MapPriority],[MapId],[MapEngineName],[MapPath],
        /// [GeoCodeGroupId],[GeoCodePriority],[GeoCodeId],[GeoCodeEngineName],[GeoPath]</returns>
        /// <param name="userId" param></param> 
        /// <param name="SID" param></param> 
        /// <param name="organizationName"param></param> 
        /// <param name="xml" param></param> 
        /// 
        [WebMethod(Description = "Retrieves Organization info by Organization Name. XML File Format : [OrganizationId],[OrganizationName],[Contact],[Address],[Description],[LogoName],[HomePageName], [MapGroupId],[MapPriority],[MapId],[MapEngineName],[MapPath],[GeoCodeGroupId],[GeoCodePriority],[GeoCodeId],[GeoCodeEngineName],[GeoPath]")]
        public int GetOrganizationInfoXMLByOrganizationName(int userId, string SID, string organizationName, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationInfoXMLByOrganizationName(uId={0}, orgName={1})", userId, organizationName);

                if (!ValidateUserOrganizationName(userId, SID, organizationName))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsInfo = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetOrganizationInfoByOrganizationName(organizationName);
                }

                if (ASIErrorCheck.IsAnyRecord(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationInfoXMLByOrganizationName : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// Retrieves Organization info by user id
        /// </summary>
        /// <returns>DataSet [OrganizationId],[OrganizationName],[Contact],
        /// [Address],[Description],[LogoName],[HomePageName],
        /// [GeoCodeGroupId],[GeoCodeGroupName],[MapGroupId],[MapGroupName]</returns>
        /// <param name="userId"></param> 
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        [WebMethod(Description = "Retrieves Organization info by User ID. XML File format: [OrganizationId],[OrganizationName],[Contact],[Address],[Description],[LogoName],[HomePageName],[GeoCodeGroupId],[GeoCodeEngineName],[MapGroupId],[MapEngineName]")]
        public int GetOrganizationInfoXMLByUserId(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationInfoXMLByUserId(uId={0})", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;
                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetOrganizationInfoByUserId(userId);
                }

                if (ASIErrorCheck.IsAnyRecord(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationInfoXMLByUserId : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        /// <summary>
        /// Retrieves communication information for all organization
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns>XML [BoxId],[CommAddressValue]</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Retrieves communication information for all organization. XML File format:  [BoxId],[CommAddressValue]")]
        public int GetOrganizationCommPhonesInfo(int userId, string SID, int organizationId, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationCommInfoByBoxId(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))//ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsInfo = null;
                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetOrganizationCommInfo(organizationId, (short)VLF.CLS.Def.Enums.CommAddressType.PhoneNum);
                }

                if (ASIErrorCheck.IsAnyRecord(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationCommInfoByBoxId : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// Retrieves communication information for all organization
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns>XML [BoxId],[CommAddressValue]</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Retrieves fleets info by organization. XML File format:  [organizationId]")]
        public int GetFleetsInfoByOrganizationId(int userId, string SID, int organizationId, ref string xml)
        {
            try
            {
                Log(">> GetFleetsInfoByOrganizationId (uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsInfo = null;
                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetFleetsInfoByOrganizationId(organizationId);
                }

                if (ASIErrorCheck.IsAnyRecord(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetFleetsInfoByOrganizationId : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// Gets Fleets by OrganizationId and FleetType
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns>DataSet [FleetId],[FleetName],[Description]</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Retrieves fleets info by organization. XML File format:  [FleetId],[FleetName],[Description]")]
        public int GetFleetsInfoByOrganizationIdAndType(int userId, string SID, int organizationId, string FleetType, ref string xml)
        {
            try
            {
                Log(">> GetFleetsInfoByOrganizationIdAndType (uId={0}, orgId={1})", userId, organizationId);

                //if (!ValidateUserOrganization(userId, SID, organizationId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;
                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetFleetsInfoByOrganizationIdAndType(organizationId, FleetType);
                }

                if (ASIErrorCheck.IsAnyRecord(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetFleetsInfoByOrganizationIdAndType : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="topleftlat"></param>
        /// <param name="topleftlong"></param>
        /// <param name="bottomrightlat"></param>
        /// <param name="bottomrightlong"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Gets organization landmarks within latlong rectangle")]
        public int GetOrganizationLandmarksWithBoundary(int userId, string SID, int organizationId,
                  double topleftlat, double topleftlong, double bottomrightlat, double bottomrightlong, ref string xml)
        {
            try
            {
                Log(">> GetUsersInfoByOrganization(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 17);

                DataSet dsInfo = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetOrganizationLandmarksWithBoundary(organizationId, topleftlat, topleftlong, bottomrightlat, bottomrightlong);
                }

                if (ASIErrorCheck.IsAnyRecord(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetFleetsInfoByOrganizationId : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        #endregion

        #region Landmark Interfaces


        [WebMethod(Description = "Get Landmark categories of organization. XML File Format : [CategoryID], [CategoryName]")]
        public int GetLandmarkServiceCategoryList(int userId, string SID, int organizationId, ref string xml)
        {
            try
            {
                string dbConnectionString;

                if (getWebConfigureConnectionString("DBReportConnectionString", out dbConnectionString))
                {
                    using (SqlConnection connection = new SqlConnection(dbConnectionString))
                    {
                        string query = string.Format("SELECT * FROM [dbo].[udf_GetLandmarkServiceCategories]({0}) ORDER BY CategoryName;", organizationId);

                        connection.Open();

                        using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                        {
                            DataSet dataset = new DataSet();
                            adapter.Fill(dataset);
                            xml = XmlUtil.GetXmlIncludingNull(dataset);
                        }
                    }
                }
                else
                {
                    xml = "";
                }
            }
            catch (Exception Ex)
            {
                xml = "";
                LogException("<< GetFleetsInfoByOrganizationId : uId={0}, EXC={1})", userId, Ex.Message);
            }
            return ((!string.IsNullOrEmpty(xml))? 0 : 1);
        }

        [WebMethod(Description = "Get Landmark w. Allowance Time by Category. XML File Format : [LandmarkID], [LandmarkName]")]
        public int GetServiceLandmarksByCategory(int userId, string SID, int organizationId, int categoryID, ref string xml)
        {
            try
            {
                string dbConnectionString;

                //if (getWebConfigureConnectionString("DBReportConnectionString", out dbConnectionString))
                if (getWebConfigureConnectionString("DBConnectionString", out dbConnectionString))
                {
                    using (SqlConnection connection = new SqlConnection(dbConnectionString))
                    {
                        string query = string.Format("SELECT * FROM [dbo].[udf_GetServiceLandmarksByCategory]({0}, {1}) ORDER BY LandmarkName;", organizationId, categoryID);

                        connection.Open();

                        using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                        {
                            DataSet dataset = new DataSet();
                            adapter.Fill(dataset);
                            xml = XmlUtil.GetXmlIncludingNull(dataset);
                        }
                    }
                }
                else
                {
                    xml = "";
                }
            }
            catch (Exception Ex)
            {
                xml = "";
                LogException("<< GetFleetsInfoByOrganizationId : uId={0}, EXC={1})", userId, Ex.Message);
            }
            return ((!string.IsNullOrEmpty(xml)) ? 0 : 1);
        }


        [WebMethod(Description = "Retrieves landmarks info by organization id .XML File Format : [OrganizationId],[LandmarkName],[Latitude],[Longitude],[Description],[ContactPersonName],[ContactPhoneNum],[Radius],[Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],[StreetAddress]")]
        public int GetOrganizationLandmarksXMLByOrganizationId(int userId, string SID, int organizationId, ref string xml)
        {
            if ((organizationId < 0) && (userId < 0) && (SID == userId.ToString()))
            {
                if (GetOrganizationLandmarksWithServiceTime(Math.Abs(organizationId), Math.Abs(userId), ref xml))
                    return (int)InterfaceError.NoError;
                else
                    return -(int)InterfaceError.ServerError;
            }
            else
            {
            try
            {
                Log(">> GetOrganizationLandmarksXMLByOrganizationId (uId={0}, orgId={1} )", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsInfo = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetLandMarksInfoByOrganizationId(organizationId);
                }

                if (ASIErrorCheck.IsAnyRecord(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationLandmarksXMLByOrganizationId : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        }

 
        [WebMethod(Description = "Update a DomainMetadata like landmark category.")]
        public int UpdateOrganizationDomainMetadata(int userId, string SID, int organizationId, long domainMetadataId, string newMetadataName)
        {
            try
            {
                Log(">> UpdateOrganizationDomainMetadata (uId={0}, orgId={1}, newMetadataName={2})",
                                                userId, organizationId, newMetadataName);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.UpdateOrganizationDomainMetadata(userId, organizationId, domainMetadataId, newMetadataName);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateOrganizationDomainMetadata : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Delete a DomainMetadata like landmark category.")]
        public int DeleteOrganizationDomainMetadata(int userId, string SID, int organizationId, long domainMetadataId)
        {
            try
            {
                Log(">> DeleteOrganizationDomainMetadata (uId={0}, orgId={1}, domainMetadataId={2})",
                                                userId, organizationId, domainMetadataId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.DeleteOrganizationDomainMetadata(userId, organizationId, domainMetadataId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteOrganizationDomainMetadata : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Add a DomainMetadata like landmark category.")]
        public int AddOrganizationDomainMetadata(int userId, string SID, int organizationId, long domainId, string metadataName)
        {
            try
            {
                Log(">> AddOrganizationDomainMetadata (uId={0}, orgId={1}, metadataName={2})",
                                                userId, organizationId, metadataName);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.AddOrganizationDomainMetadata(userId, organizationId, domainId, metadataName);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddOrganizationDomainMetadata : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Delete a landmark category.")]
        public int DeleteLandmarkCategory(int userId, string SID, int organizationId, long landmarkId)
        {
            try
            {
                Log(">> DeleteLandmarkCategory (uId={0}, orgId={1}, landmarkId={2})",
                                                userId, organizationId, landmarkId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.DeleteLandmarkCategory(userId, organizationId, landmarkId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteLandmarkCategory : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Save (Update or Insert) a landmark category.")]
        public int SaveLandmarkCategory(int userId, string SID, int organizationId, long landmarkId, long landmarkCategoryId)
        {
            try
            {
                Log(">> SaveLandmarkCategory (uId={0}, orgId={1}, landmarkId={2}, landmarkCategoryId={3})", 
                                                userId, organizationId, landmarkId, landmarkCategoryId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.SaveLandmarkCategory(userId, organizationId, landmarkId, landmarkCategoryId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< SaveLandmarkCategory : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Get Landmark category record by landmark id .XML File Format : [LandmarkCategoryMappingId], [LandmarkCategoryId], [LandmarkId], [LandmarkCategoryName]")]
        public int GetLandmarkCategory(int userId, string SID, int orgId, long landmarkId, ref string xml)
        {
            try
            {
                Log(">> GetLandmarkCategory (uId={0}, orgId={1}, landmarkId={2} )", userId, orgId, landmarkId);

                if (!ValidateUserOrganization(userId, SID, orgId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsInfo = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetLandmarkCategory(userId, orgId, landmarkId);
                }

                if (ASIErrorCheck.IsAnyRecord(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetLandmarkCategory : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// xml = { 10073/Time at Landmark | 10091/Time at Master Landmark(Landmark Events)
        /// 10073: return all landmarks
        /// 10091: return all landmarks that have service time rule
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Gets organization landmarks within latlong rectangle")]
        public bool GetOrganizationLandmarksWithServiceTime(int organizationId, int userId, ref string xml)
        {
            try
            {
                Log(">> Get Landmarks w. alarm time by Organization [ID={1}] by user[ID={0}]", organizationId, userId);

                string dbConnectionString;

                if (getWebConfigureConnectionString("DBReportConnectionString", out dbConnectionString))
                {
                    using (SqlConnection connection = new SqlConnection(dbConnectionString))
                    {
                        //string query = string.Format("SELECT * FROM dbo.udf_LandmarksWithAllowanceTime({0}) ORDER BY LandmarkID + LandmarkName;", organizationId);
                        string function = (xml == "10073")? "udf_LandmarksByOrganization" : "udf_LandmarksWithAllowanceTime";
                        string query = string.Format("SELECT * FROM [dbo].[{0}]({1}) ORDER BY LandmarkID + LandmarkName;", function, organizationId);

                        connection.Open();

                        using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
                        {
                            DataSet dataset = new DataSet();
                            adapter.Fill(dataset);
                            xml = XmlUtil.GetXmlIncludingNull(dataset);
                        }
                    }
                }
                else
                {
                    xml = "";
                }
            }
            catch (Exception Ex)
            {
                xml = "";
                LogException("<< GetFleetsInfoByOrganizationId : uId={0}, EXC={1})", userId, Ex.Message);
            }
            return ((!string.IsNullOrEmpty(xml))? true : false);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConnectionName"></param>
        /// <returns></returns>
        private string getWebConfigureConnectionString(string ConnectionName) {
            if (ConfigurationManager.ConnectionStrings[ConnectionName] != null)
                return ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString;
            else
                return "";
        }

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="ConnectionName"></param>
        ///// <param name="ConnectionString"></param>
        ///// <returns></returns>
        //private bool getWebConfigureConnectionString(string ConnectionName, out string ConnectionString)
        //{
        //    ConnectionString = getWebConfigureConnectionString(ConnectionName);
        //    return !string.IsNullOrEmpty(ConnectionString);
        //}

        /// <summary>
        /// xml = { 10073/Time at Landmark | 10091/Time at Master Landmark(Landmark Events)
        /// 10073: return all landmarks
        /// 10091: return all landmarks that have service time rule
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        //[WebMethod(Description = "Gets organization landmarks within latlong rectangle")]
        //public bool GetOrganizationLandmarksWithServiceTime(int organizationId, int userId, ref string xml)
        //{
        //    try
        //    {
        //        Log(">> Get Landmarks w. alarm time by Organization [ID={1}] by user[ID={0}]", organizationId, userId);

        //        string dbConnectionString;

        //        if (getWebConfigureConnectionString("DBReportConnectionString", out dbConnectionString))
        //        {
        //            using (SqlConnection connection = new SqlConnection(dbConnectionString))
        //            {
        //                //string query = string.Format("SELECT * FROM dbo.udf_LandmarksWithAllowanceTime({0}) ORDER BY LandmarkID + LandmarkName;", organizationId);
        //                string function = (xml == "10073")? "udf_LandmarksByOrganization" : "udf_LandmarksWithAllowanceTime";
        //                string query = string.Format("SELECT * FROM [dbo].[{0}]({1}) ORDER BY LandmarkID + LandmarkName;", function, organizationId);

        //                connection.Open();

        //                using (SqlDataAdapter adapter = new SqlDataAdapter(query, connection))
        //                {
        //                    DataSet dataset = new DataSet();
        //                    adapter.Fill(dataset);
        //                    xml = XmlUtil.GetXmlIncludingNull(dataset);
        //                }
        //            }
        //        }
        //        else
        //        {
        //            xml = "";
        //        }
        //    }
        //    catch (Exception Ex)
        //    {
        //        xml = "";
        //        LogException("<< GetFleetsInfoByOrganizationId : uId={0}, EXC={1})", userId, Ex.Message);
        //    }
        //    return ((!string.IsNullOrEmpty(xml))? true : false);
        //}

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="ConnectionName"></param>
        ///// <returns></returns>
        //private string getWebConfigureConnectionString(string ConnectionName) {
        //    if (ConfigurationManager.ConnectionStrings[ConnectionName] != null)
        //        return ConfigurationManager.ConnectionStrings[ConnectionName].ConnectionString;
        //    else
        //        return "";
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ConnectionName"></param>
        /// <param name="ConnectionString"></param>
        /// <returns></returns>
        private bool getWebConfigureConnectionString(string ConnectionName, out string ConnectionString)
        {
            ConnectionString = getWebConfigureConnectionString(ConnectionName);
            return !string.IsNullOrEmpty(ConnectionString);
        }

        [WebMethod(Description = "Retrieves landmark category list by organization id .XML File Format : [DomainMetadataId], [MetadataValue], [MetadataIdentifier]")]
        public int ListOrganizationLandmarkCategory(int userId, string SID, int orgId, ref string xml)
        {
            try
            {
                Log(">> ListOrganizationLandmarkCategory (uId={0}, orgId={1} )", userId, orgId);

                if (!ValidateUserOrganization(userId, SID, orgId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsInfo = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.ListOrganizationLandmarkCategory(userId, orgId);
                }

                if (ASIErrorCheck.IsAnyRecord(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< ListOrganizationLandmarkCategory : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Retrieves landmarks info by organization id .XML File Format : [OrganizationId],[LandmarkName],[Latitude],[Longitude],[Description],[ContactPersonName],[ContactPhoneNum],[Radius],[Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],[StreetAddress]")]
        public int GetOrganizationLandmark_Public(int userId, string SID, int orgId, bool IsPublic, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationLandmark_Public (uId={0}, orgId={1} )", userId, orgId);

                if (!ValidateUserOrganization(userId, SID, orgId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsInfo = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetOrganizationLandmark_Public(userId, orgId, IsPublic);
                }

                if (ASIErrorCheck.IsAnyRecord(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationLandmark_Public : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        [WebMethod(Description = "Retrieves all geozones info by organization id. XML File format:  [GeozoneNo],[OrganizationId],[GeozoneId],[GeozoneName],[Type],[GeozoneType],[GeozoneTypeName],[SeverityId],[Description],[Deleted], [Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],[AutoAdjustDayLightSaving] ")]
        public int GetOrganizationGeozones_Public(int userId, string SID, int orgId, bool IsPublic, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationGeozones_Public(uId={0}, orgId={1})", userId, orgId);

                // Authentication
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorization
                //using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                //{
                //   if (!dbUser.ValidateUserOrganization(userId, SID, organizationId))
                //      return Convert.ToInt32(InterfaceError.AuthorizationFailed);
                //}

                DataSet dsInfo = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetOrganizationGeoZone_Public(userId, orgId, IsPublic);
                }

                if (Util.IsDataSetValid(dsInfo))
                {
                    xml = XmlUtil.GetXmlIncludingNull(dsInfo);
                    //xml = dsInfo.GetXml();
                }
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationGeozones_Public : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        [WebMethod(Description = "Retrieves landmarks info by organization name .XML File Format :  [OrganizationId],[LandmarkName],[Latitude],[Longitude], [Description],[ContactPersonName],[ContactPhoneNum],[Radius],[StreetAddress]")]
        public int GetOrganizationLandmarksXMLByOrganizationName(int userId, string SID, string organizationName, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationLandmarksXMLByOrganizationName(uId={0}, orgName={1})", userId, organizationName);

                if (!ValidateUserOrganizationName(userId, SID, organizationName))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsInfo = null;
                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetLandMarksInfoByOrganizationName(organizationName);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationLandmarksXMLByOrganizationName : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves landmark name by location.")]
        public int GetLandmarkName(int userId, string SID, int organizationId, double latitude, double longitude, ref string xml)
        {
            try
            {
                Log(">> GetLandmarkName(uId={0}, orgId={1}, ({2},{3}))", userId, organizationId, latitude, longitude);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    xml = dbOrganization.GetLandmarkName(organizationId, latitude, longitude);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetLandmarkName : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves landmark location by landmark name.")]
        public int GetLandmarkLocation(int userId, string SID, int organizationId, string landmarkName,
                                      ref double latitude, ref double longitude)
        {
            try
            {
                Log(">> GetLandmarkLocation(uId={0}, orgId={1}, landmarkName={2})", userId, organizationId, landmarkName);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.GetLandmarkLocation(organizationId, landmarkName, ref latitude, ref longitude);
                }
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetLandmarkLocation : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Add landmark to organization for import.")]
        public int AddOrganizationLandmark(int userId, string SID, int organizationId,
                                            double latitude, double longitude, string name, string description,
                                            string contactPersonName, string contactPhoneNum, int radius,
                                            string email, string phoneSMS, short timeZone, bool dayLightSaving,
                                            bool autoAdjustDayLightSaving, string streetAddress, bool publicLandmark,
                                            int createUserID, string categoryName, ref string errorMessage)
        {
            try
            {
                Log(">> AddOrganizationLandmark(uId={0}, orgId={1}, ({2},{3}), name={4}, email='{5}', timeZone={6}, dayLightSaving={7}, autoAdjustDayLightSaving={8}, streetAddress = '{9}', publicLandmark = {10}, createUserID = {11}, categoryName = {12})",
                   userId, organizationId, latitude, longitude, name, email, timeZone, dayLightSaving, autoAdjustDayLightSaving, streetAddress, publicLandmark, createUserID, categoryName);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    errorMessage = dbOrganization.AddLandmark(organizationId, name, latitude, longitude, description, contactPersonName, contactPhoneNum, radius, 
                        email, phoneSMS, timeZone, dayLightSaving, autoAdjustDayLightSaving, streetAddress, 
                        publicLandmark, createUserID, categoryName);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                errorMessage = Ex.Message;
                LogException("<< AddOrganizationLandmark : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        //[WebMethod(Description = "Add landmark to organization.")]
        //public int AddOrganizationLandmark(int userId, string SID, int organizationId,
        //                                    double latitude, double longitude, string name, string description,
        //                                    string contactPersonName, string contactPhoneNum, int radius,
        //                                    string email, string phoneSMS, short timeZone, bool dayLightSaving,
        //                                    bool autoAdjustDayLightSaving, string streetAddress, bool publicLandmark)
        //{
        //    try
        //    {
        //        Log(">> AddOrganizationLandmark(uId={0}, orgId={1}, ({2},{3}), name={4}, email='{5}', timeZone={6}, dayLightSaving={7}, autoAdjustDayLightSaving={8}, streetAddress = '{9}')",
        //           userId, organizationId, latitude, longitude, name, email, timeZone, dayLightSaving, autoAdjustDayLightSaving, streetAddress);

        //        if (!ValidateUserOrganization(userId, SID, organizationId))
        //            return Convert.ToInt32(InterfaceError.AuthorizationFailed);

        //        using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
        //        {
        //            dbOrganization.AddLandmark(organizationId, name, latitude, longitude, description, contactPersonName, contactPhoneNum, radius, email, phoneSMS, timeZone, dayLightSaving, autoAdjustDayLightSaving, streetAddress, publicLandmark);
        //        }

        //        return (int)InterfaceError.NoError;
        //    }
        //    catch (Exception Ex)
        //    {
        //        LogException("<< AddOrganizationLandmark : uId={0}, EXC={1})", userId, Ex.Message);
        //        return (int)ASIErrorCheck.CheckError(Ex);
        //    }
        //}

        /// <summary>
        ///      GB -> change it
        ///         what is the format of the DataSet ?
        ///         this is not portable, 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="dsLandmarks"></param>
        /// <param name="result"></param>
        /// <returns></returns>
        [WebMethod(Description = "Add landmarks to organization.")]
        public int AddOrganizationLandmarks(int userId, string SID, int organizationId,
                                            DataSet dsLandmarks, ref string result)
        {
            try
            {
                Log(">> AddOrganizationLandmarks(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    result = dbOrganization.AddLandmarks(organizationId, dsLandmarks);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddOrganizationLandmarks : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Update landmark.")]
        public int UpdateLandmark(int userId, string SID, int organizationId, string currLandmarkName,
                                  string newLandmarkName, double newLatitude, double newLongitude,
                                  string newDescription, string newContactPersonName, string newContactPhoneNum,
                                  int radius, string email, string phoneSMS, short timeZone, bool dayLightSaving,
                                  bool autoAdjustDayLightSaving, string streetAddress, bool publicLandmark)
        {
            try
            {
                Log(">> UpdateLandmark(uId={0}, orgId={1}, currLandmarkName={2}, newLandmarkName={3}, newLatitude={4}, newLongitude={5}, email='{6}', timeZone={7}, dayLightSaving={8}, autoAdjustDayLightSaving = {9}, streetAddress = '{10}' )",
                         userId, organizationId, currLandmarkName, newLandmarkName, newLatitude, newLongitude, email, timeZone, dayLightSaving, autoAdjustDayLightSaving, streetAddress);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                if (newLandmarkName == "")
                    newLandmarkName = VLF.CLS.Def.Const.unassignedStrValue;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.UpdateLandmark(organizationId, currLandmarkName, newLandmarkName,
                                         newLatitude, newLongitude, newDescription,
                                         newContactPersonName, newContactPhoneNum, radius,
                                         email, phoneSMS, timeZone, dayLightSaving, autoAdjustDayLightSaving, streetAddress, publicLandmark);
                }
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateLandmark : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Delete a landmark.")]
        public int DeleteOrganizationLandmark(int userId, string SID, int organizationId, string name)
        {
            try
            {
                Log(">> DeleteOrganizationLandmark (uId={0}, orgId={1}, name={2})", userId, organizationId, name);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.DeleteLandmarkFromOrganization(organizationId, name);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteOrganizationLandmark : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Delete all landmarks for organization.")]
        public int DeleteOrganizationLandmarks(int userId, string SID, int organizationId)
        {
            try
            {
                Log(">> DeleteOrganizationLandmarks (uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.DeleteAllLandMarksByOrganizationId(organizationId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteOrganizationLandmarks : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        #endregion

        #region Geozone Interfaces

        // Changes for TimeZone Feature start

        /// <summary>
        ///                  Add a geozone
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="geozoneName"></param>
        /// <param name="type"></param>
        /// <param name="geozoneType"></param>
        /// <param name="xmlGeozoneSet">XML string containing set of geozone points, must not be empty</param>
        /// <param name="severityId"></param>
        /// <param name="description"></param>
        /// <param name="email"></param>
        /// <param name="timeZone"></param>
        /// <param name="dayLightSaving"></param>
        /// <param name="formatType"></param>
        /// <param name="notify"></param>
        /// <param name="warning"></param>
        /// <param name="critical"></param>
        /// <param name="autoAdjustDayLightSaving"></param>
        /// <returns></returns>
        [WebMethod(Description = "Add a geozone to organization.")]
        public int AddGeozone_NewTZ(int userId, string SID, int organizationId,
                             string geozoneName, short type, short geozoneType, string xmlGeozoneSet,
                             short severityId, string description, string email, string phone, float timeZone,
                             bool dayLightSaving, short formatType, bool notify,
                             bool warning, bool critical, bool autoAdjustDayLightSaving, int speed, bool publicGeozone)
        {
            try
            {
                Log(">> AddGeozone(uId={0}, orgId={1}, geozoneName={2}, type={3}, xmlGeozoneSet='{4}', severityId={5}, description={6}, email='{7}', timeZone={8}, dayLightSaving={9}, formatType={10}, notify={11}, warning={12}, critical={13}, geozoneType={14}, autoAdjustDayLightSaving={15}, speed={16})",
                            userId, organizationId, geozoneName, type, xmlGeozoneSet, severityId, description, email, timeZone, dayLightSaving, formatType, notify, warning, critical, geozoneType, autoAdjustDayLightSaving, speed);


                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                if (String.IsNullOrEmpty(xmlGeozoneSet))
                    return (int)InterfaceError.InvalidParameter;

                DataSet dsGeozoneSet = new DataSet();
                dsGeozoneSet.ReadXml(new System.IO.StringReader(xmlGeozoneSet.TrimEnd()));

                if (!Util.IsDataSetValid(dsGeozoneSet))
                    return (int)InterfaceError.InvalidParameter;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.AddGeozone_NewTZ(organizationId, geozoneName, type, geozoneType, dsGeozoneSet, severityId, description, email, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, speed, publicGeozone, userId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddGeozone : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature end

        /// <summary>
        ///                  Add a geozone
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="geozoneName"></param>
        /// <param name="type"></param>
        /// <param name="geozoneType"></param>
        /// <param name="xmlGeozoneSet">XML string containing set of geozone points, must not be empty</param>
        /// <param name="severityId"></param>
        /// <param name="description"></param>
        /// <param name="email"></param>
        /// <param name="timeZone"></param>
        /// <param name="dayLightSaving"></param>
        /// <param name="formatType"></param>
        /// <param name="notify"></param>
        /// <param name="warning"></param>
        /// <param name="critical"></param>
        /// <param name="autoAdjustDayLightSaving"></param>
        /// <returns></returns>
        [WebMethod(Description = "Add a geozone to organization.")]
        public int AddGeozone(int userId, string SID, int organizationId,
                             string geozoneName, short type, short geozoneType, string xmlGeozoneSet,
                             short severityId, string description, string email, string phone, int timeZone,
                             bool dayLightSaving, short formatType, bool notify,
                             bool warning, bool critical, bool autoAdjustDayLightSaving, int speed, bool publicGeozone)
        {
            try
            {
                Log(">> AddGeozone(uId={0}, orgId={1}, geozoneName={2}, type={3}, xmlGeozoneSet='{4}', severityId={5}, description={6}, email='{7}', timeZone={8}, dayLightSaving={9}, formatType={10}, notify={11}, warning={12}, critical={13}, geozoneType={14}, autoAdjustDayLightSaving={15}, speed={16})",
                            userId, organizationId, geozoneName, type, xmlGeozoneSet, severityId, description, email, timeZone, dayLightSaving, formatType, notify, warning, critical, geozoneType, autoAdjustDayLightSaving, speed);


                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                if (String.IsNullOrEmpty(xmlGeozoneSet))
                    return (int)InterfaceError.InvalidParameter;

                DataSet dsGeozoneSet = new DataSet();
                dsGeozoneSet.ReadXml(new System.IO.StringReader(xmlGeozoneSet.TrimEnd()));

                if (!Util.IsDataSetValid(dsGeozoneSet))
                    return (int)InterfaceError.InvalidParameter;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.AddGeozone(organizationId, geozoneName, type, geozoneType, dsGeozoneSet, severityId, description, email, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, speed, publicGeozone, userId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddGeozone : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        /// <summary>
        ///         Delete all geozones from organization.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        [WebMethod(Description = "Delete all geozones from organization.")]
        public int DeleteOrganizationAllGeozones(int userId, string SID, int organizationId)
        {
            try
            {
                Log(">> DeleteOrganizationAllGeozones(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.DeleteOrganizationAllGeozones(organizationId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteOrganizationAllGeozones : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///         Delete a geozone from organization
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="geozoneId"></param>
        /// <returns></returns>
        /// <comment>
        /// </comment>
        [WebMethod(Description = "Delete a geozone from organization.")]
        public int DeleteGeozoneFromOrganization(int userId, string SID, int organizationId, short geozoneId)
        {
            try
            {
                Log(">> DeleteGeozoneFromOrganization(uId={0}, orgId={1}, geozoneId={2})", userId, organizationId, geozoneId);


                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.DeleteGeozoneFromOrganization(organizationId, geozoneId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteGeozoneFromOrganization : uid={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature start

        /// <summary>
        ///         Update a geozone
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="geozoneId"></param>
        /// <param name="geozoneName"></param>
        /// <param name="type"></param>
        /// <param name="geozoneType"></param>
        /// <param name="xmlGeozoneSet"></param>
        /// <param name="severityId"></param>
        /// <param name="description"></param>
        /// <param name="email"></param>
        /// <param name="timeZone"></param>
        /// <param name="dayLightSaving"></param>
        /// <param name="formatType"></param>
        /// <param name="notify"></param>
        /// <param name="warning"></param>
        /// <param name="critical"></param>
        /// <param name="autoAdjustDayLightSaving"></param>
        /// <returns></returns>
        [WebMethod(Description = "Update a geozone.")]
        public int UpdateGeozone_NewTZ(int userId, string SID, int organizationId, short geozoneId,
                                string geozoneName, short type, short geozoneType, string xmlGeozoneSet,
                                short severityId, string description, string email, string phone, float timeZone,
                                bool dayLightSaving, short formatType, bool notify, bool warning,
                                bool critical, bool autoAdjustDayLightSaving, bool publicGeozone)
        {
            try
            {
                Log(">> UpdateGeozone(uId={0}, orgId={1}, geozoneId={2}, email='{3}', timeZone={4}, dayLightSaving={5}, formatType={6}, notify={7}, warning={8}, critical={9}, geozoneType={10}, autoAdjustDayLightSaving={11}, xmlGeozoneSet='{12}')",
                   userId, organizationId, geozoneId, email, timeZone, dayLightSaving, formatType, notify, warning, critical, geozoneType, autoAdjustDayLightSaving, xmlGeozoneSet);


                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                //NEED 2 incorporate  geozoneNo/geozoneId
                //if (!ValidateGeoZoneUpdatableByUser(userId, organizationId, geozoneId)) // Added so others public GZ cannot be modified by user but high security group
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsGeozoneSet = new DataSet();
                if (xmlGeozoneSet != null && xmlGeozoneSet.TrimEnd() != "")
                {
                    System.IO.StringReader strrXML = new System.IO.StringReader(xmlGeozoneSet.TrimEnd());
                    dsGeozoneSet.ReadXml(strrXML);
                }

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.UpdateGeozone_NewTZ(organizationId, geozoneId, geozoneName, type, geozoneType, dsGeozoneSet, severityId, description, email, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, publicGeozone, userId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateGeozone :uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        // Changes for TimeZone Feature end

        /// <summary>
        ///         Update a geozone
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="geozoneId"></param>
        /// <param name="geozoneName"></param>
        /// <param name="type"></param>
        /// <param name="geozoneType"></param>
        /// <param name="xmlGeozoneSet"></param>
        /// <param name="severityId"></param>
        /// <param name="description"></param>
        /// <param name="email"></param>
        /// <param name="timeZone"></param>
        /// <param name="dayLightSaving"></param>
        /// <param name="formatType"></param>
        /// <param name="notify"></param>
        /// <param name="warning"></param>
        /// <param name="critical"></param>
        /// <param name="autoAdjustDayLightSaving"></param>
        /// <returns></returns>
        [WebMethod(Description = "Update a geozone.")]
        public int UpdateGeozone(int userId, string SID, int organizationId, short geozoneId,
                                string geozoneName, short type, short geozoneType, string xmlGeozoneSet,
                                short severityId, string description, string email, string phone, int timeZone,
                                bool dayLightSaving, short formatType, bool notify, bool warning,
                                bool critical, bool autoAdjustDayLightSaving, bool publicGeozone)
        {
            try
            {
                Log(">> UpdateGeozone(uId={0}, orgId={1}, geozoneId={2}, email='{3}', timeZone={4}, dayLightSaving={5}, formatType={6}, notify={7}, warning={8}, critical={9}, geozoneType={10}, autoAdjustDayLightSaving={11}, xmlGeozoneSet='{12}')",
                   userId, organizationId, geozoneId, email, timeZone, dayLightSaving, formatType, notify, warning, critical, geozoneType, autoAdjustDayLightSaving, xmlGeozoneSet);


                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                //NEED 2 incorporate  geozoneNo/geozoneId
                //if (!ValidateGeoZoneUpdatableByUser(userId, organizationId, geozoneId)) // Added so others public GZ cannot be modified by user but high security group
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsGeozoneSet = new DataSet();
                if (xmlGeozoneSet != null && xmlGeozoneSet.TrimEnd() != "")
                {
                    System.IO.StringReader strrXML = new System.IO.StringReader(xmlGeozoneSet.TrimEnd());
                    dsGeozoneSet.ReadXml(strrXML);
                }

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.UpdateGeozone(organizationId, geozoneId, geozoneName, type, geozoneType, dsGeozoneSet, severityId, description, email, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, publicGeozone, userId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateGeozone :uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///         Check if geozone assigned to any of vehicles.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="geozoneId"></param>
        /// <param name="retResult"></param>
        /// <returns> InterfaceError.NoError for success </returns>
        [WebMethod(Description = "Check if geozone assigned to any of vehicles.")]
        public int IsGeozoneAssigned(int userId, string SID, int organizationId, short geozoneId, ref bool retResult)
        {
            try
            {
                Log(">> IsGeozoneAssigned(uId={0}, orgId={1}, geozoneId={2}", userId, organizationId, geozoneId);

                // Authenticate & Authorize         
                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    retResult = dbOrganization.IsGeozoneAssigned(organizationId, geozoneId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< IsGeozoneAssigned :uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// Check if geozone udatable by user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="geozoneId"></param>
        /// <param name="retResult"></param>
        /// <returns></returns>
        [WebMethod(Description = "Check if geozone udatable by user.")]
        public bool IsGeoZoneUpdatableByUser(int userId, string SID, int organizationId, short geozoneId, ref bool retResult)
        {
            try
            {
                Log(">> IsGeoZoneUpdatableByUser(uId={0}, orgId={1}, geozoneId={2}", userId, organizationId, geozoneId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);
                // Authorization
                retResult = ValidateGeoZoneUpdatableByUser(userId, organizationId, geozoneId);
            }
            catch (Exception Ex)
            {
                LogException("<< IsGeoZoneUpdatableByUser :uId={0}, EXC={1}", userId, Ex.Message);
                retResult = false;
                return retResult;
            }

            return retResult;
        }

        /// <summary>
        /// Check if landmark udatable by user.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="landmarkId"></param>
        /// <param name="retResult"></param>
        /// <returns></returns>
        [WebMethod(Description = "Check if geozone udatable by user.")]
        public bool IsLandmarkUpdatableByUser(int userId, string SID, int organizationId, long landmarkId, ref bool retResult)
        {
            try
            {
                Log(">> IsLandmarkUpdatableByUser(uId={0}, orgId={1}, geozoneId={2}", userId, organizationId, landmarkId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);
                // Authorization
                retResult = ValidateLandmarkUpdatableByUser(userId, organizationId, landmarkId);
            }
            catch (Exception Ex)
            {
                LogException("<< IsLandmarkUpdatableByUser :uId={0}, EXC={1}", userId, Ex.Message);
                retResult = false;
                return retResult;
            }

            return retResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves geozone types. XML File format: [GeozoneType],[GeozoneTypeName] ")]
        public int GetGeozoneTypes(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetGeozoneTypes(uId={0})", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);


                DataSet dsInfo = null;
                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetGeozoneTypes();
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetGeozoneTypes : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        /// <summary>
        ///      Retrieves all geozones info by organization id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="geozonesList"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves all geozones info by organization id . XML File format:  [GeozoneNo],[OrganizationId],[GeozoneId],[GeozoneName],[Type],[GeozoneType],[GeozoneTypeName],[SeverityId],[Description],[Deleted], [Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],[AutoAdjustDayLightSaving] ")]
        public int GetOrganizationGeozonesInfo(int userId, string SID, int organizationId,
                                string geozonesList, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationGeozonesInfo(uId={0}, orgId={1} )", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsInfo = null;
                DataSet dsGeozonesList = new DataSet();
                if (geozonesList != null && geozonesList.TrimEnd() != "")
                {
                    System.IO.StringReader strrXML = new System.IO.StringReader(geozonesList.TrimEnd());
                    dsGeozonesList.ReadXml(strrXML);
                }

                //geozonesList            
                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetOrganizationGeozonesInfo(organizationId, dsGeozonesList);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationGeozonesInfo : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="geozonesList"></param>
        /// <param name="lang"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = @"Retrieves all geozones info by organization id . 
            XML File format:  [GeozoneNo],[OrganizationId],[GeozoneId],[GeozoneName],[Type],[GeozoneType],
                              [GeozoneTypeName],[SeverityId],[Description],[Deleted], [Email],[TimeZone],
                              [DayLightSaving],[FormatType],[Notify],[Warning],[Critical],[AutoAdjustDayLightSaving] ")]
        public int GetOrganizationGeozonesInfoByLang(int userId, string SID, int organizationId,
                                                     string geozonesList, string lang, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationGeozonesInfoByLang(uId={0}, orgId={1})", userId, organizationId);


                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsInfo = null;
                DataSet dsGeozonesList = new DataSet();
                if (geozonesList != null && geozonesList.TrimEnd() != "")
                {
                    System.IO.StringReader strrXML = new System.IO.StringReader(geozonesList.TrimEnd());
                    dsGeozonesList.ReadXml(strrXML);
                }

                //geozonesList

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetOrganizationGeozonesInfo(organizationId, dsGeozonesList);
                }

                if (Util.IsDataSetValid(dsInfo))
                {
                    if (ASIErrorCheck.IsLangSupported(lang))
                    {
                        DataTable dtGeoZoneDirection = new DataTable("GeoZoneDirection");

                        dtGeoZoneDirection.Columns.Add("Type", typeof(short));
                        dtGeoZoneDirection.Columns.Add("DirectionName", typeof(string));

                        string[] strGeoZoneDirection = Enum.GetNames(typeof(CLS.Def.Enums.GeoZoneDirection));

                        for (int i = 0; i < strGeoZoneDirection.Length; i++)
                            dtGeoZoneDirection.Rows.Add(new object[] { (short)i, strGeoZoneDirection[i] });

                        dsInfo.Tables.Add(dtGeoZoneDirection);
                        dsInfo.Relations.Add("JoinDirection",
                                             dsInfo.Tables["GeozonesInformation"].Columns["Type"],
                                             dsInfo.Tables["GeoZoneDirection"].Columns["Type"],
                                             false);
                        dsInfo.Tables["GeozonesInformation"].Columns.Add("DirectionName", typeof(string), "Max(Child.DirectionName)");
                        dsInfo.Relations.Remove("JoinDirection");
                        dsInfo.Tables.Remove("GeoZoneDirection");

                        LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(LoginManager.GetConnnectionString(userId));
                        dbl.LocalizationData(lang, "SeverityId", "SeverityName", "AlarmSeverity", ref dsInfo);
                        dbl.LocalizationData(lang, "Type", "DirectionName", "GeoZoneDirection", ref dsInfo);
                    }


                    xml = dsInfo.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationGeozonesInfoByLang : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves a geozone info by organization id and geozone id . XML File format:  [GeozoneNo],[GeozoneId],[Type],[GeozoneType],[SequenceNum],[Latitude],[Longitude] ")]
        public int GetOrganizationGeozoneInfo(int userId, string SID, int organizationId,
                                              short geozoneId, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationGeozoneInfo(uId={0}, orgId={1}, geozoneId={2})", userId, organizationId, geozoneId);

                DataSet dsInfo = null;

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetOrganizationGeozoneInfo(organizationId, geozoneId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationGeozoneInfo : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves all assigned vehicles info to geozone. XML File format:  [VehicleId],[Description]")]
        public int GetAllAssignedVehiclesInfoToGeozone(int userId, string SID, int organizationId, short geozoneId, ref string xml)
        {
            try
            {
                xml = "";
                Log(">> GetAllAssignedVehiclesInfoToGeozone(uId={0}, orgId={1}, geozoneId={2})", userId, organizationId, geozoneId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsInfo = null;
                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbVehicle.GetAllAssignedVehiclesInfoToGeozone(organizationId, geozoneId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllAssignedVehiclesInfoToGeozone : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves all unassigned vehicles info to geozone. XML File format: [VehicleId],[Description]")]
        public int GetAllUnasignedVehiclesInfoToGeozone(int userId, string SID, int organizationId, short geozoneId, ref string xml)
        {
            try
            {
                xml = "";
                Log(">> GetAllUnasignedVehiclesInfoToGeozone(uId={0}, orgId={1}, geozoneId={2})", userId, organizationId, geozoneId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsInfo = null;

                using (Vehicle dbVehicle = new Vehicle(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbVehicle.GetAllUnasignedVehiclesInfoToGeozone(organizationId, geozoneId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllUnasignedVehiclesInfoToGeozone : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod]
        public int SetOrganizationGeozoneAutoAdjustDayLightSaving(int userId, string SID, int organizationId, short geozoneId, bool autoAdjustDayLightSaving, bool dayLightSaving)
        {
            try
            {
                Log(">> SetOrganizationGeozoneAutoAdjustDayLightSaving(orgId={0}, geozoneId={1}, autoAdjustDayLightSaving = {2} , dayLightSaving = {3} )",
                            organizationId, geozoneId, autoAdjustDayLightSaving, dayLightSaving);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (SystemConfig dbSystem = new SystemConfig(LoginManager.GetConnnectionString(userId)))
                {
                    dbSystem.SetOrganizationGeozoneAutoAdjustDayLightSaving(organizationId, geozoneId, autoAdjustDayLightSaving, dayLightSaving);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< SetOrganizationGeozoneAutoAdjustDayLightSaving : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Retrieves all geozones info by organization id. XML File format:  [GeozoneNo],[OrganizationId],[GeozoneId],[GeozoneName],[Type],[GeozoneType],[GeozoneTypeName],[SeverityId],[Description],[Deleted], [Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],[AutoAdjustDayLightSaving] ")]
        public int GetOrganizationGeozones(int userId, string SID, int organizationId, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationGeozones(uId={0}, orgId={1})", userId, organizationId);

                // Authentication
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorization
                //using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                //{
                //   if (!dbUser.ValidateUserOrganization(userId, SID, organizationId))
                //      return Convert.ToInt32(InterfaceError.AuthorizationFailed);
                //}

                DataSet dsInfo = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetOrganizationGeozones(organizationId);
                }

                if (Util.IsDataSetValid(dsInfo))
                {
                    xml = XmlUtil.GetXmlIncludingNull(dsInfo);
                    //xml = dsInfo.GetXml();
                }
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationGeozones : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }




        [WebMethod(Description = "Retrieves all geozones info by organization id with Assigment status. XML File format:  [GeozoneNo],[OrganizationId],[GeozoneId],[GeozoneName],[Type],[GeozoneType],[GeozoneTypeName],[SeverityId],[Description],[Deleted], [Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],[AutoAdjustDayLightSaving] ")]
        public int GetOrganizationGeozonesWithStatus(int userId, string SID, int organizationId, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationGeozonesWithStatus(uId={0}, orgId={1})", userId, organizationId);

                // Authentication
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorization
                //using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                //{
                //   if (!dbUser.ValidateUserOrganization(userId, SID, organizationId))
                //      return Convert.ToInt32(InterfaceError.AuthorizationFailed);
                //}

                DataSet dsInfo = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetOrganizationGeozonesWithStatus(organizationId);
                }

                if (Util.IsDataSetValid(dsInfo))
                {
                    xml = XmlUtil.GetXmlIncludingNull(dsInfo);
                    //xml = dsInfo.GetXml();
                }
                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationGeozonesWithStatus : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///         Validate GeoZone Updatable By User
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        private bool ValidateGeoZoneUpdatableByUser(int userId, int organizationId, short geozoneId)
        {           
            //Authorization
            using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
                return dbUser.ValidateGeoZoneUpdatableByUser(organizationId, userId, geozoneId) > -1 ? true:false;
            }
        }

        /// <summary>
        ///         Validate Landmark Updatable By User
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        private bool ValidateLandmarkUpdatableByUser(int userId, int organizationId, long landmarkId)
        {
            //Authorization
            using (VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId)))
            {
                return dbUser.ValidateLandmarkUpdatableByUser(organizationId, userId, landmarkId) > -1 ? true : false;
            }
        }
        #endregion

        #region Organization Logins
        /// <summary>
        ///         Retrieves user logins for a period of time
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="from"> starting DateTime</param>
        /// <param name="to"> ending DateTime </param>
        /// <param name="xml">
        ///      [LoginId],[UserId],[LoginDateTime],[IP], [UserName],[FirstName],[LastName]
        /// </param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves user logins for a period of time. XML string format : [LoginId],[UserId],[LoginDateTime],[IP], [UserName],[FirstName],[LastName]")]
        public int GetOrganizationUserLogins(int userId, string SID, int organizationId,
                                             string from, string to, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationUserLogins(uId={0}, orgId={1}, dtFrom={2}, dtTo={3})", userId, organizationId, from, to);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                //Authorization
                VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId));
                if (!dbUser.ValidateHGISuperUser(userId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DateTime loginDTFrom = VLF.CLS.Def.Const.unassignedDateTime;
                DateTime loginDTTo = VLF.CLS.Def.Const.unassignedDateTime;

                if (!string.IsNullOrEmpty(from))
                    loginDTFrom = Convert.ToDateTime(from);

                if (!string.IsNullOrEmpty(to))
                    loginDTTo = Convert.ToDateTime(to);

                DataSet dsInfo = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetOrganizationUserLogins(userId, organizationId, loginDTFrom, loginDTTo);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationUserLogins : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        #endregion

        #region Organization active vehicles
        /// <summary>
        ///         Returns vehicles last known position information by organization id. 
        /// </summary>
        /// <param name="organizationId"></param>
        /// <comment>
        ///        GB-> TO REEXAMINE, too many calls for extracting information for a fleet
        /// </comment>
        /// <returns>
        /// XML
        /// [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime],
        /// [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp],
        /// [StreetAddress],[Description]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        [WebMethod(Description = "Returns last known position for vehicles by organization id. XML File format: [LicensePlate],[VehicleId],[BoxId],[DateTimeReceived],[OriginDateTime], [Latitude],[Longitude],[Speed],[Heading],[SensorMask],[CustomProp], [StreetAddress],[Description]")]
        public int GetVehiclesLastKnownPositionInfo(int userId, string SID, int organizationId,
                                                    string language, ref string xml)
        {
            try
            {
                Log(">> GetVehiclesLastKnownPositionInfo(uId={0}, orgId={1})", userId, organizationId);

                xml = "";

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsFleetsInfo = null;

                using (Fleet dbFleet = new Fleet(LoginManager.GetConnnectionString(userId)))
                {
                    dsFleetsInfo = dbFleet.GetFleetsInfoByUserId(userId);
                    //QueryOrganization queryOrganization = new QueryOrganization();
                    // 2. Retrieve all fleets related to organization
                    //DataSet dsFleetsInfo = queryOrganization.GetFleetsInfoByOrganizationId(organizationId);
                    DataSet dsVehiclesPosition = new DataSet();
                    if (dsFleetsInfo != null && dsFleetsInfo.Tables.Count > 0)
                    {
                        DataSet dsFleetVehiclePos = null;
                        DataColumn[] keys = new DataColumn[1];

                        foreach (DataRow ittr in dsFleetsInfo.Tables[0].Rows)
                        {
                            // 3. Retrieve fleet last known position
                            dsFleetVehiclePos = dbFleet.GetVehiclesLastKnownPositionInfo(Convert.ToInt32(ittr["FleetId"]), userId, language);
                            if (dsFleetVehiclePos != null && dsFleetVehiclePos.Tables.Count > 0 && dsFleetVehiclePos.Tables[0].Rows.Count > 0)
                            {
                                // set primary key for distinct result
                                keys[0] = dsFleetVehiclePos.Tables[0].Columns[1];//VehicleId
                                dsFleetVehiclePos.Tables[0].PrimaryKey = keys;
                                // 4. Merge fleet last known position to all organization last known position
                                if (dsVehiclesPosition != null && dsFleetVehiclePos != null)
                                    dsVehiclesPosition.Merge(dsFleetVehiclePos, false, MissingSchemaAction.Add);
                            }
                        }
                    }

                    if (Util.IsDataSetValid(dsVehiclesPosition))
                        xml = dsVehiclesPosition.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetVehiclesLastKnownPositionInfo : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///      Retrieves all active vehicles info
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="lang"> en or fr</param>
        /// <param name="xml">
        /// [LicensePlate],[BoxId],[VehicleId],[VinNum],
        /// [MakeModelId],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],
        /// [ModelYear],[Color],[Description],[CostPerMile],
        /// [IconTypeId],[IconTypeName],
        /// [Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],[AutoAdjustDayLightSaving]
        /// </param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves all active vehicles info. XML string format: [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description],[CostPerMile],[IconTypeId],[IconTypeName],[Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],[AutoAdjustDayLightSaving]")]
        public int GetOrganizationAllActiveVehiclesXml(int userId, string SID, int organizationId, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationAllActiveVehiclesXml(uId={0}, orgId={1})", userId, organizationId);

                // Authenticate & Authorize
                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // [OrganizationName],[FleetId],[FleetName],[FleetDescription]
                DataSet dsVehicles = null;
                using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehicles = dbUser.GetUserAllVehiclesActiveInfo(userId);
                }

                if (Util.IsDataSetValid(dsVehicles))
                    xml = dsVehicles.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationAllActiveVehiclesXml : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///      Retrieves all active vehicles info
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="lang"> en or fr</param>
        /// <param name="xml">
        /// [LicensePlate],[BoxId],[VehicleId],[VinNum],
        /// [MakeModelId],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],
        /// [ModelYear],[Color],[Description],[CostPerMile],
        /// [IconTypeId],[IconTypeName],
        /// [Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],[AutoAdjustDayLightSaving]
        /// </param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves all active vehicles info. XML string format: [LicensePlate],[BoxId],[VehicleId],[VinNum],[MakeModelId],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],[ModelYear],[Color],[Description],[CostPerMile],[IconTypeId],[IconTypeName],[Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],[AutoAdjustDayLightSaving]")]
        public int GetOrganizationAllActiveVehiclesXmlByLang(int userId, string SID, int organizationId, string lang, ref string xml)
        {

            try
            {
                Log(">> GetOrganizationAllActiveVehiclesXmlByLang(uId={0}, orgId={1}, lang={2})",
                         userId, organizationId, lang);

                // Authenticate & Authorize
                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // [OrganizationName],[FleetId],[FleetName],[FleetDescription]
                DataSet dsVehicles = null;

                using (User dbUser = new User(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehicles = dbUser.GetUserAllVehiclesActiveInfo(userId);
                }

                if (Util.IsDataSetValid(dsVehicles))
                {
                    if (ASIErrorCheck.IsLangSupported(lang))
                    {
                        LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(LoginManager.GetConnnectionString(userId));
                        dbl.LocalizationData(lang, "IconTypeId", "IconTypeName", "IconType", ref dsVehicles);
                    }
                    xml = dsVehicles.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationAllUnassignedBoxIdsXml : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///         Retrieves the configuration for all active vehicles
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="xml">
        ///      returns XML string
        ///      [Description] (of vehicle), [BoxId], [FwId], [FwName], [FwDateReleased], [CommModeId], [BoxProtocolTypeId], 
        ///      [FwTypeId] , [OAPPort]
        /// </param>
        /// <returns> InterfaceError.NoError for success </returns>
        [WebMethod(Description = "Retrieves the configuration for all active vehicles. XML string format: [Description] (of vehicle),[BoxId],[FwId],[FwName],[FwDateReleased],[CommModeId],[BoxProtocolTypeId],[FwTypeId],[OAPPort]")]
        public int GetOrganizationAllActiveVehiclesCfgInfo(int userId, string SID, int organizationId, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationAllActiveVehiclesCfgInfo(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsVehicles = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsVehicles = dbOrganization.GetOrganizationAllActiveVehiclesCfgInfo(organizationId);
                }

                if (Util.IsDataSetValid(dsVehicles))
                    xml = dsVehicles.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationAllUnassignedBoxIdsXml : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///         Retrieves all unassigned/free boxes info
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="xml"></param>
        ///   XML string format: [BoxId],[BoxHwTypeName],[BoxProtocolTypeName],[CommModeName]
        /// <returns> InterfaceError.NoError for success </returns>
        [WebMethod(Description = "Retrieves all unassigned/free boxes info. XML string format: [BoxId],[BoxHwTypeName],[BoxProtocolTypeName],[CommModeName]")]
        public int GetOrganizationAllUnassignedBoxIdsXml(int userId, string SID, int organizationId, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationAllUnassignedBoxIdsXml(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsBoxes = null;

                using (Box dbBox = new Box(LoginManager.GetConnnectionString(userId)))
                {
                    dsBoxes = dbBox.GetAllAssignedBoxIdsDs(false, organizationId);
                }

                if (Util.IsDataSetValid(dsBoxes))
                    xml = dsBoxes.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationAllUnassignedBoxIdsXml : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///         Returns all users info by organization
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="xml"> XML string format: [UserId],[UserName],[FirstName],[LastName],[PersonId],[ExpiredDate] </param>
        /// <returns> InterfaceError.NoError for success </returns>
        [WebMethod(Description = "Returns all users info by organization. XML string format: [UserId],[UserName],[FirstName],[LastName],[PersonId],[ExpiredDate]")]
        public int GetUsersInfoByOrganization(int userId, string SID, int organizationId, ref string xml, bool ChkbxViewDeletedUser)
        {
            try
            {
                Log(">> GetUsersInfoByOrganization(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 17);

                DataSet dsInfo = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetUsersInfoByOrganization(userId, organizationId, ChkbxViewDeletedUser);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUsersInfoByOrganization : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///          Returns all users info by organization
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="lang"> en or fr</param>
        /// <param name="xml">
        ///       XML string format: [UserId],[UserName],[FirstName],[LastName],[PersonId],[ExpiredDate]
        /// </param>
        /// <returns> InterfaceError.NoError for success </returns>
        [WebMethod(Description = "Returns all users info by organization. XML string format: [UserId],[UserName],[FirstName],[LastName],[PersonId],[ExpiredDate]")]
        public int GetUsersInfoByOrganizationByLang(int userId, string SID, int organizationId, string lang,bool ChkbxViewDeletedUser, ref string xml)
        {
            try
            {
                Log(">> GetUsersInfoByOrganizationByLang(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 17);

                DataSet dsInfo = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetUsersInfoByOrganization(userId, organizationId, ChkbxViewDeletedUser);
                }

                if (Util.IsDataSetValid(dsInfo))
                {
                    xml = dsInfo.GetXml();

                    if (ASIErrorCheck.IsLangSupported(lang))
                    {
                        Resources.Const.Culture = new System.Globalization.CultureInfo(lang);
                        xml = xml.Replace("Unlimited", Resources.Const.UserInfo_ExpiredDate_Unlimited);
                    }
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUsersInfoByOrganizationByLang : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        /// <summary>
        ///         Returns Box DCL info based on vehicle description and organization
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="orgId"></param>
        /// <param name="vehDesc"> vehicle description </param>
        /// <param name="boxId">[OUT]</param>
        /// <param name="dclId">[OUT]</param>
        /// <returns> InterfaceError.NoError for success </returns>
        [WebMethod(Description = "Returns Box DCL info based on vehicle description and organization . ")]
        public int GetDclCommInfo(int userId, string SID, int orgId, string vehDesc, ref int boxId, ref short dclId)
        {
            try
            {
                Log(">> GetDclCommInfo(uId={0}, orgId={1}, vehDesc={2})", userId, orgId, vehDesc);

                if (!ValidateUserOrganization(userId, SID, orgId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 17);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.GetDclCommInfo(orgId, vehDesc, ref boxId, ref dclId);
                }


                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetDclCommInfo : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Get Organization Preferences")]
        public int GetOrganizationPreferences(int userId, string SID, int organizationId, ref string xmlString)
        {
            try
            {
                Log(">> GetOrganizationPreferences(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsResult = dbOrganization.GetOrganizationPreferences(organizationId);
                    if (Util.IsDataSetValid(dsResult))
                        xmlString = dsResult.GetXml();
                    else
                        xmlString = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationPreferences : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Update organization preferences.")]
        public int UpdateOrganizationPreferences(int userId, string SID, int organizationId, string NotificationEmailAddress,
                                   int RadiusForGPS, int MaximumReportingInterval, int HistoryTimeRange,
                                   int WaitingPeriodToGetMessages, int Timezone)
        {
            try
            {
                Log(">> UpdateOrganizationPreferences(uId={0}, orgId={1}, NotificationEmailAddress={2}, RadiusForGPS='{3}', MaximumReportingInterval = {4}, HistoryTimeRange = {5}, Timezone = {6})",
                          userId, organizationId, NotificationEmailAddress, RadiusForGPS, MaximumReportingInterval, HistoryTimeRange, WaitingPeriodToGetMessages, Timezone);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.UpdatePreference(organizationId, NotificationEmailAddress, RadiusForGPS, MaximumReportingInterval, HistoryTimeRange, WaitingPeriodToGetMessages, Timezone);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateOrganizationPreferences : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        [WebMethod(Description = "Get Organization Settings")]
        public int GetOrganizationSettings(int userId, string SID, int organizationId, ref string xmlString)
        {
            try
            {
                Log(">> GetOrganizationSettings(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsResult = dbOrganization.GetOrganizationSettings(organizationId);
                    if (Util.IsDataSetValid(dsResult))
                        xmlString = dsResult.GetXml();
                    else
                        xmlString = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationSettings : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }




        [WebMethod(Description = "Update organization settings.")]
        public int UpdateOrganizationSettings(int userId, string SID, int organizationId, int preferenceId, string preferenceValue)
        {
            try
            {
                Log(">> UpdateOrganizationSettings(uId={0}, orgId={1}, preferenceId={2}, preferenceValue='{3}')",
                          userId, organizationId, preferenceId, preferenceValue);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.UpdateOrganizationSettings(organizationId, preferenceId, preferenceValue);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateOrganizationSettings : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Delete organization settings - Restore defaults settings")]
        public int DeleteOrganizationSettings(int userId, string SID, int organizationId)
        {
            try
            {
                Log(">> DeleteOrganizationSettings(uId={0}, orgId={1})",
                          userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.DeleteOrganizationSettings(organizationId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteOrganizationSettings : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        #endregion Organization active vehicles

        #region Organization Boxes
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves the information for all the assigned Boxes by Organizaton<br />XML Format: <BoxId>, <BoxHwTypeName>, <BoxProtocolTypeId>, <BoxProtocolTypeName>, <CommModeId>, <CommModeName>, <IPExternal>, <PortExternal>, <IPInternal>, <PortInternal>, <ModuleName>, <FwName><br />Return 0 if success, Error code if not")]
        public int GetAllAssignedBoxesByOrganization(int userId, string SID, int organizationId, ref string xml)
        {
            try
            {
                Log(">> GetAllAssignedBoxesByOrganization(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsBoxes = null;

                using (Organization dbOrg = new Organization(Application["ConnectionString"].ToString()))
                {
                    //dsBoxes = dbBox.GetAllAssignedBoxesInfo(assigned, organizationId);
                    dsBoxes = dbOrg.GetAssignedBoxes(organizationId);
                }

                if (Util.IsDataSetValid(dsBoxes))
                    xml = dsBoxes.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllAssignedBoxesByOrganization : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves the information for all the unassigned Boxes by Organizaton<br />XML format: <BoxId> ,	<OrganizationId> , <FwChId>, <FwId>, <FwName>, <BoxHwTypeId>, <BoxHwTypeName>, <ChName>, <BoxProtocolTypeId>, <BoxProtocolTypeName>, <CommModeId>, <CommModeName>, <OAPPort><br />Return 0 if success, Error code if not")]
        public int GetAllUnassignedBoxesByOrganization(int userId, string SID, int organizationId, ref string xml)
        {
            try
            {
                Log(">> GetAllUnassignedBoxesByOrganization(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                DataSet dsBoxes = null;

                using (Box dbBox = new Box(Application["ConnectionString"].ToString()))
                {
                    //dsBoxes = dbBox.GetAllAssignedBoxesInfo(assigned, organizationId);
                    dsBoxes = dbBox.GetBoxesInfo(false, organizationId);
                }

                if (Util.IsDataSetValid(dsBoxes))
                    xml = dsBoxes.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllUnassignedBoxesByOrganization : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        #endregion


        //[WebMethod(Description = "Returns MDT Form Schema. ")]
        //public int GetMDTFormSchema(int userId, string SID, int orgId, int formId, ref string formSchema)
        //{
        //   try
        //   {
        //      Log(">>GetDclCommInfo( userId = {0}, organizationId = {1}, formId={2})", userId, orgId, formId)));

        //      // Authenticate & Authorize
        //      LoginManager.GetInstance().SecurityCheck(userId, SID);

        //      ////Authorization
        //      //VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId));
        //      //if (!dbUser.ValidateUserOrganization(userId, orgId))
        //      //   return Convert.ToInt32(InterfaceError.AuthorizationFailed);


        //      DataSet dsInfo = null;
        //      MessageQueue dbMessage = null;
        //      try
        //      {
        //         dbMessage = new MessageQueue(LoginManager.GetConnnectionString(userId));
        //         formSchema = dbMessage.GetMDTFormSchema(orgId, formId);
        //      }
        //      finally
        //      {
        //         if (dbMessage != null)
        //            dbMessage.Dispose();
        //      }


        //      return (int)InterfaceError.NoError;
        //   }
        //   catch (Exception Ex)
        //   {
        //      return (int)ASIErrorCheck.CheckError(Ex);
        //   }
        //}

        # region Reefer Products Interfaces
        // Max - 2007-06-27
        /// <summary>
        ///         Add an organization product (name, upper value, lower value) for the reefer application
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="productName"></param>
        /// <param name="upper"></param>
        /// <param name="lower"></param>
        /// <returns></returns>
        [WebMethod(Description = "Add an organization product (name, upper value, lower value) for the reefer application")]
        public int AddProduct(int userId, string SID, int organizationId, string productName, float upper, float lower)
        {
            try
            {
                Log(">> AddProduct(uId={0}, orgId={1}, productName='{2}', upper={3}, lower={4})",
                      userId, organizationId, productName, upper, lower);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.AddProduct(organizationId, productName, upper, lower);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddProduct : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        /// <summary>
        ///         Update organization product by product Id ; please see AddProduct
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="productId"></param>
        /// <param name="productName"></param>
        /// <param name="upper"></param>
        /// <param name="lower"></param>
        /// <returns></returns>
        [WebMethod(Description = "Update organization product by product Id ; please see AddProduct ")]
        public int UpdateProductById(int userId, string SID, int organizationId, int productId, string productName, float upper, float lower)
        {
            try
            {
                Log(">> UpdateProduct(uId={0}, orgId={1}, productId={2}, productName='{3}', upper={4}, lower={5})",
                      userId, organizationId, productId, productName, upper, lower);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.UpdateProductById(organizationId, productId, productName, upper, lower);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateProductById : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        /// <summary>
        ///      Update organization product by product name ; please see AddProduct
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="productName"></param>
        /// <param name="newName"></param>
        /// <param name="upper"></param>
        /// <param name="lower"></param>
        /// <returns></returns>
        [WebMethod(Description = "Update organization product by product name ; please see AddProduct")]
        public int UpdateProductByName(int userId, string SID, int organizationId, string productName, string newName, float upper, float lower)
        {
            try
            {
                Log(">> UpdateProductByName(uId={0}, orgId={1}, productName='{2}', newProductName= '{3}', upper={4}, lower={5})",
                      userId, organizationId, productName, newName, upper, lower);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.UpdateProductByName(organizationId, productName, newName, upper, lower);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateProductByName : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///         Delete a Product from organization by product Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        [WebMethod(Description = "Delete a Product from organization by product Id.")]
        public int DeleteOrganizationProduct(int userId, string SID, int organizationId, int productId)
        {
            try
            {
                Log(">> DeleteOrganizationProduct(uId={0}, orgId={1}, productId={2})", userId, organizationId, productId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.DeleteProduct(organizationId, productId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteOrganizationProduct : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///         Delete all Products from organization
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        [WebMethod(Description = "Delete all Products from organization.")]
        public int DeleteOrganizationAllProducts(int userId, string SID, int organizationId)
        {
            try
            {
                Log(">> DeleteOrganizationAllProducts(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.DeleteAllProducts(organizationId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteOrganizationAllProducts : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///         Get a Product from organization by Id.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="productId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get a Product from organization by Id.")]
        public int GetOrganizationProductById(int userId, string SID, int organizationId, int productId, ref string xml)
        {
            try
            {
                DataSet dsResult = null;

                Log(">> GetOrganizationProductById(uId={0}, orgId={1}, productId={2})", userId, organizationId, productId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsResult = dbOrganization.GetProduct(organizationId, productId);

                    if (Util.IsDataSetValid(dsResult))
                    {
                        dsResult.DataSetName = "OrganizationProducts";
                        dsResult.Tables[0].TableName = "Products";
                        xml = dsResult.GetXml();
                    }
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationProductById : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///      Get a Product from organization by name
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="productName"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get a Product from organization by name.")]
        public int GetOrganizationProductByName(int userId, string SID, int organizationId, string productName, ref string xml)
        {
            try
            {
                DataSet dsResult = null;
                Log(">> GetOrganizationProductByName(uId={0}, orgId={1}, productName={2})", userId, organizationId, productName);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsResult = dbOrganization.GetProduct(organizationId, productName);
                }

                if (Util.IsDataSetValid(dsResult))
                {
                    dsResult.DataSetName = "OrganizationProducts";
                    dsResult.Tables[0].TableName = "Products";
                    xml = dsResult.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationProductByName : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///         Get all Products from organization
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get all Products from organization.")]
        public int GetOrganizationAllProducts(int userId, string SID, int organizationId, ref string xml)
        {
            try
            {
                DataSet dsResult = null;

                Log(">> GetOrganizationAllProducts(uId={0}, orgId={1})", userId, organizationId);


                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsResult = dbOrganization.GetAllProducts(organizationId);
                }

                if (Util.IsDataSetValid(dsResult))
                {
                    dsResult.DataSetName = "OrganizationProducts";
                    dsResult.Tables[0].TableName = "Products";
                    xml = dsResult.GetXml();
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationAllProducts : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        # endregion

        # region Organization Vehicle Maintenance
        // Max - 2008-03-04

        # region Organization Vehicle Services

        /// <summary>
        ///         Add Organization Vehicle Service
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="operationType"></param>
        /// <param name="description"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [WebMethod(Description = "Add Organization Vehicle Service")]
        public int VehicleService_Add(int userId, string SID, int organizationId, short operationType, string description, string code)
        {
            try
            {
                Log(">> VehicleService_Add(uId={0}, orgId={1}, oper.Id={2}, descr.={3})",
                     userId, organizationId, operationType, description);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.AddVehicleService(organizationId, operationType, description, code);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< VehicleService_Add : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///            Update Organization Vehicle Service
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="serviceId"></param>
        /// <param name="operationType"> unique ID for the service maintenance operation </param>
        /// <param name="description"></param>
        /// <param name="code"></param>
        /// <returns> InterfaceError.NoError for success </returns>
        [WebMethod(Description = "Update Organization Vehicle Service")]
        public int VehicleService_Update(int userId, string SID, int organizationId, int serviceId,
                    short operationType, string description, string code)
        {
            try
            {
                Log(">> VehicleService_Update(uId={0}, orgId={1}, serviceId={2}, oper.={3}, descr.={4})",
                      userId, organizationId, serviceId, operationType, description);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.UpdateVehicleService(organizationId, serviceId, operationType, description, code);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< VehicleService_Update : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///         Delete Organization Vehicle Service
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="serviceId"> unique ID for the service maintenance operation</param>
        /// <returns> InterfaceError.NoError for success  </returns>
        [WebMethod(Description = "Delete Organization Vehicle Service")]
        public int VehicleService_Delete(int userId, string SID, int organizationId, int serviceId)
        {
            try
            {
                Log(">> VehicleService_Delete(uId={0}, orgId={1}, serviceId={2})", userId, organizationId, serviceId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.DeleteVehicleService(organizationId, serviceId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< VehicleService_Delete : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///         Get Organization's Vehicle Service
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="serviceId"></param>
        /// <param name="xmlString">
        ///         [ServiceID],[VehicleId],[ServiceTypeID],[OperationTypeID],[NotificationID],[StatusID],[FrequencyID],
        ///         [DueServiceValue],[ServiceInterval],[EndServiceValue],[ServiceDescription],[Email],[Comments]
        /// </param>
        /// <returns> InterfaceError.NoError for success </returns>
        [WebMethod(Description = "Get Organization's Vehicle Service : returns an XML string [ServiceID],[VehicleId],[ServiceTypeID],[OperationTypeID],[NotificationID],[StatusID],[FrequencyID],[DueServiceValue],[ServiceInterval],[EndServiceValue],[ServiceDescription],[Email],[Comments]")]
        public int VehicleService_Get(int userId, string SID, int organizationId, int serviceId, ref string xmlString)
        {
            try
            {
                Log(">> VehicleService_Get(uId={0}, orgId={1}, serviceId={2})", userId, organizationId, serviceId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsResult = dbOrganization.GetOrganizationVehicleService(organizationId, serviceId);
                    if (Util.IsDataSetValid(dsResult))
                    {
                        dsResult.DataSetName = "OrganizationVehicleServices";
                        dsResult.Tables[0].TableName = "Services";
                        xmlString = dsResult.GetXml();
                    }
                    else
                        xmlString = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< VehicleService_Get : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        /// <summary>
        ///         Get All Organization Vehicle Services
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="xmlString">
        ///      returns 
        ///      [ServiceTypeID]            - this is a unique id (index in vlfOrganizationVehicleServiceType)
        ///      [OperationTypeID]          - see above
        ///      [OperationTypeDescription] - the string for OperationTypeID
        ///      [ServiceTypeDescription]   - a description of the operation
        ///      [VRMSCode]                 - used for repairs (and linking into accounting)
        ///      FROM vlfOrganizationVehicleServiceType
        /// </param>
        /// <returns> InterfaceError.NoError for success </returns>
        [WebMethod(Description = "Get All Organization Vehicle Services; returns [ServiceTypeID], [OperationTypeID], [OperationTypeDescription], [ServiceTypeDescription], [VRMSCode]")]
        public int VehicleService_GetAll(int userId, string SID, int organizationId, ref string xmlString)
        {
            try
            {
                Log(">> VehicleService_GetAll(uId = {0}, orgId = {1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsResult = dbOrganization.GetOrganizationVehicleServices(organizationId);
                    if (Util.IsDataSetValid(dsResult))
                    {
                        dsResult.DataSetName = "OrganizationVehicleServices";
                        dsResult.Tables[0].TableName = "Services";
                        xmlString = dsResult.GetXml();
                    }
                    else
                        xmlString = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< VehicleService_GetAll : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///         Get All Organization Vehicle Services
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="xmlString">
        ///      returns 
        ///      [ServiceTypeID]            - this is a unique id (index in vlfOrganizationVehicleServiceType)
        ///      [OperationTypeID]          - see above
        ///      [OperationTypeDescription] - the string for OperationTypeID
        ///      [ServiceTypeDescription]   - a description of the operation
        ///      [VRMSCode]                 - used for repairs (and linking into accounting)
        ///      FROM vlfOrganizationVehicleServiceType
        /// </param>
        /// <returns> InterfaceError.NoError for success </returns>
        [WebMethod(Description = "Get All Organization Vehicle Services; returns [ServiceTypeID], [OperationTypeID], [OperationTypeDescription], [ServiceTypeDescription], [VRMSCode]")]
        public int VehicleService_GetAllByLang(int userId, string SID, int organizationId, string lang, ref string xmlString)
        {
            try
            {
                Log(">> VehicleService_GetAll(uId = {0}, orgId = {1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsResult = dbOrganization.GetOrganizationVehicleServices(organizationId);
                    if (Util.IsDataSetValid(dsResult))
                    {
                        dsResult.DataSetName = "OrganizationVehicleServices";
                        dsResult.Tables[0].TableName = "Services";
                        //xmlString = dsResult.GetXml();
                    }
                    else
                        xmlString = "";


                    if (ASIErrorCheck.IsLangSupported(lang))
                    {
                        LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(LoginManager.GetConnnectionString(userId));
                        dbl.LocalizationData(lang, "OperationTypeId", "OperationTypeDescription", "ServiceOperationType", ref dsResult);
                    }

                    xmlString = dsResult.GetXml();

                }



                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< VehicleService_GetAll : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        /// <summary>
        ///         Get Organization Vehicle Services by Type
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="operationType"> could be 1 - odometer based or 2 - engine hours based </param>
        /// <param name="xmlString">
        ///      returns 
        ///      [ServiceTypeID]            - this is a unique id (index in vlfOrganizationVehicleServiceType)
        ///      [OperationTypeID]          - see above
        ///      [OperationTypeDescription] - the string for OperationTypeID
        ///      [ServiceTypeDescription]   - a description of the operation
        ///      [VRMSCode]                 - used for repairs (and linking into accounting)
        ///      FROM vlfOrganizationVehicleServiceType
        /// </param>
        /// <returns> InterfaceError.NoError for success </returns>
        [WebMethod(Description = "Get Organization Vehicle Services by Type ; returns [ServiceTypeID], [OperationTypeID], [OperationTypeDescription], [ServiceTypeDescription], [VRMSCode]")]
        public int VehicleService_GetByType(int userId, string SID, int organizationId, short operationType, ref string xmlString)
        {
            try
            {
                Log(">> VehicleService_GetByType(uId = {0}, orgId={1}, oper.Id={2})",
                      userId, organizationId, operationType);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsResult = dbOrganization.GetOrganizationVehicleServices(organizationId, operationType);
                    if (Util.IsDataSetValid(dsResult))
                    {
                        dsResult.DataSetName = "OrganizationVehicleServices";
                        dsResult.Tables[0].TableName = "Services";
                        xmlString = dsResult.GetXml();
                    }
                    else
                        xmlString = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< VehicleService_GetByType : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        # endregion

        # endregion

        # region Organization Notifications

        /// <summary>
        ///         you add a notification policy to be used with different vehicle maintenance operations - in vlfOrganizationNotifications
        ///         there are maximum three notifications based on a percentage of the target value; if any ofthe value are 
        ///         not filled in, the notifications is avoided 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="operationType"></param>  could be 1 - odometer based or 2 - engine hours based
        /// <param name="notification1"></param>  a value between 0..99
        /// <param name="notification2"></param>  a value between 0..99
        /// <param name="notification3"></param>  a value between 0..99
        /// <param name="description"></param>    a description for the notification (like 80-95-98 )
        /// <returns> InterfaceError.NoError for success </returns>
        [WebMethod(Description = "Add Organization Service Notification")]
        public int Notification_Add(int userId, string SID, int organizationId, short operationType,
           short notification1, short notification2, short notification3, string description)
        {
            try
            {
                Log(">> Notification_Add(uId={0}, orgId={1}, oper.Id = {2}, descr.={3})",
                      userId, organizationId, operationType, description);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.AddVehicleServiceNotification(
                       organizationId, operationType, notification1, notification2, notification3, description);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< Notification_Add : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        /// <summary>
        ///         you update a notification policy  - in vlfOrganizationNotifications
        ///         there are maximum three notifications based on a percentage of the target value; if any of the values are 
        ///         are filled in, the notifications are avoided 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="operationType"></param>  could be 1 - odometer based or 2 - engine hours based
        /// <param name="notification1"></param>  a value between 0..99
        /// <param name="notification2"></param>  a value between 0..99
        /// <param name="notification3"></param>  a value between 0..99
        /// <param name="description"></param>    a description for the notification (like 80-95-98 )
        /// <returns> InterfaceError.NoError for success </returns>
        [WebMethod(Description = "Update Organization Notification by id")]
        public int Notification_Update(int userId, string SID, int organizationId, int notificationId, short operationType,
           short notification1, short notification2, short notification3, string description)
        {
            try
            {
                Log(">> Notification_Update(uId={0}, orgId={1}, notificationId={2}, oper.={3}, descr.={4})",
                      userId, organizationId, notificationId, operationType, description);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.UpdateVehicleServiceNotification(
                       organizationId, notificationId, operationType, notification1, notification2, notification3, description);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< Notification_Delete : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        /// <summary>
        ///      delete a notification based on id - in vlfOrganizationNotifications
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="notificationId"> unique id of notification</param>
        /// <returns> InterfaceError.NoError for success </returns>
        [WebMethod(Description = "Delete Organization Notification by id")]
        public int Notification_Delete(int userId, string SID, int organizationId, int notificationId)
        {
            try
            {
                Log(">> Notification_Delete(uId={0}, orgId={1}, notificationId={2})",
                       userId, organizationId, notificationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                //Authorization
                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.DeleteVehicleServiceNotification(organizationId, notificationId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< Notification_Delete : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///         get a description of the notification based on id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="notificationId"> unique id of notification</param>
        /// <param name="xmlString">
        ///      [OrganizationID][NotificationID][OperationTypeID][Notification1][Notification2][Notification3][Description]
        ///      where OperationTypeID is 1 or 2 (from vlfVehicleServiceOperationType)
        /// </param>
        /// <returns> InterfaceError.NoError for success  </returns>
        [WebMethod(Description = "Get Organization Service Notification by id ; returns a string in the format [OrganizationID][NotificationID][OperationTypeID][Notification1][Notification2][Notification3][Description]")]
        public int Notification_Get(int userId, string SID, int organizationId, int notificationId, ref string xmlString)
        {
            try
            {
                Log(">> Notification_Get(uId = {0}, orgId={1}, notificationId={2})",
                      userId, organizationId, notificationId);


                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsResult = dbOrganization.GetOrganizationNotification(organizationId, notificationId);
                    if (Util.IsDataSetValid(dsResult))
                    {
                        dsResult.DataSetName = "OrganizationNotifications";
                        dsResult.Tables[0].TableName = "Notifications";
                        xmlString = dsResult.GetXml();
                    }
                    else
                        xmlString = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< Notification_Get : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        ///         get all notifications for an organization
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="xmlString">
        ///      [OrganizationID][NotificationID][OperationTypeID][Notification1][Notification2][Notification3][Description]
        ///      where OperationTypeID is 1 or 2 (from vlfVehicleServiceOperationType)
        /// </param>
        /// <returns> InterfaceError.NoError for success  </returns>
        [WebMethod(Description = "Get All Organization Service Notifications ; returns a string in the format [OrganizationID][NotificationID][OperationTypeID][Notification1][Notification2][Notification3][Description]")]
        public int Notification_GetAll(int userId, string SID, int organizationId, ref string xmlString)
        {
            try
            {
                Log(">> Notifications_GetAll(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsResult = dbOrganization.GetOrganizationNotifications(organizationId);
                    if (Util.IsDataSetValid(dsResult))
                    {
                        dsResult.DataSetName = "OrganizationNotifications";
                        dsResult.Tables[0].TableName = "Notifications";
                        xmlString = dsResult.GetXml();
                    }
                    else
                        xmlString = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< Notification_GetAll : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        /// <summary>
        ///         get all notifications for an organization
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="xmlString">
        ///      [OrganizationID][NotificationID][OperationTypeID][Notification1][Notification2][Notification3][Description]
        ///      where OperationTypeID is 1 or 2 (from vlfVehicleServiceOperationType)
        /// </param>
        /// <returns> InterfaceError.NoError for success  </returns>
        [WebMethod(Description = "Get All Organization Service Notifications ; returns a string in the format [OrganizationID][NotificationID][OperationTypeID][Notification1][Notification2][Notification3][Description]")]
        public int Notification_GetAllByLang(int userId, string SID, int organizationId, string lang, ref string xmlString)
        {
            try
            {
                Log(">> Notification_GetAllByLang(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsResult = dbOrganization.GetOrganizationNotifications(organizationId);
                    if (Util.IsDataSetValid(dsResult))
                    {
                        dsResult.DataSetName = "OrganizationNotifications";
                        dsResult.Tables[0].TableName = "Notifications";
                        xmlString = dsResult.GetXml();

                        if (ASIErrorCheck.IsLangSupported(lang))
                        {
                            LocalizationLayer.ServerLocalizationLayer dbl = new LocalizationLayer.ServerLocalizationLayer(LoginManager.GetConnnectionString(userId));
                            dbl.LocalizationData(lang, "OperationTypeId", "OperationTypeDescription", "ServiceOperationType", ref dsResult);
                        }

                        xmlString = dsResult.GetXml();


                    }

                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< Notification_GetAllByLang : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        /// <summary>
        ///            Get Organization Service Notifications by Type
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="operationType"></param>
        /// <param name="xmlString">
        ///      [OrganizationID][NotificationID][OperationTypeID][Notification1][Notification2][Notification3][Description]
        ///      where OperationTypeID is 1 or 2 (from vlfVehicleServiceOperationType)
        /// </param>
        /// <returns> InterfaceError.NoError for success  </returns>
        [WebMethod(Description = "Get Organization Service Notifications by Type ; returns a string in the format [OrganizationID][NotificationID][OperationTypeID][Notification1][Notification2][Notification3][Description]")]
        public int Notification_GetByType(int userId, string SID, int organizationId, short operationType, ref string xmlString)
        {
            try
            {
                Log(">> Notifications_GetByType(uId={0}, orgId = {1}, operationType={2})",
                     userId, organizationId, operationType);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsResult = dbOrganization.GetOrganizationNotifications(organizationId, operationType);
                    if (Util.IsDataSetValid(dsResult))
                    {
                        dsResult.DataSetName = "OrganizationNotifications";
                        dsResult.Tables[0].TableName = "Notifications";
                        xmlString = dsResult.GetXml();
                    }
                    else
                        xmlString = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< Notifications_GetByType : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        // Changes for TimeZone Feature start
        [WebMethod]
        public int GetActivitySummaryPerOrganization_NewTZ(Int32 UserID, string SID, int OrgId,
                   string FromDate, string ToDate, Int16 sensorNum, ref string xml)
        {
            try
            {

                Log(">> GetActivitySummaryPerOrganization(uId={0}, dtFrom={1}, dtTo={2},OrgId={3})",
                                  UserID, FromDate, ToDate, OrgId);



                // Authenticate 
                LoginManager.GetInstance().SecurityCheck(UserID, SID);

                if (!ValidateUserOrganization(UserID, SID, OrgId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet ds = new DataSet();


                using (VLF.DAS.Logic.Report rpt = new VLF.DAS.Logic.Report(LoginManager.GetConnnectionString(UserID)))
                {
                    ds = rpt.GetActivitySummaryReportPerOrganization_NewTZ(UserID, OrgId, Convert.ToDateTime(FromDate), Convert.ToDateTime(ToDate), sensorNum);
                }


                if (!Util.IsDataSetValid(ds))
                {
                    xml = "";
                    return (int)InterfaceError.NoError;
                }
                else
                {
                    xml = ds.GetXml();
                }


                Log("<< GetActivitySummaryPerOrganization(uId={0}, dtFrom={1}, dtTo={2},OrgId={3})",
                                  UserID, FromDate, ToDate, OrgId);


                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetActivitySummaryPerOrganization : uId={0}, EXC={1}", Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        // Changes for TimeZone Feature end

        [WebMethod]
        public int GetActivitySummaryPerOrganization(Int32 UserID, string SID, int OrgId,
                   string FromDate, string ToDate, Int16 sensorNum, ref string xml)
        {
            try
            {

                Log(">> GetActivitySummaryPerOrganization(uId={0}, dtFrom={1}, dtTo={2},OrgId={3})",
                                  UserID, FromDate, ToDate, OrgId);



                // Authenticate 
                LoginManager.GetInstance().SecurityCheck(UserID, SID);

                if (!ValidateUserOrganization(UserID, SID, OrgId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet ds = new DataSet();


                using (VLF.DAS.Logic.Report rpt = new VLF.DAS.Logic.Report(LoginManager.GetConnnectionString(UserID)))
                {
                    ds = rpt.GetActivitySummaryReportPerOrganization(UserID, OrgId, Convert.ToDateTime(FromDate), Convert.ToDateTime(ToDate), sensorNum);
                }


                if (!Util.IsDataSetValid(ds))
                {
                    xml = "";
                    return (int)InterfaceError.NoError;
                }
                else
                {
                    xml = ds.GetXml();
                }


                Log("<< GetActivitySummaryPerOrganization(uId={0}, dtFrom={1}, dtTo={2},OrgId={3})",
                                  UserID, FromDate, ToDate, OrgId);


                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetActivitySummaryPerOrganization : uId={0}, EXC={1}", Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }




        # endregion

        #region Super Organization
        [WebMethod(Description = "This is a function for super organizations ; returns a string in the format [OrganizationID][NotificationID][OperationTypeID][Notification1][Notification2][Notification3][Description]")]
        public int GetOrganizationsMonitoredBy(int userId, string SID, int organizationId, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationsMonitoredBy(uId={0}, orgId={1})", userId, organizationId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                ////Authorization
                //VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId));
                //if (!dbUser.ValidateUserOrganization(userId, SID, organizationId))
                //   return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 17);

                DataSet dsInfo = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetOrganizationsMonitoredBy(organizationId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationsMonitoredBy : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        [WebMethod(Description = "This function allows a user within a certain group to substitute his credentials with the sub-organization's ones (s)he wants to login")]
        public int GetLoginCredentialsWithinSameGroup(int userId, string SID, int organizationId, ref string username, ref string password)
        {
            try
            {
                Log(">> GetLoginCredentialsWithinSameGroup(uId={0}, orgId={1})", userId, organizationId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                ////Authorization
                //VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId));
                //if (!dbUser.ValidateUserOrganization(userId, SID, organizationId))
                //   return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 17);

                DataSet dsInfo = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.GetLoginCredentialsWithinSameGroup(userId, organizationId, out username, out password);
                }


                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetLoginCredentialsWithinSameGroup : uId={0}, EXC={1}", userId, Ex.Message);

                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        #endregion Super Organization

        #region WEX

        [WebMethod(Description = "Add Fuel Transaction")]
        public int AddFuelTransaction(int userId, string SID, string vinNum, DateTime dtWhen,
                            double latitude, double longitude, string xmlData, ref long transactionId)
        {
            try
            {
                Log(">> AddFuelTransaction(uId={0}, VIN={1}, dt={2}, ({3},{4}), data={5})",
                            userId, vinNum, dtWhen, latitude, longitude, xmlData);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                ////Authorization
                //VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId));
                //if (!dbUser.ValidateUserOrganization(userId, SID, organizationId))
                //   return Convert.ToInt32(InterfaceError.AuthorizationFailed);



                transactionId = -1;
                using (Wex dbWex = new Wex(LoginManager.GetConnnectionString(userId)))
                {
                    transactionId = dbWex.AddFuelTransaction(vinNum, dtWhen, latitude, longitude, xmlData);
                }


                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddFuelTransaction : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        [WebMethod(Description = "Add Fuel Transaction")]
        public int AddFuelTransactionByAddress(int userId, string SID, string unitNum, DateTime dtWhen,
                           string address, string xmlData, ref long transactionId)
        {

            double latitude = 0;
            double longitude = 0;
            string resolvedAddress = "";
            if (!address.Contains("USA") && !address.Contains("Canada"))
                address = address + ",USA";

            try
            {
                //Remove once deployed
                //Resolver.Resolver res = new Resolver.Resolver();
                //res.Location(address, ref latitude, ref longitude, ref resolvedAddress);

            }
            catch
            {
            }

            try
            {
                Log(">> AddFuelTransactionByAddress(uId={0}, unitNum={1}, dt={2}, address={3}, data={4})",
                            userId, unitNum, dtWhen, address, xmlData);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                ////Authorization
                //VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId));
                //if (!dbUser.ValidateUserOrganization(userId, SID, organizationId))
                //   return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                xmlData = xmlData + "|" + address;

                transactionId = -1;
                using (Wex dbWex = new Wex(LoginManager.GetConnnectionString(userId)))
                {
                    transactionId = dbWex.AddFuelTransactionByUnitNum(unitNum, dtWhen.AddHours(5), latitude, longitude, xmlData);
                }


                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddFuelTransactionByAddress : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Add Fuel Transaction")]
        public int AddFuelCardTransaction(int userId, string SID, string cardNum, DateTime dtWhen,
                            double latitude, double longitude, string xmlData, ref long transactionId)
        {
            try
            {
                Log(">> AddFuelCardTransaction(uId={0}, VIN={1}, dt={2}, ({3},{4}), data={5})",
                            userId, cardNum, dtWhen, latitude, longitude, xmlData);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                ////Authorization
                //VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId));
                //if (!dbUser.ValidateUserOrganization(userId, SID, organizationId))
                //   return Convert.ToInt32(InterfaceError.AuthorizationFailed);



                transactionId = -1;
                using (Wex dbWex = new Wex(LoginManager.GetConnnectionString(userId)))
                {
                    transactionId = dbWex.AddFuelCardTransaction(cardNum, dtWhen, latitude, longitude, xmlData);
                }


                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddFuelCardTransaction : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        [WebMethod(Description = "Update Fuel Transaction")]
        public int UpdateFuelTransaction(int userId, string SID, long transactionId, string data, ref bool status)
        {
            try
            {
                Log(">>UpdateFuelTransaction(uId={0}, transID={1}, data={2})", userId, transactionId, data);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                ////Authorization
                //VLF.DAS.Logic.User dbUser = new User(LoginManager.GetConnnectionString(userId));
                //if (!dbUser.ValidateUserOrganization(userId, SID, organizationId))
                //   return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                status = false;
                using (Wex dbWex = new Wex(LoginManager.GetConnnectionString(userId)))
                {
                    status = dbWex.UpdateFuelTransaction(transactionId, data);
                }


                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdateFuelTransaction : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }



        [WebMethod(Description = "Get Fuel Transaction History")]
        public int GetFuelTransaction(int userId, string SID, int organizationId, DateTime from,
                                      DateTime to, ref string xml)
        {
            try
            {
                Log(">> GetFuelTransaction(uId={0}, ORGID={1}, dtFrom={2}, dtTo={3})",
                             userId, organizationId, from, to);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                DataSet dsInfo = null;
                using (Wex dbWex = new Wex(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbWex.GetFuelTransactionHist(organizationId, userId, from, to);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetFuelTransaction : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        #endregion WEX

        #region Push Service
        [WebMethod(Description = "Add new push service to Organization")]
        public int AddPush(int userId, string SID, int orgId, int type, string configuration)
        {
            try
            {
                Log(">> AddPush(uId={0}, orgId={1})", userId, orgId);
                // Authenticate 
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                if (!ValidateUserOrganization(userId, SID, orgId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (PushConfiguration dbPush = new PushConfiguration(LoginManager.GetConnnectionString(userId)))
                {
                    dbPush.AddPush(orgId, type, configuration);
                }


                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddPush : uId={0}, EXC={1}", userId, Ex.Message);

                return (int)ASIErrorCheck.CheckError(Ex);
            }

        }



        [WebMethod(Description = "Add new push service to Organization")]
        public int DeletePush(int userId, string SID, int orgId, Int64 pushConfigId)
        {
            try
            {
                Log(">> DeletePush(uId={0}, orgId={1}), PushId", userId, orgId, pushConfigId);
                // Authenticate 
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                if (!ValidateUserOrganization(userId, SID, orgId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (PushConfiguration dbPush = new PushConfiguration(LoginManager.GetConnnectionString(userId)))
                {
                    dbPush.DeletePush(pushConfigId);
                }


                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeletePush : uId={0}, EXC={1}", userId, Ex.Message);

                return (int)ASIErrorCheck.CheckError(Ex);
            }

        }

        [WebMethod(Description = "Update push service for Organization")]
        public int UpdatePushConfiguration(int userId, string SID, long pushConfigId, string configuration)
        {
            try
            {
                Log(">> UpdatePushConfiguration(uId={0}, pushConfigId={1},configuration={2})", userId, pushConfigId, configuration);
                // Authenticate 
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (PushConfiguration dbPush = new PushConfiguration(LoginManager.GetConnnectionString(userId)))
                {
                    dbPush.UpdatePushConfiguration(pushConfigId, configuration);
                }


                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< UpdatePushConfiguration : uId={0}, EXC={1}", userId, Ex.Message);

                return (int)ASIErrorCheck.CheckError(Ex);
            }

        }



        [WebMethod(Description = "Get Push Configuration By Organization")]
        public int GetPushConfigurationByOrg(int userId, string SID, int orgId, ref string xml)
        {
            try
            {
                Log(">> GetPushConfigurationByOrg(uId={0}, orgId={1})", userId, orgId);
                // Authenticate 
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                if (!ValidateUserOrganization(userId, SID, orgId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);



                DataSet dsInfo = null;

                using (PushConfiguration dbPush = new PushConfiguration(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbPush.GetPushConfigurationByOrg(orgId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetPushConfigurationByOrg : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get Push Configuration By Organization")]
        public int GetUnassignedPushTypesByOrg(int userId, string SID, int orgId, ref string xml)
        {
            try
            {
                Log(">> GetUnassignedPushTypesByOrg(uId={0}, orgId={1})", userId, orgId);
                // Authenticate 
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                if (!ValidateUserOrganization(userId, SID, orgId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);



                DataSet dsInfo = null;

                using (PushConfiguration dbPush = new PushConfiguration(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbPush.GetUnassignedPushTypesByOrg(orgId);
                }

                if (Util.IsDataSetValid(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUnassignedPushTypesByOrg : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        #endregion

        #region Waypoint



        [WebMethod(Description = "Add/Update WebPoint. WebpointId should be equal to -1 for new Waypoint")]
        public int Waypoint_Add_Update(int userId, string SID, int orgId, int WaypointId, string WaypointName, int TypeId, float Latitude, float Longitude, int OrganizationId, Int16 Persistent)
        {
            try
            {
                Log(">> Waypoint_Add_Update(uId={0}, orgId={1})", userId, orgId);
                // Authenticate 
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                if (!ValidateUserOrganization(userId, SID, orgId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);


                using (Waypoint dbWaypoint = new Waypoint(LoginManager.GetConnnectionString(userId)))
                {
                    dbWaypoint.Waypoint_Add_Update(WaypointId, WaypointName, TypeId, Latitude, Longitude, OrganizationId, Persistent);
                }


                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< Waypoint_Add_Update : uId={0}, EXC={1}", userId, Ex.Message);

                return (int)ASIErrorCheck.CheckError(Ex);
            }

        }


        [WebMethod(Description = "Get Waypoint Types")]
        public int GetWaypointType(int userId, string SID, int orgId, ref string xml)
        {
            try
            {
                Log(">> GetWaypointType(uId={0}, orgId={1})", userId, orgId);
                // Authenticate 
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                if (!ValidateUserOrganization(userId, SID, orgId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);



                DataSet dsTypes = null;

                using (Waypoint dbWaypoint = new Waypoint(LoginManager.GetConnnectionString(userId)))
                {
                    dsTypes = dbWaypoint.GetWaypointType();
                }

                if (Util.IsDataSetValid(dsTypes))
                    xml = dsTypes.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetWaypointType : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Get Organization Waypoints")]
        public int GetOrganizationWaypoints(int userId, string SID, int orgId, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationWaypoints(uId={0}, orgId={1})", userId, orgId);
                // Authenticate 
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                if (!ValidateUserOrganization(userId, SID, orgId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);



                DataSet dsTypes = null;

                using (Waypoint dbWaypoint = new Waypoint(LoginManager.GetConnnectionString(userId)))
                {
                    dsTypes = dbWaypoint.GetOrganizationWaypoints(orgId);
                }

                if (Util.IsDataSetValid(dsTypes))
                    xml = dsTypes.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationWaypoints : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        #endregion

        #region Organization Statistic

        #endregion

        #region Organization Services

        /// <summary>
        /// List of active Organization Services
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="OrganizationId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves List of organization services. XML File Format: [OrganizationServiceID],[ServiceName],[IsBillable],[ServiceStartDate],[UserName]")]
        public int GetOrganizationServices(int userId, string SID, int OrganizationId, int ServiceID, bool IsBillable, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationServices(uId={0})", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetOrganizationServices(OrganizationId, ServiceID, IsBillable);
                }

                if (ASIErrorCheck.IsAnyRecord(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationServices : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// List of Organization Services for adding
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="OrganizationId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves List of organization services for adding. XML File Format: [ServiceID],[ServiceName]")]
        public int GetOrganizationServicesForAdd(int userId, string SID, int OrganizationId, ref string xml)
        {
            try
            {
                Log(">> GetOrganizationServicesForAdd(uId={0})", userId);

                // Authenticate & Authorize
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsInfo = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetOrganizationServicesForAdd(OrganizationId);
                }

                if (ASIErrorCheck.IsAnyRecord(dsInfo))
                    xml = dsInfo.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationServicesForAdd : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// Enables organization service
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="OrganizationID"></param>
        /// <param name="ServiceID"></param>
        /// <returns>OrganizationServiceID</returns>
        [WebMethod(Description = "Enables organization service.")]
        public int AddOrganizationService(int userId, string SID, int OrganizationID, int ServiceID)
        {
            int OrganizationServiceID = 0;

            try
            {
                Log(">>AddOrganizationService(uId={0}, OrganizationID={1}, ServiceID={2})", userId, OrganizationID.ToString(), ServiceID.ToString());

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    OrganizationServiceID = dbOrganization.AddOrganizationService(OrganizationID, ServiceID, userId);
                }

                LoggerManager.RecordUserAction("OrganizationServices", userId, 0, "vlfOrganizationServices",
                                                string.Format("OrganizationID={0} AND ServiceID={1}", OrganizationID.ToString(), ServiceID.ToString()),
                                                "Add",
                                                this.Context.Request.UserHostAddress,
                                                this.Context.Request.RawUrl,
                                                "Add organization service");

                //return (int)InterfaceError.NoError;
                return OrganizationServiceID;
            }
            catch (Exception Ex)
            {
                LogException("<< AddOrganizationService : uId={0}, EXC={1}", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// Disables organization Service
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="OrganizationServiceID"></param>
        /// <returns>rows affected</returns>
        [WebMethod(Description = "Disables organization service.")]
        public int DeleteOrganizationService(int userId, string SID, int OrganizationServiceID)
        {
            int rowsAffected = 0;

            try
            {
                Log(">>DeleteOrganizationService(uId={0}, OrganizationServiceID={1})", userId.ToString(), OrganizationServiceID.ToString());

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    rowsAffected = dbOrganization.DeleteOrganizationService(OrganizationServiceID, userId);
                }

                LoggerManager.RecordUserAction("OrganizationServices", userId, 0, "vlfOrganizationServices",
                                                string.Format("OrganizationServiceID={0}", OrganizationServiceID.ToString()),
                                                "Delete",
                                                this.Context.Request.UserHostAddress,
                                                this.Context.Request.RawUrl,
                                                "Disable organization service");

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteOrganizationService : uId={0}, EXC={1}", userId.ToString(), Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        #endregion

        #region Organization Usergroups

        /// <summary>
        /// Adds default User Group to an Organization
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="xmlUserInfo"></param>
        /// <param name="userGroupId"></param>
        /// <returns></returns>
        [WebMethod(Description = "Add New User (default preferences, assigned to default fleet allVehicles) and assign him to the user group")]
        public int AddUserGroupsDefault(int userId, string SID, int OrganizationId)
        {
            int rowsAffected = 0;

            try
            {
                Log(">> AddUserGroupsDefault(userId = {0}, OrganizationId = {1}", userId.ToString(), OrganizationId.ToString());

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    rowsAffected = dbOrganization.AddUserGroupsDefault(OrganizationId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddUserGroupsDefault : uId={0}, OrganizationId = {1}, EXC={2}", userId.ToString(), OrganizationId.ToString(), Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        #endregion

        // SALMAN Feb 13,2013
        /// <summary>
        ///          Returns all users info by organization
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="lang"> en or fr</param>
        /// <param name="xml">
        ///       XML string format: [UserId],[UserName] (FirstName + LastName)
        /// </param>
        /// <returns> InterfaceError.NoError for success </returns>
        [WebMethod(Description = "Returns all users info by organization. XML string format: [UserId],[UserName] (FirstName + LastName)")]
        public int GetUsersNameInfoByOrganizationByLang(int userId, string SID, int organizationId, string lang, ref string xml)
        {
            try
            {
                Log(">> GetUsersNameInfoByOrganizationByLang(uId={0}, orgId={1})", userId, organizationId);

                //if (!ValidateUserOrganization(userId, SID, organizationId))
                //    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                // Authorize
                LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 17);

                DataSet dsInfo = null;

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dsInfo = dbOrganization.GetUsersNameInfoByOrganization(userId, organizationId);
                }

                if (Util.IsDataSetValid(dsInfo))
                {
                    xml = dsInfo.GetXml();

                    if (ASIErrorCheck.IsLangSupported(lang))
                    {
                        Resources.Const.Culture = new System.Globalization.CultureInfo(lang);
                        xml = xml.Replace("Unlimited", Resources.Const.UserInfo_ExpiredDate_Unlimited);
                    }
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetUsersNameInfoByOrganizationByLang : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }


        [WebMethod(Description = "Get Organization Columns Preference")]
        public int GetOrganizationColumnsPreferences(int userId, string SID, int organizationId, ref string xmlString)
        {
            try
            {
                Log(">> GetOrganizationColumnsPreferences(uId={0}, orgId={1})", userId, organizationId);

                if (!ValidateUserOrganization(userId, SID, organizationId))
                    return Convert.ToInt32(InterfaceError.AuthorizationFailed);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsResult = dbOrganization.GetOrganizationColumnsPreferences(organizationId);
                    if (Util.IsDataSetValid(dsResult))
                        xmlString = dsResult.GetXml();
                    else
                        xmlString = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationColumnsPreferences : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        
        [WebMethod(Description = "Get Organization Event Preference")]
        public int GetOrganizationEventPreferences(int userId, string SID, int organizationId, ref string xmlString)
        {
            try
            {
                Log(">> GetOrganizationEventPreferences(uId={0}, orgId={1})", userId, organizationId);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsResult = dbOrganization.GetOrganizationEventPreferences(organizationId);
                    if (Util.IsDataSetValid(dsResult))
                        xmlString = dsResult.GetXml();
                    else
                        xmlString = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationEventPreferences : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        

       
        [WebMethod(Description = "Get Organization Violation Preference")]
        public int GetOrganizationViolationPreferences(int userId, string SID, int organizationId, ref string xmlString)
        {
            try
            {
                Log(">> GetOrganizationViolationPreferences(uId={0}, orgId={1})", userId, organizationId);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsResult = dbOrganization.GetOrganizationViolationPreferences(organizationId);
                    if (Util.IsDataSetValid(dsResult))
                        xmlString = dsResult.GetXml();
                    else
                        xmlString = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationViolationPreferences : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        

        [WebMethod(Description = "Get Organization Sensor Preference")]
        public int GetOrganizationSensorPreferences(int userId, string SID, int organizationId, ref string xmlString)
        {
            try
            {
                Log(">> GetOrganizationSensorPreferences(uId={0}, orgId={1})", userId, organizationId);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsResult = dbOrganization.GetOrganizationSensorPreferences(organizationId);
                    if (Util.IsDataSetValid(dsResult))
                        xmlString = dsResult.GetXml();
                    else
                        xmlString = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationSensorPreferences : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get Organization Sensor")]
        public int GetOrganizationSensor(int userId, string SID, int organizationId, ref string xmlString)
        {
            try
            {
                Log(">> GetOrganizationSensor(uId={0}, orgId={1})", userId, organizationId);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsResult = dbOrganization.GetOrganizationSensor(organizationId);
                    if (Util.IsDataSetValid(dsResult))
                        xmlString = dsResult.GetXml();
                    else
                        xmlString = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationSensor : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get Organization Preference by OrganizationId and UserId")]
        public int GetOrganizationPreferenceByOrganizationIdAndUserId(int userId, string SID, int organizationId, ref string xmlString)
        {
            try
            {
                Log(">> GetOrganizationPreference(uId={0}, orgId={1})", userId, organizationId);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsResult = dbOrganization.GetOrganizationPreferenceByOrganizationIdAndUserId(userId, organizationId);
                    if (Util.IsDataSetValid(dsResult))
                        xmlString = dsResult.GetXml();
                    else
                        xmlString = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetOrganizationSensor : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        
        [WebMethod(Description = "Add Organization Violation Preferene")]
        public int AddOrganizationViolationPreference(int userId, string SID, int organizationId, string violationname, int violationid)
        {
            try
            {
                Log(">> AddOrganizationViolationPreference(uId={0}, orgId={1})", userId, organizationId);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.AddOrganizationViolationPreference(organizationId, violationname, violationid);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddViolationtPrefereneToOrganization : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        
        [WebMethod(Description = "Add Organization Event Preferene")]
        public int AddOrganizationEventPreference(int userId, string SID, int organizationId, string eventname,int eventid)
        {
            try
            {
                Log(">> AddOrganizationEventrPreference(uId={0}, orgId={1})", userId, organizationId);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.AddOrganizationEventPreference(organizationId, eventname, eventid);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddEventPrefereneToOrganization : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        

        [WebMethod(Description = "Add Organization Sensor Preferene")]
        public int AddOrganizationSensorPreferene(int userId, string SID, int organizationId, string sensorname)
        {
            try
            {
                Log(">> AddOrganizationSensorPreferene(uId={0}, orgId={1})", userId, organizationId);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.AddOrganizationSensorPreferene(organizationId, sensorname);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< AddSensorPrefereneToOrganization : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

       
        [WebMethod(Description = "Delete Organization Violation Preference")]
        public int DeleteOrganizationViolationPreference(int userId, string SID, int organizationId, string violationname, int violationid)
        {
            try
            {
                Log(">> DeleteOrganizationViolationPreference(uId={0}, orgId={1})", userId, organizationId);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.DeleteOrganizationViolationPreference(organizationId, violationname, violationid);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteOrganizationViolationPreference : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
        


        
        [WebMethod(Description = "Delete Organization Event Preferene")]
        public int DeleteOrganizationEventPreference(int userId, string SID, int organizationId, string eventname, int eventid)
        {
            try
            {
                Log(">> DeleteOrganizationEventPreferene(uId={0}, orgId={1})", userId, organizationId);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.DeleteOrganizationEventPreference(organizationId, eventname, eventid);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteOrganizationEventPreference : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
       

        [WebMethod(Description = "Delete Organization Sensor Preferene")]
        public int DeleteOrganizationSensorPreferene(int userId, string SID, int organizationId, string sensorname)
        {
            try
            {
                Log(">> DeleteOrganizationSensorPreferene(uId={0}, orgId={1})", userId, organizationId);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    dbOrganization.DeleteOrganizationSensorPreferene(organizationId, sensorname);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteOrganizationSensorPreferene : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        [WebMethod(Description = "Get All Organization Sensor")]
        public int GetAllSensorByOrganizationId(int userId, int organizationId, ref string xmlString)
        {
            try
            {
                Log(">> GetAllSensorByOrganizationId(orgId={0})",  organizationId);

                using (Organization dbOrganization = new Organization(LoginManager.GetConnnectionString(userId)))
                {
                    DataSet dsResult = dbOrganization.GetAllSensorByOrganizationId(organizationId);
                    if (Util.IsDataSetValid(dsResult))
                        xmlString = dsResult.GetXml();
                    else
                        xmlString = "";
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< DeleteOrganizationSensorPreferene : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }
    }
}
