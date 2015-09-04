using System;
using System.Data;			// for DataSet
using System.Data.SqlClient;	// for SqlException
using VLF.ERR;
using VLF.CLS;
using VLF.DAS.DB;
using System.Collections.Generic;


namespace VLF.DAS.Logic
{
    /// <summary>
    /// Provides interface to organization functionality in database
    /// </summary>
    public partial class Organization : Das
    {
        DB.Landmark landmark = null;
        DB.Organization organization = null;
        DB.DriverAssignment driverAssignment = null;
        DB.OrganizationGeozone organizationGeozone = null;
        DB.UserLogin userLogin = null;
        DB.TemperaturePlan product = null; // 2007-06-27
        DB.OrganizationVehicleServiceType svcType = null; // 2008-01-31
        DB.OrganizationNotifications notification = null; // 2008-01-31
        DB.OrganizationPreferences preferences = null; //2008-12-04

        #region General Interfaces

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="connectionString"></param>
        public Organization(string connectionString)
            : base(connectionString)
        {
            landmark = new DB.Landmark(sqlExec);
            organization = new DB.Organization(sqlExec);
            driverAssignment = new DB.DriverAssignment(sqlExec);
            userLogin = new DB.UserLogin(sqlExec);
            organizationGeozone = new DB.OrganizationGeozone(sqlExec);
            product = new DB.TemperaturePlan(sqlExec); // 2007-06-27
            svcType = new OrganizationVehicleServiceType(this.sqlExec); // 2008-01-31
            notification = new OrganizationNotifications(this.sqlExec); // 2008-01-31
            preferences = new OrganizationPreferences(this.sqlExec); // 2008-12-04
        }

        /// <summary>
        /// Destructor
        /// </summary>
        public new void Dispose()
        {
            base.Dispose();
        }
        #endregion

        #region Organization Interfaces


        /// <summary>
        ///         the new method reflecting a super organization hierarchy
        /// </summary>
        /// <param name="superOrganizationId"></param>
        /// <param name="organizationName"></param>
        /// <param name="contact"></param>
        /// <param name="address"></param>
        /// <param name="description"></param>
        /// <param name="logoName"></param>
        /// <param name="homePageName"></param>
        /// <param name="mapGroupId"></param>
        /// <param name="geoCodeGroupId"></param>
        /// <returns></returns>
        public int AddOrganization(int superOrganizationId, string organizationName, string contact, string address, string description, string logoName, string homePageName, int mapGroupId, int geoCodeGroupId)
        {
            int organizationId = VLF.CLS.Def.Const.unassignedIntValue;
            try
            {
                // 1. Begin transaction
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                // 2. Add new organization
                organizationId = organization.AddOrganization(superOrganizationId, organizationName, contact, address, description, logoName, homePageName, mapGroupId, geoCodeGroupId);
                // 3. Add default fleet to organization
                DB.Fleet fleet = new DB.Fleet(sqlExec);
                fleet.AddFleet(VLF.CLS.Def.Const.defaultFleetName, organizationId, "");
                // 4. Save all changes
                sqlExec.CommitTransaction();
            }
            catch (SqlException objException)
            {
                // Rollback all changes
                organizationId = VLF.CLS.Def.Const.unassignedIntValue;
                sqlExec.RollbackTransaction();
                Util.ProcessDbException("Cannot add the organization " + organizationName, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                sqlExec.RollbackTransaction();
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                // Rollback all changes
                organizationId = VLF.CLS.Def.Const.unassignedIntValue;
                sqlExec.RollbackTransaction();
                throw new DASException(objException.Message);
            }
            return organizationId;
            //return organization.AddOrganization(organizationName,contact,address,description);
        }


        /// <summary>
        /// Add new organization.
        /// </summary>
        /// <param name="organizationName"></param>
        /// <param name="contact"></param>
        /// <param name="address"></param>
        /// <param name="description"></param>
        /// <param name="logoName"></param>
        /// <param name="homePageName"></param>
        /// <param name="mapGroupId"></param>
        /// <param name="geoCodeGroupId"></param>
        /// <returns>int next organization id</returns>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if organization alredy exists.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public int AddOrganization(string organizationName, string contact, string address, string description, string logoName, string homePageName, int mapGroupId, int geoCodeGroupId)
        {
            int organizationId = VLF.CLS.Def.Const.unassignedIntValue;
            try
            {
                // 1. Begin transaction
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                // 2. Add new organization
                organizationId = organization.AddOrganization(organizationName, contact, address, description, logoName, homePageName, mapGroupId, geoCodeGroupId);
                // 3. Add default fleet to organization
                DB.Fleet fleet = new DB.Fleet(sqlExec);
                fleet.AddFleet(VLF.CLS.Def.Const.defaultFleetName, organizationId, "");
                // 4. Save all changes
                sqlExec.CommitTransaction();
            }
            catch (SqlException objException)
            {
                // Rollback all changes
                organizationId = VLF.CLS.Def.Const.unassignedIntValue;
                sqlExec.RollbackTransaction();
                Util.ProcessDbException("Cannot add the organization " + organizationName, objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                sqlExec.RollbackTransaction();
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                // Rollback all changes
                organizationId = VLF.CLS.Def.Const.unassignedIntValue;
                sqlExec.RollbackTransaction();
                throw new DASException(objException.Message);
            }
            return organizationId;
            //return organization.AddOrganization(organizationName,contact,address,description);
        }

        /// <summary>
        /// Update organization information.
        /// </summary>
        /// <returns>void</returns>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if organization alredy exists.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        /// <param name="organizationId"></param>
        /// <param name="organizationName"></param>
        /// <param name="contact"></param>
        /// <param name="address"></param>
        /// <param name="description"></param>
        /// <param name="logoName"></param>
        /// <param name="homePageName"></param>
        /// <param name="mapGroupId"></param>
        /// <param name="geoCodeGroupId"></param>
        public void UpdateInfo(int organizationId, string organizationName, string contact, string address, string description, string logoName, string homePageName, int mapGroupId, int geoCodeGroupId)
        {
            organization.UpdateInfo(organizationId, organizationName, contact, address, description, logoName, homePageName, mapGroupId, geoCodeGroupId);
        }

        /// <summary>
        /// Update organization information.
        /// </summary>
        /// <param name="superOrganizationId"></param>
        /// <param name="organizationId"></param>
        /// <param name="organizationName"></param>
        /// <param name="contact"></param>
        /// <param name="address"></param>
        /// <param name="description"></param>
        /// <param name="logoName"></param>
        /// <param name="homePageName"></param>
        /// <param name="mapGroupId"></param>
        /// <param name="geoCodeGroupId"></param>
        public void UpdateInfo(int superOrganizationId, int organizationId, string organizationName, string contact, string address, string description, string logoName, string homePageName, int mapGroupId, int geoCodeGroupId)
        {
            organization.UpdateInfo(superOrganizationId, organizationId, organizationName, contact, address, description, logoName, homePageName, mapGroupId, geoCodeGroupId);
        }


        /// <summary>
        /// UpdateOrganizationHistoryAccess
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="HistoryInterval"></param>
        public void UpdateOrganizationHistoryAccess(int organizationId, Int16 HistoryInterval)
        {
            organization.UpdateOrganizationHistoryAccess(organizationId, HistoryInterval); 
        }

        /// <summary>
        /// Deletes organization calling stored procedure and using cascade deleting
        /// </summary>
        /// <param name="orgID">Organization ID</param>
        /// <returns type="int">Rows deleted</returns>
        public int DeleteOrganization(int orgID)
        {
            int boxes = GetOrganizationBoxes(orgID);
            if (boxes > 0) throw new DASDbException("Cannot delete organization that has boxes");
            return organization.DeleteRowsByIntField("OrganizationId", orgID, "organization Id");
        }

        /// <summary>
        /// Delete existing organization.
        /// </summary>
        /// <param name="organizationName"></param> 
        /// <returns>Rows Affected</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public int DeleteOrganizationByOrganizationName(string organizationName)
        {
            return DeleteOrganizationByOrganizationId(organization.GetOrganizationIdByOrganizationName(organizationName));
        }

        /// <summary>
        /// Delete existing organization.
        /// </summary>
        /// <param name="organizationId"></param> 
        /// <returns>Rows Affected</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public int DeleteOrganizationByOrganizationId(int organizationId)
        {
            int rowsAffected = 0;
            try
            {
                int boxes = GetOrganizationBoxes(organizationId);
                if (boxes > 0) throw new DASDbException("Cannot delete organization having boxes");

                // 1. Begin transaction
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

                // 2. Delete organization landmarks
                //DB.Landmark landmark = new DB.Landmark(sqlExec);
                rowsAffected += landmark.DeleteAllLandMarksByOrganizationId(organizationId);

                # region 3. Delete organization geozones
                //DB.OrganizationGeozone organizationGeozone = new DB.OrganizationGeozone(sqlExec);
                DataSet dsOrganizationGeozonesInfo = organizationGeozone.GetOrganizationGeozonesInfo(organizationId, null);
                if (Util.IsDataSetValid(dsOrganizationGeozonesInfo))
                {
                    foreach (DataRow ittr in dsOrganizationGeozonesInfo.Tables[0].Rows)
                    {
                        // 3.1 Delete geozone set
                        rowsAffected += organizationGeozone.DeleteGeozoneSet(Convert.ToInt64(ittr["GeozoneNo"]));
                        // 3.2 Delete geozone from organization
                        rowsAffected += organizationGeozone.DeleteGeozoneFromOrganization(organizationId, Convert.ToInt16(ittr["GeozoneId"]));
                    }
                }
                # endregion

                // 4. Delete organization users
                using (User user = new User(this.ConnectionString))
                {
                    List<int> users = user.GetOrganizationUsers(organizationId);
                    if (users != null)
                    {
                        foreach (int userId in users)
                        {
                            rowsAffected += user.DeleteUserByUserId(userId);
                        }
                    }
                }

                // 5. Delete organization vehicles
                using (Vehicle vehicle = new Vehicle(this.ConnectionString))
                {
                    rowsAffected += vehicle.DeleteOrganizationVehicles(organizationId);
                }

                // 6. Delete organization fleets
                using (Fleet fleet = new Fleet(this.ConnectionString))
                {
                    rowsAffected += fleet.DeleteOrganizationFleets(organizationId);
                }

                // 7. Delete organization drivers
                using (DriverManager drvr = new DriverManager(this.ConnectionString))
                {
                    DataSet dsDrivers = drvr.GetDriversForOrganization(organizationId);
                    if (Util.IsDataSetValid(dsDrivers))
                    {
                        foreach (DataRow row in dsDrivers.Tables[0].Rows)
                        {
                            rowsAffected += drvr.DeleteDriver(Convert.ToInt32(row["DriverId"]));
                        }
                    }
                }

                // 8. Delete Organization Preferences

                new OrganizationPreferences(sqlExec).DeleteOrganizationPreferenceByOrganizationId(organizationId);


                // 9. Delete organization
                rowsAffected += organization.DeleteOrganizationByOrganizationId(organizationId);

                // 10. Save all changes
                sqlExec.CommitTransaction();
            }
            catch (SqlException objException)
            {
                rowsAffected = 0;
                sqlExec.RollbackTransaction();
                Util.ProcessDbException("Cannot delete the organization ID " + organizationId.ToString(), objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                rowsAffected = 0;
                sqlExec.RollbackTransaction();
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                rowsAffected = 0;
                sqlExec.RollbackTransaction();
                throw new DASException(objException.Message);
            }
            return rowsAffected;
        }

        /// <summary>
        /// Get count of organization boxes
        /// </summary>
        /// <param name="orgID"></param>
        /// <returns>number of boxes</returns>
        public int GetOrganizationBoxes(int orgID)
        {
            int boxes = -1;
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@orgId", SqlDbType.Int, orgID);
            object res = sqlExec.SQLExecuteScalar("SELECT COUNT(BoxId) FROM vlfBox with (nolock) WHERE OrganizationId = @orgId");
            if (res != null)
                boxes = (int)res;

            return boxes;
        }

        /// <summary>
        /// Gets assigned boxes for org.
        /// </summary>
        /// <param name="orgId">Organization Id</param>
        /// <returns></returns>
        public DataSet GetAssignedBoxes(int orgId)
        {
            DataSet dsResult = new DataSet();
            //try
            //{
            sqlExec.ClearCommandParameters();
            sqlExec.AddCommandParam("@orgId", SqlDbType.Int, orgId);
            dsResult = sqlExec.SPExecuteDataset("OrganizationGetAssignedBoxes");
            //}
            //catch (SqlException se)
            //{
            //   Util.ProcessDbException("Stored procedure error", se);
            //   throw se;
            //}
            //catch (DASDbConnectionClosed exCnn)
            //{
            //   throw new DASDbConnectionClosed(exCnn.Message);
            //}
            return dsResult;
        }

        /// <summary>
        /// Retrieves Organization info
        /// </summary>
        /// <returns>DataSet [OrganizationId],[OrganizationName],[Contact],
        /// [Address],[Description],[LogoName],[HomePageName],
        /// [MapGroupId],[MapGroupName],[MapPriority],[MapId],[MapEngineName],[MapPath],[MapExternalPath],
        /// [GeoCodeGroupId],[GeoCodeGroupName],[GeoCodePriority],[GeoCodeId],[GeoCodeEngineName],[GeoPath]</returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        public DataSet GetOrganizationInfoByOrganizationId(int organizationId)
        {
            DataSet dsResult = organization.GetOrganizationInfoByOrganizationId(organizationId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "OrganizationInformation";
                }
                dsResult.DataSetName = "Fleet";
            }
            return dsResult;
        }

        /// <summary>
        /// Retrieves Organization info
        /// </summary>
        /// <returns>DataSet [OrganizationId],[OrganizationName],[Contact],
        /// [Address],[Description],[LogoName],[HomePageName],
        /// [MapGroupId],[MapGroupName],[MapPriority],[MapId],[MapEngineName],[MapPath],[MapExternalPath],
        /// [GeoCodeGroupId],[GeoCodeGroupName],[GeoCodePriority],[GeoCodeId],[GeoCodeEngineName],[GeoPath]</returns>
        /// <param name="organizationName"></param> 
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        public DataSet GetOrganizationInfoByOrganizationName(string organizationName)
        {
            DataSet dsResult = organization.GetOrganizationInfoByOrganizationName(organizationName);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "OrganizationInformation";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }

        /// <summary>
        /// Returns organization id by organization name.
        /// </summary>
        /// <param name="organizationName"></param> 
        /// <returns>int</returns>
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        public int GetOrganizationIdByOrganizationName(string organizationName)
        {
            return organization.GetOrganizationIdByOrganizationName(organizationName);
        }

        /// <summary>
        /// Returns organization name by organization Id. 	
        /// </summary>
        /// <param name="organizationId"></param> 
        /// <returns>int</returns>
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        public string GetOrganizationNameByOrganizationId(int organizationId)
        {
            return organization.GetOrganizationNameByOrganizationId(organizationId);
        }

        /// <summary>
        /// Retrieves information about all organizations
        /// </summary>
        /// <returns>string[,] of [OrganizationId][OrganizationName]</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public string[,] GetAllOrganizationsInfoStruct()
        {
            string[,] resultArr = null;
            DataSet sqlDataSet = organization.GetAllRecords();
            if ((sqlDataSet != null) && (sqlDataSet.Tables[0].Rows.Count > 0))
            {
                resultArr = new string[sqlDataSet.Tables[0].Rows.Count, 2];
                int index = 0;
                foreach (DataRow currRow in sqlDataSet.Tables[0].Rows)
                {
                    resultArr[index, 0] = Convert.ToString(currRow[0]);
                    resultArr[index, 1] = Convert.ToString(currRow[1]).TrimEnd();
                    ++index;
                }
            }
            return resultArr;
        }

        /// <summary>
        /// Retrieves information about all organizations 
        /// </summary>
        /// <returns>DataSet [OrganizationId],[OrganizationName],[Contact],[Address],[Description]</returns>
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        public DataSet GetAllOrganizationsInfo()
        {
            DataSet dsResult = organization.GetAllOrganizationsInfo();
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "OrganizationInformation";
                }
                dsResult.DataSetName = "Organizations";
            }
            return dsResult;
        }

        /// <summary>
        /// Retrieves information about all organizations availabel for the userid
        /// user organization and all child organization (if super organization)
        /// </summary>
        /// <returns>DataSet [OrganizationId],[OrganizationName],[Contact],[Address],[Description]</returns>
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        public DataSet GetOrganizationsInfoListByUser(int userId)
        {
            DataSet dsResult = organization.GetOrganizationsInfoListByUser(userId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "OrganizationInformation";
                }
                dsResult.DataSetName = "Organizations";
            }
            return dsResult;
        }

        /// <summary>
        /// Get total organizations number
        /// </summary>
        /// <returns>int</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public int GetTotalOrganizations()
        {
            int totalOrganizations = 0;
            DataSet dsResult = organization.GetAllRecords();
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    totalOrganizations = dsResult.Tables[0].Rows.Count;
                }
            }
            return totalOrganizations;
        }

        /// <summary>
        /// Retrieves all license plates for specific organization
        /// </summary>
        /// <returns>DataSet [LicensePlate]</returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetLicensePlatesByOrganizationId(int organizationId)
        {
            DataSet dsResult = organization.GetLicensePlatesByOrganizationId(organizationId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "LicensePlates";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }

        /// <summary>
        /// Retrieves all unassigned vehicles license plates to driver for specific organization
        /// </summary>
        /// <returns>DataSet [LicensePlate]</returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetUnassignedLicensePlatesByOrganizationId(int organizationId)
        {
            DataSet dsResult = driverAssignment.GetUnassignedDriversForOrganization(organizationId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "UnassignedDrivers";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }

        /// <summary>
        /// Gets the organization ID owning the given fleet ID
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns></returns>
        public int GetOrganizationIdByFleetId(int fleetId)
        {
            return organization.GetOrganizationIdByFleetId(fleetId);
        }

        /// <summary>
        /// Gets the organization name owning the given fleet ID
        /// </summary>
        /// <param name="fleetId"></param>
        /// <returns></returns>
        public String GetOrganizationNameByFleetId(int fleetId)
        {
            return organization.GetOrganizationNameByFleetId(fleetId);
        }

        /// <summary>
        /// Gets the organization ID owning the given box ID
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns></returns>
        public int GetOrganizationIdByBoxId(int boxId)
        {
            return organization.GetOrganizationIdByBoxId(boxId);
        }

        /// <summary>
        /// Gets the organization name owning the given box ID
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns></returns>
        public String GetOrganizationNameByBoxId(int boxId)
        {
            return organization.GetOrganizationNameByBoxId(boxId);
        }

        /// <summary>
        /// Retrieves Organization ids
        /// </summary>
        /// <returns>DataSet [OrganizationId]</returns>
        /// <param name="boxId"></param> 
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        public DataSet GetOrganizationIdsByBoxId(int boxId)
        {
            DataSet dsResult = organization.GetOrganizationIdsByBoxId(boxId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "OrganizationIds";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }

        /// <summary>
        /// Returns fleets information by organization id. 
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns>DataSet [FleetId],[FleetName],[Description],[OrganizationId],[OrganizationName]</returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public DataSet GetFleetsInfoByOrganizationId(int organizationId)
        {
            DataSet dsResult = organization.GetFleetsInfoByOrganizationId(organizationId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "FleetsInformation";
                }
                dsResult.DataSetName = "Fleet";
            }
            return dsResult;
        }

        /// <summary>
        /// Gets Fleets by OrganizationId and FleetType
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <param name="FleetType"></param>
        /// <returns>DataSet [FleetId],[FleetName],[Description]</returns>
        public DataSet GetFleetsInfoByOrganizationIdAndType(int OrganizationId, string FleetType)
        {
            return organization.GetFleetsInfoByOrganizationIdAndType(OrganizationId, FleetType);   
        }

        /// <summary>
        /// Returns all users info assigned to the organization. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="organizationId"></param>
        /// <returns>DataSet [UserId],[UserName],[FirstName],[LastName],[PersonId],[ExpiredDate]</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetUsersInfoByOrganization(int userId, int organizationId, bool ChkbxViewDeletedUser)
        {
            DataSet dsResult = organization.GetUsersInfoByOrganization(userId, organizationId, ChkbxViewDeletedUser);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "UsersInformation";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }

        /// <summary>
        /// Returns all users info assigned to the organization. 
        /// </summary>
        /// <returns>DataSet [UserId],[UserName],[FirstName],[LastName],[PersonId]</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        /// <param name="userId"></param>
        /// <param name="organizationName"></param>
        public DataSet GetUsersInfoByOrganization(int userId, string organizationName, bool ChkbxViewDeletedUser)
        {
            DataSet dsResult = organization.GetUsersInfoByOrganization(userId, GetOrganizationIdByOrganizationName(organizationName), ChkbxViewDeletedUser);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "UsersInformation";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
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
        public DataSet GetOrganizationInfoByUserId(int userId)
        {
            DataSet dsResult = organization.GetOrganizationInfoByUserId(userId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "OrganizationInfo";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }

        /// <summary>
        /// Returns all user assigned to the group for specific organization. 
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="userGroupId"></param>
        /// <returns>DataSet [UserId],[UserName],[FirstName],[LastName]</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetOrganizationUsersByUserGroup(int organizationId, short userGroupId)
        {
            DataSet dsResult = organization.GetOrganizationUsersByUserGroup(organizationId, userGroupId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "OrganizationUsers";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }

        /// <summary>
        /// Returns all user assigned to the group for specific organization. 
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="userGroupId"></param>
        /// <returns>DataSet [UserId],[UserName],[Password],[FirstName],[LastName]</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetOrganizationUsersSecurityByUserGroup(int organizationId, short userGroupId)
        {
            DataSet dsResult = organization.GetOrganizationUsersSecurityByUserGroup(organizationId, userGroupId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "OrganizationUsers";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }

        /// <summary>
        /// Retrieves all active(assigned to the box) vihecles info XML format.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns>DataSet [LicensePlate],[BoxId],[VehicleId],[VinNum],
        /// [MakeModelId],[MakeName],[ModelName],[VehicleTypeName],[StateProvince],
        /// [ModelYear],[Color],[Description],[CostPerMile],
        /// [IconTypeId],[IconTypeName]</returns>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public DataSet GetAllActiveVehiclesInfo(int organizationId)
        {
            DB.VehicleInfo vehicleInfo = new DB.VehicleInfo(sqlExec);
            // 1. Retrieves all vehicle assignments
            DataSet dsResult = vehicleInfo.GetAllVehiclesActivesInfo(organizationId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "OrganizationAllActiveVehiclesInfo";
                }
                dsResult.DataSetName = "Vehicle";
            }
            return dsResult;
        }

        /// <summary>
        /// Retrieves communication information for all organization
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="commAddressTypeId"></param>
        /// <returns>DataSet [BoxId],[CommAddressValue]</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetOrganizationCommInfo(int organizationId, short commAddressTypeId)
        {
            DataSet dsResult = organization.GetOrganizationCommInfo(organizationId, commAddressTypeId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "GetOrganizationCommInfo";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }

        /// <summary>
        /// Privacy verification of Organization and Device integrity
        /// </summary>
        /// <param name="userId">userId from Login</param>
        /// <param name="boxId">boxId from Vehicle information</param>
        /// <returns>boolean</returns>
        public bool VerifyOrganizationBoxIntegrityByUserId(int userId, int boxId)
        {
            var orgInfo = organization.GetOrganizationInfoByUserId(userId);
            if (!Util.IsDataSetValid(orgInfo))
                return false;

            return Convert.ToInt32(orgInfo.Tables[0].Rows[0][0]) == organization.GetOrganizationIdByBoxId(boxId);
        }
        #endregion

        #region Landmark Interfaces


        /// <summary>
        /// Add new landmark for import
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="landmarkName"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <param name="description"></param>
        /// <param name="contactPersonName"></param>
        /// <param name="contactPhoneNum"></param>
        /// <param name="radius"></param>
        /// <param name="email"></param>
        /// <param name="timeZone"></param>
        /// <param name="dayLightSaving"></param>
        /// <param name="autoAdjustDayLightSaving"></param>
        /// <param name="streetAddress"></param>
        /// <param name="publicLandmark"></param>
        /// <param name="createUserID"></param>
        /// <param name="categoryName"></param>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if landmark for specific organization already exists.</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public string AddLandmark(int organizationId, string landmarkName, double latitude,
           double longitude, string description, string contactPersonName,
           string contactPhoneNum, int radius, string email, string phone, short timeZone, bool dayLightSaving, bool autoAdjustDayLightSaving,
           string streetAddress, bool publicLandmark,int createUserID, string categoryName)
        {
            string rvMessage = null; 
            try
            {
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);

                rvMessage = landmark.AddLandmark(organizationId, landmarkName, latitude, longitude, description, contactPersonName, 
                    contactPhoneNum, radius, email, phone, timeZone, dayLightSaving, autoAdjustDayLightSaving, 
                    streetAddress, publicLandmark, createUserID, categoryName);

                sqlExec.CommitTransaction();
            }
            catch (Exception ex)
            {
                // Rollback all changes
                sqlExec.RollbackTransaction();
                throw;
            }

            return rvMessage;
        }


        ///// <summary>
        ///// Add new landmark.
        ///// </summary>
        ///// <param name="organizationId"></param>
        ///// <param name="landmarkName"></param>
        ///// <param name="latitude"></param>
        ///// <param name="longitude"></param>
        ///// <param name="description"></param>
        ///// <param name="contactPersonName"></param>
        ///// <param name="contactPhoneNum"></param>
        ///// <param name="radius"></param>
        ///// <param name="email"></param>
        ///// <param name="timeZone"></param>
        ///// <param name="dayLightSaving"></param>
        ///// <param name="autoAdjustDayLightSaving"></param>
        ///// <param name="streetAddress"></param>
        ///// <exception cref="DASAppDataAlreadyExistsException">Thrown if landmark for specific organization already exists.</exception>
        ///// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        //public void AddLandmark(int organizationId, string landmarkName, double latitude,
        //   double longitude, string description, string contactPersonName,
        //   string contactPhoneNum, int radius, string email, string phone, short timeZone, bool dayLightSaving, bool autoAdjustDayLightSaving,
        //   string streetAddress, bool publicLandmark)
        //{

        //    landmark.AddLandmark(organizationId, landmarkName, latitude, longitude, description, contactPersonName, contactPhoneNum, radius, email, phone, timeZone, dayLightSaving, autoAdjustDayLightSaving, streetAddress, publicLandmark);
        //}

        /// <summary>
        /// Add several landmarks for organization
        /// </summary>
        /// <param name="organizationId">Organization Id</param>
        /// <param name="dsLandmarks">DataSet of landmarks</param>
        /// <returns>XML string of rows that were not added</returns>
        public string AddLandmarks(int organizationId, DataSet dsLandmarks)
        {
            bool resultOk = false;
            string xmlResult = "";
            DataSet ds = new DataSet();
            if (Util.IsDataSetValid(dsLandmarks))
            {
                //resultOk = new bool[dsLandmarks.Tables[0].Rows.Count];
                //resultOk.Initialize();
                DataTable dt = new DataTable();
                dt.Columns.Add("LandmarkName");
                dt.Columns.Add("LandmarkAddress");
                for (int i = 0; i < dsLandmarks.Tables[0].Rows.Count; i++)
                {
                    DataRow dr = dsLandmarks.Tables[0].Rows[i];
                    try
                    {
                        landmark.AddLandmark(organizationId,
                           dr["Name"].ToString(),
                           Convert.ToDouble(dr["Latitude"]),
                           Convert.ToDouble(dr["Longitude"]),
                           dr["Description"].ToString(),
                           dr["Contact"].ToString(),
                           dr["PhoneNumber"].ToString(),
                           Convert.ToInt32(dr["Radius"]),
                           dr["Email"].ToString(),
                           dr["Phone"].ToString(),
                           Convert.ToInt16(dr["TimeZone"]),
                           Convert.ToBoolean(dr["DayLightSaving"]),
                           Convert.ToBoolean(dr["AutoDayLightSaving"]),
                           dr["Address"].ToString(),
                           ref resultOk);
                        if (!resultOk) dt.Rows.Add(dr["Name"], dr["Address"]);
                    }
                    catch
                    {
                        dt.Rows.Add(dr["Name"], dr["Address"]);
                    }
                }
                ds.Tables.Add(dt);
                if (Util.IsDataSetValid(ds)) xmlResult = ds.GetXml();
            }
            return xmlResult;
        }

        /// <summary>
        /// Deletes all landmarks related to specific organization.
        /// </summary>
        /// <returns>void</returns>
        /// <param name="organizationId"></param>
        /// <exception cref="DASAppResultNotFoundException">Thrown if alarm does not exist</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public int DeleteAllLandMarksByOrganizationId(int organizationId)
        {
            return landmark.DeleteAllLandMarksByOrganizationId(organizationId);
        }
        /// <summary>
        /// Deletes landmark from organization.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="landmarkName"></param>
        /// <returns></returns>
        public int DeleteLandmarkFromOrganization(int organizationId, string landmarkName)
        {
            return landmark.DeleteLandmarkFromOrganization(organizationId, landmarkName);
        }
        /// <summary>
        /// Update landmark info.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="currLandmarkName"></param>
        /// <param name="newLandmarkName"></param>
        /// <param name="newLatitude"></param>
        /// <param name="newLongitude"></param>
        /// <param name="newDescription"></param>
        /// <param name="newContactPersonName"></param>
        /// <param name="newContactPhoneNum"></param>
        /// <param name="radius"></param>
        /// <param name="email"></param>
        /// <param name="timeZone"></param>
        /// <param name="dayLightSaving"></param>
        /// <param name="autoAdjustDayLightSaving"></param>
        /// <param name="streetAddress"></param>
        /// <remarks>
        /// In case of updating landmark info except landmark name, set newLandmarkName to VLF.CLS.Def.Const.unassignedStrValue
        /// </remarks>
        /// <returns>void</returns>
        /// <exception cref="DASAppResultNotFoundException">Thrown if alarm does not exist.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public void UpdateLandmark(int organizationId, string currLandmarkName, string newLandmarkName,
           double newLatitude, double newLongitude, string newDescription,
           string newContactPersonName, string newContactPhoneNum, int radius,
           string email,string phone, short timeZone, bool dayLightSaving, bool autoAdjustDayLightSaving,
              string streetAddress, bool publicLandmark)
        {

            landmark.UpdateLandmark(organizationId, currLandmarkName, newLandmarkName,
                              newLatitude, newLongitude, newDescription,
                              newContactPersonName, newContactPhoneNum, radius,
                              email, phone,timeZone, dayLightSaving, autoAdjustDayLightSaving,
                                       streetAddress, publicLandmark);
        }
        /// <summary>
        /// Retrieves landmarks info by organization id 
        /// </summary>
        /// <returns>
        /// DataSet [OrganizationId],[LandmarkName],[Latitude],[Longitude],
        /// [Description],[ContactPersonName],[ContactPhoneNum],[Radius],
        /// [Email],[TimeZone],[DayLightSaving],[AutoAdjustDayLightSaving],
        /// [StreetAddress]
        /// </returns>
        /// <param name="organizationId"></param> 
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        public DataSet GetLandMarksInfoByOrganizationId(int organizationId)
        {
            DataSet dsResult = landmark.GetLandMarksInfoByOrganizationId(organizationId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "LandmarksInformation";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }

        public int SaveLandmarkCategory(int userId, int orgId, long landmarkId, long landmarkCategoryId)
        {
            return landmark.SaveLandmarkCategory(userId, orgId, landmarkId, landmarkCategoryId);
        }

        public int DeleteLandmarkCategory(int userId, int orgId, long landmarkId)
        {
            return landmark.DeleteLandmarkCategory(userId, orgId, landmarkId);
        }

        

        public int AddOrganizationDomainMetadata(int userId, int orgId, long domainId, string metadataName)
        {
            return organization.AddOrganizationDomainMetadata(userId, orgId, domainId, metadataName);
        }

        public int DeleteOrganizationDomainMetadata(int userId, int orgId, long domainMetadataId)
        {
            return organization.DeleteOrganizationDomainMetadata(userId, orgId, domainMetadataId);
        }

        public int UpdateOrganizationDomainMetadata(int userId, int orgId, long domainMetadataId, string newMetadataName)
        {
            return organization.UpdateOrganizationDomainMetadata(userId, orgId, domainMetadataId, newMetadataName);
        }
        


        public DataSet GetLandmarkCategory(int userId, int orgId, long landmarkId)
        {
            DataSet dsResult = landmark.GetLandmarkCategory(userId, orgId, landmarkId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "LandmarkCategory";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }


        public DataSet ListOrganizationLandmarkCategory(int userId, int orgId)
        {
            DataSet dsResult = landmark.ListOrganizationLandmarkCategory(userId, orgId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "LandmarkCategory";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }

        public DataSet GetOrganizationLandmark_Public(int userId, int orgId, bool IsPublic)
        {
            DataSet dsResult = landmark.GetOrganizationLandmark_Public(userId, orgId, IsPublic);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "LandmarksInformation";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }

        /// <summary>
        /// Retrieves all landmarks info with street != null
        /// </summary>
        /// <returns>
        /// DataSet [OrganizationId],[LandmarkName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetAllLandMarksInfo()
        {
            DataSet dsResult = landmark.GetAllLandMarksInfo();
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "GetAllLandMarksInfo";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="orgIds"></param>
        /// <returns></returns>
        public DataSet GetAllLandMarksInfo(int[] orgIds)
        {
            DataSet dsResult = landmark.GetAllLandMarksInfo(orgIds);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "GetAllLandMarksInfo";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }
        /// <summary>
        /// Update landmark info.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="landmarkName"></param>
        /// <param name="streetAddress"></param>
        /// <remarks>
        /// In case of updating landmark info except landmark name, set newLandmarkName to VLF.CLS.Def.Const.unassignedStrValue
        /// </remarks>
        /// <returns>void</returns>
        /// <exception cref="DASAppResultNotFoundException">Thrown if alarm does not exist.</exception>
        /// <exception cref="DASAppInvalidValueException">Thrown in case of incorrect value.</exception>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void UpdateLandmarkStreetAddress(int organizationId, string landmarkName, string streetAddress)
        {
            landmark.UpdateLandmarkStreetAddress(organizationId, landmarkName, streetAddress);
        }
        /// <summary>
        /// Retrieves landmarks info by organization name 
        /// </summary>
        /// <returns>
        /// DataSet [OrganizationId],[LandmarkName],[Latitude],[Longitude],
        /// [Description],[ContactPersonName],[ContactPhoneNum],[Radius],[StreetAddress]
        /// </returns>
        /// <param name="organizationName"></param> 
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        public DataSet GetLandMarksInfoByOrganizationName(string organizationName)
        {
            DataSet dsResult = landmark.GetLandMarksInfoByOrganizationName(organizationName);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "LandmarksInformation";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }

        /// <summary>
        /// Retrieves all landmarks info
        /// </summary>
        /// <returns>
        /// DataSet [OrganizationId],[LandmarkName],[Latitude],[Longitude],
        /// [Description],[ContactPersonName],[ContactPhoneNum],[Radius]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all  exception cases.</exception>
        public DataSet GetAllLandmarks()
        {
            DataSet dsResult = landmark.GetAllRecords();
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "LandmarksInformation";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }
        /// <summary>
        /// Retrieves landmark name by location
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        /// <remarks>
        /// If landmark does not exist, returns VLF.CLS.Def.Const.unassignedStrValue
        /// </remarks>
        /// <returns>
        /// string [landmark name]
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public string GetLandmarkName(int organizationId, double latitude, double longitude)
        {
            return landmark.GetLandmarkName(organizationId, latitude, longitude);
        }
        /// <summary>
        /// Retrieves landmark location by landmark name 
        /// </summary>
        /// <remarks>
        /// If landmark does not exist, returns latitude=0,longitude=0
        /// </remarks>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        /// <param name="organizationId"></param>
        /// <param name="landmarkName"></param>
        /// <param name="latitude"></param>
        /// <param name="longitude"></param>
        public void GetLandmarkLocation(int organizationId, string landmarkName, ref double latitude, ref double longitude)
        {
            landmark.GetLandmarkLocation(organizationId, landmarkName, ref latitude, ref longitude);
        }
        #endregion

        #region Geozone Interfaces
        // Changes for TimeZone Feature start
        /// <summary>
        /// Add new geozone to organization.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="geozoneName"></param>
        /// <param name="type"></param>
        /// <param name="geozoneType"></param>
        /// <param name="dsGeozoneSet"></param>
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
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if geozone for specific organization already exists.</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public void AddGeozone_NewTZ(int organizationId, string geozoneName,
                          short type, short geozoneType, DataSet dsGeozoneSet,
                          short severityId, string description,
                          string email, string phone, float timeZone, bool dayLightSaving,
                          short formatType, bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, int speed, bool publicGeozone, int userId)
        {

            try
            {
                // 1. Begin transaction
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                // 2. Add new geozone
                Int64 geozoneNo = organizationGeozone.AddGeozone_NewTZ(organizationId, geozoneName, type, geozoneType, severityId, description, email, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, speed, publicGeozone, userId);
                // 3. Add geozone location
                organizationGeozone.AddGeozoneSet(geozoneNo, dsGeozoneSet);
                // 4. Save all changes
                sqlExec.CommitTransaction();
            }
            catch (SqlException objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                Util.ProcessDbException("Unable to add new geozone ", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                sqlExec.RollbackTransaction();
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                throw new DASException(objException.Message);
            }
        }


        // Changes for TimeZone Feature end

        /// <summary>
        /// Add new geozone to organization.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="geozoneName"></param>
        /// <param name="type"></param>
        /// <param name="geozoneType"></param>
        /// <param name="dsGeozoneSet"></param>
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
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if geozone for specific organization already exists.</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public void AddGeozone(int organizationId, string geozoneName,
                          short type, short geozoneType, DataSet dsGeozoneSet,
                          short severityId, string description,
                          string email, string phone, int timeZone, bool dayLightSaving,
                          short formatType, bool notify, bool warning, bool critical, bool autoAdjustDayLightSaving, int speed, bool publicGeozone, int userId)
        {

            try
            {
                // 1. Begin transaction
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                // 2. Add new geozone
                Int64 geozoneNo = organizationGeozone.AddGeozone(organizationId, geozoneName, type, geozoneType, severityId, description, email, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, speed, publicGeozone, userId);
                // 3. Add geozone location
                organizationGeozone.AddGeozoneSet(geozoneNo, dsGeozoneSet);
                // 4. Save all changes
                sqlExec.CommitTransaction();
            }
            catch (SqlException objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                Util.ProcessDbException("Unable to add new geozone ", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                sqlExec.RollbackTransaction();
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                throw new DASException(objException.Message);
            }
        }
        /// <summary>
        /// Deletes all geozones related to specific organization.
        /// </summary>
        /// <returns>Rows affected</returns>
        /// <param name="organizationId"></param>
        /// <exception cref="DASAppResultNotFoundException">Thrown if geozone does not exist</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        public int DeleteOrganizationAllGeozones(int organizationId)
        {
            int rowsAffected = 0;
            DataSet dsOrganizationGeozonesInfo = organizationGeozone.GetOrganizationGeozonesInfo(organizationId, null);
            if (dsOrganizationGeozonesInfo == null || dsOrganizationGeozonesInfo.Tables.Count == 0 || dsOrganizationGeozonesInfo.Tables[0].Rows.Count == 0)
                return rowsAffected;
            try
            {
                // 1. Begin transaction
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                foreach (DataRow ittr in dsOrganizationGeozonesInfo.Tables[0].Rows)
                {
                    // 2. Delete geozone set
                    organizationGeozone.DeleteGeozoneSet(Convert.ToInt64(ittr["GeozoneNo"]));
                    // 3. Delete geozone from organization
                    rowsAffected += organizationGeozone.DeleteGeozoneFromOrganization(organizationId, Convert.ToInt16(ittr["GeozoneId"]));
                }
                // 4. Save all changes
                sqlExec.CommitTransaction();
            }
            catch (SqlException objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                Util.ProcessDbException("Unable to delete geozone ", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                sqlExec.RollbackTransaction();
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                throw new DASException(objException.Message);
            }
            return rowsAffected;
        }
        /// <summary>
        /// Deletes geozone from organization.
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="geozoneId"></param>
        /// <returns></returns>
        public int DeleteGeozoneFromOrganization(int organizationId, short geozoneId)
        {
            int rowsAffected = 0;
            try
            {
                // 1. Begin transaction
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                // 2. Delete geozone set
                organizationGeozone.DeleteGeozoneSet(organizationGeozone.GetGeozoneNoByGeozoneId(organizationId, geozoneId));
                // 3. Delete geozone from organization
                rowsAffected = organizationGeozone.DeleteGeozoneFromOrganization(organizationId, geozoneId);
                // 4. Save all changes
                sqlExec.CommitTransaction();
            }
            catch (SqlException objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                Util.ProcessDbException("Unable to delete geozone ", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                sqlExec.RollbackTransaction();
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                throw new DASException(objException.Message);
            }
            return rowsAffected;
        }

        // Changes for TimeZone Feature start
        /// <summary>
        /// Update geozone info.
        /// </summary>
        /// <returns>void</returns>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if geozone for specific organization already exists.</exception>
        /// <exception cref="DASAppResultNotFoundException">Thrown if geozone does not exist.</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        /// <param name="organizationId"></param>
        /// <param name="geozoneId"></param>
        /// <param name="geozoneName"></param>
        /// <param name="type"></param>
        /// <param name="geozoneType"></param>
        /// <param name="dsGeozoneSet"></param>
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
        public void UpdateGeozone_NewTZ(int organizationId, short geozoneId, string geozoneName, short type,
                          Int64 geozoneType, DataSet dsGeozoneSet, short severityId, string description,
                          string email, string phone, float timeZone, bool dayLightSaving, short formatType, bool notify,
                          bool warning, bool critical, bool autoAdjustDayLightSaving, bool publicLandmark, int userId)
        {



            try
            {
                // 1. Begin transaction
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                // 2. Update unassigned geozone location
                if (organizationGeozone.IsGeozoneAssigned(organizationId, geozoneId) == false)
                {
                    Int64 geozoneNo = organizationGeozone.GetGeozoneNoByGeozoneId(organizationId, geozoneId);
                    if (geozoneNo == VLF.CLS.Def.Const.unassignedIntValue)
                        throw new DASAppViolatedIntegrityConstraintsException("Unable to retrieve GeozoneNo by geozone id=" + geozoneId);
                    organizationGeozone.DeleteGeozoneSet(geozoneNo);
                    organizationGeozone.AddGeozoneSet(geozoneNo, dsGeozoneSet);
                }
                // 3. Update geozone info
                organizationGeozone.UpdateGeozone_NewTZ(organizationId, geozoneId, geozoneName, type, geozoneType, severityId, description, email, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, publicLandmark, userId);

                // 4. Save all changes
                sqlExec.CommitTransaction();
            }
            catch (SqlException objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                Util.ProcessDbException("Unable to update geozone ", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                sqlExec.RollbackTransaction();
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                throw new DASException(objException.Message);
            }
        }


        // Changes for TimeZone Feature end

        /// <summary>
        /// Update geozone info.
        /// </summary>
        /// <returns>void</returns>
        /// <exception cref="DASAppDataAlreadyExistsException">Thrown if geozone for specific organization already exists.</exception>
        /// <exception cref="DASAppResultNotFoundException">Thrown if geozone does not exist.</exception>
        /// <exception cref="DASException">Thrown DASException in all other exception cases.</exception>
        /// <param name="organizationId"></param>
        /// <param name="geozoneId"></param>
        /// <param name="geozoneName"></param>
        /// <param name="type"></param>
        /// <param name="geozoneType"></param>
        /// <param name="dsGeozoneSet"></param>
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
        public void UpdateGeozone(int organizationId, short geozoneId, string geozoneName, short type,
                          Int64 geozoneType, DataSet dsGeozoneSet, short severityId, string description,
                          string email, string phone, int timeZone, bool dayLightSaving, short formatType, bool notify,
                          bool warning, bool critical, bool autoAdjustDayLightSaving, bool publicLandmark, int userId)
        {


      
            try
            {
                // 1. Begin transaction
                sqlExec.BeginTransactionWithIsolationLevel(IsolationLevel.ReadCommitted);
                // 2. Update unassigned geozone location
                if (organizationGeozone.IsGeozoneAssigned(organizationId, geozoneId) == false)
                {
                    Int64 geozoneNo = organizationGeozone.GetGeozoneNoByGeozoneId(organizationId, geozoneId);
                    if (geozoneNo == VLF.CLS.Def.Const.unassignedIntValue)
                        throw new DASAppViolatedIntegrityConstraintsException("Unable to retrieve GeozoneNo by geozone id=" + geozoneId);
                    organizationGeozone.DeleteGeozoneSet(geozoneNo);
                    organizationGeozone.AddGeozoneSet(geozoneNo, dsGeozoneSet);
                }
                // 3. Update geozone info
                organizationGeozone.UpdateGeozone(organizationId, geozoneId, geozoneName, type, geozoneType, severityId, description, email, phone, timeZone, dayLightSaving, formatType, notify, warning, critical, autoAdjustDayLightSaving, publicLandmark, userId);
                
                // 4. Save all changes
                sqlExec.CommitTransaction();
            }
            catch (SqlException objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                Util.ProcessDbException("Unable to update geozone ", objException);
            }
            catch (DASDbConnectionClosed exCnn)
            {
                sqlExec.RollbackTransaction();
                throw new DASDbConnectionClosed(exCnn.Message);
            }
            catch (Exception objException)
            {
                // 4. Rollback all changes
                sqlExec.RollbackTransaction();
                throw new DASException(objException.Message);
            }
        }

        /// <summary>
        /// Check if geozone assigned to any of vehicles
        /// </summary>
        /// <param name="organizationId"></param> 
        /// <param name="geozoneId"></param> 
        /// <returns>bool</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public bool IsGeozoneAssigned(int organizationId, short geozoneId)
        {
            return organizationGeozone.IsGeozoneAssigned(organizationId, geozoneId);
        }
        /// <summary>
        /// Retrieves geozones info by organization id 
        /// </summary>
        /// <param name="organizationId"></param> 
        /// <param name="dsGeozonesList"></param> 
        /// <returns>
        /// DataSet [GeozoneNo],[OrganizationId],[GeozoneId],[GeozoneName],[Type],
        /// [GeozoneType],[GeozoneTypeName],[SeverityId],[Description],[Deleted]
        /// [Email],[TimeZone],[DayLightSaving],[FormatType],[Notify],[Warning],[Critical],[AutoAdjustDayLightSaving]</returns>
        /// </returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetOrganizationGeozonesInfo(int organizationId, DataSet dsGeozonesList)
        {
            DataSet dsResult = organizationGeozone.GetOrganizationGeozonesInfo(organizationId, dsGeozonesList);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "GeozonesInformation";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }
        /// <summary>
        /// Retrieves geozone types
        /// </summary>
        /// <returns>
        /// DataSet [GeozoneType],[GeozoneTypeName]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetGeozoneTypes()
        {
            DataSet dsResult = organizationGeozone.GetGeozoneTypes();
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "GeozoneTypes";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }
        /// <summary>
        /// Retrieves geozone info by organization id and geozone Id 
        /// </summary>
        /// <param name="organizationId"></param> 
        /// <param name="geozoneId"></param> 
        /// <returns>
        /// DataSet [GeozoneNo],[GeozoneId],[Type],[GeozoneType],[SequenceNum],[Latitude],[Longitude],[GeozoneName]</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetOrganizationGeozoneInfo(int organizationId, short geozoneId)
        {
            DataSet dsResult = organizationGeozone.GetOrganizationGeozoneInfo(organizationId, geozoneId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "GeozoneInformation";
                }
                dsResult.DataSetName = "Organization";
            }
            return dsResult;
        }

        /// <summary>
        /// Get geozones for organization
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataSet GetOrganizationGeozones(int organizationId)
        {
            DataSet dsGeozones =
               this.organizationGeozone.GetRowsByIntField("OrganizationId", organizationId, "Organization Geozones");
            if (dsGeozones != null)
            {
                if (dsGeozones.Tables.Count > 0)
                {
                    dsGeozones.Tables[0].TableName = "Geozones";
                }
                dsGeozones.DataSetName = "Organization";
            }
            return dsGeozones;
        }



        /// <summary>
        /// Get geozones for organization
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataSet GetOrganizationGeoZone_Public(int userId, int orgId, bool IsPublic)
        {
            DataSet dsGeozones =
               this.organizationGeozone.GetOrganizationGeoZone_Public(userId, orgId, IsPublic);
            if (dsGeozones != null)
            {
                if (dsGeozones.Tables.Count > 0)
                {
                    dsGeozones.Tables[0].TableName = "Geozones";
                }
                dsGeozones.DataSetName = "Organization";
            }
            return dsGeozones;
        }

        #endregion

        #region Organization Logins
        // Changes for TimeZone Feature start
        /// <summary>
        /// Retrieves user logins.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="organizationId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>DataSet [LoginId],[UserId],[LoginDateTime],[IP],
        /// [UserName],[FirstName],[LastName]</returns>
        public DataSet GetOrganizationUserLogins_NewTZ(int userId, int organizationId, DateTime from, DateTime to)
        {
            DataSet dsResult = userLogin.GetOrganizationUserLogins_NewTZ(userId, organizationId, from, to);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "OrganizationUsersLoginsInfo";
                }
                dsResult.DataSetName = "User";
            }
            return dsResult;
        }
        // Changes for TimeZone Feature end

        /// <summary>
        /// Retrieves user logins.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="organizationId"></param>
        /// <param name="from"></param>
        /// <param name="to"></param>
        /// <returns>DataSet [LoginId],[UserId],[LoginDateTime],[IP],
        /// [UserName],[FirstName],[LastName]</returns>
        public DataSet GetOrganizationUserLogins(int userId, int organizationId, DateTime from, DateTime to)
        {
            DataSet dsResult = userLogin.GetOrganizationUserLogins(userId, organizationId, from, to);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "OrganizationUsersLoginsInfo";
                }
                dsResult.DataSetName = "User";
            }
            return dsResult;
        }

        /// <summary>
        ///      this function is going to find what is the super-organization where the notifications are 
        ///      sent out
        ///      Designed for Bell, WEX with which we share the same back-end
        /// </summary>
        /// <param name="boxId"></param>
        /// <returns></returns>
        public string GetSystemEmailAddress(int boxId)
        {
            DB.Organization dbOrg = new DB.Organization(sqlExec);
            return dbOrg.GetSystemEmailAddress(boxId);
        }

        /// <summary>
        ///      this is a function for super organizations
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataSet GetOrganizationsMonitoredBy(int organizationId)
        {
            DB.Organization dbOrg = new DB.Organization(sqlExec);
            return dbOrg.GetOrganizationsMonitoredBy(organizationId);
        }

        /// <summary>
        /// Retrieves SuperOrganization ids and descriptions
        /// </summary>
        /// <returns></returns>
        public DataSet GetSuperOrganizations()
        {
            DB.Organization dbOrg = new DB.Organization(sqlExec);
            return dbOrg.GetSuperOrganizations();
        }

        /// <summary>
        ///      it allows a user to login in a different organization if his organization 
        ///      is the Super Organization (monitors all other organizations)
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="username"></param>
        /// <param name="password"></param>
        public void GetLoginCredentialsWithinSameGroup(int userid, int organizationId, out string username, out string password)
        {
            DB.Organization dbOrg = new DB.Organization(sqlExec);
            dbOrg.GetLoginCredentialsWithinSameGroup(userid, organizationId, out username, out password);
        }
        #endregion

        /// <summary>
        /// Gets all vehicles active assignment configuration information for current organization
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns>DataSet [Description],[BoxId],[FwId],[FwName],[FwDateReleased],[CommModeId],[BoxProtocolTypeId],[FwTypeId]</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public DataSet GetOrganizationAllActiveVehiclesCfgInfo(int organizationId)
        {
            DB.VehicleInfo vehicleInfo = new DB.VehicleInfo(sqlExec);
            // 1. Retrieves all vehicle assignments
            DataSet dsResult = vehicleInfo.GetOrganizationAllActiveVehiclesCfgInfo(organizationId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "GetOrganizationAllActiveVehiclesCfgInfo";
                }
                dsResult.DataSetName = "Vehicle";
            }
            return dsResult;
        }

        /// <summary>
        /// Get DCL Information based on Organization and vehicle description
        /// </summary>
        /// <returns>boxId, dclID</returns>
        /// <exception cref="DASDbConnectionClosed">Thrown if connection to database has been closed.</exception>
        /// <exception cref="DASException">Thrown in all other exception cases.</exception>
        public void GetDclCommInfo(int orgID, string vehDesc, ref int boxID, ref short dclID)
        {
            DB.Organization dbOrg = new DB.Organization(sqlExec);
            // 1. Retrieves all vehicle assignments
            DataSet dsResult = dbOrg.GetDclCommInfo(orgID, vehDesc);
            if (dsResult != null)
            {
                if ((dsResult.Tables.Count > 0) && (dsResult.Tables[0].Rows.Count > 0))
                {
                    boxID = Convert.ToInt32(dsResult.Tables[0].Rows[0]["BoxID"]);
                    dclID = Convert.ToInt16(dsResult.Tables[0].Rows[0]["dclID"]);
                }
            }
        }

        /// <summary>
        /// Get Audit Info
        /// </summary>
        /// <returns></returns>
        public DataSet GetAuditGroupInfo()
        {
            return this.organization.GetAuditGroupInfo();
        }


        /// <summary>
        /// Gets organization landmarks within latlong rectangle
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="topleftlat"></param>
        /// <param name="topleftlong"></param>
        /// <param name="bottomrightlat"></param>
        /// <param name="bottomrightlong"></param>
        /// <returns></returns>
        public DataSet GetOrganizationLandmarksWithBoundary(int organizationId, double topleftlat, double topleftlong, double bottomrightlat, double bottomrightlong)
        {
            return this.organization.GetOrganizationLandmarksWithBoundary(organizationId, topleftlat, topleftlong, bottomrightlat, bottomrightlong);
        }

        /// <summary>
        /// Get Organization Geozones with Status
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns></returns>
        public DataSet GetOrganizationGeozonesWithStatus(int organizationId)
        {
            return this.organizationGeozone.GetOrganizationGeozonesWithStatus(organizationId);
        }


        ///// <summary>
        ///// Update Organization Preferences
        ///// </summary>
        ///// <param name="organizationId"></param>
        ///// <param name="NotificationEmailAddress"></param>
        ///// <param name="RadiusForGPS"></param>
        ///// <param name="MaximumReportingInterval"></param>
        ///// <param name="HistoryTimeRange"></param>
        ///// <param name="WaitingPeriodToGetMessages"></param>
        ///// <param name="Timezone"></param>
        //public void UpdateOrganizationPreferences(int organizationId, string NotificationEmailAddress, int RadiusForGPS, int MaximumReportingInterval, int HistoryTimeRange, int WaitingPeriodToGetMessages, int Timezone)
        //{
        //    organization.UpdateOrganizationPreferences(organizationId, NotificationEmailAddress, RadiusForGPS, MaximumReportingInterval, HistoryTimeRange, WaitingPeriodToGetMessages, Timezone);
        //}

        ///// <summary>
        ///// Get Organization Preferences
        ///// </summary>
        ///// <param name="organizationId"></param>
        ///// <returns></returns>
        //public DataSet GetOrganizationPreferences(int organizationId)
        //{
        //    return organization.GetOrganizationPreferences(organizationId);
        //}


        /// <summary>
        /// Get organization skills
        /// </summary>
        /// <param name="organizationId"></param>
        /// <returns>DataSet</returns>
        public DataSet GetOrganizationSkills(int organizationId)
        {
            return this.organization.GetOrganizationSkills(organizationId);
        }

        /// <summary>
        /// Save, Update, delete the organization skills
        /// </summary>
        /// <param name="organizationId"></param>
        /// <param name="skillName"></param>
        /// <param name="description"></param>
        /// <param name="skillId"></param>        
        /// <param name="delete"></param>
        /// <returns>bool</returns>
        public int SaveOrganizationSkill(int organizationId, string skillName, string description = null, int skillId = 0, int delete = 0)
        {
            var dbOrg = new DB.Organization(sqlExec);
            try
            {
                DataSet dbResult = dbOrg.SaveSkill(organizationId, skillName, description, skillId, delete);
                int id = Convert.ToInt32(dbResult.Tables[0].Rows[0]["SkillId"]);
                if (id > 0)
                {
                    return id;
                }
                return 0;
            }
            catch (Exception exception)
            {

            }
            return 0;

        }

        public Int16 OrganizationIP_Validate(int organizationId, string ip)
        {
            Int16 validIp = 0;
            DataSet dbResult = organization.OrganizationIP_Validate(organizationId, ip);

            if (dbResult != null && dbResult.Tables.Count > 0 && dbResult.Tables[0].Rows.Count > 0)
                validIp = Convert.ToInt16(dbResult.Tables[0].Rows[0]["validIp"]);  
     
            return validIp;
        }


        //SALMAN Feb 13,2013
        /// <summary>
        /// Returns all users info assigned to the organization. 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="organizationId"></param>
        /// <returns>DataSet [UserId],[UserName] (FirstName + LastName)</returns>
        /// <exception cref="DASException">Thrown DASException in all exception cases.</exception>
        public DataSet GetUsersNameInfoByOrganization(int userId, int organizationId)
        {
            DataSet dsResult = organization.GetUsersNameInfoByOrganization(userId, organizationId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "UsersNameInformation";
                }
                dsResult.DataSetName = "UsersName";
            }
            return dsResult;
        }

        #region Organization Services

        /// <summary>
        /// Retrieves List of organization Services.
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns>DataSet</returns>
        public DataSet GetOrganizationServices(int OrganizationId, int ServiceID, bool IsBillable)
        {
            DataSet dsResult = organization.GetOrganizationServices(OrganizationId, ServiceID, IsBillable);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "OrganizationServices";
                }
                dsResult.DataSetName = "OrganizationServices";
            }
            return dsResult;
        }

        /// <summary>
        /// Retrieves List of organization Services for add
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns>DataSet</returns>
        public DataSet GetOrganizationServicesForAdd(int OrganizationId)
        {
            DataSet dsResult = organization.GetOrganizationServicesForAdd(OrganizationId);
            if (dsResult != null)
            {
                if (dsResult.Tables.Count > 0)
                {
                    dsResult.Tables[0].TableName = "OrganizationServices";
                }
                dsResult.DataSetName = "OrganizationServices";
            }
            return dsResult;
        }

        /// <summary>
        /// Enables organization Service
        /// </summary>
        /// <param name="OrganizationID"></param>
        /// <param name="ServiceID"></param>
        /// <returns>OrganizationServiceID</returns>
        public int AddOrganizationService(int OrganizationID, int ServiceID, int UserID)
        {
            return organization.AddOrganizationService(OrganizationID, ServiceID, UserID);
        }

        /// <summary>
        /// Disables organization Service
        /// </summary>
        /// <param name="OrganizationServiceID"></param>
        /// <returns>rows affected</returns>
        public int DeleteOrganizationService(int OrganizationServiceID, int UserID)
        {
            return organization.DeleteOrganizationService(OrganizationServiceID, UserID);
        }

        /// <summary>
        /// Adds default User Group to an Organization
        /// </summary>
        /// <param name="OrganizationId"></param>
        /// <returns>rows affected</returns>
        public int AddUserGroupsDefault(int OrganizationId)
        {
            return organization.AddUserGroupsDefault(OrganizationId);
        }
        #endregion

        public DataSet GetAllSensorByOrganizationId(int organizationId)
        {
            return organization.GetAllSensorByOrganizationId(organizationId);
        }
    }
}
