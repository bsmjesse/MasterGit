using System;
using System.Web;
using System.Collections;
using System.Data;
using System.Diagnostics;
using System.Web.Services;
using System.Web.Services.Protocols;
using VLF.DAS.Logic;
using VLF.ERRSecurity;
using VLF.CLS.Def.Structures;


namespace VLF.ASI.Interfaces
{

    /// <summary>
    /// Summary description for DBPersonInfo
    /// </summary>
    [WebService(Namespace = "http://www.sentinelfm.com")]
    public class DBPersonInfo : System.Web.Services.WebService
    {

        public DBPersonInfo()
        {

            //Uncomment the following line if using designed components 
            //InitializeComponent(); 
        }



        #region refactored functions
        /// <summary>
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

        #endregion  refactored functions

        #region PersonInfo functions
        
        /// <summary>
        /// Retrieves all the persons assigned to the organization
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="organizationId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves the list of Persons information, XML Format: [PersonId], [FirstName], [LastName]")]
        public int GetAllPersonsInfoByOrganization(int userId, string SID, int organizationId, ref string xml)
        {
            try
            {
                Log(">> GetAllPersonsInfoByOrganization(userId={0}, organizationId={1}", userId, organizationId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorize
               /// LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                DataSet dsResult = null;

                using (PersonInfo dbSc = new PersonInfo(Application["ConnectionString"].ToString()))
                {
                    dsResult = dbSc.GetAllPersonsInfo(organizationId);
                }

                if (ASIErrorCheck.IsAnyRecord(dsResult))
                    xml = dsResult.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllPersonsInfoByOrganization : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// Retrieves all the persons not assigned to any of the organizations
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves the list of Unassigned Persons information, XML Format: [PersonId], [FirstName], [LastName]")]
        public int GetAllUnassignedPersonsInfo(int userId, string SID, ref string xml)
        {
            try
            {
                Log(">> GetAllUnassignedPersonsInfo(userId={0}", userId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                DataSet dsResult = null;

                using (PersonInfo dbSc = new PersonInfo(Application["ConnectionString"].ToString()))
                {
                    dsResult = dbSc.GetAllUnassignedPersons();
                }

                if (ASIErrorCheck.IsAnyRecord(dsResult))
                    xml = dsResult.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetAllUnassignedPersonsInfo : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="personId"></param>
        /// <param name="xml"></param>
        /// <returns></returns>
        [WebMethod(Description = "Retrieves information for a person using the id, XML Format: [PersonId], [DriverLicense], [FirstName], [LastName], [MiddleName], [Birthday], [Address], [City], [StateProvince], [Country], [PhoneNo1], [PhoneNo2], [CellNo], [LicenseExpDate], [LicenseEndorsements], [Height], [Weight], [Gender], [EyeColor], [HairColor], [IdMarks], [Certifications], [Description]")]
        public int GetPersonInfoById(int userId, string SID, string personId, ref string xml)
        {
            try
            {
                Log(">> GetPersonsInfoById(userId={0}, personId={1}", userId, personId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                // Authorize
                ///LoginManager.GetInstance().AuthorizeOperation(userId, VLF.CLS.Def.Enums.OperationType.Gui, 18);

                DataSet dsResult = null;

                using (PersonInfo dbSc = new PersonInfo(Application["ConnectionString"].ToString()))
                {
                    dsResult = dbSc.GetPersonInfoByPersonId(personId);
                }

                if (ASIErrorCheck.IsAnyRecord(dsResult))
                    xml = XmlUtil.GetXmlIncludingNull(dsResult); //dsResult.GetXml();

                return (int)InterfaceError.NoError;
            }
            catch (Exception Ex)
            {
                LogException("<< GetPersonsInfoById : uId={0}, EXC={1})", userId, Ex.Message);
                return (int)ASIErrorCheck.CheckError(Ex);
            }
        }

        /// <summary>
        /// Create a new person Info, the Id of the new entry is return by the ref personId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="xmlPersonInfo"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        [WebMethod(Description = "Create a new PersonInfo")]
        public int CreatePersonInfo(int userId, string SID, string xmlPersonInfo, ref string personId)
        {
            try
            {
                Log(">> CreatePersonInfo(uId={0}, data={1})", userId, xmlPersonInfo);
                PersonInfoStruct dInfo = (PersonInfoStruct)XmlUtil.FromXml(xmlPersonInfo, typeof(PersonInfoStruct));

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (PersonInfo pi = new PersonInfo(Application["ConnectionString"].ToString()))
                {
                    pi.AddPerson(ref dInfo);
                    personId = dInfo.personId;
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< CreatePersonInfo : uId={0},EXC={1})", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="personId"></param>
        /// <returns></returns>
        [WebMethod(Description = "Delete existing person info")]
        public int DeletePersonByPersonId(int userId, string SID, string personId)
        {
            try
            {
                Log(">> DeletePersonByPersonId(uId={0}, personId={1})", userId, personId);

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (PersonInfo pi = new PersonInfo(Application["ConnectionString"].ToString()))
                {
                    pi.DeletePersonByPersonId(personId);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< DeletePersonByPersonId : uId={0},EXC={1})", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        /// <summary>
        /// Update an existing person info
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="xmlPersonInfo"></param>
        /// <returns></returns>
        [WebMethod(Description = "Update existing person info")]
        public int UpdatePersonInfo(int userId, string SID, string xmlPersonInfo)
        {
            try
            {
                Log(">> UpdatePersonInfo(uId={0}, data={1})", userId, xmlPersonInfo);
                PersonInfoStruct dInfo = (PersonInfoStruct)XmlUtil.FromXml(xmlPersonInfo, typeof(PersonInfoStruct));

                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (PersonInfo pi = new PersonInfo(Application["ConnectionString"].ToString()))
                {
                    pi.UpdateInfo(dInfo);
                }

                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< UpdatePersonInfo : uId={0},EXC={1})", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="SID"></param>
        /// <param name="personIds"></param>
        /// <returns></returns>
        [WebMethod(Description = "Get All Unassigned persons")]
        public int GetAllUnassignedPersonsIds(int userId, string SID, ref string[] personIds)
        {
            ArrayList pIds = null;
            try
            {
                Log(">> GetAllUnassignedPersonsIds(uId={0})", userId);
                
                // Authenticate
                LoginManager.GetInstance().SecurityCheck(userId, SID);

                using (PersonInfo pi = new PersonInfo(Application["ConnectionString"].ToString()))
                {
                    pIds = pi.GetAllUnassignedPersonsIds();
                    personIds = new string[pIds.Count];
                    //personIds = pIds.ToArray(int);
                    personIds = pIds.ToArray(typeof(string)) as string[];
                }
                return (int)InterfaceError.NoError;
            }
            catch (Exception ex)
            {
                LogException("<< UpdatePersonInfo : uId={0},EXC={1})", userId, ex.Message);
                return (int)ASIErrorCheck.CheckError(ex);
            }
        }


        #endregion

    }

}